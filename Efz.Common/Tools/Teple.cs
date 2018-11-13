// Helper class to aid in concurrent
// collection use.

using System;

namespace Efz.Tools {
  
  /// <summary>
  /// Helper class referencing 0 classes for completion.
  /// </summary>
  public class Teple : IArgs {
    
    //-------------------------------------------//
    
    public static Teple<A> New<A>(A a) {
      return new Teple<A>(a);
    }
    
    public static Teple<A,B> New<A,B>(A a, B b) {
      return new Teple<A,B>(a, b);
    }
    
    public static Teple<A,B,C> New<A,B,C>(A a, B b, C c) {
      return new Teple<A,B,C>(a, b, c);
    }
    
    public static Teple<A,B,C,D> New<A,B,C,D>(A a, B b, C c, D d) {
      return new Teple<A,B,C,D>(a, b, c, d);
    }
    
    public static Teple<A,B,C,D,E> New<A,B,C,D,E>(A a, B b, C c, D d, E e) {
      return new Teple<A,B,C,D,E>(a, b, c, d, e);
    }
    
    public static Teple<A,B,C,D,E,F> New<A,B,C,D,E,F>(A a, B b, C c, D d, E e, F f) {
      return new Teple<A,B,C,D,E,F>(a, b, c, d, e, f);
    }
    
    public static Teple<A,B,C,D,E,F,G> New<A,B,C,D,E,F,G>(A a, B b, C c, D d, E e, F f, G g) {
      return new Teple<A,B,C,D,E,F,G>(a, b, c, d, e, f, g);
    }
    
    public static Teple<A,B,C,D,E,F,G,H> New<A,B,C,D,E,F,G,H>(A a, B b, C c, D d, E e, F f, G g, H h) {
      return new Teple<A,B,C,D,E,F,G,H>(a, b, c, d, e, f, g, h);
    }
    
    public static Teple<A,B,C,D,E,F,G,H,I> New<A,B,C,D,E,F,G,H,I>(A a, B b, C c, D d, E e, F f, G g, H h, I i) {
      return new Teple<A,B,C,D,E,F,G,H,I>(a, b, c, d, e, f, g, h, i);
    }
    
