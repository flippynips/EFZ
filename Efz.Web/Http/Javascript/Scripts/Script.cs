/*
 * User: Bob
 * Date: 14/11/2016
 * Time: 10:53
 */
/*
 * User: Joshua
 * Date: 6/08/2016
 * Time: 6:11 PM
 */
using System;
using System.Text;
using Efz.Collections;
using Efz.Web.Display;

namespace Efz.Web.Client.Scripts {
  
  /// <summary>
  /// Base class for javascript controllers.
  /// </summary>
  public abstract class Script {
    
    //----------------------------------//
    
    /// <summary>
    /// The web page this script belongs to.
    /// </summary>
    public Element Element;
    /// <summary>
    /// Collection of parameters this script will handle.
    /// </summary>
    public ArrayRig<string> Parameters;
    
    /// <summary>
    /// Unique name for the sript.
    /// </summary>
    public string Name {
      get {
        if(_name == null) _name = "TODO";
        return _name;
      }
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Inner name of the script.
    /// </summary>
    protected string _name;
    
    /// <summary>
    /// Start a function.
    /// </summary>
    protected const string _openFunction = "function(";
    /// <summary>
    /// End a function.
    /// </summary>
    protected const string _closeFuncion = ");\n";
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize the base component of a script.
    /// </summary>
    protected Script(Element element) {
      Element = element;
    }
    
    /// <summary>
    /// Get append the script to the specified builder.
    /// </summary>
    protected virtual string ToString(StringBuilder builder) {
      // has the name been assigned
      if(_name != null) {
        // yes, set it as the name
        builder.Append(_name);
        builder.Append(Chars.Equal);
      }
      builder.Append(_openFunction);
      Append(builder);
      return builder.ToString();
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Get the javascript representation of the script item. 
    /// </summary>
    protected abstract void Append(StringBuilder builder);
    
  }
  
}
