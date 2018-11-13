using System;

namespace Efz {
  
  /// <summary>
  /// Wrapper for an action with a flag for completion and another for whether this action should be removed from an execution queue.
  /// </summary>
  public class ActionAct : IRun, IEquatable<ActionAct>, IEquatable<IRun>, IEquatable<Action> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// This actions runnable item.
    /// </summary>
    public IRun Runnable;
    /// <summary>
    /// Flag to indicate the action task is not currently running. True by default.
    /// Both this and 'ToRun' must be 'true' in order for it to be picked up by a
    /// 'ThreadHandle'.
    /// </summary>
    public bool Ready = true;
    /// <summary>
    /// Should this action be run? True by default.
    /// </summary>
    public bool ToRun = true;
    /// <summary>
    /// Flag to remove this action from the queue of tasks to execute
    /// this flag will be inverted when the task is successfully removed.
    /// Default is false.
    /// </summary>
    public bool Remove;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    public ActionAct(IRun runnable) {
      Runnable = runnable;
    }
    
    public ActionAct(Action runnable) {
      Runnable = new ActionSet(runnable);
    }
    
    /// <summary>
    /// Task with single flag for removal after running.
    /// </summary>
    public ActionAct(IRun action, bool remove) {
      Runnable = action;
      Remove = remove;
    }
    
    /// <summary>
    /// Task with single flag for removal after running.
    /// </summary>
    public ActionAct(Action action, bool remove) {
      Runnable = new ActionSet(action);
      Remove = remove;
    }
    
    /// <summary>
    /// Run the action.
    /// </summary>
    public virtual void Run() {
      Runnable.Run();
      Ready = true;
    }
    
    /// <summary>
    /// Stop the action task from running.
    /// This is guaranteed to not run the action task again unless
    /// called from within the action that is run.
    /// </summary>
    public void Stop() {
      ToRun = false;
      Remove = true;
    }
    
    public bool Equals(Action onRun) {
      return Runnable.Equals(onRun);
    }
    
    public bool Equals(IRun onRun) {
      return onRun.Equals(Runnable);
    }
    
    public bool Equals(ActionAct task) {
      return task.Runnable.Equals(Runnable);
    }
    
    override public int GetHashCode() {
      return Runnable.GetHashCode();
    }
    
    override public bool Equals(object obj) {
      if(obj == null) {
        return false;
      }
      ActionAct task = obj as ActionAct;
      
      return task != null && task.Runnable.Equals(Runnable);
    }
    
    public override string ToString() {
      return "[ActionTask Runnable="+Runnable+", Ran="+Ready+", Run="+ToRun+", Remove="+Remove+"]";
    }
    
    //-------------------------------------------//
    
  }
  
}

