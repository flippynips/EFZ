/*
 * User: FloppyNipples
 * Date: 17/07/2017
 * Time: 10:40
 */
using System;

namespace Efz.Web.Javascript {
  
  /// <summary>
  /// Representation of a base javascript object.
  /// </summary>
  public abstract class Js {
    
    //----------------------------------//
    
    /// <summary>
    /// Base js type reference.
    /// </summary>
    public static Type Type = typeof(Js);
    
    /// <summary>
    /// Var declaration.
    /// </summary>
    public const string Var = "var ";
    /// <summary>
    /// Equality string.
    /// </summary>
    public const string Equal = " = ";
    /// <summary>
    /// Or equality operator.
    /// </summary>
    public const string Or = " || ";
    /// <summary>
    /// Prototype reference string.
    /// </summary>
    public const string PrototypeReference = ".prototype";
    /// <summary>
    /// Function declaration.
    /// </summary>
    public const string Function = "function(";
    /// <summary>
    /// 'This' reference.
    /// </summary>
    public const string This = "this.";
    /// <summary>
    /// Null reference.
    /// </summary>
    public const string Null = "null";
    
    /// <summary>
    /// Determines whether the specified type is an ancestor of
    /// the base Js class.
    /// </summary>
    public static bool IsAncestor(Type type) {
      while(type != Js.Type) {
        type = type.BaseType;
        if(type == null) return false;
      }
      return true;
    }
    
    //----------------------------------//
    
    //----------------------------------//
    
    internal Js() {
    }
    
    /// <summary>
    /// Main method used to build javascript components.
    /// </summary>
    public abstract void Build(JsBuilder builder);
    
    //----------------------------------//
    
  }
  
}
