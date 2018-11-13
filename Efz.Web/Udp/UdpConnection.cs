using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;

using Efz.Collections;
using Efz.Threading;
using Efz.Tools;

namespace Efz.Web {
  
  /// <summary>
  /// Represents a client communicating with a web server.
  /// </summary>
  public class UdpConnection : IEquatable<UdpConnection> {
    
    //----------------------------------//
    
    /// <summary>
    /// Web server instance this connection belongs to.
    /// </summary>
    public readonly UdpServer Server;
    /// <summary>
    /// The client socket instance.
    /// </summary>
    public readonly AsyncUdpSocket AsyncSocket;
    /// <summary>
    /// Get the local endpoint if socket is bound to one.
    /// </summary>
    public IPEndPoint LocalEndPoint {
      get { return (IPEndPoint)AsyncSocket.Socket.LocalEndPoint; }
    }
    /// <summary>
    /// Get the remote address for the web client.
    /// </summary>
    public readonly IPEndPoint RemoteEndPoint;
    
    /// <summary>
    /// Number of milliseconds of innactivity before the connection is dropped.
    /// If less than 1, the connection doesn't time out.
    /// </summary>
    public long TimeoutMilliseconds {
      get {
        return _timeoutMilliseconds;
      }
      set {
        _timeoutMilliseconds = value;
        if(_timeoutMilliseconds <= 0) {
          if(_timeoutTimer == null) return;
          _timeoutTimer.Run = false;
          _timeoutTimer = null;
          return;
        }
        if(_timeoutTimer == null) {
          _timeoutTimer = new Timer(_timeoutMilliseconds, Dispose);
        } else {
          _timeoutTimer.Reset(_timeoutMilliseconds);
        }
      }
    }
    /// <summary>
    /// Callback on an exception within the connection.
    /// </summary>
    public IAction<Exception> OnError {
      get { return _onError.Action; }
      set {
        _lock.Take();
        _onError.Action = value;
        if(_onError.Action != null) {
          foreach(var request in _errors) _onError.Run(request);
          _errors.Clear();
        }
        _lock.Release();
      }
    }
    
