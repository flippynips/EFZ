using System;
using System.Collections.Generic;
using System.Collections;

namespace Efz.Collections {
  
  /// <summary>
  /// A class designed for management of an array in favor of speed. Much faster than most collections.
  /// Should be used delicately as most error checking is omitted in favor of speed.
  /// </summary>
  public class ArrayRig<T> : IEnumerable<T>, IDisposable {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The array managed by this array rig. Best not to access this directly.
    /// </summary>
    public T[] Array;
    /// <summary>
    /// Current number of referenced items in array.
    /// </summary>
    public int Count;
    /// <summary>
    /// The current capacity of the array.
    /// </summary>
    public int Capacity;
        
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initializes a new ArrayRig with default starting size 8.
    /// </summary>
    public ArrayRig() {
      Capacity = 2;
      Array = new T[Capacity];
      Count = 0;
    }
    
    /// <summary>
    /// Initializes with the content of the specified array. Note that a 'count' and 'index' of 0 or less
    /// will cause the array rig to reference the array itself.
    /// </summary>
    /// <param name="array">Array to reference content of.</param>
    /// <param name = "index">(optional) Index to start the copy from.</param>
    /// <param name="count">(optional) Count of array rig to be initialized with.</param>
    public ArrayRig(T[] array, int index = 0, int count = -1) {
      if(array.Length == 0) {
        Capacity = count <= 0 ? 2 : count;
        Array = new T[Capacity];
      } else {
        if(count < 0) {
          if(index > 0) {
            Count = Capacity = array.Length - index;
            System.Array.Copy(array, index, Array = new T[Capacity], 0, Count);
          } else {
            Count = Capacity = array.Length;
            Array = array;
          }
        } else {
          Capacity = Count = count;
          System.Array.Copy(array, index, Array = new T[Count], 0, Count);
        }
      }
    }
    
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="size">(optional) Size of array to begin with.</param>
    public ArrayRig(int size) {
      Capacity = size < 1 ? 1 : size;
      Array = new T[Capacity];
      Count = 0;
    }
    
    /// <summary>
    /// Create a new array rig by cloning another.
    /// </summary>
    /// <param name="arrayRig">Array rig to clone.</param>
    public unsafe ArrayRig(ArrayRig<T> arrayRig) {
      Count = arrayRig.Count;
      Capacity = arrayRig.Array.Length;
      Array = new T[Capacity];
      
      System.Array.Copy(arrayRig.Array, Array = new T[Capacity], Count);
    }
    
    /// <summary>
    /// Add the specified item to the end of the array.
    /// </summary>
    public void Add(T item) { 
      // is the array full?
      if(Capacity == Count) {
        // double array size
        Capacity += Capacity;
        System.Array.Resize<T>(ref Array, Capacity);
      }
      Array[Count] = item;
      ++Count;
    }
    
    /// <summary>
    /// Add the specified items to the end of the array.
    /// </summary>
    public void AddItems(params T[] items) {
      if(Capacity < items.Length + Count) {
        Capacity = Count + items.Length;
        System.Array.Resize<T>(ref Array, Capacity);
      }
      for(int i = 0; i < items.Length; ++i) {
        Array[Count] = items[i];
        ++Count;
      }
    }
    
    /// <summary>
    /// Add a collection of items to the array rig.
    /// </summary>
    public void AddCollection(IEnumerable<T> items) {
      foreach(T item in items) Add(item);
    }
    
    /// <summary>
    /// Add the specified items to the end of the current array.
    /// </summary>
    public void Add(T[] items, int start, int end) {
      if(Capacity < end - start + Count + 1) {
        Capacity = Count + end - start;
        System.Array.Resize<T>(ref Array, Capacity);
      }
      for(int i = start; i < end; ++i) {
        Array[Count] = items[i];
        ++Count;
      }
    }
    
    /// <summary>
    /// Add the specified items to the end of the array.
    /// </summary>
    public void AddItems(ArrayRig<T> items) {
      if(Capacity < items.Count + Count) {
        Capacity = Count + items.Count;
        System.Array.Resize<T>(ref Array, Capacity);
      }
      for(int i = 0; i < items.Count; ++i) {
        Array[Count] = items[i];
        ++Count;
      }
    }
    
