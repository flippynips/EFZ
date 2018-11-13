// Helper class to aid in concurrent
// collection use.

namespace Efz {
  
  /// <summary>
  /// Static helper methods for structs.
  /// </summary>
  public static class Struct {
    
    //-------------------------------------------//
    
    public static Struct<A> New<A>(A a) {
      return new Struct<A>(a);
    }
    
    public static Struct<A,B> New<A,B>(A a, B b) {
      return new Struct<A,B>(a, b);
    }
    
    public static Struct<A,B,C> New<A,B,C>(A a, B b, C c) {
      return new Struct<A,B,C>(a, b, c);
    }
    
    public static Struct<A,B,C,D> New<A,B,C,D>(A a, B b, C c, D d) {
      return new Struct<A,B,C,D>(a, b, c, d);
    }
    
    public static Struct<A,B,C,D,E> New<A,B,C,D,E>(A a, B b, C c, D d, E e) {
      return new Struct<A,B,C,D,E>(a, b, c, d, e);
    }
    
    public static Struct<A,B,C,D,E,F> New<A,B,C,D,E,F>(A a, B b, C c, D d, E e, F f) {
      return new Struct<A,B,C,D,E,F>(a, b, c, d, e, f);
    }
    
    public static Struct<A,B,C,D,E,F,G> New<A,B,C,D,E,F,G>(A a, B b, C c, D d, E e, F f, G g) {
      return new Struct<A,B,C,D,E,F,G>(a, b, c, d, e, f, g);
    }
    
    public static Struct<A,B,C,D,E,F,G,H> New<A,B,C,D,E,F,G,H>(A a, B b, C c, D d, E e, F f, G g, H h) {
      return new Struct<A,B,C,D,E,F,G,H>(a, b, c, d, e, f, g, h);
    }
    
    public static Struct<A,B,C,D,E,F,G,H,I> New<A,B,C,D,E,F,G,H,I>(A a, B b, C c, D d, E e, F f, G g, H h, I i) {
      return new Struct<A,B,C,D,E,F,G,H,I>(a, b, c, d, e, f, g, h, i);
    }
    
    public static Struct<A,B,C,D,E,F,G,H,I,J> New<A,B,C,D,E,F,G,H,I,J>(A a, B b, C c, D d, E e, F f, G g, H h, I i, J j) {
      return new Struct<A,B,C,D,E,F,G,H,I,J>(a, b, c, d, e, f, g, h, i, j);
    }
    
    //-------------------------------------------//
    
  }
  
