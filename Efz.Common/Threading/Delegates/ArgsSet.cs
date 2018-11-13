using System;

namespace Efz {
  
  /// <summary>
  /// Implementation of Args.
  /// </summary>
  public class Args : IArgs {
    
    //-------------------------------------------//
    
    public override string ToString() {
      return base.ToString();
    }
    
  }
  
  /// <summary>
  /// Implementation of Args.
  /// </summary>
  public class Args<A> : IArgs, IArgs<A> {
    
    //-------------------------------------------//
    
    public A ArgA { get; set; }
    
    //-------------------------------------------//
    
    public override string ToString() {
      return string.Format("[Args ArgA={0}]", ArgA);
    }
    
  }
  
  /// <summary>
  /// Implementation of Args.
  /// </summary>
  public class Args<A,B> : Args<A>, IArgs<A,B> {
    
    //-------------------------------------------//
    
    public B ArgB { get; set; }
    
    //-------------------------------------------//
    
    public override string ToString() {
      return string.Format("[Args ArgA={0}, ArgB={1}]", ArgA, ArgB);
    }
    
  }
  
  /// <summary>
  /// Implementation of Args.
  /// </summary>
  public class Args<A,B,C> : Args<A,B>, IArgs<A,B,C> {
    
    //-------------------------------------------//
    
    public C ArgC { get; set; }
    
    //-------------------------------------------//
    
    public override string ToString() {
      return string.Format("[Args ArgA={0}, ArgB={1}, ArgC={2}]", ArgA, ArgB, ArgC);
    }
    
  }
  
  /// <summary>
  /// Implementation of Args.
  /// </summary>
  public class Args<A,B,C,D> : Args<A,B,C>, IArgs<A,B,C,D> {
    
    //-------------------------------------------//
    
    public D ArgD { get; set; }
    
    //-------------------------------------------//
    
    public override string ToString() {
      return string.Format("[Args ArgA={0}, ArgB={1}, ArgC={2}, ArgD={3}]", ArgA, ArgB, ArgC, ArgD);
    }
    
  }
  
  /// <summary>
  /// Implementation of Args.
  /// </summary>
  public class Args<A,B,C,D,E> : Args<A,B,C,D>, IArgs<A,B,C,D,E> {
    
    //-------------------------------------------//
    
    public E ArgE { get; set; }
    
    //-------------------------------------------//
    
    public override string ToString() {
      return string.Format("[Args ArgA={0}, ArgB={1}, ArgC={2}, ArgD={3}, ArgE={4}]", ArgA, ArgB, ArgC, ArgD, ArgE);
    }
    
  }
  
}
