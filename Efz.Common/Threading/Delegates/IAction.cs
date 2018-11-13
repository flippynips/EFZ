using System;

namespace Efz {
  
  public interface IAction : IRun, IArgs {
    
    //-------------------------------------------//
    
  }
  
  public interface IAction<A> : IAction, IRun, IArgs<A> {
    
    //-------------------------------------------//
    
  }
  
  public interface IAction<A,B> : IAction<A>, IRun, IArgs<A,B> {
    
    //-------------------------------------------//
    
  }
  
  public interface IAction<A,B,C> : IAction<A,B>, IRun, IArgs<A,B,C> {
    
    //-------------------------------------------//
    
  }
  
  public interface IAction<A,B,C,D> : IAction<A,B,C>, IRun, IArgs<A,B,C,D> {
    
    //-------------------------------------------//
    
  }
  
  public interface IAction<A,B,C,D,E> : IAction<A,B,C,D>, IRun, IArgs<A,B,C,D,E> {
    
    //-------------------------------------------//
    
  }
  
  public interface IAction<A,B,C,D,E,F> : IAction<A,B,C,D,E>, IRun, IArgs<A,B,C,D,E,F> {
    
    //-------------------------------------------//
    
  }
  
  public interface IAction<A,B,C,D,E,F,G> : IAction<A,B,C,D,E,F>, IRun, IArgs<A,B,C,D,E,F,G> {
    
    //-------------------------------------------//
    
  }
  
  public interface IAction<A,B,C,D,E,F,G,H> : IAction<A,B,C,D,E,F,G>, IRun, IArgs<A,B,C,D,E,F,G,H> {
    
    //-------------------------------------------//
    
  }
  
  public interface IAction<A,B,C,D,E,F,G,H,I> : IAction<A,B,C,D,E,F,G,H>, IRun, IArgs<A,B,C,D,E,F,G,H,I> {
    
    //-------------------------------------------//
    
  }
  
  public interface IAction<A,B,C,D,E,F,G,H,I,J> : IAction<A,B,C,D,E,F,G,H,I>, IRun, IArgs<A,B,C,D,E,F,G,H,I,J> {
    
    //-------------------------------------------//
    
  }
  
}
