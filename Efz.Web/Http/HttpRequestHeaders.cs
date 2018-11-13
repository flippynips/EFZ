/*
 * User: Joshua
 * Date: 23/10/2016
 * Time: 1:06 AM
 */
using System;
using System.Collections.Generic;

namespace Efz.Web {
  
  /// <summary>
  /// Headers used for requests.
  /// </summary>
  public class HttpRequestHeaders : IEnumerable<KeyValuePair<string, string>> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Get or set a request header by key.
    /// </summary>
    public string this[HttpRequestHeader key] {
      get {
        string value;
        return Headers.TryGetValue(key, out value) ? value : null;
      }
      set {
        if(value == null) {
          if(Headers.ContainsKey(key)) Headers.Remove(key);
        } else {
          Headers[key] = value;
        }
      }
    }
    
    /// <summary>
    /// Get or set a request header by key.
    /// </summary>
    public string this[string key] {
      get {
        string value;
        if(Custom.TryGetValue(key, out value)) return value;
        if(Headers.TryGetValue(HttpRequestHeaderExtensions.GetKey(key), out value)) return value;
        return null;
      }
      set {
        HttpRequestHeader header = HttpRequestHeaderExtensions.GetKey(key);
        if(header == HttpRequestHeader.Unknown) Custom[key] = value;
        else Headers[header] = value;
      }
    }
    
    /// <summary>
    /// Values in the collection.
    /// </summary>
    internal Dictionary<HttpRequestHeader, string> Headers;
    /// <summary>
    /// Custom header key value pairs.
    /// </summary>
    internal Dictionary<string, string> Custom;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Create a new collection of http headers.
    /// </summary>
    public HttpRequestHeaders() {
      Headers = new Dictionary<HttpRequestHeader, string>();
      Custom = new Dictionary<string, string>();
    }
    
    public override string ToString() {
      return "[HttpRequestHeaders Headers="+Headers.Derive(e => e.Key + " : " + e.Value)+", Custom="+Custom.Derive(e => e.Key + " : " + e.Value)+"]";
    }
    
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
      return new Enumerator(this);
    }
    
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Enumerator of the headers class.
    /// </summary>
    public class Enumerator : IEnumerator<KeyValuePair<string, string>> {
      
      //--------------------------------//
      
      object System.Collections.IEnumerator.Current {
        get { return _current; }
      }
      
      public KeyValuePair<string, string> Current {
        get { return _current; }
      }
      
      //--------------------------------//
      
      /// <summary>
      /// Headers.
      /// </summary>
      private readonly HttpRequestHeaders _headers;
      /// <summary>
      /// Current key value pair.
      /// </summary>
      private KeyValuePair<string, string> _current;
      
      /// <summary>
      /// Current custom headers enumerator.
      /// </summary>
      private IEnumerator<KeyValuePair<string,string>> _customEnumerator;
      /// <summary>
      /// Current defined headers enumerator.
      /// </summary>
      private IEnumerator<KeyValuePair<HttpRequestHeader, string>> _definedEnumerator;
      
      //--------------------------------//
      
      public Enumerator(HttpRequestHeaders headers) {
        
        _headers = headers;
        _definedEnumerator = _headers.Headers.GetEnumerator();
        _customEnumerator = _headers.Custom.GetEnumerator();
        
      }
      
      public void Dispose() {
        
        if(_definedEnumerator != null) _definedEnumerator.Dispose();
        if(_customEnumerator != null) _customEnumerator.Dispose();
        
      }
      
      /// <summary>
      /// Move to the next header.
      /// </summary>
      public bool MoveNext() {
        
        if(_definedEnumerator == null) {
          
          if(_customEnumerator.MoveNext()) {
            _current = _customEnumerator.Current;
            return true;
          }
          
          return false;
          
        }
        
        if(_definedEnumerator.MoveNext()) {
          foreach(var entry in HttpRequestHeaderExtensions.Map.Value) {
            if(entry.Value == _definedEnumerator.Current.Key) {
              _current = new KeyValuePair<string, string>(entry.Key, _definedEnumerator.Current.Value);
              return true;
            }
          }
          _current = new KeyValuePair<string, string>(_definedEnumerator.Current.Key.ToString(), _definedEnumerator.Current.Value);
          return true;
        }
        
        _definedEnumerator = null;
        
        return MoveNext();
        
      }
      
      public void Reset() {
        
        if(_definedEnumerator != null) _definedEnumerator.Dispose();
        if(_customEnumerator != null) _customEnumerator.Dispose();
        _definedEnumerator = _headers.Headers.GetEnumerator();
        _customEnumerator = _headers.Custom.GetEnumerator();
        
      }
      
      //--------------------------------//
      
    }
    
  }
  
}
