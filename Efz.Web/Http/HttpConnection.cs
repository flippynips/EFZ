using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;

using Efz.Threading;
using Efz.Tools;
using Efz.Web.Display;

namespace Efz.Web {
  
  /// <summary>
  /// Represents a client communicating with a web server.
  /// </summary>
  public class HttpConnection : IDisposable, IEquatable<HttpConnection> {
    
    //----------------------------------//
    
    /// <summary>
    /// Client this connection belongs to.
    /// </summary>
    public HttpClient Client {
      get {
        return _client;
      }
      set {
        _lock.Take();
        if(_client != null) _client.RemoveConnection(this);
        _client = value;
        if(_client != null) _client.AddConnection(this);
        _lock.Release();
      }
    }
    /// <summary>
    /// Web server instance this connection belongs to.
    /// </summary>
    public readonly HttpServer Server;
    
    /// <summary>
    /// Tcp client instance, this web client represents.
    /// </summary>
    public readonly TcpClient TcpClient;
    /// <summary>
    /// The client socket instance.
    /// </summary>
    public readonly AsyncTcpSocket Socket;
    /// <summary>
    /// Collection of headers that will be sent before any more data.
    /// </summary>
    public readonly HttpResponseHeaders Headers;
    
    /// <summary>
    /// Get the remote address for the web client.
    /// </summary>
    public IPEndPoint RemoteEndpoint;
    
    //----------------------------------//
    
    /// <summary>
    /// Inner client reference.
    /// </summary>
    protected HttpClient _client;
    
    /// <summary>
    /// Last compression used for the data sent to the client.
    /// </summary>
    protected DecompressionMethods _compression;
    /// <summary>
    /// Default compression option.
    /// </summary>
    protected DecompressionMethods _defaultCompression;
    
    /// <summary>
    /// On a request successfully sent.
    /// </summary>
    protected IAction _onSent;
    /// <summary>
    /// Encoder used to retrieve a byte representation of
    /// strings to be sent to the client.
    /// </summary>
    protected Encoder _encoder;
    /// <summary>
    /// Current web request.
    /// </summary>
    protected HttpRequest _request;
    /// <summary>
    /// Has the client been disposed of?
    /// </summary>
    protected bool _disposed;
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
    protected const long _timeoutMilliseconds = Time.Minute * 30;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct a new web context for the specified alchemy user context.
    /// </summary>
    public HttpConnection(HttpServer server, TcpClient tcpClient) {
      
      // persist the server
      Server = server;
      // persist the tcp client
      TcpClient = tcpClient;
      
      try {
        // persist the socket
        Socket = new AsyncTcpSocket(TcpClient.Client, OnReceived, OnSocketError);
        RemoteEndpoint = Socket.RemoteEndPoint as IPEndPoint;
      } catch {
        var timer = new Timer(100, Dispose);
        return;
      }
      
      _timeout = new Timer(_timeoutMilliseconds, Dispose);
      
      // initialize the collection of server headers
      Headers = new HttpResponseHeaders(true);
      
      _defaultCompression = DecompressionMethods.GZip;
      SetCompression(_defaultCompression);
      
      // create the encoder
      _encoder = Encoding.UTF8.GetEncoder();
      _request = new HttpRequest(this);
      _lock = new Lock();
      
      // create a web client instance
      _client = new HttpClient(Server);
      // add this connection to the client
      _client.AddConnection(this);
      
      Socket.Start();
      
      Log.Info("New connection '"+this+"'.");
      
    }
    
    /// <summary>
    /// Close this connection and remove it from the client connection pool.
    /// </summary>
    public void Dispose() {
      
      _lock.Take();
      try {
        if(_disposed) return;
        
        _disposed = true;
        
        Log.Info("Disposing of connection '"+this+"'.");
        
        // dispose of the socket
        if(Socket != null) Socket.Dispose();
        // ensure the client connection has been closed
        if(TcpClient != null) TcpClient.Close();
        
        // remove the client from the server collection
        if(_client != null) _client.RemoveConnection(this);
        
        if(_request != null) _request.Dispose();
        
      } finally {
        
        _lock.Release();
        
      }
      
    }
    
