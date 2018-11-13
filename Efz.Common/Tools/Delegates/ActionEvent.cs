using System;
using Efz.Collections;

namespace Efz {
  
  public class ActionEvent : IAction {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Collection of actions that will be run.
    /// </summary>
    public ArrayRig<IAction> Actions = new ArrayRig<IAction>();
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionEvent() {}
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionEvent(Action action) {
      Actions.Add(new ActionSet(action));
    }
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionEvent(IAction _action) {
      Actions.Add(_action);
    }
    
    /// <summary>
    /// Run this action.
    /// </summary>
    virtual public void Run() {
      foreach(IAction action in Actions) {
        action.Run();
      }
    }
    
    /// <summary>
    /// Add an action to the action event.
    /// </summary>
    public void Add(Action action) {
      Actions.Add(new ActionSet(action));
    }
    
    /// <summary>
    /// Add an IAction to the action event.
    /// </summary>
    public void Add(IAction action) {
      Actions.Add(action);
    }
    
    /// <summary>
    /// Clear all actions from this instance.
    /// </summary>
    public void Clear() {
      Actions.Clear();
    }
    
    static public ActionEvent operator +(ActionEvent ev, Action action) {
      ev.Actions.Add(new ActionSet(action));
      return ev;
    }
    
    static public ActionEvent operator +(ActionEvent ev, IAction action) {
      ev.Actions.Add(action);
      return ev;
    }
    
    static public ActionEvent operator -(ActionEvent ev, Action action) {
      ev.Actions.Remove(new ActionSet(action));
      return ev;
    }
    
    static public ActionEvent operator -(ActionEvent ev, IAction action) {
      ev.Actions.Remove(action);
      return ev;
    }
    
    static public implicit operator ActionEvent(Action action) {
      return new ActionEvent(action);
    }
    
    public override string ToString() {
      return "[ActionEvent Actions="+Actions+"]";
    }
    
    //-------------------------------------------//
    
  }
  
  public class ActionEvent<A> : IAction, IAction<A> {
    
    //-------------------------------------------//
    
    public ArrayRig<IAction<A>> Actions = new ArrayRig<IAction<A>>();
    
    public A ArgA {get;set;}
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionEvent() { }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionEvent(Action<A> action) {
      Actions.Add(new ActionSet<A>(action));
    }
    
    /// <summary>
    /// Run this action.
    /// </summary>
    virtual public void Run() {
      bool setA = !Equals(ArgA, default(A));
      foreach(IAction<A> action in Actions) {
        if(setA) action.ArgA = ArgA;
        action.Run();
      }
    }
    
    /// <summary>
    /// Add an action to the action event.
    /// </summary>
    public void Add(Action<A> action) {
      Actions.Add(new ActionSet<A>(action));
    }
    
    /// <summary>
    /// Add an IAction to the action event.
    /// </summary>
    public void Add(IAction<A> action) {
      Actions.Add(action);
    }
    
    /// <summary>
    /// Clear all actions from this instance.
    /// </summary>
    public void Clear() {
      Actions.Clear();
    }
    
    static public ActionEvent<A> operator +(ActionEvent<A> ev, Action<A> action) {
      ev.Actions.Add(new ActionSet<A>(action));
      return ev;
    }
    
    static public ActionEvent<A> operator +(ActionEvent<A> ev, IAction<A> action) {
      ev.Actions.Add(action);
      return ev;
    }
    
    static public ActionEvent<A> operator -(ActionEvent<A> ev, Action<A> action) {
      ev.Actions.Remove(new ActionSet<A>(action));
      return ev;
    }
    
    static public ActionEvent<A> operator -(ActionEvent<A> ev, IAction<A> action) {
      ev.Actions.Add(action);
      return ev;
    }
    
    static public implicit operator ActionEvent<A>(Action<A> action) {
      return new ActionEvent<A>(action);
    }
    
    public override string ToString() {
      return "[ActionEvent Actions="+Actions+", ArgA="+ArgA+"]";
    }
    
    //-------------------------------------------//
    
  }

  public class ActionEvent<A,B> : IAction<A,B> {
    
    //-------------------------------------------//
    
    public ArrayRig<IAction<A,B>> Actions = new ArrayRig<IAction<A, B>>();
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionEvent() { }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionEvent(Action<A,B> _action) {
      Actions.Add(new ActionSet<A,B>(_action));
    }
    
    /// <summary>
    /// Run this action.
    /// </summary>
    public virtual void Run() {
      bool setA = !Equals(ArgA, default(A));
      bool setB = !Equals(ArgB, default(B));
      foreach(IAction<A,B> action in Actions) {
        if(setA) action.ArgA = ArgA;
        if(setB) action.ArgB = ArgB;
        action.Run();
      }
    }
    
    /// <summary>
    /// Add an action to the action event.
    /// </summary>
    public void Add(Action<A,B> action) {
      Actions.Add(new ActionSet<A,B>(action));
    }
    