    /// <summary>
    /// Add the specified items from the start index to the end index to the end of this rig.
    /// </summary>
    public void Add(ArrayRig<T> items, int start, int end) {
      if(Capacity < end - start + Count + 1) {
        Capacity = Count + end - start;
        System.Array.Resize<T>(ref Array, Capacity);
      }
      for(int i = start; i < end; ++i) {
        Array[Count] = items[i];
        ++Count;
      }
    }
    
    /// <summary>
    /// Insert the specified item at the specified index. Order of the array is maintained.
    /// </summary>
    public void Insert(T item, int index) {
      if(Count == Capacity) {
        Capacity += Capacity;
        System.Array.Resize<T>(ref Array, Capacity);
      }
      ++Count;
      T shift = default(T);
      bool flip = false;
      for(int i = index; i < Count; ++i) {
        if(flip) {
          item = Array[i];
          Array[i] = shift;
        } else {
          shift = Array[i];
          Array[i] = item;
        }
        flip = !flip;
      }
    }
    
    /// <summary>
    /// Insert the specified item at the specified index. Order of the array is not maintained.
    /// The item previously at the specified index becomes the last element in the array.
    /// </summary>
    public void InsertQuick(T item, int index) {
      if(index == Count) {
        if(Count == Capacity) {
          Capacity += Capacity;
          System.Array.Resize<T>(ref Array, Capacity);
        }
        ++Count;
        Array[index] = item;
      } else {
        if(Count == Capacity) {
          Capacity += Capacity;
          System.Array.Resize<T>(ref Array, Capacity);
        }
        ++Count;
        Array[Count-1] = Array[index];
        Array[index] = item;
      }
    }
    
    /// <summary>
    /// Removes the item at the specified index. This is fast as the order of the array is not maintained.
    /// </summary>
    public void RemoveQuick(int index) {
      --Count;
      Array[index] = Array[Count];
    }
    
    /// <summary>
    /// Remove the specified item from the array. This is fast as the order of the array is not maintained.
    /// </summary>
    /// <param name="item">Item to remove.</param>
    public void RemoveQuick(T item) {
      for(int i = Count-1; i >= 0; --i) {
        if(Array[i].Equals(item)) {
          --Count;
          Array[i] = Array[Count];
          break;
        }
      }
    }
    
    /// <summary>
    /// Removes the specified item and returns whether it was found within the array.
    /// The order of the array is not maintained.
    /// </summary>
    public bool RemoveCheckQuick(T item) {
      for(int i = Count-1; i >= 0; --i) {
        if(Array[i].Equals(item)) {
          --Count;
          Array[i] = Array[Count];
          return true;
        }
      }
      return false;
    }
    
    /// <summary>
    /// Remove the specified item from the array and return it. This is fast as the order of the array is not maintained.
    /// </summary>
    public T RemoveReturnQuick(int index) {
      --Count;
      T _item = Array[index];
      Array[index] = Array[Count];
      return _item;
    }
    
    /// <summary>
    /// Remove the item at the specified index, maintaining the order of array items.
    /// </summary>
    public void Remove(int index) {
      --Count;
      if(index == Count) {
        Array[index] = default(T);
      } else {
        for(int i = index; i < Count; ++i) {
          Array[i] = Array[i+1];
        }
      }
    }
    
    /// <summary>
    /// Remove the item at the specified index, maintaining the order of array items.
    /// Returns the item that was removed or default(T).
    /// </summary>
    public T RemoveReturn(int index) {
      --Count;
      T item;
      if(index == Count) {
        item = Array[index];
        Array[index] = default(T);
      } else if(index < Count) {
        item = Array[index];
        for(int i = index; i < Count; ++i) Array[i] = Array[i+1];
      } else {
        return default(T);
      }
      return item;
    }
    
    /// <summary>
    /// Remove the specified item from the array, maintaining order of array items.
    /// </summary>
    public void Remove(T item) {
      bool found = false;
      for(int i = 0; i < Count; ++i) {
        if(found) {
          Array[i] = Array[i + 1];
        } else {
          if(Array[i].Equals(item)) {
            --Count;
            if(i == Count) Array[i] = default(T);
            else Array[i] = Array[i + 1];
            found = true;
          }
        }
      }
    }
    
    /// <summary>
    /// Removes an item and returns if it was found in the array.
    /// </summary>
    public bool RemoveCheck(T item) {
      bool found = false;
      for(int i = 0; i < Count; ++i) {
        if(found) {
          Array[i] = Array[i + 1];
        } else {
          if(Array[i].Equals(item)) {
            Array[i] = Array[i + 1];
            found = true;
            Count--;
          }
        }
      }
      return found;
    }
    
