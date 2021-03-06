﻿
namespace Efz {
    
  public interface IGet<A> {
        
    //-------------------------------------------//
    
    A GetA {get;}
    
    //-------------------------------------------//
    
  }
  
  public interface IGet<A,B> : IGet<A> {
        
    //-------------------------------------------//
    
    B GetB {get;}
    
    //-------------------------------------------//
    
  }
  
  public interface IGet<A,B,C> : IGet<A,B> {
        
    //-------------------------------------------//
    
    C GetC {get;}
    
    //-------------------------------------------//
    
  }
  
  public interface IGet<A,B,C,D> : IGet<A,B,C> {
        
    //-------------------------------------------//
    
    D GetD {get;}
    
    //-------------------------------------------//
    
  }
  
  public interface IGet<A,B,C,D,E> : IGet<A,B> {
        
    //-------------------------------------------//
    
    E GetE {get;}
    
    //-------------------------------------------//
    
  }
  
}
