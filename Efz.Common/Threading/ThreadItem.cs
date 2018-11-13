using System.Collections.Generic;
using System.Threading;

namespace Efz.Threading {
  
  /// <summary>
  /// Structure containing some generic threadsafe handling of a generic item.
  /// </summary>
  public class ThreadItem<T> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Get or set the item associated with the current thread.
    /// </summary>
    public T Item {
      get {
        // get the item associated with the current thread
        return _items[Thread.CurrentThread.ManagedThreadId];
      }
      set {
        // set the item associated with the current thread
        _items[Thread.CurrentThread.ManagedThreadId] = value;
      }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Item map between threads and items.
    /// </summary>
    protected Dictionary<int,T> _items;
    /// <summary>
    /// Optional function to retrieve a new item.
    /// </summary>
    protected FuncSet<T> _getItem;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initializes a new threadsafe value with an initial default value.
    /// </summary>
    public ThreadItem() {
      _items = new Dictionary<int, T>();
      Reset();
      ThreadHandle.Handles.Take();
      ThreadHandle.Handles.Item.OnAdd += OnAdd;
      ThreadHandle.Handles.Item.OnRemove += OnRemove;
      ThreadHandle.Handles.Release();
      _items[Thread.CurrentThread.ManagedThreadId] = default(T);
    }
    
    /// <summary>
    /// Initializes a new threadsafe value with a function to get the initial item value.
    /// </summary>
    public ThreadItem(Func<T> getItem) {
      _getItem = new FuncSet<T>(getItem);
      _items = new Dictionary<int, T>();
      Reset();
      ThreadHandle.Handles.Take();
      ThreadHandle.Handles.Item.OnAdd += OnAdd;
      ThreadHandle.Handles.Item.OnRemove += OnRemove;
      ThreadHandle.Handles.Release();
      _items[Thread.CurrentThread.ManagedThreadId] = getItem == null ? default(T) : getItem();
    }
    
    /// <summary>
    /// Initializes a new threadsafe value as either default or with a function to get the item.
    /// </summary>
    public ThreadItem(FuncSet<T> getItem = null) {
      _getItem = getItem;
      _items = new Dictionary<int,T>();
      Reset();
      ThreadHandle.Handles.Take();
      ThreadHandle.Handles.Item.OnAdd += OnAdd;
      ThreadHandle.Handles.Item.OnRemove += OnRemove;
      ThreadHandle.Handles.Release();
      if(_getItem == null) _items[Thread.CurrentThread.ManagedThreadId] = default(T);
      else _items[Thread.CurrentThread.ManagedThreadId] = _getItem.Run();
    }
    
    /// <summary>
    /// Set all threaded items.
    /// </summary>
    public void Reset() {
      ThreadHandle.Handles.Take();
      try {
        if(_getItem == null) {
          foreach(ThreadHandle handle in ThreadHandle.Handles.Item) {
            _items[handle.Id] = default(T);
          }
        } else {
          foreach(ThreadHandle handle in ThreadHandle.Handles.Item) {
            _items[handle.Id] = _getItem.Run();
          }
        }
      } finally {
        ThreadHandle.Handles.Release();
      }
    }
    
    /// <summary>
    /// Synchonize all threaded items.
    /// </summary>
    public void Synchonize(T item) {
      foreach(ThreadHandle handle in ThreadHandle.Handles.TakeItem()) {
        _items[handle.Id] = item;
      }
      ThreadHandle.Handles.Release();
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// On a new thread handle.
    /// </summary>
    protected void OnAdd(ThreadHandle handle) {
      _items[handle.Id] = _getItem == null ? default(T) : _getItem.Run();
    }
    
    /// <summary>
    /// On a thread handle being removed.
    /// </summary>
    protected void OnRemove(ThreadHandle handle) {
      _items.Remove(handle.Id);
    }
    
  }

}