/*
 * User: Joshua
 * Date: 1/11/2016
 * Time: 12:59 AM
 */
using System;
using System.Collections.Generic;

using Efz.Collections;
using Efz.Threading;

namespace Efz.Web {
  
  /// <summary>
  /// Hander of web request paths and method types. More specific paths are prioritised.
  /// </summary>
  public class HttpRequestResolver {
    
    //----------------------------------//
    
    /// <summary>
    /// Method type and callback combination.
    /// </summary>
    protected class Route {
      
      /// <summary>
      /// Method type flag/s.
      /// </summary>
      public readonly HttpMethod Methods;
      /// <summary>
      /// Callback action.
      /// </summary>
      public readonly ActionPop<string, HttpRequest> OnResolve;
      
      public Route(HttpMethod methods, IAction<string, HttpRequest> callback) {
        Methods = methods;
        OnResolve = new ActionPop<string, HttpRequest>(callback);
      }
      
      /// <summary>
      /// Run the route callback.
      /// </summary>
      public void Run(string path, HttpRequest request) {
        OnResolve.Run(path, request);
      }
      
      public override int GetHashCode() {
        return Methods.GetHashCode();
      }
      
      // disable once MemberHidesStaticFromOuterClass
      public override bool Equals(object obj) {
        var route = obj as Route;
        return route != null && Methods.Is(route.Methods);
      }
      
    }
    
    /// <summary>
    /// Callback when the path isn't resolved.
    /// </summary>
    public ActionPop<string, HttpRequest> DefaultCallback;
    
    //----------------------------------//
    
    /// <summary>
    /// Path definitions and callbacks with sub-paths.
    /// </summary>
    protected Dictionary<string, ArrayRig<Route>> _routes;
    /// <summary>
    /// Lock used for external access to the path resolver.
    /// </summary>
    protected Lock _lock;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a new path resolver.
    /// </summary>
    public HttpRequestResolver() {
      _routes = new Dictionary<string, ArrayRig<Route>>();
      _lock = new Lock();
    }
    
    /// <summary>
    /// Initialize a new path resolver.
    /// </summary>
    public HttpRequestResolver(Action<string, HttpRequest> defaultCallback) : this() {
      if(defaultCallback == null) throw new ArgumentNullException("defaultCallback");
      DefaultCallback = new ActionPop<string, HttpRequest>(defaultCallback);
    }
    
    /// <summary>
    /// Initialize a new path resolver.
    /// </summary>
    public HttpRequestResolver(IAction<string, HttpRequest> defaultCallback) : this() {
      if(defaultCallback == null) throw new ArgumentNullException("defaultCallback");
      DefaultCallback = new ActionPop<string, HttpRequest>(defaultCallback);
    }
    
    /// <summary>
    /// Add the specified routine as a resolver of requests.
    /// </summary>
    public void Add(HttpRoutine routine) {
      // skip null paths
      if(routine.Path == null || routine.Methods == HttpMethod.None) return;
      Add(routine.Path, routine.Methods, routine.OnRequest);
    }
    
    /// <summary>
    /// Add a root path and callback for the sub path.
    /// </summary>
    public void Add(string path, HttpMethod methods, Action<string, HttpRequest> callback) {
      Add(path, methods, new ActionPop<string, HttpRequest>(callback));
    }
    
    /// <summary>
    /// Add a root path and callback for the sub path.
    /// </summary>
    public void Add(string path, HttpMethod methods, IAction<string, HttpRequest> callback) {
      if(string.IsNullOrEmpty(path)) throw new InvalidOperationException("Null or empty paths aren't allowed. Use the DefaultCallback field.");
      
      path = path.ToLowercase();
      if(path[0] == Chars.ForwardSlash) path = path.Substring(1);
      
      _lock.Take();
      ArrayRig<Route> routes;
      if(_routes.TryGetValue(path, out routes)) {
        foreach(var route in routes) {
          if(route.Methods.Is(methods)) throw new Exception("Route methods cannot be defined twice.");
        }
        routes.Add(new Route(methods, callback));
      } else {
        routes = new ArrayRig<Route>();
        routes.Add(new Route(methods, callback));
        _routes.Add(path, routes);
      }
      _lock.Release();
    }
    
    /// <summary>
    /// Remove a path and method callback from the resolver.
    /// </summary>
    public void Remove(string path, HttpMethod methods) {
      _lock.Take();
      if(path[0] == Chars.ForwardSlash) path = path.Substring(1);
      ArrayRig<Route> routes;
      if(_routes.TryGetValue(path, out routes)) {
        routes.RemoveSingle(r => r.Methods.Is(methods));
      }
      _lock.Release();
    }
    
    /// <summary>
    /// Resolve the specified path.
    /// </summary>
    public void Resolve(string path, HttpMethod method, HttpRequest value) {
      
      path = path.ToLowercase();
      
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
      
      Route route = null;
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
        ArrayRig<Route> routes;
        if(_routes.TryGetValue(builder.ToString(), out routes)) {
          // yes, check the method
          foreach(var r in routes) {
            if(r.Methods.Is(method)) {
              // persist the longest path with callback
              route = r;
              subPathIndex = builder.Length;
            }
          }
        }
        
      }
      
      // get the sub path
      string subPath;
      if(subPathIndex == 0) subPath = builder.ToString();
      else if(builder.Length == subPathIndex) subPath = string.Empty;
      else subPath = builder.ToString(subPathIndex + 1, builder.Length - subPathIndex - 1);
      
      StringBuilderCache.Set(builder);
      
      // has the callback been set?
      if(route == null) {
        
        // no, has the default callback been set?
        if(DefaultCallback == null) {
          // no, log the error
          Log.Error("No default callback set. Request for '"+path+"' went unresolved.");
          return;
        }
        
        // set the default callback
        DefaultCallback.Run(subPath, value);
      } else {
        
        // run route callback
        route.Run(subPath, value);
      }
      
    }
    
    /// <summary>
    /// Resolve the specified path with the specified callback. If the path has a defined
    /// callback, it will be trimmed to a sub-path.
    /// </summary>
    public void Resolve(string path, HttpMethod method, IAction<string> onResolved) {
      
      path = path.ToLowercase();
      
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
        ArrayRig<Route> routes;
        if(_routes.TryGetValue(builder.ToString(), out routes)) {
          foreach(var route in routes) {
            if(route.Methods.Is(method)) subPathIndex = index;
          }
        }
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
