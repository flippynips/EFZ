/*
 * User: FloppyNipples
 * Date: 17/07/2017
 * Time: 12:08
 */
using System;

namespace Efz.Web.Javascript {
  
  /// <summary>
  /// Base javascript variable reference.
  /// </summary>
  public class JsVar : JsValue<Js> {
    
    //----------------------------------//
    
    /// <summary>
    /// Name of the variable. Can be null if the variable is dynamically defined.
    /// </summary>
    public string Name;
    
    //----------------------------------//
    
    //----------------------------------//
    
    protected JsVar(string name, Js value) {
      Name = name;
      Value = value;
    }
    
    protected JsVar(Js value) {
      Value = value;
    }
    
    public override void Build(JsBuilder builder) {
      if(string.IsNullOrEmpty(Name)) Name = builder.NextName;
      builder.String.Append(Name);
    }
    
    public override string ToString() {
      return Name;
    }
    
    public static implicit operator string(JsVar var) {
      return var.Name;
    }
    
    public static implicit operator JsVar(JsStr jsStr) {
      return new JsVar(jsStr);
    }
    
    public static implicit operator JsVar(JsNum jsNum) {
      return new JsVar(jsNum);
    }
    
    public static implicit operator JsVar(JsClass jsClass) {
      return new JsVar(jsClass);
    }
    
    public static implicit operator JsVar(JsMethod jsMethod) {
      return new JsVar(jsMethod);
    }
    
    //----------------------------------//
    
    
  }
}
