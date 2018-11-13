
namespace Efz.Threading {
  
  /// <summary>
  /// Structure containing some generic threadsafe handling of a generic item.
  /// </summary>
  public class Shared<T> : LockShared {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Gets of sets the shared item.
    /// LOCK THE ITEM BEFORE ACCESSING THIS.
    /// </summary>
    public T Item {
      get {
        #if DEBUG
        if(!Locked) throw new System.Exception("Accessed shared item is not locked");
        #endif
        return _item;
      }
      set {
        #if DEBUG
        if(!Locked) throw new System.Exception("Accessed shared item is not locked");
        #endif
        _item = value;
      }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Non threadsafe item access. Lock access before accessing and Unlock after accessing this.
    /// </summary>
    protected T _item;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initializes a new threadsafe value.
    /// </summary>
    public Shared() {
      _item = default(T);
    }
    
    /// <summary>
    /// Initializes a new threadsafe value of the item specified.
    /// </summary>
    public Shared(T item) {
      _item = item;
    }
    
    /// <summary>
    /// Runs an action when the item becomes available.
    /// </summary>
    public void TryItem(IAction<T> onAvailable) {
      onAvailable.ArgA = _item;
      TryLock(onAvailable);
    }
    
    /// <summary>
    /// Ensures the current value is only accessible in the current thread.
    /// </summary>
    public T TakeItem() {
      Take();
      return _item;
    }
    
    /// <summary>
    /// Perform a single method call on the item then release it automatically.
    /// </summary>
    public void ItemAction(System.Action<T> action) {
      action(TakeItem());
      Release();
    }
    
    /// <summary>
    /// Ensures the current value is accessible to all threads and sets the value.
    /// </summary>
    public void ReleaseItem(T item) {
      _item = item;
      Release();
    }
    
    //-------------------------------------------//
    
  }

}