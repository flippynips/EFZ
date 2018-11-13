/*
 * User: FloppyNipples
 * Date: 26/08/2018
 * Time: 1:17 PM
 */
using System;
using System.IO;
using System.Threading.Tasks;
using Efz.Collections;
using Efz.Threading;

namespace Efz.Web.Display {
  
  /// <summary>
  /// Represents a link between an element builder function.
  /// </summary>
  public class ElementLink {
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// Last retrieved element.
    /// </summary>
    protected Element _element;
    
    /// <summary>
    /// Path if specified, will result in the specified file being
    /// parsed into the element structure.
    /// </summary>
    protected string _path;
    /// <summary>
    /// Function that is called in order to retrieve the element
    /// instead of parsing a file.
    /// </summary>
    protected IFunc<Element> _getElement;
    
    /// <summary>
    /// On the element being retrieved.
    /// </summary>
    protected IAction<Element> _onBuild;
    /// <summary>
    /// Callbacks for when the element is constructed.
    /// </summary>
    protected ArrayRig<IAction<Element>> _callbacks;
    /// <summary>
    /// Timeout of the element link, after which the next request will
    /// cause the element to be re-parsed. If '-1' the element is not
    /// re-retrieved.
    /// </summary>
    protected int _cacheTime;
    /// <summary>
    /// The next timestamp when the link is to be renewed.
    /// </summary>
    protected long _nextUpdate;
    
    /// <summary>
    /// Lock to be used for any external access to the element link.
    /// </summary>
    protected Lock _lock;
    /// <summary>
    /// Flag indicating the element is being retrieved.
    /// </summary>
    protected bool _processing;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct a new element link.
    /// </summary>
    public ElementLink(string path, IAction<Element> onBuild = null, int cacheTime = -1) {
      
      _path = path;
      _onBuild = onBuild;
      _cacheTime = cacheTime;
      _nextUpdate = Time.Milliseconds + _cacheTime;
      _callbacks = new ArrayRig<IAction<Element>>();
      _lock = new Lock();
      _processing = false;
      
    }
    
    /// <summary>
    /// Construct a new element link.
    /// </summary>
    public ElementLink(IFunc<Element> getElement, IAction<Element> onBuild = null, int cacheTime = -1) {
      
      _getElement = getElement;
      _cacheTime = cacheTime;
      _nextUpdate = Time.Milliseconds + _cacheTime;
      _onBuild = onBuild;
      _callbacks = new ArrayRig<IAction<Element>>();
      _lock = new Lock();
      _processing = false;
      
    }
    
    /// <summary>
    /// Get the element with a callback.
    /// </summary>
    public void Get(IAction<Element> onGet) {
      
      _lock.Take();
      
      _callbacks.Add(onGet);
      
      if(_processing) {
        _lock.Release();
        return;
      }
      
      _processing = true;
      
      // check if the link has or should be updated
      if(_element == null) {
        if(_path == null) {
          ManagerUpdate.Control.AddSingle(OnRetrieved, _getElement.Run());
        } else {
          new ElementParser(_path, Act.New(OnParsed, (ElementParser)null)).Run();
        }
      } else {
        if(_nextUpdate < Time.Milliseconds) {
          if(_path == null) {
            ManagerUpdate.Control.AddSingle(OnRetrieved, _getElement.Run());
          } else {
            new ElementParser(_path, Act.New(OnParsed, (ElementParser)null)).Run();
          }
        } else {
          _processing = false;
          foreach(var callback in _callbacks) {
            callback.ArgA = _element.Clone();
            ManagerUpdate.Control.AddSingle(callback);
          }
          _callbacks.Clear();
        }
      }
      
      _lock.Release();
      
    }
    
