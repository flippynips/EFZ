using System;

namespace Efz.Threading {
  
  /// <summary>
  /// Structure containing some generic threadsafe handling of a generic item.
  /// </summary>
  public class LockItem<T> where T : new() {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Get or set the value in a threadsafe manner.
    /// </summary>
    public T Item {
      get {
        #if DEBUG
        if(!Locker.Locked) throw new Exception("Shared item accessed is not locked");
        #endif
        return _item;
      }
      set {
        #if DEBUG
        if(!Locker.Locked) throw new Exception("Shared item accessed is not locked");
        #endif
        _item = value;
      }
    }
    
    /// <summary>
    /// The current thread lock instance.
    /// </summary>
    public Lock Locker;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Non threadsafe item access. Run CheckOut before and CheckIn after accessing this.
    /// </summary>
    protected T _item;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initializes a new threadsafe value.
    /// </summary>
    public LockItem() {
      _item = new T();
      Locker = new Lock();
    }
    
    /// <summary>
    /// Initializes a new threadsafe value with the gate specified.
    /// </summary>
    public LockItem(Lock @lock) {
      _item = new T();
      Locker = @lock;
    }
    
    /// <summary>
    /// Initializes a new threadsafe value of the item specified.
    /// </summary>
    public LockItem(T item) {
      _item = item;
      Locker = new Lock();
    }
    
    /// <summary>
    /// Initializes a new threadsafe value of the item specified.
    /// </summary>
    public LockItem(Lock @lock, T item) {
      _item = item;
      Locker = @lock;
    }
    
    /// <summary>
    /// Ensures the current value is only accessible in the current thread.
    /// </summary>
    public T TakeItem() {
      Locker.Take();
      return _item;
    }
    
    /// <summary>
    /// Take the lock.
    /// </summary>
    public void Take() {
      Locker.Take();
    }
    
    /// <summary>
    /// Ensures the current value is accessible to all threads.
    /// </summary>
    public void Release() {
      Locker.Release();
    }
    
    /// <summary>
    /// Ensures the current value is accessible to all threads and sets the value.
    /// </summary>
    public void Release(T item) {
      _item = item;
      Locker.Release();
    }
    
    /// <summary>
    /// Implivitly convert to an item locker.
    /// </summary>
    static public implicit operator LockItem<T>(T item) {
      return new LockItem<T>(item);
    }
    
    //-------------------------------------------//
    
  }

}