    /// <summary>
    /// Callback on udp request.
    /// </summary>
    public IAction<UdpMessage> OnMessage {
      get { return _onMessage.Action; }
      set {
        _lock.Take();
        _onMessage.Action = value;
        if(_onMessage.Action != null) {
          foreach(var request in _messages) _onMessage.Run(request);
          _messages.Clear();
        }
        _lock.Release();
      }
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Default compression option.
    /// </summary>
    protected DecompressionMethods _defaultCompression;
    
    /// <summary>
    /// Current header information to be included with the next message.
    /// </summary>
    protected UdpHeader _header;
    
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
    protected Timer _timeoutTimer;
    /// <summary>
    /// Number of milliseconds clients are.
    /// </summary>
    protected long _timeoutMilliseconds = Time.Second * 10;
    
    /// <summary>
    /// Current request that received bytes will be added to.
    /// </summary>
    protected UdpMessage _message;
    /// <summary>
    /// Backlog of requests if the callback isn't assigned.
    /// </summary>
    protected ArrayRig<UdpMessage> _messages;
    /// <summary>
    /// Action pop callback on new requests.
    /// </summary>
    protected ActionPop<UdpMessage> _onMessage;
    
    /// <summary>
    /// Action pop callback on exceptions.
    /// </summary>
    protected ActionPop<Exception> _onError;
    /// <summary>
    /// Backlock of exceptions if the error callback isn't assigned.
    /// </summary>
    protected ArrayRig<Exception> _errors;
    
    //----------------------------------//
    
    /// <summary>
    /// Create a udp connection.
    /// </summary>
    public UdpConnection(UdpServer server, IPEndPoint localEndpoint, IPEndPoint remoteEndpoint) {
      
      Server = server;
      RemoteEndPoint = remoteEndpoint;
      
      _timeoutTimer = new Timer(_timeoutMilliseconds, ActionSet.New(ProcessError, new TimeoutException("UdpConnection timeout.")));
      
      _header = new UdpHeader();
      _header.Compression = _defaultCompression = DecompressionMethods.None;
      
      // create the encoder
      _encoder = Encoding.UTF8.GetEncoder();
      _lock = new Lock();
      _message = new UdpMessage(this);
      
      _messages = new ArrayRig<UdpMessage>();
      _onMessage = new ActionPop<UdpMessage>();
      
      _errors = new ArrayRig<Exception>();
      _onError = new ActionPop<Exception>();
      
      var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      
      AsyncSocket = new AsyncUdpSocket(socket, localEndpoint, OnReceived, ProcessError);
      AsyncSocket.TargetEndPoint = RemoteEndPoint;
      
    }
    
    /// <summary>
    /// Create a new incoming udp connection to handle communication to a single endpoint.
    /// </summary>
    public UdpConnection(UdpServer server, IPEndPoint remoteEndpoint, IPEndPoint localEndpoint, byte[] buffer, int count) {
      
      // persist the server
      Server = server;
      RemoteEndPoint = remoteEndpoint;
      
      _timeoutTimer = new Timer(_timeoutMilliseconds, ActionSet.New(ProcessError, new TimeoutException("UdpConnection timeout.")));
      
      _header = new UdpHeader();
      _header.Compression = _defaultCompression = DecompressionMethods.None;
      
      // create the encoder
      _encoder = Encoding.UTF8.GetEncoder();
      _lock = new Lock();
      _message = new UdpMessage(this);
      
      _messages = new ArrayRig<UdpMessage>();
      _onMessage = new ActionPop<UdpMessage>();
      
      _errors = new ArrayRig<Exception>();
      _onError = new ActionPop<Exception>();
      
      // call on received for the first buffer
      OnReceived(RemoteEndPoint, buffer, count);
      
      // create a socket instance for the udp connection
      var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      
      // create an async socket to manage the socket
      AsyncSocket = new AsyncUdpSocket(socket, localEndpoint, OnReceived, ProcessError);
      AsyncSocket.TargetEndPoint = RemoteEndPoint;
      
    }
    
    /// <summary>
    /// Create a new udp connection to handle communication to a single endpoint from the specified socket.
    /// </summary>
    public UdpConnection(UdpServer server, Socket socket, IPEndPoint remoteEndpoint) {
      
      // persist the server
      Server = server;
      RemoteEndPoint = remoteEndpoint;
      
      _timeoutTimer = new Timer(_timeoutMilliseconds, ActionSet.New(ProcessError, new TimeoutException("UdpConnection timeout.")));
      
      _header = new UdpHeader();
      _header.Compression = _defaultCompression = DecompressionMethods.None;
      
      // create the encoder
      _encoder = Encoding.UTF8.GetEncoder();
      _lock = new Lock();
      _message = new UdpMessage(this);
      
      _messages = new ArrayRig<UdpMessage>();
      _onMessage = new ActionPop<UdpMessage>();
      
      _errors = new ArrayRig<Exception>();
      _onError = new ActionPop<Exception>();
      
      // create an async socket to manage the socket
      AsyncSocket = new AsyncUdpSocket(socket, (IPEndPoint)socket.LocalEndPoint, OnReceived, ProcessError);
      AsyncSocket.TargetEndPoint = RemoteEndPoint;
      
    }
    
    /// <summary>
    /// Close this connection and remove it from the client connection pool.
    /// </summary>
    public void Dispose() {
      
      _lock.Take();
      if(_disposed) {
        _lock.Release();
        return;
      }
      _disposed = true;
      
      if(_timeoutTimer != null) {
        _timeoutTimer.Run = false;
        _timeoutTimer = null;
      }
      
      Log.Info("Disposing of connection '"+this+"'.");
      
      // should the underlying socket be disposed?
      if(Server.AsyncSocket.Socket != AsyncSocket.Socket) {
        // yes, dispose
        AsyncSocket.Socket.Dispose();
      }
      
      // dispose of the async socket
      AsyncSocket.Dispose();
      
      // remove the client from the server collection
      Server.RemoveConnection(this);
      
      _message.Dispose();
      
      _lock.Release();
      
    }
    
    /// <summary>
    /// Send the specified string to the client.
    /// </summary>
    public unsafe void Send(string str, TransportOptions options = null, IAction onSent = null) {
      
      // is disposed?
      if(_disposed) {
        Log.Warning("Cannot send from disposed connection.");
        return;
      }
      
			// reset the timeout
			if(_timeoutTimer != null) _timeoutTimer.Reset(_timeoutMilliseconds);
      
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
      
      // send the byte buffer
      SendBytes(buffer, 0, count);
      
      // cache the buffer
      BufferCache.Set(buffer);
      
    }
    
    /// <summary>
    /// Send the specified byte collection to the client. This may take a number of buffers to complete
    /// and therefore not be synchronous.
    /// </summary>
    public void Send(byte[] buffer, int offset, int count, TransportOptions options = null, IAction onSent = null) {
      
      // is disposed?
      if(_disposed) {
        Log.Warning("Cannot send from disposed connection.");
        return;
      }
      
			// reset the timeout
			if(_timeoutTimer != null) _timeoutTimer.Reset(_timeoutMilliseconds);

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
    public void Send(Stream stream, int length = -1, TransportOptions options = null, IAction onSent = null) {
      
      // is disposed?
      if(_disposed) {
        Log.Warning("Cannot send from disposed connection.");
        return;
      }
      
			// reset the timeout
			if(_timeoutTimer != null) _timeoutTimer.Reset(_timeoutMilliseconds);
      
      // start sending the stream
      StartSendStream(stream, length == -1 ? (int)(stream.Length - stream.Position) : length, options, onSent);
      
    }
    
    /// <summary>
    /// Send the specified udp message. This assumes the stream was written
    /// </summary>
    public void Send(UdpMessage message, TransportOptions options = null, IAction onSent = null) {
      
      // is disposed?
      if(_disposed) {
        Log.Warning("Cannot send from disposed connection.");
        return;
      }
      
			// reset the timeout
			if(_timeoutTimer != null) _timeoutTimer.Reset(_timeoutMilliseconds);

      // reset the message stream position
      message.Buffer.Position = 0;
      
      // start sending the message stream
      StartSendStream(message.Buffer.Stream, (int)message.Buffer.WriteEnd, options, onSent);
      
    }
    
    /// <summary>
    /// Send the specified web resource.
    /// </summary>
    public void Send(WebResource resource, TransportOptions options = null, IAction onSent = null) {
      
      // is disposed?
      if(_disposed) {
        Log.Warning("Cannot send from disposed connection.");
        return;
      }
      
      // get the resource stream then prepare to send the resource stream data
      resource.GetStream(ActionSet.New(SendResource, (Stream)null, (Lock)null, resource, options, onSent));
      
    }
    
    /// <summary>
    /// Send the file at the specified local path.
    /// </summary>
    public void SendLocal(string path, TransportOptions options = null, IAction onSent = null) {
      
      // is disposed?
      if(_disposed) {
        Log.Warning("Cannot send from disposed connection.");
        return;
      }
      
			// reset the timeout
			if(_timeoutTimer != null) _timeoutTimer.Reset(_timeoutMilliseconds);

      // get the extension from the path
      string extension;
      int extensionIndex = path.LastIndexOf(Chars.Stop);
      extension = extensionIndex == -1 ? null : path.Substring(extensionIndex);
      
      // open a file stream to the resource
      Stream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
      
      // start sending the file
      StartSendStream(fileStream, (int)fileStream.Length, options, onSent);
      
    }
    
    /// <summary>
    /// On data being received by the local end point.
    /// </summary>
    internal void OnReceived(IPEndPoint endpoint, byte[] bytes, int count) {
      
      // should the client receive?
      if(_disposed) {
        // no, cache the buffer
        BufferCache.Set(bytes);
        // skip
        return;
      }
      
      // should the endpoint be received from?
      if(RemoteEndPoint.Port != endpoint.Port || !RemoteEndPoint.Address.Equals(endpoint.Address)) {
        
        // no, cache the buffer
        BufferCache.Set(bytes);
        
        // log a warning
        Log.Warning("Connection "+this+" received from incorrect endoint '"+endpoint+"'.");
        
        return;
      }
      
      // reset the timeout
      if(_timeoutTimer != null) _timeoutTimer.Reset(_timeoutMilliseconds);
      
      int index = 0;
      
      _lock.Take();
      
      try {
        
        // while the bytes received haven't been completely parsed
        while(count != 0) {
          
          // try passing the received bytes to the current request
          int added = _message.TryAdd(bytes, index, count);
          
          // has the request been completed?
          if(_message.Complete) {
            
            // has the callback on requests been specified?
            if(_onMessage.Action == null) {
              
              // no, add to the backlog collection
              _messages.Add(_message);
              
            } else {
              
              // yes, run the callback
              _onMessage.Run(_message);
              
            }
            
            // construct a new message to receive bytes
            _message = new UdpMessage(this);
            
          } else if(added == -1) {
            
            _lock.Release();
            
            // no, there was an error, possibly with the format of the request
            // callback with the error
            ProcessError(new Exception(_message.ErrorMessage));
            
            // re-allocate the buffer
            BufferCache.Set(bytes);
            
            return;
          }
          
          // increment the index
          index += added;
          // decrement the number of bytes received that are still to be processed
          count -= added;
          
        }
        
        _lock.Release();
        
      } catch(Exception ex) {
        
        // replace a potentially corrupt message
        _message.Dispose();
        _message = new UdpMessage(this);
        
        _lock.Release();
        
        Log.Error("Exception processing message.", ex);
        
        // process the exception
        ProcessError(ex);
        
        return;
      }
      
    }
    
    /// <summary>
    /// Equality comparison between web clients.
    /// </summary>
    public bool Equals(UdpConnection other) {
      return RemoteEndPoint.Address == other.RemoteEndPoint.Address &&
             RemoteEndPoint.Port == other.RemoteEndPoint.Port;
    }
    
    /// <summary>
    /// Hashcode of the udp connection is based on the endpoint.
    /// </summary>
    public override int GetHashCode() {
      return RemoteEndPoint.GetHashCode();
    }
    
    /// <summary>
    /// Get a string representation of the web client.
    /// </summary>
    public override string ToString() {
      return "[UdpConnection LocalEndPoint: "+AsyncSocket.LocalEndPoint+
        " RemoteEndPoint: "+RemoteEndPoint+" TargetEndPoint: "+AsyncSocket.TargetEndPoint+"]";
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Process an exception occurance.
    /// </summary>
    protected void ProcessError(Exception ex) {
      
      if(_onError.Action == null) {
        
        _lock.Take();
        if(_onError.Action == null) {
          Log.Error("Exception in udp connection.", ex);
          _errors.Add(ex);
          _lock.Release();
        } else {
          _lock.Release();
          _onError.Run(ex);
        }
        
      } else _onError.Run(ex);
    }
    
    /// <summary>
    /// Process the send options into headers for a following send operation.
    /// </summary>
    protected void ProcessOptions(TransportOptions options) {
      
      if(options == null) {
        _header.Compression = _defaultCompression;
        _header.EncryptionPassword = null;
        return;
      }
      
      _header.Compression = options.Compression;
      _header.EncryptionPassword = options.EncryptionPassword;
      
    }
    
    /// <summary>
    /// Prepare and send header information to the client.
    /// </summary>
    protected void EnqueueHeader(int length) {
      
      // set the header body length
      _header.BodyLength = length;
      
      ByteBuffer writer = new ByteBuffer(new MemoryStream());
      
      // serialize the header
      _header.Serialize(writer);
      
      // reset the writers stream position
      writer.Stream.Position = 0;
      
      // enqueue the header bytes to be sent
      AsyncSocket.Enqueue(writer.Stream, (int)writer.WriteEnd);
      
      // dispose of the writer and underlying stream
      writer.Close();
      
    }
    
    /// <summary>
    /// Begin sending the stream of a web resource and prepare to release the resource lock on completion.
    /// </summary>
    protected void SendResource(Stream stream, Lock resourceLock, WebResource resource, TransportOptions options, IAction onSent) {
      
      // was the stream retrieved?
      if(stream == null) {
        
        // no, unlock the resource
        resourceLock.Release();
        
        // callback
        ProcessError(new Exception("A resource stream '"+resource.FullPath+"' was unable to be resolved."));
        
        return;
      }
      
      _lock.Take();
      
      if(onSent == null) _onSent = new ActionSet(resourceLock.Release);
      else _onSent = new ActionPair(resourceLock.Release, onSent);
      
      // set the content type
      ProcessOptions(options);
      
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
        
        // no, the resource compression matches the required compression
        
        // is the stream to be encrypted? yes, encrypt it
        if(_header.EncryptionPassword != null) {
          stream = Crypto.EncryptWithPassword(stream, _header.EncryptionPassword);
          length = (int)stream.Length;
        }
        
        try {
          
          // send the headers with the stream length
          EnqueueHeader(length);
          
          // send the stream
          AsyncSocket.Enqueue(stream, length);
          AsyncSocket.Send(_onSent);
          
        } finally {
          
          _onSent = null;
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
    protected void StartSendStream(Stream stream, int length, TransportOptions options = null, IAction onSent = null) {
      
      // does the buffer need to be compressed?
      if(options != null && options.Compression != DecompressionMethods.None ||
        _defaultCompression != DecompressionMethods.None) {
        
        // yes, compress the stream
        stream = StreamHelper.Compress(options == null ? _defaultCompression : options.Compression,
          System.IO.Compression.CompressionLevel.Fastest, stream, length);
        length = (int)stream.Length;
        
        // take the send lock
        _lock.Take();
        
        // process the options
        ProcessOptions(options);
        
        // is the stream to be encrypted? yes, encrypt it
        if(_header.EncryptionPassword != null) {
          var encryptedStream = Crypto.EncryptWithPassword(stream, _header.EncryptionPassword);
          // dispose of the compressed stream
          stream.Dispose();
          // reference the encrypted stream
          stream = encryptedStream;
          // set the new stream length
          length = (int)stream.Length;
        }
        
      } else {
        
        // no, take the send lock
        _lock.Take();
        
        // process the options
        ProcessOptions(options);
        
        // is the stream to be encrypted? yes, encrypt it
        if(_header.EncryptionPassword != null) {
          stream = Crypto.EncryptWithPassword(stream, _header.EncryptionPassword);
          length = (int)stream.Length;
        }
        
      }
      
      try {
        
        // enqueue the header
        EnqueueHeader(length);
        
        // send the stream
        AsyncSocket.Enqueue(stream, length);
        AsyncSocket.Send(onSent);
        
      } finally {
        
        _lock.Release();
        
      }
      
    }
    
    /// <summary>
    /// On a web resource stream being compressed.
    /// </summary>
    protected void StartSendStream(MemoryStream stream, WebResource resource) {
      
      try {
        // replace the resource stream with the compressed bytes
        resource.ReplaceStreamLocked(stream, _header.Compression);
        
        // is the stream to be encrypted? yes, encrypt it
        if(_header.EncryptionPassword != null) stream = Crypto.EncryptWithPassword(stream, _header.EncryptionPassword);
        
        EnqueueHeader((int)stream.Length);
        
        // send the stream
        AsyncSocket.Enqueue(stream, (int)stream.Length);
        
        stream.Dispose();
        
        AsyncSocket.Send(_onSent);
        _onSent = null;
        
      } finally {
        
        // release the lock
        _lock.Release();
        
      }
      
    }
    
    /// <summary>
    /// Send a buffer of bytes to the client. Releases the lock.
    /// </summary>
    protected void SendBytes(byte[] bytes, int offset, int count) {
      
      try {
        
        // yes, does the buffer need to be compressed?
        if(_header.Compression != DecompressionMethods.None) {
          // yes, compress the bytes
          bytes = StreamHelper.Compress(_header.Compression,
            System.IO.Compression.CompressionLevel.Optimal, bytes, offset, ref count);
          offset = 0;
        }
        
        // are the bytes to be encrypted?
        if(_header.EncryptionPassword != null) {
          
          // yes, encrypt them
          byte[] buffer = BufferCache.Get(count);
          Buffer.BlockCopy(bytes, offset, buffer, 0, count);
          bytes = Crypto.EncryptWithPassword(buffer, _header.EncryptionPassword);
          
          // send headers with the content length
          EnqueueHeader(bytes.Length);
          
          // send the bytes
          AsyncSocket.Enqueue(bytes, 0, bytes.Length);
          AsyncSocket.Send(_onSent);
          
        } else {
          
          // send headers with the content length
          EnqueueHeader(count);
          
          // send the bytes
          AsyncSocket.Enqueue(bytes, offset, count);
          AsyncSocket.Send(_onSent);
          
        }
        
      } finally {
        
        _onSent = null;
        _lock.Release();
        
      }
      
      
    }
    
  }
  
}

