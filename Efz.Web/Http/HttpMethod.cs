using System;

namespace Efz.Web {

  /// <summary>
  /// Type of http request.
  /// </summary>
  [Flags]
  public enum HttpMethod {
    None   = 1<<0,
    Get    = 1<<1,
    Post   = 1<<2,
    Put    = 1<<3,
    Update = 1<<4,
    Delete = 1<<5,
  }
  
  
  /// <summary>
  /// Extension methods for method types.
  /// </summary>
  public static class ExtendMethodType {
    
    /// <summary>
    /// Does this method include the specified method.
    /// </summary>
    public static bool Is(this HttpMethod methodType, HttpMethod other) {
      return (methodType & other) == other;
    }
    
  }
  
}

