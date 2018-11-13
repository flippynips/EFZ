using System;

using Efz.Threading;

namespace Efz {
  
  /// <summary>
  /// Wrapper to reference an action and needle to run on.
  /// </summary>
  public class Act : IRun, IAction {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The action this act will run.
    /// </summary>
    public IAction Action;
    /// <summary>
    /// The needle this tasks action will be run on.
    /// </summary>
    public Needle Needle;
    
    //-------------------------------------------//
    
    public Act() {}
    public Act(IAction action, Needle needle = null) {
      Action = action;
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action action, Needle needle = null) {
      Action = new ActionSet(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    
    public void Run() {
      Needle.AddSingle(Action);
    }
    
    public static implicit operator Act(Action action) {
      return new Act(action, ManagerUpdate.Control);
    }
    
    public static Act New(Action action) {
      return new Act(action);
    }
    
    public static Act<A> New<A>(Action<A> action) {
      return new Act<A>(action);
    }
    
    public static Act<A,B> New<A,B>(Action<A,B> action) {
      return new Act<A,B>(action);
    }
    
    public static Act<A,B,C> New<A,B,C>(Action<A,B,C> action) {
      return new Act<A,B,C>(action);
    }
    
    public static Act<A,B,C,D> New<A,B,C,D>(Action<A,B,C,D> action) {
      return new Act<A,B,C,D>(action);
    }
    
    public static Act<A,B,C,D,E> New<A,B,C,D,E>(Action<A,B,C,D,E> action) {
      return new Act<A,B,C,D,E>(action);
    }
    
    public static Act<A,B,C,D,E,F> New<A,B,C,D,E,F>(Action<A,B,C,D,E,F> action) {
      return new Act<A,B,C,D,E,F>(action);
    }
    
    public static Act<A,B,C,D,E,F,G> New<A,B,C,D,E,F,G>(Action<A,B,C,D,E,F,G> action) {
      return new Act<A,B,C,D,E,F,G>(action);
    }
    
    public static Act<A,B,C,D,E,F,G,H> New<A,B,C,D,E,F,G,H>(Action<A,B,C,D,E,F,G,H> action) {
      return new Act<A,B,C,D,E,F,G,H>(action);
    }
    
    public static Act<A,B,C,D,E,F,G,H,I> New<A,B,C,D,E,F,G,H,I>(Action<A,B,C,D,E,F,G,H,I> action) {
      return new Act<A,B,C,D,E,F,G,H,I>(action);
    }
    
    public static Act<A,B,C,D,E,F,G,H,I,J> New<A,B,C,D,E,F,G,H,I,J>(Action<A,B,C,D,E,F,G,H,I,J> action) {
      return new Act<A,B,C,D,E,F,G,H,I,J>(action);
    }
    
    public static Act<A> New<A>(Action<A> action, A a) {
      return new Act<A>(action, a);
    }
    
    public static Act<A,B> New<A,B>(Action<A,B> action, A a, B b) {
      return new Act<A,B>(action, a, b);
    }
    
    public static Act<A,B,C> New<A,B,C>(Action<A,B,C> action, A a, B b, C c) {
      return new Act<A,B,C>(action, a, b, c);
    }
    
    public static Act<A,B,C,D> New<A,B,C,D>(Action<A,B,C,D> action, A a, B b, C c, D d) {
      return new Act<A,B,C,D>(action, a, b, c, d);
    }
    
    public static Act<A,B,C,D,E> New<A,B,C,D,E>(Action<A,B,C,D,E> action, A a, B b, C c, D d, E e) {
      return new Act<A,B,C,D,E>(action, a, b, c, d, e);
    }
    
    public static Act<A,B,C,D,E,F> New<A,B,C,D,E,F>(Action<A,B,C,D,E,F> action, A a, B b, C c, D d, E e, F f) {
      return new Act<A,B,C,D,E,F>(action, a, b, c, d, e, f);
    }
    
    public static Act<A,B,C,D,E,F,G> New<A,B,C,D,E,F,G>(Action<A,B,C,D,E,F,G> action, A a, B b, C c, D d, E e, F f, G g) {
      return new Act<A,B,C,D,E,F,G>(action, a, b, c, d, e, f, g);
    }
    
    public static Act<A,B,C,D,E,F,G,H> New<A,B,C,D,E,F,G,H>(Action<A,B,C,D,E,F,G,H> action, A a, B b, C c, D d, E e, F f, G g, H h) {
      return new Act<A,B,C,D,E,F,G,H>(action, a, b, c, d, e, f, g, h);
    }
    
    public static Act<A,B,C,D,E,F,G,H,I> New<A,B,C,D,E,F,G,H,I>(Action<A,B,C,D,E,F,G,H,I> action, A a, B b, C c, D d, E e, F f, G g,
      H h, I i) {
      return new Act<A,B,C,D,E,F,G,H,I>(action, a, b, c, d, e, f, g, h, i);
    }
    
    public static Act<A,B,C,D,E,F,G,H,I,J> New<A,B,C,D,E,F,G,H,I,J>(Action<A,B,C,D,E,F,G,H,I,J> action, A a, B b, C c, D d, E e, F f,
      G g, H h, I i, J j) {
      return new Act<A,B,C,D,E,F,G,H,I,J>(action, a, b, c, d, e, f, g, h, i, j);
    }
    
    public static Act New(IAction action) {
      return new Act(action);
    }
    
    public static Act<A> New<A>(IAction<A> action) {
      return new Act<A>(action);
    }
    
    public static Act<A,B> New<A,B>(IAction<A,B> action) {
      return new Act<A,B>(action);
    }
    
    public static Act<A,B,C> New<A,B,C>(IAction<A,B,C> action) {
      return new Act<A,B,C>(action);
    }
    
    public static Act<A,B,C,D> New<A,B,C,D>(IAction<A,B,C,D> action) {
      return new Act<A,B,C,D>(action);
    }
    
    public static Act<A,B,C,D,E> New<A,B,C,D,E>(IAction<A,B,C,D,E> action) {
      return new Act<A,B,C,D,E>(action);
    }
    
    public static Act<A,B,C,D,E,F> New<A,B,C,D,E,F>(IAction<A,B,C,D,E,F> action) {
      return new Act<A,B,C,D,E,F>(action);
    }
    
    public static Act<A,B,C,D,E,F,G> New<A,B,C,D,E,F,G>(IAction<A,B,C,D,E,F,G> action) {
      return new Act<A,B,C,D,E,F,G>(action);
    }
    
    public static Act<A,B,C,D,E,F,G,H> New<A,B,C,D,E,F,G,H>(IAction<A,B,C,D,E,F,G,H> action) {
      return new Act<A,B,C,D,E,F,G,H>(action);
    }
    
    public static Act<A,B,C,D,E,F,G,H,I> New<A,B,C,D,E,F,G,H,I>(IAction<A,B,C,D,E,F,G,H,I> action) {
      return new Act<A,B,C,D,E,F,G,H,I>(action);
    }
    
    public static Act<A,B,C,D,E,F,G,H,I,J> New<A,B,C,D,E,F,G,H,I,J>(IAction<A,B,C,D,E,F,G,H,I,J> action) {
      return new Act<A,B,C,D,E,F,G,H,I,J>(action);
    }
    
    public static Act<A> New<A>(IAction<A> action, A a) {
      return new Act<A>(action, a);
    }
    
    public static Act<A,B> New<A,B>(IAction<A,B> action, A a, B b) {
      return new Act<A,B>(action, a, b);
    }
    
    public static Act<A,B,C> New<A,B,C>(IAction<A,B,C> action, A a, B b, C c) {
      return new Act<A,B,C>(action, a, b, c);
    }
    
    public static Act<A,B,C,D> New<A,B,C,D>(IAction<A,B,C,D> action, A a, B b, C c, D d) {
      return new Act<A,B,C,D>(action, a, b, c, d);
    }
    
    public static Act<A,B,C,D,E> New<A,B,C,D,E>(IAction<A,B,C,D,E> action, A a, B b, C c, D d, E e) {
      return new Act<A,B,C,D,E>(action, a, b, c, d, e);
    }
    
    public static Act<A,B,C,D,E,F> New<A,B,C,D,E,F>(IAction<A,B,C,D,E,F> action, A a, B b, C c, D d, E e, F f) {
      return new Act<A,B,C,D,E,F>(action, a, b, c, d, e, f);
    }
    
    public static Act<A,B,C,D,E,F,G> New<A,B,C,D,E,F,G>(IAction<A,B,C,D,E,F,G> action, A a, B b, C c, D d, E e, F f, G g) {
      return new Act<A,B,C,D,E,F,G>(action, a, b, c, d, e, f, g);
    }
    
    public static Act<A,B,C,D,E,F,G,H> New<A,B,C,D,E,F,G,H>(IAction<A,B,C,D,E,F,G,H> action, A a, B b, C c, D d, E e, F f, G g, H h) {
      return new Act<A,B,C,D,E,F,G,H>(action, a, b, c, d, e, f, g, h);
    }
    
    public static Act<A,B,C,D,E,F,G,H,I> New<A,B,C,D,E,F,G,H,I>(IAction<A,B,C,D,E,F,G,H,I> action, A a, B b, C c, D d, E e, F f, G g,
      H h, I i) {
      return new Act<A,B,C,D,E,F,G,H,I>(action, a, b, c, d, e, f, g, h, i);
    }
    
    public static Act<A,B,C,D,E,F,G,H,I,J> New<A,B,C,D,E,F,G,H,I,J>(IAction<A,B,C,D,E,F,G,H,I,J> action, A a, B b, C c, D d, E e, F f,
      G g, H h, I i, J j) {
      return new Act<A,B,C,D,E,F,G,H,I,J>(action, a, b, c, d, e, f, g, h, i, j);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Wrapper to reference an action and needle to run on.
  /// </summary>
  public class Act<A> : IRun, IAction, IAction<A> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The action this task will run.
    /// </summary>
    public IAction<A> Action;
    /// <summary>
    /// The needle this tasks action will be run on.
    /// </summary>
    public Needle Needle;
    
    public A ArgA { get { return Action.ArgA; } set { Action.ArgA = value; } }
    
    //-------------------------------------------//
    
    public Act() {}
    public Act(IAction<A> action, Needle needle = null) {
      Action = action;
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A> action, Needle needle = null) {
      Action = new ActionSet<A>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A> action, A a, Needle needle = null) {
      Action = new ActionSet<A>(action);
      Needle = needle ?? ManagerUpdate.Control;
      Action.ArgA = a;
    }
    public Act(IAction<A> action, A a, Needle needle = null) {
      Needle = needle ?? ManagerUpdate.Control;
      action.ArgA = a;
      Action = action;
    }
    
    public void Run() {
      Needle.AddSingle(Action);
    }
    
    public static implicit operator Act<A>(Action<A> action) {
      return new Act<A>(action, ManagerUpdate.Control);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Wrapper to reference an action and needle to run on.
  /// </summary>
  public class Act<A,B> : IRun, IAction, IAction<A>, IAction<A,B> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The action this task will run.
    /// </summary>
    public IAction<A,B> Action;
    /// <summary>
    /// The needle this tasks action will be run on.
    /// </summary>
    public Needle Needle;
    
    public A ArgA { get { return Action.ArgA; } set { Action.ArgA = value; } }
    public B ArgB { get { return Action.ArgB; } set { Action.ArgB = value; } }
    
    //-------------------------------------------//
    
    public Act() {}
    public Act(IAction<A,B> action, Needle needle = null) {
      Action = action;
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A,B> action, Needle needle = null) {
      Action = new ActionSet<A,B>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A,B> action, A a, B b, Needle needle = null) {
      ActionSet<A,B> iAction = new ActionSet<A,B>(action);
      Needle = needle ?? ManagerUpdate.Control;
      iAction.ArgA = a;
      iAction.ArgB = b;
      Action = iAction;
    }
    public Act(IAction<A,B> action, A a, B b, Needle needle = null) {
      Needle = needle ?? ManagerUpdate.Control;
      action.ArgA = a;
      action.ArgB = b;
      Action = action;
    }
    
    public void Run() {
      Needle.AddSingle(Action);
    }
    
    public static implicit operator Act<A,B>(Action<A,B> action) {
      return new Act<A,B>(action, ManagerUpdate.Control);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Wrapper to reference an action and needle to run on.
  /// </summary>
  public class Act<A,B,C> : IRun, IAction<A>, IAction<A,B>, IAction<A,B,C> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The action this task will run.
    /// </summary>
    public IAction<A,B,C> Action;
    /// <summary>
    /// The needle this tasks action will be run on.
    /// </summary>
    public Needle Needle;
    
    public A ArgA { get { return Action.ArgA; } set { Action.ArgA = value; } }
    public B ArgB { get { return Action.ArgB; } set { Action.ArgB = value; } }
    public C ArgC { get { return Action.ArgC; } set { Action.ArgC = value; } }
    
    //-------------------------------------------//
    
    public Act() {}
    public Act(IAction<A,B,C> action, Needle needle = null) {
      Action = action;
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A,B,C> action, Needle needle = null) {
      Action = new ActionSet<A,B,C>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A,B,C> action, A a, B b, C c, Needle needle = null) {
      ActionSet<A,B,C> iAction = new ActionSet<A,B,C>(action);
      Needle = needle ?? ManagerUpdate.Control;
      iAction.ArgA = a;
      iAction.ArgB = b;
      iAction.ArgC = c;
      Action = iAction;
    }
    public Act(IAction<A,B,C> action, A a, B b, C c, Needle needle = null) {
      Needle = needle ?? ManagerUpdate.Control;
      action.ArgA = a;
      action.ArgB = b;
      action.ArgC = c;
      Action = action;
    }
    
    public void Run() {
      Needle.AddSingle(Action);
    }
    
    public static implicit operator Act<A,B,C>(Action<A,B,C> action) {
      return new Act<A,B,C>(action, ManagerUpdate.Control);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Wrapper to reference an action and needle to run on.
  /// </summary>
  public class Act<A,B,C,D> : IRun, IAction, IAction<A>, IAction<A,B>, IAction<A,B,C>, IAction<A,B,C,D> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The action this task will run.
    /// </summary>
    public IAction<A,B,C,D> Action;
    /// <summary>
    /// The needle this tasks action will be run on.
    /// </summary>
    public Needle Needle;
    
    public A ArgA { get { return Action.ArgA; } set { Action.ArgA = value; } }
    public B ArgB { get { return Action.ArgB; } set { Action.ArgB = value; } }
    public C ArgC { get { return Action.ArgC; } set { Action.ArgC = value; } }
    public D ArgD { get { return Action.ArgD; } set { Action.ArgD = value; } }
    
    //-------------------------------------------//
    
    public Act() {}
    public Act(IAction<A,B,C,D> action, Needle needle = null) {
      Action = action;
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A,B,C,D> action, Needle needle = null) {
      Action = new ActionSet<A,B,C,D>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A,B,C,D> action, A a, B b, C c, D d, Needle needle = null) {
      ActionSet<A,B,C,D> iAction = new ActionSet<A,B,C,D>(action);
      Needle = needle ?? ManagerUpdate.Control;
      iAction.ArgA = a;
      iAction.ArgB = b;
      iAction.ArgC = c;
      iAction.ArgD = d;
      Action = iAction;
    }
    public Act(IAction<A,B,C,D> action, A a, B b, C c, D d, Needle needle = null) {
      Needle = needle ?? ManagerUpdate.Control;
      action.ArgA = a;
      action.ArgB = b;
      action.ArgC = c;
      action.ArgD = d;
      Action = action;
    }
    
    public void Run() {
      Needle.AddSingle(Action);
    }
    
    public static implicit operator Act<A,B,C,D>(Action<A,B,C,D> action) {
      return new Act<A,B,C,D>(action, ManagerUpdate.Control);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Wrapper to reference an action and needle to run on.
  /// </summary>
  public class Act<A,B,C,D,E> : IRun, IAction, IAction<A>, IAction<A,B>, IAction<A,B,C>, IAction<A,B,C,D>, IAction<A,B,C,D,E> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The action this task will run.
    /// </summary>
    public IAction<A,B,C,D,E> Action;
    /// <summary>
    /// The needle this tasks action will be run on.
    /// </summary>
    public Needle Needle;
    
    public A ArgA { get { return Action.ArgA; } set { Action.ArgA = value; } }
    public B ArgB { get { return Action.ArgB; } set { Action.ArgB = value; } }
    public C ArgC { get { return Action.ArgC; } set { Action.ArgC = value; } }
    public D ArgD { get { return Action.ArgD; } set { Action.ArgD = value; } }
    public E ArgE { get { return Action.ArgE; } set { Action.ArgE = value; } }
    
    //-------------------------------------------//
    
    public Act() {}
    public Act(IAction<A,B,C,D,E> action, Needle needle = null) {
      Action = action;
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A,B,C,D,E> action, Needle needle = null) {
      Action = new ActionSet<A,B,C,D,E>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A,B,C,D,E> action, A a, B b, C c, D d, E e, Needle needle = null) {
      ActionSet<A,B,C,D,E> iAction = new ActionSet<A,B,C,D,E>(action);
      Needle = needle ?? ManagerUpdate.Control;
      iAction.ArgA = a;
      iAction.ArgB = b;
      iAction.ArgC = c;
      iAction.ArgD = d;
      iAction.ArgE = e;
      Action = iAction;
    }
    public Act(IAction<A,B,C,D,E> action, A a, B b, C c, D d, E e, Needle needle = null) {
      Needle = needle ?? ManagerUpdate.Control;
      action.ArgA = a;
      action.ArgB = b;
      action.ArgC = c;
      action.ArgD = d;
      action.ArgE = e;
      Action = action;
    }
    
    public void Run() {
      Needle.AddSingle(Action);
    }
    
    public static implicit operator Act<A,B,C,D,E>(Action<A,B,C,D,E> action) {
      return new Act<A,B,C,D,E>(action, ManagerUpdate.Control);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Wrapper to reference an action and needle to run on.
  /// </summary>
  public class Act<A,B,C,D,E,F> : IRun, IAction, IAction<A>, IAction<A,B>, IAction<A,B,C>, IAction<A,B,C,D>, IAction<A,B,C,D,E>,
    IAction<A,B,C,D,E,F> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The action this task will run.
    /// </summary>
    public IAction<A,B,C,D,E,F> Action;
    /// <summary>
    /// The needle this tasks action will be run on.
    /// </summary>
    public Needle Needle;
    
    public A ArgA { get { return Action.ArgA; } set { Action.ArgA = value; } }
    public B ArgB { get { return Action.ArgB; } set { Action.ArgB = value; } }
    public C ArgC { get { return Action.ArgC; } set { Action.ArgC = value; } }
    public D ArgD { get { return Action.ArgD; } set { Action.ArgD = value; } }
    public E ArgE { get { return Action.ArgE; } set { Action.ArgE = value; } }
    public F ArgF { get { return Action.ArgF; } set { Action.ArgF = value; } }
    
    //-------------------------------------------//
    
    public Act() {}
    public Act(IAction<A,B,C,D,E,F> action, Needle needle = null) {
      Action = action;
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A,B,C,D,E,F> action, Needle needle = null) {
      Action = new ActionSet<A,B,C,D,E,F>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A,B,C,D,E,F> action, A a, B b, C c, D d, E e, F f, Needle needle = null) {
      ActionSet<A,B,C,D,E,F> iAction = new ActionSet<A,B,C,D,E,F>(action);
      Needle = needle ?? ManagerUpdate.Control;
      iAction.ArgA = a;
      iAction.ArgB = b;
      iAction.ArgC = c;
      iAction.ArgD = d;
      iAction.ArgE = e;
      iAction.ArgF = f;
      Action = iAction;
    }
    public Act(IAction<A,B,C,D,E,F> action, A a, B b, C c, D d, E e, F f, Needle needle = null) {
      Needle = needle ?? ManagerUpdate.Control;
      action.ArgA = a;
      action.ArgB = b;
      action.ArgC = c;
      action.ArgD = d;
      action.ArgE = e;
      action.ArgF = f;
      Action = action;
    }
    
    public void Run() {
      Needle.AddSingle(Action);
    }
    
    public static implicit operator Act<A,B,C,D,E,F>(Action<A,B,C,D,E,F> action) {
      return new Act<A,B,C,D,E,F>(action, ManagerUpdate.Control);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Wrapper to reference an action and needle to run on.
  /// </summary>
  public class Act<A,B,C,D,E,F,G> : IRun, IAction, IAction<A>, IAction<A,B>, IAction<A,B,C>, IAction<A,B,C,D>, IAction<A,B,C,D,E>,
    IAction<A,B,C,D,E,F>, IAction<A,B,C,D,E,F,G> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The action this task will run.
    /// </summary>
    public IAction<A,B,C,D,E,F,G> Action;
    /// <summary>
    /// The needle this tasks action will be run on.
    /// </summary>
    public Needle Needle;
    
    public A ArgA { get { return Action.ArgA; } set { Action.ArgA = value; } }
    public B ArgB { get { return Action.ArgB; } set { Action.ArgB = value; } }
    public C ArgC { get { return Action.ArgC; } set { Action.ArgC = value; } }
    public D ArgD { get { return Action.ArgD; } set { Action.ArgD = value; } }
    public E ArgE { get { return Action.ArgE; } set { Action.ArgE = value; } }
    public F ArgF { get { return Action.ArgF; } set { Action.ArgF = value; } }
    public G ArgG { get { return Action.ArgG; } set { Action.ArgG = value; } }
    
    //-------------------------------------------//
    
    public Act() {}
    public Act(IAction<A,B,C,D,E,F,G> action, Needle needle = null) {
      Action = action;
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A,B,C,D,E,F,G> action, Needle needle = null) {
      Action = new ActionSet<A,B,C,D,E,F,G>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A,B,C,D,E,F,G> action, A a, B b, C c, D d, E e, F f, G g, Needle needle = null) {
      ActionSet<A,B,C,D,E,F,G> iAction = new ActionSet<A,B,C,D,E,F,G>(action);
      Needle = needle ?? ManagerUpdate.Control;
      iAction.ArgA = a;
      iAction.ArgB = b;
      iAction.ArgC = c;
      iAction.ArgD = d;
      iAction.ArgE = e;
      iAction.ArgF = f;
      iAction.ArgG = g;
      Action = iAction;
    }
    public Act(IAction<A,B,C,D,E,F,G> action, A a, B b, C c, D d, E e, F f, G g, Needle needle = null) {
      Needle = needle ?? ManagerUpdate.Control;
      action.ArgA = a;
      action.ArgB = b;
      action.ArgC = c;
      action.ArgD = d;
      action.ArgE = e;
      action.ArgF = f;
      action.ArgG = g;
      Action = action;
    }
    
    public void Run() {
      Needle.AddSingle(Action);
    }
    
    public static implicit operator Act<A,B,C,D,E,F,G>(Action<A,B,C,D,E,F,G> action) {
      return new Act<A,B,C,D,E,F,G>(action, ManagerUpdate.Control);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Wrapper to reference an action and needle to run on.
  /// </summary>
  public class Act<A,B,C,D,E,F,G,H> : IRun, IAction, IAction<A>, IAction<A,B>, IAction<A,B,C>, IAction<A,B,C,D>, IAction<A,B,C,D,E>,
    IAction<A,B,C,D,E,F>, IAction<A,B,C,D,E,F,G>, IAction<A,B,C,D,E,F,G,H> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The action this task will run.
    /// </summary>
    public IAction<A,B,C,D,E,F,G,H> Action;
    /// <summary>
    /// The needle this tasks action will be run on.
    /// </summary>
    public Needle Needle;
    
    public A ArgA { get { return Action.ArgA; } set { Action.ArgA = value; } }
    public B ArgB { get { return Action.ArgB; } set { Action.ArgB = value; } }
    public C ArgC { get { return Action.ArgC; } set { Action.ArgC = value; } }
    public D ArgD { get { return Action.ArgD; } set { Action.ArgD = value; } }
    public E ArgE { get { return Action.ArgE; } set { Action.ArgE = value; } }
    public F ArgF { get { return Action.ArgF; } set { Action.ArgF = value; } }
    public G ArgG { get { return Action.ArgG; } set { Action.ArgG = value; } }
    public H ArgH { get { return Action.ArgH; } set { Action.ArgH = value; } }
    
    //-------------------------------------------//
    
    public Act() {}
    public Act(IAction<A,B,C,D,E,F,G,H> action, Needle needle = null) {
      Action = action;
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A,B,C,D,E,F,G,H> action, Needle needle = null) {
      Action = new ActionSet<A,B,C,D,E,F,G,H>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A,B,C,D,E,F,G,H> action, A a, B b, C c, D d, E e, F f, G g, H h, Needle needle = null) {
      ActionSet<A,B,C,D,E,F,G,H> iAction = new ActionSet<A,B,C,D,E,F,G,H>(action);
      Needle = needle ?? ManagerUpdate.Control;
      iAction.ArgA = a;
      iAction.ArgB = b;
      iAction.ArgC = c;
      iAction.ArgD = d;
      iAction.ArgE = e;
      iAction.ArgF = f;
      iAction.ArgG = g;
      iAction.ArgH = h;
      Action = iAction;
    }
    public Act(IAction<A,B,C,D,E,F,G,H> action, A a, B b, C c, D d, E e, F f, G g, H h, Needle needle = null) {
      Needle = needle ?? ManagerUpdate.Control;
      action.ArgA = a;
      action.ArgB = b;
      action.ArgC = c;
      action.ArgD = d;
      action.ArgE = e;
      action.ArgF = f;
      action.ArgG = g;
      action.ArgH = h;
      Action = action;
    }
    
    public void Run() {
      Needle.AddSingle(Action);
    }
    
    public static implicit operator Act<A,B,C,D,E,F,G,H>(Action<A,B,C,D,E,F,G,H> action) {
      return new Act<A,B,C,D,E,F,G,H>(action, ManagerUpdate.Control);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Wrapper to reference an action and needle to run on.
  /// </summary>
  public class Act<A,B,C,D,E,F,G,H,I> : IRun, IAction, IAction<A>, IAction<A,B>, IAction<A,B,C>, IAction<A,B,C,D>, IAction<A,B,C,D,E>,
    IAction<A,B,C,D,E,F>, IAction<A,B,C,D,E,F,G>, IAction<A,B,C,D,E,F,G,H>, IAction<A,B,C,D,E,F,G,H,I> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The action this task will run.
    /// </summary>
    public IAction<A,B,C,D,E,F,G,H,I> Action;
    /// <summary>
    /// The needle this tasks action will be run on.
    /// </summary>
    public Needle Needle;
    
    public A ArgA { get { return Action.ArgA; } set { Action.ArgA = value; } }
    public B ArgB { get { return Action.ArgB; } set { Action.ArgB = value; } }
    public C ArgC { get { return Action.ArgC; } set { Action.ArgC = value; } }
    public D ArgD { get { return Action.ArgD; } set { Action.ArgD = value; } }
    public E ArgE { get { return Action.ArgE; } set { Action.ArgE = value; } }
    public F ArgF { get { return Action.ArgF; } set { Action.ArgF = value; } }
    public G ArgG { get { return Action.ArgG; } set { Action.ArgG = value; } }
    public H ArgH { get { return Action.ArgH; } set { Action.ArgH = value; } }
    public I ArgI { get { return Action.ArgI; } set { Action.ArgI = value; } }
    
    //-------------------------------------------//
    
    public Act() {}
    public Act(IAction<A,B,C,D,E,F,G,H,I> action, Needle needle = null) {
      Action = action;
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A,B,C,D,E,F,G,H,I> action, Needle needle = null) {
      Action = new ActionSet<A,B,C,D,E,F,G,H,I>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A,B,C,D,E,F,G,H,I> action, A a, B b, C c, D d, E e, F f, G g, H h, I i, Needle needle = null) {
      ActionSet<A,B,C,D,E,F,G,H,I> iAction = new ActionSet<A,B,C,D,E,F,G,H,I>(action);
      Needle = needle ?? ManagerUpdate.Control;
      iAction.ArgA = a;
      iAction.ArgB = b;
      iAction.ArgC = c;
      iAction.ArgD = d;
      iAction.ArgE = e;
      iAction.ArgF = f;
      iAction.ArgG = g;
      iAction.ArgH = h;
      iAction.ArgI = i;
      Action = iAction;
    }
    public Act(IAction<A,B,C,D,E,F,G,H,I> action, A a, B b, C c, D d, E e, F f, G g, H h, I i, Needle needle = null) {
      Needle = needle ?? ManagerUpdate.Control;
      action.ArgA = a;
      action.ArgB = b;
      action.ArgC = c;
      action.ArgD = d;
      action.ArgE = e;
      action.ArgF = f;
      action.ArgG = g;
      action.ArgH = h;
      action.ArgI = i;
      Action = action;
    }
    
    public void Run() {
      Needle.AddSingle(Action);
    }
    
    public static implicit operator Act<A,B,C,D,E,F,G,H,I>(Action<A,B,C,D,E,F,G,H,I> action) {
      return new Act<A,B,C,D,E,F,G,H,I>(action, ManagerUpdate.Control);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Wrapper to reference an action and needle to run on.
  /// </summary>
  public class Act<A,B,C,D,E,F,G,H,I,J> : IRun, IAction, IAction<A>, IAction<A,B>, IAction<A,B,C>, IAction<A,B,C,D>, IAction<A,B,C,D,E>,
    IAction<A,B,C,D,E,F>, IAction<A,B,C,D,E,F,G>, IAction<A,B,C,D,E,F,G,H>, IAction<A,B,C,D,E,F,G,H,I>, IAction<A,B,C,D,E,F,G,H,I,J> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The action this task will run.
    /// </summary>
    public IAction<A,B,C,D,E,F,G,H,I,J> Action;
    /// <summary>
    /// The needle this tasks action will be run on.
    /// </summary>
    public Needle Needle;
    
    public A ArgA { get { return Action.ArgA; } set { Action.ArgA = value; } }
    public B ArgB { get { return Action.ArgB; } set { Action.ArgB = value; } }
    public C ArgC { get { return Action.ArgC; } set { Action.ArgC = value; } }
    public D ArgD { get { return Action.ArgD; } set { Action.ArgD = value; } }
    public E ArgE { get { return Action.ArgE; } set { Action.ArgE = value; } }
    public F ArgF { get { return Action.ArgF; } set { Action.ArgF = value; } }
    public G ArgG { get { return Action.ArgG; } set { Action.ArgG = value; } }
    public H ArgH { get { return Action.ArgH; } set { Action.ArgH = value; } }
    public I ArgI { get { return Action.ArgI; } set { Action.ArgI = value; } }
    public J ArgJ { get { return Action.ArgJ; } set { Action.ArgJ = value; } }
    
    //-------------------------------------------//
    
    public Act() {}
    public Act(IAction<A,B,C,D,E,F,G,H,I,J> action, Needle needle = null) {
      Action = action;
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A,B,C,D,E,F,G,H,I,J> action, Needle needle = null) {
      Action = new ActionSet<A,B,C,D,E,F,G,H,I,J>(action);
      Needle = needle ?? ManagerUpdate.Control;
    }
    public Act(Action<A,B,C,D,E,F,G,H,I,J> action, A a, B b, C c, D d, E e, F f, G g, H h, I i, J j, Needle needle = null) {
      ActionSet<A,B,C,D,E,F,G,H,I,J> iAction = new ActionSet<A,B,C,D,E,F,G,H,I,J>(action);
      Needle = needle ?? ManagerUpdate.Control;
      iAction.ArgA = a;
      iAction.ArgB = b;
      iAction.ArgC = c;
      iAction.ArgD = d;
      iAction.ArgE = e;
      iAction.ArgF = f;
      iAction.ArgG = g;
      iAction.ArgH = h;
      iAction.ArgI = i;
      iAction.ArgJ = j;
      Action = iAction;
    }
    public Act(IAction<A,B,C,D,E,F,G,H,I,J> action, A a, B b, C c, D d, E e, F f, G g, H h, I i, J j, Needle needle = null) {
      Needle = needle ?? ManagerUpdate.Control;
      action.ArgA = a;
      action.ArgB = b;
      action.ArgC = c;
      action.ArgD = d;
      action.ArgE = e;
      action.ArgF = f;
      action.ArgG = g;
      action.ArgH = h;
      action.ArgI = i;
      action.ArgJ = j;
      Action = action;
    }
    
    public void Run() {
      Needle.AddSingle(Action);
    }
    
    public static implicit operator Act<A,B,C,D,E,F,G,H,I,J>(Action<A,B,C,D,E,F,G,H,I,J> action) {
      return new Act<A,B,C,D,E,F,G,H,I,J>(action, ManagerUpdate.Control);
    }
    
    //-------------------------------------------//
    
  }
  

}
