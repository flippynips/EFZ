using System;
using System.Linq;

namespace Efz {
  
  /// <summary>
  /// Initialize a sequence of actions to be called in place of an ActionSet.
  /// </summary>
  public class ActionPair : IAction {
    
    //-------------------------------------------//
    
    public IAction ActionA;
    public IAction ActionB;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Required empty constructor.
    /// </summary>
    public ActionPair() {}
    
    /// <summary>
    /// Initialize with two action types.
    /// </summary>
    public ActionPair(IAction actionA, IAction actionB) {
      ActionA = actionA;
      ActionB = actionB;
    }
    public ActionPair(IAction actionA, Action actionB) {
      ActionA = actionA;
      ActionB = new ActionSet(actionB);
    }
    public ActionPair(Action actionA, IAction actionB) {
      ActionA = new ActionSet(actionA);
      ActionB = actionB;
    }
    public ActionPair(Action actionA, Action actionB) {
      ActionA = new ActionSet(actionA);
      ActionB = new ActionSet(actionB);
    }
    
    public virtual void Run() {
      ActionA.Run();
      ActionB.Run();
    }
    
    public override string ToString() {
      return string.Format("[ActionPair ActionA={0}, ActionB={1}]", ActionA, ActionB);
    }
    
    //-------------------------------------------//
    
  }
  
  public class ActionPair<A> : IRun, IAction, IAction<A> {
    
    //-------------------------------------------//
    
    public IAction<A> ActionA;
    public IAction<A> ActionB;
    
    public A ArgA {
      get { return ActionA.ArgA; }
      set { ActionA.ArgA = ActionB.ArgA = value; }
    }
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Required empty constructor.
    /// </summary>
    public ActionPair() {}
    
    /// <summary>
    /// Initialize with at least one action.
    /// </summary>
    public ActionPair(IAction<A> actionA, IAction<A> actionB) {
      ActionA = actionA;
      ActionB = actionB;
    }
    public ActionPair(IAction<A> actionA, Action<A> actionB) {
      ActionA = actionA;
      ActionB = new ActionSet<A>(actionB);
    }
    public ActionPair(Action<A> actionA, IAction<A> actionB) {
      ActionA = new ActionSet<A>(actionA);
      ActionB = actionB;
    }
    public ActionPair(Action<A> actionA, Action<A> actionB) {
      ActionA = new ActionSet<A>(actionA);
      ActionB = new ActionSet<A>(actionB);
    }
    
    public void Run() {
      ActionA.Run();
      ActionB.Run();
    }
    
    public void Run(A a) {
      ActionA.ArgA = ActionB.ArgA = a;
      ActionA.Run();
      ActionB.Run();
    }
    
    public override string ToString() {
      return string.Format("[ActionPair ActionA={0}, ActionB={1}]", ActionA, ActionB);
    }
    
    //-------------------------------------------//
    
  }
  
  public class ActionPair<A,B> : IAction, IAction<A>, IAction<A,B> {
    
    //-------------------------------------------//
    
    public IAction<A,B> ActionA;
    public IAction<A,B> ActionB;
    
    public A ArgA {
      get { return ActionA.ArgA; }
      set { ActionA.ArgA = ActionB.ArgA = value; }
    }
    
    public B ArgB {
      get { return ActionA.ArgB; }
      set { ActionA.ArgB = ActionB.ArgB = value; }
    }
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Required empty constructor.
    /// </summary>
    public ActionPair() {}
    
    /// <summary>
    /// Initialize with at least one action.
    /// </summary>
    public ActionPair(IAction<A,B> actionA, IAction<A,B> actionB) {
      ActionA = actionA;
      ActionB = actionB;
    }
    public ActionPair(IAction<A,B> actionA, Action<A,B> actionB) {
      ActionA = actionA;
      ActionB = new ActionSet<A,B>(actionB);
    }
    public ActionPair(Action<A,B> actionA, IAction<A,B> actionB) {
      ActionA = new ActionSet<A,B>(actionA);
      ActionB = actionB;
    }
    public ActionPair(Action<A,B> actionA, Action<A,B> actionB) {
      ActionA = new ActionSet<A,B>(actionA);
      ActionB = new ActionSet<A,B>(actionB);
    }
    
    public void Run() {
      ActionA.Run();
      ActionB.Run();
    }
    
