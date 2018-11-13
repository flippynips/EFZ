using System;
using Efz.Collections;
using Efz.Threading;

namespace Efz {
  
  public class ActionSafeEvent : IAction {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Collection of actions that will be run.
    /// </summary>
    public ArrayRig<IAction> Actions = new ArrayRig<IAction>();
    
    //-------------------------------------------//
    
    /// <summary>
    /// Lock used for external access to the action.
    /// </summary>
    protected Lock _lock = new Lock();
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionSafeEvent() {}
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionSafeEvent(Action _action) {
      Actions.Add(new ActionSet(_action));
    }
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionSafeEvent(IAction _action) {
      Actions.Add(_action);
    }
    
    /// <summary>
    /// Run this action.
    /// </summary>
    virtual public void Run() {
      _lock.Take();
      foreach(IAction action in Actions) action.Run();
      _lock.Release();
    }
    
    /// <summary>
    /// Add an action to the action event.
    /// </summary>
    public void Add(Action action) {
      _lock.Take();
      Actions.Add(new ActionSet(action));
      _lock.Release();
    }
    
    /// <summary>
    /// Add an IAction to the action event.
    /// </summary>
    public void Add(IAction action) {
      _lock.Take();
      Actions.Add(action);
      _lock.Release();
    }
    
    /// <summary>
    /// Clear all actions from this instance.
    /// </summary>
    public void Clear() {
      _lock.Take();
      Actions.Clear();
      _lock.Release();
    }
    
    static public ActionSafeEvent operator +(ActionSafeEvent ev, Action action) {
      ev._lock.Take();
      ev.Actions.Add(new ActionSet(action));
      ev._lock.Release();
      return ev;
    }
    
    static public ActionSafeEvent operator +(ActionSafeEvent ev, IAction action) {
      ev._lock.Take();
      ev.Actions.Add(action);
      ev._lock.Release();
      return ev;
    }
    
    static public ActionSafeEvent operator -(ActionSafeEvent ev, Action _action) {
      ev._lock.Take();
      ev.Actions.RemoveQuick(new ActionSet(_action));
      ev._lock.Release();
      return ev;
    }
    
    static public ActionSafeEvent operator -(ActionSafeEvent ev, IAction _action) {
      ev._lock.Take();
      ev.Actions.RemoveQuick(_action);
      ev._lock.Release();
      return ev;
    }
    
    static public implicit operator ActionSafeEvent(Action _action) {
      return new ActionSafeEvent(_action);
    }
    
    public override string ToString() {
      return "[ActionSafeEvent Actions="+Actions+"]";
    }
    
    //-------------------------------------------//
    
  }
  
  public class ActionSafeEvent<A> : IAction, IAction<A> {
    
    //-------------------------------------------//
    
    public ArrayRig<IAction<A>> Actions = new ArrayRig<IAction<A>>();
    
    public A ArgA {get;set;}
    
    //-------------------------------------------//
    
    /// <summary>
    /// Lock used for external access to the action.
    /// </summary>
    protected Lock _lock = new Lock();
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionSafeEvent() { }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionSafeEvent(Action<A> action) {
      Actions.Add(new ActionSet<A>(action));
    }
    
    /// <summary>
    /// Run this action.
    /// </summary>
    virtual public void Run() {
      _lock.Take();
      foreach(IAction<A> action in Actions) {
        action.ArgA = ArgA;
        action.Run();
      }
      _lock.Release();
    }
    
    /// <summary>
    /// Add an action to the action event.
    /// </summary>
    public void Add(Action<A> action) {
      _lock.Take();
      Actions.Add(new ActionSet<A>(action));
      _lock.Release();
    }
    
    /// <summary>
    /// Add an IAction to the action event.
    /// </summary>
    public void Add(IAction<A> action) {
      _lock.Take();
      Actions.Add(action);
      _lock.Release();
    }
    
    /// <summary>
    /// Clear all actions from this instance.
    /// </summary>
    public void Clear() {
      _lock.Take();
      Actions.Clear();
      _lock.Release();
    }
    
    static public ActionSafeEvent<A> operator +(ActionSafeEvent<A> ev, Action<A> action) {
      ev._lock.Take();
      ev.Actions.Add(new ActionSet<A>(action));
      ev._lock.Release();
      return ev;
    }
    
    static public ActionSafeEvent<A> operator +(ActionSafeEvent<A> ev, IAction<A> action) {
      ev._lock.Take();
      ev.Actions.Add(action);
      ev._lock.Release();
      return ev;
    }
    
