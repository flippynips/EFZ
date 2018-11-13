using System;

namespace Efz {
  
  /// <summary>
  /// An action wrapper in which the arguments are retrieved at run-time.
  /// </summary>
  public class ActionPocket : IAction, IEquatable<IAction> {
    
    //-------------------------------------------//
    
    public IAction Action;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionPocket() {}
    public ActionPocket(Action _action) {
      Action = new ActionSet(_action);
    }
    public ActionPocket(IAction _action) {
      Action = _action;
    }
    
    /// <summary>
    /// Run this action.
    /// </summary>
    public void Run() {
      Action.Run();
    }
    
    public bool Equals(IAction _other) {
      return _other.Equals(Action);
    }
    
    override public bool Equals(object _object) {
      if(_object == null) {
        return false;
      }
      ActionPocket actionPocket = _object as ActionPocket;
      if(actionPocket == null) {
        return false;
      }
      return this.Equals(actionPocket);
    }
    
    override public int GetHashCode() {
      return Action.GetHashCode();
    }
    
    static public implicit operator ActionPocket(Action _action) {
      return new ActionPocket(_action);
    }
    
    public override string ToString() {
      return string.Format("[ActionPocket Action={0}]", Action);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// An action wrapper in which the arguments are retrieved at run-time.
  /// </summary>
  public class ActionPocket<A> : IAction, IAction<A>, IEquatable<Action<A>> {
    
    //-------------------------------------------//
    
    public A ArgA {
      get {
        return FuncA.Run();
      }
      set {
        #if DEBUG
        throw new ArgumentException("Pocket arguments cannot be assigned. They are retrieved with a Function.");
        #endif
      }
    }
    
    public IAction<A> Action;

    public IFunc<A> FuncA;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionPocket() {}
    public ActionPocket(Action<A> action, IFunc<A> a = null) {
      Action = new ActionSet<A>(action);
      FuncA = a;
    }
    public ActionPocket(IAction<A> action, IFunc<A> a = null) {
      Action = action;
      FuncA = a;
    }
    
    public async void Run() {
      Action.ArgA = await FuncA.RunAsync();
      Action.Run();
    }
    
    public bool Equals(Action<A> _action) {
      return _action.Equals(Action);
    }
    
    static public implicit operator ActionPocket<A>(Action<A> _action) {
      return new ActionPocket<A>(_action);
    }
    
    public override string ToString() {
      return string.Format("[ActionPocket Action={0}, FuncA={1}]", Action, FuncA);
    }
    
    //-------------------------------------------//

  }
  
  /// <summary>
  /// An action wrapper in which the arguments are retrieved at run-time.
  /// </summary>
  public class ActionPocket<A,B> : IAction, IAction<A>, IAction<A,B>, IEquatable<Action<A,B>> {
    
    //-------------------------------------------//
    
    public A ArgA {
      get {
        return FuncA.Run();
      }
      set {
        #if DEBUG
        throw new ArgumentException("Pocket arguments cannot be assigned. They are retrieved with a Function.");
        #endif
      }
    }
    public B ArgB {
      get {
        return FuncB.Run();
      }
      set {
        #if DEBUG
        throw new ArgumentException("Pocket arguments cannot be assigned. They are retrieved with a Function.");
        #endif
      }
    }
    
    public IAction<A,B> Action;
    
    public FuncSet<A> FuncA;
    public FuncSet<B> FuncB;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionPocket() {}
    public ActionPocket(Action<A,B> action, FuncSet<A> a = null, FuncSet<B> b = null) {
      Action = new ActionSet<A,B>(action);
      FuncA = a;
      FuncB = b;
    }
    public ActionPocket(IAction<A,B> action, FuncSet<A> a = null, FuncSet<B> b = null) {
      Action = action;
      FuncA = a;
      FuncB = b;
    }
    
    public async void Run() {
      Action.ArgA = await FuncA.RunAsync();
      Action.ArgB = await FuncB.RunAsync();
      Action.Run();
    }
    
    public bool Equals(Action<A,B> _action) {
      return _action.Equals(Action);
    }
    
    static public implicit operator ActionPocket<A,B>(Action<A,B> _action) {
      return new ActionPocket<A,B>(_action);
    }
    
    public override string ToString() {
      return string.Format("[ActionPocket Action={0}, FuncA={1}, FuncB={2}]", Action, FuncA, FuncB);
    }
    
    //-------------------------------------------//

  }
  
  /// <summary>
  /// An action wrapper in which the arguments are retrieved at run-time.
  /// </summary>
  public class ActionPocket<A,B,C> : IAction, IAction<A>, IAction<A,B>, IAction<A,B,C>, IEquatable<Action<A,B,C>> {
    
    //-------------------------------------------//
    
    public A ArgA {
      get {
        return FuncA.Run();
      }
      set {
        #if DEBUG
        throw new ArgumentException("Pocket arguments cannot be assigned. They are retrieved with a Function.");
        #endif
      }
    }
    public B ArgB {
      get {
        return FuncB.Run();
      }
      set {
        #if DEBUG
        throw new ArgumentException("Pocket arguments cannot be assigned. They are retrieved with a Function.");
        #endif
      }
    }
    public C ArgC {
      get {
        return FuncC.Run();
      }
      set {
        #if DEBUG
        throw new ArgumentException("Pocket arguments cannot be assigned. They are retrieved with a Function.");
        #endif
      }
    }
    
    public IAction<A,B,C> Action;
    
    public IFunc<A> FuncA;
    public IFunc<B> FuncB;
    public IFunc<C> FuncC;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionPocket() {}
    public ActionPocket(Action<A,B,C> action, IFunc<A> a = null, IFunc<B> b = null, IFunc<C> c = null) {
      Action = new ActionSet<A,B,C>(action);
      FuncA = a;
      FuncB = b;
      FuncC = c;
    }
    public ActionPocket(IAction<A,B,C> action, IFunc<A> a = null, IFunc<B> b = null, IFunc<C> c = null) {
      Action = action;
      FuncA = a;
      FuncB = b;
      FuncC = c;
    }
    
    public async void Run() {
      Action.ArgA = await FuncA.RunAsync();
      Action.ArgB = await FuncB.RunAsync();
      Action.ArgC = await FuncC.RunAsync();
      Action.Run();
    }
    
    public bool Equals(Action<A,B,C> action) {
      return action.Equals(Action);
    }
    
    static public implicit operator ActionPocket<A,B,C>(Action<A,B,C> action) {
      return new ActionPocket<A,B,C>(action);
    }
    
    public override string ToString() {
      return string.Format("[ActionPocket Action={0}, FuncA={1}, FuncB={2}, FuncC={3}]", Action, FuncA, FuncB, FuncC);
    }

    //-------------------------------------------//

  }
  
  /// <summary>
  /// An action wrapper in which the arguments are retrieved at run-time.
  /// </summary>
  public class ActionPocket<A,B,C,D> : IAction, IAction<A>, IAction<A,B>, IAction<A,B,C>, IAction<A,B,C,D>, IEquatable<Action<A,B,C,D>> {
    
    //-------------------------------------------//
    
    public A ArgA {
      get {
        return FuncA.Run();
      }
      set {
        #if DEBUG
        throw new ArgumentException("Pocket arguments cannot be assigned. They are retrieved with a Function.");
        #endif
      }
    }
    public B ArgB {
      get {
        return FuncB.Run();
      }
      set {
        #if DEBUG
        throw new ArgumentException("Pocket arguments cannot be assigned. They are retrieved with a Function.");
        #endif
      }
    }
    public C ArgC {
      get {
        return FuncC.Run();
      }
      set {
        #if DEBUG
        throw new ArgumentException("Pocket arguments cannot be assigned. They are retrieved with a Function.");
        #endif
      }
    }
    public D ArgD {
      get {
        return FuncD.Run();
      }
      set {
        #if DEBUG
        throw new ArgumentException("Pocket arguments cannot be assigned. They are retrieved with a Function.");
        #endif
      }
    }
    
    public IAction<A,B,C,D> Action;
    
    public IFunc<A> FuncA;
    public IFunc<B> FuncB;
    public IFunc<C> FuncC;
    public IFunc<D> FuncD;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionPocket() {
    }
    public ActionPocket(Action<A,B,C,D> action, IFunc<A> a = null, IFunc<B> b = null, IFunc<C> c = null, IFunc<D> d = null) {
      Action = new ActionSet<A,B,C,D>(action);
      FuncA = a;
      FuncB = b;
      FuncC = c;
      FuncD = d;
    }
    public ActionPocket(IAction<A,B,C,D> action, IFunc<A> a = null, IFunc<B> b = null, IFunc<C> c = null, IFunc<D> d = null) {
      Action = action;
      FuncA = a;
      FuncB = b;
      FuncC = c;
      FuncD = d;
    }
    
    public async void Run() {
      Action.ArgA = await FuncA.RunAsync();
      Action.ArgB = await FuncB.RunAsync();
      Action.ArgC = await FuncC.RunAsync();
      Action.ArgD = await FuncD.RunAsync();
      Action.Run();
    }
    
    public bool Equals(Action<A,B,C,D> action) {
      return action.Equals(Action);
    }
    
    static public implicit operator ActionPocket<A,B,C,D>(Action<A,B,C,D> action) {
      return new ActionPocket<A,B,C,D>(action);
    }
    
    public override string ToString() {
      return string.Format("[ActionPocket Action={0}, FuncA={1}, FuncB={2}, FuncC={3}, FuncD={4}]", Action, FuncA, FuncB, FuncC, FuncD);
    }
    
    //-------------------------------------------//

  }
  
  /// <summary>
  /// An action wrapper in which the arguments are retrieved at run-time.
  /// </summary>
  public class ActionPocket<A,B,C,D,E> : IAction, IAction<A>, IAction<A,B>, IAction<A,B,C>, IAction<A,B,C,D>, IAction<A,B,C,D,E>, IEquatable<Action<A,B,C,D,E>> {
    
    //-------------------------------------------//
    
    public A ArgA {
      get {
        return FuncA.Run();
      }
      set {
        #if DEBUG
        throw new ArgumentException("Pocket arguments cannot be assigned. They are retrieved with a Function.");
        #endif
      }
    }
    public B ArgB {
      get {
        return FuncB.Run();
      }
      set {
        #if DEBUG
        throw new ArgumentException("Pocket arguments cannot be assigned. They are retrieved with a Function.");
        #endif
      }
    }
    public C ArgC {
      get {
        return FuncC.Run();
      }
      set {
        #if DEBUG
        throw new ArgumentException("Pocket arguments cannot be assigned. They are retrieved with a Function.");
        #endif
      }
    }
    public D ArgD {
      get {
        return FuncD.Run();
      }
      set {
        #if DEBUG
        throw new ArgumentException("Pocket arguments cannot be assigned. They are retrieved with a Function.");
        #endif
      }
    }
    public E ArgE {
      get {
        return FuncE.Run();
      }
      set {
        #if DEBUG
        throw new ArgumentException("Pocket arguments cannot be assigned. They are retrieved with a Function.");
        #endif
      }
    }
    
    public IAction<A,B,C,D,E> Action;
    
    public IFunc<A> FuncA;
    public IFunc<B> FuncB;
    public IFunc<C> FuncC;
    public IFunc<D> FuncD;
    public IFunc<E> FuncE;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionPocket() {}
    public ActionPocket(Action<A,B,C,D,E> action, IFunc<A> a = null, IFunc<B> b = null, IFunc<C> c = null, IFunc<D> d = null, IFunc<E> e = null) {
      Action = new ActionSet<A,B,C,D,E>(action);
      FuncA = a;
      FuncB = b;
      FuncC = c;
      FuncD = d;
      FuncE = e;
    }
    public ActionPocket(IAction<A,B,C,D,E> action, IFunc<A> a = null, IFunc<B> b = null, IFunc<C> c = null, IFunc<D> d = null, IFunc<E> e = null) {
      Action = action;
      FuncA = a;
      FuncB = b;
      FuncC = c;
      FuncD = d;
      FuncE = e;
    }
    
    public async void Run() {
      Action.ArgA = await FuncA.RunAsync();
      Action.ArgB = await FuncB.RunAsync();
      Action.ArgC = await FuncC.RunAsync();
      Action.ArgD = await FuncD.RunAsync();
      Action.ArgE = await FuncE.RunAsync();
      Action.Run();
    }
    
    public bool Equals(Action<A,B,C,D,E> action) {
      return action.Equals(Action);
    }
    
    static public implicit operator ActionPocket<A,B,C,D,E>(Action<A,B,C,D,E> action) {
      return new ActionPocket<A,B,C,D,E>(action);
    }
    
    public override string ToString() {
      return string.Format("[ActionPocket Action={0}, FuncA={1}, FuncB={2}, FuncC={3}, FuncD={4}, FuncE={5}]", Action, FuncA, FuncB, FuncC, FuncD, FuncE);
    }
    
    //-------------------------------------------//

  }


}
