/*
 * User: FloppyNipples
 * Date: 30/03/2017
 * Time: 15:48
 */
using System;

namespace Efz.Web {
  
  /// <summary>
  /// Base web routine class. Represents a module that will potentially process POST request parameters,
  /// build web elements for a response ect.
  /// </summary>
  public abstract class HttpRoutine {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Path to this routine.
    /// </summary>
    public readonly string Path;
    /// <summary>
    /// Methods handled by this routine.
    /// </summary>
    public readonly HttpMethod Methods;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new operation.
    /// </summary>
    protected HttpRoutine(string path, HttpMethod methods) {
      Path = path.ToLowercase();
      Methods = methods;
    }
    
    /// <summary>
    /// On a web request.
    /// </summary>
    public abstract void OnRequest(string path, HttpRequest request);
    
    //-------------------------------------------//
    
  }
  
}
