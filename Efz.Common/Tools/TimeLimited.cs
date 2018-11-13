using System;
using System.Diagnostics;

using Efz.Threading;
using Efz.Tools;

namespace Efz {
  
  /// <summary>
  /// Class for managing a time limited resource. Allows specification of a time
  /// frame within which a limited number of calls to the 'Next' method will
  /// return true.
  /// </summary>
  public class TimeLimited {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Limit to apply.
    /// </summary>
    public int Limit;
    /// <summary>
    /// Time frame in which to apply the limit.
    /// </summary>
    public int Seconds;
    
    /// <summary>
    /// The current number of calls this time frame.
    /// </summary>
    public int Count {
      get {
        _lock.Take();
        CheckFrame();
        int count = (int)_count;
        _lock.Release();
        return count;
      }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Inner number of requests during the current frame.
    /// </summary>
    protected int _count;
    
    /// <summary>
    /// Last timestamp.
    /// </summary>
    protected long _timestamp;
    
    /// <summary>
    /// Lock used for threadsafety.
    /// </summary>
    protected Lock _lock;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize with the timeout time in milliseconds.
    /// getComplete gets whether the task is complete.
    /// </summary>
    public TimeLimited(int limit, int seconds) {
      Limit = limit;
      Seconds = seconds;
      
      _lock = new Lock();
      _count = 0;
      _timestamp = Time.Timestamp;
    }
    
    /// <summary>
    /// Increment the request count by one. Returns false if the request
    /// has not been reached.
    /// </summary>
    public bool Next() {
      _lock.Take();
      if(((Time.Timestamp - _timestamp) / Time.SecondFrequency) > Seconds) {
        _timestamp = Time.Timestamp;
        _count = 1;
        _lock.Release();
        return true;
      }
      if(_count < Limit) {
        ++_count;
        _lock.Release();
        return true;
      }
      _lock.Release();
      return false;
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Check and reset frame count if needed.
    /// </summary>
    protected void CheckFrame() {
      if(((Time.Timestamp - _timestamp) / Time.SecondFrequency) > Seconds) {
        _timestamp = Time.Timestamp;
        _count = 0;
      }
    }
    
    
  }

}