    static public ActionSafeEvent<A> operator -(ActionSafeEvent<A> ev, Action<A> _action) {
      ev._lock.Take();
      ev.Actions.RemoveQuick(new ActionSet<A>(_action));
      ev._lock.Release();
      return ev;
    }
    
    static public ActionSafeEvent<A> operator -(ActionSafeEvent<A> ev, IAction<A> _action) {
      ev._lock.Take();
      ev.Actions.Add(_action);
      ev._lock.Release();
      return ev;
    }
    
    static public implicit operator ActionSafeEvent<A>(Action<A> _action) {
      return new ActionSafeEvent<A>(_action);
    }
    
    public override string ToString() {
      return "[ActionSafeEvent Actions="+Actions+", ArgA="+ArgA+"]";
    }
    
    //-------------------------------------------//
    
  }

  public class ActionSafeEvent<A,B> : IAction<A,B> {
    
    //-------------------------------------------//
    
    public ArrayRig<IAction<A,B>> Actions = new ArrayRig<IAction<A, B>>();
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    
    //-------------------------------------------//
    
    /// <summary>
    /// Lock used for external access to the action.
    /// </summary>
    protected Lock _lock = new Lock();
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionSafeEvent() { }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionSafeEvent(Action<A,B> action) {
      Actions.Add(new ActionSet<A,B>(action));
    }
    
    /// <summary>
    /// Run this action.
    /// </summary>
    public virtual void Run() {
      _lock.Take();
      foreach(IAction<A,B> action in Actions) {
        action.ArgA = ArgA;
        action.ArgB = ArgB;
        action.Run();
      }
      _lock.Release();
    }
    
    /// <summary>
    /// Add an action to the action event.
    /// </summary>
    public void Add(Action<A,B> action) {
      _lock.Take();
      Actions.Add(new ActionSet<A,B>(action));
      _lock.Release();
    }
    
    /// <summary>
    /// Add an IAction to the action event.
    /// </summary>
    public void Add(IAction<A,B> action) {
      _lock.Take();
      Actions.Add(action);
      _lock.Release();
    }
    
    /// <summary>
    /// Clear all actions from this instance.
    /// </summary>
    public void Clear() {
      _lock.Take();
      Actions.Clear();
      _lock.Release();
    }
    
    static public ActionSafeEvent<A,B> operator +(ActionSafeEvent<A,B> ev, Action<A,B> _action) {
      ev._lock.Take();
      ev.Actions.Add(new ActionSet<A,B>(_action));
      ev._lock.Release();
      return ev;
    }
    
    static public ActionSafeEvent<A,B> operator +(ActionSafeEvent<A,B> ev, IAction<A,B> _action) {
      ev._lock.Take();
      ev.Actions.Add(_action);
      ev._lock.Release();
      return ev;
    }
    
    static public ActionSafeEvent<A,B> operator -(ActionSafeEvent<A,B> ev, Action<A,B> _action) {
      ev._lock.Take();
      ev.Actions.RemoveQuick(new ActionSet<A,B>(_action));
      ev._lock.Release();
      return ev;
    }
    
    static public ActionSafeEvent<A,B> operator -(ActionSafeEvent<A,B> ev, IAction<A,B> _action) {
      ev._lock.Take();
      ev.Actions.Add(_action);
      ev._lock.Release();
      return ev;
    }
    
    static public implicit operator ActionSafeEvent<A,B>(Action<A,B> _action) {
      return new ActionSafeEvent<A,B>(_action);
    }
    
    public override string ToString() {
      return "[ActionSafeEvent Actions="+Actions+", ArgA="+ArgA+", ArgB="+ArgB+"]";
    }
    
    //-------------------------------------------//
    
  }

  public class ActionSafeEvent<A,B,C> : IAction<A,B,C> {
    
    //-------------------------------------------//
    
    public ArrayRig<IAction<A,B,C>> Actions = new ArrayRig<IAction<A, B, C>>();
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    public C ArgC {get;set;}
    
    //-------------------------------------------//
    
    /// <summary>
    /// Lock used for external access to the action.
    /// </summary>
    protected Lock _lock = new Lock();
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionSafeEvent() {}
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionSafeEvent(Action<A,B,C> _action) {
      Actions.Add(new ActionSet<A,B,C>(_action));
    }
    
    /// <summary>
    /// Run this action.
    /// </summary>
    virtual public void Run() {
      _lock.Take();
      foreach(IAction<A,B,C> action in Actions) {
        action.ArgA = ArgA;
        action.ArgB = ArgB;
        action.ArgC = ArgC;
        action.Run();
      }
      _lock.Release();
    }
    
