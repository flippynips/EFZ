using System;

using Efz.Collections;
using Efz.Tools;

namespace Efz.Threading {
  
  /// <summary>
  /// Used for a sequence of tasks that are run sequentially.
  /// Each task is completed before the next task is run. There are
  /// never two tasks running at the same time.
  /// </summary>
  public class NeedleElastic : Needle {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Delta time in milliseconds between each complete cycle of tasks.
    /// </summary>
    public override long Delta {
      get { return _deltaTicks / Time.Frequency; }
      set { _deltaTicks = value * Time.Frequency; }
    }
    
    /// <summary>
    /// Get the number of tasks.
    /// </summary>
    public override int Count { 
      get { return _tasks.Count; }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// The lock for reading and writing current tasks.
    /// </summary>
    protected readonly Lock _lock;
    
    /// <summary>
    /// Last ticks since last update.
    /// </summary>
    protected long _deltaTicks;
    /// <summary>
    /// Milliseconds since tasks were last run.
    /// Only used for needles that aren't continuous.
    /// </summary>
    protected long _lastDeltaTicks;
    
    /// <summary>
    /// Flag for the paused state of the needle.
    /// </summary>
    protected bool _paused;
    
    /// <summary>
    /// This indicates the needle is not currently waiting for an iteration of delta time to run its actions.
    /// </summary>
    protected bool _running;
    /// <summary>
    /// The current set, action combination.
    /// </summary>
    protected ActionAct _current;
    
    /// <summary>
    /// Tasks to be delegated by this Needle.
    /// </summary>
    protected SafeQueue<ActionAct> _tasks;
    
    /// <summary>
    /// Persistance of the number of tasks each delta time update.
    /// </summary>
    protected int _taskCount;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Needles manage queues of tasks to be repeatedly executed after a length
    /// of time.
    /// </summary>
    public NeedleElastic(string name, byte priority = 0) : base(name, priority) {
      _tasks = new SafeQueue<ActionAct>();
      _lock = new Lock();
      _current = new ActionAct((IRun)null);
    }
    
    /// <summary>
    /// Releases all resource used by the needle.
    /// </summary>
    public override void Dispose() {
      _tasks.Dispose();
    }
    
    /// <summary>
    /// Run all pending tasks.
    /// </summary>
    public override void RunAll() {
      
      ActionAct task;
      while(Next(out task)) task.Run();
      
    }
    
    /// <summary>
    /// Return a task if there is one.
    /// </summary>
    public override bool Next(out ActionAct task) {
      
      // is the current task still running, has the needle been
      // paused or is the lock already taken?
      if(_paused || !_current.Ready || !_lock.TryTake) {
        // yes, return no task
        task = null;
        return false;
      }
      
      // get the next task
      while(--_taskCount >= 0 && _tasks.Dequeue()) {
        
        if(!_tasks.Current.Remove) _tasks.Enqueue(_tasks.Current);
        
        // should the task be run?
        if(_tasks.Current.ToRun && _tasks.Current.Ready) {
          // set the current item
          task = _current = _tasks.Current;
          _current.Ready = false;
          
          _lock.Release();
          return true;
        }
        
      }
      
      // get the number of ticks since the last update
      _deltaTicks = Time.Timestamp - _lastDeltaTicks;
      _lastDeltaTicks += _deltaTicks;
      
      // no task
      _lock.Release();
      
      // get the number of tasks to execute next iteration
      _taskCount = _tasks.Count;
      
      task = null;
      return false;
    }
    
    /// <summary>
    /// Add an Action to be called once in order on this needle.
    /// </summary>
    public override void AddSingle(Action action) {
      AddUpdate(new ActionAct(action, true));
    }
    
    /// <summary>
    /// Add an ActionSet to be called once in order on this needle.
    /// </summary>
    public override void AddSingle(IRun action) {
      AddUpdate(new ActionAct(action, true));
    }
    
    /// <summary>
    /// Add an Task to be called in order on this needle.
    /// </summary>
    public override void AddUpdate(ActionAct task) {
      _lock.Take();
      _tasks.Enqueue(task);
      _lock.Release();
    }
    
    /// <summary>
    /// Add an action to be called on update using this needle.
    /// </summary>
    public override ActionAct AddUpdate(Action action) {
      var task = new ActionAct(action, false);
      _lock.Take();
      _tasks.Enqueue(task);
      _lock.Release();
      return task;
    }
    
    /// <summary>
    /// Add an action to be called on update using this needle.
    /// </summary>
    public override ActionAct AddUpdate(IAction action) {
      var task = new ActionAct(action, false);
      _lock.Take();
      _tasks.Enqueue(task);
      _lock.Release();
      return task;
    }
    
    /// <summary>
    /// Pause this needle for the specified number of milliseconds.
    /// </summary>
    public override void Pause(long time) {
      _lock.Take();
      _paused = true;
      new Timer(time, () => {
        _lastDeltaTicks = Time.Timestamp;
        _paused = false;
      });
      _lock.Release();
    }
    
    /// <summary>
    /// Reset the timing values used to calculate iterations.
    /// </summary>
    public override void ResetDelta() {
      _deltaTicks = 0L;
      _lastDeltaTicks = Time.Timestamp;
    }
    
    public override string ToString() {
      return "[Needle Priority="+Priority+", Delta="+Delta+", Pause="+_paused+", Locker="+_lock+", Current="+_current+"]";
    }
    
    //-------------------------------------------//
    
  }

}