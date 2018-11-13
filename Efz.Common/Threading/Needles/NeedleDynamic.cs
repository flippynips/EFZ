using System;

using Efz.Collections;
using Efz.Tools;

namespace Efz.Threading {
  
  /// <summary>
  /// Used for tasks that are to be run indiscriminently whenever
  /// a thread is available to run them. Tasks are still checked
  /// for 'ToRun' and 'Ready', so an update task will not be run
  /// by two threads at the same time.
  /// </summary>
  public class NeedleDynamic : Needle {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Delta of the dynamic needle.
    /// </summary>
    public override long Delta { get; set; }
    /// <summary>
    /// Get the number of tasks waiting to be run.
    /// </summary>
    public override int Count {
      get { return _tasks.Count; }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// The lock for updating value types.
    /// </summary>
    protected readonly Lock _lock;
    
    /// <summary>
    /// Flag for the paused state of the needle.
    /// </summary>
    protected bool _paused;
    
    /// <summary>
    /// The current set, action combination.
    /// </summary>
    protected ActionAct _current;
    /// <summary>
    /// Tasks to be delegated by this Needle.
    /// </summary>
    protected Queue<ActionAct> _tasks;
    
    /// <summary>
    /// Lock for the delta time updates.
    /// </summary>
    protected Lock _deltaLock;
    /// <summary>
    /// Last time the delta was updated.
    /// </summary>
    protected long _lastUpdate;
    
    /// <summary>
    /// Number of update tasks used to end iteration.
    /// </summary>
    protected int _updateCount;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Needles manage queues of tasks to be repeatedly executed after a length
    /// of time.
    /// </summary>
    public NeedleDynamic(string name, byte priority = 0) : base(name, priority) {
      _tasks = new Queue<ActionAct>();
      _lock = new Lock(true);
      _lastUpdate = Time.Timestamp;
    }
    
    /// <summary>
    /// Releases all resource used by the needle.
    /// </summary>
    public override void Dispose() {
    }
    
    /// <summary>
    /// Return a task if there is one.
    /// </summary>
    public override bool Next(out ActionAct task) {
      
      // has the needle been paused?
      if(_paused) {
        // yes, return no task
        task = null;
        return false;
      }
      
      // update the delta time
      if(_lock.TryTake) {
        Delta = (Time.Timestamp - _lastUpdate) / Time.Frequency;
        _lastUpdate = Time.Timestamp;
        
        _updateCount = 0;
        
        // get the next task
        while(_tasks.Dequeue(out task)) {
          
          // updating tasks is much less likely with dynamic needles
          if(!task.Remove) {
            
            // re-enqueue the task
            _tasks.Enqueue(task);
            
            // should the task be run?
            if(task.ToRun && task.Ready) {
              // set the current item
              _lock.Release();
              _current = task;
              task.Ready = false;
              return true;
            }
            
            // does the number of update tasks equal the number of remaining
            // tasks?
            if(++_updateCount == _tasks.Count) {
              // yes, return
              _lock.Release();
              return false;
            }
            
          }
          
          // should the task be run?
          if(task.ToRun && task.Ready) {
            // set the current item
            _lock.Release();
            _current = task;
            task.Ready = false;
            return true;
          }
        }
        _lock.Release();
      }
      
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
    /// Add an Task to be called on this needle.
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
      _paused = true;
      new Timer(time, () => {
        _lastUpdate = Time.Timestamp;
        _paused = false;
      });
    }
    
    /// <summary>
    /// Reset the timing values used to calculate iterations.
    /// </summary>
    public override void ResetDelta() {
      _lastUpdate = Time.Timestamp;
    }
    
    public override string ToString() {
      return "[Needle Priority="+Priority+", Delta="+Delta+", Pause="+_paused+", Current="+_current+"]";
    }
    
    //-------------------------------------------//
    
  }

}