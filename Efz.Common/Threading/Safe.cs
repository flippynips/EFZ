/*
 * User: FloppyNipples
 * Date: 4/11/2017
 * Time: 12:25 PM
 */
using System;

namespace Efz.Threading {
  
  /// <summary>
  /// Provides threadsafe setting and getting for value types.
  /// </summary>
  public class Safe<T> {
    
    //-------------------------------//
    
    /// <summary>
    /// Get or set the value in the safe.
    /// </summary>
    public T Value {
      get {
        _lock.Take();
        var val = _value;
        _lock.Release();
        return val;
      }
      set {
        _lock.Take();
        _value = value;
        _lock.Release();
      }
    }
    
    //-------------------------------//
    
    /// <summary>
    /// Item stored in the safe.
    /// </summary>
    protected T _value;
    
    /// <summary>
    /// Lock used for safe access.
    /// </summary>
    protected Lock _lock;
    
    //-------------------------------//
    
    /// <summary>
    /// Create a new lock.
    /// </summary>
    public Safe(Lock useLock = null) {
      _lock = useLock ?? new Lock();
    }
    
    public static implicit operator T(Safe<T> safe) {
      safe._lock.Take();
      var val = safe._value;
      safe._lock.Release();
      return val;
    }
    
    //-------------------------------//
    
  }
  
}
