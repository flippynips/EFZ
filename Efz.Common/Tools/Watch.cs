using System;
using Efz.Threading;

namespace Efz.Tools {
  
  /// <summary>
  /// Repeating timer for repeatable scheduled actions.
  /// </summary>
  public class Watch : IDisposable {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Is the current watch running?
    /// </summary>
    public bool Run {
      get {
        return _running;
      }
      set {
        if(_running == value) return;
        _running = value;
        if(_running) {
          _updater = _needle.AddUpdate(Update);
        } else {
          _updater.Stop();
        }
      }
    }
    
    /// <summary>
    /// Time in milliseconds between each iteration.
    /// Set to '0' to stop the watch.
    /// </summary>
    public long Time {
      get { return _time; }
      set {
        
        if(_time == value) {
          CurrentTime = _time;
          return;
        }
        
        if(_time == 0 && value > 0) {
          CurrentTime = _time = value;
          if(!_needleSpecified) {
            if(_time % 1000 == 0) {
              _needle = ManagerUpdate.Iterant;
            } else {
              _needle = ManagerUpdate.Polling;
            }
          }
          if(_running) {
            _updater.Stop();
            _updater = _needle.AddUpdate(Update);
          }
          return;
        } else if(value == 0) {
          _running = false;
          _updater.Stop();
          return;
        }
        
        CurrentTime = _time = value;
        
        if(!_needleSpecified) {
          if(_time % 1000 == 0) {
            if(_needle != ManagerUpdate.Iterant) {
              _needle = ManagerUpdate.Iterant;
              if(_running) {
                _updater.Stop();
                _updater = _needle.AddUpdate(Update);
              }
            }
          } else if(_needle != ManagerUpdate.Polling) {
            _needle = ManagerUpdate.Polling;
            if(_running) {
              _updater.Stop();
              _updater = _needle.AddUpdate(Update);
            }
          }
        }
        
      }
    }
    /// <summary>
    /// The current time in milliseconds in the current iteration.
    /// </summary>
    public long CurrentTime;
    
    /// <summary>
    /// Is the watch iterating?
    /// </summary>
    public bool Repeat;
    /// <summary>
    /// Task run on each iteration.
    /// </summary>
    public IAction OnDone;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Updater task for the watch.
    /// </summary>
    protected ActionAct _updater;
    /// <summary>
    /// Should the timer be reset on the next update.
    /// </summary>
    protected bool _reset;
    /// <summary>
    /// Is the timer active?
    /// </summary>
    protected bool _running;
    
    /// <summary>
    /// Inner time in milliseconds between each iteration.
    /// </summary>
    protected long _time;
    
    /// <summary>
    /// The needle this watch uses for updates.
    /// </summary>
    protected Needle _needle;
    /// <summary>
    /// Has the needle been specified? If so, the needle won't
    /// be changed when iteration time is updated.
    /// </summary>
    protected bool _needleSpecified;
    
    //-------------------------------------------//
    
    public Watch(long milliseconds, bool repeat, Action onDone, bool run = true, Needle needle = null) : this(milliseconds, repeat, new Act(onDone, needle), run, needle) {}
    
    public Watch(long milliseconds, bool repeat, IAction onDone, bool run = true, Needle needle = null) {
      _time = CurrentTime = milliseconds;
      Repeat = repeat;
      OnDone = onDone;
      _updater = new ActionAct(Update);
      _running = run;
      
      // was the needle specified?
      if(needle == null) {
        // no, derive the correct needle
        _needle = _time % 1000 == 0 ?
          ManagerUpdate.Iterant :
          ManagerUpdate.Polling;
      } else {
        // yes, persist the needle
        _needleSpecified = true;
        _needle = needle;
      }
      
      if(_running && _time > 0) _needle.AddUpdate(_updater);
    }
    
    public void Dispose() {
      _updater.ToRun = false;
      _updater.Remove = true;
    }
    
    public void Reset() {
      if(_running) {
        _reset = true;
      } else {
        CurrentTime = _time;
        Run = true;
      }
    }
    
    public void Reset(long time) {
      if(_running) {
        _time = time;
        _reset = true;
      } else {
        CurrentTime = Time = time;
        Run = true;
      }
    }
    
    public void Update() {
      if(_running) {
        if(_reset) {
          CurrentTime = _time;
          _reset = false;
        }
        CurrentTime -= _needle.Delta;
        while(CurrentTime <= 0) {
          OnDone.Run();
          if(Repeat) {
            CurrentTime += _time;
          } else {
            Run = false;
            CurrentTime = _time;
            break;
          }
        }
      } else {
        _updater.Stop();
      }
    }

    //-------------------------------------------//


  }

}