    /// <summary>
    /// Removes an item and returns at which index it was found in the array.
    /// </summary>
    public int RemoveIndex(T item) {
      bool found = false;
      int index = -1;
      for(int i = 0; i < Count; ++i) {
        if(found) {
          Array[i] = Array[i + 1];
        } else {
          if(Array[i].Equals(item)) {
            Array[i] = Array[i + 1];
            found = true;
            index = i;
            Count--;
          }
        }
      }
      return index;
    }
    
    /// <summary>
    /// Remove the last item in the array and return it.
    /// The reference will be removed from the array.
    /// </summary>
    public T Pop() {
      --Count;
      T item = Array[Count];
      Array[Count] = default(T);
      return item;
    }
    
    /// <summary>
    /// Does the array contain the specified item?
    /// </summary>
    public bool Contains(T item) {
      for(int i = Count-1; i >= 0; --i) {
        if(Array[i].Equals(item)) return true;
      }
      return false;
    }
    
    /// <summary>
    /// Swap the items in the array specified by index1 and index2.
    /// </summary>
    public void Swap(int index1, int index2) {
      if(!index1.Equals(index2)) {
        T swap = Array[index2];
        Array[index2] = Array[index1];
        Array[index1] = swap;
      }
    }
    
    /// <summary>
    /// Gets the index of the specified item.
    /// Returns '-1' if the item isn't found.
    /// </summary>
    public int IndexOf(T item) {
      for(int i = 0; i < Count; ++i) {
        if(Array[i].Equals(item)) {
          return i;
        }
      }
      return -1;
    }
    
    /// <summary>
    /// Get the last index of the specified item.
    /// Returns '-1' if the item isn't found.
    /// </summary>
    public int LastIndexOf(T item) {
      for(int i = Count-1; i >= 0; --i) {
        if(Array[i].Equals(item)) {
          return i;
        }
      }
      return -1;
    }
    
    /// <summary>
    /// Does the array start with the specified sequence of items.
    /// </summary>
    public bool StartsWith(params T[] items) {
      // does the collection contain enough items? no, return
      if(Count < items.Length) return false;
      // iterate the collection of items
      for(int i = items.Length-1; i >= 0; --i) {
        if(!Array[i].Equals(items[i])) {
          // the items aren't equal
          return false;
        }
      }
      // the items were equal
      return true;
    }
    
    /// <summary>
    /// Does the array start with the specified sequence of items.
    /// </summary>
    public bool EndsWith(params T[] items) {
      // does the collection contain enough items? no, return
      if(Count < items.Length) return false;
      // iterate the collection of items
      for(int i = items.Length-1; i >= 0; --i) {
        if(!Array[Count + i - items.Length].Equals(items[i])) {
          // the items aren't equal
          return false;
        }
      }
      // the items were equal
      return true;
    }
    
    /// <summary>
    /// Replace one item with another.
    /// </summary>
    /// <param name="before">The item to be replaced.</param>
    /// <param name="after">The item to replace the other.</param>
    public void Replace(T before, T after) {
      for(int i = Count - 1; i >= 0; --i) {
        if(Array[i].Equals(before)) {
          Array[i] = after;
          break;
        }
      }
    }
    
    /// <summary>
    /// Replace all instances of one item with another.
    /// </summary>
    /// <param name="before">The item to be replaced.</param>
    /// <param name="after">The item to replace the other.</param>
    public void ReplaceAll(T before, T after) {
      for(int i = Count - 1; i >= 0; --i) {
        if(Array[i].Equals(before)) {
          Array[i] = after;
        }
      }
    }
    
    /// <summary>
    /// Map the current set of items to the new rig of the defined type.
    /// Optionally a predicate determines whether the item is mapped.
    /// </summary>
    public ArrayRig<V> Map<V>(Func<T, V> mapper, Func<T, bool> predicate = null) {
      ArrayRig<V> mapped = new ArrayRig<V>();
      if(predicate == null) {
        foreach(T item in this) {
          mapped.Add(mapper(item));
        }
      } else {
        foreach(T item in this) {
          if(predicate(item)) mapped.Add(mapper(item));
        }
      }
      return mapped;
    }
    
    /// <summary>
    /// Try get the item that satisfies the conditional. Returns default if no match.
    /// </summary>
    public T GetSingle(Func<T, bool> predicate) {
      foreach(T item in this) {
        if(predicate(item)) {
          return item;
        }
      }
      return default(T);
    }
    
