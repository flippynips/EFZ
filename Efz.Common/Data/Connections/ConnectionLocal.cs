using System;
using System.IO;
using System.Text;

using Efz.Network;
using Efz.Threading;
using Efz.Tools;

namespace Efz.Network {
  
  /// <summary>
  /// Handles structures for retrieving data from local resources.
  /// </summary>
  public class ConnectionLocal : Connection<ConnectionLocal>,
    IGetValue<ByteBuffer>,
    IGetValue<FileStream> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The current state of this connection.
    /// </summary>
    public override ConnectionState State { get; protected set; }
    
    public FileMode FileMode {
      get {
        return _fileMode;
      }
      set {
        if(_fileMode == value) return;
        _fileMode = value;
        Close();
      }
    }
    public FileAccess FileAccess {
      get {
        return _fileAccess;
      }
      set {
        if(_fileAccess == value) return;
        _fileAccess = value;
        Close();
      }
    }
    public FileShare FileShare {
      get {
        return _fileShare;
      }
      set {
        if(_fileShare == value) return;
        _fileShare = value;
        Close();
      }
    }
    
    public Encoding Encoding {
      get {
        return _encoding;
      }
      set {
        if(_encoding == value) return;
        _encoding = value;
        Close();
      }
    }
    public int BufferSize {
      get {
        return _bufferSize;
      }
      set {
        if(_bufferSize == value) return;
        _bufferSize = value;
        Close();
      }
    }
    
    //-------------------------------------------//
    
    protected FileMode _fileMode     = FileMode.OpenOrCreate;
    protected FileAccess _fileAccess = FileAccess.ReadWrite;
    protected FileShare _fileShare   = FileShare.None;
    protected int _bufferSize        = 8192;
    protected Encoding _encoding     = Encoding.ASCII;
    
    protected FileStream _fileStream;
    protected ByteBuffer _byteStream;
    
    protected ByteBuffer ByteStream {
      get {
        if(State.Is(ConnectionState.Open)) { return _byteStream ?? (_fileStream == null ? null : _byteStream = new ByteBuffer(_fileStream, _bufferSize, Encoding)); }
        return null;
      }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new local connection.
    /// </summary>
    public ConnectionLocal() {
    }
    
    public override void Dispose() {
      if(!State.Is(ConnectionState.Closed)) {
        Close();
      }
    }
    
    /// <summary>
    /// Get the current stream writer for this connection. Makes the request if not already made.
    /// </summary>
    public void Get(IAction<ByteBuffer> onValue, Needle needle = null) {
      if(!State.Is(ConnectionState.Open)) {
        Open(new ActionPocket<ByteBuffer>(onValue, new FuncSet<ByteBuffer>(() => ByteStream)));
      } else {
        onValue.ArgA = ByteStream;
        _locker.TryLock(onValue);
      }
    }
    /// <summary>
    /// Get the current stream writer for this connection. Makes the request if not already made.
    /// </summary>
    public void Get(out Teple<LockShared, ByteBuffer> stream) {
      if(!State.Is(ConnectionState.Open)) Open();
      _locker.Take();
      stream = new Teple<LockShared, ByteBuffer>(_locker, ByteStream);
    }
    
    /// <summary>
    /// Get the base file stream for this connection. Makes the request if not already made.
    /// </summary>
    public void Get(IAction<FileStream> onValue, Needle needle = null) {
      if(!State.Is(ConnectionState.Open)) {
        Open(new ActionPocket<FileStream>(onValue, new FuncSet<FileStream>(() => _fileStream)));
      } else {
        onValue.ArgA = _fileStream;
        _locker.TryLock(onValue, needle);
      }
    }
    public void Get(out Teple<LockShared, FileStream> fileStream) {
      if(!State.Is(ConnectionState.Open)) Open();
      _locker.Take();
      fileStream = new Teple<LockShared, FileStream>(_locker, _fileStream);
    }
    
    //-------------------------------------------//
    
    protected override bool OpenConnection() {
      switch(FileMode) {
        case FileMode.OpenOrCreate:
        case FileMode.CreateNew:
        case FileMode.Create:
          try {
            _fileStream = new FileStream(Path, FileMode, FileAccess, FileShare, BufferSize);
          } catch(Exception ex) {
            Log.Warning(this + " FileStream could not be created. " + ex.Messages());
            State = ConnectionState.Broken;
          }
          break;
        case FileMode.Open:
        case FileMode.Append:
        case FileMode.Truncate:
          try {
            // ensure the path exists
            ManagerResources.CreateFilePath(Path);
            // get the file stream
            _fileStream = new FileStream(Path, FileMode, FileAccess, FileShare, BufferSize);
          } catch(Exception ex) {
            Log.Warning(this + " FileStream could not be created. " + ex.Messages());
            State = ConnectionState.Broken;
          }
          break;
      }
      return !State.Is(ConnectionState.Broken);
    }
    
    /// <summary>
    /// Connection is locked before this method is called.
    /// </summary>
    protected override void CloseConnection() {
      
      // dispose of all resources
      if(_byteStream != null) _byteStream.Close();
      if(_fileStream != null) _fileStream.Close();
      _byteStream = null;
      _fileStream = null;
      
    }
    
  }
  
}
