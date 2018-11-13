/*
 * User: FloppyNipples
 * Date: 29/04/2018
 * Time: 11:12 AM
 */
using System;
using System.Collections.Generic;
using Efz.Collections;
using Efz.Threading;

namespace Efz.Data {
  
  /// <summary>
  /// Threadsafe, rotating collection of items with a maximum size.
  /// </summary>
  public class CacheValue<TValue> : IEnumerable<TValue>, IDisposable {
    
    //----------------------------------//
    
    /// <summary>
    /// Maximum size of the cache.
    /// </summary>
    public long MaxCount;
    /// <summary>
    /// Get the current number of items in the cache.
    /// </summary>
    public long Count {
      get { return _lookup.Count; }
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Rotating queue.
    /// </summary>
    protected ArrayQueue<TValue> _queue;
    /// <summary>
    /// Fast lookup of items.
    /// </summary>
    protected Dictionary<TValue, int> _lookup;
    
    /// <summary>
    /// Lock used for any cache changes.
    /// </summary>
    protected Lock _lock;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a cache with the specified max number of items.
    /// </summary>
    public CacheValue(long maxCount = 1024) {
      MaxCount = maxCount;
      _queue = new ArrayQueue<TValue>();
      _lookup = new Dictionary<TValue, int>();
      
      _lock = new Lock();
    }
    
    /// <summary>
    /// Dispose of the cache instance.
    /// </summary>
    public void Dispose() {
      _queue.Dispose();
      _queue = null;
      _lookup = null;
    }
    
    /// <summary>
    /// Add the the specified item to the cache. Returns 'true' if
    /// an item of the same value was not found.
    /// </summary>
    public bool Add(TValue item) {
      _lock.Take();
      
      // does the item already exist in the cache?
      int count;
      if(_lookup.TryGetValue(item, out count)) {
        
        // yes, add the item to the end of the queue
        _queue.Enqueue(item);
        // increment the number of duplicate items in the lookup
        _lookup[item] = count + 1;
        
        _lock.Release();
        
        return false;
        
      } else {
        
        // add the item to the current queue
        _queue.Enqueue(item);
        // add the item to the lookup
        _lookup.Add(item, 1);
        
        // has the max number of items been reached?
        if(_lookup.Count > MaxCount) {
          // yes, dequeue an item
          _queue.Next();
          
          // remove it from the lookup if not already removed
          if(_lookup.TryGetValue(_queue.Current, out count)) {
            if(count == 1) {
              _lookup.Remove(_queue.Current);
            } else {
              _lookup[_queue.Current] = count - 1;
            }
          }
        }
        
        _lock.Release();
        
        return true;
        
      }
      
      
    }
    
    /// <summary>
    /// Remove an item from the cache.
    /// </summary>
    public void Remove(TValue item) {
      // remove from the lookup, queue will ignore upon removal
      if(_lookup.ContainsKey(item)) _lookup.Remove(item);
    }
    
    /// <summary>
    /// Check whether the cache contains the specified key.
    /// </summary>
    public bool Contains(TValue value) {
      return _lookup.ContainsKey(value);
    }
    
    /// <summary>
    /// Get the value associated with the specified key.
    /// Returns 'default(TValue)' if the key isn't found.
    /// </summary>
    public TValue Get(TValue value) {
      int count;
      if(_lookup.TryGetValue(value, out count)) return value;
      return default(TValue);
    }
    
    public IEnumerator<TValue> GetEnumerator() {
      return _lookup.Keys.GetEnumerator();
    }
    
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }
    
    //----------------------------------//
    
  }
}
