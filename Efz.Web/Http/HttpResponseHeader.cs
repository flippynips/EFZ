/*
 * User: FloppyNipples
 * Date: 26/02/2017
 * Time: 19:09
 */
using System;
using System.Collections.Generic;

namespace Efz.Web {
  
  /// <summary>
  /// Headers keys for http responses.
  /// </summary>
  public enum HttpResponseHeader {
    
    //----------------------------------//
    
    CacheControl        = 1,
    Connection          = 2,
    Date                = 3,
    KeepAlive           = 4,
    Pragma              = 5,
    Trailer             = 6,
    TransferEncoding    = 7,
    Upgrade             = 8,
    Via                 = 9,
    Warning             = 10,
    Allow               = 11,
    ContentLength       = 12,
    ContentType         = 13,
    ContentEncoding     = 14,
    ContentLanguage     = 15,
    ContentLocation     = 16,
    ContentMd5          = 17,
    ContentRange        = 18,
    Expires             = 19,
    LastModified        = 20,
    AcceptRanges        = 21,
    Age                 = 22,
    ETag                = 23,
    Location            = 24,
    ProxyAuthenticate   = 25,
    RetryAfter          = 26,
    Server              = 27,
    SetCookie           = 28,
    Vary                = 29,
    WwwAuthenticate     = 30,
    AccessControlAllowOrigin = 31,
    AcceptPatch         = 32,
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Extensions of the http response header.
  /// </summary>
  public static class HttpResponseHeaderExtensions {
    
    /// <summary>
    /// Get a string representation of the response header.
    /// </summary>
    public static string GetString(this HttpResponseHeader key) {
      return _map[key];
    }
    
    /// <summary>
    /// Inner map of http response headers to their string representations.
    /// </summary>
    private static Dictionary<HttpResponseHeader, string> _map;
    
    static HttpResponseHeaderExtensions() {
      _map = BuildMap();
    }
    
    /// <summary>
    /// Inner method used to build the map of style keys.
    /// </summary>
    private static Dictionary<HttpResponseHeader, string> BuildMap() {
      
      var map = new Dictionary<HttpResponseHeader, string>();
      var builder = StringBuilderCache.Get();
      foreach(var value in (HttpResponseHeader[])Enum.GetValues(typeof(HttpResponseHeader))) {
        builder.Length = 0;
        
        bool first = true;
        foreach(var c in value.ToString()) {
          if(Char.IsUpper(c)) {
            if(first) first = false;
            else builder.Append(Chars.Dash);
            builder.Append(c);
          } else {
            builder.Append(c);
          }
        }
        
        map.Add(value, builder.ToString());
      }
      
      StringBuilderCache.Set(builder);
      
      return map;
    }
    
    
    
  }
  
  
}
