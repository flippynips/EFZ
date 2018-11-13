using System;
using System.Threading.Tasks;

namespace Efz {
  
  /// <summary>
  /// Static helper functions for the func set types.
  /// </summary>
  public static class FuncSet {
    
    /// <summary>
    /// Create a new function set from the specified parameters.
    /// </summary>
    public static FuncSet<TReturn> New<TReturn>(Func<TReturn> func) {
      return new FuncSet<TReturn>(func);
    }
    
    /// <summary>
    /// Create a new function set from the specified parameters.
    /// </summary>
    public static FuncSet<A,TReturn> New<A,TReturn>(Func<A,TReturn> func,A a) {
      return new FuncSet<A,TReturn>(func,a);
    }
    
    /// <summary>
    /// Create a new function set from the specified parameters.
    /// </summary>
    public static FuncSet<A,B,TReturn> New<A,B,TReturn>(Func<A,B,TReturn> func,A a,B b) {
      return new FuncSet<A,B,TReturn>(func,a,b);
    }
    
    /// <summary>
    /// Create a new function set from the specified parameters.
    /// </summary>
    public static FuncSet<A,B,C,TReturn> New<A,B,C,TReturn>(Func<A,B,C,TReturn> func,A a,B b,C c) {
      return new FuncSet<A,B,C,TReturn>(func,a,b,c);
    }
    
    /// <summary>
    /// Create a new function set from the specified parameters.
    /// </summary>
    public static FuncSet<A,B,C,D,TReturn> New<A,B,C,D,TReturn>(Func<A,B,C,D,TReturn> func,A a,B b,C c,D d) {
      return new FuncSet<A,B,C,D,TReturn>(func,a,b,c,d);
    }
    
    /// <summary>
    /// Create a new function set from the specified parameters.
    /// </summary>
    public static FuncSet<A,B,C,D,E,TReturn> New<A,B,C,D,E,TReturn>(Func<A,B,C,D,E,TReturn> func,A a,B b,C c,D d,E e) {
      return new FuncSet<A,B,C,D,E,TReturn>(func,a,b,c,d,e);
    }
    
    /// <summary>
    /// Create a new function set from the specified parameters.
    /// </summary>
    public static FuncSet<A,B,C,D,E,F,TReturn> New<A,B,C,D,E,F,TReturn>(Func<A,B,C,D,E,F,TReturn> func,
      A a,B b,C c,D d,E e, F f) {
      return new FuncSet<A,B,C,D,E,F,TReturn>(func,a,b,c,d,e,f);
    }
    
    /// <summary>
    /// Create a new function set from the specified parameters.
    /// </summary>
    public static FuncSet<A,B,C,D,E,F,G,TReturn> New<A,B,C,D,E,F,G,TReturn>(Func<A,B,C,D,E,F,G,TReturn> func,
      A a,B b,C c,D d,E e, F f, G g) {
      return new FuncSet<A,B,C,D,E,F,G,TReturn>(func,a,b,c,d,e,f,g);
    }
    
    /// <summary>
    /// Create a new function set from the specified parameters.
    /// </summary>
    public static FuncSet<A,B,C,D,E,F,G,H,TReturn> New<A,B,C,D,E,F,G,H,TReturn>(Func<A,B,C,D,E,F,G,H,TReturn> func,
      A a,B b,C c,D d,E e, F f, G g, H h) {
      return new FuncSet<A,B,C,D,E,F,G,H,TReturn>(func,a,b,c,d,e,f,g,h);
    }
    
    /// <summary>
    /// Create a new function set from the specified parameters.
    /// </summary>
    public static FuncSet<A,B,C,D,E,F,G,H,I,TReturn> New<A,B,C,D,E,F,G,H,I,TReturn>(Func<A,B,C,D,E,F,G,H,I,TReturn> func,
      A a,B b,C c,D d,E e, F f, G g, H h, I i) {
      return new FuncSet<A,B,C,D,E,F,G,H,I,TReturn>(func,a,b,c,d,e,f,g,h,i);
    }
    
