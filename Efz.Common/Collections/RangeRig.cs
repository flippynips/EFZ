using System;

namespace Efz.Collections {
  
  /// <summary>
  /// Ranges of comparable keys automatically mapped to values.
  /// A maximum size of value ranges indicates when a new value is requested.
  /// </summary>
  public class RangeRig<TKey, TValue> : IDisposable where TKey : IComparable<TKey> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Sorted ranges by minimum key.
    /// </summary>
    public SortedRig<TKey, Range> Ranges;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new range map. Optionally with an initial capacity.
    /// </summary>
    public RangeRig(Func<TKey, TKey, TKey> getAverage, int capacity = 2) {
      // construct the sorted rig
      Ranges = new SortedRig<TKey, Range>(getAverage, capacity);
    }
    
    /// <summary>
    /// Initialize a RangeRig with a preconstructed collection of Ranges.
    /// </summary>
    public RangeRig(SortedRig<TKey, Range> ranges) {
      Ranges = ranges;
    }
    
    /// <summary>
    /// Dispose of the range maps resources.
    /// </summary>
    public void Dispose() {
      Ranges.Dispose();
    }
    
    /// <summary>
    /// Get the value of the range the specified key is closest to.
    /// </summary>
    public TValue this[TKey key] {
      get {
        // get the index closest to the key
        int index = Ranges.IndexClosest(key);
        if(index == -1) return default(TValue);
        
        // get the range at the index
        Struct<TKey, Range> range = Ranges.Array[index];
        int minCompare = key.CompareTo(range.ArgA);
        
        // if the key is less than the range min
        if(minCompare <= 0) {
          
          // if looking at the minimum range
          if(index == 0) {
            // move the current range minimum
            Ranges.Array[index] = new Struct<TKey, Range>(key, range.ArgB);
            // return the value
            return range.ArgB.Value;
          }
          
          Struct<TKey, Range> prev = Ranges.Array[index-1];
          int maxCompare = key.CompareTo(prev.ArgB.Max);
          
          // if the previous range contains the key
          if(maxCompare >= 0) {
            // return that ranges value
            return prev.ArgB.Value;
          }
          
          // if the previous range maximum is closer to the key
          if(-maxCompare < minCompare) {
            // move the previous range maximum
            Ranges.Array[index-1] = new Struct<TKey, Range>(prev.ArgA, new Range{
              Max = key,
              Value = prev.ArgB.Value
            });
            
            // return that ranges value
            return prev.ArgB.Value;
          }
          
          // move the current range minumum
          Ranges.Array[index] = new Struct<TKey, Range>(key, range.ArgB);
          
          // return the target range value
          return range.ArgB.Value;
          
        }
        
        // if the key is beyond the current range maximum
        if(key.CompareTo(range.ArgB.Max) >= 0) {
          
          // move the current range maximum
          Ranges.Array[index] = new Struct<TKey, Range>(range.ArgA, new Range{
            Max = key,
            Value = range.ArgB.Value
          });
          
        }
        
        // the key is within the current range
        return range.ArgB.Value;
      }
    }
    
    /// <summary>
    /// Get the value of the range the specified range is associated with.
    /// The method does not move ranges and will return default(TValue) if
    /// no range matches the key.
    /// </summary>
    public bool Get(TKey key, out TValue value) {
      
      // get the index closest to the key
      int index = Ranges.IndexClosest(key);
      if(index == -1) {
        value = default(TValue);
        return false;
      }
      
      // get the range at the index
      Struct<TKey, Range> range = Ranges.Array[index];
      int minCompare = key.CompareTo(range.ArgA);
      
      // if the key is greater than the range min
      if(minCompare > 0) {
        
        // if the key is beyond the current range maximum
        if(key.CompareTo(range.ArgB.Max) > 0) {
          value = default(TValue);
          return false;
        }
        
        // the key is within the current range
        value = range.ArgB.Value;
        return true;
      }
      
      // if looking at the minimum range
      if(index == 0) {
        value = default(TValue);
        return false;
      }
      
      Struct<TKey, Range> prev = Ranges.Array[index-1];
      int maxCompare = key.CompareTo(prev.ArgB.Max);
      
      // if the previous range contains the key
      if(maxCompare < 0) {
        // return the previous ranges value
        value = prev.ArgB.Value;
        return true;
      }
      
      value = default(TValue);
      return false;
    }
    
    /// <summary>
    /// Get the index of the range the specified range is associated with.
    /// The method does not move ranges and will return -1 if
    /// no range matches the key.
    /// </summary>
    public int IndexOf(TKey key) {
      
      // get the index closest to the key
      int index = Ranges.IndexClosest(key);
      if(index == -1) return index;
      
      // get the range at the index
      Struct<TKey, Range> range = Ranges.Array[index];
      int minCompare = key.CompareTo(range.ArgA);
      
      // if the key is greater than the range min
      if(minCompare > 0) {
        
        // if the key is beyond the current range maximum
        if(key.CompareTo(range.ArgB.Max) > 0) return -1;
        
        // the key is within the current range
        return index;
      }
      
      // if looking at the minimum range
      if(index == 0) return -1;
      
      Struct<TKey, Range> prev = Ranges.Array[index-1];
      int maxCompare = key.CompareTo(prev.ArgB.Max);
      
      // if the previous range contains the key, return the previous ranges value
      if(maxCompare < 0) return index;
      
      return -1;
    }
    
