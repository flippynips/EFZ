using System;
using System.Collections.Generic;
using System.Collections;

namespace Efz.Collections {
  
  /// <summary>
  /// A class designed for management of a collection fo key-value pairs.
  /// Should be used delicately as most error checking is omitted in favor of speed.
  /// Accepts multiple matching keys however some functionality is clearly not
  /// designed favoring this.
  /// </summary>
  public class SortedRig<TKey, TValue> : IEnumerable<Struct<TKey, TValue>> where TKey : IComparable<TKey> {
        
    //-------------------------------------------//
    
    /// <summary>
    /// Array that represents the keys and values of this rig. Best to not access this directly.
    /// </summary>
    public Struct<TKey, TValue>[] Array;
    
    /// <summary>
    /// Current number of referenced items in sorted rig.
    /// </summary>
    public int Count;
    /// <summary>
    /// The capacity of the sorted rig.
    /// </summary>
    public int Capacity;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Function that can be used to get an average of two TKeys.
    /// </summary>
    private Func<TKey, TKey, TKey> _getAverage;
    
    //-------------------------------------------//
    
    protected SortedRig(Func<TKey, TKey, TKey> getAverage) {
      
      if(getAverage == null) {
        
        if(typeof(TKey) == typeof(double)) {
          _getAverage = (Func<TKey, TKey, TKey>)Delegate.CreateDelegate(typeof(Func<TKey, TKey, TKey>), this.GetType(), "AverageDouble");
        } else if(typeof(TKey) == typeof(float)) {
          _getAverage = (Func<TKey, TKey, TKey>)Delegate.CreateDelegate(typeof(Func<TKey, TKey, TKey>), this.GetType(), "AverageFloat");
        } else if(typeof(TKey) == typeof(int)) {
          _getAverage = (Func<TKey, TKey, TKey>)Delegate.CreateDelegate(typeof(Func<TKey, TKey, TKey>), this.GetType(), "AverageInt32");
        } else if(typeof(TKey) == typeof(long)) {
          _getAverage = (Func<TKey, TKey, TKey>)Delegate.CreateDelegate(typeof(Func<TKey, TKey, TKey>), this.GetType(), "AverageInt64");
        } else {
          throw new NotSupportedException("Type '"+typeof(TKey)+"' doesn't have a built in average implementation.");
        }
        
      } else {
        
        _getAverage = getAverage;
        
      }
      
    }
    
    /// <summary>
    /// Initializes a new sorted rig with the specified capacity.
    /// </summary>
    public SortedRig(Func<TKey, TKey, TKey> getAverage = null, int capacity = 2) : this(getAverage) {
      Capacity = capacity;
      Array = new Struct<TKey, TValue>[Capacity];
      Count = 0;
    }
    
    /// <summary>
    /// Initializes with the content of the specified array. If count is specified,
    /// the arrays are copied, else they are used directly.
    /// </summary>
    public SortedRig(Func<TKey, TKey, TKey> getAverage, Struct<TKey, TValue>[] items, int count = -1) : this(getAverage) {
      
      Capacity = items.Length;
      if(count < 0) {
        Count = Capacity;
        Array = items;
      } else {
        Count = count;
        Array = new Struct<TKey, TValue>[Count];
        // copy the array
        System.Array.Copy(items, Array, Count);
      }
      
    }
    
