using System;

namespace Efz.Maths {
    
  public struct DoubleRange : IGet<double> {
        
    //-------------------------------------------//
    
    public double GetA {
      get {
        return value.Item1 + Randomize.Double * (value.Item2 - value.Item1);
      }
    }
    
    public Tuple<double,double> value;

    //-------------------------------------------//
    
    
    //-------------------------------------------//
    
    public DoubleRange(double _a, double _b) {
      value = new Tuple<double, double>(_a, _b);
    }
    
    public void Set(Tuple<double,double> _value) {
      value = _value;
    }
    
  }

}
