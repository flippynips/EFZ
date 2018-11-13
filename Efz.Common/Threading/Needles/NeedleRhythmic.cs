using System;

using Efz.Collections;
using Efz.Tools;

namespace Efz.Threading {
  
  /// <summary>
  /// Needle used for regularly updating tasks.
  /// </summary>
  public class NeedleRhythmic : Needle {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Delta remains constant.
    /// </summary>
    public override long Delta { get; set; }
    
    /// <summary>
    /// Get the number of tasks waiting to be run.
    /// Not super accurate.
    /// </summary>
    public override int Count {
      get { return _tasks.A.Count + _tasks.B.Count; }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// The lock for reading and writing current tasks.
    /// </summary>
    //protected readonly Lock _lock;
    
    /// <summary>
    /// Next timestamp that an iteration of tasks should be run.
    /// </summary>
    protected long _nextTimestamp;
    /// <summary>
    /// Target ticks per update.
    /// </summary>
    protected long _targetTicks;
    /// <summary>
    /// Temp field for the current timestamp.
    /// </summary>
    protected long _currentTicks;
    
    /// <summary>
    /// Flag for the paused state of the needle.
    /// </summary>
    protected bool _paused;
    
    /// <summary>
    /// This indicates the needle is not currently waiting for an iteration of delta time to run its actions.
    /// </summary>
    protected bool _running;
    /// <summary>
    /// Flag indicating that the current running iteration has not flipped the tasks collection.
    /// </summary>
    protected bool _flip;
    /// <summary>
    /// The current set, action combination.
    /// </summary>
    protected ActionAct _current;
    
    /// <summary>
    /// Tasks to be delegated by this Needle.
    /// </summary>
    protected Flipper<Belt<ActionAct>> _tasks;
    /// <summary>
    /// Lock for external access to the tasks in collection A.
    /// </summary>
    protected Lock _lockA;
    /// <summary>
    /// Lock for external access to the tasks in collection B.
    /// </summary>
    protected Lock _lockB;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Needles manage queues of tasks to be repeatedly executed after a length
    /// of time.
    /// </summary>
    public NeedleRhythmic(string name, long delta = 1000/60, byte priority = 50) : base(name, priority) {
      Delta = delta;
      _targetTicks = (long)(Delta * Time.Frequency);
      if(_targetTicks < 1) throw new ArgumentException("Delta target cannot be less than '0'.");
      _nextTimestamp = Time.Timestamp + _targetTicks;
      _tasks = new Flipper<Belt<ActionAct>>(new Belt<ActionAct>(true), new Belt<ActionAct>(true));
      _lockA = new Lock();
      _lockB = new Lock();
    }
    
    /// <summary>
    /// Releases all resource used by the needle.
    /// </summary>
    public override void Dispose() {
      _tasks.A.Clear();
      _tasks.B.Clear();
    }
    
    /// <summary>
    /// Return a task if there is one.
    /// </summary>
    public override bool Next(out ActionAct task) {
      
      // has the needle been paused or is the lock already taken?
      if(_paused) {
        // yes, return no task
        task = null;
        return false;
      }
      
      // if currently running
      if(_running) {
        
        // try get the A side lock
        if(_lockA.TryTake) {
          var tasks = _tasks.A;
          
          // get the next task
          while(tasks.Next() && !tasks.Loop) {
            
            if(tasks.Current.Remove) {
              tasks.RemoveCurrent();
              tasks.Current.Remove = false;
            }
            
            task = tasks.Current;
            
            // should the task be run?
            if(task.ToRun && task.Ready) {
              _current = task;
              // set the current item
              task.Ready = false;
              
              _lockA.Release();
              return true;
            }
            
          }
          
          if(_flip) {
            _flip = false;
            
            _lockB.Take();
            _tasks.Flip();
            _lockB.Release();
            
            tasks = _tasks.A;
            
            // get the next task
            while(tasks.Next() && !tasks.Loop) {
              
              if(tasks.Current.Remove) {
                tasks.RemoveCurrent();
                tasks.Current.Remove = false;
              }
              
              task = tasks.Current;
              
              // should the task be run?
              if(task.ToRun && task.Ready) {
                _current = task;
                // set the current item
                task.Ready = false;
                
                _lockA.Release();
                return true;
              }
              
            }
          }
          
          // return to wait for delta time
          _running = false;
          
          // release the a lock
          _lockA.Release();
          
        } else {
          task = null;
          return false;
        }
        
      }
      
      if(!_lockA.TryTake) {
        task = null;
        return false;
      }
      
      // if delta is greater than target it's time for another loop of tasks
      if(Time.Timestamp >= _nextTimestamp) {
        
        // add the target number of ticks
        _nextTimestamp = _nextTimestamp + _targetTicks;
        
        // this is running
        _running = true;
        _flip = true;
        
        // log of target ticks vs actual
//        if(Delta == 1000) {
//          Log.D(System.Threading.Thread.CurrentThread.Name + " Offset " + (_nextTimestamp - _targetTicks - Time.Timestamp));
//        }
        
        var tasks = _tasks.A;
        
        // get the next task
        while(tasks.Next() && !tasks.Loop) {
            
          if(tasks.Current.Remove) {
            tasks.RemoveCurrent();
            tasks.Current.Remove = false;
          }
          
          task = tasks.Current;
          
          // should the task be run?
          if(task.ToRun && task.Ready) {
            _current = task;
            // set the current item
            task.Ready = false;
            
            _lockA.Release();
            return true;
          }
          
        }
        
        if(_flip) {
          _flip = false;
          
          _lockB.Take();
          _tasks.Flip();
          _lockB.Release();
          
          tasks = _tasks.A;
          
          // get the next task
          while(tasks.Next() && !tasks.Loop) {
            
            if(tasks.Current.Remove) {
              tasks.RemoveCurrent();
              tasks.Current.Remove = false;
            }
            
            task = tasks.Current;
            
            // should the task be run?
            if(task.ToRun && task.Ready) {
              _current = task;
              // set the current item
              task.Ready = false;
              
              _lockA.Release();
              return true;
            }
            
          }
        }
        
        _running = false;
        
      }
      
      // release the a lock
      _lockA.Release();
      
      // no task return
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
      _lockB.Take();
      _tasks.B.Enqueue(task);
      _lockB.Release();
    }
    
    /// <summary>
    /// Add an action to be called on update using this needle.
    /// </summary>
    public override ActionAct AddUpdate(Action action) {
      var task = new ActionAct(action, false);
      _lockB.Take();
      _tasks.B.Enqueue(task);
      _lockB.Release();
      return task;
    }
    
    /// <summary>
    /// Add an action to be called on update using this needle.
    /// </summary>
    public override ActionAct AddUpdate(IAction action) {
      var task = new ActionAct(action, false);
      _lockB.Take();
      _tasks.B.Enqueue(task);
      _lockB.Release();
      return task;
    }
    
    /// <summary>
    /// Pause this needle for the specified number of milliseconds.
    /// </summary>
    public override void Pause(long time) {
      _paused = true;
      new Timer(time, () => {
        _nextTimestamp = Time.Timestamp + _targetTicks;
        _paused = false;
      });
    }
    
    /// <summary>
    /// Reset the timing values used to calculate iterations.
    /// </summary>
    public override void ResetDelta() {
      _nextTimestamp = Time.Timestamp + _targetTicks;
    }
    
    public override string ToString() {
      return "[Needle DeltaTarget="+_targetTicks+", Priority="+Priority+", Delta="+Delta+", Pause="+_paused+", Running="+_running+", Current="+_current+"]";
    }
    
    //-------------------------------------------//
    
  }

}