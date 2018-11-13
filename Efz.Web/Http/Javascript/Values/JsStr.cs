/*
 * User: FloppyNipples
 * Date: 17/07/2017
 * Time: 11:16
 */
using System;

namespace Efz.Web.Javascript {
  
  /// <summary>
  /// A javascript string.
  /// </summary>
  public class JsStr : JsValue<string> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    //----------------------------------//
    
    public JsStr(string value) {
      Value = value;
    }
    
    public override void Build(JsBuilder builder) {
      builder.String.Append(Chars.DoubleQuote);
      builder.String.Append(Value);
      builder.String.Append(Chars.DoubleQuote);
    }
    
    public override string ToString() {
      return Value;
    }
    
    public static JsStr operator +(JsStr jsStr, string str) {
      return new JsStr(jsStr.Value + str);
    }
    
    public static JsStr operator +(string str, JsStr jsStr) {
      return new JsStr(str + jsStr.Value);
    }
    
    //----------------------------------//
    
  }
  
}