    /// <summary>
    /// Add the specified key value pair to the sorted rig.
    /// </summary>
    public void Add(TKey key, TValue value) {
      
      // get the index where the key and value should be inserted
      int index = IndexClosest(key);
      
      if(Capacity.Equals(Count)) {
        // double the capacity
        Capacity += Capacity;
        System.Array.Resize<Struct<TKey, TValue>>(ref Array, Capacity);
      }
      
      // if key-value should just be added
      if(index == -1) {
        
        Array[Count] = new Struct<TKey, TValue>(key, value);
        Count = 1;
        
      } else if(key.CompareTo(Array[index].ArgA) == 1) {
        
        // insert the key and value at 'index'
        Struct<TKey,TValue> shiftA = new Struct<TKey, TValue>(key, value);
        Struct<TKey,TValue> shiftB = default(Struct<TKey,TValue>);
        bool flip = true;
        for(int i = index + 1; i <= Count; ++i) {
          if(flip) {
            shiftB = Array[i];
            Array[i] = shiftA;
          } else {
            shiftA = Array[i];
            Array[i] = shiftB;
          }
          flip = !flip;
        }
        
        ++Count;
        
      } else {
        
        // insert the key and value at 'index'
        Struct<TKey,TValue> shiftA = new Struct<TKey, TValue>(key, value);
        Struct<TKey,TValue> shiftB = default(Struct<TKey,TValue>);
        bool flip = true;
        for(int i = index; i <= Count; ++i) {
          if(flip) {
            shiftB = Array[i];
            Array[i] = shiftA;
          } else {
            shiftA = Array[i];
            Array[i] = shiftB;
          }
          flip = !flip;
        }
        
        ++Count;
        
      }
      
    }
    
    /// <summary>
    /// Add the specified key value pair to the sorted rig and get the index
    /// that it was inserted at.
    /// </summary>
    public int AddIndex(TKey key, TValue value) {
      
      // get the index where the key and value should be inserted
      int index = IndexClosest(key);
      
      if(Capacity.Equals(Count)) {
        // double the capacity
        Capacity += Capacity;
        System.Array.Resize<Struct<TKey, TValue>>(ref Array, Capacity);
      }
      
      // if key-value should just be added
      if(index == -1) {
        
        index = 0;
        Array[index] = new Struct<TKey, TValue>(key, value);
        Count = 1;
        
      } else {
        
        // insert the key and value at 'index'
        Struct<TKey,TValue> shiftA = new Struct<TKey, TValue>(key, value);
        Struct<TKey,TValue> shiftB = default(Struct<TKey,TValue>);
        bool flip = true;
        for(int i = index; i <= Count; ++i) {
          if(flip) {
            shiftB = Array[i];
            Array[i] = shiftA;
          } else {
            shiftA = Array[i];
            Array[i] = shiftB;
          }
          flip = !flip;
        }
        
        ++Count;
        
      }
      
      // return the index that the item was added at
      return index;
      
    }
    
    /// <summary>
    /// Gets the closest value to the specified key.
    /// Returns default(TValue) if no items.
    /// </summary>
    public TValue ValueClosest(IComparable<TKey> key) {
      
      // no items skip
      if(Count == 0) return default(TValue);
      
      // early out if key is a maximum
      if(key.CompareTo(Array[Count-1].ArgA) >= 0) return Array[Count-1].ArgB;
      
      // early out if key is a minimum
      if(key.CompareTo(Array[0].ArgA) <= 0) return Array[0].ArgB;
      
      int max = Count;
      int min = 0;
      int next = min + (max - min >> 1);
      int median = -1;
      int compare;
      
      // while there are keys between the max and min
      while(median != next) {
        median = next;
        
        // compare the key at index with the target
        compare = key.CompareTo(Array[median].ArgA);
        
        // if the keys are equal
        if (compare == 0) return Array[median].ArgB;
        
        // if the key at index is less than the target
        if (compare < 0) {
          // move the max down
          max = median + 1;
        } else {
          // move the min up
          min = median - 1;
        }
        
        // set the median of the max and min
        next = min + (max - min >> 1);
        
      }
      
      // compare with value below
      if(median > 0) {
        var average = _getAverage(Array[median].ArgA, Array[median-1].ArgA);
        compare = key.CompareTo(average);
        if(compare < 0) return Array[median-1].ArgB;
      }
      
      // compare with value above
      if(median < Count-1) {
        var average = _getAverage(Array[median].ArgA, Array[median+1].ArgA);
        compare = key.CompareTo(average);
        if(compare > 0) return Array[median+1].ArgB;
      }
      
      // the key doesn't exist, return the closest
      return Array[median].ArgB;
      
    }
    
