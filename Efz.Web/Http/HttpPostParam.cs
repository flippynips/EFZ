/*
 * User: FloppyNipples
 * Date: 20/03/2017
 * Time: 01:02
 */
using System;
using System.Collections.Generic;

namespace Efz.Web {
  
  /// <summary>
  /// Interface for all parameter types.
  /// </summary>
  public interface IHttpPostParam {
    Dictionary<string,string> Params { get; }
  }
  
  /// <summary>
  /// A collection of post parameters that can represent numerous value types.
  /// </summary>
  public class HttpPostParam<T> : IHttpPostParam {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The post paramter value.
    /// </summary>
    public T Value;
    /// <summary>
    /// Parameters that describe the post parameter value.
    /// </summary>
    public Dictionary<string, string> Params {
      get { return _params; }
    }
    
    /// <summary>
    /// Get a parameter by key.
    /// </summary>
    public string this[string key] {
      get {
        string value;
        return _params.TryGetValue(key, out value) ? value : string.Empty;
      }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Inner collection of parameters.
    /// </summary>
    protected Dictionary<string,string> _params;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new, empty post parameter.
    /// </summary>
    public HttpPostParam() {
      _params = new Dictionary<string, string>();
    }
    
    /// <summary>
    /// Initialize a new post parameter.
    /// </summary>
    public HttpPostParam(T value) {
      Value = value;
      _params = new Dictionary<string, string>();
    }
    
    public HttpPostParam(T value, Dictionary<string,string> parameters) {
      Value = value;
      _params = parameters;
    }
    
    /// <summary>
    /// Get a string representation of the post parameter.
    /// </summary>
    public override string ToString() {
      return _params.Join() + " - " + Value;
    }
    
    //-------------------------------------------//
    
  }
}
