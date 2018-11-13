using System;
using System.Net;
using System.Net.Sockets;
using Efz.Collections;
using Efz.Data;

namespace Efz.Web {

  /// <summary>
  /// Handler of a TCP server.
  /// </summary>
  public class HttpServer : IDisposable {
    
    //----------------------------------//
    
    /// <summary>
    /// The port being listenned to.
    /// </summary>
    public readonly int Port;
    /// <value>
    /// The listener address.
    /// </value>
    public readonly IPAddress ListenAddress;
    
    /// <summary>
    /// Get the name of the server.
    /// </summary>
    public string Name { get { return _name; } }
    
    /// <summary>
    /// Collection of connections.
    /// </summary>
    public readonly Capsule<HttpConnection> Connections;
    /// <summary>
    /// Collection of current clients.
    /// </summary>
    public readonly Capsule<HttpClient> Clients;
    
    /// <summary>
    /// Get or set the default client send timeout.
    /// </summary>
    public int SendTimeout;
    /// <summary>
    /// Get or set the strike limit for clients. Default is three.
    /// </summary>
    public int StrikeLimit = 3;
    
    //----------------------------------//
    
    /// <summary>
    /// Inner tcp listenner.
    /// </summary>
    protected TcpListener _listener;
    /// <summary>
    /// Has the server been disposed?
    /// </summary>
    protected bool _stopped;
    
    /// <summary>
    /// On new client action pop.
    /// </summary>
    protected readonly ActionPop<TcpClient> _onClient;
    
    /// <summary>
    /// Inner name of the server.
    /// </summary>
    protected string _name;
    /// <summary>
    /// Configuration instance.
    /// </summary>
    protected Configuration _config;
    /// <summary>
    /// Inner context action called when a new context is added.
    /// </summary>
    protected ActionPop<HttpConnection> _onConnection;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a web server by name. The name indicates the .
    /// </summary>
    public HttpServer(Configuration config, Action<HttpConnection> onConnection) :
      this(config, new ActionSet<HttpConnection>(onConnection)) {}
    
    /// <summary>
    /// Construct and start a new web server.
    /// </summary>
    public HttpServer(Configuration config, IAction<HttpConnection> onConnection) {
      
      // persist the configuration reference
      _config = config;
      
      // get the server name
      _name = _config.GetString("Name", "Efz");
      
      if(onConnection != null) _onConnection = new ActionPop<HttpConnection>(onConnection);
      
      if (config["Port"].Int32 > 0) Port = config["Port"].Int32;
      
      // create the clients collection
      Connections = new Capsule<HttpConnection>();
      Clients = new Capsule<HttpClient>();
      
      _name = "Efz";
      
      ListenAddress = IPAddress.Parse(config["Address"].String) ?? IPAddress.Any;
      _onClient = new ActionPop<TcpClient>(OnClient);
      _stopped = true;
      
    }
    
    /// <summary>
    /// Create a new tcp server.
    /// </summary>
    public HttpServer(int listenPort, IPAddress listenAddress) {
      if (listenPort > 0) Port = listenPort;
      
      // create the clients collection
      Connections = new Capsule<HttpConnection>();
      Clients = new Capsule<HttpClient>();
      
      _name = "Efz";
      
      ListenAddress = listenAddress ?? IPAddress.Any;
      _onClient = new ActionPop<TcpClient>(OnClient);
      _stopped = true;
    }
    
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public virtual void Dispose() {
      if(_stopped) return;
      _stopped = true;
      // iterate and dispose current clients
      foreach(var client in Connections.ToArray()) client.Dispose();
      try {
        _listener.Stop();
      } catch(Exception ex) {
        Log.Error("TcpListenner stop exception.", ex);
      }
    }
    
    /// <summary>
    /// Starts this instance.
    /// </summary>
    public virtual void Start() {
      if (!_stopped) return;
      try {
        _listener = new TcpListener(ListenAddress, Port);
        _listener.Start();
      } catch(Exception ex) {
        Log.Error("TcpListenner start exception.", ex);
        return;
      }
      _stopped = false;
      ManagerUpdate.Control.AddSingle(Listen);
    }
    
    /// <summary>
    /// Stops this instance.
    /// </summary>
    public virtual void Stop() {
      if(_stopped) return;
      try {
        _listener.Stop();
      } catch(Exception ex) {
        Log.Error("TcpListenner stop exception.", ex);
        return;
      }
      _stopped = true;
    }
    
    /// <summary>
    /// Restarts this instance.
    /// </summary>
    public virtual void Restart() {
      Stop();
      Start();
    }
    
    /// <summary>
    /// Remove the client from the collection of clients.
    /// </summary>
    internal void Remove(HttpClient client) {
      Clients.Remove(client);
    }
    
    /// <summary>
    /// Add a connection.
    /// </summary>
    internal void Add(HttpConnection connection) {
      Connections.Add(connection);
    }
    
    /// <summary>
    /// Remove a connection.
    /// </summary>
    internal void Remove(HttpConnection connection) {
      Connections.Remove(connection);
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Listens on the ip and port specified.
    /// </summary>
    private void Listen() {
      if(_stopped) return;
      try {
        _listener.BeginAcceptTcpClient(OnAcceptClient, null);
      } catch (SocketException ex) {
        // log
        Log.Error("Tcp server listen exception.", ex);
        ManagerUpdate.Control.AddSingle(Listen);
      }
    }
    
    /// <summary>
    /// On a client being accepted by the listenner.
    /// </summary>
    private void OnAcceptClient(IAsyncResult result) {
      if(_stopped) return;
      
      try {
        TcpClient client = _listener.EndAcceptTcpClient(result);
        _onClient.Run(client);
      } catch (Exception ex) {
        Log.Error("Accepting client exception.", ex);
      }
      
      ManagerUpdate.Control.AddSingle(Listen);
    }
    
    /// <summary>
    /// On a new tcp client connection.
    /// </summary>
    protected void OnClient(TcpClient tcpClient) {
      // are we still running? no, skip
      if(_stopped) return;
      
      // does the socket indicate connection?
      if(tcpClient.Connected) {
        
        // yes, create a new connection for the tcp client
        var connection = new HttpConnection(this, tcpClient);
        
        // add the connection
        Connections.Add(connection);
        
        // run the client callback
        if(_onConnection != null) _onConnection.Run(connection);
        
      } else {
        
        // dispose of the client
        tcpClient.Close();
      }
    }
    
  }
  
}