    /// <summary>
    /// Gets the index of the specified key. If there are duplicate keys, the
    /// index returned will indicate the lowest matching index.
    /// </summary>
    public int IndexRange(IComparable<TKey> key) {
      // if no items - skip
      if(Count == 0) return -1;
      
      int max = Count;
      int min = 0;
      int next = min + (max - min >> 1);
      int median = -1;
      int compare;
      
      // while there are keys between the max and min
      while(median != next) {
        median = next;
        
        // compare the key at index with the target
        compare = key.CompareTo(Array[median].ArgA);
        
        // if the keys are equal
        if (compare == 0) {
          // find the first index with the key
          if(median != 0) {
            while(median != 0 && key.CompareTo(Array[--median].ArgA) == 0) {}
            ++median;
          }
          return median;
        }
        
        // if the key at index is less than the target
        if (compare < 0) {
          // move the max down
          max = median + 1;
        } else {
          // move the min up
          min = median - 1;
        }
        
        // set the median of the max and min
        next = min + (max - min >> 1);
        
      }
      
      // compare with value below
      if(median > 0) {
        var average = _getAverage(Array[median].ArgA, Array[median-1].ArgA);
        compare = key.CompareTo(average);
        if(compare < 0) return key.CompareTo(Array[median-1].ArgA) == 0 ? median : -1;
      }
      
      // compare with value above
      if(median < Count-1) {
        var average = _getAverage(Array[median].ArgA, Array[median+1].ArgA);
        compare = key.CompareTo(average);
        if(compare > 0) return key.CompareTo(Array[median+1].ArgA) == 0 ? median : -1;
      }
      
      // if the last median isn't the key, the key doesn't exist
      return key.CompareTo(Array[median].ArgA) == 0 ? median : -1;
    }
    
    /// <summary>
    /// Gets the index of the specified key. This may be within a range
    /// of values with the same key. Returns -1 if the key isn't found.
    /// </summary>
    public int IndexOf(IComparable<TKey> key) {
      // if no items - skip
      if(Count == 0) return -1;
      
      int max = Count;
      int min = 0;
      int next = min + (max - min >> 1);
      int median = -1;
      int compare;
      
      // while there are keys between the max and min
      while(median != next) {
        median = next;
        
        // compare the key at index with the target
        compare = key.CompareTo(Array[median].ArgA);
        
        // if the keys are equal
        if (compare == 0) return median;
        
        // is the key smaller than the current index? yes, move the max down
        if (compare < 0) max = median + 1;
        // no, move the min up
        else min = median - 1;
        
        // set the median of the max and min
        next = min + (max - min >> 1);
        
      }
      
      // compare with value below
      if(median > 0) {
        var average = _getAverage(Array[median].ArgA, Array[median-1].ArgA);
        compare = key.CompareTo(average);
        if(compare < 0) return key.CompareTo(Array[median-1].ArgA) == 0 ? median : -1;
      }
      
      // compare with value above
      if(median < Count-1) {
        var average = _getAverage(Array[median].ArgA, Array[median+1].ArgA);
        compare = key.CompareTo(average);
        if(compare > 0) return key.CompareTo(Array[median+1].ArgA) == 0 ? median : -1;
      }
      
      // if the last median isn't the key, the key doesn't exist
      return key.CompareTo(Array[median].ArgA) == 0 ? median : -1;
    }
    