  // disable ConvertToAutoProperty
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public struct Struct<A> : IArgs<A> {
    
    //-------------------------------------------//
    
    public A ArgA {
      get { return _a; }
      set { _a = value; }
    }
    
    private A _a;
    
    public Struct(A a) {
      _a = a;
    }
    
    public override bool Equals(object obj) {
      return (obj is Struct<A>) && Equals((Struct<A>)obj);
    }
    
    public bool Equals(Struct<A> other) {
      return _a.Equals(other._a);
    }
    
    public override int GetHashCode() {
      return 1000000007 * _a.GetHashCode();
    }
    
    public static bool operator ==(Struct<A> lhs, Struct<A> rhs) {
      return lhs.Equals(rhs);
    }
    
    public static bool operator !=(Struct<A> lhs, Struct<A> rhs) {
      return !lhs.Equals(rhs);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public struct Struct<A,B> : IArgs<A,B> {
    
    //-------------------------------------------//
    
    public A ArgA {
      get { return _a; }
      set { _a = value; }
    }
    
    public B ArgB {
      get { return _b; }
      set { _b = value; }
    }
    
    private A _a;
    private B _b;
    
    public Struct(A a, B b) {
      _a = a;
      _b = b;
    }
    
    public Struct(A a) {
      _a = a;
      _b = default(B);
    }
    
    public override bool Equals(object obj) {
      return (obj is Struct<A,B>) && Equals((Struct<A,B>)obj);
    }
    
    public bool Equals(Struct<A,B> other) {
      return
        _a.Equals(other._a) &&
        _b.Equals(other._b);
    }
    
    public override int GetHashCode() {
      return 1000000007 * _a.GetHashCode() + 1000000009 * _b.GetHashCode();
    }
    
    public static bool operator ==(Struct<A,B> lhs, Struct<A,B> rhs) {
      return lhs.Equals(rhs);
    }
    
    public static bool operator !=(Struct<A,B> lhs, Struct<A,B> rhs) {
      return !lhs.Equals(rhs);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public struct Struct<A,B,C> : IArgs<A,B,C> {
        
    //-------------------------------------------//
    
    public A ArgA {
      get { return _a; }
      set { _a = value; }
    }
    
    public B ArgB {
      get { return _b; }
      set { _b = value; }
    }
    
    public C ArgC {
      get { return _c; }
      set { _c = value; }
    }
    
    private A _a;
    private B _b;
    private C _c;
    
    public Struct(A a, B b, C c) {
      _a = a;
      _b = b;
      _c = c;
    }
    
    public Struct(A a, B b) {
      _a = a;
      _b = b;
      _c = default(C);
    }
    
    public Struct(A a) {
      _a = a;
      _b = default(B);
      _c = default(C);
    }
    
    public override bool Equals(object other) {
      return (other is Struct<A,B,C>) && Equals((Struct<A,B,C>)other);
    }
    
    public bool Equals(Struct<A,B,C> other) {
      return
        this._a.Equals(other._a) &&
        this._b.Equals(other._b) &&
        this._c.Equals(other._c);
    }
    
    public override int GetHashCode() {
      int hashCode = 0;
      unchecked {
        hashCode += 1000000007 * _a.GetHashCode();
        hashCode += 1000000009 * _b.GetHashCode();
        hashCode += 1000000021 * _c.GetHashCode(); }
      return hashCode;
    }
    
    public static bool operator ==(Struct<A,B,C> lhs, Struct<A,B,C> rhs) {
      return lhs.Equals(rhs);
    }
    
    public static bool operator !=(Struct<A,B,C> lhs, Struct<A,B,C> rhs) {
      return !(lhs == rhs);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public struct Struct<A,B,C,D> : IArgs<A,B,C,D> {
        
    //-------------------------------------------//
    
    public A ArgA {
      get { return _a; }
      set { _a = value; }
    }
    
    public B ArgB {
      get { return _b; }
      set { _b = value; }
    }
    
    public C ArgC {
      get { return _c; }
      set { _c = value; }
    }
    
    public D ArgD {
      get { return _d; }
      set { _d = value; }
    }
    
    private A _a;
    private B _b;
    private C _c;
    private D _d;
    
    public Struct(A a, B b, C c, D d) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
    }
    
    public Struct(A a, B b, C c) {
      _a = a;
      _b = b;
      _c = c;
      _d = default(D);
    }
    
    public Struct(A a, B b) {
      _a = a;
      _b = b;
      _c = default(C);
      _d = default(D);
    }
    
    public Struct(A a) {
      _a = a;
      _b = default(B);
      _c = default(C);
      _d = default(D);
    }
    
    public override bool Equals(object other) {
      return (other is Struct<A,B,C,D>) && Equals((Struct<A,B,C,D>)other);
    }
    
    public bool Equals(Struct<A,B,C,D> other) {
      return
        this._a.Equals(other._a) &&
        this._b.Equals(other._b) &&
        this._c.Equals(other._c) &&
        this._d.Equals(other._d);
    }
    
    public override int GetHashCode() {
      int hashCode = 0;
      unchecked {
        hashCode += 1000000007 * _a.GetHashCode();
        hashCode += 1000000009 * _b.GetHashCode();
        hashCode += 1000000021 * _c.GetHashCode();
        hashCode += 1000000033 * _d.GetHashCode(); }
      return hashCode;
    }
    
    public static bool operator ==(Struct<A,B,C,D> lhs, Struct<A,B,C,D> rhs) {
      return lhs.Equals(rhs);
    }
    
    public static bool operator !=(Struct<A,B,C,D> lhs, Struct<A,B,C,D> rhs) {
      return !(lhs == rhs);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public struct Struct<A,B,C,D,E> : IArgs<A,B,C,D,E> {
        
    //-------------------------------------------//
    
    public A ArgA {
      get { return _a; }
      set { _a = value; }
    }
    
    public B ArgB {
      get { return _b; }
      set { _b = value; }
    }
    
    public C ArgC {
      get { return _c; }
      set { _c = value; }
    }
    
    public D ArgD {
      get { return _d; }
      set { _d = value; }
    }
    
    public E ArgE {
      get { return _e; }
      set { _e = value; }
    }
    
    private A _a;
    private B _b;
    private C _c;
    private D _d;
    private E _e;
    
    public Struct(A a, B b, C c, D d, E e) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
    }
    
    public Struct(A a, B b, C c, D d) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = default(E);
    }
    
    public Struct(A a, B b, C c) {
      _a = a;
      _b = b;
      _c = c;
      _d = default(D);
      _e = default(E);
    }
    
    public Struct(A a, B b) {
      _a = a;
      _b = b;
      _c = default(C);
      _d = default(D);
      _e = default(E);
    }
    
    public Struct(A a) {
      _a = a;
      _b = default(B);
      _c = default(C);
      _d = default(D);
      _e = default(E);
    }
    
    public override bool Equals(object other) {
      return (other is Struct<A,B,C,D,E>) && Equals((Struct<A,B,C,D,E>)other);
    }
    
    public bool Equals(Struct<A,B,C,D,E> other) {
      return
        this._a.Equals(other._a) &&
        this._b.Equals(other._b) &&
        this._c.Equals(other._c) &&
        this._d.Equals(other._d) &&
        this._e.Equals(other._e);
    }
    
    public override int GetHashCode() {
      int hashCode = 0;
      unchecked {
        hashCode+=1000000007 * _a.GetHashCode();
        hashCode+=1000000009 * _b.GetHashCode();
        hashCode+=1000000021 * _c.GetHashCode();
        hashCode+=1000000033 * _d.GetHashCode();
        hashCode+=1000000087 * _e.GetHashCode(); }
      return hashCode;
    }
    
    public static bool operator ==(Struct<A,B,C,D,E> lhs, Struct<A,B,C,D,E> rhs) {
      return lhs.Equals(rhs);
    }
    
    public static bool operator !=(Struct<A,B,C,D,E> lhs, Struct<A,B,C,D,E> rhs) {
      return !(lhs == rhs);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public struct Struct<A,B,C,D,E,F> : IArgs<A,B,C,D,E,F> {
        
    //-------------------------------------------//
    
    public A ArgA {
      get { return _a; }
      set { _a = value; }
    }
    
    public B ArgB {
      get { return _b; }
      set { _b = value; }
    }
    
    public C ArgC {
      get { return _c; }
      set { _c = value; }
    }
    
    public D ArgD {
      get { return _d; }
      set { _d = value; }
    }
    
    public E ArgE {
      get { return _e; }
      set { _e = value; }
    }
    
    public F ArgF {
      get { return _f; }
      set { _f = value; }
    }
    
    private A _a;
    private B _b;
    private C _c;
    private D _d;
    private E _e;
    private F _f;
    
    public Struct(A a, B b, C c, D d, E e, F f) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = f;
    }
    
    public Struct(A a, B b, C c, D d, E e) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = default(F);
    }
    
    public Struct(A a, B b, C c, D d) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = default(E);
      _f = default(F);
    }
    
    public Struct(A a, B b, C c) {
      _a = a;
      _b = b;
      _c = c;
      _d = default(D);
      _e = default(E);
      _f = default(F);
    }
    
    public Struct(A a, B b) {
      _a = a;
      _b = b;
      _c = default(C);
      _d = default(D);
      _e = default(E);
      _f = default(F);
    }
    
    public Struct(A a) {
      _a = a;
      _b = default(B);
      _c = default(C);
      _d = default(D);
      _e = default(E);
      _f = default(F);
    }
    
    public override bool Equals(object other) {
      return (other is Struct<A,B,C,D,E,F>) && Equals((Struct<A,B,C,D,E,F>)other);
    }
    
    public bool Equals(Struct<A,B,C,D,E,F> other) {
      return
        this._a.Equals(other._a) &&
        this._b.Equals(other._b) &&
        this._c.Equals(other._c) &&
        this._d.Equals(other._d) &&
        this._e.Equals(other._e) &&
        this._f.Equals(other._f);
    }
    
    public override int GetHashCode() {
      int hashCode = 0;
      unchecked {
        hashCode+=1000000007 * _a.GetHashCode();
        hashCode+=1000000009 * _b.GetHashCode();
        hashCode+=1000000021 * _c.GetHashCode();
        hashCode+=1000000033 * _d.GetHashCode();
        hashCode+=1000000087 * _e.GetHashCode();
        hashCode+=1000000093 * _f.GetHashCode(); }
      return hashCode;
    }
    
    public static bool operator ==(Struct<A,B,C,D,E,F> lhs, Struct<A,B,C,D,E,F> rhs) {
      return lhs.Equals(rhs);
    }
    
    public static bool operator !=(Struct<A,B,C,D,E,F> lhs, Struct<A,B,C,D,E,F> rhs) {
      return !(lhs == rhs);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public struct Struct<A,B,C,D,E,F,G> : IArgs<A,B,C,D,E,F,G> {
        
    //-------------------------------------------//
    
    public A ArgA {
      get { return _a; }
      set { _a = value; }
    }
    
    public B ArgB {
      get { return _b; }
      set { _b = value; }
    }
    
    public C ArgC {
      get { return _c; }
      set { _c = value; }
    }
    
    public D ArgD {
      get { return _d; }
      set { _d = value; }
    }
    
    public E ArgE {
      get { return _e; }
      set { _e = value; }
    }
    
    public F ArgF {
      get { return _f; }
      set { _f = value; }
    }
    
    public G ArgG {
      get { return _g; }
      set { _g = value; }
    }
    
    private A _a;
    private B _b;
    private C _c;
    private D _d;
    private E _e;
    private F _f;
    private G _g;
    
    public Struct(A a, B b, C c, D d, E e, F f, G g) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = f;
      _g = g;
    }
    
    public Struct(A a, B b, C c, D d, E e, F f) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = f;
      _g = default(G);
    }
    
    public Struct(A a, B b, C c, D d, E e) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = default(F);
      _g = default(G);
    }
    
    public Struct(A a, B b, C c, D d) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = default(E);
      _f = default(F);
      _g = default(G);
    }
    
    public Struct(A a, B b, C c) {
      _a = a;
      _b = b;
      _c = c;
      _d = default(D);
      _e = default(E);
      _f = default(F);
      _g = default(G);
    }
    
    public Struct(A a, B b) {
      _a = a;
      _b = b;
      _c = default(C);
      _d = default(D);
      _e = default(E);
      _f = default(F);
      _g = default(G);
    }
    
    public Struct(A a) {
      _a = a;
      _b = default(B);
      _c = default(C);
      _d = default(D);
      _e = default(E);
      _f = default(F);
      _g = default(G);
    }
    
    public override bool Equals(object other) {
      return (other is Struct<A,B,C,D,E,F,G>) && Equals((Struct<A,B,C,D,E,F,G>)other);
    }
    
    public bool Equals(Struct<A,B,C,D,E,F,G> other) {
      return
        this._a.Equals(other._a) &&
        this._b.Equals(other._b) &&
        this._c.Equals(other._c) &&
        this._d.Equals(other._d) &&
        this._e.Equals(other._e) &&
        this._f.Equals(other._f) &&
        this._g.Equals(other._g);
    }
    
    public override int GetHashCode() {
      int hashCode = 0;
      unchecked {
        hashCode += 1000000007 * _a.GetHashCode();
        hashCode += 1000000009 * _b.GetHashCode();
        hashCode += 1000000021 * _c.GetHashCode();
        hashCode += 1000000033 * _d.GetHashCode();
        hashCode += 1000000087 * _e.GetHashCode();
        hashCode += 1000000093 * _f.GetHashCode();
        hashCode += 1000000097 * _g.GetHashCode(); }
      return hashCode;
    }
    
    public static bool operator ==(Struct<A,B,C,D,E,F,G> lhs, Struct<A,B,C,D,E,F,G> rhs) {
      return lhs.Equals(rhs);
    }
    
    public static bool operator !=(Struct<A,B,C,D,E,F,G> lhs, Struct<A,B,C,D,E,F,G> rhs) {
      return !(lhs == rhs);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public struct Struct<A,B,C,D,E,F,G,H> : IArgs<A,B,C,D,E,F,G,H> {
        
    //-------------------------------------------//
    
    public A ArgA {
      get { return _a; }
      set { _a = value; }
    }
    
    public B ArgB {
      get { return _b; }
      set { _b = value; }
    }
    
    public C ArgC {
      get { return _c; }
      set { _c = value; }
    }
    
    public D ArgD {
      get { return _d; }
      set { _d = value; }
    }
    
    public E ArgE {
      get { return _e; }
      set { _e = value; }
    }
    
    public F ArgF {
      get { return _f; }
      set { _f = value; }
    }
    
    public G ArgG {
      get { return _g; }
      set { _g = value; }
    }
    
    public H ArgH {
      get { return _h; }
      set { _h = value; }
    }
    
    private A _a;
    private B _b;
    private C _c;
    private D _d;
    private E _e;
    private F _f;
    private G _g;
    private H _h;
    
    public Struct(A a, B b, C c, D d, E e, F f, G g, H h) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = f;
      _g = g;
      _h = h;
    }
    
    public Struct(A a, B b, C c, D d, E e, F f, G g) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = f;
      _g = g;
      _h = default(H);
    }
    
    public Struct(A a, B b, C c, D d, E e, F f) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = f;
      _g = default(G);
      _h = default(H);
    }
    
    public Struct(A a, B b, C c, D d, E e) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = default(F);
      _g = default(G);
      _h = default(H);
    }
    
    public Struct(A a, B b, C c, D d) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = default(E);
      _f = default(F);
      _g = default(G);
      _h = default(H);
    }
    
