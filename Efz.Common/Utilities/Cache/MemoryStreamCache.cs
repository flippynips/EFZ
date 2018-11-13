/*
 * User: FloppyNipples
 * Date: 18/02/2018
 * Time: 11:27 AM
 */
using System;
using System.IO;

namespace Efz {
  
  /// <summary>
  /// Description of MemoryStreamCache.
  /// </summary>
  public static class MemoryStreamCache {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// The max memory stream size to cache.
    /// </summary>
    private const int _maxStreamSize = 8096;
    
    /// <summary>
    /// The Current cached instance.
    /// </summary>
    [ThreadStatic]
    private static MemoryStream CachedInstance;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Get an extendable memory stream of specified size.
    /// </summary>
    public static MemoryStream Get(int capacity = 8096) {
      if (capacity < _maxStreamSize) {
        MemoryStream cachedInstance = CachedInstance;
        if (cachedInstance != null && capacity <= cachedInstance.Capacity) {
          CachedInstance = null;
          cachedInstance.Capacity = capacity;
          return cachedInstance;
        }
      }
      return new MemoryStream();
    }
    
    /// <summary>
    /// Relinquish the extendable memory stream to the cache.
    /// </summary>
    public static void Set(MemoryStream stream) {
      if (stream.Capacity < _maxStreamSize) CachedInstance = stream;
    }
    
    /// <summary>
    /// Release the extendable memory stream and retrieve an array representation.
    /// </summary>
    public static byte[] SetAndGet(MemoryStream stream) {
      var value = stream.ToArray();
      if (stream.Capacity < _maxStreamSize) CachedInstance = stream;
      return value;
    }
    
    //-------------------------------------------//
    
  }
  
}