    /// <summary>
    /// Add an action to the action event.
    /// </summary>
    public void Add(Action<A,B,C> action) {
      _lock.Take();
      Actions.Add(new ActionSet<A,B,C>(action));
      _lock.Release();
    }
    
    /// <summary>
    /// Add an IAction to the action event.
    /// </summary>
    public void Add(IAction<A,B,C> action) {
      _lock.Take();
      Actions.Add(action);
      _lock.Release();
    }
    
    /// <summary>
    /// Clear all actions from this instance.
    /// </summary>
    public void Clear() {
      _lock.Take();
      Actions.Clear();
      _lock.Release();
    }
    
    static public ActionSafeEvent<A,B,C> operator +(ActionSafeEvent<A,B,C> ev, Action<A,B,C> action) {
      ev._lock.Take();
      ev.Actions.Add(new ActionSet<A,B,C>(action));
      ev._lock.Release();
      return ev;
    }
    
    static public ActionSafeEvent<A,B,C> operator +(ActionSafeEvent<A,B,C> ev, IAction<A,B,C> action) {
      ev._lock.Take();
      ev.Actions.Add(action);
      ev._lock.Release();
      return ev;
    }
    
    static public ActionSafeEvent<A,B,C> operator -(ActionSafeEvent<A,B,C> ev, Action<A,B,C> _action) {
      ev._lock.Take();
      ev.Actions.RemoveQuick(new ActionSet<A,B,C>(_action));
      ev._lock.Release();
      return ev;
    }
    
    static public ActionSafeEvent<A,B,C> operator -(ActionSafeEvent<A,B,C> ev, IAction<A,B,C> action) {
      ev._lock.Take();
      ev.Actions.Add(action);
      ev._lock.Release();
      return ev;
    }
    
    static public implicit operator ActionSafeEvent<A,B,C>(Action<A,B,C> _action) {
      return new ActionSafeEvent<A,B,C>(_action);
    }
    
    public override string ToString() {
      return "[ActionSafeEvent Actions="+Actions+", ArgA="+ArgA+", ArgB="+ArgB+", ArgC="+ArgC+"]";
    }
    
    //-------------------------------------------//
    
  }

  public class ActionSafeEvent<A,B,C,D> : IAction<A,B,C,D> {
    
    //-------------------------------------------//
    
    public ArrayRig<IAction<A,B,C,D>> Actions = new ArrayRig<IAction<A, B, C, D>>();
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    public C ArgC {get;set;}
    public D ArgD {get;set;}
    
    //-------------------------------------------//
    
    /// <summary>
    /// Lock used for external access to the action.
    /// </summary>
    protected Lock _lock = new Lock();
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionSafeEvent() { }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionSafeEvent(Action<A,B,C,D> _action) {
      Actions.Add(new ActionSet<A,B,C,D>(_action));
    }
    
    /// <summary>
    /// Run this action.
    /// </summary>
    virtual public void Run() {
      _lock.Take();
      foreach(IAction<A,B,C,D> action in Actions) {
        action.ArgA = ArgA;
        action.ArgB = ArgB;
        action.ArgC = ArgC;
        action.ArgD = ArgD;
        action.Run();
      }
      _lock.Release();
    }
    
    /// <summary>
    /// Add an action to the action event.
    /// </summary>
    public void Add(Action<A,B,C,D> action) {
      _lock.Take();
      Actions.Add(new ActionSet<A,B,C,D>(action));
      _lock.Release();
    }
    
    /// <summary>
    /// Add an IAction to the action event.
    /// </summary>
    public void Add(IAction<A,B,C,D> action) {
      _lock.Take();
      Actions.Add(action);
      _lock.Release();
    }
    
    /// <summary>
    /// Clear all actions from this instance.
    /// </summary>
    public void Clear() {
      Actions.Clear();
    }
    
    static public ActionSafeEvent<A,B,C,D> operator +(ActionSafeEvent<A,B,C,D> ev, Action<A,B,C,D> action) {
      ev._lock.Take();
      ev.Actions.Add(new ActionSet<A,B,C,D>(action));
      ev._lock.Release();
      return ev;
    }
    
    static public ActionSafeEvent<A,B,C,D> operator +(ActionSafeEvent<A,B,C,D> ev, IAction<A,B,C,D> action) {
      ev._lock.Take();
      ev.Actions.Add(action);
      ev._lock.Release();
      return ev;
    }
    
