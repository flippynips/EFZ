/*
 * User: FloppyNipples
 * Date: 05/02/2017
 * Time: 17:16
 */
using System;

using Efz.Data;

namespace Efz.Web {
  
  /// <summary>
  /// Attributes that describe requirements for a web resource.
  /// </summary>
  public class HttpRequirement {
    
    //----------------------------------//
    
    /// <summary>
    /// Path the requirement represents.
    /// </summary>
    public readonly string Path;
    
    /// <summary>
    /// Required client attributes in order to access the resource. Checked if
    /// not null.
    /// </summary>
    public Node Attributes;
    /// <summary>
    /// Delegate used for custom authentication handler.
    /// </summary>
    public delegate bool OnRequestDelegate(HttpConnection connection, string path);
    /// <summary>
    /// If not null, this will be run to authenticate the client for the
    /// specified resource.
    /// </summary>
    public OnRequestDelegate OnRequest;
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize authentication for the specified path.
    /// </summary>
    internal HttpRequirement(string path) {
      Path = path;
    }
    
    /// <summary>
    /// Is the client allowed to access the resource?
    /// </summary>
    public bool Allow(HttpRequest request) {
      // are the attributes set?
      if(!request.Client.Attributes.Contains(Attributes)) return false;
      
      // return the result of the authentication
      return OnRequest == null || OnRequest(request.Connection, Path);
    }
    
    /// <summary>
    /// Save the authentication into the node.
    /// </summary>
    public void Save(Node node) {
      // set the path
      node["Path"].String = Path;
      node["Attributes"] = Attributes;
    }
    
    /// <summary>
    /// Load the requirement from the node.
    /// </summary>
    public static HttpRequirement Load(Node node) {
      // create the authentication
      var authentication = new HttpRequirement(node["Path"].String);
      // copy the assigned attributes
      authentication.Attributes = Node.Clone(node["Attributes"]);
      
      return authentication;
    }
    
    //----------------------------------//
    
  }
  
  
}
