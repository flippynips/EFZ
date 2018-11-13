/*
 * User: FloppyNipples
 * Date: 24/02/2018
 * Time: 6:00 PM
 */
using System;
using System.Collections.Generic;

namespace Efz.Collections {
  
  /// <summary>
  /// Description of IGetKey.
  /// </summary>
  public interface IFlowRigValue {
    
    //---------------------------------------//
    
    /// <summary>
    /// Get the key of the item.
    /// </summary>
    double Key { get; }
    
    //---------------------------------------//
    
  }
  
  /// <summary>
  /// A collection that represents a dynamic sequential segment of another collection. Items of a higher
  /// key are prioritized over those with a lower key. A maxiumum number of items are maintained.
  /// Each items hash set is used to ensure unique values.
  /// </summary>
  public class FlowRig<TValue> where TValue : IFlowRigValue {
    
    //---------------------------------------//
    
    public int Count {
      get { return Collection.Count; }
    }
    
    /// <summary>
    /// The maximum number of items to keep in the collection.
    /// Refresh() to cause the segment to adjust immediately.
    /// </summary>
    public int MaxItems;
    /// <summary>
    /// Target key to keep. Keys closer to this will be kept over those further.
    /// </summary>
    public double TargetKey = double.MaxValue;
    
    /// <summary>
    /// Optional action called on an item being removed from the collection.
    /// </summary>
    public Action<TValue> OnRemove;
    
    /// <summary>
    /// Collection of values.
    /// </summary>
    public HashSet<TValue> Collection;
    
    //---------------------------------------//
    
    /// <summary>
    /// Current sorted collection of items.
    /// </summary>
    protected SortedRig<double, TValue> _rig;
    /// <summary>
    /// Flag indicating whether the collection has been added to or removed
    /// from since the last add.
    /// </summary>
    protected bool _changed;
    
    //---------------------------------------//
    
    /// <summary>
    /// Create a new map segment.
    /// </summary>
    public FlowRig(double target, int maxItems = 100, IEqualityComparer<TValue> equalityComparer = null) {
      
      TargetKey = target;
      MaxItems = maxItems;
      _rig = new SortedRig<double, TValue>(SortedRig<double, TValue>.AverageDouble);
      Collection = equalityComparer == null ? new HashSet<TValue>() : new HashSet<TValue>(equalityComparer);
      
    }
    
    /// <summary>
    /// Get the current values as an array.
    /// </summary>
    public TValue[] ToArray() {
      
      var result = new TValue[_rig.Count];
      Array.Copy(_rig.Array, result, _rig.Count);
      return result;
      
    }
    
    /// <summary>
    /// Try add the specified key and value to the collection. Returns
    /// whether the value was added or exists already within the rig.
    /// </summary>
    public bool TryAdd(TValue value) {
      
      if(Collection.Contains(value)) return true;
      
      if(Collection.Count <= MaxItems) {
        Collection.Add(value);
        _changed = true;
        return true;
      }
      
      if(_changed) {
        
        _rig.Clear();
        foreach(TValue item in Collection) {
          _rig.Add(item.Key, item);
        }
        
        _changed = false;
        
      }
      
      double key = value.Key;
      double difference = Math.Abs(TargetKey - key);
      double lowerDifference;
      double higherDifference;
      
      bool shouldAdd = true;
      
      while(Collection.Count > MaxItems) {
        
        Struct<double, TValue> lowest = _rig.Array[0];
        Struct<double, TValue> highest = _rig.Array[_rig.Count-1];
        lowerDifference = Math.Abs(TargetKey - lowest.ArgA);
        higherDifference = Math.Abs(TargetKey - highest.ArgA);
        
        if(shouldAdd) {
          shouldAdd &= difference <= lowerDifference && difference <= higherDifference;
        }
        
        if(lowerDifference > higherDifference) {
          Collection.Remove(lowest.ArgB);
          _rig.RemoveAt(0);
          if(OnRemove != null) OnRemove(lowest.ArgB);
        } else {
          Collection.Remove(highest.ArgB);
          _rig.RemoveAt(_rig.Count-1);
          if(OnRemove != null) OnRemove(highest.ArgB);
        }
        
      }
      
      if(shouldAdd) {
        Collection.Add(value);
        _rig.Add(key, value);
        return true;
      }
      
      return false;
      
    }
    
    /// <summary>
    /// Add the specified value to the collection regardless of max item limit.
    /// </summary>
    public void Add(TValue value) {
      
      if(Collection.Contains(value)) return;
      
      Collection.Add(value);
      _changed = true;
      
    }
    
    /// <summary>
    /// Remove the specified key from the current collection.
    /// Doesn't call 'OnRemove'.
    /// </summary>
    public void Remove(TValue value) {
      
      _changed = true;
      Collection.Remove(value);
      
    }
    
    /// <summary>
    /// Checks for the existance of the specified key. In the collection.
    /// </summary>
    public bool Contains(TValue value) {
      return Collection.Contains(value);
    }
    
    /// <summary>
    /// Gets the closest value to the specified key.
    /// </summary>
    public TValue Closest(double key) {
      if(_changed) {
        
        _rig.Clear();
        foreach(TValue item in Collection) {
          _rig.Add(item.Key, item);
        }
        
        _changed = false;
        
      }
      
      return _rig.ValueClosest(key);
    }
    
    /// <summary>
    /// Clear the content of the map view.
    /// </summary>
    public void Clear() {
      Collection.Clear();
      _changed = true;
    }
    
    //---------------------------------------//
    
  }
  
}
