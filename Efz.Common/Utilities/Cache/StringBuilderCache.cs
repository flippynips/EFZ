/*
 * User: Joshua
 * Date: 9/08/2016
 * Time: 12:12 AM
 */
using System;
using System.Text;

namespace Efz {
  
  /// <summary>
  /// Caches string builders.
  /// </summary>
  public static class StringBuilderCache {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// The max builder to cache.
    /// </summary>
    private const int _maxBuilderSize = 301;
    
    /// <summary>
    /// The Current cached instance.
    /// </summary>
    [ThreadStatic]
    private static StringBuilder CachedInstance;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Get a builder of specified size.
    /// </summary>
    public static StringBuilder Get(int capacity = 32) {
      if (capacity < _maxBuilderSize) {
        StringBuilder cachedInstance = CachedInstance;
        if (cachedInstance != null && capacity <= cachedInstance.Capacity) {
          CachedInstance = null;
          cachedInstance.Clear();
          return cachedInstance;
        }
      }
      return new StringBuilder(capacity);
    }
    
    /// <summary>
    /// Relinquish the string builder to the cache.
    /// </summary>
    public static void Set(StringBuilder sb) {
      if (sb.Capacity < _maxBuilderSize) StringBuilderCache.CachedInstance = sb;
    }
    
    /// <summary>
    /// Release the string builder and retrieve the value of the string in the cache.
    /// </summary>
    public static string SetAndGet(StringBuilder sb) {
      string value = sb.ToString();
      if (sb.Capacity < _maxBuilderSize) StringBuilderCache.CachedInstance = sb;
      return value;
    }
    
    /// <summary>
    /// Release the string builder and retrieve the value of the string in the cache.
    /// </summary>
    public static string SetAndGet(StringBuilder sb, int startIndex, int length) {
      string value = sb.ToString(startIndex, length);
      StringBuilderCache.Set(sb);
      return value;
    }
    
    //-------------------------------------------//
    
  }
  
}