    /// <summary>
    /// Gets indexes of the specified key. Returns an empty collection if
    /// the key isn't found.
    /// </summary>
    public ArrayRig<int> AllIndexesOf(IComparable<TKey> key) {
      // if no items - skip
      if(Count == 0) return new ArrayRig<int>(1);
      
      int max = Count;
      int min = 0;
      int next = min + (max - min >> 1);
      int median = -1;
      int compare;
      ArrayRig<int> collection = new ArrayRig<int>();
      
      // while there are keys between the max and min
      while(median != next) {
        median = next;
        
        // compare the key at index with the target
        compare = key.CompareTo(Array[median].ArgA);
        
        // if the keys are equal
        if (compare == 0) {
          collection.Add(median);
          for(int i = median-1; i >= 0; --i) {
            if(key.CompareTo(Array[i].ArgA) == 0) collection.Add(i);
            else break;
          }
          for(int i = median + 1; i < Count; ++i) {
            if(key.CompareTo(Array[i].ArgA) == 0) collection.Add(i);
            else break;
          }
          return collection;
        }
        
        // is the key smaller than the current index? yes, move the max down
        if (compare < 0) max = median + 1;
        // no, move the min up
        else min = median - 1;
        
        // set the median of the max and min
        next = min + (max - min >> 1);
        
      }
      
      if(median > 0) {
        var average = _getAverage(Array[median].ArgA, Array[median-1].ArgA);
        compare = key.CompareTo(average);
        if(compare < 0) --median;
      }
      
      if(key.CompareTo(Array[median].ArgA) != 0) {
        collection.Add(median);
        for(int i = median-1; i >= 0; --i) {
          if(key.CompareTo(Array[i].ArgA) == 0) collection.Add(i);
        }
        for(int i = median + 1; i < Count; ++i) {
          if(key.CompareTo(Array[i].ArgA) == 0) collection.Add(i);
        }
      }
      
      // if the last median isn't the key, the key doesn't exist
      return collection;
    }
    
    /// <summary>
    /// Gets the closest index less than or equal to the specified key.
    /// Returns -1 if no items.
    /// </summary>
    public int IndexClosest(IComparable<TKey> key) {

      // no items skip
      if(Count == 0) return -1;
      
      // early out if key is a maximum
      if(key.CompareTo(Array[Count-1].ArgA) >= 0) return Count-1;
      
      // early out if key is a minimum
      if(key.CompareTo(Array[0].ArgA) <= 0) return 0;
      
      int max = Count;
      int min = 0;
      int next = min + (max - min >> 1);
      int median = -1;
      int compare;
      
      // while there are keys between the max and min
      while(median != next) {
        median = next;
        
        // compare the key at index with the target
        compare = key.CompareTo(Array[median].ArgA);
        
        // if the keys are equal
        if (compare == 0) return median;
        
        // if the key at index is less than the target
        if (compare < 0) {
          // move the max down
          max = median + 1;
        } else {
          // move the min up
          min = median - 1;
        }
        
        // set the median of the max and min
        next = min + (max - min >> 1);
        
      }
      
      // compare with value below
      if(median > 0) {
        var average = _getAverage(Array[median].ArgA, Array[median-1].ArgA);
        compare = key.CompareTo(average);
        if(compare < 0) return median - 1;
      }
      
      // compare with value above
      if(median < Count-1) {
        var average = _getAverage(Array[median].ArgA, Array[median+1].ArgA);
        compare = key.CompareTo(average);
        if(compare > 0) return median + 1;
      }
      
      // the key doesn't exist, return the closest
      return median;
    }
    
    /// <summary>
    /// Gets whether the rig contains the specified key.
    /// </summary>
    public bool Contains(IComparable<TKey> key) {
      // if no items - skip
      if(Count == 0) return false;
      
      int max = Count;
      int min = 0;
      int next = min + (max - min >> 1);
      int median = -1;
      int compare;
      
      // while there are keys between the max and min
      while(median != next) {
        median = next;
        
        // compare the key at index with the target
        compare = key.CompareTo(Array[median].ArgA);
        
        // if the keys are equal
        if (compare == 0) return true;
        
        // if the key at index is less than the target
        if (compare < 0) {
          // move the max down
          max = median + 1;
        } else {
          // move the min up
          min = median - 1;
        }
        
        // set the median of the max and min
        next = min + (max - min >> 1);
        
      }
      
      // compare with value below
      if(median > 0) {
        var average = _getAverage(Array[median].ArgA, Array[median-1].ArgA);
        compare = key.CompareTo(average);
        if(compare < 0) return key.CompareTo(Array[median-1].ArgA) == 0;
      }
      
      // compare with value above
      if(median < Count-1) {
        var average = _getAverage(Array[median].ArgA, Array[median+1].ArgA);
        compare = key.CompareTo(average);
        if(compare > 0) return key.CompareTo(Array[median+1].ArgA) == 0;
      }
      
      // if the last median isn't the key, the key doesn't exist
      return key.CompareTo(Array[median].ArgA) == 0;
    }
    