    /// <summary>
    /// Create a new function set from the specified parameters.
    /// </summary>
    public static FuncSet<A,B,C,D,E,F,G,H,I,J,TReturn> New<A,B,C,D,E,F,G,H,I,J,TReturn>(Func<A,B,C,D,E,F,G,H,I,J,TReturn> func,
      A a,B b,C c,D d,E e, F f, G g, H h, I i, J j) {
      return new FuncSet<A,B,C,D,E,F,G,H,I,J,TReturn>(func,a,b,c,d,e,f,g,h,i,j);
    }
    
  }
  
  /// <summary>
  /// Factory async wrapper for the Func delegate.
  /// </summary>
  public class FuncSet<TReturn> :
    IFunc,
    IFunc<TReturn>,
    IEquatable<Func<TReturn>>,
    IEquatable<FuncSet<TReturn>> {
    
    //-------------------------------------------//
    
    public Func<TReturn> Func;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor for convenience.
    /// </summary>
    public FuncSet() {}
    
    public FuncSet(Func<TReturn> func) {
      Func = func;
    }
    
    public virtual TReturn Run() {
      return Func();
    }
    
    public virtual async Task<TReturn> RunAsync() {
      return await Task.Run<TReturn>(() => Func());
    }
    
    public bool Equals(FuncSet<TReturn> other) {
      return other.Func.Equals(Func);
    }
    
    public bool Equals(Func<TReturn> func) {
      return func.Equals(Func);
    }
    
    override public bool Equals(object obj) {
      if(obj == null) {
        return false;
      }
      FuncSet<TReturn> funcSet = obj as FuncSet<TReturn>;
      if(funcSet == null) {
        return false;
      }
      return this.Equals(funcSet);
    }
    
    override public int GetHashCode() {
      return Func.GetHashCode();
    }
    
    static public implicit operator FuncSet<TReturn>(Func<TReturn> func) {
      return new FuncSet<TReturn>(func);
    }
    
    public override string ToString() {
      return "[FuncSet Func="+Func+"]";
    }
    
    //-------------------------------------------//
    
  }
  
  public class FuncSet<A, TReturn> :
    IFunc,
    IFunc<TReturn>,
    IFunc<A,TReturn>,
    IArgs<A>,
    IEquatable<Func<A,TReturn>> {
    
    //-------------------------------------------//
    
    public A ArgA { get; set; }
    
    public Func<A, TReturn> Func;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    public FuncSet() {}
    
    public FuncSet(Func<A,TReturn> func) {
      Func = func;
    }
    
    public FuncSet(Func<A, TReturn> func, A a) {
      Func = func;
      ArgA = a;
    }

    public virtual TReturn Run() {
      return Func(ArgA);
    }
    
    public virtual TReturn Run(A a) {
      return Func(a);
    }
    
    public virtual async Task<TReturn> RunAsync() {
      return await Task.Run<TReturn>(() => Func(ArgA));
    }
    
    public virtual async Task<TReturn> RunAsync(A a) {
      return await Task.Run<TReturn>(() => Func(a));
    }
    
    public bool Equals(Func<A, TReturn> func) {
      return func.Equals(Func);
    }
    
    static public implicit operator FuncSet<A, TReturn>(Func<A, TReturn> func) {
      return new FuncSet<A, TReturn>(func);
    }
    
    public override string ToString() {
      return "[FuncSet Result="+Func+", ArgA="+ArgA+"]";
    }

    //-------------------------------------------//
    
  }

