using System;

namespace Efz.Maths {
    
  public struct RangeVector2 : IGet<Vector2> {
        
    //-------------------------------------------//
    
    public Vector2 GetA {
      get {
        return new Vector2(
          value.Item1.X + Randomize.Double * (value.Item2.X - value.Item1.X),
          value.Item1.Y + Randomize.Double * (value.Item2.Y - value.Item1.Y));
      }
    }
    
    public Tuple<Vector2,Vector2> value;
    
    //-------------------------------------------//
    
    
    //-------------------------------------------//
    
    public RangeVector2(Vector2 _x, Vector2 _y) {
      value = new Tuple<Vector2, Vector2>(_x, _y);
    }
    
  }

}
