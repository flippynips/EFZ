using System;

using System.Threading.Tasks;
using Efz.Threading;

namespace Efz.Tools {
  
  /// <summary>
  /// Timeout for inline use.
  /// </summary>
  public class TimeInline {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Number of milliseconds each 'Wait' call sleeps for.
    /// </summary>
    public const int DefaultSleepMilliseconds = 100;
    
    /// <summary>
    /// Has the inline timer timed out?
    /// </summary>
    public bool TimedOut {
      get {
        _lock.Take();
        bool timed = _timeoutTimestamp < DateTime.UtcNow.Ticks;
        _lock.Release();
        return timed;
      }
    }
    
    /// <summary>
    /// Flag indicating task success before timeout.
    /// </summary>
    public bool Success {
      get { return _success; }
    }
    
    /// <summary>
    /// Update method to check if the timer is complete. Will return true if neither timed out or got success.
    /// Example Use : while(timer.Wait) { }
    /// </summary>
    public bool Wait {
      get {
        _lock.Take();
        // check task status
        _success = IsComplete();
        
        bool end = _success || _timeoutTimestamp < DateTime.UtcNow.Ticks;
        
        _lock.Release();
        
        if(!end) System.Threading.Thread.Sleep(SleepMilliseconds);
        
        return !end; 
      }
    }
    
    /// <summary>
    /// Number of milliseconds the thread will sleep for each 'Wait' call.
    /// </summary>
    public int SleepMilliseconds;
    /// <summary>
    /// Function used to determine completion.
    /// </summary>
    public Func<bool> IsComplete;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Lock used for async access to inline timer.
    /// </summary>
    protected Lock _lock;
    /// <summary>
    /// Inner flag representing task completion.
    /// </summary>
    protected bool _success;
    
    /// <summary>
    /// Tick count when the timer is to be timed out.
    /// </summary>
    protected long _timeoutTimestamp;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize with the timeout time in milliseconds.
    /// getComplete gets whether the task is complete.
    /// </summary>
    public TimeInline(long time, Func<bool> isComplete = null, int sleepMilliseconds = DefaultSleepMilliseconds) {
      IsComplete = isComplete;
      _timeoutTimestamp = Time.Timestamp + time * Time.Frequency;
      _lock = new Lock();
      SleepMilliseconds = sleepMilliseconds;
    }
    
    /// <summary>
    /// Asynchronously wait an iteration.
    /// Example Use : while(await timer.WaitAsync()) { }
    /// </summary>
    public async Task<bool> WaitAsync() {
      _lock.Take();
      // check task status
      _success = IsComplete();
      
      bool end = _success || _timeoutTimestamp < DateTime.UtcNow.Ticks;
      
      _lock.Release();
      
      if(!end) await Task.Delay(SleepMilliseconds);
      
      return !end;
    }
    
    //-------------------------------------------//
    
  }

}
