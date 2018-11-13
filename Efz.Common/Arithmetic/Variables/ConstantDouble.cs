using System;

namespace Efz.Maths {
    
  public struct ConstantDouble : IGet<double> {
        
    //-------------------------------------------//
    
    public double GetA {
      get {
        return value;
      }
    }
    
    //-------------------------------------------//
    
    private double value;
    
    //-------------------------------------------//
    
    public ConstantDouble(double _value) {
      value = _value;
    }
    
    public void Set(double _value) {
      value = _value;
    }
    
  }

}
