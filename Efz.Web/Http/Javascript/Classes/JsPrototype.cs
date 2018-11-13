/*
 * User: FloppyNipples
 * Date: 17/07/2017
 * Time: 13:45
 */
using System;
using System.Collections.Generic;
using System.Reflection;

using Efz.Collections;

namespace Efz.Web.Javascript {
  
  /// <summary>
  /// Definition of a javascript object initializer. Just as JsObj describes
  /// an object instance, this class describes the template of that object.
  /// </summary>
  public class JsPrototype {
    
    //----------------------------------//
    
    /// <summary>
    /// Name of the prototype.
    /// </summary>
    public string Name;
    
    /// <summary>
    /// Parameters used to create the js object.
    /// </summary>
    public ArrayRig<Dictionary<string, Js>> Constructors;
    /// <summary>
    /// Properties of the javascript object.
    /// </summary>
    public Dictionary<string, Js> Properties;
    /// <summary>
    /// Functions of the javascript object.
    /// </summary>
    public Dictionary<string, JsMethod> Methods;
    
    //----------------------------------//
    
    /// <summary>
    /// Command to run during construction.
    /// </summary>
    protected JsCommandString _constructor;
    
    //----------------------------------//
    
    /// <summary>
    /// Create a template from the specified js obj class.
    /// </summary>
    public JsPrototype(JsClass jsClass) {
      
      Constructors = new ArrayRig<Dictionary<string, Js>>();
      Properties = new Dictionary<string, Js>();
      Methods = new Dictionary<string, JsMethod>();
      
      var type = jsClass.GetType();
      Name = type.Name;
      
      var constructorMethod = type.GetMethod("Constructor", BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly, null, new Type[0], null);
      if(constructorMethod != null && constructorMethod.ReturnType == typeof(string)) {
        _constructor = new JsCommandString((string)constructorMethod.Invoke(null, null));
      }
      
      // iterate class constructors
      foreach(var constructor in type.GetConstructors()) {
        
        var parameters = new Dictionary<string, Js>();
        Constructors.Add(parameters);
        
        // iterate parameters
        foreach(var parameter in constructor.GetParameters()) {
          
          // is the parameter a JsBuilder?
          if(parameter.ParameterType == typeof(JsBuilder)) continue;
          
          // is the type valid?
          if(!Js.IsAncestor(parameter.ParameterType))
            throw new InvalidOperationException("Parameter '"+parameter.Name+"' in class '"+Name+"' constructor doesn't inherit from Js.");
          
          // add the parameter
          parameters.Add(parameter.Name, parameter.HasDefaultValue ? (Js)parameter.DefaultValue : null);
          
        }
        
      }
      
      // iterate the class properties
      foreach(var property in type.GetProperties(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance)) {
        var attribute = property.GetCustomAttribute<JsProperty>(true);
        if(attribute == null) continue;
        
        if(!Js.IsAncestor(property.PropertyType))
          throw new InvalidOperationException("Property '"+property.Name+"' in class '"+Name+"' doesn't inherit from Js.");
        
        var propertyValue = property.GetValue(jsClass);
        if(propertyValue == null) Properties.Add(property.Name, null);
        else Properties.Add(property.Name, (Js)propertyValue);
        
      }
      
      // iterate class functions
      foreach(var method in type.GetMethods(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance)) {
        var attribute = method.GetCustomAttribute<JsFunction>(true);
        if(attribute == null) continue;
        
        Methods.Add(method.Name, new JsMethod(jsClass, method));
        
      }
      
    }
    
    /// <summary>
    /// Build the javascript object.
    /// </summary>
    public void Build(JsBuilder builder) {
      
      bool first = true;
      
      // no constructors? no problem
      if(Constructors.Count == 0) {
        builder.String.Append(Js.Var);
        builder.String.Append(Name);
        builder.String.Append(Js.Equal);
        builder.String.Append(Js.Function);
        builder.String.Append(Chars.BracketClose);
        builder.String.Append(Chars.BraceOpen);
        builder.String.Append(Chars.BraceClose);
        builder.String.Append(Chars.SemiColon);
      }
      
      // iterate constructors
      foreach(var constructor in Constructors) {
        builder.String.Append(Js.Var);
        builder.String.Append(Name);
        builder.String.Append(Js.Equal);
        builder.String.Append(Js.Function);
        
        first = true;
        
        // iterate the parameters
        foreach(var parameter in constructor) {
          if(first) first = false;
          else builder.String.Append(Chars.Comma);
          builder.String.Append(parameter.Key);
        }
        builder.String.Append(Chars.BracketClose);
        builder.String.Append(Chars.BraceOpen);
        
        // iterate the passed parameters
        foreach(var parameter in constructor) {
          builder.String.Append(Js.This);
          builder.String.Append(parameter.Key);
          builder.String.Append(Js.Equal);
          if(parameter.Value == null) builder.String.Append(parameter.Key);
          else {
            builder.String.Append(parameter.Key);
            builder.String.Append(Js.Or);
            parameter.Value.Build(builder);
          }
          builder.String.Append(Chars.SemiColon);
        }
        
        // iterate static properties
        foreach(var property in Properties) {
          builder.String.Append(property.Key);
          builder.String.Append(Js.Equal);
          if(property.Value == null) builder.String.Append(Js.Null);
          else property.Value.Build(builder);
          builder.String.Append(Chars.SemiColon);
        }
        
        if(_constructor != null) _constructor.Build(builder);
        
        builder.String.Append(Chars.BraceClose);
        builder.String.Append(Chars.SemiColon);
      }
      
      // iterate class functions
      foreach(var method in Methods) {
        builder.String.Append(Name);
        builder.String.Append(Js.PrototypeReference);
        builder.String.Append(Chars.Stop);
        builder.String.Append(method.Key);
        builder.String.Append(Js.Equal);
        method.Value.Build(builder);
      }
      
    }
    
    //----------------------------------//
    
  }
  
}
