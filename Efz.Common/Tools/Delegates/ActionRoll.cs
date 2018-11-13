using System;

using Efz.Collections;
using Efz.Threading;

namespace Efz {
  
  /// <summary>
  /// Threadsafe action wrapper with a series of arguments. Running this action will cause one set of arguments
  /// to be dequeued and be assigned to the Action. Note that if an incomplete set of arguments is added
  /// to the queue, not all action arguments will be overriden.
  /// </summary>
  public class ActionRoll : IAction {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Action that will be run with the specified args.
    /// </summary>
    public IAction Action {
      get {
        if(!ActionSet) return null;
        var action = _action.TakeItem();
        _action.Release();
        return action;
      }
      set {
        _action.Take();
        _action.ReleaseItem(value);
        ActionSet = value != null;
      }
    }
    
    /// <summary>
    /// Flag indicating whether an action has been assigned to the roll.
    /// </summary>
    public bool ActionSet;
    
    //-------------------------------------------//

    /// <summary>
    /// Collection of arguments to be passed to the IAction.
    /// </summary>
    protected SafeQueue<IArgs> _argsSet;
    /// <summary>
    /// Inner shared action for lazy initialization.
    /// </summary>
    protected Shared<IAction> _action;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionRoll() {
      _argsSet = new SafeQueue<IArgs>();
      _action = new Shared<IAction>();
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionRoll(IAction action) {
      _argsSet = new SafeQueue<IArgs>();
      _action = new Shared<IAction>(action);
      ActionSet = true;
    }
    
    public ActionRoll(Action action) {
      _argsSet = new SafeQueue<IArgs>();
      _action = new Shared<IAction>(new ActionSet(action));
      ActionSet = true;
    }

    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      // is there another set of arguments?
      if(_argsSet.Dequeue()) {
        
        // run the action
        var action = _action.TakeItem();
        _action.Release();
        action.Run();
        while(_argsSet.Dequeue()) action.Run();
      }
    }
    
    public override string ToString() {
      return "[ActionRoll Action="+_action.ToString()+", ArgsSet="+_argsSet.ToString()+"]";
    }
    
    //-------------------------------------------//

  }
  
  /// <summary>
  /// Threadsafe action wrapper with a series of arguments. Running this action will cause one set of arguments
  /// to be dequeued and be assigned to the Action. Note that if an incomplete set of arguments is added
  /// to the queue, not all action arguments will be overriden.
  /// </summary>
  public class ActionRoll<A> : IAction<A> {
    
    //-------------------------------------------//
    
    public IAction<A> Action {
      get {
        if(!ActionSet) return null;
        var action = _action.TakeItem();
        _action.Release();
        return action;
      }
      set {
        _action.Take();
        _action.ReleaseItem(value);
        ActionSet = value != null;
      }
    }
    
    /// <summary>
    /// Flag indicating whether an action has been assigned to the roll.
    /// </summary>
    public bool ActionSet;
    
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
    /// <summary>
    /// Inner shared action reference for lazy init.
    /// </summary>
    protected Shared<IAction<A>> _action;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionRoll() {
      _argsSet = new SafeQueue<IArgs<A>>();
      _action = new Shared<IAction<A>>();
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionRoll(IAction<A> action) {
      _argsSet = new SafeQueue<IArgs<A>>();
      _action = new Shared<IAction<A>>(action);
      ActionSet = true;
    }
    
    public ActionRoll(Action<A> action) {
      _argsSet = new SafeQueue<IArgs<A>>();
      _action = new Shared<IAction<A>>(new ActionSet<A>(action));
      ActionSet = true;
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
    public void Run(A a) {
      _argsSet.Enqueue(new Args<A> { ArgA = a });
      Run();
    }
    
    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      IArgs<A> args;
      // are there arguments?
      if(_argsSet.Dequeue(out args)) {
        
        // yes, get a reference to the current action
        var action = _action.TakeItem();
        _action.Release();
        
        // yes, get the next set of arguments
        action.ArgA = args.ArgA;
        // run the action
        action.Run();
        
        // while there are more arguments
        while(_argsSet.Dequeue(out args)) {
          // get the next set of arguments
          action.ArgA = args.ArgA;
          // run the action
          action.Run();
        }
        
      }
    }
    
    public override string ToString() {
      return "[ActionRoll Action="+_action.ToString()+", ArgsSet="+_argsSet.ToString()+"]";
    }
    
    //-------------------------------------------//

  }