    static public ActionSafeEvent<A,B,C,D> operator -(ActionSafeEvent<A,B,C,D> ev, Action<A,B,C,D> action) {
      ev._lock.Take();
      ev.Actions.RemoveQuick(new ActionSet<A,B,C,D>(action));
      ev._lock.Release();
      return ev;
    }
    
    static public ActionSafeEvent<A,B,C,D> operator -(ActionSafeEvent<A,B,C,D> ev, IAction<A,B,C,D> action) {
      ev._lock.Take();
      ev.Actions.RemoveQuick(action);
      ev._lock.Release();
      return ev;
    }
    
    static public implicit operator ActionSafeEvent<A,B,C,D>(Action<A,B,C,D> _action) {
      return new ActionSafeEvent<A,B,C,D>(_action);
    }
    
    public override string ToString() {
      return "[ActionSafeEvent Actions="+Actions+", ArgA="+ArgA+", ArgB="+ArgB+", ArgC="+ArgC+", ArgD="+ArgD+"]";
    }
    
    //-------------------------------------------//
    
  }
  
  public class ActionSafeEvent<A,B,C,D,E> : IAction<A,B,C,D,E> {
    
    //-------------------------------------------//
    
    public ArrayRig<IAction<A,B,C,D,E>> Actions = new ArrayRig<IAction<A, B, C, D, E>>();
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    public C ArgC {get;set;}
    public D ArgD {get;set;}
    public E ArgE {get;set;}
    
    //-------------------------------------------//
    
    /// <summary>
    /// Lock used for external access to the action.
    /// </summary>
    protected Lock _lock = new Lock();
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionSafeEvent() { }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionSafeEvent(Action<A,B,C,D,E> action) {
      Actions.Add(new ActionSet<A,B,C,D,E>(action));
    }
    
    /// <summary>
    /// Run this action.
    /// </summary>
    virtual public void Run() {
      _lock.Take();
      foreach(IAction<A,B,C,D,E> action in Actions) {
        action.ArgA = ArgA;
        action.ArgB = ArgB;
        action.ArgC = ArgC;
        action.ArgD = ArgD;
        action.ArgE = ArgE;
        action.Run();
      }
      _lock.Release();
    }
    
    /// <summary>
    /// Add an action to the action event.
    /// </summary>
    public void Add(Action<A,B,C,D,E> action) {
      _lock.Take();
      Actions.Add(new ActionSet<A,B,C,D,E>(action));
      _lock.Release();
    }
    
    /// <summary>
    /// Add an IAction to the action event.
    /// </summary>
    public void Add(IAction<A,B,C,D,E> action) {
      _lock.Take();
      Actions.Add(action);
      _lock.Release();
    }
    
    /// <summary>
    /// Clear all actions from this instance.
    /// </summary>
    public void Clear() {
      _lock.Take();
      Actions.Clear();
      _lock.Release();
    }
    
    static public ActionSafeEvent<A,B,C,D,E> operator +(ActionSafeEvent<A,B,C,D,E> ev, Action<A,B,C,D,E> action) {
      ev._lock.Take();
      ev.Actions.Add(new ActionSet<A,B,C,D,E>(action));
      ev._lock.Release();
      return ev;
    }
    
    static public ActionSafeEvent<A,B,C,D,E> operator +(ActionSafeEvent<A,B,C,D,E> ev, IAction<A,B,C,D,E> action) {
      ev._lock.Take();
      ev.Actions.Add(action);
      ev._lock.Release();
      return ev;
    }
    
    static public ActionSafeEvent<A,B,C,D,E> operator -(ActionSafeEvent<A,B,C,D,E> ev, Action<A,B,C,D,E> action) {
      ev._lock.Take();
      ev.Actions.RemoveQuick(new ActionSet<A,B,C,D,E>(action));
      ev._lock.Release();
      return ev;
    }
    
    static public ActionSafeEvent<A,B,C,D,E> operator -(ActionSafeEvent<A,B,C,D,E> ev, IAction<A,B,C,D,E> action) {
      ev._lock.Take();
      ev.Actions.RemoveQuick(action);
      ev._lock.Release();
      return ev;
    }
    
    static public implicit operator ActionSafeEvent<A,B,C,D,E>(Action<A,B,C,D,E> _action) {
      return new ActionSafeEvent<A,B,C,D,E>(_action);
    }
    
    public override string ToString() {
      return "[ActionSafeEvent Actions="+Actions+", ArgA="+ArgA+", ArgB="+ArgB+", ArgC="+ArgC+", ArgD="+ArgD+", ArgE="+ArgE+"]";
    }
    
    //-------------------------------------------//
    
  }


}