    /// <summary>
    /// Remove the item at the specified index.
    /// </summary>
    public void RemoveAt(int index) {
      --Count;
      for(int i = index; i < Count; ++i) Array[i] = Array[i+1];
    }
    
    /// <summary>
    /// Remove a single item of the specified key.
    /// </summary>
    public void Remove(IComparable<TKey> key) {
      int index = IndexOf(key);
      // skip if key doesn't exist
      if(index == -1) return;
      // shift all items down one index
      --Count;
      for(int i = index; i < Count; ++i) Array[i] = Array[i+1];
    }
    
    /// <summary>
    /// Remove all items of the specified key.
    /// </summary>
    public void RemoveAll(IComparable<TKey> key) {
      int index = IndexRange(key);
      
      // key doesn't exist
      if(index == -1) return;
      
      // get the number of items to remove
      int count = index;
      while(++count < Count && key.CompareTo(Array[count].ArgA) == 0) {}
      count = count - index - 1;
      
      // iterate through items removing 'count' items
      for(int i = index; i < Count - count; ++i) Array[i] = Array[i+count];

      // decrement the total Count
      Count -= count;
    }
    
    /// <summary>
    /// Reset this instance while not nulling references.
    /// </summary>
    public void Reset() {
      Count = 0;
    }
    
    /// <summary>
    /// Reset this instance and the array within for reuse.
    /// </summary>
    public void Clear() {
      Array = new Struct<TKey, TValue>[Capacity];
      Count = 0;
    }
    
    /// <summary>
    /// Releases all resource used by the collection.
    /// </summary>
    public void Dispose() {
      Count = -1;
      Array = null;
    }
    
    /// <summary>
    /// Manually resize the contained array.
    /// If count is greater than the array size, it is reduced.
    /// </summary>
    public void Resize(int size) {
      Capacity = size;
      System.Array.Resize<Struct<TKey,TValue>>(ref Array, size);
      if(Count > size) Count = size;
    }
    
    /// <summary>
    /// Get or set the value of the specified key. Returns 'default(TValue)' if not found.
    /// </summary>
    public TValue this[TKey key] {
      get {
        // if no items - skip
        if(Count == 0) return default(TValue);
        
        int max = Count;
        int min = 0;
        int next = min + (max - min >> 1);
        int median = -1;
        int compare;
        
        // while there are keys between the max and min
        while(median != next) {
          
          median = next;
          
          // compare the key at index with the target
          compare = key.CompareTo(Array[median].ArgA);
          
          // if the keys are equal
          if (compare == 0) {
            return Array[median].ArgB;
          }
          
          // if the key at index is less than the target
          if (compare < 0) {
            // move the max down
            max = median + 1;
          } else {
            // move the min up
            min = median - 1;
          }
          
          // set the next median of the max and min
          next = min + (max - min >> 1);
          
        }
        
        if(median > 0) {
          var average = _getAverage(Array[median].ArgA, Array[median-1].ArgA);
          compare = key.CompareTo(average);
          if(compare < 0) --median;
        }
        
        // if the last median isn't the key, the key doesn't exist
        return key.CompareTo(Array[median].ArgA) == 0 ? Array[median].ArgB : default(TValue);
      }
      set {
        // if no items - add the key
        if(Count == 0) Add(key, value);
        
        int max = Count;
        int min = 0;
        int next = min + (max - min >> 1);
        int median = -1;
        int compare;
        
        // while there are keys between the max and min
        while(median != next) {
          median = next;
          
          // compare the key at index with the target
          compare = key.CompareTo(Array[median].ArgA);
          
          // if the keys are equal
          if (compare == 0) {
            Array[median] = new Struct<TKey, TValue>(key, value);
          }
          
          // if the key at index is less than the target
          if (compare < 0) {
            // move the max down
            max = median + 1;
          } else {
            // move the min up
            min = median - 1;
          }
          
          // set the median of the max and min
          next = min + (max - min >> 1);
          
        }
        
        if(median > 0) {
          var average = _getAverage(Array[median].ArgA, Array[median-1].ArgA);
          compare = key.CompareTo(average);
          if(compare < 0) --median;
        }
        
        // if the last median isn't the key, the key doesn't exist
        if(key.CompareTo(Array[median].ArgA) == 0) {
          // key exists - set
          Array[median] = new Struct<TKey, TValue>(key, value);
        } else {
          // increase capacity if required
          if(Capacity.Equals(++Count)) {
            // double the capacity
            Capacity += Capacity;
            System.Array.Resize<Struct<TKey, TValue>>(ref Array, Capacity);
          }
          
          // insert the key value pair at median index
          Struct<TKey,TValue> shiftA = new Struct<TKey, TValue>(key, value);
          Struct<TKey,TValue> shiftB = default(Struct<TKey, TValue>);
          bool flip = true;
          for(int i = median; i < Count; ++i) {
            if(flip) {
              shiftB = Array[i];
              Array[i] = shiftA;
            } else {
              shiftA = Array[i];
              Array[i] = shiftB;
            }
            flip = !flip;
          }
        }
        
      }
    }
    
