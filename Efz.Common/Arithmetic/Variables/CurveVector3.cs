using System;

namespace Efz.Maths {
    
  public class CurveVector3 : Curve<Vector3> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    public override Vector3 Get(double delta) {
      if(Deltas.Count > 3) {
        int index;
        switch(Interpolation) {
        case Interpolation.Linear:
          index = Deltas.Count-1;
          while(--index > 0) {
            if(delta > Deltas[index]) {
              break;
            }
          }
          delta = (delta - Deltas[index]) / (Deltas[index+1] - Deltas[index]);
          return Values[index] * (1 - delta) + Values[index+1] * delta;
        case Interpolation.Cosine:
          index = Deltas.Count-1;
          while(--index > 0) {
            if(delta > Deltas[index]) {
              break;
            }
          }
          delta = (delta - Deltas[index]) / (Deltas[index+1] - Deltas[index]);
          delta = (1-Math.Cos(delta * Meth.Pi))/2;
          return Values[index] * (1 - delta) + Values[index+1] * delta;
        case Interpolation.Cubic:
          index = Deltas.Count-4;
          while(--index > 0) {
            if(delta > Deltas[index]) {
              break;
            }
          }
          delta = (delta - Deltas[index]) / (Deltas[index+1] - Deltas[index]);
          
          double delta2 = delta * delta;
          Vector3 a0 = Values[index+3] - Values[index+2] - Values[index] + Values[index+1];
          Vector3 a1 = Values[index] - Values[index+1] - a0;
          Vector3 a2 = Values[index+2] - Values[index];
          Vector3 a3 = Values[index+1];
          
          return a0 * delta * delta2 + a1 * delta2 + a2 * delta + a3;
        }
      } else {
        switch(Deltas.Count) {
        case 3:
          int index;
          switch(Interpolation) {
          case Interpolation.Linear:
            index = Deltas.Count-1;
            while(--index > 0) {
              if(delta > Deltas[index]) {
                break;
              }
            }
            delta = (delta - Deltas[index]) / (Deltas[index+1] - Deltas[index]);
            return Values[index] * (1 - delta) + Values[index+1] * delta;
          case Interpolation.Cosine:
          case Interpolation.Cubic:
            index = Deltas.Count-1;
            while(--index > 0) {
              if(delta > Deltas[index]) {
                break;
              }
            }
            delta = (delta - Deltas[index]) / (Deltas[index+1] - Deltas[index]);
            delta = (1-Math.Cos(delta * Meth.Pi))/2;
            return Values[index] * (1 - delta) + Values[index+1] * delta;
          }
          break;
        case 2:
          switch(Interpolation) {
          case Interpolation.Linear:
            delta = (delta - Deltas[0]) / (Deltas[1] - Deltas[0]);
            return Values[0] * (1 - delta) + Values[1] * delta;
          case Interpolation.Cosine:
          case Interpolation.Cubic:
            delta = (delta - Deltas[0]) / (Deltas[1] - Deltas[0]);
            delta = (1-Math.Cos(delta * Meth.Pi))/2;
            return Values[0] * (1 - delta) + Values[1] * delta;
          }
          break;
        case 1:
          return Values[0];
        }
      }
      return Vector3.Zero;
    }
    
    //-------------------------------------------//
    
  }

}