  public class FuncSet<A, B, TReturn> :
    IFunc,
    IFunc<TReturn>,
    IFunc<A,TReturn>,
    IFunc<A,B,TReturn>,
    IArgs<A,B>,
    IEquatable<Func<A,B,TReturn>> {
    
    //-------------------------------------------//
    
    public A ArgA { get; set; }
    public B ArgB { get; set; }
    
    public Func<A, B, TReturn> Func;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    public FuncSet() {}
    
    public FuncSet(Func<A,B,TReturn> func) {
      Func = func;
    }
    
    public FuncSet(Func<A, B, TReturn> func, A a, B b = default(B)) {
      Func = func;
      ArgA = a;
      ArgB = b;
    }

    public virtual TReturn Run() {
      return Func(ArgA, ArgB);
    }
    
    public virtual async Task<TReturn> RunAsync() {
      return await Task.Run<TReturn>(() => Func(ArgA, ArgB));
    }
    
    public virtual TReturn Run(A a, B b) {
      return Func(a, b);
    }
    
    public virtual async Task<TReturn> RunAsync(A a, B b) {
      return await Task.Run<TReturn>(() => Func(a, b));
    }
    
    public bool Equals(Func<A, B, TReturn> func) {
      return func.Equals(Func);
    }
    
    static public implicit operator FuncSet<A, B, TReturn>(Func<A, B, TReturn> func) {
      return new FuncSet<A, B, TReturn>(func);
    }
    
    public override string ToString() {
      return "[FuncSet Func="+Func+", ArgA="+ArgA+", ArgB="+ArgB+"]";
    }
    
    //-------------------------------------------//

  }

  public class FuncSet<A, B, C, TReturn> :
    IFunc,
    IFunc<TReturn>,
    IFunc<A,TReturn>,
    IFunc<A,B,TReturn>,
    IFunc<A,B,C,TReturn>,
    IArgs<A,B,C>,
    IEquatable<Func<A, B, C, TReturn>> {
    
    //-------------------------------------------//
    
    public A ArgA { get; set; }
    public B ArgB { get; set; }
    public C ArgC { get; set; }
    
    public Func<A, B, C, TReturn> Func;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    public FuncSet() {}
    
    public FuncSet(Func<A,B,C,TReturn> func) {
      Func = func;
    }
    
    public FuncSet(Func<A, B, C, TReturn> func, A a, B b = default(B), C c = default(C)) {
      Func = func;
      ArgA = a;
      ArgB = b;
      ArgC = c;
    }
    
    public virtual TReturn Run() {
      return Func(ArgA, ArgB, ArgC);
    }
    
    public virtual async Task<TReturn> RunAsync() {
      return await Task.Run<TReturn>(() => Func(ArgA, ArgB, ArgC));
    }
    
    public virtual TReturn Run(A a, B b, C c) {
      return Func(a, b, c);
    }
    
    public virtual async Task<TReturn> RunAsync(A a, B b, C c) {
      return await Task.Run<TReturn>(() => Func(a, b, c));
    }
    
    public bool Equals(Func<A, B, C, TReturn> func) {
      return func.Equals(Func);
    }
    
    static public implicit operator FuncSet<A, B, C, TReturn>(Func<A, B, C, TReturn> func) {
      return new FuncSet<A, B, C, TReturn>(func);
    }
    
    public override string ToString() {
      return "[FuncSet Func="+Func+", ArgA="+ArgA+", ArgB="+ArgB+", ArgC="+ArgC+"]";
    }
    
    //-------------------------------------------//
    
  }

