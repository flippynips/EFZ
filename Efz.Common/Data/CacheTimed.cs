/*
 * User: Joshua
 * Date: 1/11/2016
 * Time: 12:59 AM
 */
using System;
using System.Collections.Generic;

using Efz.Collections;
using Efz.Threading;
using Efz.Tools;

namespace Efz.Data {
  
  /// <summary>
  /// Threadsafe, rotating collection of items with a maximum size and time to live.
  /// </summary>
  public class CacheTimed<TKey, TValue> : IDisposable {
    
    //----------------------------------//
    
    /// <summary>
    /// Maximum size of the cache.
    /// </summary>
    public long MaxSize;
    /// <summary>
    /// Maximum size of a single item allowed in the cache. By default, this is half the total
    /// size of the cache.
    /// </summary>
    public long MaxItemSize;
    /// <summary>
    /// Default time each item has before being discarded on its next request.
    /// </summary>
    public long DefaultTimeToLive;
    
    /// <summary>
    /// Get the current size of the cache.
    /// </summary>
    public long Size { get { return _size; } }
    
    /// <summary>
    /// Callback on an item being removed from the cache.
    /// </summary>
    public IAction<TValue> OnRemoved {
      get {
        return _onRemoved.Action;
      }
      set {
        _onRemoved.Action = value;
      }
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Rotating queue.
    /// </summary>
    protected ArrayQueue<TKey> _queue;
    /// <summary>
    /// Fast lookup of items.
    /// </summary>
    protected Dictionary<TKey, Teple<long, long, long, TValue>> _lookup;
    /// <summary>
    /// Current total size of the cached items.
    /// </summary>
    protected long _size;
    /// <summary>
    /// Inner function used to retrieve the item cache size.
    /// </summary>
    protected Func<TValue, long> _getItemSize;
    
    /// <summary>
    /// Callback on an item being removed from the cache.
    /// </summary>
    protected ActionPop<TValue> _onRemoved;
    
    /// <summary>
    /// Lock used for any cache changes.
    /// </summary>
    protected Lock _lock;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a cache with the specified max culmunative item size.
    /// </summary>
    public CacheTimed(long defaultTimeToLive = 30000, long maxSize = 1000, Func<TValue, long> getItemSize = null) {
      DefaultTimeToLive = defaultTimeToLive;
      MaxSize = maxSize;
      MaxItemSize = MaxSize / 1000;
      if(MaxItemSize < 1) MaxItemSize = 1;
      _queue = new ArrayQueue<TKey>();
      _lookup = new Dictionary<TKey, Teple<long, long, long, TValue>>();
      
      _getItemSize = getItemSize;
      
      _lock = new Lock();
    }
    
    /// <summary>
    /// Dispose of the cache instance.
    /// </summary>
    public void Dispose() {
      _queue.Dispose();
      
      _lock.Take();
      
      foreach(var entry in _lookup) _onRemoved.Run(entry.Value.ArgD);
      
      _lookup = null;
      
      _lock.Release();
    }
    
    /// <summary>
    /// Pop an item from the cache. If the item has a single entry within the cache
    /// the size of the cache will be reduced.
    /// </summary>
    public void Pop() {
      
      // move to the next queued item
      if(!_queue.Next()) return;
      // get the lookup record
      var current = _lookup[_queue.Current];
      
      // are there duplicate entries for the current item
      if(current.ArgA == 1) {
        
        // no, decrement the size of the current item
        _size -= current.ArgB;
        
        // remove the lookup entry
        _lookup.Remove(_queue.Current);
        
        // run the callback
        _onRemoved.Run(current.ArgD);
        
      } else {
        
        // yes, decrement the number of cache entries
        --current.ArgA;
        
      }
    }
    
    /// <summary>
    /// Add or update the the specified key and value in the cache. Returns
    /// false if a reference to an existing entry was updated.
    /// </summary>
    public bool Set(TKey key, TValue item) {
      _lock.Take();
      
      // get the item size
      long itemSize = _getItemSize == null ? 1L : _getItemSize(item);
      
      // does the item already exist in the cache?
      Teple<long, long, long, TValue> cached;
      if(_lookup.TryGetValue(key, out cached)) {
        
        // yes, add the item to the end of the queue
        _queue.Enqueue(key);
        // increment the number of duplicate items in the lookup
        ++cached.ArgA;
        // update the reference
        cached.ArgD = item;
        // update the ttl
        cached.ArgC = Time.Milliseconds + DefaultTimeToLive;
        
        _lock.Release();
        return false;
      }
        
      // is the item too large for the cache?
      if(itemSize > MaxItemSize) {
        // yes, skip adding it
        _lock.Release();
        return true;
      }
      
      // add the item size to the current total
      _size += itemSize;
      
      // add the item to the current queue
      _queue.Enqueue(key);
      // add the item to the lookup
      _lookup.Add(key, new Teple<long, long, long, TValue>(1L, itemSize, Time.Milliseconds + DefaultTimeToLive, item));
      
      // has the cache overflowed?
      if(_size > MaxSize) {
        
        // while the size has overflowed, iterate
        while(_size > MaxSize) {
          
          // move to the next queued item
          _queue.Next();
          // get the lookup record
          var current = _lookup[_queue.Current];
          
          // are there duplicate entries for the current item
          if(current.ArgA == 1) {
            
            // no, decrement the size of the current item
            _size -= current.ArgB;
            // remove the lookup entry
            _lookup.Remove(_queue.Current);
            
            // run the callback
            _onRemoved.Run(current.ArgD);
            
          } else {
            
            // yes, decrement the number of cache entries
            --current.ArgA;
            
          }
          
        }
      }
      
      _lock.Release();
      
      return true;
    }
    
    /// <summary>
    /// Add the value or retrieve an existing value in a single threadsafe call.
    /// </summary>
    public TValue AddOrGet(TKey key, TValue item) {
      _lock.Take();
      
      // get the item size
      long itemSize = _getItemSize == null ? 1L : _getItemSize(item);
      
      // does the item already exist in the cache?
      Teple<long, long, long, TValue> cached;
      if(_lookup.TryGetValue(key, out cached)) {
        
        // yes, has the item expired?
        if(cached.ArgC > Time.Milliseconds) {
          
          // yes, remove the existing entry
          _lookup.Remove(key);
          
        } else {
          
          // no, add the item to the end of the queue
          _queue.Enqueue(key);
          
          // increment the number of duplicate items in the lookup
          ++cached.ArgA;
          
          _lock.Release();
          return cached.ArgD;
        }
      }
        
      // is the item too large for the cache?
      if(itemSize > MaxItemSize) {
        // yes, skip adding it
        _lock.Release();
        return item;
      }
      
      // add the item size to the current total
      _size += itemSize;
      
      // add the item to the current queue
      _queue.Enqueue(key);
      
      // add the item to the lookup
      _lookup.Add(key, new Teple<long, long, long, TValue>(1L, itemSize, Time.Milliseconds + DefaultTimeToLive, item));
      
      // has the cache overflowed?
      if(_size > MaxSize) {
        
        // while the size has overflowed, iterate
        while(_size > MaxSize) {
          
          // move to the next queued item
          _queue.Next();
          // get the lookup record
          var current = _lookup[_queue.Current];
          
          // are there duplicate entries for the current item
          if(current.ArgA == 1) {
            
            // no, decrement the size of the current item
            _size -= current.ArgB;
            // remove the lookup entry
            _lookup.Remove(_queue.Current);
            
            // run the callback
            _onRemoved.Run(current.ArgD);
            
          } else {
            
            // yes, decrement the number of cache entries
            --current.ArgA;
            
          }
          
        }
      }
      
      _lock.Release();
      
      return item;
    }
    
    /// <summary>
    /// Check whether the cache contains the specified key.
    /// </summary>
    public bool Contains(TKey key) {
      Teple<long, long, long, TValue> item;
      return _lookup.TryGetValue(key, out item) && item.ArgC < Time.Milliseconds;
    }
    
    /// <summary>
    /// Get the value associated with the specified key.
    /// Returns 'default(TValue)' if the key isn't found.
    /// </summary>
    public TValue Get(TKey key) {
      Teple<long, long, long, TValue> value;
      _lock.Take();
      if(_lookup.TryGetValue(key, out value)) {
        if(value.ArgC > Time.Milliseconds) {
          _lock.Release();
          return value.ArgD;
        }
        _lookup.Remove(key);
      }
      _lock.Release();
      return default(TValue);
    }
    
    /// <summary>
    /// Remove an entry from the cache by key. Note the callback isn't
    /// called.
    /// </summary>
    public void Remove(TKey key) {
      _lock.Take();
      _lookup.Remove(key);
      _lock.Release();
    }
    
    /// <summary>
    /// Get a copy of the current collection of values in the cache.
    /// </summary>
    public ArrayRig<TValue> GetValues() {
      ArrayRig<TValue> collection = new ArrayRig<TValue>();
      _lock.Take();
      foreach(var entry in _lookup) {
        if(entry.Value.ArgC > Time.Milliseconds) collection.Add(entry.Value.ArgD);
      }
      _lock.Release();
      return collection;
    }
    
    //----------------------------------//
    
  }
  
}
