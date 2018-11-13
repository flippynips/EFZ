using System;

namespace Efz.Maths {
  
  public struct ConstantVector : IGet<Vector2> {
    
    //-------------------------------------------//
    
    public Vector2 GetA {
      get {
        return value;
      }
    }
    
    public Vector2 value;
    
    //-------------------------------------------//
    
    
    //-------------------------------------------//
    
    public ConstantVector(Vector2 _value) {
      value = _value;
    }
    
  }

}
