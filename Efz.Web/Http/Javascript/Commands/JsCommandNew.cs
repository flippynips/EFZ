/*
 * User: FloppyNipples
 * Date: 17/07/2017
 * Time: 13:12
 */
using System;

namespace Efz.Web.Javascript {
  
  /// <summary>
  /// Base javascript command class. Building block of javascript functionality.
  /// Essentially the definition of a single line of js code.
  /// </summary>
  public class JsCommandNew : JsCommand {
    
    //----------------------------------//
    
    /// <summary>
    /// Object that will be created with this command.
    /// </summary>
    public JsClass Class;
    /// <summary>
    /// Collection of parameters.
    /// </summary>
    public Js[] Parameters;
    
    //----------------------------------//
    
    //----------------------------------//
    
    internal JsCommandNew(JsClass jsObj, Js[] parameters) {
      Class = jsObj;
      Parameters = parameters;
    }
    
    /// <summary>
    /// Append the command.
    /// </summary>
    public override void Build(JsBuilder builder) {
      builder.String.Append("new ");
      builder.String.Append(Class.Prototype.Name);
      builder.String.Append(Chars.BracketOpen);
      bool first = true;
      foreach(var parameter in Parameters) {
        if(first) first = false;
        else builder.String.Append(Chars.Comma);
        parameter.Build(builder);
      }
      builder.String.Append(Chars.BracketClose);
      builder.String.Append(Chars.SemiColon);
    }
    
    //----------------------------------//
    
  }
  
}
