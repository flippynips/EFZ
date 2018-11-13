using System;

using Efz.Collections;

namespace Efz.Maths {
  
  public enum Interpolation : byte {
    Linear,
    Cosine,
    Cubic
  }
  
  /// <summary>
  /// Base class for generic interpolated unit curves.
  /// </summary>
  abstract public class Curve<B> : ITranslate<double,B> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Interpolation to apply between points in the curve.
    /// </summary>
    public Interpolation Interpolation = Interpolation.Linear;
    /// <summary>
    /// Values of the curve.
    /// </summary>
    public ArrayRig<B> Values;
    /// <summary>
    /// Deltas of the curve.
    /// </summary>
    public ArrayRig<double> Deltas;
    
    //-------------------------------------------//
    
    
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new curve.
    /// </summary>
    protected Curve() {
      Values = new ArrayRig<B>();
      Deltas = new ArrayRig<double>();
    }
    
    /// <summary>
    /// Add the next value at the specified delta time.
    /// </summary>
    public void Add(double delta, B value) {
      switch(Values.Count) {
        case 0:
          Values.Add(value);
          Deltas.Add(delta);
          break;
        case 1:
          if(delta > Deltas[0]) {
            Values.Add(value);
            Deltas.Add(delta);
          } else {
            Values.Insert(value, 0);
            Deltas.Insert(delta, 0);
          }
          break;
        default:
          int index = Values.Count + 1;
          while(--index > 0) {
            if(Deltas[index] < delta) {
              Values.Insert(value, index);
              Deltas.Insert(delta, index);
              return;
            }
            if(Deltas[index].Equal(delta)) {
              Values[index] = value;
              return;
            }
          }
          break;
      }
    }
    
    abstract public B Get(double delta);
    
    //-------------------------------------------//
    
    
    
  }

}
