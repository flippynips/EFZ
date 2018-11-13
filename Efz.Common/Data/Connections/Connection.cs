using System;

using Efz.Threading;
using Efz.Tools;

namespace Efz.Network {
  
  /// <summary>
  /// Common Connection interface.
  /// </summary>
  public interface IConnection : IDisposable {
    /// <summary>
    /// The current connection path.
    /// </summary>
    string Path { get; set; }
    /// <summary>
    /// The current state of the connection.
    /// </summary>
    ConnectionState State { get; }
    /// <summary>
    /// Open the connection.
    /// </summary>
    void Open(IRun _onOpen = null);
    /// <summary>
    /// Close the connection.
    /// </summary>
    void Close(IRun _onClose = null);
  }
  
  /// <summary>
  /// Main class for connections to an external resource.
  /// </summary>
  public abstract class Connection<C> : IConnection where C : Connection<C>, new() {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The current state of this connection.
    /// </summary>
    public abstract ConnectionState State { get; protected set; }
    
    /// <summary>
    /// The path or connection string to the resource.
    /// </summary>
    public virtual string Path {
      get {
        return _path;
      }
      set {
        _path = value;
        // if connection is open
        if(State.Is(ConnectionState.Open)) {
          // close and open to refresh path
          Close();
          Open();
        }
      }
    }
    
    /// <summary>
    /// Time in milliseconds the connection is inactive for before being closed.
    /// A value of -1 will remove the timer.
    /// </summary>
    public long Timeout {
      get {
        return _timeout;
      }
      set {
        if(_timeout != value) {
          _timeout = value;
          if(_timeout == -1) {
            // remove timer
            if(_timer != null) {
              _locker.OnLock = null;
              _locker.OnUnlock = null;
              _timer.Run = false;
              _timer = null;
            }
          } else if(_timer == null) {
            _timer = new Timer(_timeout, OnTimeout, false);
          } else {
            _timer.Reset(_timeout);
          }
        }
      }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Inner connection path value.
    /// </summary>
    protected string _path;
    
    /// <summary>
    /// Number of milliseconds this connection will remain idle before being
    /// removed and disposed of.
    /// Default is 1400.
    /// </summary>
    protected long _timeout;
    /// <summary>
    /// Timer used for timeout.
    /// </summary>
    protected Timer _timer;
    /// <summary>
    /// The lock for access to the connection stream.
    /// </summary>
    protected LockSharedEvents _locker;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new connection to the resource at the path defined.
    /// </summary>
    protected Connection() {
      // default 2 second timeout
      _timeout = 2000;
      _timer = new Timer(_timeout, OnTimeout, false);
      
      _locker = new LockSharedEvents();
      
      State = ConnectionState.Closed;
      
      // setup the lock to restart the timeout timer
      _locker.OnLock = new ActionSet(() => _timer.Run = false);
      _locker.OnUnlock = new ActionSet<long>(_timer.Reset, _timeout);
    }
    
    /// <summary>
    /// Perform initialization of the connection. Returns if openned successfully. On open callback is performed with the lock active.
    /// </summary>
    public void Open(IRun onOpen = null) {
      Open(onOpen, true);
    }
    /// <summary>
    /// Perform any functions in order to close the connection and dispose of resources.
    /// </summary>
    public void Close(IRun onClose = null) {
      Close(onClose, true);
    }
    
    /// <summary>
    /// Called automatically when the connection is disposed of.
    /// </summary>
    public abstract void Dispose();
    
    //-------------------------------------------//
    
    /// <summary>
    /// When the connection has been inactive for the specified
    /// 'timeout' number of milliseconds.
    /// </summary>
    protected virtual void OnTimeout() {
      // remove connection from view
      ManagerConnections.Remove(this);
      // close the connection
      Close();
      _locker.OnLock = null;
      _locker.OnUnlock = null;
    }
    
    protected virtual void Open(IRun onOpen, bool tryLock) {
      // early out if open
      if(State.Is(ConnectionState.Open)) {
        if(onOpen != null) onOpen.Run();
        return;
      }
      
      if(tryLock) {
        // defer execution
        if(onOpen != null) {
          // get the lock
          if(!(tryLock = _locker.TryTake)) {
            _locker.TryLock(new ActionSet<IRun, bool>(Open, onOpen, false));
            return;
          }
        } else {
          // get lock
          _locker.Take();
        }
      }
      
      // set state
      State = ConnectionState.Openning;
      
      // open the connection
      if(OpenConnection()) {
        // connection was openned successfully
        State = ConnectionState.Open;
        
        // run on open
        if(onOpen != null) onOpen.Run();
      } else {
        // connection didn't open successfully - dispose of connection resources
        CloseConnection();
      }
      // unlock
      if(tryLock) _locker.Release();
    }
    
    /// <summary>
    /// Close the connection and dispose of resources. On close callback is performed with the lock active.
    /// </summary>
    protected virtual void Close(IRun onClose, bool takeLock) {
      // early out if not open
      if(State.Is(ConnectionState.Closed)) {
        if(onClose != null) onClose.Run();
        return;
      }
      
      if(takeLock) {
        // defer execution
        if(onClose != null) {
          // get the lock
          if(!(takeLock = _locker.TryTake)) {
            _locker.TryLock(new ActionSet<IRun, bool>(Close, onClose, false));
            return;
          }
        } else {
          // get lock
          _locker.Take();
        }
      }
      
      // set flags
      State = ConnectionState.Closed;
      
      // close connection
      CloseConnection();
      
      // run on close
      if(onClose != null) onClose.Run();
      
      // unlock
      if(takeLock) _locker.Release();
    }
    
    /// <summary>
    /// Opens the connection. Returns if connection was openned or is available.
    /// </summary>
    protected abstract bool OpenConnection();
    /// <summary>
    /// Closes the connection and disposes of resources.
    /// </summary>
    protected abstract void CloseConnection();
    
  }
  
}
