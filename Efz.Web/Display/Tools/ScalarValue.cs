/*
 * User: Joshua
 * Date: 1/11/2016
 * Time: 12:59 AM
 */
using System;

using Efz.Data;
using Efz.Web.Display;

namespace Efz.Web.Display {
  
  /// <summary>
  /// A value with a medium.
  /// </summary>
  public class ScalarValue {
    
    //----------------------------------//
    
    /// <summary>
    /// Value of the scalar value in whole numbers.
    /// </summary>
    public int Value;
    /// <summary>
    /// Medium of the value.
    /// </summary>
    public ValueMedium Medium = ValueMedium.None;
    
    //----------------------------------//
    
    //----------------------------------//
    
    public ScalarValue(int value) {
      Value = value;
    }
    
    public ScalarValue(int value, ValueMedium medium) {
      Value = value;
      Medium = medium;
    }
    
    /// <summary>
    /// Set the value as the specified medium.
    /// </summary>
    public void Set(float value, ValueMedium medium) {
      Medium = medium;
      switch(Medium) {
        case ValueMedium.Percent:
          Value = (int)(value * 10000);
          break;
        default:
          Value = (int)value;
          break;
      }
    }
    
    /// <summary>
    /// Get the value as the specified medium possibly using the parents
    /// value in the calculation.
    /// </summary>
    public float Get(ValueMedium medium, int parentPixels) {
      switch(Medium) {
        case ValueMedium.Percent:
          switch(medium) {
            case ValueMedium.Percent:
              return Value / 10000.0f;
            case ValueMedium.Pixels:
              return (Value / 10000.0f) * parentPixels;
          }
          break;
        case ValueMedium.Pixels:
          switch(medium) {
            case ValueMedium.Percent:
              return (float)parentPixels / Value;
          }
          break;
      }
      return Value;
    }
    
    public static implicit operator ScalarValue(int value) {
      return new ScalarValue(value);
    }
    
    public static implicit operator int(ScalarValue value) {
      return value.Value;
    }
    
    /// <summary>
    /// Subtract one scalar value from another.
    /// </summary>
    public static ScalarValue operator +(ScalarValue valueA, ScalarValue valueB) {
      if(valueA.Medium == ValueMedium.None ||
        valueB.Medium == ValueMedium.None ||
        valueA.Medium == valueB.Medium) {
        return valueA.Value + valueB.Value;
      }
      throw new Exception("Scalar values do not have the same medium.");
    }
    
    /// <summary>
    /// Add one scalar value from another.
    /// </summary>
    public static ScalarValue operator -(ScalarValue valueA, ScalarValue valueB) {
      if(valueA.Medium == ValueMedium.None ||
        valueB.Medium == ValueMedium.None ||
        valueA.Medium == valueB.Medium) {
        return valueA.Value - valueB.Value;
      }
      throw new Exception("Scalar values do not have the same medium.");
    }
    
    /// <summary>
    /// Multiply one scalar value with another.
    /// </summary>
    public static ScalarValue operator *(ScalarValue valueA, ScalarValue valueB) {
      if(valueA.Medium == ValueMedium.None ||
        valueB.Medium == ValueMedium.None ||
        valueA.Medium == valueB.Medium) {
        switch(valueA.Medium) {
          case ValueMedium.Percent:
            return (int)(valueA.Value / 10000.0f + valueB.Value);
          default:
            return valueA.Value * valueB.Value;
        }
      }
      throw new Exception("Scalar values do not have the same medium.");
    }
    
    /// <summary>
    /// Subtract one scalar value from another.
    /// </summary>
    public static ScalarValue operator /(ScalarValue valueA, ScalarValue valueB) {
      if(valueA.Medium == ValueMedium.None ||
        valueB.Medium == ValueMedium.None ||
        valueA.Medium == valueB.Medium) {
        return valueA.Value / valueB.Value;
      }
      throw new Exception("Scalar values do not have the same medium.");
    }
    
    //----------------------------------//
    
  }
  
}
