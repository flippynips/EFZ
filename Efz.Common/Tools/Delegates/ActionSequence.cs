/*
 * User: FloppyNipples
 * Date: 06/05/2017
 * Time: 23:05
 */
using System;

using System.Threading;
using Efz.Collections;
using Efz.Threading;

namespace Efz {
  
  /// <summary>
  /// Threadsafe management of a sequence of actions.
  /// </summary>
  public class ActionSequence : IAction {
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// Queue of actions to be run.
    /// </summary>
    protected SafeQueue<IAction> _queue;
    /// <summary>
    /// Pair of actions in order to continue running actions.
    /// </summary>
    protected ActionPair _pair;
    /// <summary>
    /// Is the action sequence running?
    /// </summary>
    protected int _running;
    
    /// <summary>
    /// Needle used to run the actions.
    /// </summary>
    protected Needle _needle;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a new action sequence.
    /// </summary>
    public ActionSequence(Needle needle = null) {
      _pair = new ActionPair();
      _pair.ActionB = new ActionSet(Next);
      _queue = new SafeQueue<IAction>();
      _needle = needle ?? ManagerUpdate.Control;
    }
    
    /// <summary>
    /// Add an action to be run in the sequence.
    /// </summary>
    public void Add(Action action) {
      _queue.Enqueue(new ActionSet(action));
    }
    
    /// <summary>
    /// Add an action to be run in the sequence.
    /// </summary>
    public void Add(IAction action) {
      _queue.Enqueue(action);
    }
    
    /// <summary>
    /// Add an action to be run in the sequence and ensure the sequence is running.
    /// </summary>
    public void AddRun(Action action) {
      _queue.Enqueue(new ActionSet(action));
      if(Interlocked.CompareExchange(ref _running, 1, 0) == 0) _needle.AddSingle(Next);
    }
    
    /// <summary>
    /// Add an action to be run in the sequence and ensure the sequence is running.
    /// </summary>
    public void AddRun(IAction action) {
      _queue.Enqueue(action);
      if(Interlocked.CompareExchange(ref _running, 1, 0) == 0) _needle.AddSingle(Next);
    }
    
    /// <summary>
    /// Run the sequences of actions.
    /// </summary>
    public void Run() {
      if(Interlocked.CompareExchange(ref _running, 1, 0) == 0) _needle.AddSingle(Next);
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Run the next action.
    /// </summary>
    protected void Next() {
      if(_queue.Dequeue()) {
        _queue.Current.Run();
        _needle.AddSingle(Next);
      } else {
        Interlocked.Decrement(ref _running);
        if(_queue.Count > 0 && Interlocked.CompareExchange(ref _running, 1, 0) == 0) Next();
      }
    }
    
  }
}
