/*
 * User: Bob
 * Date: 14/11/2016
 * Time: 12:44
 */
/*
 * User: Joshua
 * Date: 6/08/2016
 * Time: 6:11 PM
 */
using System;
using Efz.Web.Display;

namespace Efz.Web.Client.Scripts {
  
  /// <summary>
  /// Custom script.
  /// </summary>
  public class CustomScript : Script {
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// The script.
    /// </summary>
    protected string _script;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize the custom script.
    /// </summary>
    public CustomScript(Element element, string script) : base(element) {
      _script = script;
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Append the script.
    /// </summary>
    protected override void Append(System.Text.StringBuilder builder) {
      builder.Append(_script);
    }
    
  }
  
}
