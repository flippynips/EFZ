using System;
using System.Collections.Generic;
using System.Collections;

using Efz.Threading;

namespace Efz.Collections {
  
  /// <summary>
  /// Structure containing some accurate threadsafe handling of a collection.
  /// Fast if removal is sparse and iteration is often.
  /// Order of items is not maintained.
  /// </summary>
  public class Capsule<T> : IEnumerable<T> where T : class, IEquatable<T> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Number of items in the collection.
    /// </summary>
    public int Count { get { return _rig.Count; } }
    
    //-------------------------------------------//
    
    private HashSet<T> _collection;
    private HashSet<T> _toRemove;
    private HashSet<T> _toAdd;
    
    private Lock _collectionLock;
    private Lock _toRemoveLock;
    private Lock _toAddLock;
    
    private bool _changes;
    private ArrayRig<T> _rig;
    
    //-------------------------------------------//
    
    public Capsule(IEqualityComparer<T> comparer) {
      _rig        = new ArrayRig<T>();
      _collection = new HashSet<T>(comparer);
      _toAdd      = new HashSet<T>(comparer);
      _toRemove   = new HashSet<T>(comparer);
      _collectionLock = new Lock();
      _toRemoveLock = new Lock();
      _toAddLock = new Lock();
    }
    
    public Capsule() {
      _rig        = new ArrayRig<T>();
      _collection = new HashSet<T>();
      _toAdd      = new HashSet<T>();
      _toRemove   = new HashSet<T>();
      _collectionLock = new Lock();
      _toRemoveLock = new Lock();
      _toAddLock = new Lock();
    }
    
    /// <summary>
    /// Clear all items from the capsule.
    /// </summary>
    public void Clear() {
      _toAddLock.Take();
      _toAdd.Clear();
      _toAddLock.Release();
      _toRemoveLock.Take();
      _toRemove.Clear();
      _toRemoveLock.Release();
      _collectionLock.Take();
      _collection.Clear();
      _rig.Reset();
      _collectionLock.Release();
    }
    
    /// <summary>
    /// Add an item to the capsule.
    /// </summary>
    public void Add(T item) {
      _toAddLock.Take();
      _toAdd.Add(item);
      _toAddLock.Release();
      _changes = true;
    }
    
    /// <summary>
    /// Remove the specified item from the capsule.
    /// </summary>
    public void Remove(T item) {
      _toRemoveLock.Take();
      _toRemove.Add(item);
      _toRemoveLock.Release();
      _changes = true;
    }
    
    /// <summary>
    /// Returns an existing item that matches the item specified within the
    /// capsule. If the item doesn't exist, this returns false.
    /// </summary>
    public bool Get(T item, out T result) {
      
      // make changes if necessary
      if(_changes) {
        _changes = false;
        
        // Add items to collection.
        _toAddLock.Take();
        foreach(T addition in _toAdd) {
          _collectionLock.Take();
          _collection.Add(addition);
          _collectionLock.Release();
          _rig.Add(addition);
        }
        _toAdd.Clear();
        _toAddLock.Release();
        
        // Remove items from collection.
        _toRemoveLock.Take();
        foreach(T removal in _toRemove) {
          _collectionLock.Take();
          _collection.Remove(removal);
          _collectionLock.Release();
          _rig.RemoveQuick(removal);
        }
        _toRemove.Clear();
        _toRemoveLock.Release();
      }
      
      _collectionLock.Take();
      foreach(T it in _collection) {
        if(it.Equals(item)) {
          _collectionLock.Release();
          result = it;
          return true;
        }
      }
      _collectionLock.Release();
      result = default(T);
      return false;
    }
    
    /// <summary>
    /// Checks if the capsule contains the specified item.
    /// </summary>
    public bool Contains(T item) {
      
      if(_changes) {
        _changes = false;
        
        // Add items to collection.
        _toAddLock.Take();
        foreach(T addition in _toAdd) {
          _collectionLock.Take();
          _collection.Add(addition);
          _collectionLock.Release();
          _rig.Add(addition);
        }
        _toAdd.Clear();
        _toAddLock.Release();
        
        // Remove items from collection.
        _toRemoveLock.Take();
        foreach(T removal in _toRemove) {
          _collectionLock.Take();
          _collection.Remove(removal);
          _collectionLock.Release();
          _rig.RemoveQuick(removal);
        }
        _toRemove.Clear();
        _toRemoveLock.Release();
      }
      
      _collectionLock.Take();
      if(_collection.Contains(item)) {
        _collectionLock.Release();
        return true;
      }
      _collectionLock.Release();
      
      return false;
    }
    