    public Struct(A a, B b, C c) {
      _a = a;
      _b = b;
      _c = c;
      _d = default(D);
      _e = default(E);
      _f = default(F);
      _g = default(G);
      _h = default(H);
    }
    
    public Struct(A a, B b) {
      _a = a;
      _b = b;
      _c = default(C);
      _d = default(D);
      _e = default(E);
      _f = default(F);
      _g = default(G);
      _h = default(H);
    }
    
    public Struct(A a) {
      _a = a;
      _b = default(B);
      _c = default(C);
      _d = default(D);
      _e = default(E);
      _f = default(F);
      _g = default(G);
      _h = default(H);
    }
    
    public override bool Equals(object other) {
      return (other is Struct<A,B,C,D,E,F,G,H>) && Equals((Struct<A,B,C,D,E,F,G,H>)other);
    }
    
    public bool Equals(Struct<A,B,C,D,E,F,G,H> other) {
      return
        this._a.Equals(other._a) &&
        this._b.Equals(other._b) &&
        this._c.Equals(other._c) &&
        this._d.Equals(other._d) &&
        this._e.Equals(other._e) &&
        this._f.Equals(other._f) &&
        this._g.Equals(other._g) &&
        this._h.Equals(other._h);
    }
    
    public override int GetHashCode() {
      int hashCode = 0;
      unchecked {
        hashCode += 1000000007 * _a.GetHashCode();
        hashCode += 1000000009 * _b.GetHashCode();
        hashCode += 1000000021 * _c.GetHashCode();
        hashCode += 1000000033 * _d.GetHashCode();
        hashCode += 1000000087 * _e.GetHashCode();
        hashCode += 1000000093 * _f.GetHashCode();
        hashCode += 1000000097 * _g.GetHashCode();
        hashCode += 1000000103 * _h.GetHashCode();
      }
      return hashCode;
    }
    
