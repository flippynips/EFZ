/*
 * User: Joshua
 * Date: 18/10/2016
 * Time: 11:11 PM
 */
using System;
using System.Text;

namespace Efz.Logs {
  
  /// <summary>
  /// Caches string builders.
  /// </summary>
  public static class StringBuilderCache {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// The max builder to cache.
    /// </summary>
    private const int _builderLength = 301;
    
    /// <summary>
    /// Cached string builder.
    /// </summary>
    [ThreadStatic]
    private static StringBuilder CachedInstance;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Get a builder of specified size.
    /// </summary>
    public static StringBuilder Get() {
      if(CachedInstance != null) {
        StringBuilder cachedInstance = CachedInstance;
        CachedInstance = null;
        cachedInstance.Length = 0;
        return cachedInstance;
      }
      return new StringBuilder(_builderLength);
    }
    
    /// <summary>
    /// Relinquish the string builder to the cache.
    /// </summary>
    public static void Set(StringBuilder sb) {
      if (sb.Capacity < _builderLength) CachedInstance = sb;
    }
    
    /// <summary>
    /// Release the string builder and retrieve the value of the string in the cache.
    /// </summary>
    public static string SetAndGet(StringBuilder sb) {
      string value = sb.ToString();
      StringBuilderCache.Set(sb);
      return value;
    }
    
    //-------------------------------------------//
    
  }
  
}
