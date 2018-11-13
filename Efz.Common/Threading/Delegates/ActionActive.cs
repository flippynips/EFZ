using System;
using System.Linq;

namespace Efz {
  
  /// <summary>
  /// Simple helper to prevent null reference calls if the action isn't set.
  /// </summary>
  public class ActionActive : IAction {
    
    //-------------------------------------------//
    
    public IAction Action {
      get {
        return action;
      }
      set {
        action = value;
        set = action != null;
      }
    }
    
    public bool set;
    
    //-------------------------------------------//
    
    protected IAction action;
    
    //-------------------------------------------//
    
    public ActionActive() {}
    public ActionActive(Action _action) {
      Action = new ActionSet(_action);
    }
    public ActionActive(IAction _action) {
      Action = _action;
    }
    
    public void Run() {
      if(set) {
        action.Run();
      }
    }

    static public implicit operator ActionActive(Action _action) {
      return new ActionActive(_action);
    }
    
    static public implicit operator Action(ActionActive _actionActive) {
      return _actionActive.Run;
    }
    
    public override string ToString() {
      return string.Format("[ActionActive Set={0}, Action={1}]", set, action);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Simple structure around an action to prevent null reference calls if the action isn't set.
  /// </summary>
  public class ActionActive<A> : IAction<A> {
    
    //-------------------------------------------//
    
    public IAction<A> Action {
      get {
        return action;
      }
      set {
        action = value;
        set = action != null;
      }
    }
    
    public A ArgA {get;set;}
    
    public bool set;
    
    //-------------------------------------------//
    
    protected IAction<A> action;
    
    //-------------------------------------------//
    
    public ActionActive() {}
    public ActionActive(Action<A> _action) {
      Action = new ActionSet<A>(_action);
    }
    public ActionActive(IAction<A> _action) {
      Action = _action;
    }
    
    public void Run() {
      if(set) {
        action.Run();
      }
    }
    
    public void Run(A _a) {
      if(set) {
        action.ArgA = _a;
        action.Run();
      }
    }
    
    static public implicit operator ActionActive<A>(Action<A> _action) {
      return new ActionActive<A>(_action);
    }
    
    public override string ToString() {
      return string.Format("[ActionActive Set={0}, Action={1}, ArgA={2}]", set, action, ArgA);
    }
    
    //-------------------------------------------//

  }
  
  /// <summary>
  /// Simple structure around an action to prevent null reference calls if the action isn't set.
  /// </summary>
  public class ActionActive<A,B> : IAction<A,B> {
    
    //-------------------------------------------//
    
    public IAction<A,B> Action {
      get {
        return action;
      }
      set {
        action = value;
        set = action != null;
      }
    }
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    
    public bool set;
    
    //-------------------------------------------//
    
    protected IAction<A, B> action;
    
    //-------------------------------------------//
    
    public ActionActive() {}
    public ActionActive(Action<A,B> _action) {
      Action = new ActionSet<A,B>(_action);
    }
    public ActionActive(IAction<A,B> _action) {
      Action = _action;
    }
    
    public void Run() {
      if(set) {
        action.Run();
      }
    }
    
    public void Run(A _a, B _b) {
      if(set) {
        action.ArgA = _a;
        action.ArgB = _b;
        action.Run();
      }
    }
    
    static public implicit operator ActionActive<A,B>(Action<A,B> _action) {
      return new ActionActive<A,B>(_action);
    }
    
    public override string ToString() {
      return string.Format("[ActionActive Set={0}, Action={1}, ArgA={2}, ArgB={3}]", set, action, ArgA, ArgB);
    }
    
    //-------------------------------------------//

  }
  
  /// <summary>
  /// Simple structure around an action to prevent null reference calls if the action isn't set.
  /// </summary>
  public class ActionActive<A,B,C> : IAction<A,B,C> {
    
    //-------------------------------------------//
    
    public IAction<A,B,C> Action {
      get {
        return action;
      }
      set {
        action = value;
        set = action != null;
      }
    }
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    public C ArgC {get;set;}
    
    public bool set;
    
    //-------------------------------------------//
    
    protected IAction<A, B, C> action;
    
    //-------------------------------------------//
    
    public ActionActive() {}
    public ActionActive(Action<A,B,C> _action) {
      Action = new ActionSet<A,B,C>(_action);
    }
    public ActionActive(IAction<A,B,C> _action) {
      Action = _action;
    }
    
    public void Run() {
      if(set) {
        action.Run();
      }
    }
    
    public void Run(A _a, B _b, C _c) {
      if(set) {
        action.ArgA = _a;
        action.ArgB = _b;
        action.ArgC = _c;
        action.Run();
      }
    }
    
    static public implicit operator ActionActive<A,B,C>(Action<A,B,C> _action) {
      return new ActionActive<A,B,C>(_action);
    }
    
    public override string ToString() {
      return string.Format("[ActionActive Set={0}, Action={1}, ArgA={2}, ArgB={3}, ArgC={4}]", set, action, ArgA, ArgB, ArgC);
    }
    
    //-------------------------------------------//

  }
  
  /// <summary>
  /// Simple structure around an action to prevent null reference calls if the action isn't set.
  /// </summary>
  public class ActionActive<A,B,C,D> : IAction<A,B,C,D> {
    
    //-------------------------------------------//
    
    public IAction<A,B,C,D> Action {
      get {
        return action;
      }
      set {
        action = value;
        set = action != null;
      }
    }
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    public C ArgC {get;set;}
    public D ArgD {get;set;}
    
    public bool set;
    
    //-------------------------------------------//
    
    private IAction<A, B, C, D> action;
    
    //-------------------------------------------//
    
    public ActionActive() {}
    public ActionActive(Action<A,B,C,D> _action) {
      Action = new ActionSet<A,B,C,D>(_action);
    }
    public ActionActive(IAction<A, B, C, D> _action) {
      Action = _action;
    }
    
    public void Run() {
      if(set) {
        action.Run();
      }
    }
    
    public void Run(A _a, B _b, C _c, D _d) {
      if(set) {
        action.ArgA = _a;
        action.ArgB = _b;
        action.ArgC = _c;
        action.ArgD = _d;
        action.Run();
      }
    }
    
    static public implicit operator ActionActive<A,B,C,D>(Action<A,B,C,D> _action) {
      return new ActionActive<A,B,C,D>(_action);
    }
    
    public override string ToString() {
      return string.Format("[ActionActive Set={0}, Action={1}, ArgA={2}, ArgB={3}, ArgC={4}, ArgD={5}]", set, action, ArgA, ArgB, ArgC, ArgD);
    }
    
    //-------------------------------------------//

  }
  
  /// <summary>
  /// Simple structure around an action to prevent null reference calls if the action isn't set.
  /// </summary>
  public class ActionActive<A,B,C,D,E> : IAction<A,B,C,D,E> {
    
    //-------------------------------------------//
    
    public IAction<A,B,C,D,E> Action {
      get {
        return action;
      }
      set {
        action = value;
        set = action != null;
      }
    }
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    public C ArgC {get;set;}
    public D ArgD {get;set;}
    public E ArgE {get;set;}
    
    public bool set;
    
    //-------------------------------------------//
    
    private IAction<A,B,C,D,E> action;
    
    //-------------------------------------------//
    
    public ActionActive() {}
    public ActionActive(Action<A,B,C,D,E> _action) {
      Action = new ActionSet<A,B,C,D,E>(_action);
    }
    public ActionActive(IAction<A,B,C,D,E> _action) {
      Action = _action;
    }
    
    public void Run() {
      if(set) {
        action.Run();
      }
    }
    
    public void Run(A _a, B _b, C _c, D _d, E _e) {
      if(set) {
        action.ArgA = _a;
        action.ArgB = _b;
        action.ArgC = _c;
        action.ArgD = _d;
        action.ArgE = _e;
        action.Run();
      }
    }
    
    static public implicit operator ActionActive<A,B,C,D,E>(Action<A,B,C,D,E> _action) {
      return new ActionActive<A,B,C,D,E>(_action);
    }
    
    public override string ToString() {
      return string.Format("[ActionActive Set={0}, Action={1}, ArgA={2}, ArgB={3}, ArgC={4}, ArgD={5}, ArgE={6}]", set, action, ArgA, ArgB, ArgC, ArgD, ArgE);
    }

    //-------------------------------------------//

  }

}
