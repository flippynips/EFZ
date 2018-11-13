using System;


namespace Efz.Maths {
    
  public class CurveDouble : Curve<double> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    override public double Get(double value) {
      if(Deltas.Count > 3) {
        int index;
        switch(Interpolation) {
        case Interpolation.Linear:
          index = Deltas.Count-1;
          while(--index > 0) {
            if(value > Deltas[index]) {
              break;
            }
          }
          value = (value - Deltas[index]) / (Deltas[index+1] - Deltas[index]);
          return Values[index] * (1 - value) + Values[index+1] * value;
        case Interpolation.Cosine:
          index = Deltas.Count-1;
          while(--index > 0) {
            if(value > Deltas[index]) {
              break;
            }
          }
          value = (value - Deltas[index]) / (Deltas[index+1] - Deltas[index]);
          value = (1-Math.Cos(value * Meth.Pi))/2;
          return Values[index] * (1 - value) + Values[index+1] * value;
        case Interpolation.Cubic:
          index = Deltas.Count-4;
          while(--index > 0) {
            if(value > Deltas[index]) {
              break;
            }
          }
          value = (value - Deltas[index]) / (Deltas[index+1] - Deltas[index]);

          double delta2 = value * value;
          double a0 = Values[index+3] - Values[index+2] - Values[index] + Values[index+1];
          double a1 = Values[index] - Values[index+1] - a0;
          double a2 = Values[index+2] - Values[index];
          double a3 = Values[index+1];

          return a0 * value * delta2 + a1 * delta2 + a2 * value + a3;
        }
      } else {
        switch(Deltas.Count) {
        case 3:
          int index;
          switch(Interpolation) {
          case Interpolation.Linear:
            index = Deltas.Count-1;
            while(--index > 0) {
              if(value > Deltas[index]) {
                break;
              }
            }
            value = (value - Deltas[index]) / (Deltas[index+1] - Deltas[index]);
            return Values[index] * (1 - value) + Values[index+1] * value;
          case Interpolation.Cosine:
          case Interpolation.Cubic:
            index = Deltas.Count-1;
            while(--index > 0) {
              if(value > Deltas[index]) {
                break;
              }
            }
            value = (value - Deltas[index]) / (Deltas[index+1] - Deltas[index]);
            value = (1-Math.Cos(value * Meth.Pi))/2;
            return Values[index] * (1 - value) + Values[index+1] * value;
          }
          break;
        case 2:
          switch(Interpolation) {
          case Interpolation.Linear:
            value = (value - Deltas[0]) / (Deltas[1] - Deltas[0]);
            return Values[0] * (1 - value) + Values[1] * value;
          case Interpolation.Cosine:
          case Interpolation.Cubic:
            value = (value - Deltas[0]) / (Deltas[1] - Deltas[0]);
            value = (1-Math.Cos(value * Meth.Pi))/2;
            return Values[0] * (1 - value) + Values[1] * value;
          }
          break;
        case 1:
          return Values[0];
        }
      }
      return 0;
    }
    
    //-------------------------------------------//
    
  }

}