    /// <summary>
    /// Add an IAction to the action event.
    /// </summary>
    public void Add(IAction<A,B> action) {
      Actions.Add(action);
    }
    
    /// <summary>
    /// Clear all actions from this instance.
    /// </summary>
    public void Clear() {
      Actions.Clear();
    }
    
    static public ActionEvent<A,B> operator +(ActionEvent<A,B> ev, Action<A,B> action) {
      ev.Actions.Add(new ActionSet<A,B>(action));
      return ev;
    }
    
    static public ActionEvent<A,B> operator +(ActionEvent<A,B> ev, IAction<A,B> action) {
      ev.Actions.Add(action);
      return ev;
    }
    
    static public ActionEvent<A,B> operator -(ActionEvent<A,B> ev, Action<A,B> action) {
      ev.Actions.Remove(new ActionSet<A,B>(action));
      return ev;
    }
    
    static public ActionEvent<A,B> operator -(ActionEvent<A,B> ev, IAction<A,B> action) {
      ev.Actions.Add(action);
      return ev;
    }
    
    static public implicit operator ActionEvent<A,B>(Action<A,B> action) {
      return new ActionEvent<A,B>(action);
    }
    
    public override string ToString() {
      return "[ActionEvent Actions="+Actions+", ArgA="+ArgA+", ArgB="+ArgB+"]";
    }
    
    //-------------------------------------------//
    
  }

  public class ActionEvent<A,B,C> : IAction<A,B,C> {
    
    //-------------------------------------------//
    
    public ArrayRig<IAction<A,B,C>> Actions = new ArrayRig<IAction<A, B, C>>();
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    public C ArgC {get;set;}
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionEvent() {}
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionEvent(Action<A,B,C> action) {
      Actions.Add(new ActionSet<A,B,C>(action));
    }
    
    /// <summary>
    /// Run this action.
    /// </summary>
    virtual public void Run() {
      bool setA = !Equals(ArgA, default(A));
      bool setB = !Equals(ArgB, default(B));
      bool setC = !Equals(ArgC, default(C));
      foreach(IAction<A,B,C> action in Actions) {
        if(setA) action.ArgA = ArgA;
        if(setB) action.ArgB = ArgB;
        if(setC) action.ArgC = ArgC;
        action.Run();
      }
    }
    
    /// <summary>
    /// Add an action to the action event.
    /// </summary>
    public void Add(Action<A,B,C> action) {
      Actions.Add(new ActionSet<A,B,C>(action));
    }
    
    /// <summary>
    /// Add an IAction to the action event.
    /// </summary>
    public void Add(IAction<A,B,C> action) {
      Actions.Add(action);
    }
    
    /// <summary>
    /// Clear all actions from this instance.
    /// </summary>
    public void Clear() {
      Actions.Clear();
    }
    
    static public ActionEvent<A,B,C> operator +(ActionEvent<A,B,C> ev, Action<A,B,C> action) {
      ev.Actions.Add(new ActionSet<A,B,C>(action));
      return ev;
    }
    
    static public ActionEvent<A,B,C> operator +(ActionEvent<A,B,C> ev, IAction<A,B,C> action) {
      ev.Actions.Add(action);
      return ev;
    }
    
    static public ActionEvent<A,B,C> operator -(ActionEvent<A,B,C> ev, Action<A,B,C> action) {
      ev.Actions.Remove(new ActionSet<A,B,C>(action));
      return ev;
    }
    
    static public ActionEvent<A,B,C> operator -(ActionEvent<A,B,C> ev, IAction<A,B,C> action) {
      ev.Actions.Add(action);
      return ev;
    }
    
    static public implicit operator ActionEvent<A,B,C>(Action<A,B,C> action) {
      return new ActionEvent<A,B,C>(action);
    }
    
    public override string ToString() {
      return "[ActionEvent Actions="+Actions+", ArgA="+ArgA+", ArgB="+ArgB+", ArgC="+ArgC+"]";
    }
    
    //-------------------------------------------//
    
  }

  public class ActionEvent<A,B,C,D> : IAction<A,B,C,D> {
    
    //-------------------------------------------//
    
    public ArrayRig<IAction<A,B,C,D>> Actions = new ArrayRig<IAction<A, B, C, D>>();
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    public C ArgC {get;set;}
    public D ArgD {get;set;}
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionEvent() { }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionEvent(Action<A,B,C,D> action) {
      Actions.Add(new ActionSet<A,B,C,D>(action));
    }
    
    /// <summary>
    /// Run this action.
    /// </summary>
    virtual public void Run() {
      bool setA = !Equals(ArgA, default(A));
      bool setB = !Equals(ArgB, default(B));
      bool setC = !Equals(ArgC, default(C));
      bool setD = !Equals(ArgD, default(D));
      foreach(IAction<A,B,C,D> action in Actions) {
        if(setA) action.ArgA = ArgA;
        if(setB) action.ArgB = ArgB;
        if(setC) action.ArgC = ArgC;
        if(setD) action.ArgD = ArgD;
        action.Run();
      }
    }
    