    /// <summary>
    /// Redirect to the specified path.
    /// </summary>
    public void RedirectTo(string path) {
      
      Log.Info("Redirecting client "+this+" to path '"+path+"'.");
      
      _lock.Take();
      
      Headers.SetSingle(HttpResponseHeader.Location, path);
      Headers[HttpResponseHeader.ContentLength] = "0";
      Headers.StatusCode = 303;
      
      if(_disposed) return;
      
      Headers.SetSingle(HttpResponseHeader.ContentEncoding, null);
      Headers.Remove(HttpResponseHeader.ContentType);
      
      // send the header
      Socket.Send(Headers.Bytes);
      
      _lock.Release();
    }
    
    /// <summary>
    /// Send an unchanged cache message to the client.
    /// </summary>
    public void SendCacheUnchanged(long age) {
      
      Log.Info("Sending cache unchanged with age '"+age+"' to client "+this+".");
      
      _lock.Take();
      
      Headers.StatusCode = 304;
      Headers[HttpResponseHeader.ContentLength] = "0";
      Headers.SetSingle(HttpResponseHeader.CacheControl, "private, max-age=" + age);
      
      Headers.Remove(HttpResponseHeader.ContentEncoding);
      Headers.Remove(HttpResponseHeader.ContentType);
      
      
      if(_disposed) {
        _lock.Release();
        return;
      }
      
      // send the header
      Socket.Send(Headers.Bytes);
      
      _lock.Release();
    }
    
    /// <summary>
    /// Send only the headers with 0 content length.
    /// </summary>
    public void Send(HttpSendOptions options) {
      _lock.Take();
      // process the send options
      ProcessOptions(options);
      SendHeaders(0);
      _lock.Release();
    }
    
    /// <summary>
    /// Send the specified element to the client.
    /// </summary>
    public unsafe void Send(Element element, bool buildStyle = true, HttpSendOptions options = null, IAction onSent = null) {
      
      Log.Info("Sending element '"+element+"' to client "+this+".");
      
      if(buildStyle) {
        // build the style
        var style = element.FindChild("style");
        style.ContentString = element.BuildCss();
        element.EncodeContent = false;
      }
      
      // build the element into a string
      string str =
        "<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\">" +
        element.Build();
      
      _lock.Take();
      
      // have the options been specified? yes, process them
      if(options == null || options.ContentType == null) {
        Headers[HttpResponseHeader.ContentType] = "text/html; charset=UTF-8";
      }
      ProcessOptions(options);
      
      _onSent = onSent;
      
      // resize buffer as required
      var buffer = BufferCache.Get(str.Length + str.Length);
      
      // get pointers to the string and the send byte buffer
      int count;
      fixed(char* src = str)
      fixed(byte* dst = &buffer[0]) {
        // get the bytes that the string represents
        count = _encoder.GetBytes(src, str.Length, dst, buffer.Length, true);
      }
      
      // send the bytes
      SendBytes(buffer, 0, count);
      
      BufferCache.Set(buffer);
      
    }
    
    /// <summary>
    /// Send the specified string to the client.
    /// </summary>
    public unsafe void Send(string str, HttpSendOptions options = null, IAction onSent = null) {
      
      Log.Info("Sending string '"+str+"' to client "+this+".");
      
      // is the string empty? skip
      if(string.IsNullOrEmpty(str)) return;
      
      _lock.Take();
      
      // process send options
      ProcessOptions(options);
      
      _onSent = onSent;
      
      var buffer = BufferCache.Get(str.Length + str.Length);
      int count;
      // get pointers to the string and the send byte buffer
      fixed(char* src = str)
      fixed(byte* dst = &buffer[0]) {
        
        // get the bytes that the string represents
        count = _encoder.GetBytes(src, str.Length, dst, buffer.Length, true);
        
      }
      
      SendBytes(buffer, 0, count);
      
      BufferCache.Set(buffer);
      
    }
    
    /// <summary>
    /// Send the specified byte collection to the client. This may take a number of buffers to complete
    /// and therefore not be synchronous.
    /// </summary>
    public void Send(byte[] buffer, int offset, int count, HttpSendOptions options = null, IAction onSent = null) {
      
      _lock.Take();
      
      // process the options
      ProcessOptions(options);
      
      _onSent = onSent;
      
      // perform the send
      SendBytes(buffer, offset, count);
      
    }
    