  public class FuncSet<A, B, C, D, TReturn> :
    IFunc,
    IFunc<TReturn>,
    IFunc<A,TReturn>,
    IFunc<A,B,TReturn>,
    IFunc<A,B,C,TReturn>,
    IFunc<A,B,C,D,TReturn>,
    IArgs<A,B,C,D>, IEquatable<Func<A,B,C,D,TReturn>> {
    
    //-------------------------------------------//
    
    public A ArgA { get; set; }
    public B ArgB { get; set; }
    public C ArgC { get; set; }
    public D ArgD { get; set; }
    
    public Func<A, B, C, D, TReturn> Func;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    public FuncSet() {}
    
    public FuncSet(Func<A,B,C,D,TReturn> func) {
      Func = func;
    }
    
    public FuncSet(Func<A, B, C, D, TReturn> func, A a, B b = default(B), C c = default(C), D d = default(D)) {
      Func = func;
      ArgA = a;
      ArgB = b;
      ArgC = c;
      ArgD = d;
    }
    
    public virtual TReturn Run() {
      return Func(ArgA, ArgB, ArgC, ArgD);
    }
    
    public virtual async Task<TReturn> RunAsync() {
      return await Task.Run<TReturn>(() => Func(ArgA, ArgB, ArgC, ArgD));
    }
    
    public virtual TReturn Run(A a, B b, C c, D d) {
      return Func(a, b, c, d);
    }
    
    public virtual async Task<TReturn> RunAsync(A a, B b, C c, D d) {
      return await Task.Run<TReturn>(() => Func(a, b, c, d));
    }
    
    public bool Equals(Func<A, B, C, D, TReturn> func) {
      return func.Equals(Func);
    }
    
    static public implicit operator FuncSet<A, B, C, D, TReturn>(Func<A, B, C, D, TReturn> func) {
      return new FuncSet<A, B, C, D, TReturn>(func);
    }
    
    public override string ToString() {
      return "[FuncSet Func="+Func+", ArgA="+ArgA+", ArgB="+ArgB+", ArgC="+ArgC+", ArgD="+ArgD+"]";
    }
    
    //-------------------------------------------//

  }
  
  public class FuncSet<A, B, C, D, E, TReturn> :
    IFunc,
    IFunc<TReturn>,
    IFunc<A,TReturn>,
    IFunc<A,B,TReturn>,
    IFunc<A,B,C,TReturn>,
    IFunc<A,B,C,D,TReturn>,
    IFunc<A,B,C,D,E,TReturn>,
    IArgs<A,B,C,D,E>,
    IEquatable<Func<A,B,C,D,E,TReturn>> {
    
    //-------------------------------------------//
    
    public A ArgA { get; set; }
    public B ArgB { get; set; }
    public C ArgC { get; set; }
    public D ArgD { get; set; }
    public E ArgE { get; set; }
    
    public Func<A, B, C, D, E, TReturn> Func;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    public FuncSet() {}
    
    public FuncSet(Func<A,B,C,D,E,TReturn> func) {
      Func = func;
    }
    
    public FuncSet(Func<A, B, C, D, E, TReturn> func, A a, B b = default(B), C c = default(C), D d = default(D), E e = default(E)) {
      Func = func;
      ArgA = a;
      ArgB = b;
      ArgC = c;
      ArgD = d;
      ArgE = e;
    }

    public virtual TReturn Run() {
      return Func(ArgA, ArgB, ArgC, ArgD, ArgE);
    }
    
    public virtual async Task<TReturn> RunAsync() {
      return await Task.Run<TReturn>(() => Func(ArgA, ArgB, ArgC, ArgD, ArgE));
    }
    
    public virtual TReturn Run(A a, B b, C c, D d, E e) {
      return Func(a, b, c, d, e);
    }
    
    public virtual async Task<TReturn> RunAsync(A a, B b, C c, D d, E e) {
      return await Task.Run<TReturn>(() => Func(a, b, c, d, e));
    }
    
    public bool Equals(Func<A, B, C, D, E, TReturn> func) {
      return func.Equals(Func);
    }
    
    static public implicit operator FuncSet<A, B, C, D, E, TReturn>(Func<A, B, C, D, E, TReturn> func) {
      return new FuncSet<A, B, C, D, E, TReturn>(func);
    }
    
    public override string ToString() {
      return "[FuncSet Func="+Func+", ArgA="+ArgA+", ArgB="+ArgB+", ArgC="+ArgC+", ArgD="+ArgD+", ArgE="+ArgE+"]";
    }

    //-------------------------------------------//

  }
  