    /// <summary>
    /// Add an action to the action event.
    /// </summary>
    public void Add(Action<A,B,C,D> action) {
      Actions.Add(new ActionSet<A,B,C,D>(action));
    }
    
    /// <summary>
    /// Add an IAction to the action event.
    /// </summary>
    public void Add(IAction<A,B,C,D> action) {
      Actions.Add(action);
    }
    
    /// <summary>
    /// Clear all actions from this instance.
    /// </summary>
    public void Clear() {
      Actions.Clear();
    }
    
    static public ActionEvent<A,B,C,D> operator +(ActionEvent<A,B,C,D> ev, Action<A,B,C,D> action) {
      ev.Actions.Add(new ActionSet<A,B,C,D>(action));
      return ev;
    }
    
    static public ActionEvent<A,B,C,D> operator +(ActionEvent<A,B,C,D> ev, IAction<A,B,C,D> action) {
      ev.Actions.Add(action);
      return ev;
    }
    
    static public ActionEvent<A,B,C,D> operator -(ActionEvent<A,B,C,D> ev, Action<A,B,C,D> action) {
      ev.Actions.Remove(new ActionSet<A,B,C,D>(action));
      return ev;
    }
    
    static public ActionEvent<A,B,C,D> operator -(ActionEvent<A,B,C,D> ev, IAction<A,B,C,D> action) {
      ev.Actions.Remove(action);
      return ev;
    }
    
    static public implicit operator ActionEvent<A,B,C,D>(Action<A,B,C,D> action) {
      return new ActionEvent<A,B,C,D>(action);
    }
    
    public override string ToString() {
      return "[ActionEvent Actions="+Actions+", ArgA="+ArgA+", ArgB="+ArgB+", ArgC="+ArgC+", ArgD="+ArgD+"]";
    }
    
    //-------------------------------------------//
    
  }
  
  public class ActionEvent<A,B,C,D,E> : IAction<A,B,C,D,E> {
    
    //-------------------------------------------//
    
    public ArrayRig<IAction<A,B,C,D,E>> Actions = new ArrayRig<IAction<A, B, C, D, E>>();
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    public C ArgC {get;set;}
    public D ArgD {get;set;}
    public E ArgE {get;set;}
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionEvent() { }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionEvent(Action<A,B,C,D,E> _action) {
      Actions.Add(new ActionSet<A,B,C,D,E>(_action));
    }
    
    /// <summary>
    /// Run this action.
    /// </summary>
    virtual public void Run() {
      bool setA = !Equals(ArgA, default(A));
      bool setB = !Equals(ArgB, default(B));
      bool setC = !Equals(ArgC, default(C));
      bool setD = !Equals(ArgD, default(D));
      bool setE = !Equals(ArgE, default(E));
      foreach(IAction<A,B,C,D,E> action in Actions) {
        if(setA) action.ArgA = ArgA;
        if(setB) action.ArgB = ArgB;
        if(setC) action.ArgC = ArgC;
        if(setD) action.ArgD = ArgD;
        if(setE) action.ArgE = ArgE;
        action.Run();
      }
    }
    
    /// <summary>
    /// Add an action to the action event.
    /// </summary>
    public void Add(Action<A,B,C,D,E> action) {
      Actions.Add(new ActionSet<A,B,C,D,E>(action));
    }
    
    /// <summary>
    /// Add an IAction to the action event.
    /// </summary>
    public void Add(IAction<A,B,C,D,E> action) {
      Actions.Add(action);
    }
    
    /// <summary>
    /// Clear all actions from this instance.
    /// </summary>
    public void Clear() {
      Actions.Clear();
    }
    
    static public ActionEvent<A,B,C,D,E> operator +(ActionEvent<A,B,C,D,E> ev, Action<A,B,C,D,E> action) {
      ev.Actions.Add(new ActionSet<A,B,C,D,E>(action));
      return ev;
    }
    
    static public ActionEvent<A,B,C,D,E> operator +(ActionEvent<A,B,C,D,E> ev, IAction<A,B,C,D,E> action) {
      ev.Actions.Add(action);
      return ev;
    }
    
    static public ActionEvent<A,B,C,D,E> operator -(ActionEvent<A,B,C,D,E> ev, Action<A,B,C,D,E> action) {
      ev.Actions.Remove(new ActionSet<A,B,C,D,E>(action));
      return ev;
    }
    
    static public ActionEvent<A,B,C,D,E> operator -(ActionEvent<A,B,C,D,E> ev, IAction<A,B,C,D,E> action) {
      ev.Actions.Remove(action);
      return ev;
    }
    
    static public implicit operator ActionEvent<A,B,C,D,E>(Action<A,B,C,D,E> action) {
      return new ActionEvent<A,B,C,D,E>(action);
    }
    
    public override string ToString() {
      return "[ActionEvent Actions="+Actions+", ArgA="+ArgA+", ArgB="+ArgB+", ArgC="+ArgC+", ArgD="+ArgD+", ArgE="+ArgE+"]";
    }
    
    //-------------------------------------------//
    
  }


}
