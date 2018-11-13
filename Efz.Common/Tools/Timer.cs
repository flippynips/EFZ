using System;
using Efz.Threading;

namespace Efz.Tools {
  
  /// <summary>
  /// Simple Timer that makes use of the polling Needle for updates.
  /// </summary>
  public class Timer {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The remaining number of milliseconds on the Timer.
    /// </summary>
    public long Time;
    /// <summary>
    /// The Task that will be run on timeout.
    /// </summary>
    public Act OnDone;
    /// <summary>
    /// Get or set whether the Timer is running.
    /// </summary>
    public bool Run {
      get {
        return _updater.ToRun;
      }
      set {
        // if the current running value doesn't match the value to be set
        if(_updater.ToRun != value) {
          // if the timer is to start running
          if(value) {
            // if the updater was set to be removed and has not yet been removed (unlikely)
            if(_updater.Remove) {
              // initialize and add a new update task
              _updater = new ActionAct(Update);
              _needle.AddUpdate(_updater);
            } else {
              // set run flag
              _updater.ToRun = true;
              // add the updater
              _needle.AddUpdate(_updater);
            }
          } else {
            _updater.ToRun = false;
            _updater.Remove = true;
          }
        }
      }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// The Task used for updating.
    /// </summary>
    private ActionAct _updater;
    /// <summary>
    /// The needle this timer uses for updates.
    /// </summary>
    private readonly Needle _needle;
    
    //-------------------------------------------//
    
    public Timer(Action onDone) : this(new Act(onDone)) {}
    public Timer(IAction onDone) : this(new Act(onDone)) {}
    public Timer(Act onDone) {
      OnDone = onDone;
      _updater = new ActionAct(Update);
      _updater.ToRun = false;
    }
    
    public Timer(long time, Action onDone, bool run = true, Needle needle = null) : this(time, new Act(onDone), run, needle) {}
    public Timer(long time, IAction onDone, bool run = true, Needle needle = null) : this(time, new Act(onDone), run, needle) {}
    public Timer(long time, Act onDone, bool run = true, Needle needle = null) {
      
      Time    = time;
      OnDone  = onDone;
      _updater = new ActionAct(Update);
      _updater.ToRun = false;
      
      // has the needle been specified?
      if(needle == null) {
        // no, derive the needle to use
        _needle = Time % 1000 == 0 ? ManagerUpdate.Iterant : ManagerUpdate.Polling;
      } else {
        // yes, persist the needle
        _needle = needle;
      }
      
      Run = run;
    }
    
    /// <summary>
    /// Reset this timer to the specified time.
    /// </summary>
    public void Reset(long time) {
      Time = time;
      Run = true;
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Decrement the remaining time.
    /// </summary>
    private void Update() {
      Time -= _needle.Delta;
      if(Time <= 0) {
        Time = 0;
        _updater.ToRun = false;
        _updater.Ready = true;
        OnDone.Run();
        // if the timer wasn't restarted
        if(!_updater.ToRun) {
          _updater.Remove = true;
        }
      }
    }
    
  }

}