    public static bool operator ==(Struct<A,B,C,D,E,F,G,H> lhs, Struct<A,B,C,D,E,F,G,H> rhs) {
      return lhs.Equals(rhs);
    }
    
    public static bool operator !=(Struct<A,B,C,D,E,F,G,H> lhs, Struct<A,B,C,D,E,F,G,H> rhs) {
      return !(lhs == rhs);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public struct Struct<A,B,C,D,E,F,G,H,I> : IArgs<A,B,C,D,E,F,G,H,I> {
        
    //-------------------------------------------//
    
    public A ArgA {
      get { return _a; }
      set { _a = value; }
    }
    
    public B ArgB {
      get { return _b; }
      set { _b = value; }
    }
    
    public C ArgC {
      get { return _c; }
      set { _c = value; }
    }
    
    public D ArgD {
      get { return _d; }
      set { _d = value; }
    }
    
    public E ArgE {
      get { return _e; }
      set { _e = value; }
    }
    
    public F ArgF {
      get { return _f; }
      set { _f = value; }
    }
    
    public G ArgG {
      get { return _g; }
      set { _g = value; }
    }
    
    public H ArgH {
      get { return _h; }
      set { _h = value; }
    }
    
    public I ArgI {
      get { return _i; }
      set { _i = value; }
    }
    
    private A _a;
    private B _b;
    private C _c;
    private D _d;
    private E _e;
    private F _f;
    private G _g;
    private H _h;
    private I _i;
    
    public Struct(A a, B b, C c, D d, E e, F f, G g, H h, I i) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = f;
      _g = g;
      _h = h;
      _i = i;
    }
    
    public Struct(A a, B b, C c, D d, E e, F f, G g, H h) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = f;
      _g = g;
      _h = h;
      _i = default(I);
    }
    