  public class FuncSet<A,B,C,D,E,F,TReturn> :
    IFunc,
    IFunc<TReturn>,
    IFunc<A,TReturn>,
    IFunc<A,B,TReturn>,
    IFunc<A,B,C,TReturn>,
    IFunc<A,B,C,D,TReturn>,
    IFunc<A,B,C,D,E,TReturn>,
    IFunc<A,B,C,D,E,F,TReturn>,
    IArgs<A,B,C,D,E,F>,
    IEquatable<Func<A,B,C,D,E,F,TReturn>> {
    
    //-------------------------------------------//
    
    public A ArgA { get; set; }
    public B ArgB { get; set; }
    public C ArgC { get; set; }
    public D ArgD { get; set; }
    public E ArgE { get; set; }
    public F ArgF { get; set; }
    
    public Func<A,B,C,D,E,F,TReturn> Func;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    public FuncSet() {}
    
    public FuncSet(Func<A,B,C,D,E,F,TReturn> func) {
      Func = func;
    }
    
    public FuncSet(Func<A,B,C,D,E,F,TReturn> func, A a, B b = default(B), C c = default(C), D d = default(D), E e = default(E), F f = default(F)) {
      Func = func;
      ArgA = a;
      ArgB = b;
      ArgC = c;
      ArgD = d;
      ArgE = e;
      ArgF = f;
    }

    public virtual TReturn Run() {
      return Func(ArgA, ArgB, ArgC, ArgD, ArgE, ArgF);
    }
    
    public virtual async Task<TReturn> RunAsync() {
      return await Task.Run<TReturn>(() => Func(ArgA, ArgB, ArgC, ArgD, ArgE, ArgF));
    }
    
    public virtual TReturn Run(A a, B b, C c, D d, E e, F f) {
      return Func(a, b, c, d, e, f);
    }
    
    public virtual async Task<TReturn> RunAsync(A a, B b, C c, D d, E e, F f) {
      return await Task.Run<TReturn>(() => Func(a, b, c, d, e, f));
    }
    
    public bool Equals(Func<A,B,C,D,E,F,TReturn> func) {
      return func.Equals(Func);
    }
    
    static public implicit operator FuncSet<A,B,C,D,E,F,TReturn>(Func<A,B,C,D,E,F,TReturn> func) {
      return new FuncSet<A,B,C,D,E,F,TReturn>(func);
    }
    
    public override string ToString() {
      return "[FuncSet Func="+Func+", ArgA="+ArgA+", ArgB="+ArgB+", ArgC="+ArgC+", ArgD="+ArgD+", ArgE="+ArgE+", ArgF="+ArgF+"]";
    }

    //-------------------------------------------//

  }
  
  public class FuncSet<A,B,C,D,E,F,G,TReturn> :
    IFunc,
    IFunc<TReturn>,
    IFunc<A,TReturn>,
    IFunc<A,B,TReturn>,
    IFunc<A,B,C,TReturn>,
    IFunc<A,B,C,D,TReturn>,
    IFunc<A,B,C,D,E,TReturn>,
    IFunc<A,B,C,D,E,F,TReturn>,
    IFunc<A,B,C,D,E,F,G,TReturn>,
    IArgs<A,B,C,D,E,F,G>,
    IEquatable<Func<A,B,C,D,E,F,G,TReturn>> {
    
    //-------------------------------------------//
    
    public A ArgA { get; set; }
    public B ArgB { get; set; }
    public C ArgC { get; set; }
    public D ArgD { get; set; }
    public E ArgE { get; set; }
    public F ArgF { get; set; }
    public G ArgG { get; set; }
    
    public Func<A,B,C,D,E,F,G,TReturn> Func;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    public FuncSet() {}
    
    public FuncSet(Func<A,B,C,D,E,F,G,TReturn> func) {
      Func = func;
    }
    
    public FuncSet(Func<A,B,C,D,E,F,G,TReturn> func,
      A a,
      B b = default(B),
      C c = default(C),
      D d = default(D),
      E e = default(E),
      F f = default(F),
      G g = default(G)) {
      Func = func;
      ArgA = a;
      ArgB = b;
      ArgC = c;
      ArgD = d;
      ArgE = e;
      ArgF = f;
      ArgG = g;
    }

    public virtual TReturn Run() {
      return Func(ArgA, ArgB, ArgC, ArgD, ArgE, ArgF, ArgG);
    }
    
    public virtual async Task<TReturn> RunAsync() {
      return await Task.Run<TReturn>(() => Func(ArgA, ArgB, ArgC, ArgD, ArgE, ArgF, ArgG));
    }
    
    public virtual TReturn Run(A a, B b, C c, D d, E e, F f, G g) {
      return Func(a, b, c, d, e, f, g);
    }
    
    public virtual async Task<TReturn> RunAsync(A a, B b, C c, D d, E e, F f, G g) {
      return await Task.Run<TReturn>(() => Func(a, b, c, d, e, f, g));
    }
    
    public bool Equals(Func<A,B,C,D,E,F,G,TReturn> func) {
      return func.Equals(Func);
    }
    
    static public implicit operator FuncSet<A,B,C,D,E,F,G,TReturn>(Func<A,B,C,D,E,F,G,TReturn> func) {
      return new FuncSet<A,B,C,D,E,F,G,TReturn>(func);
    }
    
    public override string ToString() {
      return "[FuncSet Func="+Func+", ArgA="+ArgA+", ArgB="+ArgB+", ArgC="+ArgC+", ArgD="+ArgD+", ArgE="+ArgE+", ArgF="+ArgF+", ArgG="+ArgG+"]";
    }

    //-------------------------------------------//

  }
  
