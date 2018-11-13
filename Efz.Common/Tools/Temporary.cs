using System;
using System.Collections.Generic;

namespace Efz {
  
  /// <summary>
  /// Functions for storing and accessing references temporarily without
  /// using a local variable.
  /// </summary>
  public static class Temporary {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Items saved by key.
    /// </summary>
    private static Dictionary<string, object> _byKey;
    /// <summary>
    /// Items saved by type.
    /// </summary>
    private static Dictionary<Type, object> _byType;
    
    //-------------------------------------------//
    
    static Temporary() {
      _byKey = new Dictionary<string, object>();
      _byType = new Dictionary<Type, object>();
    }
    
    /// <summary>
    /// Save an item reference by key.
    /// </summary>
    public static void Set(string key, object item) {
      _byKey[key] = item;
    }
    
    /// <summary>
    /// Get an item reference by key. Does not remove the item from the temporary store.
    /// </summary>
    public static void Get<T>(string key, out T item) {
      item = (T)_byKey[key];
    }
    
    /// <summary>
    /// Get an item reference by key. Removes the item from the temporary store.
    /// </summary>
    public static void GetAndClear<T>(string key, out T item) {
      item = (T)_byKey[key];
      _byKey.Remove(key);
    }
    
    
    /// <summary>
    /// Save an item reference by type. Safe only if the type is known
    /// to be unique in the temporary store.
    /// </summary>
    public static void Set<T>(T item) {
      _byType[typeof(T)] = item;
    }
    
    /// <summary>
    /// Get an item reference by type. Does not remove the item from the temporary store.
    /// </summary>
    public static void Get<T>(out T item) {
      item = (T)_byType[typeof(T)];
    }
    
    /// <summary>
    /// Get an item reference by type. Removes the item from the temporary store.
    /// </summary>
    public static void GetAndClear<T>(out T item) {
      item = (T)_byType[typeof(T)];
      _byType.Remove(typeof(T));
    }
    
    
    //-------------------------------------------//
    
  }
  
}
