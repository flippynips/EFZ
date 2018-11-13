/*
 * User: FloppyNipples
 * Date: 11/02/2017
 * Time: 12:30
 */
using System;
using Efz.Data;

namespace Efz.Web {
  
  /// <summary>
  /// Definition of a redirect that defines a replacement request path.
  /// </summary>
  public class HttpRedirect {
    
    //----------------------------------//
    
    /// <summary>
    /// The target path that will replace the request path.
    /// </summary>
    public string Target;
    /// <summary>
    /// Is the target absolute? This means the existing request path will be
    /// cleared and the specified target will be the destination. Else the
    /// remaining request path is appended to the request path.
    /// </summary>
    public bool Absolute;
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a new redirect to the specified target.
    /// </summary>
    public HttpRedirect(string target, bool absolute) {
      Target = target;
      Absolute = absolute;
    }
    
    /// <summary>
    /// Save the the redirect structure into the node.
    /// </summary>
    public void Save(Node node) {
      node["Target"].String = Target;
      node["Absolute"].Bool = Absolute;
    }
    
    /// <summary>
    /// Load the redirect from the node.
    /// </summary>
    public static HttpRedirect Load(Node node) {
      return new HttpRedirect(node["Target"].String, node["Absolute"].Bool);
    }
    
  }
  
}
