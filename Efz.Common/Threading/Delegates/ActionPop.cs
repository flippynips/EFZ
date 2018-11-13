using System;

using Efz.Threading;

namespace Efz {
  
  /// <summary>
  /// Threadsafe action wrapper that uses the latest set of arguments when the action
  /// is run.
  /// </summary>
  public class ActionPop : IRun, IAction {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Needle used to run the action.
    /// </summary>
    public Needle Needle;
    
    /// <summary>
    /// Action that is run on the specified needle.
    /// </summary>
    public IAction Action {
      get { return _action.Action; }
      set { _action.Action = value; }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Inner action roll instance.
    /// </summary>
    protected ActionRoll _action;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionPop() {
      _action = new ActionRoll();
      Needle = ManagerUpdate.Control;
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionPop(IAction action, Needle needle = null) {
      _action = new ActionRoll(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionPop(Action action, Needle needle = null) {
      _action = new ActionRoll(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    
    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      Needle.AddSingle(_action);
    }
    
    public override string ToString() {
      return "[ActionRoll Action="+_action+"]";
    }
    
    //-------------------------------------------//

  }
  
  /// <summary>
  /// Threadsafe action wrapper that uses the latest set of arguments when the action
  /// is run.
  /// </summary>
  public class ActionPop<A> : IRun, IAction, IAction<A> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Needle used to run the action.
    /// </summary>
    public Needle Needle;
    
    /// <summary>
    /// Action that is run on the specified needle.
    /// </summary>
    public IAction<A> Action {
      get { return _action.Action; }
      set { _action.Action = value; }
    }
    
    public A ArgA {get;set;}
    
    //-------------------------------------------//
    
    /// <summary>
    /// Inner action roll instance.
    /// </summary>
    protected ActionRoll<A> _action;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionPop() {
      _action = new ActionRoll<A>();
      Needle = ManagerUpdate.Control;
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionPop(IAction<A> action, Needle needle = null) {
      _action = new ActionRoll<A>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    
    public ActionPop(Action<A> action, Needle needle = null) {
      _action = new ActionRoll<A>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    
    public ActionPop(Action<A> action, A a) :
      this(new ActionSet<A>(action), a) {
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionPop(IAction<A> action, A a) {
      _action = new ActionRoll<A>(action);
      ArgA = a;
    }
    
    /// <summary>
    /// Add the specified parameter and run.
    /// </summary>
    public void Run(A a) {
      _action.Add(a);
      Needle.AddSingle(_action);
    }
    
    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      _action.Add(ArgA);
      Needle.AddSingle(_action);
    }
    
    public override string ToString() {
      return "[ActionRoll Action="+_action+",ArgA="+ArgA+"]";
    }
    
    //-------------------------------------------//

  }

  /// <summary>
  /// Threadsafe action wrapper that uses the latest set of arguments when the action
  /// is run.
  /// </summary>
  public class ActionPop<A,B> : IRun, IAction, IAction<A>, IAction<A,B> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Needle used to run the action.
    /// </summary>
    public Needle Needle;
    
    /// <summary>
    /// Action that is run on the specified needle.
    /// </summary>
    public IAction<A,B> Action {
      get { return _action.Action; }
      set { _action.Action = value; }
    }
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    
    //-------------------------------------------//
    
    /// <summary>
    /// Inner action roll instance.
    /// </summary>
    protected ActionRoll<A,B> _action;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionPop() {
      _action = new ActionRoll<A,B>();
      Needle = ManagerUpdate.Control;
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionPop(IAction<A,B> action, Needle needle = null) {
      _action = new ActionRoll<A,B>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    
    public ActionPop(Action<A,B> action, Needle needle = null) {
      _action = new ActionRoll<A,B>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    
    public ActionPop(Action<A,B> action, A a, B b = default(B)) :
      this(new ActionSet<A,B>(action), a, b) {
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionPop(IAction<A,B> action, A a, B b = default(B)) {
      _action = new ActionRoll<A,B>(action);
      ArgA = a;
      ArgB = b;
    }
    
    /// <summary>
    /// Add the specified parameter and run.
    /// </summary>
    public void Run(A a, B b) {
      _action.Add(a, b);
      Needle.AddSingle(_action);
    }
    
    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      _action.Add(ArgA, ArgB);
      Needle.AddSingle(_action);
    }
    
    public override string ToString() {
      return "[ActionRoll Action="+_action+",ArgA="+ArgA+", ArgB="+ArgB+"]";
    }
    
    //-------------------------------------------//

  }

  /// <summary>
  /// Threadsafe action wrapper that uses the latest set of arguments when the action
  /// is run.
  /// </summary>
  public class ActionPop<A,B,C> : IRun, IAction, IAction<A>, IAction<A,B>, IAction<A,B,C> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Needle used to run the action.
    /// </summary>
    public Needle Needle;
    
    /// <summary>
    /// Action that is run on the specified needle.
    /// </summary>
    public IAction<A,B,C> Action {
      get { return _action.Action; }
      set { _action.Action = value; }
    }
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    public C ArgC {get;set;}
    
    //-------------------------------------------//
    
    protected ActionRoll<A,B,C> _action;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionPop() {
      _action = new ActionRoll<A,B,C>();
      Needle = ManagerUpdate.Control;
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionPop(IAction<A,B,C> action, Needle needle = null) {
      _action = new ActionRoll<A,B,C>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    
    public ActionPop(Action<A,B,C> action, Needle needle = null) {
      _action = new ActionRoll<A,B,C>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    
    public ActionPop(Action<A,B,C> action, A a, B b = default(B), C c = default(C)) :
      this(new ActionSet<A,B,C>(action), a, b, c) {
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionPop(IAction<A,B,C> action, A a, B b = default(B), C c = default(C)) {
      _action = new ActionRoll<A,B,C>(action);
      ArgA = a;
      ArgB = b;
      ArgC = c;
    }
    
    /// <summary>
    /// Add the specified parameter and run.
    /// </summary>
    public void Run(A a, B b, C c) {
      _action.Add(a, b, c);
      Needle.AddSingle(_action);
    }
    
    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      _action.Add(ArgA, ArgB, ArgC);
      Needle.AddSingle(_action);
    }
    
    public override string ToString() {
      return "[ActionRoll Action="+_action+",ArgA="+ArgA+", ArgB="+ArgB+",ArgC="+ArgC+"]";
    }
    
    //-------------------------------------------//

  }

  /// <summary>
  /// Threadsafe action wrapper that uses the latest set of arguments when the action
  /// is run.
  /// </summary>
  public class ActionPop<A,B,C,D> : IRun, IAction, IAction<A>, IAction<A,B>, IAction<A,B,C>, IAction<A,B,C,D> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Needle used to run the action.
    /// </summary>
    public Needle Needle;
    
    /// <summary>
    /// Action that is run on the specified needle.
    /// </summary>
    public IAction<A,B,C,D> Action {
      get { return _action.Action; }
      set { _action.Action = value; }
    }
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    public C ArgC {get;set;}
    public D ArgD {get;set;}
    
    //-------------------------------------------//
    
    protected ActionRoll<A,B,C,D> _action;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionPop() {
      _action = new ActionRoll<A,B,C,D>();
      Needle = ManagerUpdate.Control;
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionPop(IAction<A,B,C,D> action, Needle needle = null) {
      _action = new ActionRoll<A,B,C,D>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    
    public ActionPop(Action<A,B,C,D> action, Needle needle = null) {
      _action = new ActionRoll<A,B,C,D>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    
    public ActionPop(Action<A,B,C,D> action, A a, B b = default(B), C c = default(C), D d = default(D)) :
      this(new ActionSet<A,B,C,D>(action), a, b, c, d) {
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionPop(IAction<A,B,C,D> action, A a, B b = default(B), C c = default(C), D d = default(D)) {
      _action = new ActionRoll<A,B,C,D>(action);
      ArgA = a;
      ArgB = b;
      ArgC = c;
      ArgD = d;
    }
    
    /// <summary>
    /// Add the specified parameter and run.
    /// </summary>
    public void Run(A a, B b, C c, D d) {
      _action.Add(a, b, c, d);
      Needle.AddSingle(_action);
    }
    
    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      _action.Add(ArgA, ArgB, ArgC, ArgD);
      Needle.AddSingle(_action);
    }
    
    public override string ToString() {
      return "[ActionRoll Action="+_action+",ArgA="+ArgA+", ArgB="+ArgB+",ArgC="+ArgC+",ArgD="+ArgD+"]";
    }
    
    //-------------------------------------------//

  }

  /// <summary>
  /// Threadsafe action wrapper that uses the latest set of arguments when the action
  /// is run.
  /// </summary>
  public class ActionPop<A,B,C,D,E> : IRun, IAction, IAction<A>, IAction<A,B>, IAction<A,B,C>, IAction<A,B,C,D>, IAction<A,B,C,D,E> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Needle used to run the action.
    /// </summary>
    public Needle Needle;
    
    /// <summary>
    /// Action that is run on the specified needle.
    /// </summary>
    public IAction<A,B,C,D,E> Action {
      get { return _action.Action; }
      set { _action.Action = value; }
    }
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    public C ArgC {get;set;}
    public D ArgD {get;set;}
    public E ArgE {get;set;}
    
    //-------------------------------------------//
    
    protected ActionRoll<A,B,C,D,E> _action;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionPop() {
      _action = new ActionRoll<A,B,C,D,E>();
      Needle = ManagerUpdate.Control;
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionPop(IAction<A,B,C,D,E> action, Needle needle = null) {
      _action = new ActionRoll<A,B,C,D,E>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    
    public ActionPop(Action<A,B,C,D,E> action, Needle needle = null) {
      _action = new ActionRoll<A,B,C,D,E>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    
    public ActionPop(Action<A,B,C,D,E> action, A a, B b = default(B), C c = default(C), D d = default(D), E e = default(E)) :
      this(new ActionSet<A,B,C,D,E>(action), a, b, c, d, e) {
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionPop(IAction<A,B,C,D,E> action, A a, B b = default(B), C c = default(C), D d = default(D), E e = default(E)) {
      _action = new ActionRoll<A,B,C,D,E>(action);
      ArgA = a;
      ArgB = b;
      ArgC = c;
      ArgD = d;
      ArgE = e;
    }
    
    /// <summary>
    /// Add the specified parameter and run.
    /// </summary>
    public void Run(A a, B b, C c, D d, E e) {
      _action.Add(a, b, c, d, e);
      Needle.AddSingle(_action);
    }
    
    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      _action.Add(ArgA, ArgB, ArgC, ArgD, ArgE);
      Needle.AddSingle(_action);
    }
    
    public override string ToString() {
      return "[ActionRoll Action="+_action+",ArgA="+ArgA+", ArgB="+ArgB+",ArgC="+ArgC+",ArgD="+ArgD+",ArgE="+ArgE+"]";
    }
    
    //-------------------------------------------//

  }


}
