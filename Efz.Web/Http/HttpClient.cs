using System;
using System.Text;
using System.Net.Sockets;

using Efz.Collections;
using Efz.Data;
using Efz.Threading;
using Efz.Tools;

namespace Efz.Web {
  
  /// <summary>
  /// Represents a client communicating with a web server.
  /// </summary>
  public class HttpClient : IDisposable, IEquatable<HttpClient> {
    
    //----------------------------------//
    
    /// <summary>
    /// Web server instance this client belongs to.
    /// </summary>
    public readonly HttpServer Server;
    /// <summary>
    /// Connections established by this client.
    /// </summary>
    public readonly Capsule<HttpConnection> Connections;
    
    /// <summary>
    /// Node containing attributes this web client is associated with.
    /// </summary>
    public Node Attributes;
    /// <summary>
    /// Time limit to apply to client requests if required. Default
    /// value is 'Null'.
    /// </summary>
    public TimeLimited RequestLimit;
    
    /// <summary>
    /// Get or set a client node.
    /// </summary>
    public Node this[string key] {
      get { return Attributes[key]; }
      set { Attributes[key] = value; }
    }
    
    /// <summary>
    /// Get or set a client node.
    /// </summary>
    public Node this[params string[] keys] {
      get { return Attributes[keys]; }
      set { Attributes[keys] = value; }
    }
    
    /// <summary>
    /// On bytes received from the client connection.
    /// Use only immediate actions. No task based action.
    /// </summary>
    public IAction<HttpRequest> OnReceive {
      get { return OnRequest.Action; }
      set {
        _lock.Take();
        OnRequest.Action = value;
        if(OnRequest.Action != null) {
          foreach(var request in _requests) OnRequest.Run(request);
          _requests.Clear();
          _lock.Release();
        } else _lock.Release();
      }
    }
    
    /// <summary>
    /// Action run on an error occuring with sending data to the client.
    /// Use only immediate actions. No task based actions.
    /// </summary>
    public IAction<Exception> OnError {
      get { return OnErrorRoll.Action; }
      set { OnErrorRoll.Action = value; }
    }
    
    /// <summary>
    /// On a new request from one of the client connections.
    /// </summary>
    internal ActionPop<HttpRequest> OnRequest;
    /// <summary>
    /// On an error occuring with sending to the client.
    /// </summary>
    internal ActionRoll<Exception> OnErrorRoll;
    
    //----------------------------------//
    
    /// <summary>
    /// Encoder used to retrieve a byte representation of
    /// strings to be sent to the client.
    /// </summary>
    protected Encoder _encoder;
    
    /// <summary>
    /// Has the client been disposed of?
    /// </summary>
    protected bool _disposed;
    /// <summary>
    /// A backlog of web requests.
    /// </summary>
    protected ArrayRig<HttpRequest> _requests;
    
    /// <summary>
    /// Inner unique id for this clients endpoint.
    /// </summary>
    protected string _id;
    
    /// <summary>
    /// Lock in order to ensure singular sending and receiving.
    /// </summary>
    protected Lock _lock;
    
    /// <summary>
    /// Timer used for inactivity.
    /// </summary>
    protected Timer _timeout;
    
    /// <summary>
    /// Number of milliseconds clients are.
    /// </summary>
    protected const long _timeoutMilliseconds = Time.Minute * 10;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct a new web context for the specified alchemy user context.
    /// </summary>
    public HttpClient(HttpServer server) {
      
      // persist the server
      Server = server;
      
      _timeout = new Timer(_timeoutMilliseconds, Dispose);
      
      OnRequest = new ActionPop<HttpRequest>();
      OnErrorRoll = new ActionRoll<Exception>();
      
      // create the encoder and decoder
      _encoder = Encoding.UTF8.GetEncoder();
      
      _requests = new ArrayRig<HttpRequest>();
      
      Connections = new Capsule<HttpConnection>();
      
      _lock = new Lock();
      
      Log.Info("New connection '"+this+"'.");
      
    }
    
    /// <summary>
    /// Close any existing connection with the client and disposes of any unmanaged
    /// resources.
    /// </summary>
    /// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Efz.Web.HttpWebClient"/>. The
    /// <see cref="Dispose"/> method leaves the <see cref="Efz.Web.HttpWebClient"/> in an unusable state. After calling
    /// <see cref="Dispose"/>, you must release all references to the <see cref="Efz.Web.HttpWebClient"/> so the garbage
    /// collector can reclaim the memory that the <see cref="Efz.Web.HttpWebClient"/> was occupying.</remarks>
    public void Dispose() {
      
      _lock.Take();
      if(_disposed) {
        _lock.Release();
        return;
      }
      _disposed = true;
      _lock.Release();
      
      if(Connections.Count > 0) {
        Log.Error("Disposing of client '"+this+"'.");
      }
      
      // remove the client from the server collection
      Server.Remove(this);
      
    }
    
    /// <summary>
    /// Add a new connection to this web client.
    /// </summary>
    internal virtual void AddConnection(TcpClient tcpClient) {
      Connections.Add(new HttpConnection(Server, tcpClient));
    }
    
    /// <summary>
    /// Adda a connection to this web client.
    /// </summary>
    internal virtual void AddConnection(HttpConnection connection) {
      Connections.Add(connection);
    }
    
    /// <summary>
    /// Remove a connection from the web client.
    /// </summary>
    internal virtual void RemoveConnection(HttpConnection connection) {
      Connections.Remove(connection);
      // have all the connections been removed?
      if(Connections.Count == 0) {
        // yes, dispose of the client
        Dispose();
      }
    }
    
    /// <summary>
    /// Add a request.
    /// </summary>
    internal void AddRequest(HttpRequest request) {
      _lock.Take();
      // yes, has the callback method been assigned?
      if(OnRequest.Action == null) {
        // no, add to the backlog of requests
        _requests.Add(request);
        _lock.Release();
      } else {
        // yes, run the callback immediately
        _lock.Release();
        OnRequest.Run(request);
      }
    }
    
    /// <summary>
    /// Equality for web clients.
    /// </summary>
    public bool Equals(HttpClient client) {
      return this == client;
    }
    
    /// <summary>
    /// Get a string representation of the web client.
    /// </summary>
    public override string ToString() {
      if(Connections.Count == 0) return "[WebClient No Connections]";
      var builder = StringBuilderCache.Get();
      builder.Append("[WebClient ");
      bool first = true;
      foreach(var connection in Connections) {
        if(first) first = false;
        else builder.Append(Chars.Comma);
        builder.Append(connection.RemoteEndpoint);
      }
      builder.Append(" ]");
      return StringBuilderCache.SetAndGet(builder);
    }
    
    //----------------------------------//
    
  }

}

