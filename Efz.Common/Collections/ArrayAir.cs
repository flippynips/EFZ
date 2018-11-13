using System;
using System.Collections.Generic;
using System.Collections;

namespace Efz.Collections {
  
  /// <summary>
  /// A class designed for management of an array with callback functionality.
  /// </summary>
  public class ArrayAir<T> : IEnumerable<T> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The array managed by this array rig.
    /// </summary>
    public T[] Array;
    /// <summary>
    /// Current number of referenced items in array.
    /// </summary>
    public int Count;
    /// <summary>
    /// The total size of the array.
    /// </summary>
    public int Capacity;
    
    /// <summary>
    /// Event call on add.
    /// </summary>
    public event Action<T> OnAdd;
    /// <summary>
    /// Event call on remove.
    /// </summary>
    public event Action<T> OnRemove;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initializes a new collection with default starting size 2.
    /// </summary>
    public ArrayAir() {
      Capacity = 2;
      Array = new T[Capacity];
      Count = 0;
    }
    
    /// <summary>
    /// Initializes a new instance of the collection.
    /// </summary>
    /// <param name="size">(optional) Size of array to begin with.</param>
    public ArrayAir(int size) {
      Capacity = size;
      Array = new T[Capacity];
      Count = 0;
    }
    
    /// <summary>
    /// Initializes with the content of the specified array.
    /// </summary>
    /// <param name="array">Array to reference content of.</param>
    /// <param name="count">(optional) Count of array rig to be initialized with. If unspecified, the array will be referenced directly.</param>
    public ArrayAir(T[] array, int count = -1) {
      Capacity = array.Length;
      if(count < 0) {
        Count = Array.Length;
        Array = array;
      } else {
        Count = count;
        System.Array.Copy(array, Array = new T[Capacity], Capacity);
      }
    }
    
    /// <summary>
    /// Create a new array air by cloning another.
    /// </summary>
    public ArrayAir(ArrayAir<T> array) {
      Count = array.Count;
      Capacity = array.Array.Length;
      System.Array.Copy(array.Array, Array = new T[Capacity], Count);
    }
    
    /// <summary>
    /// Add the specified item to the end of the array.
    /// </summary>
    public void Add(T item) {
      Array[Count] = item;
      if(Capacity.Equals(++Count)) {
        Capacity += Capacity;
        System.Array.Resize<T>(ref Array, Capacity);
      }
      // is the add callback set? yes, run
      if(OnAdd != null) OnAdd(item);
    }
    
    /// <summary>
    /// Add the specified items to the end of the array.
    /// </summary>
    public void Add(params T[] items) {
      if(Capacity < items.Length + Count + 1) {
        Capacity = Count + items.Length;
        System.Array.Resize<T>(ref Array, Capacity);
      }
      for(int i = 0; i < items.Length; ++i) {
        Array[Count] = items[i];
        ++Count;
        if(OnAdd != null) OnAdd(items[i]);
      }
    }
    
    /// <summary>
    /// Insert the specified item at the specified index. Order of the array is maintained.
    /// </summary>
    public void InsertOrdered(T item, int index) {
      if(Capacity.Equals(++Count)) {
        Capacity += Capacity;
        System.Array.Resize<T>(ref Array, Capacity);
      }
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
      if(OnAdd != null) OnAdd(item);
    }
    