    /// <summary>
    /// Send the specified stream, optionally with a byte length. This method is
    /// asynchronous and will close the stream on completion.
    /// </summary>
    public void Send(Stream stream, int length = -1, HttpSendOptions options = null, IAction onSent = null) {
      
      // start sending the stream
      StartSendStream(stream, length == -1 ? (int)(stream.Length - stream.Position) : length, options, onSent);
      
    }
    
    /// <summary>
    /// Send the specified web resource.
    /// </summary>
    public void Send(WebResource resource, HttpSendOptions options = null, IAction onSent = null) {
      
      Log.Info("Sending web resource "+resource+" bytes to client "+this+".");
      
      // get the resource stream then prepare to send the resource stream data
      resource.GetStream(ActionSet.New(SendResource, (Stream)null, (Lock)null, resource, options, onSent));
      
    }
    
    /// <summary>
    /// Send the file at the specified local path.
    /// </summary>
    public void SendLocal(string path, HttpSendOptions options = null, IAction onSent = null) {
      
      Log.Info("Sending local file '"+path+"' bytes to client "+this+".");
      
      // get the extension from the path
      string extension;
      int extensionIndex = path.LastIndexOf(Chars.Stop);
      if(extensionIndex == -1) extension = null;
      else extension = path.Substring(extensionIndex);
      
      if(options == null) options = new HttpSendOptions{ContentType = Mime.GetType(extension)};
      
      // open a file stream to the resource
      var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
      
      // start sending the file
      StartSendStream(fileStream, (int)fileStream.Length, options, onSent);
      
    }
    
    /// <summary>
    /// Equality comparison between web clients.
    /// </summary>
    public bool Equals(HttpConnection other) {
      return RemoteEndpoint == other.RemoteEndpoint;
    }
    
