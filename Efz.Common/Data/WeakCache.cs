/*
 * User: FloppyNipples
 * Date: 19/02/2017
 * Time: 22:45
 */
using System;

using System.Threading;
using Efz.Tools;

namespace Efz.Data {
  
  /// <summary>
  /// Cache of pre-built components of a site.
  /// </summary>
  public class WeakCache<T> where T : class {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Timeout of the cached element.
    /// </summary>
    public int CacheTime;
    
    /// <summary>
    /// Get the cached item. Reference will be refreshed if timed out
    /// or weak reference lost.
    /// </summary>
    public T Item {
      get {
        if(Time.Milliseconds > _updateTime || CacheTime == 0) {
          if(Interlocked.CompareExchange(ref CacheTime, 0, -1) > 0) {
            _updateTime = Time.Milliseconds + CacheTime;
          }
          _reference.Item = _reference.GetItem.Run();
        }
        return _reference.Item;
      }
    }
    
    /// <summary>
    /// Function used to retrieve the item.
    /// </summary>
    public IFunc<T> GetItem {
      get {
        return _reference.GetItem;
      }
      set {
        _reference.GetItem = value;
        _updateTime = 0;
      }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Weak reference to the entity.
    /// </summary>
    protected WeakReferencer<T> _reference;
    /// <summary>
    /// Time of the next update.
    /// </summary>
    protected long _updateTime;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Construct a timeout cache, optionally with a cache timeout.
    /// </summary>
    public WeakCache(Func<T> getItem, int cacheTime = -1) {
      CacheTime = cacheTime;
      if(cacheTime == -1) _updateTime = long.MaxValue;
      _reference = new WeakReferencer<T>(getItem);
    }
    
    /// <summary>
    /// Construct a timeout cache, optionally with a cache timeout.
    /// </summary>
    public WeakCache(IFunc<T> getItem, int cacheTime = -1) {
      CacheTime = cacheTime;
      if(cacheTime == -1) _updateTime = long.MaxValue;
      _reference = new WeakReferencer<T>(getItem);
    }
    
    /// <summary>
    /// Cause the cached item to be refreshed on the next request.
    /// </summary>
    public void Invalidate() {
      if(Interlocked.CompareExchange(ref CacheTime, -1, 0) != 0) {
        _updateTime = long.MinValue;
      }
    }
    
    //-------------------------------------------//
    
  }
    
}
