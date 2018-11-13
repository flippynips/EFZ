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
  /// Threadsafe, rotating collection of items with a maximum size.
  /// </summary>
  public class Cache<TKey, TValue> : IEnumerable<TValue>, IDisposable {
    
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
    /// Get the current size of the cache.
    /// </summary>
    public long Size { get { return _size; } }
    
    /// <summary>
    /// Callback on an item being removed from the cache.
    /// </summary>
    public IAction<TValue> OnRemoved {
      get { return _onRemoved.Action; }
      set { _onRemoved.Action = value; }
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Rotating queue.
    /// </summary>
    protected ArrayQueue<TKey> _queue;
    /// <summary>
    /// Fast lookup of items.
    /// </summary>
    protected Dictionary<TKey, Teple<long, long, TValue>> _lookup;
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
    protected ActionRoll<TValue> _onRemoved;
    
    /// <summary>
    /// Has the cache been peeked at?
    /// </summary>
    protected bool _peeked;
    
    /// <summary>
    /// Lock used for any cache changes.
    /// </summary>
    protected Lock _lock;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a cache with the specified max culmunative item size.
    /// </summary>
    public Cache(long maxSize = 1000, Func<TValue, long> getItemSize = null) {
      MaxSize = maxSize;
      MaxItemSize = MaxSize / 1000;
      if(MaxItemSize < 1) MaxItemSize = 1;
      _queue = new ArrayQueue<TKey>();
      _lookup = new Dictionary<TKey, Teple<long, long, TValue>>();
      
      _getItemSize = getItemSize;
      
      _onRemoved = new ActionRoll<TValue>();
      
      _lock = new Lock();
    }
    
    /// <summary>
    /// Dispose of the cache instance.
    /// Removes all existing cache entries and runs OnRemove callback.
    /// </summary>
    public void Dispose() {
      
      _lock.Take();
      try {
        if(_queue == null) return;

        _queue.Dispose();
        _queue = null;

        if(_onRemoved.Action != null) {
          foreach(var entry in _lookup) _onRemoved.Run(entry.Value.ArgC);
        }
      } finally {
        _lock.Release();
      }
      
    }
    
    /// <summary>
    /// Peek at the top item from the cache. Will not return the same item
    /// twice.
    /// </summary>
    public TValue Peek() {
      
      _lock.Take();
      
      // has the cache been peeked at?
      if(_peeked) {
        _lock.Release();
        return default(TValue);
      }
      
      TValue item;
      if(_queue.Peek()) {
        _peeked = true;
        
        // get the item record
        var value = _lookup[_queue.Current];
        if(value.ArgA == 0) {
          _queue.Next();
          while(value.ArgA == 0) {
            if(!_queue.Next()) {
              _peeked = false;
              _lock.Release();
              return default(TValue);
            }
            value = _lookup[_queue.Current];
          }
        }
        
        item = value.ArgC;
        
        _lock.Release();
        
      } else {
        _lock.Release();
        return default(TValue);
      }
      
      return item;
    }
    
    /// <summary>
    /// Pop an item from the cache. If the item has a single entry within the cache
    /// the size of the cache will be reduced.
    /// </summary>
    public void Pop() {
      
      _lock.Take();
      // move to the next queued item
      if(!_queue.Next()) {
        _lock.Release();
        return;
      }
      
      // get the lookup record
      Teple<long, long, TValue> current = _lookup[_queue.Current];
      
      while(current.ArgA == 0) {
        if(!_queue.Next()) {
          _lock.Release();
          return;
        }
        current = _lookup[_queue.Current];
      }
      
      _peeked = false;
      
      // are there duplicate entries for the current item
      if(current.ArgA == 1) {
        
        // no, decrement the size of the current item
        _size -= current.ArgB;
        // remove the lookup entry
        _lookup.Remove(_queue.Current);
        
        _lock.Release();
        
        // is the callback set?
        if(_onRemoved.Action != null) {
          // yes, run it
          _onRemoved.Run(current.ArgC);
        }
        
      } else {
        
        // yes, decrement the number of cache entries
        --current.ArgA;
        
        _lock.Release();
        
      }
    }
    
    /// <summary>
    /// Add the the specified key and value to the cache. Returns
    /// false if the key already exists in the collection.
    /// </summary>
    public bool Add(TKey key, TValue item) {
      _lock.Take();
      
      // get the item size
      long itemSize = _getItemSize == null ? 1L : _getItemSize(item);
      
      // does the item already exist in the cache?
      Teple<long, long, TValue> cached;
      if(_lookup.TryGetValue(key, out cached) && cached.ArgA > 0) {
        
        // yes, add the item to the end of the queue
        _queue.Enqueue(key);
        
        // increment the number of duplicate items in the lookup
        ++cached.ArgA;
        // update the reference
        cached.ArgC = item;
        
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
      _lookup[key] = new Teple<long, long, TValue>(1L, itemSize, item);
      
      // has the cache overflowed?
      if(_size > MaxSize) {
        
        // while the size has overflowed, iterate
        while(_size > MaxSize) {
          
          // move to the next queued item
          _queue.Next();
          _peeked = false;
          
          // get the lookup record
          Teple<long, long, TValue> current = _lookup[_queue.Current];
          
          // are there duplicate entries for the current item
          if(current.ArgA == 1) {
            
            // no, decrement the size of the current item
            _size -= current.ArgB;
            // remove the lookup entry
            _lookup.Remove(_queue.Current);
            
            // is the callback set?
            if(_onRemoved.Action != null) {
              // yes, run it
              _onRemoved.Run(current.ArgC);
            }
            
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
      Teple<long, long, TValue> cached;
      if(_lookup.TryGetValue(key, out cached) && cached.ArgA > 0) {
        
        // yes, add the item to the end of the queue
        _queue.Enqueue(key);
        // increment the number of duplicate items in the lookup
        ++cached.ArgA;
        
        _lock.Release();
        return cached.ArgC;
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
      _lookup[key] = new Teple<long, long, TValue>(1L, itemSize, item);
      
      // has the cache overflowed?
      if(_size > MaxSize) {
        
        // while the size has overflowed, iterate
        while(_size > MaxSize) {
          
          // move to the next queued item
          _queue.Next();
          _peeked = false;
          
          // get the lookup record
          Teple<long, long, TValue> current = _lookup[_queue.Current];
          
          if(current.ArgA == 0) continue;
          
          // are there duplicate entries for the current item
          if(current.ArgA == 1) {
            
            // no, decrement the size of the current item
            _size -= current.ArgB;
            // remove the lookup entry
            _lookup.Remove(_queue.Current);
            
            // is the callback set?
            if(_onRemoved.Action != null) {
              // yes, run it
              _onRemoved.Run(current.ArgC);
            }
            
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
      _lock.Take();
      Teple<long, long, TValue> value;
      var contains = _lookup.TryGetValue(key, out value) && value.ArgA > 0;
      _lock.Release();
      return contains;
    }
    
    /// <summary>
    /// Get the value associated with the specified key.
    /// Returns 'default(TValue)' if the key isn't found.
    /// </summary>
    public TValue Get(TKey key) {
      Teple<long, long, TValue> value;
      _lock.Take();
      if(_lookup.TryGetValue(key, out value)) {
        _lock.Release();
        return value.ArgC;
      }
      _lock.Release();
      return default(TValue);
    }
    
    /// <summary>
    /// Remove an entry from the cache by key. The callback isn't called.
    /// </summary>
    public void Remove(TKey key) {
      Teple<long, long, TValue> value;
      _lock.Take();
      if(_lookup.TryGetValue(key, out value)) {
        _size -= value.ArgB;
        value.ArgC = default(TValue);
        value.ArgA = 0;
      }
      _lock.Release();
    }
    
    /// <summary>
    /// Remove an entry by key and return the value if specified. The callback isn't called.
    /// </summary>
    public TValue RemoveGet(TKey key) {
      Teple<long, long, TValue> value;
      _lock.Take();
      if(_lookup.TryGetValue(key, out value)) {
        _size -= value.ArgB;
        value.ArgC = default(TValue);
        value.ArgA = 0;
        _lock.Release();
        return value.ArgC;
      }
      _lock.Release();
      return default(TValue);
    }
    
    /// <summary>
    /// Get a copy of the current collection of values in the cache.
    /// </summary>
    public ArrayRig<TValue> GetValues() {
      ArrayRig<TValue> collection = new ArrayRig<TValue>();
      _lock.Take();
      foreach(var entry in _lookup) collection.Add(entry.Value.ArgC);
      _lock.Release();
      return collection;
    }
    
    /// <summary>
    /// Get an enumerator for the cache.
    /// </summary>
    public IEnumerator<TValue> GetEnumerator() {
      return new Enumerator(this);
    }
    
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      throw new NotImplementedException();
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Enumerator for cache values.
    /// </summary>
    public class Enumerator : IEnumerator<TValue> {
      
      //--------------------------------//
      
      /// <summary>
      /// Current object value.
      /// </summary>
      object System.Collections.IEnumerator.Current {
        get { return _enumerator.Current.Value.ArgC; }
      }
      
      /// <summary>
      /// Current value.
      /// </summary>
      public TValue Current {
        get { return _enumerator.Current.Value.ArgC; }
      }
      
      //--------------------------------//
      
      /// <summary>
      /// Cache that is being enumerated.
      /// </summary>
      private Cache<TKey, TValue> _cache;
      /// <summary>
      /// Enumerator for the cache.
      /// </summary>
      private Dictionary<TKey, Teple<long, long, TValue>>.Enumerator _enumerator;
      
      //--------------------------------//
      
      public Enumerator(Cache<TKey, TValue> cache) {
        _cache = cache;
        _enumerator = _cache._lookup.GetEnumerator();
      }
      
      public bool MoveNext() {
        return _enumerator.MoveNext();
      }
    
      public void Reset() {
        _enumerator.Dispose();
        _enumerator = _cache._lookup.GetEnumerator();
      }
      
      public void Dispose() {
        _enumerator.Dispose();
      }
      
      //--------------------------------//
      
    }
    
  }
  
}