    /// <summary>
    /// Get a string representation of the web client.
    /// </summary>
    public override string ToString() {
      return RemoteEndpoint.Address.ToString()+Chars.Colon+RemoteEndpoint.Port;
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Process the send options into headers for a following send operation.
    /// </summary>
    protected void ProcessOptions(HttpSendOptions options) {
      
      if(options == null) {
        SetCompression(_defaultCompression);
        return;
      }
      
      // was the content type specified? set the content type
      if(options.ContentType != null) {
        Headers.SetSingle(HttpResponseHeader.ContentType, options.ContentType);
      }
      
      // is the data to be cached?
      if(options.CacheTime.HasValue) {
        if(options.CacheTime.Value == 0) Headers.SetSingle(HttpResponseHeader.CacheControl, "no-cache, no-store");
        else Headers.SetSingle(HttpResponseHeader.CacheControl, "private, max-age=" + options.CacheTime);
      }
      
      // assign the status code if different
      if(options.StatusCode != Headers.StatusCode) Headers.StatusCode = options.StatusCode;
      
      SetCompression(options.Compression);
      
    }
    
    /// <summary>
    /// Send the current header with the specified content length.
    /// </summary>
    protected void SendHeaders(int contentLength) {
      
      if(_disposed) return;
      
      // set the content length
      Headers.Set(HttpResponseHeader.ContentLength, contentLength.ToString());
      
      // send the header
      Socket.Send(Headers.Bytes);
      
    }
    
    /// <summary>
    /// Begin sending the stream of a web resource and prepare to release the resource lock on completion.
    /// </summary>
    protected void SendResource(Stream stream, Lock resourceLock, WebResource resource, HttpSendOptions options, IAction onSent) {
      
      // was the stream retrieved?
      if(stream == null) {
        // no, unlock the resource
        resourceLock.Release();
        
        // callback
        _client.OnErrorRoll.Run(new Exception("A resource stream '"+resource.FullPath+"' was unable to be resolved."));
        return;
      }
      
      _lock.Take();
      
      if(onSent == null) _onSent = new ActionSet(resourceLock.Release);
      else _onSent = new ActionPair(resourceLock.Release, onSent);
      
      // set the content type
      ProcessOptions(options);
      if(options == null) Headers[HttpResponseHeader.ContentType] = resource.MimeType;
      
      int length = resource.Size == -1 ? (int)stream.Length : (int)resource.Size;
      
      // start sending the stream
      StartSendStream(stream, length, resource);
    }
    
    /// <summary>
    /// Start sending the stream. The client is locked until the stream is complete.
    /// </summary>
    protected void StartSendStream(Stream stream, int length, WebResource resource) {
      
      // yes, does the buffer need to be compressed?
      if(_defaultCompression == resource.Compression) {
        
        // send the headers with the stream length
        SendHeaders((int)stream.Length);
        
        Log.Info("Sending stream of length '"+length+"' to client "+this+".");
        
        // no, the resource compression matches the required compression
        
        // send the stream
        Socket.Send(stream, length);
        
        // is the callback set?
        if(_onSent != null) {
          // yes, run it
          var onSent = _onSent;
          _onSent = null;
          _lock.Release();
          onSent.Run();
        } else {
          // no, release the lock
          _lock.Release();
        }
        
      } else {
        
        // is the web resource compressed?
        if(resource.Compression != DecompressionMethods.None) {
          // yes, decompress the resource and replace the web resource stream
          resource.ReplaceStreamLocked(StreamHelper.Decompress(resource.Compression, stream, ref length), DecompressionMethods.None);
        }
        
        // yes, compress the bytes
        StreamHelper.Compress(Act.New(StartSendStream, (MemoryStream)null, resource), _defaultCompression,
          System.IO.Compression.CompressionLevel.Fastest, stream, length, false);
        
      }
    }
    
    /// <summary>
    /// Start sending the stream. The client is locked until the stream is complete.
    /// </summary>
    protected void StartSendStream(Stream stream, int length, HttpSendOptions options = null, IAction onSent = null) {
      
      // does the buffer need to be compressed?
      if(options != null && options.Compression == DecompressionMethods.None ||
        _defaultCompression == DecompressionMethods.None) {
        
        // no, take the send lock
        _lock.Take();
        
        // process the options
        ProcessOptions(options);
        
        // send the header
        SendHeaders(length);
        
        Log.Info("Sending stream of length '"+length+"' to client "+this+".");
        
        // send the stream
        Socket.Send(stream, length);
        
        _lock.Release();
        
        // is the callback set? yes, run it
        if(onSent != null) onSent.Run();
        
      } else {
        
        // yes, compress the bytes
        StreamHelper.Compress(new Act<MemoryStream, HttpSendOptions, IAction>(StartSendStream, null, options, onSent),
          options == null ? _defaultCompression : options.Compression,
          System.IO.Compression.CompressionLevel.Fastest, stream, length, false);
        
      }
    }
    
    /// <summary>
    /// On the stream being compressed.
    /// </summary>
    protected void StartSendStream(MemoryStream stream, HttpSendOptions options, IAction onSent) {
      
      _lock.Take();
      
      // process the options
      ProcessOptions(options);
      
      // send the headers
      SendHeaders((int)stream.Length);
      
      Log.Info("Sending stream of length '"+(int)stream.Length+"' to client "+this+".");
      
      // send the stream
      Socket.Send(stream, (int)stream.Length);
      
      _lock.Release();
      
      // is the callback set? yes, run it
      if(onSent != null) onSent.Run();
      
      // dispose of the memory stream
      stream.Dispose();
      
    }
    
    /// <summary>
    /// On the stream being compressed.
    /// </summary>
    protected void StartSendStream(MemoryStream stream, WebResource resource) {
      
      // replace the resource stream with the compressed bytes
      resource.ReplaceStreamLocked(stream, _compression);
      
      SendHeaders((int)stream.Length);
      
      Log.Info("Sending stream of length '"+stream.Length+"' to client "+this+".");
      
      // send the stream
      Socket.Send(stream, (int)stream.Length);
      
      stream.Dispose();
      
      // is the callback set?
      if(_onSent != null) {
        // yes, run it
        var onSent = _onSent;
        _onSent = null;
        _lock.Release();
        onSent.Run();
      } else {
        // no, release the lock
        _lock.Release();
      }
      
    }
    
    /// <summary>
    /// Send a buffer of bytes to the client. Releases the lock.
    /// </summary>
    protected void SendBytes(byte[] bytes, int offset, int count) {
      
      // yes, does the buffer need to be compressed?
      if(_compression != DecompressionMethods.None) {
        // yes, compress the bytes
        bytes = StreamHelper.Compress(_compression, System.IO.Compression.CompressionLevel.Optimal, bytes, offset, ref count);
        offset = 0;
      }
      
      // send headers with the content length
      SendHeaders(count);
      
      // send the bytes
      Socket.Send(bytes, offset, count);
      
      _lock.Release();
      
    }
    
    /// <summary>
    /// Receive data available in the receive args.
    /// </summary>
    protected void OnReceived(byte[] bytes, int count) {
      
      // should the client receive?
      if(_disposed) {
        // no, pass the bytes
        BufferCache.Set(bytes);
        // skip
        return;
      }
      
      _timeout.Reset(_timeoutMilliseconds);
      
      int index = 0;
      
      // while the bytes received haven't been completely parsed
      while(count != 0) {
        
        // try passing the received bytes to the current request
        int added = _request.TryAdd(bytes, index, count);
        
        // has the request been completed?
        if(_request.Complete) {
          
          // determine the compressesion to use based off of the accept encoding
          // headers received
          DetermineCompression();
          
          // log the new request completion
          Log.Info("New request "+_request+".");
          
          Client.AddRequest(_request);
          
          // construct a new web request
          _request = new HttpRequest(this);
          
        } else if(added == -1) {
          
          // no, there was an error, possibly with the format of the request
          Log.Warning("Error adding '"+count+"' bytes to request '"+_request+"'.");
          
          // callback with the error
          if(_client.OnErrorRoll.Action != null) {
            _client.OnErrorRoll.Run(new Exception(_request.ErrorMessage));
          }
          
          // re-allocate the buffer
          BufferCache.Set(bytes);
          
          // dispose of the client
          Dispose();
          
          return;
        }
        
        // increment the index
        index += added;
        // decrement the number of bytes received that are still to be processed
        count -= added;
      }
      
      // re-allocate the buffer
      BufferCache.Set(bytes);
      
    }
    
    /// <summary>
    /// On an error with the socket.
    /// </summary>
    protected void OnSocketError(Exception ex) {
      Log.Warning("Socket errror : " + ex.Message);
      _client.OnErrorRoll.Run(ex);
    }
    
    /// <summary>
    /// Set the compression for the next request.
    /// </summary>
    protected void SetCompression(DecompressionMethods compression) {
      
      if(compression == _compression) return;
      
      _compression = compression;
      // set the encoding header
      switch(_compression) {
        case DecompressionMethods.Deflate:
          Headers[HttpResponseHeader.ContentEncoding] = "deflate";
          break;
        case DecompressionMethods.GZip:
          Headers[HttpResponseHeader.ContentEncoding] = "gzip";
          break;
        default:
          Headers[HttpResponseHeader.ContentEncoding] = "charset=UTF-8";
          break;
      }
      
    }
    
    /// <summary>
    /// Determine the available compression to be used with the next response.
    /// </summary>
    protected void DetermineCompression() {
      
      // determine the compressesion to use based off of the headers received
      string compressesion = _request.Headers[HttpRequestHeader.AcceptEncoding];
      if(string.IsNullOrEmpty(compressesion)) {
        _defaultCompression = DecompressionMethods.None;
        return;
      }
      
      // iterate the compression specifications
      foreach(var compressionValue in compressesion.Split(Chars.Comma)) {
        string[] compressesionParams = compressionValue.Split(Chars.SemiColon);
        switch(compressesionParams[0].TrimSpace()) {
          case "gzip":
            if(compressesionParams.Length == 1) {
              // preferred
              _defaultCompression = DecompressionMethods.GZip;
              return;
            }
            if(compressesionParams.Length == 2 && compressesionParams[1].Equals("q=1", StringComparison.OrdinalIgnoreCase)) {
              _defaultCompression = DecompressionMethods.GZip;
              return;
            }
            break;
          case "deflate":
            if(compressesionParams.Length == 1) {
              _defaultCompression = DecompressionMethods.Deflate;
            }
            if(compressesionParams.Length == 2 && compressesionParams[1].Equals("q=1", StringComparison.OrdinalIgnoreCase)) {
              _defaultCompression = DecompressionMethods.Deflate;
            }
            break;
          case "identity":
            _defaultCompression = DecompressionMethods.None;
            return;
        }
      }
    }
    
  }

}

