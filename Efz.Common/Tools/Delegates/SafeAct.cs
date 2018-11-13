using System;

using Efz.Collections;
using Efz.Threading;

namespace Efz {
  
  /// <summary>
  /// Threadsafe action wrapper with a series of arguments. Running this action will cause one set of arguments
  /// to be dequeued and be assigned to the Action. Note that if an incomplete set of arguments is added
  /// to the queue, not all action arguments will be overriden.
  /// </summary>
  public class SafeAct : IAction {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The action this task will run.
    /// </summary>
    public event Action Action;
    
    /// <summary>
    /// Needle to run the actions on.
    /// </summary>
    public Needle Needle;

    //-------------------------------------------//

    /// <summary>
    /// Collection of arguments to be passed to the IAction.
    /// </summary>
    protected SafeQueue<IArgs> _argsSet;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public SafeAct() {
      _argsSet = new SafeQueue<IArgs>();
    }
    
    public SafeAct(Action action, Needle needle = null) {
      Action = action;
      Needle = needle ?? ManagerUpdate.Control;
      _argsSet = new SafeQueue<IArgs>();
    }
    
    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      // is there another set of arguments?
      if(_argsSet.Dequeue()) {
        
        // run the action
        Needle.AddSingle(Action);
      }
    }
    
    public override string ToString() {
      return "[ActionRoll Action="+Action.ToString()+", ArgsSet="+_argsSet.ToString()+"]";
    }
    
