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
    /// Action to be run.
    /// </summary>
    public IAction Action;
    /// <summary>
    /// Current arguments.
    /// </summary>
    public IArgs Arguments;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Collection of arguments to be passed to the IAction.
    /// </summary>
    protected SafeQueue<IArgs> _argsSet;
    /// <summary>
    /// Lock used when dequeuing arguments.
    /// </summary>
    protected Lock _lock;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionRoll() {
      _lock = new Lock();
      _argsSet = new SafeQueue<IArgs>();
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionRoll(IAction action) {
      _argsSet = new SafeQueue<IArgs>();
      _lock = new Lock();
      Action = action;
    }
    
    public ActionRoll(Action action) {
      _argsSet = new SafeQueue<IArgs>();
      _lock = new Lock();
      Action = new ActionSet(action);
    }
    
    /// <summary>
    /// Add a set of arguments to be called by the action roll.
    /// </summary>
    public void Add(IArgs args) {
      _argsSet.Enqueue(args);
    }
    
    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run(IArgs args) {
      _argsSet.Enqueue(args);
      Run();
    }
    
    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      
      // are there arguments?
      if(_argsSet.Dequeue(out Arguments)) {
        
        // yes, run the action
        _lock.Take();
        try {
          Action.Run();
        } finally {
          _lock.Release();
        }
        
        // while there are more arguments
        while(_argsSet.Dequeue(out Arguments)) {
          
          // run the action
          _lock.Take();
          try {
            Action.Run();
          } finally {
            _lock.Release();
          }
          
        }
        
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
  public class ActionRoll<A> : IAction<A> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Action to be run.
    /// </summary>
    public IAction<A> Action;
    
    /// <summary>
    /// An argument to be passed to the action.
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
    /// Lock used when dequeuing arguments.
    /// </summary>
    protected Lock _lock;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionRoll() {
      _lock = new Lock();
      _argsSet = new SafeQueue<IArgs<A>>();
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionRoll(IAction<A> action) {
      _argsSet = new SafeQueue<IArgs<A>>();
      _lock = new Lock();
      Action = action;
    }
    
    public ActionRoll(Action<A> action) {
      _argsSet = new SafeQueue<IArgs<A>>();
      _lock = new Lock();
      Action = new ActionSet<A>(action);
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
        
        _lock.Take();
        
        try {
          
          // yes, get the next set of arguments
          Action.ArgA = args.ArgA;
          
          // run the action
          Action.Run();
          
        } finally {
          
          _lock.Release();
          
        }
        
        // while there are more arguments
        while(_argsSet.Dequeue(out args)) {
          
          _lock.Take();
          
          try {
            
            // yes, get the next set of arguments
            Action.ArgA = args.ArgA;
            
            // run the action
            Action.Run();
            
          } finally {
            
            _lock.Release();
            
          }
          
        }
        
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
  public class ActionRoll<A,B> : IAction<A,B> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Action to be run.
    /// </summary>
    public IAction<A,B> Action;
    
    /// <summary>
    /// An argument to be passed to the action.
    /// </summary>
    public A ArgA {
      get { return _argsSet.Current.ArgA; }
      set { _argsSet.Current.ArgA = value; }
    }
    /// <summary>
    /// An argument to be passed to the action.
    /// </summary>
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
    /// Lock used when dequeuing arguments.
    /// </summary>
    protected Lock _lock;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionRoll() {
      _lock = new Lock();
      _argsSet = new SafeQueue<IArgs<A,B>>();
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionRoll(IAction<A,B> action) {
      _argsSet = new SafeQueue<IArgs<A,B>>();
      _lock = new Lock();
      Action = action;
    }
    
    public ActionRoll(Action<A,B> action) {
      _argsSet = new SafeQueue<IArgs<A,B>>();
      _lock = new Lock();
      Action = new ActionSet<A,B>(action);
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
      _argsSet.Enqueue(new Args<A,B> { ArgA = a, ArgB = b });
      Run();
    }
    
    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      IArgs<A,B> args;
      
      // are there arguments?
      if(_argsSet.Dequeue(out args)) {
        
        _lock.Take();
        try {
          
          // yes, get the next set of arguments
          Action.ArgA = args.ArgA;
          Action.ArgB = args.ArgB;
          
          // run the action
          Action.Run();
          
        } finally {
          _lock.Release();
        }
        
        // while there are more arguments
        while(_argsSet.Dequeue(out args)) {
          
          _lock.Take();
          try {
            
            // get the next set of arguments
            Action.ArgA = args.ArgA;
            Action.ArgB = args.ArgB;
            
            // run the action
            Action.Run();
            
          } finally {
            _lock.Release();
          }
          
        }
        
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
  public class ActionRoll<A,B,C> : IAction<A,B,C> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Action to be run.
    /// </summary>
    public IAction<A,B,C> Action;
    
    /// <summary>
    /// An argument to be passed to the action.
    /// </summary>
    public A ArgA {
      get { return _argsSet.Current.ArgA; }
      set { _argsSet.Current.ArgA = value; }
    }
    /// <summary>
    /// An argument to be passed to the action.
    /// </summary>
    public B ArgB {
      get { return _argsSet.Current.ArgB; }
      set { _argsSet.Current.ArgB = value; }
    }
    /// <summary>
    /// An argument to be passed to the action.
    /// </summary>
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
    /// Lock used when dequeuing arguments.
    /// </summary>
    protected Lock _lock;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionRoll() {
      _lock = new Lock();
      _argsSet = new SafeQueue<IArgs<A,B,C>>();
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionRoll(IAction<A,B,C> action) {
      _argsSet = new SafeQueue<IArgs<A,B,C>>();
      _lock = new Lock();
      Action = action;
    }
    
    public ActionRoll(Action<A,B,C> action) {
      _argsSet = new SafeQueue<IArgs<A,B,C>>();
      _lock = new Lock();
      Action = new ActionSet<A,B,C>(action);
    }
    
    /// <summary>
    /// Add a set of arguments to be called by the action roll.
    /// </summary>
    public void Add(A a, B b, C c) {
      _argsSet.Enqueue(new Args<A,B,C> {
        ArgA = a,
        ArgB = b,
        ArgC = c,
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
      _argsSet.Enqueue(new Args<A,B,C> { ArgA = a, ArgB = b, ArgC = c });
      Run();
    }
    
    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      IArgs<A,B,C> args;
      
      // are there arguments?
      if(_argsSet.Dequeue(out args)) {
        
        _lock.Take();
        try {
          // yes, get the next set of arguments
          Action.ArgA = args.ArgA;
          Action.ArgB = args.ArgB;
          Action.ArgC = args.ArgC;
          
          // run the action
          Action.Run();
        } finally {
          _lock.Release();
        }
        
        // while there are more arguments
        while(_argsSet.Dequeue(out args)) {
          
          _lock.Take();
          try {
            // get the next set of arguments
            Action.ArgA = args.ArgA;
            Action.ArgB = args.ArgB;
            Action.ArgC = args.ArgC;
            
            // run the action
            Action.Run();
          } finally {
            _lock.Release();
          }
          
        }
        
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
  public class ActionRoll<A,B,C,D> : IAction<A,B,C,D> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Action to be run.
    /// </summary>
    public IAction<A,B,C,D> Action;
    
    /// <summary>
    /// An argument to be passed to the action.
    /// </summary>
    public A ArgA {
      get { return _argsSet.Current.ArgA; }
      set { _argsSet.Current.ArgA = value; }
    }
    /// <summary>
    /// An argument to be passed to the action.
    /// </summary>
    public B ArgB {
      get { return _argsSet.Current.ArgB; }
      set { _argsSet.Current.ArgB = value; }
    }
    /// <summary>
    /// An argument to be passed to the action.
    /// </summary>
    public C ArgC {
      get { return _argsSet.Current.ArgC; }
      set { _argsSet.Current.ArgC = value; }
    }
    /// <summary>
    /// An argument to be passed to the action.
    /// </summary>
    public D ArgD {
      get { return _argsSet.Current.ArgD; }
      set { _argsSet.Current.ArgD = value; }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Collection of arguments to be passed to the IAction.
    /// </summary>
    protected SafeQueue<IArgs<A,B,C,D>> _argsSet;
    /// <summary>
    /// Lock used when dequeuing arguments.
    /// </summary>
    protected Lock _lock;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionRoll() {
      _lock = new Lock();
      _argsSet = new SafeQueue<IArgs<A,B,C,D>>();
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionRoll(IAction<A,B,C,D> action) {
      _argsSet = new SafeQueue<IArgs<A,B,C,D>>();
      _lock = new Lock();
      Action = action;
    }
    
    public ActionRoll(Action<A,B,C,D> action) {
      _argsSet = new SafeQueue<IArgs<A,B,C,D>>();
      _lock = new Lock();
      Action = new ActionSet<A,B,C,D>(action);
    }
    
    /// <summary>
    /// Add a set of arguments to be called by the action roll.
    /// </summary>
    public void Add(A a, B b, C c, D d) {
      _argsSet.Enqueue(new Args<A,B,C,D> {
        ArgA = a,
        ArgB = b,
        ArgC = c,
        ArgD = d,
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
      _argsSet.Enqueue(new Args<A,B,C,D> { ArgA = a, ArgB = b, ArgC = c, ArgD = d });
      Run();
    }
    
    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      IArgs<A,B,C,D> args;
      
      // are there arguments?
      if(_argsSet.Dequeue(out args)) {
        
        _lock.Take();
        try {
          // yes, get the next set of arguments
          Action.ArgA = args.ArgA;
          Action.ArgB = args.ArgB;
          Action.ArgC = args.ArgC;
          Action.ArgD = args.ArgD;
          
          // run the action
          Action.Run();
        } finally {
          _lock.Release();
        }
        
        // while there are more arguments
        while(_argsSet.Dequeue(out args)) {
          
          _lock.Take();
          try {
            // get the next set of arguments
            Action.ArgA = args.ArgA;
            Action.ArgB = args.ArgB;
            Action.ArgC = args.ArgC;
            Action.ArgD = args.ArgD;
            
            // run the action
            Action.Run();
          } finally {
            _lock.Release();
          }
          
        }
        
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
  public class ActionRoll<A,B,C,D,E> : IAction<A,B,C,D,E> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Action to be run.
    /// </summary>
    public IAction<A,B,C,D,E> Action;
    
    /// <summary>
    /// An argument to be passed to the action.
    /// </summary>
    public A ArgA {
      get { return _argsSet.Current.ArgA; }
      set { _argsSet.Current.ArgA = value; }
    }
    /// <summary>
    /// An argument to be passed to the action.
    /// </summary>
    public B ArgB {
      get { return _argsSet.Current.ArgB; }
      set { _argsSet.Current.ArgB = value; }
    }
    /// <summary>
    /// An argument to be passed to the action.
    /// </summary>
    public C ArgC {
      get { return _argsSet.Current.ArgC; }
      set { _argsSet.Current.ArgC = value; }
    }
    /// <summary>
    /// An argument to be passed to the action.
    /// </summary>
    public D ArgD {
      get { return _argsSet.Current.ArgD; }
      set { _argsSet.Current.ArgD = value; }
    }
    /// <summary>
    /// An argument to be passed to the action.
    /// </summary>
    public E ArgE {
      get { return _argsSet.Current.ArgE; }
      set { _argsSet.Current.ArgE = value; }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Collection of arguments to be passed to the IAction.
    /// </summary>
    protected SafeQueue<IArgs<A,B,C,D,E>> _argsSet;
    /// <summary>
    /// Lock used when dequeuing arguments.
    /// </summary>
    protected Lock _lock;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionRoll() {
      _lock = new Lock();
      _argsSet = new SafeQueue<IArgs<A,B,C,D,E>>();
    }
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionRoll(IAction<A,B,C,D,E> action) {
      _argsSet = new SafeQueue<IArgs<A,B,C,D,E>>();
      _lock = new Lock();
      Action = action;
    }
    
    public ActionRoll(Action<A,B,C,D,E> action) {
      _argsSet = new SafeQueue<IArgs<A,B,C,D,E>>();
      _lock = new Lock();
      Action = new ActionSet<A,B,C,D,E>(action);
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
        ArgE = e,
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
      _argsSet.Enqueue(new Args<A,B,C,D,E> { ArgA = a, ArgB = b, ArgC = c, ArgD = d, ArgE = e, });
      Run();
    }
    
    /// <summary>
    /// Run a single set of arguments.
    /// </summary>
    public void Run() {
      IArgs<A,B,C,D,E> args;
      
      // are there arguments?
      if(_argsSet.Dequeue(out args)) {
        
        _lock.Take();
        try {
          // yes, get the next set of arguments
          Action.ArgA = args.ArgA;
          Action.ArgB = args.ArgB;
          Action.ArgC = args.ArgC;
          Action.ArgD = args.ArgD;
          Action.ArgE = args.ArgE;
          
          // run the action
          Action.Run();
        } finally {
          _lock.Release();
        }
        
        // while there are more arguments
        while(_argsSet.Dequeue(out args)) {
          
          _lock.Take();
          try {
            // get the next set of arguments
            Action.ArgA = args.ArgA;
            Action.ArgB = args.ArgB;
            Action.ArgC = args.ArgC;
            Action.ArgD = args.ArgD;
            Action.ArgE = args.ArgE;
            
            // run the action
            Action.Run();
          } finally {
            _lock.Release();
          }
          
        }
        
      }
      
    }
    
    public override string ToString() {
      return "[ActionRoll Action="+Action.ToString()+", ArgsSet="+_argsSet.ToString()+"]";
    }
    
    //-------------------------------------------//
    
  }


}