    /// <summary>
    /// Insert the specified item at the specified index. Order of the array is not maintained.
    /// The item previously at the specified index becomes the last element in the array.
    /// </summary>
    public void Insert(T item, int index) {
      if(Capacity.Equals(++Count)) {
        Capacity += Capacity;
        System.Array.Resize<T>(ref Array, Capacity);
      }
      Array[Count-1] = Array[index];
      Array[index] = item;
      if(OnAdd != null) {
        OnAdd(item);
      }
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
    /// </summary>
    public int IndexOf(T item) {
      for(int i = Count-1; i >= 0; --i) {
        if(Array[i].Equals(item)) {
          return i;
        }
      }
      return -1;
    }
    
    /// <summary>
    /// Removes the item at the specified index. This is fast as the order of the array is not maintained.
    /// </summary>
    public void Remove(int index) {
      --Count;
      Array[index] = Array[Count];
      if(OnRemove != null) {
        OnRemove(Array[index]);
      }
    }
    
    /// <summary>
    /// Remove the specified item from the array. This is fast as the order of the array is not maintained.
    /// </summary>
    public void Remove(T item) {
      for(int i = Count-1; i >= 0; --i) {
        if(Array[i].Equals(item)) {
          --Count;
          Array[i] = Array[Count];
          break;
        }
      }
      if(OnRemove != null) {
        OnRemove(item);
      }
    }
    
    /// <summary>
    /// Removes the specified item and returns whether it was found within the array.
    /// </summary>
    public bool RemoveCheck(T item) {
      for(int i = Count-1; i >= 0; --i) {
        if(Array[i].Equals(item)) {
          --Count;
          Array[i] = Array[Count];
          if(OnRemove != null) OnRemove(item);
          return true;
        }
      }
      return false;
    }
    
    /// <summary>
    /// Remove the specified item from the array and return it. This is fast as the order of the array is not maintained.
    /// </summary>
    public T RemoveReturn(int index) {
      --Count;
      T _item = Array[index];
      Array[index] = Array[Count];
      if(OnRemove != null) OnRemove(Array[index]);
      return _item;
    }
    
    /// <summary>
    /// Remove the item at the specified index, maintaining the order of array items.
    /// </summary>
    /// <param name="index">Index of item to be removed.</param>
    public void RemoveOrdered(int index) {
      T item = Array[index];
      for(int i = index; i < Count; ++i) {
        Array[i] = Array[i+1];
      }
      --Count;
      if(OnRemove != null) OnRemove(item);
    }
    
    /// <summary>
    /// Remove the specified item from the array, maintaining order of array items.
    /// </summary>
    public void RemoveOrdered(T item) {
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
      if(OnRemove != null) OnRemove(item);
    }
    
    /// <summary>
    /// Removes an item and returns if it was found in the array.
    /// </summary>
    public bool RemoveCheckOrdered(T item) {
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
      if(found && OnRemove != null) OnRemove(item);
      return found;
    }
    
    /// <summary>
    /// Removes an item and returns at which index it was found in the array.
    /// </summary>
    public int RemoveOrderedIndex(T item) {
      bool found = false;
      int index = -1;
      T it = default(T);
      for(int i = 0; i < Count; ++i) {
        if(found) {
          Array[i] = Array[i + 1];
        } else {
          if(Array[i].Equals(item)) {
            it = Array[i];
            Array[i] = Array[i + 1];
            found = true;
            index = i;
            Count--;
          }
        }
      }
      if(found && OnRemove != null) OnRemove(item);
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
      if(OnRemove != null) OnRemove(item);
      return item;
    }
    
    /// <summary>
    /// Does the array contain the specified item?
    /// </summary>
    /// <param name="item">T.</param>
    public bool Contains(T item) {
      for(int i = Count-1; i >= 0; --i) {
        if(Array[i].Equals(item)) {
          return true;
        }
      }
      return false;
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
          if(OnRemove != null) {
            OnRemove(before);
          }
          if(OnAdd != null) OnAdd(after);
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
          if(OnRemove != null) {
            OnRemove(before);
          }
          if(OnAdd != null) OnAdd(after);
        }
      }
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
    /// Reset this instance while not nulling references.
    /// </summary>
    public void Reset() {
      if(OnRemove != null) {
        foreach(T item in this) OnRemove(item);
      }
      Count = 0;
    }
    
    /// <summary>
    /// Reset this instance and the array within.
    /// </summary>
    public void Clear() {
      Reset();
      Array = new T[Capacity];
    }
    
    /// <summary>
    /// Releases all resource used by the <see cref="sp.tools.ArrayExtend`1"/> object.
    /// </summary>
    public void Dispose() {
      Count = -1;
      Array = null;
      OnAdd = null;
      OnRemove = null;
    }
    
    /// <summary>
    /// Manually resize the contained array.
    /// If count is greater than the array size, it is reduced.
    /// </summary>
    public void Resize(int size) {
      Capacity = size;
      System.Array.Resize<T>(ref Array, Capacity);
      if(Count > Capacity) Count = Capacity;
    }
    
    /// <summary>
    /// Sort using the specified comparer.
    /// </summary>
    /// <param name="comparer">Comparer to use to sort the array.</param>
    public void Sort(IComparer<T> comparer) {
      T swap;
      for(int i = Count-2; i >= 0; --i) {
        if(comparer.Compare(Array[i], Array[i+1]).Equals(-1)) {
          swap = Array[i];
          Array[i] = Array[i+1];
          Array[i+1] = swap;
        }
      }
    }
    
    public T[] GetArray() {
      T[] a = new T[Count];
      for(int i = a.Length-1; i >= 0; --i) {
        a[i] = Array[i];
      }
      return a;
    }
    
    /// <summary>
    /// Gets or sets the array item at the specified index.
    /// </summary>
    /// <param name="index">Index to access.</param>
    public T this[int index] {
      get {
        return Array[index];
      }
      set {
        Array[index] = value;
      }
    }
    
    public IEnumerator<T> GetEnumerator() {
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
          return _current;
        }
      }
      
      object IEnumerator.Current {
        get {
          return _current;
        }
      }
      
      //-------------------------------------------//
      
      private T _current;
      private int _index;
      
      private ArrayAir<T> _rig;
      
      //-------------------------------------------//
      
      public Enumerator(ArrayAir<T> rig) {
        _rig = rig;
        _index = -1;
      }
      
      public void Dispose() {
        _rig = null;
      }
      
      public bool MoveNext() {
        if(++_index < _rig.Count) {
          _current = _rig[_index];
          return true;
        }
        return false;
      }
      
      public void Reset() {
        _index = -1;
      }
      
      //-------------------------------------------//
      
    }
        
  }

}