/*
 * User: FloppyNipples
 * Date: 26/02/2017
 * Time: 19:09
 */
using System;
using System.Collections.Generic;

using System.IO;
using System.Threading.Tasks;
using Efz.Data;
using Efz.Threading;

namespace Efz.Web.Display {
  
  /// <summary>
  /// Management of multiple elements.
  /// </summary>
  public class Elements {
    
    //----------------------------------//
    
    /// <summary>
    /// Local path where html elements can be found relative to.
    /// </summary>
    public string Path;
    
    //----------------------------------//
    
    /// <summary>
    /// Collection of keys to element links.
    /// </summary>
    protected Dictionary<string, ElementLink> _elements;
    /// <summary>
    /// Element links.
    /// </summary>
    protected Dictionary<string, ElementLink> _paths;
    /// <summary>
    /// Lock used for external access.
    /// </summary>
    protected Lock _lock;
    
    /// <summary>
    /// Watcher of the file system.
    /// </summary>
    protected FileSystemWatcher _watcher;
    
    //----------------------------------//
    
    /// <summary>
    /// Init a new page manager local to the specified path.
    /// </summary>
    public Elements(string path) {
      // persist the path
      Path = path;
      
      // init the lock
      _lock = new Lock();
      // init the collection to be used for keys and paths
      _elements = new Dictionary<string, ElementLink>();
      _paths = new Dictionary<string, ElementLink>();
      
      _watcher = new FileSystemWatcher(Path, "*.html");
      _watcher.IncludeSubdirectories = true;
      _watcher.NotifyFilter = NotifyFilters.DirectoryName |
        NotifyFilters.FileName |
        NotifyFilters.LastAccess |
        NotifyFilters.LastWrite |
        NotifyFilters.Size;
      _watcher.EnableRaisingEvents = true;
      _watcher.Changed += OnChanged;
      
    }
    
    /// <summary>
    /// Get an element by key.
    /// </summary>
    public void Get(string key, Action<Element> onGet) {
      Get(key, new ActionSet<Element>(onGet));
    }
    
    /// <summary>
    /// Get an element by key. The callback may be run with a 'Null' argument
    /// if the key isn't found in the lookup.
    /// </summary>
    public void Get(string key, IAction<Element> onGet) {
      
      ElementLink link;
      
      // take the lock
      _lock.Take();
      // get the link that defines the retrieval of the element
      if(!_elements.TryGetValue(key, out link)) {
        _lock.Release();
        // NOT FOUND
        //Log.Error("Unspecified key in elements '"+key+"'.");
        // run callback with 'Null'
        onGet.ArgA = null;
        onGet.Run();
        return;
      }
      // release the lock
      _lock.Release();
      
      link.Get(onGet);
      
    }
    
    /// <summary>
    /// Get an element by key. The callback may be run with a 'Null' argument
    /// if the key isn't found in the lookup.
    /// </summary>
    public async Task<Element> Get(string key) {
      
      // get the element link from the cache
      ElementLink link;
      
      // take the lock
      _lock.Take();
      
      // get the link that defines the retrieval of the element
      if(!_elements.TryGetValue(key, out link)) {
        _lock.Release();
        // NOT FOUND
        //Log.Error("Unspecified key in elements '"+key+"'.");
        // run callback with 'Null'
        return null;
      }
      
      // get the element
      var element = await link.GetAsync();
      
      // release the lock
      _lock.Release();
      
      return element;
    }
    
    /// <summary>
    /// Add or set an element.
    /// </summary>
    public void Set(string key, string path, bool buildNow = true) {
      path = Fs.Combine(Path, path);
      _lock.Take();
      var link = _elements[key] = new ElementLink(path);
      _paths[path] = link;
      _lock.Release();
      if(buildNow) link.Build();
    }
    
    /// <summary>
    /// Add or set an element. The cache time is a number of milliseconds that the result
    /// of the 'onBuild' action can be cached for.
    /// </summary>
    public void Set(string key, string path, Action<Element> onBuild, int cacheTime = -1, bool buildNow = true) {
      path = Fs.Combine(Path, path);
      _lock.Take();
      var link = _elements[key] = new ElementLink(path, new ActionSet<Element>(onBuild), cacheTime);
      _paths[path] = link;
      _lock.Release();
      if(buildNow) link.Build();
    }
    
    /// <summary>
    /// Add or set a page. The cache time is a number of milliseconds that the result
    /// of the 'onBuild' action can be cached for.
    /// </summary>
    public void Set(string key, string path, IAction<Element> onBuild, int cacheTime = -1, bool buildNow = true) {
      path = Fs.Combine(Path, path);
      _lock.Take();
      var link = _elements[key] = new ElementLink(path, onBuild, cacheTime);
      _paths[path] = link;
      _lock.Release();
      if(buildNow) link.Build();
    }
    
    /// <summary>
    /// Add or set an element to be retrieved using the specified method. The cache time is a number of
    /// milliseconds that the result of the 'retrieve' func can be cached for.
    /// </summary>
    public void Set(string key, Func<Element> retrieve, int cacheTime = -1, bool buildNow = true) {
      _lock.Take();
      var link = _elements[key] = new ElementLink(new FuncSet<Element>(retrieve), null, cacheTime);
      _lock.Release();
      if(buildNow) link.Build();
    }
    
    /// <summary>
    /// Add or set an element to be retrieved using the specified method. The cache time is a number of
    /// milliseconds that the result of the 'retrieve' func can be cached for.
    /// </summary>
    public void Set(string key, IFunc<Element> retrieve, int cacheTime = -1, bool buildNow = true) {
      _lock.Take();
      var link = _elements[key] = new ElementLink(retrieve, null, cacheTime);
      _lock.Release();
      if(buildNow) link.Build();
    }
    
    /// <summary>
    /// Remove a page from the pages manager.
    /// </summary>
    public void Remove(string key) {
      _lock.Take();
      _elements.Remove(key);
      _lock.Release();
    }
    
    /// <summary>
    /// Invalidate an element record such that it gets refreshed on the next request.
    /// </summary>
    public void Invalidate(string key) {
      _lock.Take();
      ElementLink link;
      if(_elements.TryGetValue(key, out link)) link.Invalidate();
      _lock.Release();
    }
    
    //----------------------------------//
    
    /// <summary>
    /// On a file within the directory being changed.
    /// </summary>
    protected void OnChanged(object sender, FileSystemEventArgs args) {
      
      ElementLink link;
      if(_paths.TryGetValue(args.FullPath.Swap(Chars.BackSlash, Chars.ForwardSlash), out link)) {
        link.Invalidate();
      }
      
    }
    
  }
  
  
}