  /// <summary>
  /// Threadsafe action wrapper with a series of arguments. Running this action will cause one set of arguments
  /// to be dequeued and be assigned to the Action. Note that if an incomplete set of arguments is added
  /// to the queue, not all action arguments will be overriden.
  /// </summary>
  public class ActionRoll<A,B> : IAction<A,B> {
    
    //-------------------------------------------//

    public IAction<A,B> Action {
      get {
        if(!ActionSet) return null;
        var action = _action.TakeItem();
        _action.Release();
        return action;
      }
      set {
        _action.Take();
        _action.ReleaseItem(value);
        ActionSet = value != null;
      }
    }
    
    /// <summary>
    /// Flag indicating whether an action has been assigned to the roll.
    /// </summary>
    public bool ActionSet;

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
    /// <summary>
    /// Inner shared action for lazy initialization.
    /// </summary>
    protected Shared<IAction<A,B>> _action;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionRoll() {
      _argsSet = new SafeQueue<IArgs<A,B>>();
      _action = new Shared<IAction<A, B>>();
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionRoll(IAction<A,B> action) {
      _argsSet = new SafeQueue<IArgs<A,B>>();
      _action = new Shared<IAction<A, B>>(action);
    }
    
    public ActionRoll(Action<A,B> action) {
      _argsSet = new SafeQueue<IArgs<A,B>>();
      _action = new Shared<IAction<A, B>>(new ActionSet<A,B>(action));
    }
    
    public ActionRoll(IAction<A,B> action, A a, B b = default(B)) {
      _argsSet = new SafeQueue<IArgs<A,B>>();
      _action = new Shared<IAction<A, B>>(action);
      Add(a,b);
    }
    
    public ActionRoll(Action<A,B> action, A a, B b = default(B)) {
      _argsSet = new SafeQueue<IArgs<A,B>>();
      _action = new Shared<IAction<A, B>>(new ActionSet<A,B>(action));
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
    public void Run(A a, B b) {
      _argsSet.Enqueue(new Args<A,B> {
        ArgA = a,
        ArgB = b
      });
      
      Run();
    }

    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      IArgs<A,B> args;
      // is there another set of arguments?
      if(_argsSet.Dequeue(out args)) {
        
        var action = _action.TakeItem();
        _action.Release();
        
        // yes, get the next set of arguments
        action.ArgA = args.ArgA;
        action.ArgB = args.ArgB;
        // run the action
        action.Run();
        
        // while there are more arguments
        while(_argsSet.Dequeue(out args)) {
          // get the next set of arguments
          action.ArgA = args.ArgA;
          action.ArgB = args.ArgB;
          // run the action
          action.Run();
        }
      }
    }
    
    public override string ToString() {
      return "[ActionRoll Action="+_action.ToString()+", ArgsSet="+_argsSet.ToString()+"]";
    }
    
    //-------------------------------------------//

  }

  /// <summary>
  /// Threadsafe action wrapper with a series of arguments. Running this action will cause one set of arguments
  /// to be dequeued and be assigned to the Action. Note that if an incomplete set of arguments is added
  /// to the queue, not all action arguments will be overriden.
  /// </summary>
  public class ActionRoll<A,B,C> : IAction<A,B,C> {
    
    //-------------------------------------------//

    public IAction<A,B,C> Action {
      get {
        if(!ActionSet) return null;
        var action = _action.TakeItem();
        _action.Release();
        return action;
      }
      set {
        _action.Take();
        _action.ReleaseItem(value);
        ActionSet = value != null;
      }
    }
    
