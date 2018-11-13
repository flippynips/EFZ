/*
 * User: Joshua
 * Date: 26/06/2016
 * Time: 1:46 AM
 */
using System;
using System.Threading.Tasks;

namespace Efz.Tools {
  
  /// <summary>
  /// Convenience wrapper for automatically maintaining weakly referenced object.
  /// </summary>
  public class WeakReferencer<T> where T : class {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Get or set the referenced item. Best to get and keep local reference.
    /// </summary>
    public T Item {
      get {
        T reference;
        if(_set && _item.TryGetTarget(out reference)) return reference;
        reference = GetItem.Run();
        if (reference != null) {
          if(_set) {
            _item.SetTarget (reference);
          } else {
            _set = true;
            _item = new WeakReference<T>(reference);
          }
        }
        return reference;
      }
      set {
        if(_set) _item.SetTarget(value);
        _item = new WeakReference<T>(value);
        _set = true;
      }
    }
    
    /// <summary>
    /// Method used to retrieve a new reference to an item.
    /// </summary>
    public IFunc<T> GetItem;
    
    //-------------------------------------------//
    
    protected WeakReference<T> _item;
    protected bool _set;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Construct a weak referencer with the function to refresh an item reference.
    /// </summary>
    public WeakReferencer(IFunc<T> getItem = null) {
      GetItem = getItem;
      _set = false;
    }
    
    /// <summary>
    /// Construct a new WeakReferencer with an initial item.
    /// </summary>
    public WeakReferencer(IFunc<T> getItem, T item) {
      GetItem = getItem;
      _item = new WeakReference<T>(item);
      _set = true;
    }
    
    /// <summary>
    /// Convenience function for more compact constructor.
    /// </summary>
    public WeakReferencer(Func<T> getItem) {
      GetItem = new FuncSet<T>(getItem);
      _set = false;
    }
    
    /// <summary>
    /// Asynchronously fetch the item.
    /// </summary>
    public async Task<T> Fetch() {
      T reference;
      if(_set && _item.TryGetTarget(out reference)) {
        return reference;
      }
      reference = await GetItem.RunAsync();
      if (reference != null) {
        if(_set) {
          _item.SetTarget (reference);
        } else {
          _set = true;
          _item = new WeakReference<T>(reference);
        }
      }
      return reference;
    }
    
    /// <summary>
    /// Implicitly convert to the type of item.
    /// </summary>
    public static implicit operator T(WeakReferencer<T> weakReference) {
      return weakReference.Item;
    }
    
    //-------------------------------------------//
    
    
  }
}
