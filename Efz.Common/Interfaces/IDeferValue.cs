using Efz.Threading;
using Efz.Tools;

namespace Efz {
  
  /// <summary>
  /// Defines a resource of which retrieval may be immediate or deferred.
  /// </summary>
  public interface IGetValue {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Specify an Action that is called.
    /// </summary>
    void Get(IAction onValue, Needle needle = null);
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Defines a resource of which retrieval may be immediate or deferred.
  /// </summary>
  public interface IGetValue<A> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Specify an Action that is called with the desired class.
    /// </summary>
    void Get(IAction<A> onValue, Needle needle = null);
    
    /// <summary>
    /// Get the value directly.
    /// </summary>
    void Get(out Teple<LockShared, A> value);
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Defines a resource of which retrieval may be immediate or deferred.
  /// </summary>
  public interface IGetValue<A,B> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Specify an Action that is called with the desired class.
    /// </summary>
    void Get(IAction<A,B> onValue, Needle needle = null);
    
    /// <summary>
    /// Get the value directly.
    /// </summary>
    void Get(out Teple<LockShared, A, B> value);
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Defines a resource of which retrieval may be immediate or deferred.
  /// </summary>
  public interface IGetValue<A,B,C> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Specify an Action that is called with the desired class.
    /// </summary>
    void Get(IAction<A,B,C> onValue, Needle needle = null);
    
    /// <summary>
    /// Get the value directly.
    /// </summary>
    void Get(out Teple<LockShared, A, B, C> value);
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Defines a resource of which retrieval may be immediate or deferred.
  /// </summary>
  public interface IGetValue<A,B,C,D> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Specify an Action that is called with the desired class.
    /// </summary>
    void Get(IAction<A,B,C,D> onValue, Needle needle = null);
    
    /// <summary>
    /// Get the value directly.
    /// </summary>
    void Get(out Teple<LockShared, A, B, C, D> value);
    
    //-------------------------------------------//
    
  }
  
  /// <summary>
  /// Defines a resource of which retrieval may be immediate or deferred.
  /// </summary>
  public interface IGetValue<A,B,C,D,E> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Specify an Action that is called with the desired class.
    /// </summary>
    void Get(IAction<A,B,C,D,E> onValue, Needle needle = null);
    
    /// <summary>
    /// Get the value directly.
    /// </summary>
    void Get(out Teple<LockShared, A, B, C, D, E> value);
    
    //-------------------------------------------//
    
  }
  
}