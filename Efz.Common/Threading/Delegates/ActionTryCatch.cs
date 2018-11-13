/*
 * User: FloppyNipples
 * Date: 16/10/2017
 * Time: 7:19 PM
 */
using System;

namespace Efz {
  
  /// <summary>
  /// An action that will wrap the runnable item in a try-catch and persist any exception that is thrown
  /// as a result. A flag will also be set indicating completion.
  /// </summary>
  public class ActionTryCatch : IRun, IEquatable<ActionAct>, IEquatable<IRun>, IEquatable<Action> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// This actions runnable item.
    /// </summary>
    public readonly IRun Runnable;
    /// <summary>
    /// Exception that was thrown during execution.
    /// </summary>
    public Exception Exception;
    /// <summary>
    /// Flag indicating that the action has been run.
    /// </summary>
    public bool Ran;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    public ActionTryCatch(IRun runnable) {
      Runnable = runnable;
    }
    
    public ActionTryCatch(Action runnable) {
      Runnable = new ActionSet(runnable);
    }
    
    /// <summary>
    /// Run the action.
    /// </summary>
    public virtual void Run() {
      try {
        Runnable.Run();
      } catch(Exception ex) {
        Exception = ex;
      } finally {
        Ran = true;
      }
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
      return "[ActionTryCatch Runnable="+Runnable+", Ran="+Ran+", Exception="+Exception+"]";
    }
    
    //-------------------------------------------//
    
  }
  
}
