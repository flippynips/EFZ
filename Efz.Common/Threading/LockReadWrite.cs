using System.Threading;

namespace Efz.Threading {
  
  /// <summary>
  /// Helper class for rotating items back and forth. Useful for threading.
  /// </summary>
  public class LockReadWrite {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Attain the lock if it's in an unlocked state.
    /// </summary>
    public bool TryTakeRead {
      get {
        if(WriteLocked || !_lock.TryTake) return false;
        
        if(!WriteLocked) {
          ReadLocked = true;
          ++_readCount;
          
          _lock.Release();
          return true;
        }
        
        _lock.Release();
        return false;
      }
    }
    
    /// <summary>
    /// Attain the lock if it's in an unlocked state.
    /// </summary>
    public bool TryTakeWrite {
      get {
        if(WriteLocked || ReadLocked || !_lock.TryTake) return false;
        
        if(!ReadLocked && !WriteLocked) {
          WriteLocked = true;
          return true;
        }
        
        _lock.Release();
        return false;
      }
    }
    
    /// <summary>
    /// Flag for if the lock is currently taken or not.
    /// </summary>
    public bool ReadLocked;
    /// <summary>
    /// Flag for if the lock is currently taken or not.
    /// </summary>
    public bool WriteLocked;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Number of times the read lock has been taken.
    /// </summary>
    protected volatile int _readCount;
    /// <summary>
    /// Lock to make transferring lock states threadsafe.
    /// </summary>
    protected Lock _lock;
    
    //-------------------------------------------//
    
    public LockReadWrite() {
      _lock = new Lock();
    }
    
    /// <summary>
    /// Get the lock. If taken the current thread will enter a queue for the lock.
    /// </summary>
    public virtual void TakeRead() {
      _lock.Take();
      ReadLocked = true;
      ++_readCount;
      _lock.Release();
    }
    
    /// <summary>
    /// Get the lock. If taken the current thread will enter a queue for the lock.
    /// </summary>
    public virtual void TakeWrite() {
      _lock.Take();
      while(ReadLocked) {
        _lock.Release();
        int iteration = 0;
        while(ReadLocked) {
          // perform reserved iterations in order to avoid context switching
          switch(++iteration) {
            case 0:
            case 5:
            case 10:
            case 15:
            case 20:
            case 25:
              Thread.Sleep(1);
              break;
            case 30:
              Thread.Sleep(3);
              iteration = 0;
              break;
            default:
              Thread.Yield();
              break;
          }
        }
        _lock.Take();
      }
      WriteLocked = true;
    }
    
    /// <summary>
    /// Release the lock from use. Another function may now take the lock.
    /// </summary>
    public virtual void ReleaseRead() {
      _lock.Take();
      if(--_readCount == 0) ReadLocked = false;
      _lock.Release();
    }
    
    /// <summary>
    /// Release the lock from use. Another function may now take the lock.
    /// </summary>
    public virtual void ReleaseWrite() {
      WriteLocked = false;
      _lock.Release();
    }
    
    /// <summary>
    /// Get a string representation of the lock.
    /// </summary>
    public override string ToString() {
      return "[Lock WriteTaken="+WriteLocked+" ReadTaken="+ReadLocked+" ReadCount="+_readCount+"]";
    }

    
    //-------------------------------------------//
    
    
  }
  
}