    public void Run(A a, B b) {
      IAction<A,B> actionSetA = ActionA as IAction<A,B>;
      IAction<A,B> actionSetB = ActionB as IAction<A,B>;
      actionSetA.ArgA = actionSetB.ArgA = a;
      actionSetA.ArgB = actionSetB.ArgB = b;
      ActionA.Run();
      ActionB.Run();
    }
    
    public override string ToString() {
      return string.Format("[ActionPair ActionA={0}, ActionB={1}]", ActionA, ActionB);
    }
    
    //-------------------------------------------//
    
  }
  
  public class ActionPair<A,B,C> : IAction, IAction<A>, IAction<A,B>, IAction<A,B,C> {
    
    //-------------------------------------------//
    
    public IAction<A,B,C> ActionA;
    public IAction<A,B,C> ActionB;
    
    public A ArgA {
      get { return ActionA.ArgA; }
      set { ActionA.ArgA = ActionB.ArgA = value; }
    }
    
    public B ArgB {
      get { return ActionA.ArgB; }
      set { ActionA.ArgB = ActionB.ArgB = value; }
    }
    
    public C ArgC {
      get { return ActionA.ArgC; }
      set { ActionA.ArgC = ActionB.ArgC = value; }
    }
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Required empty constructor.
    /// </summary>
    public ActionPair() {}
    
    /// <summary>
    /// Initialize with at least one action.
    /// </summary>
    public ActionPair(IAction<A,B,C> actionA, IAction<A,B,C> actionB) {
      ActionA = actionA;
      ActionB = actionB;
    }
    public ActionPair(IAction<A,B,C> actionA, Action<A,B,C> actionB) {
      ActionA = actionA;
      ActionB = new ActionSet<A,B,C>(actionB);
    }
    public ActionPair(Action<A,B,C> actionA, IAction<A,B,C> actionB) {
      ActionA = new ActionSet<A,B,C>(actionA);
      ActionB = actionB;
    }
    public ActionPair(Action<A,B,C> actionA, Action<A,B,C> actionB) {
      ActionA = new ActionSet<A,B,C>(actionA);
      ActionB = new ActionSet<A,B,C>(actionB);
    }
    
    public void Run() {
      ActionA.Run();
      ActionB.Run();
    }
    
    public void Run(A a, B b, C c) {
      IAction<A,B,C> actionSetA = ActionA as IAction<A,B,C>;
      IAction<A,B,C> actionSetB = ActionB as IAction<A,B,C>;
      actionSetA.ArgA = actionSetB.ArgA = a;
      actionSetA.ArgB = actionSetB.ArgB = b;
      actionSetA.ArgC = actionSetB.ArgC = c;
      ActionA.Run();
      ActionB.Run();
    }
    
    public override string ToString() {
      return string.Format("[ActionPair ActionA={0}, ActionB={1}]", ActionA, ActionB);
    }
    
    //-------------------------------------------//
    
  }
  
  public class ActionPair<A,B,C,D> : IAction, IAction<A>, IAction<A,B>, IAction<A,B,C>, IAction<A,B,C,D> {
    
    //-------------------------------------------//
    
    public IAction<A,B,C,D> ActionA;
    public IAction<A,B,C,D> ActionB;
    
    public A ArgA {
      get { return ActionA.ArgA; }
      set { ActionA.ArgA = ActionB.ArgA = value; }
    }
    
    public B ArgB {
      get { return ActionA.ArgB; }
      set { ActionA.ArgB = ActionB.ArgB = value; }
    }
    
    public C ArgC {
      get { return ActionA.ArgC; }
      set { ActionA.ArgC = ActionB.ArgC = value; }
    }
    
    public D ArgD {
      get { return ActionA.ArgD; }
      set { ActionA.ArgD = ActionB.ArgD = value; }
    }
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Required empty constructor.
    /// </summary>
    public ActionPair() {}
    
    /// <summary>
    /// Initialize with at least one action.
    /// </summary>
    public ActionPair(IAction<A,B,C,D> actionA, IAction<A,B,C,D> actionB) {
      ActionA = actionA;
      ActionB = actionB;
    }
    public ActionPair(IAction<A,B,C,D> actionA, Action<A,B,C,D> actionB) {
      ActionA = actionA;
      ActionB = new ActionSet<A,B,C,D>(actionB);
    }
    public ActionPair(Action<A,B,C,D> actionA, IAction<A,B,C,D> actionB) {
      ActionA = new ActionSet<A,B,C,D>(actionA);
      ActionB = actionB;
    }
    public ActionPair(Action<A,B,C,D> actionA, Action<A,B,C,D> actionB) {
      ActionA = new ActionSet<A,B,C,D>(actionA);
      ActionB = new ActionSet<A,B,C,D>(actionB);
    }
    