    public Struct(A a, B b, C c, D d, E e, F f, G g) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = f;
      _g = g;
      _h = default(H);
      _i = default(I);
    }
    
    public Struct(A a, B b, C c, D d, E e, F f) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = f;
      _g = default(G);
      _h = default(H);
      _i = default(I);
    }
    
    public Struct(A a, B b, C c, D d, E e) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = default(F);
      _g = default(G);
      _h = default(H);
      _i = default(I);
    }
    
    public Struct(A a, B b, C c, D d) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = default(E);
      _f = default(F);
      _g = default(G);
      _h = default(H);
      _i = default(I);
    }
    
    public Struct(A a, B b, C c) {
      _a = a;
      _b = b;
      _c = c;
      _d = default(D);
      _e = default(E);
      _f = default(F);
      _g = default(G);
      _h = default(H);
      _i = default(I);
    }
    
    public Struct(A a, B b) {
      _a = a;
      _b = b;
      _c = default(C);
      _d = default(D);
      _e = default(E);
      _f = default(F);
      _g = default(G);
      _h = default(H);
      _i = default(I);
    }
    
    public Struct(A a) {
      _a = a;
      _b = default(B);
      _c = default(C);
      _d = default(D);
      _e = default(E);
      _f = default(F);
      _g = default(G);
      _h = default(H);
      _i = default(I);
    }
    
    public override bool Equals(object other) {
      return (other is Struct<A,B,C,D,E,F,G,H,I>) && Equals((Struct<A,B,C,D,E,F,G,H,I>)other);
    }
    
    public bool Equals(Struct<A,B,C,D,E,F,G,H,I> other) {
      return
        this._a.Equals(other._a) &&
        this._b.Equals(other._b) &&
        this._c.Equals(other._c) &&
        this._d.Equals(other._d) &&
        this._e.Equals(other._e) &&
        this._f.Equals(other._f) &&
        this._g.Equals(other._g) &&
        this._h.Equals(other._h) &&
        this._i.Equals(other._i);
    }
    
    public override int GetHashCode() {
      int hashCode = 0;
      unchecked {
        hashCode += 1000000007 * _a.GetHashCode();
        hashCode += 1000000009 * _b.GetHashCode();
        hashCode += 1000000021 * _c.GetHashCode();
        hashCode += 1000000033 * _d.GetHashCode();
        hashCode += 1000000087 * _e.GetHashCode();
        hashCode += 1000000093 * _f.GetHashCode();
        hashCode += 1000000097 * _g.GetHashCode();
        hashCode += 1000000103 * _h.GetHashCode();
        hashCode += 1000000107 * _i.GetHashCode();
      }
      return hashCode;
    }
    
    public static bool operator ==(Struct<A,B,C,D,E,F,G,H,I> lhs, Struct<A,B,C,D,E,F,G,H,I> rhs) {
      return lhs.Equals(rhs);
    }
    
    public static bool operator !=(Struct<A,B,C,D,E,F,G,H,I> lhs, Struct<A,B,C,D,E,F,G,H,I> rhs) {
      return !(lhs == rhs);
    }
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Helper class referencing one or more types.
  /// </summary>
  public struct Struct<A,B,C,D,E,F,G,H,I,J> : IArgs<A,B,C,D,E,F,G,H,I,J> {
        
    //-------------------------------------------//
    
    public A ArgA {
      get { return _a; }
      set { _a = value; }
    }
    
    public B ArgB {
      get { return _b; }
      set { _b = value; }
    }
    
    public C ArgC {
      get { return _c; }
      set { _c = value; }
    }
    
    public D ArgD {
      get { return _d; }
      set { _d = value; }
    }
    
    public E ArgE {
      get { return _e; }
      set { _e = value; }
    }
    
    public F ArgF {
      get { return _f; }
      set { _f = value; }
    }
    
    public G ArgG {
      get { return _g; }
      set { _g = value; }
    }
    
    public H ArgH {
      get { return _h; }
      set { _h = value; }
    }
    
    public I ArgI {
      get { return _i; }
      set { _i = value; }
    }
    
    public J ArgJ {
      get { return _j; }
      set { _j = value; }
    }
    
    private A _a;
    private B _b;
    private C _c;
    private D _d;
    private E _e;
    private F _f;
    private G _g;
    private H _h;
    private I _i;
    private J _j;
    
    public Struct(A a, B b, C c, D d, E e, F f, G g, H h, I i, J j) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = f;
      _g = g;
      _h = h;
      _i = i;
      _j = j;
    }
    
    public Struct(A a, B b, C c, D d, E e, F f, G g, H h, I i) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = f;
      _g = g;
      _h = h;
      _i = i;
      _j = default(J);
    }
    
    public Struct(A a, B b, C c, D d, E e, F f, G g, H h) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = f;
      _g = g;
      _h = h;
      _i = default(I);
      _j = default(J);
    }
    
    public Struct(A a, B b, C c, D d, E e, F f, G g) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = f;
      _g = g;
      _h = default(H);
      _i = default(I);
      _j = default(J);
    }
    
    public Struct(A a, B b, C c, D d, E e, F f) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = f;
      _g = default(G);
      _h = default(H);
      _i = default(I);
      _j = default(J);
    }
    
    public Struct(A a, B b, C c, D d, E e) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = e;
      _f = default(F);
      _g = default(G);
      _h = default(H);
      _i = default(I);
      _j = default(J);
    }
    
    public Struct(A a, B b, C c, D d) {
      _a = a;
      _b = b;
      _c = c;
      _d = d;
      _e = default(E);
      _f = default(F);
      _g = default(G);
      _h = default(H);
      _i = default(I);
      _j = default(J);
    }
    
    public Struct(A a, B b, C c) {
      _a = a;
      _b = b;
      _c = c;
      _d = default(D);
      _e = default(E);
      _f = default(F);
      _g = default(G);
      _h = default(H);
      _i = default(I);
      _j = default(J);
    }
    
    public Struct(A a, B b) {
      _a = a;
      _b = b;
      _c = default(C);
      _d = default(D);
      _e = default(E);
      _f = default(F);
      _g = default(G);
      _h = default(H);
      _i = default(I);
      _j = default(J);
    }
    
    public Struct(A a) {
      _a = a;
      _b = default(B);
      _c = default(C);
      _d = default(D);
      _e = default(E);
      _f = default(F);
      _g = default(G);
      _h = default(H);
      _i = default(I);
      _j = default(J);
    }
    
    public override bool Equals(object other) {
      return (other is Struct<A,B,C,D,E,F,G,H,I,J>) && Equals((Struct<A,B,C,D,E,F,G,H,I,J>)other);
    }
    
    public bool Equals(Struct<A,B,C,D,E,F,G,H,I,J> other) {
      return
        this._a.Equals(other._a) &&
        this._b.Equals(other._b) &&
        this._c.Equals(other._c) &&
        this._d.Equals(other._d) &&
        this._e.Equals(other._e) &&
        this._f.Equals(other._f) &&
        this._g.Equals(other._g) &&
        this._h.Equals(other._h) &&
        this._i.Equals(other._i) &&
        this._j.Equals(other._j);
    }
    
    public override int GetHashCode() {
      int hashCode = 0;
      unchecked {
        hashCode += 1000000007 * _a.GetHashCode();
        hashCode += 1000000009 * _b.GetHashCode();
        hashCode += 1000000021 * _c.GetHashCode();
        hashCode += 1000000033 * _d.GetHashCode();
        hashCode += 1000000087 * _e.GetHashCode();
        hashCode += 1000000093 * _f.GetHashCode();
        hashCode += 1000000097 * _g.GetHashCode();
        hashCode += 1000000103 * _h.GetHashCode();
        hashCode += 1000000107 * _i.GetHashCode();
        hashCode += 1000000113 * _j.GetHashCode();
      }
      return hashCode;
    }
    
    public static bool operator ==(Struct<A,B,C,D,E,F,G,H,I,J> lhs, Struct<A,B,C,D,E,F,G,H,I,J> rhs) {
      return lhs.Equals(rhs);
    }
    
    public static bool operator !=(Struct<A,B,C,D,E,F,G,H,I,J> lhs, Struct<A,B,C,D,E,F,G,H,I,J> rhs) {
      return !(lhs == rhs);
    }
    
    //-------------------------------------------//
    
  }
  
  
}
