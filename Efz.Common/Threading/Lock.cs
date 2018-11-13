using System.Threading;

namespace Efz.Threading {
  
  /// <summary>
  /// Lightweight, lock for thread safe structures.
  /// </summary>
  public class Lock {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Attain the lock if it's in an unlocked state.
    /// </summary>
    public bool TryTake {
      get {
        // if not locked
        if(!Locked && Interlocked.CompareExchange(ref _queueEnd, 1, 0) == 0) {
          
          // flip the lock flag
          Locked = true;
          
          // set teh start of teh queue
          ++_queueStart;
          
          // lock was taken
          return true;
        }
        
        // lock wasn't taken
        return false;
      }
    }
    
    /// <summary>
    /// Flag for if the lock is currently taken or not.
    /// </summary>
    public bool Locked;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Current index in the queue.
    /// </summary>
    protected volatile int _queueStart;
    /// <summary>
    /// The length of the queue.
    /// </summary>
    protected int _queueEnd;
    
    protected bool _log;
    
    //-------------------------------------------//
    
    public Lock() {
    }
    
    /// <summary>
    /// Create a lock with option to log lock actions.
    /// </summary>
    public Lock(bool log) {
      _log = log;
    }
    
    /// <summary>
    /// Get the lock. If taken the current thread will enter a queue for the lock.
    /// </summary>
    public virtual void Take() {
      
      // increment the end index of the queue
      int queuePosition = Interlocked.Increment(ref _queueEnd);
      
      // is first in line?
      if(queuePosition == 1) {
        
        // yes flip the flag
        Locked = true;
        
        // set teh start of teh queue
        ++_queueStart;
        
      } else {
        
        //if(_log) Log.Error("Waiting for lock at position '"+queuePosition+"' : "+_queueStart+", "+_queueEnd+".");
        
        // number of iterations spent waiting for the lock
        int iteration = 0;
        
        #if DEBUG
        
        // while the current index isn't the queue position
        while(_queueStart != queuePosition) {
          // perform reserved iterations in order to avoid context switching
          switch(++iteration) {
            case 0:
            case 5:
            case 10:
            case 15:
            case 20:
            case 25:
              Thread.Sleep(0);
              break;
            case 30:
              iteration = 0;
              Thread.Sleep(1);
              break;
            default:
              Thread.Yield();
              break;
          }
          
        }
        
        #else
        
        // while the current index isn't the queue position
        while(_queueStart != queuePosition) {
          // perform reserved iterations in order to avoid context switching
          switch(++iteration) {
            case 0:
            case 5:
            case 10:
            case 15:
            case 20:
            case 25:
              Thread.Sleep(0);
              break;
            case 30:
              Thread.Sleep(1);
              iteration = 0;
              break;
            default:
              Thread.Yield();
              break;
          }
          
        }
        
        #endif
        
      }
      
      //if(_log) Log.Error("Lock taken here. " + _queueStart + ", " + _queueEnd);
      
    }
    
    /// <summary>
    /// Release the lock from use. Another function may now take the lock.
    /// </summary>
    public virtual void Release() {
      
      //if(_log) Log.Error("Lock released here. " + _queueStart + ", " + _queueEnd);
      
      // flop the locked flag
      Locked = false;
      var queueStart = _queueStart;
      _queueStart = 0;
      
      // is the queue empty? yes, set the end
      if(Interlocked.CompareExchange(ref _queueEnd, 0, queueStart) != queueStart) {
        
        // no, re-flip the flag
        Locked = true;
        
        // increment the queue start index
        _queueStart = queueStart + 1;
        
      }
      
    }
    
    /// <summary>
    /// Get a string representation of the lock.
    /// </summary>
    public override string ToString() {
      return "{Lock Taken=" + Locked + " QueueStart=" + _queueStart + " QueueEnd=" + _queueEnd + "}";
    }

    
    //-------------------------------------------//
    
    
  }
  
}