    /// <summary>
    /// Flag indicating whether an action has been assigned to the roll.
    /// </summary>
    public bool ActionSet;

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
    /// <summary>
    /// Inner shared action for lazy initialization.
    /// </summary>
    protected Shared<IAction<A,B,C>> _action;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionRoll() {
      _argsSet = new SafeQueue<IArgs<A,B,C>>();
      _action = new Shared<IAction<A, B, C>>();
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionRoll(IAction<A,B,C> action) {
      _argsSet = new SafeQueue<IArgs<A,B,C>>();
      _action = new Shared<IAction<A, B, C>>(action);
      ActionSet = true;
    }
    
    public ActionRoll(Action<A,B,C> action) {
      _argsSet = new SafeQueue<IArgs<A,B,C>>();
      _action = new Shared<IAction<A, B, C>>(new ActionSet<A,B,C>(action));
      ActionSet = true;
    }
    
    public ActionRoll(IAction<A,B,C> action, A a, B b = default(B), C c = default(C)) {
      _argsSet = new SafeQueue<IArgs<A,B,C>>();
      _action = new Shared<IAction<A, B, C>>(action);
      Add(a,b,c);
      ActionSet = true;
    }
    
    public ActionRoll(Action<A,B,C> action, A a, B b = default(B), C c = default(C)) {
      _argsSet = new SafeQueue<IArgs<A,B,C>>();
      _action = new Shared<IAction<A, B, C>>(new ActionSet<A,B,C>(action));
      Add(a,b,c);
      ActionSet = true;
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
    public void Run(A a, B b, C c) {
      _argsSet.Enqueue(new Args<A,B,C> {
        ArgA = a,
        ArgB = b,
        ArgC = c
      });
      
      Run();
    }
    
    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      IArgs<A,B,C> args;
      // is there another set of arguments?
      if(_argsSet.Dequeue(out args)) {
        
        var action = _action.TakeItem();
        _action.Release();
        
        // yes, get the next set of arguments
        action.ArgA = args.ArgA;
        action.ArgB = args.ArgB;
        action.ArgC = args.ArgC;
        // run the action
        action.Run();
        
        // while there are more arguments
        while(_argsSet.Dequeue(out args)) {
          // get the next set of arguments
          action.ArgA = args.ArgA;
          action.ArgB = args.ArgB;
          action.ArgC = args.ArgC;
          // run the action
          action.Run();
        }
        
      }
    }
    
    public override string ToString() {
      return "[ActionRoll Action="+_action.ToString()+", ArgsSet="+_argsSet.ToString()+"]";
    }
    
    //-------------------------------------------//

  }

  /// <summary>
  /// Threadsafe action wrapper with a series of arguments. Running this action will cause one set of arguments
  /// to be dequeued and be assigned to the Action. Note that if an incomplete set of arguments is added
  /// to the queue, not all action arguments will be overriden.
  /// </summary>
  public class ActionRoll<A,B,C,D> : IAction<A,B,C,D> {
    
    //-------------------------------------------//
    
    public IAction<A,B,C,D> Action {
      get {
        if(!ActionSet) return null;
        var action = _action.TakeItem();
        _action.Release();
        return action;
      }
      set {
        _action.Take();
        _action.ReleaseItem(value);
        ActionSet = value != null;
      }
    }
    
    /// <summary>
    /// Flag indicating whether an action has been assigned to the roll.
    /// </summary>
    public bool ActionSet;
    
    public A ArgA {
      get { return _argsSet.Current.ArgA; }
      set { _argsSet.Enqueue(new Args<A,B,C,D> { ArgA = value }); }
    }
    public B ArgB {
      get { return _argsSet.Current.ArgB; }
      set { _argsSet.Enqueue(new Args<A,B,C,D> { ArgB = value }); }
    }
    public C ArgC {
      get { return _argsSet.Current.ArgC; }
      set { _argsSet.Enqueue(new Args<A,B,C,D> { ArgC = value }); }
    }
    public D ArgD {
      get { return _argsSet.Current.ArgD; }
      set { _argsSet.Enqueue(new Args<A,B,C,D> { ArgD = value }); }
    }
    
