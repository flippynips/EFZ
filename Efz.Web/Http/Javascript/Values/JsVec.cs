/*
 * User: FloppyNipples
 * Date: 17/07/2017
 * Time: 10:40
 */
using System;

using Efz.Maths;
using Efz.Web.Javascript;

namespace Efz.Web.Javascript.Values {
  
  /// <summary>
  /// Representation of a vector3.
  /// </summary>
  public class JsVec : JsValue<Vector2> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a new js vector with the specified values.
    /// </summary>
    internal JsVec(double x, double y) {
      Value = new Vector2(x, y);
    }
    
    /// <summary>
    /// Initialize a new js integer with the specified value.
    /// </summary>
    internal JsVec(Vector2 vector) {
      Value = vector;
    }
    
    /// <summary>
    /// Build the javascript.
    /// </summary>
    public override void Build(JsBuilder builder) {
      builder.String.Append(Value);
    }
    
    public override bool Equals(object obj) {
      return (obj is JsVec) && Equals((JsVec)obj);
    }
    
    public bool Equals(JsVec other) {
      return Value == other.Value;
    }

    public override int GetHashCode() {
      return Value.GetHashCode();
    }
    
    public override string ToString() {
      return Value.ToString();
    }
    
    public static implicit operator Vector2(JsVec jsVec) {
      return jsVec.Value;
    }
    
    public static JsVec operator +(JsVec jsVec, Vector2 vector) {
      return new JsVec(jsVec.Value + vector);
    }
    
    public static JsVec operator +(Vector2 vector, JsVec jsVec) {
      return new JsVec(vector + jsVec.Value);
    }
    
    public static JsVec operator -(JsVec jsVec, Vector2 vector) {
      return new JsVec(jsVec.Value - vector);
    }
    
    public static JsVec operator -(Vector2 vector, JsVec jsVec) {
      return new JsVec(vector - jsVec.Value);
    }
    
    public static JsVec operator /(JsVec jsVec, Vector2 vector) {
      return new JsVec(jsVec.Value / vector);
    }
    
    public static JsVec operator /(Vector2 vector, JsVec jsVec) {
      return new JsVec(vector / jsVec.Value);
    }
    
    public static JsVec operator *(JsVec jsVec, Vector2 vector) {
      return new JsVec(jsVec.Value * vector);
    }
    
    public static JsVec operator *(Vector2 vector, JsVec jsVec) {
      return new JsVec(vector * jsVec.Value);
    }
    
    //----------------------------------//
    
  }
  
}
