﻿
namespace Efz {

  public interface ICloneable<T> where T : ICloneable<T> {
    
    //-------------------------------------------//
    
    T Clone { get; }
    
    //-------------------------------------------//
    
    
    //-------------------------------------------//
    
    
    
    //-------------------------------------------//
    
    
  }

}