    //-------------------------------------------//

    /// <summary>
    /// Collection of arguments to be passed to the IAction.
    /// </summary>
    protected SafeQueue<IArgs<A,B,C,D>> _argsSet;
    /// <summary>
    /// Inner shared action for lazy initialization.
    /// </summary>
    protected Shared<IAction<A,B,C,D>> _action;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionRoll() {
      _argsSet = new SafeQueue<IArgs<A,B,C,D>>();
      _action = new Shared<IAction<A, B, C, D>>();
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionRoll(IAction<A,B,C,D> action) {
      _argsSet = new SafeQueue<IArgs<A,B,C,D>>();
      _action = new Shared<IAction<A, B, C, D>>(action);
      ActionSet = true;
    }
    
    public ActionRoll(Action<A,B,C,D> action) {
      _argsSet = new SafeQueue<IArgs<A,B,C,D>>();
      _action = new Shared<IAction<A, B, C, D>>(new ActionSet<A,B,C,D>(action));
      ActionSet = true;
    }
    
    public ActionRoll(IAction<A,B,C,D> action, A a, B b = default(B), C c = default(C), D d = default(D)) {
      _argsSet = new SafeQueue<IArgs<A,B,C,D>>();
      _action = new Shared<IAction<A, B, C, D>>(action);
      Add(a,b,c,d);
      ActionSet = true;
    }
    
    public ActionRoll(Action<A,B,C,D> action, A a, B b = default(B), C c = default(C), D d = default(D)) {
      _argsSet = new SafeQueue<IArgs<A,B,C,D>>();
      _action = new Shared<IAction<A, B, C, D>>(new ActionSet<A,B,C,D>(action));
      Add(a,b,c,d);
      ActionSet = true;
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
    public void Run(A a, B b, C c, D d) {
      _argsSet.Enqueue(new Args<A,B,C,D> {
        ArgA = a,
        ArgB = b,
        ArgC = c,
        ArgD = d
      });
      
      Run();
    }
    
    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      IArgs<A,B,C,D> args;
      // is there another set of arguments?
      if(_argsSet.Dequeue(out args)) {
        
        var action = _action.TakeItem();
        _action.Release();
        
        // yes, get the next set of arguments
        action.ArgA = args.ArgA;
        action.ArgB = args.ArgB;
        action.ArgC = args.ArgC;
        action.ArgD = args.ArgD;
        // run the action
        action.Run();
        
        // while there are more arguments
        while(_argsSet.Dequeue(out args)) {
          // get the next set of arguments
          action.ArgA = args.ArgA;
          action.ArgB = args.ArgB;
          action.ArgC = args.ArgC;
          action.ArgD = args.ArgD;
          // run the action
          action.Run();
        }
        
      }
    }
    
    public override string ToString() {
      return "[ActionRoll Action="+_action.ToString()+", ArgsSet="+_argsSet.ToString()+"]";
    }
    
    //-------------------------------------------//

  }

  /// <summary>
  /// Threadsafe action wrapper with a series of arguments. Running this action will cause one set of arguments
  /// to be dequeued and be assigned to the Action. Note that if an incomplete set of arguments is added
  /// to the queue, not all action arguments will be overriden.
  /// </summary>
  public class ActionRoll<A,B,C,D,E> : IAction<A,B,C,D,E> {
    
    //-------------------------------------------//

    public IAction<A,B,C,D,E> Action {
      get {
        if(!ActionSet) return null;
        var action = _action.TakeItem();
        _action.Release();
        return action;
      }
      set {
        _action.Take();
        _action.ReleaseItem(value);
        ActionSet = value != null;
      }
    }
    
    /// <summary>
    /// Flag indicating whether an action has been assigned to the roll.
    /// </summary>
    public bool ActionSet;

