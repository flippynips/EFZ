using System;

namespace Efz {
  
  public interface IFunc : IArgs {
    
    //-------------------------------------------//
    
  }
  
  public interface IFunc<TReturn> : IFunc, IRun<TReturn>, IArgs {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
  }
  
  public interface IFunc<A,TReturn> : IFunc<TReturn>, IRun<TReturn>, IArgs<A> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
  }
  
  public interface IFunc<A,B,TReturn> : IFunc<A,TReturn>, IRun<TReturn>, IArgs<A,B> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
  }
  
  public interface IFunc<A,B,C,TReturn> : IFunc<A,B,TReturn>, IRun<TReturn>, IArgs<A,B,C> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
  }
  
  public interface IFunc<A,B,C,D,TReturn> : IFunc<A,B,C,TReturn>, IRun<TReturn>, IArgs<A,B,C,D> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
  }
  
  public interface IFunc<A,B,C,D,E,TReturn> : IFunc<A,B,C,D,TReturn>, IRun<TReturn>, IArgs<A,B,C,D,E> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
  }
  
  public interface IFunc<A,B,C,D,E,F,TReturn> : IFunc<A,B,C,D,E,TReturn>, IRun<TReturn>, IArgs<A,B,C,D,E,F> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
  }
  
  public interface IFunc<A,B,C,D,E,F,G,TReturn> : IFunc<A,B,C,D,E,F,TReturn>, IRun<TReturn>, IArgs<A,B,C,D,E,F,G> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
  }
  
  public interface IFunc<A,B,C,D,E,F,G,H,TReturn> : IFunc<A,B,C,D,E,F,G,TReturn>, IRun<TReturn>, IArgs<A,B,C,D,E,F,G,H> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
  }
  
  public interface IFunc<A,B,C,D,E,F,G,H,I,TReturn> : IFunc<A,B,C,D,E,F,G,H,TReturn>, IRun<TReturn>, IArgs<A,B,C,D,E,F,G,H,I> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
  }
  
  public interface IFunc<A,B,C,D,E,F,G,H,I,J,TReturn> : IFunc<A,B,C,D,E,F,G,H,I,TReturn>, IRun<TReturn>, IArgs<A,B,C,D,E,F,G,H,I,J> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
  }
  
}
