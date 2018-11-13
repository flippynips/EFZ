/*
 * User: Joshua
 * Date: 4/06/2016
 * Time: 11:55 PM
 */
using System;

using Efz.Maths;


namespace Efz {
  
  /// <summary>
  /// Value math related extensions.
  /// </summary>
  static public class ExtendMath {
    
    /// <summary>
    /// Common integer max value with a single digit buffer.
    /// </summary>
    public const int IntMaxBuffer = int.MaxValue/10;
    
    /// <summary>
    /// Determine whether two values are equal using an epsilon value to remove floating point errors.
    /// </summary>
    public static bool Equal(this float valueA, float valueB) {
      return Math.Abs(valueA - valueB) < Meth.Epsilon;
    }
    
    /// <summary>
    /// Determine whether two values are equal using an epsilon value to remove floating point errors.
    /// </summary>
    public static bool Equal(this double valueA, double valueB) {
      return Math.Abs(valueA - valueB) < Meth.Epsilon;
    }
    
    /// <summary>
    /// Determine whether two values are equal using an epsilon value to remove floating point errors.
    /// </summary>
    public static bool Equal(this double valueA, double valueB, double epsilon) {
      return Math.Abs(valueA - valueB) < epsilon;
    }
    
    /// <summary>
    /// Combine the integer with the log10 base of another.
    /// </summary>
    public static int Combine(this int valueA, int valueB) {
      return valueA + (int)Math.Floor(Math.Log10(valueA) + 1) * valueB;
    }
    /// <summary>
    /// Combine the uint with the log10 base of another.
    /// </summary>
    public static uint Combine(this uint valueA, uint valueB) {
      return valueA + (uint)Math.Floor(Math.Log10(valueA) + 1) * valueB;
    }
    
    public static ushort Lerp(this ushort value1, ushort value2, float delta) {
      return (ushort)(value1 + (value2 - value1) * delta);
    }
    
    public static int Lerp(this int value1, int value2, float delta) {
      return (int)(value1 + (value2 - value1) * delta);
    }
    
    public static float Lerp(this float value1, float value2, float delta) {
      return value1 + (value2 - value1) * delta;
    }
    
    public static double Lerp(this double value1, double value2, double delta) {
      return value1 + (value2 - value1) * delta;
    }
    
    public static Vector2 Lerp(this Vector2 value1, Vector2 value2, double delta) {
      return new Vector2(value1.X.Lerp(value2.X, delta), value1.Y.Lerp(value2.Y, delta));
    }

    public static Vector3 Lerp(this Vector3 value1, Vector3 value2, double delta) {
      return new Vector3(value1.X.Lerp(value2.X, delta), value1.Y.Lerp(value2.Y, delta), value1.Z.Lerp(value2.Z, delta));
    }
    
    public static Vector2 Normalize(this Vector2 vector) {
      double magnitude = 1 / vector.Magnitude;
      vector.X *= magnitude;
      vector.Y *= magnitude;
      return vector;
    }

    public static Vector2 Maximize(this Vector2 vector) {
      double magnitude = 1 / vector.Magnitude;
      if(magnitude < 1) {
        vector.X *= magnitude;
        vector.Y *= magnitude;
      }
      return vector;
    }
    
    public static Vector3 Maximize(this Vector3 vector) {
      double magnitude = 1 / vector.Magnitude;
      if(magnitude < 1) {
        vector.X *= magnitude;
        vector.Y *= magnitude;
        vector.Z *= magnitude;
      }
      return vector;
    }
    
    /// <summary>
    /// Get the shortest, circular distance between ids.
    /// </summary>
    public static ulong Distance(this ulong valueA, ulong valueB) {

      const ulong halfULong = ulong.MaxValue / 2;

      ulong distance;
      if(valueA > valueB) distance = valueA - valueB;
      else distance = valueB - valueB;
      if(distance > halfULong) distance = ulong.MaxValue - distance;
      return distance;

    }
    
  }
}