    public A ArgA {
      get { return _argsSet.Current.ArgA; }
      set { _argsSet.Enqueue(new Args<A,B,C,D,E> {ArgA = value}); }
    }
    public B ArgB {
      get { return _argsSet.Current.ArgB; }
      set { _argsSet.Enqueue(new Args<A,B,C,D,E> {ArgB = value}); }
    }
    public C ArgC {
      get { return _argsSet.Current.ArgC; }
      set { _argsSet.Enqueue(new Args<A,B,C,D,E> {ArgC = value}); }
    }
    public D ArgD {
      get { return _argsSet.Current.ArgD; }
      set { _argsSet.Enqueue(new Args<A,B,C,D,E> {ArgD = value}); }
    }
    public E ArgE {
      get { return _argsSet.Current.ArgE; }
      set { _argsSet.Enqueue(new Args<A,B,C,D,E> {ArgE = value}); }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Collection of arguments to be passed to the IAction.
    /// </summary>
    protected SafeQueue<IArgs<A,B,C,D,E>> _argsSet;
    /// <summary>
    /// Inner shared action reference for lazy init.
    /// </summary>
    protected Shared<IAction<A,B,C,D,E>> _action;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionRoll() {
      _argsSet = new SafeQueue<IArgs<A,B,C,D,E>>();
      _action = new Shared<IAction<A, B, C, D, E>>();
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionRoll(IAction<A,B,C,D,E> action) {
      _argsSet = new SafeQueue<IArgs<A,B,C,D,E>>();
      _action = new Shared<IAction<A, B, C, D, E>>(action);
      ActionSet = true;
    }
    
    public ActionRoll(Action<A,B,C,D,E> action) {
      _argsSet = new SafeQueue<IArgs<A,B,C,D,E>>();
      _action = new Shared<IAction<A, B, C, D, E>>(new ActionSet<A,B,C,D,E>(action));
      ActionSet = true;
    }
    
    public ActionRoll(IAction<A,B,C,D,E> action, A a, B b = default(B), C c = default(C), D d = default(D), E e = default(E)) {
      _argsSet = new SafeQueue<IArgs<A,B,C,D,E>>();
      _action = new Shared<IAction<A, B, C, D, E>>(action);
      Add(a,b,c,d,e);
      ActionSet = true;
    }
    
    public ActionRoll(Action<A,B,C,D,E> action, A a, B b = default(B), C c = default(C), D d = default(D), E e = default(E)) {
      _argsSet = new SafeQueue<IArgs<A,B,C,D,E>>();
      _action = new Shared<IAction<A, B, C, D, E>>(new ActionSet<A,B,C,D,E>(action, a, b, c, d, e));
      ActionSet = true;
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
    public void Run(A a, B b, C c, D d, E e) {
      _argsSet.Enqueue(new Args<A,B,C,D,E> {
        ArgA = a,
        ArgB = b,
        ArgC = c,
        ArgD = d,
        ArgE = e
      });
      
      Run();
    }

    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      IArgs<A,B,C,D,E> args;
      
      // is there another set of arguments?
      if(_argsSet.Dequeue(out args)) {
        
        var action = _action.TakeItem();
        _action.Release();
        
        // yes, get the next set of arguments
        action.ArgA = args.ArgA;
        action.ArgB = args.ArgB;
        action.ArgC = args.ArgC;
        action.ArgD = args.ArgD;
        action.ArgE = args.ArgE;
        // run the action
        action.Run();
        
        // while there are more arguments
        while(_argsSet.Dequeue(out args)) {
          // get the next set of arguments
          action.ArgA = args.ArgA;
          action.ArgB = args.ArgB;
          action.ArgC = args.ArgC;
          action.ArgD = args.ArgD;
          action.ArgE = args.ArgE;
          // run the action
          action.Run();
        }
        
      }
      
    }
    
    public override string ToString() {
      return "[ActionRoll Action="+_action.ToString()+", ArgsSet="+_argsSet.ToString()+"]";
    }
    
    //-------------------------------------------//

  }


}