    /// <summary>
    /// Get the element asynchronously.
    /// </summary>
    public async Task<Element> GetAsync() {
      
      Element result = null;
      
      _lock.Take();
      
      if(_processing) {
        
        if(_element == null) {
          
          _lock.Release();
          
          if(_path == null) {
            result = await _getElement.RunAsync();
          } else {
            var parser = new ElementParser(_path).RunSync();
            if(parser.Error == null) result = parser.Root;
            else Log.Error("Element parser error. " + parser.Error);
          }
          
          if(result == null) return null;
          
          if(_onBuild != null) {
            _onBuild.ArgA = result;
            _onBuild.Run();
          }
          
        } else {
          
          result = _element.Clone();
          
          _lock.Release();
          
        }
        
        return result;
        
      }
      
      // check if the link has or should be updated
      if(_element == null) {
        
        if(_path == null) {
          result = await _getElement.RunAsync();
        } else {
          var parser = new ElementParser(_path).RunSync();
          if(parser.Error == null) result = parser.Root;
          else {
            _lock.Release();
            Log.Error("Element parser error. " + parser.Error);
            return null;
          }
        }
        
        _element = result;
        
        if(_onBuild != null) {
          _onBuild.ArgA = _element;
          _onBuild.Run();
        }
        
        if(_cacheTime > 0) _nextUpdate = Time.Milliseconds + _cacheTime;
        else _nextUpdate = long.MaxValue;
        
      } else {
        
        if(_nextUpdate < Time.Milliseconds) {
          if(_path == null) {
            result = await _getElement.RunAsync();
            if(result == null) {
              _lock.Release();
              return null;
            }
          } else {
            var parser = new ElementParser(_path).RunSync();
            if(parser.Error != null) {
              _lock.Release();
              Log.Error("Element parser error. " + parser.Error);
              return null;
            }
            result = parser.Root;
          }
          
          _element = result;
          
          if(_onBuild != null) {
            _onBuild.ArgA = _element;
            _onBuild.Run();
          }
          
          if(_cacheTime > 0) _nextUpdate = Time.Milliseconds + _cacheTime;
          else _nextUpdate = long.MaxValue;
        }
        
      }
      
      // iterate and run the callbacks
      foreach(var callback in _callbacks) {
        callback.ArgA = _element.Clone();
        ManagerUpdate.Control.AddSingle(callback);
      }
      
      _callbacks.Clear();
      
      result = _element.Clone();
      
      _lock.Release();
      
      return result;
      
    }
    
    /// <summary>
    /// Build.
    /// </summary>
    public void Build() {
      _lock.Take();
      if(!_processing) {
        if(_path == null) {
          ManagerUpdate.Control.AddSingle(OnRetrieved, _getElement.Run());
        } else {
          new ElementParser(_path, Act.New(OnParsed, (ElementParser)null)).Run();
        }
      }
      _lock.Release();
    }
    
    /// <summary>
    /// Invalidate the element links current build.
    /// </summary>
    public void Invalidate() {
      _nextUpdate = Time.Milliseconds-1;
    }
    
    //----------------------------------//
    
    /// <summary>
    /// On a page having been parsed.
    /// </summary>
    protected void OnParsed(ElementParser parser) {
      
      // was the parse successful?
      if(parser.Error != null) {
        Log.Error("Element parser encountered an error. " + parser.Error);
        _processing = false;
        return;
      }
      
      // run on retrieved
      OnRetrieved(parser.Root);
      
    }
    
    /// <summary>
    /// On the element being built.
    /// </summary>
    private void OnRetrieved(Element element) {
      
      _lock.Take();
      
      _element = element;
      
      if(_onBuild != null) {
        _onBuild.ArgA = _element;
        _onBuild.Run();
      }
      
      foreach(var callback in _callbacks) {
        callback.ArgA = _element.Clone();
        ManagerUpdate.Control.AddSingle(callback);
      }
      
      _callbacks.Clear();
      
      _processing = false;
      
      if(_cacheTime > 0) _nextUpdate = Time.Milliseconds + _cacheTime;
      else _nextUpdate = long.MaxValue;
      
      _lock.Release();
      
    }
    
  }
  
}
