/*
 * User: Joshua
 * Date: 9/08/2016
 * Time: 12:12 AM
 */
using System;

namespace Efz {
  
  /// <summary>
  /// Caches byte buffers.
  /// </summary>
  public static class BufferCache {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// The current cached instance.
    /// </summary>
    [ThreadStatic]
    private static byte[] CachedInstance;
    
    //-------------------------------------------//
    
    static BufferCache() {
    }
    
    /// <summary>
    /// Get a buffer of the default size.
    /// </summary>
    public static byte[] Get() {
      if (CachedInstance != null && CachedInstance.Length >= Global.BufferSizeLocal) {
        byte[] cachedInstance = CachedInstance;
        CachedInstance = null;
        return cachedInstance;
      }
      return new byte[Global.BufferSizeLocal];
    }
    
    /// <summary>
    /// Get a buffer of specified size.
    /// </summary>
    public static byte[] Get(int capacity) {
      if (CachedInstance != null && capacity <= CachedInstance.Length) {
        byte[] cachedInstance = CachedInstance;
        CachedInstance = null;
        return cachedInstance;
      }
      return new byte[capacity];
    }
    
    /// <summary>
    /// Relinquish the buffer to the cache.
    /// </summary>
    public static void Set(byte[] buffer) {
      if (buffer.Length <= Global.BufferSizeLocal) BufferCache.CachedInstance = buffer;
    }
    
    //-------------------------------------------//
    
  }
  
}