    /// <summary>
    /// Try find the item that satisfies the conditional and remove it. Returns if an item was found.
    /// </summary>
    public bool RemoveSingle(Func<T, bool> predicate) {
      foreach(T item in this) {
        if(predicate(item)) {
          Remove(item);
          return true;
        }
      }
      return false;
    }
    
    /// <summary>
    /// Try find the item that satisfies the conditional and remove it. Returns if an item was found.
    /// Order of the array is not maintained.
    /// </summary>
    public bool RemoveSingleQuick(Func<T, bool> predicate) {
      foreach(T item in this) {
        if(predicate(item)) {
          RemoveQuick(item);
          return true;
        }
      }
      return false;
    }
    
    /// <summary>
    /// Reset this instance without nulling references.
    /// </summary>
    public void Reset() {
      Count = 0;
    }
    
    /// <summary>
    /// Reset this instance and it's contained array.
    /// </summary>
    public void Clear() {
      Array = new T[Capacity];
      Count = 0;
    }
    
    /// <summary>
    /// Releases all resource used by the <see cref="sp.tools.ArrayRig`1"/> object.
    /// </summary>
    public void Dispose() {
      Count = -1;
      Array = null;
    }
    
    /// <summary>
    /// Manually resize the contained array.
    /// If count is greater than the array size, it is reduced.
    /// </summary>
    public void SetCapacity(int size) {
      Capacity = size;
      System.Array.Resize<T>(ref Array, size);
      if(Count > size) Count = size;
    }
    
    /// <summary>
    /// Sort using the specified comparer. If the comparer returns false, the items are swapped.
    /// This way your function will specify the state your want to exist.
    /// </summary>
    public void Sort(Func<T, T, bool> comparer) {
      T swap;
      int swaps = Count - 2;
      for(int a = swaps; a > -1; --a) {
        for(int b = Count-2; b > -1; --b) {
          if(!comparer(Array[b], Array[b+1])) {
            swap = Array[b];
            Array[b] = Array[b+1];
            Array[b+1] = swap;
            ++swaps;
          }
        }
        swaps -= Count - 2;
      }
    }
    
    /// <summary>
    /// Return a copy of the array of assigned items.
    /// </summary>
    public T[] ToArray() {
      T[] a = new T[Count];
      System.Array.Copy(Array, a, Count);
      return a;
    }
    
    /// <summary>
    /// Gets or sets the array item at the specified index.
    /// </summary>
    public T this[int index] {
      get { return Array[index]; }
      set { Array[index] = value; }
    }
    
    public IEnumerator<T> GetEnumerator() {
      return new Enumerator(this);
    }
    
    public override string ToString() {
      // get display of current items
      System.Text.StringBuilder sb = new System.Text.StringBuilder();
      int index = 0;
      bool delimiter = false;
      int count = Count;
      if(count > 0) {
        if(count < 50) {
          foreach(T item in this) {
            if(delimiter) sb.Append("\n    ");
            else delimiter = true;
            // append the string representation of each item
            sb.Append(index);
            sb.Append(") ");
            if(item.Equals(default(T))) sb.Append(item);
            else sb.Append(item.ToString());
            
            ++index;
          }
        } else {
          sb.Append(count + "items");
        }
      } else {
        sb.Append("Empty");
      }
      int capacity = Capacity;
      return string.Format("\n[ArrayRig Count={0}, ArraySize={1}, ArrayItems=\n[\n    {2}\n]]\n", count, capacity, sb);
    }
    
    //-------------------------------------------//
    
    IEnumerator IEnumerable.GetEnumerator() {
      return new Enumerator(this);
    }
    
    /// <summary>
    /// Simple enumerator for ArrayRigs.
    /// </summary>
    public class Enumerator : IEnumerator<T> {
      
      //-------------------------------------------//
      
      public T Current { get; protected set; }
      object IEnumerator.Current { get { return Current; } }
      
      public int Index;
      
      //-------------------------------------------//
      
      private ArrayRig<T> _rig;
      
      //-------------------------------------------//
      
      public Enumerator(ArrayRig<T> rig) {
        _rig = rig;
        Index = 0;
      }
      
      public void Dispose() {
        _rig = null;
      }
      
      public bool MoveNext() {
        if(Index < _rig.Count) {
          Current = _rig[Index];
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