using System;

namespace Efz {
  
  /// <summary>
  /// The default wrapper for action delegates. Has arguments that can be assigned independent of execution.
  /// </summary>
  public class ActionSet :
    IAction,
    IEquatable<Action>,
    IEquatable<ActionSet> {
    
    //-------------------------------------------//
    
    public Action Action {get;set;}
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionSet() {}
    
    /// <summary>
    /// Construct an action.
    /// </summary>
    public ActionSet(Action action) {
      Action = action;
    }
    
    /// <summary>
    /// Run this action.
    /// </summary>
    virtual public void Run() {
      Action();
    }
    
    public bool Equals(Action action) {
      return action.Equals(Action);
    }
    
    public bool Equals(ActionSet action) {
      return action.Action.Equals(Action);
    }
    
    override public bool Equals(object obj) {
      if(obj == null) {
        return false;
      }
      ActionSet actionSet = obj as ActionSet;
      if(actionSet == null) {
        return false;
      }
      return this.Equals(actionSet);
    }
    
    override public int GetHashCode() {
      return Action.GetHashCode();
    }
    
    static public implicit operator ActionSet(Action action) {
      return new ActionSet(action);
    }
    
    public override string ToString() {
      return string.Format("[ActionSet Target={0}, Method={1}]", Action.Method.DeclaringType, Action.Method);
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Create a new action set from the specified parameters.
    /// </summary>
    public static ActionSet New(Action action) {
      return new ActionSet(action);
    }
    
    /// <summary>
    /// Create a new action set from the specified parameters.
    /// </summary>
    public static ActionSet<A> New<A>(Action<A> action, A a) {
      return new ActionSet<A>(action, a);
    }
    
    /// <summary>
    /// Create a new action set from the specified parameters.
    /// </summary>
    public static ActionSet<A,B> New<A,B>(Action<A,B> action, A a, B b) {
      return new ActionSet<A,B>(action, a, b);
    }
    
    /// <summary>
    /// Create a new action set from the specified parameters.
    /// </summary>
    public static ActionSet<A,B,C> New<A,B,C>(Action<A,B,C> action, A a, B b, C c) {
      return new ActionSet<A,B,C>(action, a, b, c);
    }
    
    /// <summary>
    /// Create a new action set from the specified parameters.
    /// </summary>
    public static ActionSet<A,B,C,D> New<A,B,C,D>(Action<A,B,C,D> action, A a, B b, C c, D d) {
      return new ActionSet<A,B,C,D>(action, a, b, c, d);
    }
    
    /// <summary>
    /// Create a new action set from the specified parameters.
    /// </summary>
    public static ActionSet<A,B,C,D,E> New<A,B,C,D,E>(Action<A,B,C,D,E> action,
      A a, B b, C c, D d, E e) {
      return new ActionSet<A,B,C,D,E>(action, a, b, c, d, e);
    }
    
    /// <summary>
    /// Create a new action set from the specified parameters.
    /// </summary>
    public static ActionSet<A,B,C,D,E,F> New<A,B,C,D,E,F>(Action<A,B,C,D,E,F> action,
      A a, B b, C c, D d, E e, F f) {
      return new ActionSet<A,B,C,D,E,F>(action, a, b, c, d, e, f);
    }
    
    /// <summary>
    /// Create a new action set from the specified parameters.
    /// </summary>
    public static ActionSet<A,B,C,D,E,F,G> New<A,B,C,D,E,F,G>(Action<A,B,C,D,E,F,G> action,
      A a, B b, C c, D d, E e, F f, G g) {
      return new ActionSet<A,B,C,D,E,F,G>(action, a, b, c, d, e, f, g);
    }
    
    /// <summary>
    /// Create a new action set from the specified parameters.
    /// </summary>
    public static ActionSet<A,B,C,D,E,F,G,H> New<A,B,C,D,E,F,G,H>(Action<A,B,C,D,E,F,G,H> action,
      A a, B b, C c, D d, E e, F f, G g, H h) {
      return new ActionSet<A,B,C,D,E,F,G,H>(action, a, b, c, d, e, f, g, h);
    }
    
    /// <summary>
    /// Create a new action set from the specified parameters.
    /// </summary>
    public static ActionSet<A,B,C,D,E,F,G,H,I> New<A,B,C,D,E,F,G,H,I>(Action<A,B,C,D,E,F,G,H,I> action,
      A a, B b, C c, D d, E e, F f, G g, H h, I i) {
      return new ActionSet<A,B,C,D,E,F,G,H,I>(action, a, b, c, d, e, f, g, h, i);
    }
    
    /// <summary>
    /// Create a new action set from the specified parameters.
    /// </summary>
    public static ActionSet<A,B,C,D,E,F,G,H,I,J> New<A,B,C,D,E,F,G,H,I,J>(Action<A,B,C,D,E,F,G,H,I,J> action,
      A a, B b, C c, D d, E e, F f, G g, H h, I i, J j) {
      return new ActionSet<A,B,C,D,E,F,G,H,I,J>(action, a, b, c, d, e, f, g, h, i, j);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// The default wrapper for action delegates. Has arguments that can be assigned independent of execution.
  /// </summary>
  public class ActionSet<A> :
    IAction,
    IAction<A>,
    IEquatable<Action<A>>,
    IEquatable<ActionSet<A>> {
    
    //-------------------------------------------//
    
    public Action<A> Action {get;set;}
    
    public A ArgA {get;set;}
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionSet() {}
    
    public ActionSet(Action<A> action) {
      Action = action;
    }
    
    public ActionSet(Action<A> action, A a) {
      Action = action;
      ArgA = a;
    }
    
    public void Run() {
      Action(ArgA);
    }
    
    public void Run(A a) {
      Action(ArgA = a);
    }
    
    public bool Equals(Action<A> action) {
      return action.Equals(Action);
    }
    
    public bool Equals(ActionSet<A> action) {
      return action.Action.Equals(Action);
    }
    
    static public implicit operator ActionSet<A>(Action<A> action) {
      return new ActionSet<A>(action);
    }
    
    public override string ToString() {
      return string.Format("[ActionSet Target={0}, Method={1}, ArgA={2}]", Action.Method.DeclaringType, Action.Method, ArgA);
    }
    
    //-------------------------------------------//

  }
  
  /// <summary>
  /// The default wrapper for action delegates. Has arguments that can be assigned independent of execution.
  /// </summary>
  public class ActionSet<A,B> :
    IAction,
    IAction<A>,
    IAction<A,B>,
    IEquatable<Action<A,B>>,
    IEquatable<ActionSet<A,B>> {
    
    //-------------------------------------------//
    
    public Action<A,B> Action {get;set;}
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}

    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionSet() {}
    
    public ActionSet(Action<A,B> action) {
      Action = action;
    }
    
    public ActionSet(Action<A,B> action, A a, B b = default(B)) {
      Action = action;
      ArgA = a;
      ArgB = b;
    }
    
    public void Run() {
      Action(ArgA, ArgB);
    }
    
    public void Run(A a, B b) {
      Action(ArgA = a, ArgB = b);
    }
    
    public bool Equals(Action<A,B> action) {
      return action.Equals(Action);
    }
    
    public bool Equals(ActionSet<A,B> action) {
      return action.Action.Equals(Action);
    }
    
    static public implicit operator ActionSet<A,B>(Action<A,B> action) {
      return new ActionSet<A,B>(action);
    }
    
    public override string ToString() {
      return string.Format("[ActionSet Target={0}, Method={1}, ArgA={2}, ArgB={3}]", Action.Method.DeclaringType, Action.Method, ArgA, ArgB);
    }

    //-------------------------------------------//

  }
  
  /// <summary>
  /// The default wrapper for action delegates. Has arguments that can be assigned independent of execution.
  /// </summary>
  public class ActionSet<A,B,C> :
    IAction,
    IAction<A>,
    IAction<A,B>,
    IAction<A,B,C>,
    IEquatable<Action<A,B,C>>,
    IEquatable<ActionSet<A,B,C>> {
    
    //-------------------------------------------//
    
    public Action<A,B,C> Action {get;set;}
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    public C ArgC {get;set;}

    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionSet() {}
    
    public ActionSet(Action<A,B,C> action) {
      Action = action;
    }
    
    public ActionSet(Action<A,B,C> action, A a, B b = default(B), C c = default(C)) {
      Action = action;
      ArgA = a;
      ArgB = b;
      ArgC = c;
    }
    
    public void Run() {
      Action(ArgA, ArgB, ArgC);
    }
    
    public void Run(A a, B b, C c) {
      Action(ArgA = a, ArgB = b, ArgC = c);
    }
    
    public bool Equals(Action<A,B,C> action) {
      return action.Equals(Action);
    }
    
    public bool Equals(ActionSet<A,B,C> action) {
      return action.Action.Equals(Action);
    }
    
    static public implicit operator ActionSet<A,B,C>(Action<A,B,C> action) {
      return new ActionSet<A,B,C>(action);
    }
    
    public override string ToString() {
      return string.Format("[ActionSet Target={0}, Method={1}, ArgA={2}, ArgB={3}, ArgC={4}]", Action.Method.DeclaringType, Action.Method, ArgA, ArgB, ArgC);
    }

    //-------------------------------------------//

  }
  
  /// <summary>
  /// The default wrapper for action delegates. Has arguments that can be assigned independent of execution.
  /// </summary>
  public class ActionSet<A,B,C,D> :
    IAction,
    IAction<A>,
    IAction<A,B>,
    IAction<A,B,C>,
    IAction<A,B,C,D>,
    IEquatable<Action<A,B,C,D>>,
    IEquatable<ActionSet<A,B,C,D>> {
    
    //-------------------------------------------//
    
    public Action<A,B,C,D> Action {get;set;}
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    public C ArgC {get;set;}
    public D ArgD {get;set;}
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionSet() {}
    
    public ActionSet(Action<A,B,C,D> action) {
      Action = action;
    }
    
    public ActionSet(Action<A,B,C,D> action, A a, B b = default(B), C c = default(C), D d = default(D)) {
      Action = action;
      ArgA = a;
      ArgB = b;
      ArgC = c;
      ArgD = d;
    }
    
    public void Run() {
      Action(ArgA, ArgB, ArgC, ArgD);
    }
    
    public void Run(A a, B b, C c, D d) {
      Action(ArgA = a, ArgB = b, ArgC = c, ArgD = d);
    }
    
    public bool Equals(Action<A,B,C,D> action) {
      return action.Equals(Action);
    }
    
    public bool Equals(ActionSet<A,B,C,D> action) {
      return action.Action.Equals(Action);
    }
    
    static public implicit operator ActionSet<A,B,C,D>(Action<A,B,C,D> action) {
      return new ActionSet<A, B, C, D>(action);
    }
    
    public override string ToString() {
      return string.Format("[ActionSet Target={0}, Method={1}, ArgA={2}, ArgB={3}, ArgC={4}, ArgD={5}]", Action.Method.DeclaringType, Action.Method, ArgA, ArgB, ArgC, ArgD);
    }
    
    //-------------------------------------------//

  }
  
  /// <summary>
  /// The default wrapper for action delegates. Has arguments that can be assigned independent of execution.
  /// </summary>
  public class ActionSet<A,B,C,D,E> :
    IAction,
    IAction<A>,
    IAction<A,B>,
    IAction<A,B,C>,
    IAction<A,B,C,D>,
    IAction<A,B,C,D,E>,
    IEquatable<Action<A,B,C,D,E>>,
    IEquatable<ActionSet<A,B,C,D,E>>{
    
    //-------------------------------------------//
    
    public Action<A,B,C,D,E> Action {get;set;}
    
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
    public ActionSet() {}
    
    public ActionSet(Action<A,B,C,D,E> action) {
      Action = action;
    }
    
    public ActionSet(Action<A,B,C,D,E> action, A a, B b = default(B), C c = default(C), D d = default(D),
    E e = default(E)) {
      Action = action;
      ArgA = a;
      ArgB = b;
      ArgC = c;
      ArgD = d;
      ArgE = e;
    }
    
    public void Run() {
      Action(ArgA, ArgB, ArgC, ArgD, ArgE);
    }
    
    public void Run(A a, B b, C c, D d, E e) {
      Action(ArgA = a, ArgB = b, ArgC = c, ArgD = d, ArgE = e);
    }
    
    public bool Equals(Action<A,B,C,D,E> action) {
      return action.Equals(Action);
    }
    
    public bool Equals(ActionSet<A,B,C,D,E> action) {
      return action.Action.Equals(Action);
    }
    
    static public implicit operator ActionSet<A,B,C,D,E>(Action<A,B,C,D,E> action) {
      return new ActionSet<A,B,C,D,E>(action);
    }
    
    public override string ToString() {
      return "[ActionSet Target="+Action.Method.DeclaringType+", Method="+Action.Method+", A="+ArgA+", B="+ArgB+", C="+ArgC+", D="+ArgD+", E="+ArgE+"]";
    }
    
    //-------------------------------------------//

  }
  
  /// <summary>
  /// The default wrapper for action delegates. Has arguments that can be assigned independent of execution.
  /// </summary>
  public class ActionSet<A,B,C,D,E,F> :
    IAction,
    IAction<A>,
    IAction<A,B>,
    IAction<A,B,C>,
    IAction<A,B,C,D>,
    IAction<A,B,C,D,E>,
    IAction<A,B,C,D,E,F>,
    IEquatable<Action<A,B,C,D,E,F>>,
    IEquatable<ActionSet<A,B,C,D,E,F>>{
    
    //-------------------------------------------//
    
    public Action<A,B,C,D,E,F> Action {get;set;}
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    public C ArgC {get;set;}
    public D ArgD {get;set;}
    public E ArgE {get;set;}
    public F ArgF {get;set;}

    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionSet() {}
    
    public ActionSet(Action<A,B,C,D,E,F> action) {
      Action = action;
    }
    
    public ActionSet(Action<A,B,C,D,E,F> action, A a, B b = default(B), C c = default(C), D d = default(D),
    E e = default(E), F f = default(F)) {
      Action = action;
      ArgA = a;
      ArgB = b;
      ArgC = c;
      ArgD = d;
      ArgE = e;
      ArgF = f;
    }
    
    public void Run() {
      Action(ArgA, ArgB, ArgC, ArgD, ArgE, ArgF);
    }
    
    public void Run(A a, B b, C c, D d, E e, F f) {
      Action(ArgA = a, ArgB = b, ArgC = c, ArgD = d, ArgE = e, ArgF = f);
    }
    
    public bool Equals(Action<A,B,C,D,E,F> action) {
      return action.Equals(Action);
    }
    
    public bool Equals(ActionSet<A,B,C,D,E,F> action) {
      return action.Action.Equals(Action);
    }
    
    static public implicit operator ActionSet<A,B,C,D,E,F>(Action<A,B,C,D,E,F> action) {
      return new ActionSet<A,B,C,D,E,F>(action);
    }
    
    public override string ToString() {
      return "[ActionSet Target="+Action.Method.DeclaringType+", Method="+Action.Method+", A="+ArgA+", B="+ArgB+", C="+ArgC+", D="+ArgD+", E="+ArgE+", F="+ArgF+"]";
    }
    
    //-------------------------------------------//

  }
  
  /// <summary>
  /// The default wrapper for action delegates. Has arguments that can be assigned independent of execution.
  /// </summary>
  public class ActionSet<A,B,C,D,E,F,G> :
    IAction,
    IAction<A>,
    IAction<A,B>,
    IAction<A,B,C>,
    IAction<A,B,C,D>,
    IAction<A,B,C,D,E>,
    IAction<A,B,C,D,E,F>,
    IAction<A,B,C,D,E,F,G>,
    IEquatable<Action<A,B,C,D,E,F,G>>,
    IEquatable<ActionSet<A,B,C,D,E,F,G>>{
    
    //-------------------------------------------//
    
    public Action<A,B,C,D,E,F,G> Action {get;set;}
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    public C ArgC {get;set;}
    public D ArgD {get;set;}
    public E ArgE {get;set;}
    public F ArgF {get;set;}
    public G ArgG {get;set;}

    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionSet() {}
    
    public ActionSet(Action<A,B,C,D,E,F,G> action) {
      Action = action;
    }
    
    public ActionSet(Action<A,B,C,D,E,F,G> action, A a, B b = default(B), C c = default(C), D d = default(D),
    E e = default(E), F f = default(F), G g = default(G)) {
      Action = action;
      ArgA = a;
      ArgB = b;
      ArgC = c;
      ArgD = d;
      ArgE = e;
      ArgF = f;
      ArgG = g;
    }
    
    public void Run() {
      Action(ArgA, ArgB, ArgC, ArgD, ArgE, ArgF, ArgG);
    }
    
    public void Run(A a, B b, C c, D d, E e, F f, G g) {
      Action(ArgA = a, ArgB = b, ArgC = c, ArgD = d, ArgE = e, ArgF = f, ArgG = g);
    }
    
    public bool Equals(Action<A,B,C,D,E,F,G> action) {
      return action.Equals(Action);
    }
    
    public bool Equals(ActionSet<A,B,C,D,E,F,G> action) {
      return action.Action.Equals(Action);
    }
    
    static public implicit operator ActionSet<A,B,C,D,E,F,G>(Action<A,B,C,D,E,F,G> action) {
      return new ActionSet<A,B,C,D,E,F,G>(action);
    }
    
    public override string ToString() {
      return "[ActionSet Target="+Action.Method.DeclaringType+", Method="+Action.Method+", A="+ArgA+", B="+ArgB+", C="+ArgC+", D="+ArgD+", E="+ArgE+", F="+ArgF+", G="+ArgG+"]";
    }
    
    //-------------------------------------------//

  }
  
  /// <summary>
  /// The default wrapper for action delegates. Has arguments that can be assigned independent of execution.
  /// </summary>
  public class ActionSet<A,B,C,D,E,F,G,H> :
    IAction,
    IAction<A>,
    IAction<A,B>,
    IAction<A,B,C>,
    IAction<A,B,C,D>,
    IAction<A,B,C,D,E>,
    IAction<A,B,C,D,E,F>,
    IAction<A,B,C,D,E,F,G>,
    IAction<A,B,C,D,E,F,G,H>,
    IEquatable<Action<A,B,C,D,E,F,G,H>>,
    IEquatable<ActionSet<A,B,C,D,E,F,G,H>>{
    
    //-------------------------------------------//
    
    public Action<A,B,C,D,E,F,G,H> Action {get;set;}
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    public C ArgC {get;set;}
    public D ArgD {get;set;}
    public E ArgE {get;set;}
    public F ArgF {get;set;}
    public G ArgG {get;set;}
    public H ArgH {get;set;}

    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionSet() {}
    
    public ActionSet(Action<A,B,C,D,E,F,G,H> action) {
      Action = action;
    }
    
    public ActionSet(Action<A,B,C,D,E,F,G,H> action, A a, B b = default(B), C c = default(C), D d = default(D),
    E e = default(E), F f = default(F), G g = default(G), H h = default(H)) {
      Action = action;
      ArgA = a;
      ArgB = b;
      ArgC = c;
      ArgD = d;
      ArgE = e;
      ArgF = f;
      ArgG = g;
      ArgH = h;
    }
    
    public void Run() {
      Action(ArgA, ArgB, ArgC, ArgD, ArgE, ArgF, ArgG, ArgH);
    }
    
    public void Run(A a, B b, C c, D d, E e, F f, G g, H h) {
      Action(ArgA = a, ArgB = b, ArgC = c, ArgD = d, ArgE = e, ArgF = f, ArgG = g, ArgH = h);
    }
    
    public bool Equals(Action<A,B,C,D,E,F,G,H> action) {
      return action.Equals(Action);
    }
    
    public bool Equals(ActionSet<A,B,C,D,E,F,G,H> action) {
      return action.Action.Equals(Action);
    }
    
    static public implicit operator ActionSet<A,B,C,D,E,F,G,H>(Action<A,B,C,D,E,F,G,H> action) {
      return new ActionSet<A,B,C,D,E,F,G,H>(action);
    }
    
    public override string ToString() {
      return "[ActionSet Target="+Action.Method.DeclaringType+", Method="+Action.Method+", A="+ArgA+", B="+ArgB+", C="+ArgC+", D="+ArgD+", E="+ArgE+", F="+ArgF+", G="+ArgG+", H="+ArgH+"]";
    }
    
    //-------------------------------------------//

  }
  
  /// <summary>
  /// The default wrapper for action delegates. Has arguments that can be assigned independent of execution.
  /// </summary>
  public class ActionSet<A,B,C,D,E,F,G,H,I> :
    IAction,
    IAction<A>,
    IAction<A,B>,
    IAction<A,B,C>,
    IAction<A,B,C,D>,
    IAction<A,B,C,D,E>,
    IAction<A,B,C,D,E,F>,
    IAction<A,B,C,D,E,F,G>,
    IAction<A,B,C,D,E,F,G,H>,
    IAction<A,B,C,D,E,F,G,H,I>,
    IEquatable<Action<A,B,C,D,E,F,G,H,I>>,
    IEquatable<ActionSet<A,B,C,D,E,F,G,H,I>>{
    
    //-------------------------------------------//
    
    public Action<A,B,C,D,E,F,G,H,I> Action {get;set;}
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    public C ArgC {get;set;}
    public D ArgD {get;set;}
    public E ArgE {get;set;}
    public F ArgF {get;set;}
    public G ArgG {get;set;}
    public H ArgH {get;set;}
    public I ArgI {get;set;}

    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionSet() {}
    
    public ActionSet(Action<A,B,C,D,E,F,G,H,I> action) {
      Action = action;
    }
    
    public ActionSet(Action<A,B,C,D,E,F,G,H,I> action, A a, B b = default(B), C c = default(C), D d = default(D),
    E e = default(E), F f = default(F), G g = default(G), H h = default(H), I i = default(I)) {
      Action = action;
      ArgA = a;
      ArgB = b;
      ArgC = c;
      ArgD = d;
      ArgE = e;
      ArgF = f;
      ArgG = g;
      ArgH = h;
      ArgI = i;
    }
    
    public void Run() {
      Action(ArgA, ArgB, ArgC, ArgD, ArgE, ArgF, ArgG, ArgH, ArgI);
    }
    
    public void Run(A a, B b, C c, D d, E e, F f, G g, H h, I i) {
      Action(ArgA = a, ArgB = b, ArgC = c, ArgD = d, ArgE = e, ArgF = f, ArgG = g, ArgH = h, ArgI = i);
    }
    
    public bool Equals(Action<A,B,C,D,E,F,G,H,I> action) {
      return action.Equals(Action);
    }
    
    public bool Equals(ActionSet<A,B,C,D,E,F,G,H,I> action) {
      return action.Action.Equals(Action);
    }
    
    static public implicit operator ActionSet<A,B,C,D,E,F,G,H,I>(Action<A,B,C,D,E,F,G,H,I> action) {
      return new ActionSet<A,B,C,D,E,F,G,H,I>(action);
    }
    
    public override string ToString() {
      return "[ActionSet Target="+Action.Method.DeclaringType+", Method="+Action.Method+", A="+ArgA+", B="+ArgB+", C="+ArgC+", D="+ArgD+", E="+ArgE+", F="+ArgF+", G="+ArgG+", H="+ArgH+", I="+ArgI+"]";
    }
    
    //-------------------------------------------//

  }
  
  /// <summary>
  /// The default wrapper for action delegates. Has arguments that can be assigned independent of execution.
  /// </summary>
  public class ActionSet<A,B,C,D,E,F,G,H,I,J> :
    IAction,
    IAction<A>,
    IAction<A,B>,
    IAction<A,B,C>,
    IAction<A,B,C,D>,
    IAction<A,B,C,D,E>,
    IAction<A,B,C,D,E,F>,
    IAction<A,B,C,D,E,F,G>,
    IAction<A,B,C,D,E,F,G,H>,
    IAction<A,B,C,D,E,F,G,H,I>,
    IAction<A,B,C,D,E,F,G,H,I,J>,
    IEquatable<Action<A,B,C,D,E,F,G,H,I,J>>,
    IEquatable<ActionSet<A,B,C,D,E,F,G,H,I,J>>{
    
    //-------------------------------------------//
    
    public Action<A,B,C,D,E,F,G,H,I,J> Action {get;set;}
    
    public A ArgA {get;set;}
    public B ArgB {get;set;}
    public C ArgC {get;set;}
    public D ArgD {get;set;}
    public E ArgE {get;set;}
    public F ArgF {get;set;}
    public G ArgG {get;set;}
    public H ArgH {get;set;}
    public I ArgI {get;set;}
    public J ArgJ {get;set;}

    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ActionSet() {}
    
    public ActionSet(Action<A,B,C,D,E,F,G,H,I,J> action) {
      Action = action;
    }
    
    public ActionSet(Action<A,B,C,D,E,F,G,H,I,J> action, A a, B b = default(B), C c = default(C), D d = default(D),
    E e = default(E), F f = default(F), G g = default(G), H h = default(H), I i = default(I), J j = default(J)) {
      Action = action;
      ArgA = a;
      ArgB = b;
      ArgC = c;
      ArgD = d;
      ArgE = e;
      ArgF = f;
      ArgG = g;
      ArgH = h;
      ArgI = i;
      ArgJ = j;
    }
    
    public void Run() {
      Action(ArgA, ArgB, ArgC, ArgD, ArgE, ArgF, ArgG, ArgH, ArgI, ArgJ);
    }
    
    public void Run(A a, B b, C c, D d, E e, F f, G g, H h, I i, J j) {
      Action(ArgA = a, ArgB = b, ArgC = c, ArgD = d, ArgE = e, ArgF = f, ArgG = g, ArgH = h, ArgI = i, ArgJ = j);
    }
    
    public bool Equals(Action<A,B,C,D,E,F,G,H,I,J> action) {
      return action.Equals(Action);
    }
    
    public bool Equals(ActionSet<A,B,C,D,E,F,G,H,I,J> action) {
      return action.Action.Equals(Action);
    }
    
    static public implicit operator ActionSet<A,B,C,D,E,F,G,H,I,J>(Action<A,B,C,D,E,F,G,H,I,J> action) {
      return new ActionSet<A,B,C,D,E,F,G,H,I,J>(action);
    }
    
    public override string ToString() {
      return "[ActionSet Target="+Action.Method.DeclaringType+", Method="+Action.Method+", A="+ArgA+", B="+ArgB+", C="+ArgC+", D="+ArgD+", E="+ArgE+", F="+ArgF+", G="+ArgG+", H="+ArgH+", I="+ArgI+", J="+ArgJ+"]";
    }
    
    //-------------------------------------------//

  }
  
}
