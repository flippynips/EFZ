using System;
using System.Threading;

using Efz.Collections;

namespace Efz.Threading {
  
  /// <summary>
  /// Facilitates sharing and management of a spin lock over multiple threads. Contains useful methods for defering execution if necessary.
  /// </summary>
  public class LockShared : Lock {
        
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Queued tasks that get called when the gate becomes available.
    /// </summary>
    protected SafeQueue<Act> _queue;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new shared gate.
    /// </summary>
    public LockShared() {
      _queue = new SafeQueue<Act>();
    }
    
    /// <summary>
    /// Non-blocking lock function. Calls the action when the lock is freed. Optionally do not automatically unlock once the task has run.
    /// </summary>
    public virtual void TryLock(Action onAvailable, Needle needle = null, bool autoUnlock = true) {
      TryLock(new ActionSet(onAvailable), needle, autoUnlock);
    }
    
    /// <summary>
    /// Attempts to lock the gate. The action is run once the gate becomes free.
    /// Queued actions are run in order.
    /// </summary>
    public virtual void TryLock(IAction onAvailable, Needle needle = null, bool autoUnlock = true) {
      // replace action to unlock once task has run
      if(autoUnlock) onAvailable = new ActionPair(onAvailable, Release);
      
      // try get the lock
      if(TryTake) {
        // replace needle reference if needed
        if(needle == null) ManagerUpdate.Control.AddSingle(onAvailable);
        else needle.AddSingle(onAvailable);
        return;
      }
      
      // add to queue
      _queue.Enqueue(new Act(onAvailable));
    }
    
    /// <summary>
    /// Releases the current lock or runs the next on available action.
    /// </summary>
    public override void Release() {
      
      var queueStart = _queueStart;
      _queueStart = 1;
      
      // are there any threads waiting on the lock?
      if(Interlocked.CompareExchange(ref _queueEnd, 1, queueStart) == queueStart) {
        
        // are there any actions waiting on the lock?
        Act act;
        if(_queue.Dequeue(out act)) {
          
          // set the queue start index
          _queueStart = 1;
          
          act.Run();
          
        } else {
          
          // no, release the lock
          
          queueStart = _queueStart;
          _queueStart = 0;
          
          // flop the locked flag
          Locked = false;
          
          // is the queue empty? yes, set the end
          if(Interlocked.CompareExchange(ref _queueEnd, 0, queueStart) != queueStart) {
            
            // no, re-flip the flag
            Locked = true;
            
            // increment the queue start index
            _queueStart = queueStart + 1;
            
          }
          
        }
        
      } else {
        
        // yes, increment the queue start index
        _queueStart = queueStart + 1;
        
      }
      
    }
    
    //-------------------------------------------//
    
  }
  
}