    public IEnumerator<Struct<TKey, TValue>> GetEnumerator() {
      return new Enumerator(this);
    }
    
    public override string ToString() {
      // get display of current items
      System.Text.StringBuilder sb = new System.Text.StringBuilder();
      int index = 0;
      bool delimiter = false;
      if(Count > 0) {
        foreach(Struct<TKey, TValue> item in this) {
          if(delimiter) sb.Append("\n    ");
          else delimiter = true;
          // append the string representation of each item
          sb.Append(index);
          sb.Append(") ");
          sb.Append(item.ArgA + ", " + item.ArgB);
          ++index;
        }
      } else {
        sb.Append("Empty");
      }
      return string.Format("\n[SortedRig Count={0}, Capacity={1}, Items=\n[\n    {2}\n]]\n", Count, Capacity, sb);
    }
    
    /// <summary>
    /// Get an average of two double values.
    /// </summary>
    public static double AverageDouble(double valueA, double valueB) {
      return (valueA + valueB) / 2;
    }
    
    /// <summary>
    /// Get an average of two float values.
    /// </summary>
    public static float AverageFloat(float valueA, float valueB) {
      return (valueA + valueB) / 2;
    }
    
    /// <summary>
    /// Get an average of two integer values.
    /// </summary>
    public static int AverageInt32(int valueA, int valueB) {
      return (valueA + valueB) / 2;
    }
    
    /// <summary>
    /// Get an average of two integer values.
    /// </summary>
    public static long AverageInt64(long valueA, long valueB) {
      return (valueA + valueB) / 2;
    }
    
    //-------------------------------------------//
    
    IEnumerator IEnumerable.GetEnumerator() {
      return new Enumerator(this);
    }
    
    /// <summary>
    /// Simple enumerator for the SortedRig.
    /// </summary>
    public class Enumerator : IEnumerator<Struct<TKey, TValue>> {
      
      //-------------------------------------------//
      
      public Struct<TKey, TValue> Current { get; protected set; }
      object IEnumerator.Current { get { return Current; } }
      
      public int Index;
      
      //-------------------------------------------//
      
      private SortedRig<TKey, TValue> _rig;
      
      //-------------------------------------------//
      
      public Enumerator(SortedRig<TKey, TValue> rig) {
        _rig = rig;
        Index = 0;
      }
      
      public void Dispose() {
      }
      
      public bool MoveNext() {
        if(Index < _rig.Count) {
          Current = _rig.Array[Index];
          ++Index;
          return true;
        }
        return false;
      }
      
      public void Reset() {
        Index = 0;
      }
      
      //-------------------------------------------//
      
    }
        
  }

}