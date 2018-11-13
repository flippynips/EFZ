/*
 * User: Joshua
 * Date: 1/11/2016
 * Time: 12:59 AM
 */
using System;
using System.Collections.Generic;

using Efz.Threading;

namespace Efz.Tools {
  
  /// <summary>
  /// Hander of request paths. More specific paths are prioritised.
  /// </summary>
  public class PathResolver<T> {
    
    //----------------------------------//
    
    /// <summary>
    /// Callback when the path isn't resolved.
    /// </summary>
    public IAction<string, T> DefaultCallback;
    
    //----------------------------------//
    
    /// <summary>
    /// Path definitions and callbacks with sub-paths.
    /// </summary>
    protected Dictionary<string, IAction<string, T>> _routes;
    /// <summary>
    /// Lock used for external access to the path resolver.
    /// </summary>
    protected Lock _lock;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a new path resolver.
    /// </summary>
    public PathResolver() {
      _routes = new Dictionary<string, IAction<string, T>>();
      _lock = new Lock();
    }
    
    /// <summary>
    /// Initialize a new path resolver.
    /// </summary>
    public PathResolver(Action<string,T> defaultCallback) : this() {
      DefaultCallback = new Act<string,T>(defaultCallback);
    }
    
    /// <summary>
    /// Initialize a new path resolver.
    /// </summary>
    public PathResolver(IAction<string,T> defaultCallback) : this() {
      DefaultCallback = defaultCallback;
    }
    
    /// <summary>
    /// Add a root path and callback for the sub path.
    /// </summary>
    public void Add(string path, IAction<string,T> callback) {
      _lock.Take();
      if(path[0] == Chars.ForwardSlash) {
        path = path.Substring(1);
      }
      if(_routes.ContainsKey(path)) throw new Exception("Paths cannot be defined twice.");
      
      _routes.Add(path, callback);
      
      _lock.Release();
    }
    
    /// <summary>
    /// Add a root path and callback for the sub path.
    /// </summary>
    public void Add(string path, Action<string,T> callback) {
      _lock.Take();
      if(path[0] == Chars.ForwardSlash) {
        _routes.Add(path.Substring(1), new ActionPop<string, T>(callback));
      } else {
        _routes.Add(path, new ActionPop<string, T>(callback));
      }
      _lock.Release();
    }
    
    /// <summary>
    /// Remove a path and method callback from the resolver.
    /// </summary>
    public void Remove(string path) {
      _lock.Take();
      if(path[0] == Chars.ForwardSlash) {
        _routes.Remove(path.Substring(1));
      } else {
        _routes.Remove(path);
      }
      _lock.Release();
    }
    
    /// <summary>
    /// Resolve the specified path.
    /// </summary>
    public void Resolve(string path, T value) {
      
      // get the index of the start of the params
      int subPathIndex = path.IndexOf(Chars.Question);
      string paramsSegment = null;
      string[] segments;
      
      // was the params index found?
      if(subPathIndex == -1) {
        
        // split the path into segments
        segments = path.Split(Chars.ForwardSlash);
        
      } else {
        
        // split the path into path and params
        Struct<string, string> sections = path.Split(subPathIndex, 0, -1);
        
        // split the path into segments
        segments = sections.ArgA.Split(Chars.ForwardSlash);
        
        // persist the params segment
        paramsSegment = sections.ArgB;
        
      }
      
      int index = 0;
      if(segments[index] == string.Empty) ++index;
      
      IAction<string,T> callback = null;
      var builder = StringBuilderCache.Get();
      bool first = true;
      subPathIndex = 0;
      
      // iterate
      while(index < segments.Length) {
        if(first) first = false;
        else builder.Append(Chars.ForwardSlash);
        
        builder.Append(segments[index]);
        ++index;
        // does the path have a callback?
        IAction<string,T> callbackAction;
        if(_routes.TryGetValue(builder.ToString(), out callbackAction)) {
          callback = callbackAction;
          subPathIndex = index;
        }
        
      }
      
      // get the sub path
      builder.Length = 0;
      first = true;
      while(subPathIndex < segments.Length) {
        if(first) first = false;
        else builder.Append(Chars.ForwardSlash);
        
        builder.Append(segments[subPathIndex]);
        
        ++subPathIndex;
      }
      
      // append the params segment
      builder.Append(paramsSegment);
      
      // has the callback been set?
      if(callback == null) {
        // no, has the default callback been set?
        if(DefaultCallback == null) {
          // no, log the error
          Log.Error("No default callback set. Request for '"+path+"' went unresolved.");
          return;
        }
        // set the default callback
        callback = DefaultCallback;
      }
      
      _lock.Take();
      callback.ArgA = StringBuilderCache.SetAndGet(builder);
      callback.ArgB = value;
      callback.Run();
      _lock.Release();
    }
    
    /// <summary>
    /// Resolve the specified path with the specified callback. If the path has a defined
    /// callback, it will be trimmed to a sub-path.
    /// </summary>
    public void Resolve(string path, IAction<string> onResolved) {
      // split the path into sections
      string[] sections = path.Split(Chars.ForwardSlash);
      
      int index = 0;
      if(sections[index] == string.Empty) ++index;
      
      int subPathIndex = 0;
      bool first = true;
      var builder = StringBuilderCache.Get();
      
      // iterate
      while(index < sections.Length) {
        if(first) first = false;
        else builder.Append(Chars.ForwardSlash);
        
        builder.Append(sections[index]);
        ++index;
        // does the path contain a callback? yes, update the sub path
        if(_routes.ContainsKey(builder.ToString())) subPathIndex = index;
        
      }
      
      // get the sub path
      builder.Length = 0;
      first = true;
      while(subPathIndex < sections.Length) {
        if(first) first = false;
        else builder.Append(Chars.ForwardSlash);
        
        builder.Append(sections[subPathIndex]);
        
        ++subPathIndex;
      }
      
      onResolved.ArgA = StringBuilderCache.SetAndGet(builder);
      onResolved.Run();
      
    }
    
    //----------------------------------//
    
  }
  
}