  public class FuncSet<A,B,C,D,E,F,G,H,TReturn> :
    IFunc,
    IFunc<TReturn>,
    IFunc<A,TReturn>,
    IFunc<A,B,TReturn>,
    IFunc<A,B,C,TReturn>,
    IFunc<A,B,C,D,TReturn>,
    IFunc<A,B,C,D,E,TReturn>,
    IFunc<A,B,C,D,E,F,TReturn>,
    IFunc<A,B,C,D,E,F,G,TReturn>,
    IFunc<A,B,C,D,E,F,G,H,TReturn>,
    IArgs<A,B,C,D,E,F,G,H>,
    IEquatable<Func<A,B,C,D,E,F,G,H,TReturn>> {
    
    //-------------------------------------------//
    
    public A ArgA { get; set; }
    public B ArgB { get; set; }
    public C ArgC { get; set; }
    public D ArgD { get; set; }
    public E ArgE { get; set; }
    public F ArgF { get; set; }
    public G ArgG { get; set; }
    public H ArgH { get; set; }
    
    public Func<A,B,C,D,E,F,G,H,TReturn> Func;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    public FuncSet() {}
    
    public FuncSet(Func<A,B,C,D,E,F,G,H,TReturn> func) {
      Func = func;
    }
    
    public FuncSet(Func<A,B,C,D,E,F,G,H,TReturn> func,
      A a,
      B b = default(B),
      C c = default(C),
      D d = default(D),
      E e = default(E),
      F f = default(F),
      G g = default(G),
      H h = default(H)) {
      Func = func;
      ArgA = a;
      ArgB = b;
      ArgC = c;
      ArgD = d;
      ArgE = e;
      ArgF = f;
      ArgG = g;
      ArgH = h;
    }

    public virtual TReturn Run() {
      return Func(ArgA, ArgB, ArgC, ArgD, ArgE, ArgF, ArgG, ArgH);
    }
    
    public virtual async Task<TReturn> RunAsync() {
      return await Task.Run<TReturn>(() => Func(ArgA, ArgB, ArgC, ArgD, ArgE, ArgF, ArgG, ArgH));
    }
    
    public virtual TReturn Run(A a, B b, C c, D d, E e, F f, G g, H h) {
      return Func(a, b, c, d, e, f, g, h);
    }
    
    public virtual async Task<TReturn> RunAsync(A a, B b, C c, D d, E e, F f, G g, H h) {
      return await Task.Run<TReturn>(() => Func(a, b, c, d, e, f, g, h));
    }
    
    public bool Equals(Func<A,B,C,D,E,F,G,H,TReturn> func) {
      return func.Equals(Func);
    }
    
    static public implicit operator FuncSet<A,B,C,D,E,F,G,H,TReturn>(Func<A,B,C,D,E,F,G,H,TReturn> func) {
      return new FuncSet<A,B,C,D,E,F,G,H,TReturn>(func);
    }
    
    public override string ToString() {
      return "[FuncSet Func="+Func+", ArgA="+ArgA+", ArgB="+ArgB+", ArgC="+ArgC+", ArgD="+ArgD+", ArgE="+ArgE+
        ", ArgF="+ArgF+", ArgG="+ArgG+", ArgH="+ArgH+"]";
    }

    //-------------------------------------------//

  }
  
