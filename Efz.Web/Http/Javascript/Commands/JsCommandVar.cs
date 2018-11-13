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
  public class JsCommandVar : JsCommand {
    
    //----------------------------------//
    
    /// <summary>
    /// Object that will be created with this command.
    /// </summary>
    public JsVar Variable;
    
    //----------------------------------//
    
    //----------------------------------//
    
    internal JsCommandVar(JsVar jsVar) {
      Variable = jsVar;
    }
    
    /// <summary>
    /// Append the command.
    /// </summary>
    public override void Build(JsBuilder builder) {
      builder.String.Append(Js.Var);
      Variable.Build(builder);
      builder.String.Append(Js.Equal);
      Variable.Value.Build(builder);
    }
    
    //----------------------------------//
    
  }
  
}
