/*
 * User: FloppyNipples
 * Date: 17/07/2017
 * Time: 12:03
 */
using System;
using System.Collections.Generic;

namespace Efz.Web.Javascript {
  
  /// <summary>
  /// Base javascript class definition.
  /// </summary>
  public abstract class JsClass : Js {
    
    //----------------------------------//
    
    /// <summary>
    /// Prototype of this class.
    /// </summary>
    internal JsPrototype Prototype;
    
    //----------------------------------//
    
    /// <summary>
    /// Static definition of this js object.
    /// </summary>
    protected static Dictionary<Type, JsPrototype> _definitions;
    
    /// <summary>
    /// Parameters passed to initialize the class.
    /// </summary>
    protected Js[] _parameters;
    
    //----------------------------------//
    
    static JsClass() {
      _definitions = new Dictionary<Type, JsPrototype>();
    }
    
    /// <summary>
    /// Create a new js class with the specified parameters.
    /// </summary>
    protected JsClass(params Js[] parameters) {
      _parameters = parameters;
      var type = this.GetType();
      if(!_definitions.TryGetValue(type, out Prototype)) {
        Prototype = new JsPrototype(this);
        _definitions.Add(type, Prototype);
      }
    }
    
    /// <summary>
    /// Build a class initialization.
    /// </summary>
    public override void Build(JsBuilder builder) {
      // ensure the prototype is built
      builder.Add(Prototype);
      
      builder.String.Append("new ");
      builder.String.Append(Prototype.Name);
      builder.String.Append(Chars.BracketOpen);
      bool first = true;
      foreach(var parameter in _parameters) {
        if(first) first = false;
        else builder.String.Append(Chars.Comma);
        parameter.Build(builder);
      }
      builder.String.Append(Chars.BracketClose);
      builder.String.Append(Chars.SemiColon);
      builder.String.Append(Chars.NewLine);
    }
    
    /*
    // Constructor logic
    public static string Constructor() {
      return null;
    }
    */
    
    //----------------------------------//
    
  }
}
