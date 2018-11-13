/*
 * User: FloppyNipples
 * Date: 09/05/2017
 * Time: 19:33
 */
using System;
using System.Threading;

namespace Efz.Threading {
  
  /// <summary>
  /// Custom SynchronizationContext for asynchronous operations within a Needle.
  /// </summary>
  public class NeedleContext : SynchronizationContext {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Needle that causes this context to be set.
    /// </summary>
    protected Needle _needle;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Context for the needle.
    /// </summary>
    public NeedleContext(Needle needle) {
      _needle = needle;
    }
    
    /// <summary>
    /// Create a copy of this synchronization context.
    /// </summary>
    public override SynchronizationContext CreateCopy() {
      return new NeedleContext(_needle);
    }
    
    /// <summary>
    /// Method that receives the send delegate and executes it on a thread synchronously.
    /// </summary>
    public override void Send(SendOrPostCallback d, object state) {
      
      // set the current context
      //SynchronizationContext.SetSynchronizationContext(_needle.Context);
      
      // create an action that will be run safely on the needle
      ActionTryCatch action = new ActionTryCatch(ActionSet.New(Run, d, state));
      
      // have the delegate run on the needle and wait for it to complete
      _needle.AddSingle(action);
      
      // wait while the action has not run
      while(!action.Ran) { Thread.Sleep(1); }
      
      // the action is complete - was there an exception?
      if(action.Exception != null) {
        // yes, rethrow
        throw action.Exception;
      }
      
    }
    
    /// <summary>
    /// Method that receives the post delegate and executes it on a thread asynchronously.
    /// </summary>
    public override void Post(SendOrPostCallback d, object state) {
      _needle.AddSingle(Run, d, state);
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Run the callback delegate with the state.
    /// </summary>
    protected void Run(SendOrPostCallback callback, object state) {
      callback(state);
    }
    
  }
  
}
