using System;
using Efz.Threading;

namespace Efz.Collections {
  
  /// <summary>
  /// Structure containing threadsafe handling of a generic, retrievable property.
  /// </summary>
  public class Pocket<A> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Gets the item.
    /// </summary>
    public A Get {
      get {
        _lock.Take();
        var item = _toGet();
        _lock.Release();
        return item;
      }
    }
    
    /// <summary>
    /// Get or set the method that retrieves the value.
    /// </summary>
    public Func<A> Function {
      get {
        return _toGet;
      }
      set {
        _lock.Take();
        _toGet = value;
        _lock.Release();
      }
    }
    
    //-------------------------------------------//
    
    private LockShared _lock;
    private Func<A> _toGet;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initializes a new threadsafe value.
    /// </summary>
    public Pocket(Func<A> toGet) {
      _toGet = toGet;
      _lock = new LockShared();
    }
    
    /// <summary>
    /// Try and get the pocket value.
    /// </summary>
    public void TryGet(IAction<A> onGet) {
      _lock.TryLock(new Act<IAction<A>>(OnAvailable, onGet));
    }
    
    public static implicit operator A(Pocket<A> value) {
      return value.Get;
    }
    
    //-------------------------------------------//
    
    protected void OnAvailable(IAction<A> onGet) {
      onGet.ArgA = _toGet();
      onGet.Run();
    }
    
  }

}