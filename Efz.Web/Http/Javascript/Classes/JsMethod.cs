/*
 * User: FloppyNipples
 * Date: 17/07/2017
 * Time: 12:03
 */
using System;
using System.Collections.Generic;
using System.Reflection;

using Efz.Collections;

namespace Efz.Web.Javascript {
  
  /// <summary>
  /// Description of JsObject.
  /// </summary>
  public class JsMethod : Js {
    
    //----------------------------------//
    
    /// <summary>
    /// Parameters of the javascript function.
    /// </summary>
    public Dictionary<string, Js> Parameters;
    /// <summary>
    /// Static properties of the javascript function.
    /// </summary>
    public ArrayRig<JsCommand> Commands;
    
    //----------------------------------//
    
    //----------------------------------//
    
    internal JsMethod(JsClass jsClass, MethodInfo method) {
      Parameters = new Dictionary<string, Js>();
      Commands = new ArrayRig<JsCommand>();
      
      // does the method return a string?
      if(method.ReturnType != typeof(string))
        throw new InvalidOperationException("JsMethod '"+method.Name+"' doesn't return a string.");
      
      // iterate the parameters
      foreach(var parameter in method.GetParameters()) {
        // ensure each parameter is a Js type
        if(!Js.IsAncestor(parameter.ParameterType))
          throw new InvalidOperationException("Parameter '"+parameter.Name+"' of method '"+method.Name+"' in class '"+
            method.DeclaringType.Name+"' doesn't inherit from Js.");
        
        // add the parameter
        Parameters.Add(parameter.Name, parameter.HasDefaultValue ? (Js)parameter.DefaultValue : null);
        
      }
      
      // add the string returned from the method as the javascript command
      Commands.Add(new JsCommandString((string)method.Invoke(jsClass, new object[Parameters.Count])));
      
    }
    
    /// <summary>
    /// Create a new custom javascript method from a full string representation.
    /// </summary>
    public JsMethod(string javascript) {
      Parameters = new Dictionary<string, Js>();
      Commands = new ArrayRig<JsCommand>();
      Commands.Add(new JsCommandString(javascript));
    }
    
    /// <summary>
    /// Create a new custom javascript method from a full string representation.
    /// </summary>
    public JsMethod(Dictionary<string, Js> parameters, string javascript) {
      Parameters = parameters;
      Commands = new ArrayRig<JsCommand>();
      Commands.Add(new JsCommandString(javascript));
    }
    
    /// <summary>
    /// Build the javascript method.
    /// </summary>
    public override void Build(JsBuilder builder) {
      
      builder.String.Append(Js.Function);
      
      bool first = true;
      
      // iterate the parameters
      foreach(var parameter in Parameters) {
        if(first) first = false;
        else builder.String.Append(Chars.Comma);
        builder.String.Append(parameter.Key);
      }
      builder.String.Append(Chars.BracketClose);
      builder.String.Append(Chars.BraceOpen);
      
      // iterate the parameters
      foreach(var parameter in Parameters) {
        if(parameter.Value == null) continue;
        builder.String.Append(parameter.Key);
        builder.String.Append(Js.Equal);
        builder.String.Append(parameter.Key);
        builder.String.Append(Js.Or);
        parameter.Value.Build(builder);
        builder.String.Append(Chars.SemiColon);
      }
      
      // iterate commands within the function
      foreach(var command in Commands) {
        command.Build(builder);
      }
      
      builder.String.Append(Chars.BraceClose);
      builder.String.Append(Chars.SemiColon);
      
    }
    
    //----------------------------------//
    
  }
}
