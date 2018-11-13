using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Efz.Collections;
using Efz.Data;
using Efz.Threading;

namespace Efz.Web {

  /// <summary>
  /// Handler of a UDP server.
  /// </summary>
  public class UdpServer : IDisposable {
    
    //----------------------------------//
    
    /// <summary>
    /// Local endpoint for the server.
    /// </summary>
    public readonly IPEndPoint LocalEndpoint;
    
    /// <summary>
    /// Get the name of the server.
    /// </summary>
    public string Name { get { return _name; } }
    
    /// <summary>
    /// On new client callback.
    /// </summary>
    public IAction<UdpConnection> OnConnection {
      get { return _onConnection.Action; }
      set { _onConnection.Action = value; }
    }
    
    /// <summary>
    /// Server socket wrapper.
    /// </summary>
    public AsyncUdpSocket AsyncSocket;
    
    //----------------------------------//
    
    /// <summary>
    /// Has the server been disposed?
    /// </summary>
    protected bool _stopped;
    
    /// <summary>
    /// Inner name of the server.
    /// </summary>
    protected string _name;
    /// <summary>
    /// Configuration instance.
    /// </summary>
    protected Configuration _config;
    /// <summary>
    /// On connection callback.
    /// </summary>
    protected ActionPop<UdpConnection> _onConnection;
    
    /// <summary>
    /// Current clients accepting connections.
    /// </summary>
    protected Shared<Dictionary<IPEndPoint, UdpConnection>> _connections;
    
    /// <summary>
    /// Server socket.
    /// </summary>
    protected Socket _socket;
    
    /// <summary>
    /// Socket flags used to receive data.
    /// </summary>
    protected SocketFlags _flags;
    
    //----------------------------------//
    
    /// <summary>
    /// Create a new server listening to the specified endpoint.
    /// </summary>
    public UdpServer(IPEndPoint localEndpoint, string name = "Efz") {
      _connections = new Shared<Dictionary<IPEndPoint, UdpConnection>>(new Dictionary<IPEndPoint, UdpConnection>());
      _onConnection = new ActionPop<UdpConnection>();
      
      _name = name;
      _stopped = true;
      _flags = SocketFlags.None;
      
      LocalEndpoint = localEndpoint;
      
      // create the server socket
      _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      AsyncSocket = new AsyncUdpSocket(_socket, LocalEndpoint, OnReceive, OnError);
      
    }
    
    /// <summary>
    /// Initialize a UDP server.
    /// </summary>
    public UdpServer(Configuration config, Action<UdpConnection> onConnection) :
      this(config, new ActionPop<UdpConnection>(onConnection)) {
    }
    
    /// <summary>
    /// Construct and start a new UDP server.
    /// </summary>
    public UdpServer(Configuration config, IAction<UdpConnection> onConnection) :
      this(new IPEndPoint(IPAddress.Parse(config["Address"].String) ?? IPAddress.Any,
        config["Port"].Int32), config.GetString("Name", "Efz")) {
      
      // persist the configuration reference
      _config = config;
      _onConnection.Action = onConnection;
      
    }
    
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public virtual void Dispose() {
      if(_stopped) return;
      _stopped = true;
      
      try {
        
        // get a collection of the current connections
        var connections = new ArrayRig<UdpConnection>();
        foreach(var entry in _connections.TakeItem()) connections.Add(entry.Value);
        _connections.Release();
        
        // dispose of each connection
        foreach(var connection in connections) connection.Dispose();
        
        // dispose of the listenning socket
        AsyncSocket.Dispose();
        _socket.Dispose();
        
      } catch(Exception ex) {
        
        Log.Error("Exception disposing of UDP server connections.", ex);
        
      }
      
    }
    
    /// <summary>
    /// Start listenning from the endpoint.
    /// </summary>
    public void StartReceiving() {
      if(!_stopped) return;
      
      try {
        
        AsyncSocket.StartReceiving();
        _stopped = false;
        
      } catch(Exception ex) {
        Log.Error("Exception starting a UDP server.", ex);
      }
      
    }
    
    /// <summary>
    /// Get a udp connection to the specified endpoint.
    /// </summary>
    public UdpConnection GetConnection(IPEndPoint remoteEndpoint) {
      
      // has the server been stopped? yes, throw
      if(_stopped) throw new InvalidOperationException("The server is stopped.");
      
      // try get an existing connection for the specified endpoint
      UdpConnection connection;
      if(_connections.TakeItem().TryGetValue(remoteEndpoint, out connection)) {
        _connections.Release();
        return connection;
      }
      
      try {
        
        // create the new connection
        connection = new UdpConnection(this, _socket, remoteEndpoint);
        connection.OnError = new ActionSet<Exception, UdpConnection>(OnConnectionError, (Exception)null, connection);
        
      } catch(Exception ex) {
        
        Log.Error("Exception creating connection from '"+_socket.LocalEndPoint+"' to '"+remoteEndpoint+"'.", ex);
        return null;
        
      }
      
      // add the new connection to the collection
      _connections.Item.Add(remoteEndpoint, connection);
      _connections.Release();
      
      // run callback on connection
      _onConnection.Run(connection);
      
      // return the new connnection
      return connection;
      
    }
    
    /// <summary>
    /// Remove a connection from the servers collection.
    /// </summary>
    internal void RemoveConnection(UdpConnection connection) {
      _connections.TakeItem().Remove(connection.RemoteEndPoint);
      _connections.Release();
    }
    
    //----------------------------------//
    
    /// <summary>
    /// On an exception with the socket, or data transmission.
    /// </summary>
    protected void OnError(Exception ex) {
      Log.Error("Udp Server error.", ex);
    }
    
    /// <summary>
    /// On a connection error. This is the default callback for new connections created
    /// by this server.
    /// </summary>
    protected void OnConnectionError(Exception ex, UdpConnection connection) {
      
      // log the error
      Log.Error("Connection error '"+ex+"'.");
      
      // dispose of the connection
      connection.Dispose();
      
    }
    
    /// <summary>
    /// On a client being accepted by the listenner.
    /// </summary>
    private void OnReceive(IPEndPoint endpoint, byte[] buffer, int count) {
      if(_stopped) return;
      
      UdpConnection connection;
      if(_connections.TakeItem().TryGetValue(endpoint, out connection)) {
        _connections.Release();
        
        // pass the buffer to the connection
        connection.OnReceived(endpoint, buffer, count);
        
        return;
      }
      
      try {
        
        // create a new connection
        connection = new UdpConnection(this, new IPEndPoint(IPAddress.Any, 0), endpoint);
        
      } catch(Exception ex) {
        
        Log.Error("Exception creating new incoming connection.", ex);
        
        // dispose of the connections
        _connections.Release();
        // dispose of the new connection
        if(connection != null) connection.Dispose();
        
        return;
        
      }
      
      // add the new connection
      _connections.Item.Add(connection.RemoteEndPoint, connection);
      _connections.Release();
      
      // run on connection callback
      _onConnection.Run(connection);
      
      // pass the received buffer to the connection
      connection.OnReceived(endpoint, buffer, count);
      
    }
    
  }
  
}