    //-------------------------------------------//

  }
  
  /// <summary>
  /// Threadsafe action wrapper with a series of arguments. Running this action will cause one set of arguments
  /// to be dequeued and be assigned to the Action. Note that if an incomplete set of arguments is added
  /// to the queue, not all action arguments will be overriden.
  /// </summary>
  public class SafeAct<A> : IAction<A> {
    
    //-------------------------------------------//

    /// <summary>
    /// Action run with the queued args.
    /// </summary>
    public event Action<A> Action;
    /// <summary>
    /// Needle that will run this task.
    /// </summary>
    public Needle Needle;
    
    /// <summary>
    /// The last args passed to the action.
    /// </summary>
    public A ArgA {
      get { return _argsSet.Current.ArgA; }
      set { _argsSet.Current.ArgA = value; }
    }
    
    //-------------------------------------------//

    /// <summary>
    /// Collection of arguments to be passed to the IAction.
    /// </summary>
    protected SafeQueue<IArgs<A>> _argsSet;

    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public SafeAct() {
      _argsSet = new SafeQueue<IArgs<A>>();
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public SafeAct(Action<A> action, Needle needle = null) {
      _argsSet = new SafeQueue<IArgs<A>>();
      Needle = needle ?? ManagerUpdate.Control;
      Action = action;
    }
    
    public SafeAct(Action<A> action, A a, Needle needle = null) {
      _argsSet = new SafeQueue<IArgs<A>>();
      Needle = needle ?? ManagerUpdate.Control;
      Action = action;
      Add(a);
    }

    /// <summary>
    /// Add a set of arguments to be called by the action roll.
    /// </summary>
    public void Add(A a) {
      _argsSet.Enqueue(new Args<A> {
        ArgA = a,
      });
    }

    /// <summary>
    /// Add a set of arguments to be called by the action roll.
    /// </summary>
    public void Add(IArgs<A> args) {
      _argsSet.Enqueue(args);
    }
    
    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      IArgs<A> args;
      // is there another set of arguments?
      if(_argsSet.Dequeue(out args)) {
        
        // add the action to the needle
        Needle.AddSingle(new ActionSet<A>(Action, args.ArgA));
      }
    }
    
    public override string ToString() {
      return "[ActionRoll Action="+Action.ToString()+", ArgsSet="+_argsSet.ToString()+"]";
    }
    
    //-------------------------------------------//

  }

  /// <summary>
  /// Threadsafe action wrapper with a series of arguments. Running this action will cause one set of arguments
  /// to be dequeued and be assigned to the Action. Note that if an incomplete set of arguments is added
  /// to the queue, not all action arguments will be overriden.
  /// </summary>
  public class SafeAct<A,B> : IAction<A,B> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The action that the task will run.
    /// </summary>
    public event Action<A,B> Action;
    /// <summary>
    /// Needle the tasks will be run on.
    /// </summary>
    public Needle Needle;

    public A ArgA {
      get { return _argsSet.Current.ArgA; }
      set { _argsSet.Current.ArgA = value; }
    }
    public B ArgB {
      get { return _argsSet.Current.ArgB; }
      set { _argsSet.Current.ArgB = value; }
    }
    
    //-------------------------------------------//

    /// <summary>
    /// Collection of arguments to be passed to the IAction.
    /// </summary>
    protected SafeQueue<IArgs<A,B>> _argsSet;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public SafeAct() {
      _argsSet = new SafeQueue<IArgs<A,B>>();
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public SafeAct(Action<A,B> action, Needle needle = null) {
      _argsSet = new SafeQueue<IArgs<A,B>>();
      Needle = needle ?? ManagerUpdate.Control;
      Action = action;
    }
    
    public SafeAct(Action<A,B> action, A a, B b = default(B), Needle needle = null) {
      _argsSet = new SafeQueue<IArgs<A,B>>();
      Needle = needle ?? ManagerUpdate.Control;
      Action = action;
      Add(a,b);
    }

    /// <summary>
    /// Add a set of arguments to be called by the action roll.
    /// </summary>
    public void Add(A a, B b) {
      _argsSet.Enqueue(new Args<A,B> {
        ArgA = a,
        ArgB = b,
      });
    }

    /// <summary>
    /// Add a set of arguments to be called by the action roll.
    /// </summary>
    public void Add(IArgs<A,B> args) {
      _argsSet.Enqueue(args);
    }

    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      IArgs<A,B> args;
      // is there another set of arguments?
      if(_argsSet.Dequeue(out args)) {
        
        // add the action to the needle
        Needle.AddSingle(new ActionSet<A,B>(Action, args.ArgA, args.ArgB));
      }
    }
    
    public override string ToString() {
      return "[ActionRoll Action="+Action.ToString()+", ArgsSet="+_argsSet.ToString()+"]";
    }
    
    //-------------------------------------------//

  }

  /// <summary>
  /// Threadsafe action wrapper with a series of arguments. Running this action will cause one set of arguments
  /// to be dequeued and be assigned to the Action. Note that if an incomplete set of arguments is added
  /// to the queue, not all action arguments will be overriden.
  /// </summary>
  public class SafeAct<A,B,C> : IAction<A,B,C> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The action that will be run.
    /// </summary>
    public event Action<A,B,C> Action;
    /// <summary>
    /// The needle the tasks will be run on.
    /// </summary>
    public Needle Needle;

    public A ArgA {
      get { return _argsSet.Current.ArgA; }
      set { _argsSet.Current.ArgA = value; }
    }
    public B ArgB {
      get { return _argsSet.Current.ArgB; }
      set { _argsSet.Current.ArgB = value; }
    }
    public C ArgC {
      get { return _argsSet.Current.ArgC; }
      set { _argsSet.Current.ArgC = value; }
    }
    
    //-------------------------------------------//

    /// <summary>
    /// Collection of arguments to be passed to the IAction.
    /// </summary>
    protected SafeQueue<IArgs<A,B,C>> _argsSet;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public SafeAct() {
      _argsSet = new SafeQueue<IArgs<A,B,C>>();
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public SafeAct(Action<A,B,C> action, Needle needle) {
      _argsSet = new SafeQueue<IArgs<A,B,C>>();
      Needle = needle ?? ManagerUpdate.Control;
      Action = action;
    }
    
    public SafeAct(Action<A,B,C> action, A a, B b = default(B), C c = default(C), Needle needle = null) {
      _argsSet = new SafeQueue<IArgs<A,B,C>>();
      Needle = needle ?? ManagerUpdate.Control;
      Action = action;
      Add(a,b,c);
    }

    /// <summary>
    /// Add a set of arguments to be called by the action roll.
    /// </summary>
    public void Add(A a, B b, C c) {
      _argsSet.Enqueue(new Args<A,B,C> {
        ArgA = a,
        ArgB = b,
        ArgC = c
      });
    }

    /// <summary>
    /// Add a set of arguments to be called by the action roll.
    /// </summary>
    public void Add(IArgs<A,B,C> args) {
      _argsSet.Enqueue(args);
    }
    
    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      IArgs<A,B,C> args;
      // is there another set of arguments?
      if(_argsSet.Dequeue(out args)) {
        
        // add the action to the needle
        Needle.AddSingle(new ActionSet<A,B,C>(Action, args.ArgA, args.ArgB, args.ArgC));
      }
    }
    
    public override string ToString() {
      return "[ActionRoll Action="+Action.ToString()+", ArgsSet="+_argsSet.ToString()+"]";
    }
    
    //-------------------------------------------//

  }

  /// <summary>
  /// Threadsafe action wrapper with a series of arguments. Running this action will cause one set of arguments
  /// to be dequeued and be assigned to the Action. Note that if an incomplete set of arguments is added
  /// to the queue, not all action arguments will be overriden.
  /// </summary>
  public class SafeAct<A,B,C,D> : IAction<A,B,C,D> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The action that will be run.
    /// </summary>
    public event Action<A,B,C,D> Action;
    /// <summary>
    /// The needle that the action will be run on.
    /// </summary>
    public Needle Needle;

    public A ArgA {
      get { return _argsSet.Current.ArgA; }
      set { _argsSet.Current.ArgA = value; }
    }
    public B ArgB {
      get { return _argsSet.Current.ArgB; }
      set { _argsSet.Current.ArgB = value; }
    }
    public C ArgC {
      get { return _argsSet.Current.ArgC; }
      set { _argsSet.Current.ArgC = value; }
    }
    public D ArgD {
      get { return _argsSet.Current.ArgD; }
      set { _argsSet.Current.ArgD = value; }
    }
    
    //-------------------------------------------//

    /// <summary>
    /// Collection of arguments to be passed to the IAction.
    /// </summary>
    protected SafeQueue<IArgs<A,B,C,D>> _argsSet;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public SafeAct() {
      _argsSet = new SafeQueue<IArgs<A,B,C,D>>();
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public SafeAct(Action<A,B,C,D> action, Needle needle) {
      _argsSet = new SafeQueue<IArgs<A,B,C,D>>();
      Needle = needle ?? ManagerUpdate.Control;
      Action = action;
    }
    
    public SafeAct(Action<A,B,C,D> action, A a, B b = default(B), C c = default(C), D d = default(D), Needle needle = null) {
      _argsSet = new SafeQueue<IArgs<A,B,C,D>>();
      Needle = needle ?? ManagerUpdate.Control;
      Action = action;
      Add(a,b,c,d);
    }

    /// <summary>
    /// Add a set of arguments to be called by the action roll.
    /// </summary>
    public void Add(A a, B b, C c, D d) {
      _argsSet.Enqueue(new Args<A,B,C,D> {
        ArgA = a,
        ArgB = b,
        ArgC = c,
        ArgD = d
      });
    }

    /// <summary>
    /// Add a set of arguments to be called by the action roll.
    /// </summary>
    public void Add(IArgs<A,B,C,D> args) {
      _argsSet.Enqueue(args);
    }

    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      IArgs<A,B,C,D> args;
      // is there another set of arguments?
      if(_argsSet.Dequeue(out args)) {
        
        // add the action to the needle
        Needle.AddSingle(new ActionSet<A,B,C,D>(Action, args.ArgA, args.ArgB, args.ArgC, args.ArgD));
      }
    }
    
    public override string ToString() {
      return "[ActionRoll Action="+Action.ToString()+", ArgsSet="+_argsSet.ToString()+"]";
    }
    
    //-------------------------------------------//

  }

  /// <summary>
  /// Threadsafe action wrapper with a series of arguments. Running this action will cause one set of arguments
  /// to be dequeued and be assigned to the Action. Note that if an incomplete set of arguments is added
  /// to the queue, not all action arguments will be overriden.
  /// </summary>
  public class SafeAct<A,B,C,D,E> : IAction<A,B,C,D,E> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The action that will be run.
    /// </summary>
    public event Action<A,B,C,D,E> Action;
    /// <summary>
    /// The needle that action will be run on.
    /// </summary>
    public Needle Needle;

    public A ArgA {
      get { return _argsSet.Current.ArgA; }
      set { _argsSet.Current.ArgA = value; }
    }
    public B ArgB {
      get { return _argsSet.Current.ArgB; }
      set { _argsSet.Current.ArgB = value; }
    }
    public C ArgC {
      get { return _argsSet.Current.ArgC; }
      set { _argsSet.Current.ArgC = value; }
    }
    public D ArgD {
      get { return _argsSet.Current.ArgD; }
      set { _argsSet.Current.ArgD = value; }
    }
    public E ArgE {
      get { return _argsSet.Current.ArgE; }
      set { _argsSet.Current.ArgE = value; }
    }

    //-------------------------------------------//

    /// <summary>
    /// Collection of arguments to be passed to the IAction.
    /// </summary>
    protected SafeQueue<IArgs<A,B,C,D,E>> _argsSet;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public SafeAct() {
      _argsSet = new SafeQueue<IArgs<A,B,C,D,E>>();
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public SafeAct(Action<A,B,C,D,E> action, Needle needle = null) {
      _argsSet = new SafeQueue<IArgs<A,B,C,D,E>>();
      Needle = needle ?? ManagerUpdate.Control;
      Action = action;
    }
    
    public SafeAct(Action<A,B,C,D,E> action, A a = default(A), B b = default(B), C c = default(C), D d = default(D), E e = default(E), Needle needle = null) {
      _argsSet = new SafeQueue<IArgs<A,B,C,D,E>>();
      Needle = needle ?? ManagerUpdate.Control;
      Action = action;
      Add(a,b,c,d,e);
    }

    /// <summary>
    /// Add a set of arguments to be called by the action roll.
    /// </summary>
    public void Add(A a, B b, C c, D d, E e) {
      _argsSet.Enqueue(new Args<A,B,C,D,E> {
        ArgA = a,
        ArgB = b,
        ArgC = c,
        ArgD = d,
        ArgE = e
      });
    }

    /// <summary>
    /// Add a set of arguments to be called by the action roll.
    /// </summary>
    public void Add(IArgs<A,B,C,D,E> args) {
      _argsSet.Enqueue(args);
    }

    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      IArgs<A,B,C,D,E> args;
      // is there another set of arguments?
      if(_argsSet.Dequeue(out args)) {
        
        // add the action to the needle
        Needle.AddSingle(new ActionSet<A,B,C,D,E>(Action, args.ArgA, args.ArgB, args.ArgC, args.ArgD, args.ArgE));
      }
    }
    
    public override string ToString() {
      return "[ActionRoll Action="+Action.ToString()+", ArgsSet="+_argsSet.ToString()+"]";
    }
    
    //-------------------------------------------//

  }


}
