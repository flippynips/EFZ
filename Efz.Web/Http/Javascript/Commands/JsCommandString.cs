/*
 * User: FloppyNipples
 * Date: 17/07/2017
 * Time: 13:12
 */
using System;

namespace Efz.Web.Javascript {
  
  /// <summary>
  /// Custom javascript command.
  /// </summary>
  public class JsCommandString : JsCommand {
    
    //----------------------------------//
    
    public string Javascript;
    
    //----------------------------------//
    
    //----------------------------------//
    
    internal JsCommandString(string javascript) {
      Javascript = javascript;
    }
    
    /// <summary>
    /// Append the command.
    /// </summary>
    public override void Build(JsBuilder builder) {
      builder.String.Append(Javascript);
    }
    
    //----------------------------------//
    
  }
  
}
