/*
 * User: FloppyNipples
 * Date: 09/05/2017
 * Time: 00:50
 */
using System;
using System.Collections.Generic;

namespace Efz.Tools {
  
  /// <summary>
  /// Comparer that can be generically defined.
  /// </summary>
  public class InlineComparer<T> : IEqualityComparer<T> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    protected Func<T,T,bool> _isEqual;
    protected Func<T,int> _getHashcode;
    
    //-------------------------------------------//
    
    public InlineComparer(Func<T,T,bool> isEqual = null, Func<T,int> getHashcode = null) {
      _isEqual = isEqual;
      _getHashcode = getHashcode;
    }
    
    public bool Equals(T itemX, T itemY) {
      if(_isEqual == null) return Equals(itemX, itemY);
      return _isEqual(itemX, itemY);
    }
    
    public int GetHashCode(T item) {
      if (_getHashcode == null) return item.GetHashCode();
      return _getHashcode(item);
    }
    
    //-------------------------------------------//
    
  }
  
}
