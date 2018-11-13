using System;

using Efz.Collections;
using Efz.Threading;

namespace Efz.Tools {
  
  /// <summary>
  /// A pseudo state machine for completion of tasks.
  /// </summary>
  public class TaskMachine {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Action called when each task is completed.
    /// </summary>
    public IAction<string> OnTask {
      get {
        return _onEachTask;
      }
      set {
        _lock.Take();
        _onEachTask = value;
        _lock.Release();
      }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Ticker to signal the end of the task machine.
    /// </summary>
    protected Ticker _ticker;
    /// <summary>
    /// Queue of tasks.
    /// </summary>
    protected SafeQueue<Act> _tasks;
    
    /// <summary>
    /// Callback on task.
    /// </summary>
    protected IAction<string> _onEachTask;
    
    /// <summary>
    /// On done.
    /// </summary>
    protected ActionEvent _onDone;
    /// <summary>
    /// Number of tasks to run each loop.
    /// </summary>
    protected int _runCount;
    /// <summary>
    /// Lock for external access.
    /// </summary>
    protected Lock _lock;
    
    //-------------------------------------------//
    
    public TaskMachine(IAction<string> onEachTask = null) {
      _tasks = new SafeQueue<Act>();
      _onDone = new ActionEvent();
      _ticker = new Ticker(_onDone, 0);
      _onEachTask = onEachTask;
      _lock = new Lock();
    }
    
    /// <summary>
    /// Add an action to be run on the task machine completing.
    /// </summary>
    public void AddOnDone(Action action) {
      _lock.Take();
      _onDone.Add(action);
      _lock.Release();
    }
    
    /// <summary>
    /// Add an action to be run on the task machine completing.
    /// </summary>
    public void AddOnDone(IAction action) {
      _lock.Take();
      _onDone.Add(action);
      _lock.Release();
    }
    
    /// <summary>
    /// Add an action to the task machine with an optional name identifier.
    /// </summary>
    public void Add(string name, Action action, Needle needle = null) {
      _ticker.Push();
      _tasks.Enqueue(new Act(new ActionPair(action, new ActionSet<string>(ActionComplete, name)), needle));
    }
    
    /// <summary>
    /// Add a task to the task machine with an optional name identifier.
    /// </summary>
    public void Add(string name, IAction action, Needle needle = null) {
      _ticker.Push();
      _tasks.Enqueue(new Act(new ActionPair(action, new ActionSet<string>(ActionComplete, name)), needle));
    }
    
    /// <summary>
    /// Add an action to the task machine with an optional name identifier.
    /// </summary>
    public void Add(Action action, Needle needle = null) {
      _ticker.Push();
      _tasks.Enqueue(new Act(new ActionPair(action, new ActionSet<string>(ActionComplete, string.Empty)), needle));
    }
    
    /// <summary>
    /// Add a task to the task machine with an optional name identifier.
    /// </summary>
    public void Add(IAction action, Needle needle = null) {
      _ticker.Push();
      _tasks.Enqueue(new Act(new ActionPair(action, new ActionSet<string>(ActionComplete, string.Empty)), needle));
    }
    
    /// <summary>
    /// Call run once to complete all tasks.
    /// </summary>
    public void Run() {
      _ticker.Push();
      // get run number
      _runCount = _ticker.Ticks - 1;
      // run all tasks
      while(_tasks.Dequeue()) _tasks.Current.Run();
      _ticker.Pull();
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// On each task completing.
    /// </summary>
    private void ActionComplete(string name) {
      
      _lock.Take();
      
      // on task complete
      if(_onEachTask != null) {
        _onEachTask.ArgA = name;
        _onEachTask.Run();
      }
      
      _lock.Release();
      
      // decrement the ticker count
      _ticker.Pull();
      
      // if all tasks from last Run call were complete
      // and there are more tasks - call Run again
      if(--_runCount == 0 && _ticker.Ticks != 0) Run();
    }
    
  }
  
}