  public class FuncSet<A,B,C,D,E,F,G,H,I,TReturn> :
    IFunc,
    IFunc<TReturn>,
    IFunc<A,TReturn>,
    IFunc<A,B,TReturn>,
    IFunc<A,B,C,TReturn>,
    IFunc<A,B,C,D,TReturn>,
    IFunc<A,B,C,D,E,TReturn>,
    IFunc<A,B,C,D,E,F,TReturn>,
    IFunc<A,B,C,D,E,F,G,TReturn>,
    IFunc<A,B,C,D,E,F,G,H,TReturn>,
    IFunc<A,B,C,D,E,F,G,H,I,TReturn>,
    IArgs<A,B,C,D,E,F,G,H,I>,
    IEquatable<Func<A,B,C,D,E,F,G,H,I,TReturn>> {
    
    //-------------------------------------------//
    
    public A ArgA { get; set; }
    public B ArgB { get; set; }
    public C ArgC { get; set; }
    public D ArgD { get; set; }
    public E ArgE { get; set; }
    public F ArgF { get; set; }
    public G ArgG { get; set; }
    public H ArgH { get; set; }
    public I ArgI { get; set; }
    
    public TReturn Result;
    
    public Func<A,B,C,D,E,F,G,H,I,TReturn> Func;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    public FuncSet() {}
    
    public FuncSet(Func<A,B,C,D,E,F,G,H,I,TReturn> func) {
      Func = func;
    }
    
    public FuncSet(Func<A,B,C,D,E,F,G,H,I,TReturn> func,
      A a,
      B b = default(B),
      C c = default(C),
      D d = default(D),
      E e = default(E),
      F f = default(F),
      G g = default(G),
      H h = default(H),
      I i = default(I)) {
      Func = func;
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

    public virtual TReturn Run() {
      return Func(ArgA, ArgB, ArgC, ArgD, ArgE, ArgF, ArgG, ArgH, ArgI);
    }
    
    public virtual async Task<TReturn> RunAsync() {
      return await Task.Run<TReturn>(() => Func(ArgA, ArgB, ArgC, ArgD, ArgE, ArgF, ArgG, ArgH, ArgI));
    }
    
    public virtual TReturn Run(A a, B b, C c, D d, E e, F f, G g, H h, I i) {
      return Func(a, b, c, d, e, f, g, h, i);
    }
    
    public virtual async Task<TReturn> RunAsync(A a, B b, C c, D d, E e, F f, G g, H h, I i) {
      return await Task.Run<TReturn>(() => Func(a, b, c, d, e, f, g, h, i));
    }
    
    public bool Equals(Func<A,B,C,D,E,F,G,H,I,TReturn> func) {
      return func.Equals(Func);
    }
    
    static public implicit operator FuncSet<A,B,C,D,E,F,G,H,I,TReturn>(Func<A,B,C,D,E,F,G,H,I,TReturn> func) {
      return new FuncSet<A,B,C,D,E,F,G,H,I,TReturn>(func);
    }
    
    public override string ToString() {
      return "[FuncSet Func="+Func+", ArgA="+ArgA+", ArgB="+ArgB+", ArgC="+ArgC+", ArgD="+ArgD+", ArgE="+ArgE+
        ", ArgF="+ArgF+", ArgG="+ArgG+", ArgH="+ArgH+", ArgI="+ArgI+"]";
    }
    
    //-------------------------------------------//

  }
  
  public class FuncSet<A,B,C,D,E,F,G,H,I,J,TReturn> :
    IFunc,
    IFunc<TReturn>,
    IFunc<A,TReturn>,
    IFunc<A,B,TReturn>,
    IFunc<A,B,C,TReturn>,
    IFunc<A,B,C,D,TReturn>,
    IFunc<A,B,C,D,E,TReturn>,
    IFunc<A,B,C,D,E,F,TReturn>,
    IFunc<A,B,C,D,E,F,G,TReturn>,
    IFunc<A,B,C,D,E,F,G,H,TReturn>,
    IFunc<A,B,C,D,E,F,G,H,I,TReturn>,
    IFunc<A,B,C,D,E,F,G,H,I,J,TReturn>,
    IArgs<A,B,C,D,E,F,G,H,I,J>,
    IEquatable<Func<A,B,C,D,E,F,G,H,I,J,TReturn>> {
    
    //-------------------------------------------//
    
    public A ArgA { get; set; }
    public B ArgB { get; set; }
    public C ArgC { get; set; }
    public D ArgD { get; set; }
    public E ArgE { get; set; }
    public F ArgF { get; set; }
    public G ArgG { get; set; }
    public H ArgH { get; set; }
    public I ArgI { get; set; }
    public J ArgJ { get; set; }
    
    public TReturn Result;
    
    public Func<A,B,C,D,E,F,G,H,I,J,TReturn> Func;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    public FuncSet() {}
    
    public FuncSet(Func<A,B,C,D,E,F,G,H,I,J,TReturn> func) {
      Func = func;
    }
    
    public FuncSet(Func<A,B,C,D,E,F,G,H,I,J,TReturn> func,
      A a,
      B b = default(B),
      C c = default(C),
      D d = default(D),
      E e = default(E),
      F f = default(F),
      G g = default(G),
      H h = default(H),
      I i = default(I),
      J j = default(J)) {
      Func = func;
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
    
    public virtual TReturn Run() {
      return Func(ArgA, ArgB, ArgC, ArgD, ArgE, ArgF, ArgG, ArgH, ArgI, ArgJ);
    }
    
    public virtual async Task<TReturn> RunAsync() {
      return await Task.Run<TReturn>(() => Func(ArgA, ArgB, ArgC, ArgD, ArgE, ArgF, ArgG, ArgH, ArgI, ArgJ));
    }
    
    public virtual TReturn Run(A a, B b, C c, D d, E e, F f, G g, H h, I i, J j) {
      return Func(a, b, c, d, e, f, g, h, i, j);
    }
    
    public virtual async Task<TReturn> RunAsync(A a, B b, C c, D d, E e, F f, G g, H h, I i, J j) {
      return await Task.Run<TReturn>(() => Func(a, b, c, d, e, f, g, h, i, j));
    }
    
    public bool Equals(Func<A,B,C,D,E,F,G,H,I,J,TReturn> func) {
      return func.Equals(Func);
    }
    
    static public implicit operator FuncSet<A,B,C,D,E,F,G,H,I,J,TReturn>(Func<A,B,C,D,E,F,G,H,I,J,TReturn> func) {
      return new FuncSet<A,B,C,D,E,F,G,H,I,J,TReturn>(func);
    }
    
    public override string ToString() {
      return "[FuncSet Func="+Func+", ArgA="+ArgA+", ArgB="+ArgB+", ArgC="+ArgC+", ArgD="+ArgD+", ArgE="+ArgE+
        ", ArgF="+ArgF+", ArgG="+ArgG+", ArgH="+ArgH+", ArgI="+ArgI+", ArgJ="+ArgJ+"]";
    }

    //-------------------------------------------//

  }


}