    /// <summary>
    /// Returns a match of the specified item that exists in the capsule. Or
    /// adds the item to the capsule and returns it.
    /// </summary>
    public T Match(T item) {
      
      if(_changes) {
        _changes = false;
        
        // Add items to collection.
        _toAddLock.Take();
        foreach(T addition in _toAdd) {
          _collectionLock.Take();
          _collection.Add(addition);
          _collectionLock.Release();
          _rig.Add(addition);
        }
        _toAdd.Clear();
        _toAddLock.Release();
        
        // Remove items from collection.
        _toRemoveLock.Take();
        foreach(T removal in _toRemove) {
          _collectionLock.Take();
          _collection.Remove(removal);
          _collectionLock.Release();
          _rig.RemoveQuick(removal);
        }
        _toRemove.Clear();
        _toRemoveLock.Release();
      }
      
      _collectionLock.Take();
      foreach(T it in _collection) {
        if(it.Equals(item)) {
          _collectionLock.Release();
          return it;
        }
      }
      
      _collection.Add(item);
      _rig.Add(item);
      _collectionLock.Release();
      return item;
    }
    
    /// <summary>
    /// Return an array copy of the current collection.
    /// This is a slow operation.
    /// </summary>
    public T[] ToArray() {
      if(_changes) {
        _changes = false;
        // Add items to collection.
        _toAddLock.Take();
        foreach(T item in _toAdd) {
          _collectionLock.Take();
          _collection.Add(item);
          _rig.Add(item);
          _collectionLock.Release();
        }
        _toAdd.Clear();
        _toAddLock.Release();
        // Remove items from collection.
        _toRemoveLock.Take();
        foreach(T item in _toRemove) {
          _collectionLock.Take();
          _collection.Remove(item);
          _rig.RemoveQuick(item);
          _collectionLock.Release();
        }
        _toRemove.Clear();
        _toRemoveLock.Release();
      }
      _collectionLock.Take();
      T[] array = new T[_rig.Count];
      Array.Copy(_rig.Array, array, _rig.Count);
      _collectionLock.Release();
      return array;
    }
    
    /// <summary>
    /// Returns an enumerator. Note that this copies the entire collection.
    /// </summary>
    public IEnumerator<T> GetEnumerator() {
      
      if(_changes) {
        _changes = false;
        // Add items to collection.
        _toAddLock.Take();
        foreach(T item in _toAdd) {
          _collectionLock.Take();
          _collection.Add(item);
          _collectionLock.Release();
          _rig.Add(item);
        }
        _toAdd.Clear();
        _toAddLock.Release();
        // Remove items from collection.
        _toRemoveLock.Take();
        foreach(T item in _toRemove) {
          _collection.Remove(item);
          _rig.RemoveQuick(item);
        }
        _toRemove.Clear();
        _toRemoveLock.Release();
      }
      
      return new Enumerator(this);
    }
    
    //-------------------------------------------//
    
    IEnumerator IEnumerable.GetEnumerator() {
      return new Enumerator(this);
    }
    
    private class Enumerator : IEnumerator<T> {
      
      //-------------------------------------------//
      
      public T Current {
        get {
          return current;
        }
      }
      
      object IEnumerator.Current {
        get {
          return current;
        }
      }
      
      //-------------------------------------------//
      
      private T current;
      private int index;
      
      private ArrayRig<T> rig;
      private Capsule<T> capsule;
      
      //-------------------------------------------//
      
      public Enumerator(Capsule<T> _capsule) {
        capsule = _capsule;
        rig = _capsule._rig;
        index = -1;
      }
      
      public void Dispose() {
        // Nothing to dispose!
      }
      
      public bool MoveNext() {
        capsule._collectionLock.Take();
        if(++index < rig.Count) {
          current = rig[index];
          capsule._collectionLock.Release();
          return true;
        }
        capsule._collectionLock.Release();
        return false;
      }
      
      public void Reset() {
        index = -1;
      }
      
      //-------------------------------------------//
      
      
      
      
    }
    
  }

}