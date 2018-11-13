using System.Threading;
using Efz.Collections;

namespace Efz.Threading {
  
  /// <summary>
  /// Facilitates sharing and management of a spin lock over multiple threads. Contains useful methods for defering execution if necessary.
  /// </summary>
  public class LockSharedEvents : LockShared {
        
    //-------------------------------------------//
    
    /// <summary>
    /// Action that is run whenever the shared lock is released.
    /// </summary>
    public IAction OnUnlock;
    /// <summary>
    /// Action that is run whenever the shared lock is taken.
    /// </summary>
    public IAction OnLock;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Get the lock. If taken the current thread will enter a queue for the lock.
    /// </summary>
    public override void Take() {
      
      // increment the end index of the queue
      int queuePosition = Interlocked.Increment(ref _queueEnd);
      
      // check if unlocked
      if(queuePosition == 1) {
        
        // flip the flag
        Locked = true;
        
        // set the queue start
        _queueStart = 1;
        
        if(OnLock != null) OnLock.Run();
        
      } else {
        
        // number of iterations spent waiting for the lock
        int iteration = 0;
        
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
          
          #if DEBUG
          int queueCount = queuePosition - _queueStart;
          if(queueCount > ManagerUpdate.ThreadCount && queueCount != queuePosition) {
            Log.Error("Suspected deadlock. This lock has been taken by all threads. Queue count '"+queueCount+"' and queue position '"+queuePosition+"'.");
          }
          #endif
        }
        
      }
      
    }
    
    /// <summary>
    /// Non-blocking lock function. Calls the action when the lock is freed. Optionally do not automatically unlock once the task has run.
    /// </summary>
    public override void TryLock(Action onAvailable, Needle needle = null, bool autoUnlock = true) {
      TryLock(new ActionSet(onAvailable), needle, autoUnlock);
    }
    
    /// <summary>
    /// Attempts to lock the gate. The action is run once the gate becomes free.
    /// Queued actions are run in order.
    /// </summary>
    public override void TryLock(IAction onAvailable, Needle needle = null, bool autoUnlock = true) {
      
      // try get the lock
      if(TryTake) {
        // run the on lock method if set
        if(OnLock != null) OnLock.Run();
        if(needle == null) needle = ManagerUpdate.Control;
        
        // replace action to unlock once task has run
        if(autoUnlock) {
          needle.AddSingle(new ActionPair(onAvailable, Release));
        } else {
          // run on lock function
          needle.AddSingle(onAvailable);
        }
        
        return;
      }
      
      if(autoUnlock) {
        // set an action to unlock once task has run and add the task to the queue
        _queue.Enqueue(new Act(new ActionPair(onAvailable, Release), needle));
      } else {
        // add the on available task
        _queue.Enqueue(new Act(onAvailable, needle));
      }
      
    }
    
    /// <summary>
    /// Releases the current lock or runs the next on available action.
    /// </summary>
    override public void Release() {
      
      var queueStart = _queueStart;
      _queueStart = 1;
      
      // if any thread is waiting for the lock to continue - give it precedence
      if(Interlocked.CompareExchange(ref _queueEnd, 1, queueStart) == queueStart) {
        
        // check for queued actions
        Act act;
        if(_queue.Dequeue(out act)) {
          
          // run the next task
          act.Run();
          
        } else {
          
          Locked = false;
          
          // are there any threads waiting on the lock?
          if(Interlocked.CompareExchange(ref _queueEnd, 0, 1) == 1) {
            
            // run on unlock
            if(OnUnlock != null) OnUnlock.Run();
            
          } else {
            
            // no, reset the locked flag
            Locked = true;
            
            // set the start index
            ++_queueStart;
            
          }
          
        }
        
      } else {
        
        // increment the queue start index
        _queueStart = queueStart + 1;
        
      }
      
    }
    
    //-------------------------------------------//
    
  }
  
}