    /// <summary>
    /// Set the value for the range that includes the specified key.
    /// </summary>
    public void Set(TKey key, TValue value) {
      // get the index closest to the key
      int index = Ranges.IndexClosest(key);
      if(index == -1) {
        Ranges.Add(key, new Range {
          Max = key,
          Value = value
        });
        return;
      }
      
      // get the range at the index
      Struct<TKey, Range> range = Ranges.Array[index];
      int minCompare = key.CompareTo(range.ArgA);
      
      // if the key is less than the range min
      if(minCompare < 0) {
        
        // if looking at the minimum range
        if(index == 0) {
          // move the current range minimum
          Ranges.Array[index] = new Struct<TKey, Range>(key, new Range {
            Max = range.ArgB.Max,
            Value = value
          });
        } else {
          
          Struct<TKey, Range> prev = Ranges.Array[index-1];
          int maxCompare = key.CompareTo(prev.ArgB.Max);
          
          // if the previous range doesn't contain the key
          if(maxCompare > 0) {
            
            // if the previous range maximum is closest to the key
            if(-maxCompare < minCompare) {
              // move the previous range maximum
              Ranges.Array[index-1] = new Struct<TKey, Range>(prev.ArgA, new Range{
                Max = key,
                Value = value
              });
            } else {
              
              // move the current range minumum and set the value
              Ranges.Array[index] = new Struct<TKey, Range>(key, new Range {
                Max = range.ArgB.Max,
                Value = value
              });
            }
            
          } else {
            
            // set the previous range value
            Ranges.Array[index-1] = new Struct<TKey, Range>(prev.ArgA, new Range {
              Max = prev.ArgB.Max,
              Value = value
            });
            
          }
        }
      } else {
      
        // if the key is beyond the current range maximum
        if(key.CompareTo(range.ArgB.Max) > 0) {
          
          // move the current range maximum and set the value
          Ranges.Array[index] = new Struct<TKey, Range>(range.ArgA, new Range{
            Max = key,
            Value = value
          });
          
        } else {
          
          // set the current ranges value
          Ranges.Array[index] = new Struct<TKey, Range>(range.ArgA, new Range{
            Max = range.ArgB.Max,
            Value = value
          });
          
        }
      }
      
    }
    
    /// <summary>
    /// Add the specified value and key. If the specified key is the max or min of
    /// a range then the value is not set and this method will return false.
    /// </summary>
    public bool Add(TKey key, TValue value) {
      // ranges cannot contain the key
      int index = Ranges.IndexClosest(key);
      
      // check if either the min or max values match the key to be added
      if(index != -1 && (key.CompareTo(Ranges.Array[index].ArgA) == 0 || key.CompareTo(Ranges.Array[index].ArgB.Max) == 0)) {
        // the key could not be added
        return false;
      }
      
      // add a range for the key and value
      Ranges.Add(key, new Range {
        Max = key,
        Value = value,
      });
      
      return true;
    }
    
    /// <summary>
    /// Remove the range of the specified key. Returns true if a range was removed.
    /// </summary>
    public bool Remove(TKey key) {
      // ranges cannot contain the key
      int index = Ranges.IndexClosest(key);
      
      // check if either the min or max values match the key to be added
      if(index != -1 && (key.CompareTo(Ranges.Array[index].ArgA) == 0 || key.CompareTo(Ranges.Array[index].ArgB.Max) == 0)) {
        // the key could not be added
        return false;
      }
      
      // remove the range for the key
      Ranges.Remove(key);
      
      return true;
    }
    
    /// <summary>
    /// Add the specified value and for the range specified by the min and max
    /// values. If the specified range overlaps another then the value is not
    /// set and this method will return false.
    /// </summary>
    public bool Add(TKey min, TKey max, TValue value) {
      // ranges cannot contain the key
      int index = Ranges.IndexOf(min);
      
      // if ranges exist
      if(index != -1) {
        
        // check whether the closest range overlaps the target
        Struct<TKey, Range> range = Ranges.Array[index];
        
        // compare with the closest range minimum
        int minCompare = min.CompareTo(range.ArgA);
        int maxCompare = max.CompareTo(range.ArgA);
        
        // fail if either of the range edges overlap
        if(minCompare == 0 || maxCompare == 0) return false;
        
        // if the specified range contains the minimum of the closest range
        if(minCompare < 0 && maxCompare > 0) return false;
        
        // compare with the closest range maximum
        minCompare = min.CompareTo(range.ArgB.Max);
        maxCompare = max.CompareTo(range.ArgB.Max);
        
        // if the specified range contains the maximum of the closest range
        if(minCompare < 0 && maxCompare > 0) return false;
        
        // fail if either of the range edges overlap
        if(minCompare == 0 || maxCompare == 0) return false;
        
        // get the next closest range below
        range = Ranges.Array[index - 1];
        
        // compare with the max only
        minCompare = min.CompareTo(range.ArgB.Max);
        maxCompare = max.CompareTo(range.ArgB.Max);
        
        // if the specified range contains the maximum of the range
        if(minCompare < 0 && maxCompare > 0) return false;
        
        // fail if either of the range edges overlap
        if(minCompare == 0 || maxCompare == 0) return false;
        
        // get the next closest range above
        range = Ranges.Array[index + 1];
        
        // compare with the min only
        minCompare = min.CompareTo(range.ArgA);
        maxCompare = max.CompareTo(range.ArgA);
        
        // if the specified range contains the maximum of the range
        if(maxCompare > 0 && minCompare < 0) return false;
        
        // fail if the range edge overlaps
        if(maxCompare == 0) return false;
        
      }
      
      // add the range for the min and max range
      Ranges.Add(min, new Range {
        Max = max,
        Value = value
      });
      
      return true;
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// A key range and the associated value.
    /// </summary>
    public class Range {
      
      /// <summary>
      /// Max key inclusive.
      /// </summary>
      public TKey Max;
      /// <summary>
      /// Value associated with the key range.
      /// </summary>
      public TValue Value;
      
    }
    
  }

}