    public static Teple<A,B,C,D,E,F,G,H,I,J> New<A,B,C,D,E,F,G,H,I,J>(A a, B b, C c, D d, E e, F f, G g, H h, I i, J j) {
      return new Teple<A,B,C,D,E,F,G,H,I,J>(a, b, c, d, e, f, g, h, i, j);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public class Teple<A> : Teple, IArgs<A> {
        
    //-------------------------------------------//
    
    public A ArgA {get;set;}
    
    public Teple() {}
    public Teple(A a) {
      ArgA = a;
    }
    
    public override string ToString() {
      return Chars.BracketSqOpen.ToString()+ArgA+Chars.BracketSqClose;
    }
    
    /// <summary>
    /// Equality that checks null & arguments against each other.
    /// </summary>
    public bool Equals(Teple<A> other) {
      if(other == null) return false;
      return
        Equals(ArgA, other.ArgA);
    }
    
    /// <summary>
    /// Equality operator checks each argument for equality.
    /// </summary>
    public override bool Equals(object obj) {
      return Equals(obj as Teple<A>);
    }
    
    /// <summary>
    /// Get a hashcode that combines arguments hascodes.
    /// </summary>
    public override int GetHashCode() {
      return ArgA.GetHashCode();
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public class Teple<A, B> : Teple<A>, IArgs<A,B> {
    
    //-------------------------------------------//
    
    public B ArgB {get;set;}
    
    public Teple() {}
    public Teple(A a, B b) {
      ArgA = a;
      ArgB = b;
    }
    
    public override string ToString() {
      return Chars.BracketSqOpen.ToString()+ArgA+ Chars.Comma + ArgB+Chars.BracketSqClose;
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public class Teple<A, B, C> : Teple<A, B>, IArgs<A,B,C>, IEquatable<Teple<A,B,C>> {
        
    //-------------------------------------------//
    
    public C ArgC {get;set;}
    
    public Teple() {}
    public Teple(A a, B b, C c) {
      ArgA = a;
      ArgB = b;
      ArgC = c;
    }
    
    public override string ToString() {
      return Chars.BracketSqOpen.ToString()+ArgA+Chars.Comma+ArgB+Chars.Comma+ArgC+Chars.BracketSqClose;
    }
    
    /// <summary>
    /// Equality that checks null & arguments against each other.
    /// </summary>
    public bool Equals(Teple<A,B,C> other) {
      if(other == null) return false;
      return
        Equals(ArgA, other.ArgA) &&
        Equals(ArgB, other.ArgB) &&
        Equals(ArgC, other.ArgC);
    }
    
    /// <summary>
    /// Equality operator checks each argument for equality.
    /// </summary>
    public override bool Equals(object obj) {
      return Equals(obj as Teple<A,B,C>);
    }
    
    /// <summary>
    /// Get a hashcode that combines arguments hascodes.
    /// </summary>
    public override int GetHashCode() {
      return
        ArgA.GetHashCode() ^
        ArgB.GetHashCode() | 100000007 ^
        ArgC.GetHashCode() | 100000013;
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public class Teple<A, B, C, D> : Teple<A, B, C>, IArgs<A,B,C,D> {
        
    //-------------------------------------------//
    
    public D ArgD {get;set;}
    
    public Teple() {}
    public Teple(A a, B b, C c, D d) {
      ArgA = a;
      ArgB = b;
      ArgC = c;
      ArgD = d;
    }
    
    public override string ToString() {
      return Chars.BracketSqOpen.ToString()+ArgA+Chars.Comma + ArgB+Chars.Comma + ArgC+Chars.Comma + ArgD+Chars.BracketSqClose;
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public class Teple<A, B, C, D, E> : Teple<A, B, C, D>, IArgs<A,B,C,D,E>, IEquatable<Teple<A,B,C,D,E>> {
        
    //-------------------------------------------//
    
    public E ArgE {get;set;}
    
    public Teple() {}
    public Teple(A a, B b, C c, D d, E e) {
      ArgA = a;
      ArgB = b;
      ArgC = c;
      ArgD = d;
      ArgE = e;
    }
    
    public override string ToString() {
      return Chars.BracketSqOpen.ToString()+ArgA+Chars.Comma + ArgB+Chars.Comma + ArgC+Chars.Comma + ArgD+Chars.Comma + ArgE+Chars.BracketSqClose;
    }
    
    /// <summary>
    /// Equality that checks null & arguments against each other.
    /// </summary>
    public bool Equals(Teple<A,B,C,D,E> other) {
      if(other == null) return false;
      return
        Equals(ArgA, other.ArgA) &&
        Equals(ArgB, other.ArgB) &&
        Equals(ArgC, other.ArgC) &&
        Equals(ArgD, other.ArgD) &&
        Equals(ArgE, other.ArgE);
    }
    
    /// <summary>
    /// Equality operator checks each argument for equality.
    /// </summary>
    public override bool Equals(object obj) {
      return Equals(obj as Teple<A,B,C,D,E>);
    }
    
    /// <summary>
    /// Get a hashcode that combines arguments hascodes.
    /// </summary>
    public override int GetHashCode() {
      return
        ArgA.GetHashCode() ^
        ArgB.GetHashCode() | 100000007 ^
        ArgC.GetHashCode() | 100000013 ^
        ArgD.GetHashCode() | 100000023 ^
        ArgE.GetHashCode() | 100000031;
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public class Teple<A, B, C, D, E, F> : Teple<A, B, C, D, E>, IArgs<A,B,C,D,E,F>, IEquatable<Teple<A,B,C,D,E,F>> {
        
    //-------------------------------------------//
    
    public F ArgF {get;set;}
    
    public Teple() {}
    public Teple(A a, B b, C c, D d, E e, F f) {
      ArgA = a;
      ArgB = b;
      ArgC = c;
      ArgD = d;
      ArgE = e;
      ArgF = f;
    }
    
    public override string ToString() {
      return Chars.BracketSqOpen.ToString()+ArgA+Chars.Comma + ArgB+Chars.Comma + ArgC+Chars.Comma + ArgD+Chars.Comma + ArgE+Chars.Comma + ArgF+Chars.BracketSqClose;
    }
    
    /// <summary>
    /// Equality that checks null & arguments against each other.
    /// </summary>
    public bool Equals(Teple<A,B,C,D,E,F> other) {
      if(other == null) return false;
      return
        Equals(ArgA, other.ArgA) &&
        Equals(ArgB, other.ArgB) &&
        Equals(ArgC, other.ArgC) &&
        Equals(ArgD, other.ArgD) &&
        Equals(ArgE, other.ArgE) &&
        Equals(ArgF, other.ArgF);
    }
    
    /// <summary>
    /// Equality operator checks each argument for equality.
    /// </summary>
    public override bool Equals(object obj) {
      return Equals(obj as Teple<A,B,C,D,E,F>);
    }
    
    /// <summary>
    /// Get a hashcode that combines arguments hascodes.
    /// </summary>
    public override int GetHashCode() {
      return
        ArgA.GetHashCode() ^
        ArgB.GetHashCode() | 100000007 ^
        ArgC.GetHashCode() | 100000013 ^
        ArgD.GetHashCode() | 100000023 ^
        ArgE.GetHashCode() | 100000031 ^
        ArgF.GetHashCode() | 100000037;
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public class Teple<A,B,C,D,E,F,G> : Teple<A,B,C,D,E,F>, IArgs<A,B,C,D,E,F,G>, IEquatable<Teple<A,B,C,D,E,F,G>> {
    
    //-------------------------------------------//
    
    public G ArgG {get;set;}
    
    public Teple() {}
    public Teple(A a, B b, C c, D d, E e, F f, G g) {
      ArgA = a;
      ArgB = b;
      ArgC = c;
      ArgD = d;
      ArgE = e;
      ArgF = f;
      ArgG = g;
    }
    
    public override string ToString() {
      return Chars.BracketSqOpen.ToString()+ArgA+Chars.Comma + ArgB+Chars.Comma + ArgC+Chars.Comma + ArgD+Chars.Comma +
        ArgE+Chars.Comma + ArgF+Chars.Comma + ArgG+Chars.BracketSqClose;
    }
    
    /// <summary>
    /// Equality that checks null & arguments against each other.
    /// </summary>
    public bool Equals(Teple<A,B,C,D,E,F,G> other) {
      if(other == null) return false;
      return
        Equals(ArgA, other.ArgA) &&
        Equals(ArgB, other.ArgB) &&
        Equals(ArgC, other.ArgC) &&
        Equals(ArgD, other.ArgD) &&
        Equals(ArgE, other.ArgE) &&
        Equals(ArgF, other.ArgF);
    }
    
    /// <summary>
    /// Equality operator checks each argument for equality.
    /// </summary>
    public override bool Equals(object obj) {
      return Equals(obj as Teple<A,B,C,D,E,F,G>);
    }
    
    /// <summary>
    /// Get a hashcode that combines arguments hascodes.
    /// </summary>
    public override int GetHashCode() {
      return
        ArgA.GetHashCode() ^
        ArgB.GetHashCode() | 100000007 ^
        ArgC.GetHashCode() | 100000013 ^
        ArgD.GetHashCode() | 100000023 ^
        ArgE.GetHashCode() | 100000031 ^
        ArgF.GetHashCode() | 100000037 ^
        ArgG.GetHashCode() | 100000047;
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public class Teple<A,B,C,D,E,F,G,H> : Teple<A,B,C,D,E,F,G>, IArgs<A,B,C,D,E,F,G,H>, IEquatable<Teple<A,B,C,D,E,F,G,H>> {
    
    //-------------------------------------------//
    
    public H ArgH {get;set;}
    
    public Teple() {}
    public Teple(A a, B b, C c, D d, E e, F f, G g, H h) {
      ArgA = a;
      ArgB = b;
      ArgC = c;
      ArgD = d;
      ArgE = e;
      ArgF = f;
      ArgG = g;
      ArgH = h;
    }
    
    public override string ToString() {
      return Chars.BracketSqOpen.ToString()+ArgA+Chars.Comma + ArgB+Chars.Comma + ArgC+Chars.Comma + ArgD+Chars.Comma +
        ArgE+Chars.Comma + ArgF+Chars.Comma + ArgG+Chars.Comma + ArgH+Chars.BracketSqClose;
    }
    
    /// <summary>
    /// Equality that checks null & arguments against each other.
    /// </summary>
    public bool Equals(Teple<A,B,C,D,E,F,G,H> other) {
      if(other == null) return false;
      return
        Equals(ArgA, other.ArgA) &&
        Equals(ArgB, other.ArgB) &&
        Equals(ArgC, other.ArgC) &&
        Equals(ArgD, other.ArgD) &&
        Equals(ArgE, other.ArgE) &&
        Equals(ArgF, other.ArgF) &&
        Equals(ArgG, other.ArgG) &&
        Equals(ArgH, other.ArgH);
    }
    
    /// <summary>
    /// Equality operator checks each argument for equality.
    /// </summary>
    public override bool Equals(object obj) {
      return Equals(obj as Teple<A,B,C,D,E,F,G,H>);
    }
    
    /// <summary>
    /// Get a hashcode that combines arguments hascodes.
    /// </summary>
    public override int GetHashCode() {
      return
        ArgA.GetHashCode() ^
        ArgB.GetHashCode() | 100000007 ^
        ArgC.GetHashCode() | 100000013 ^
        ArgD.GetHashCode() | 100000023 ^
        ArgE.GetHashCode() | 100000031 ^
        ArgF.GetHashCode() | 100000037 ^
        ArgG.GetHashCode() | 100000047 ^
        ArgH.GetHashCode() | 100000053;
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public class Teple<A,B,C,D,E,F,G,H,I> : Teple<A,B,C,D,E,F,G,H>, IArgs<A,B,C,D,E,F,G,H,I>, IEquatable<Teple<A,B,C,D,E,F,G,H,I>> {
    
    //-------------------------------------------//
    
    public I ArgI {get;set;}
    
    public Teple() {}
    public Teple(A a, B b, C c, D d, E e, F f, G g, H h, I i) {
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
    
    public override string ToString() {
      return Chars.BracketSqOpen.ToString()+ArgA+Chars.Comma + ArgB+Chars.Comma + ArgC+Chars.Comma + ArgD+Chars.Comma +
        ArgE+Chars.Comma + ArgF+Chars.Comma + ArgG+Chars.Comma + ArgH+Chars.Comma + ArgI+Chars.BracketSqClose;
    }
    
    /// <summary>
    /// Equality that checks null & arguments against each other.
    /// </summary>
    public bool Equals(Teple<A,B,C,D,E,F,G,H,I> other) {
      if(other == null) return false;
      return
        Equals(ArgA, other.ArgA) &&
        Equals(ArgB, other.ArgB) &&
        Equals(ArgC, other.ArgC) &&
        Equals(ArgD, other.ArgD) &&
        Equals(ArgE, other.ArgE) &&
        Equals(ArgF, other.ArgF) &&
        Equals(ArgG, other.ArgG) &&
        Equals(ArgH, other.ArgH) &&
        Equals(ArgI, other.ArgI);
    }
    
    /// <summary>
    /// Equality operator checks each argument for equality.
    /// </summary>
    public override bool Equals(object obj) {
      return Equals(obj as Teple<A,B,C,D,E,F,G,H,I>);
    }
    
    /// <summary>
    /// Get a hashcode that combines arguments hascodes.
    /// </summary>
    public override int GetHashCode() {
      return
        ArgA.GetHashCode() ^
        ArgB.GetHashCode() | 100000007 ^
        ArgC.GetHashCode() | 100000013 ^
        ArgD.GetHashCode() | 100000023 ^
        ArgE.GetHashCode() | 100000031 ^
        ArgF.GetHashCode() | 100000037 ^
        ArgG.GetHashCode() | 100000047 ^
        ArgH.GetHashCode() | 100000053 ^
        ArgI.GetHashCode() | 100000059;
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public class Teple<A,B,C,D,E,F,G,H,I,J> : Teple<A,B,C,D,E,F,G,H,I>, IArgs<A,B,C,D,E,F,G,H,I,J>, IEquatable<Teple<A,B,C,D,E,F,G,H,I,J>> {
    
    //-------------------------------------------//
    
    public J ArgJ {get;set;}
    
    public Teple() {}
    public Teple(A a, B b, C c, D d, E e, F f, G g, H h, I i, J j) {
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
    
    public override string ToString() {
      return Chars.BracketSqOpen.ToString()+ArgA+Chars.Comma + ArgB+Chars.Comma + ArgC+Chars.Comma + ArgD+Chars.Comma +
        ArgE+Chars.Comma + ArgF+Chars.Comma + ArgG+Chars.Comma + ArgH+Chars.Comma + ArgI+Chars.Comma + ArgJ+Chars.BracketSqClose;
    }
    
    /// <summary>
    /// Equality that checks null & arguments against each other.
    /// </summary>
    public bool Equals(Teple<A,B,C,D,E,F,G,H,I,J> other) {
      if(other == null) return false;
      return
        Equals(ArgA, other.ArgA) &&
        Equals(ArgB, other.ArgB) &&
        Equals(ArgC, other.ArgC) &&
        Equals(ArgD, other.ArgD) &&
        Equals(ArgE, other.ArgE) &&
        Equals(ArgF, other.ArgF) &&
        Equals(ArgG, other.ArgG) &&
        Equals(ArgH, other.ArgH) &&
        Equals(ArgI, other.ArgI) &&
        Equals(ArgJ, other.ArgJ);
    }
    
    /// <summary>
    /// Equality operator checks each argument for equality.
    /// </summary>
    public override bool Equals(object obj) {
      return Equals(obj as Teple<A,B,C,D,E,F,G,H,I,J>);
    }
    
    /// <summary>
    /// Get a hashcode that combines arguments hascodes.
    /// </summary>
    public override int GetHashCode() {
      return
        ArgA.GetHashCode() ^
        ArgB.GetHashCode() | 100000007 ^
        ArgC.GetHashCode() | 100000013 ^
        ArgD.GetHashCode() | 100000023 ^
        ArgE.GetHashCode() | 100000031 ^
        ArgF.GetHashCode() | 100000037 ^
        ArgG.GetHashCode() | 100000047 ^
        ArgH.GetHashCode() | 100000053 ^
        ArgI.GetHashCode() | 100000059 ^
        ArgJ.GetHashCode() | 100000065;
    }
    
    //-------------------------------------------//
    
  }
  
}