    public void Run() {
      ActionA.Run();
      ActionB.Run();
    }
    
    public void Run(A a, B b, C c, D d) {
      IAction<A,B,C,D> actionSetA = ActionA as IAction<A,B,C,D>;
      IAction<A,B,C,D> actionSetB = ActionB as IAction<A,B,C,D>;
      actionSetA.ArgA = actionSetB.ArgA = a;
      actionSetA.ArgB = actionSetB.ArgB = b;
      actionSetA.ArgC = actionSetB.ArgC = c;
      actionSetA.ArgD = actionSetB.ArgD = d;
      ActionA.Run();
      ActionB.Run();
    }
    
    public override string ToString() {
      return string.Format("[ActionPair ActionA={0}, ActionB={1}]", ActionA, ActionB);
    }
    
    //-------------------------------------------//
    
  }
  
  public class ActionPair<A,B,C,D,E> : IAction, IAction<A>, IAction<A,B>, IAction<A,B,C>, IAction<A,B,C,D>, IAction<A,B,C,D,E> {
    
    //-------------------------------------------//
    
    public IAction<A,B,C,D,E> ActionA;
    public IAction<A,B,C,D,E> ActionB;
    
    public A ArgA {
      get { return ActionA.ArgA; }
      set { ActionA.ArgA = ActionB.ArgA = value; }
    }
    
    public B ArgB {
      get { return ActionA.ArgB; }
      set { ActionA.ArgB = ActionB.ArgB = value; }
    }
    
    public C ArgC {
      get { return ActionA.ArgC; }
      set { ActionA.ArgC = ActionB.ArgC = value; }
    }
    
    public D ArgD {
      get { return ActionA.ArgD; }
      set { ActionA.ArgD = ActionB.ArgD = value; }
    }
    
    public E ArgE {
      get { return ActionA.ArgE; }
      set { ActionA.ArgE = ActionB.ArgE = value; }
    }
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Required empty constructor.
    /// </summary>
    public ActionPair() {}
    
    /// <summary>
    /// Initialize with at least one action.
    /// </summary>
    public ActionPair(IAction<A,B,C,D,E> actionA, IAction<A,B,C,D,E> actionB) {
      ActionA = actionA;
      ActionB = actionB;
    }
    public ActionPair(IAction<A,B,C,D,E> actionA, Action<A,B,C,D,E> actionB) {
      ActionA = actionA;
      ActionB = new ActionSet<A, B, C, D, E>(actionB);
    }
    public ActionPair(Action<A,B,C,D,E> actionA, IAction<A,B,C,D,E> actionB) {
      ActionA = new ActionSet<A,B,C,D,E>(actionA);
      ActionB = actionB;
    }
    public ActionPair(Action<A,B,C,D,E> actionA, Action<A,B,C,D,E> actionB) {
      ActionA = new ActionSet<A,B,C,D,E>(actionA);
      ActionB = new ActionSet<A,B,C,D,E>(actionB);
    }
    
    public void Run() {
      ActionA.Run();
      ActionB.Run();
    }
    
    public void Run(A a, B b, C c, D d, E e) {
      IAction<A,B,C,D,E> actionSetA = ActionA as IAction<A,B,C,D,E>;
      IAction<A,B,C,D,E> actionSetB = ActionB as IAction<A,B,C,D,E>;
      actionSetA.ArgA = actionSetB.ArgA = a;
      actionSetA.ArgB = actionSetB.ArgB = b;
      actionSetA.ArgC = actionSetB.ArgC = c;
      actionSetA.ArgD = actionSetB.ArgD = d;
      actionSetA.ArgE = actionSetB.ArgE = e;
      ActionA.Run();
      ActionB.Run();
    }
    
    public override string ToString() {
      return string.Format("[ActionPair ActionA={0}, ActionB={1}]", ActionA, ActionB);
    }
    
    //-------------------------------------------//
    
  }
  
}
