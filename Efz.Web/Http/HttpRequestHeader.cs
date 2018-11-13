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
  public enum HttpRequestHeader {
    
    //----------------------------------//
    
    Unknown               = 0,
    CacheControl          = 1,
    Connection            = 2,
    Date                  = 3,
    KeepAlive             = 4,
    Pragma                = 5,
    Trailer               = 6,
    TransferEncoding      = 7,
    Upgrade               = 8,
    Via                   = 9,
    Warning               = 10,
    Allow                 = 11,
    ContentLength         = 12,
    ContentType           = 13,
    ContentEncoding       = 14,
    ContentLanguage       = 15,
    ContentLocation       = 16,
    ContentMd5            = 17,
    ContentRange          = 18,
    Expires               = 19,
    LastModified          = 20,
    Accept                = 21,
    AcceptCharset         = 22,
    AcceptEncoding        = 23,
    AcceptLanguage        = 24,
    Authorization         = 25,
    Cookie                = 26,
    Expect                = 27,
    From                  = 28,
    Host                  = 29,
    IfMatch               = 30,
    IfModifiedSince       = 31,
    IfNoneMatch           = 32,
    IfRange               = 33,
    IfUnmodifiedSince     = 34,
    MaxForwards           = 35,
    ProxyAuthorization    = 36,
    Referer               = 37,
    Range                 = 38,
    Te                    = 39,
    Translate             = 40,
    UserAgent             = 41,
    HttpVersion           = 42,
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Extensions of the http response header.
  /// </summary>
  public static class HttpRequestHeaderExtensions {
    
    /// <summary>
    /// Get the http request header key represented by the specified string.
    /// </summary>
    public static HttpRequestHeader GetKey(string key) {
      HttpRequestHeader headerKey;
      return Map.Value.TryGetValue(key, out headerKey) ? headerKey : HttpRequestHeader.Unknown;
    }
    
    public static Lazy<Dictionary<string, HttpRequestHeader>> Map = new Lazy<Dictionary<string, HttpRequestHeader>>(BuildMap);
    
    /// <summary>
    /// Inner method used to build the map of style keys.
    /// </summary>
    private static Dictionary<string, HttpRequestHeader> BuildMap() {
      
      var map = new Dictionary<string, HttpRequestHeader>();
      var builder = StringBuilderCache.Get();
      
      foreach(var value in (HttpRequestHeader[])Enum.GetValues(typeof(HttpRequestHeader))) {
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
        
        map.Add(builder.ToString(), value);
      }
      
      StringBuilderCache.Set(builder);
      
      return map;
    }
    
  }
  
}
