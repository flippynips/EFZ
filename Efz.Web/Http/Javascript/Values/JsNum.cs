/*
 * User: FloppyNipples
 * Date: 17/07/2017
 * Time: 10:40
 */
using System;

using Efz.Web.Javascript;

namespace Efz.Web.Javascript {
  
  /// <summary>
  /// Representation of a base javascript object.
  /// </summary>
  public class JsNum : JsValue<double> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a new js integer with the specified value.
    /// </summary>
    internal JsNum(double number) {
      Value = number;
    }
    
    /// <summary>
    /// Build the javascript.
    /// </summary>
    public override void Build(JsBuilder builder) {
      builder.String.Append(Value);
    }
    
    public override string ToString() {
      return Value.ToString();
    }
    
    public static JsNum operator +(JsNum jsNum, double number) {
      return new JsNum(number + jsNum.Value);
    }
    
    public static JsNum operator +(double number, JsNum jsNum) {
      return new JsNum(number + jsNum.Value);
    }
    
    public static JsNum operator -(JsNum jsNum, double number) {
      return new JsNum(jsNum.Value - number);
    }
    
    public static JsNum operator -(double number, JsNum jsNum) {
      return new JsNum(number - jsNum.Value);
    }
    
    public static JsNum operator /(JsNum jsNum, double number) {
      return new JsNum(jsNum.Value / number);
    }
    
    public static JsNum operator /(double number, JsNum jsNum) {
      return new JsNum(number / jsNum.Value);
    }
    
    public static JsNum operator *(JsNum jsNum, double number) {
      return new JsNum(jsNum.Value * number);
    }
    
    public static JsNum operator *(double number, JsNum jsNum) {
      return new JsNum(number * jsNum.Value);
    }
    
    //----------------------------------//
    
  }
  
}
