using System;
using System.Collections.Generic;
using System.Text;

using Efz.Collections;
using Efz.Tools;

namespace Efz.Web {

  /// <summary>
  /// Collection of http response headers that are cached.
  /// </summary>
  public class HttpResponseHeaders {
    
    //----------------------------------//
    
    /// <summary>
    /// Version string as part of the header prefix.
    /// </summary>
    public string Version {
      get { return _version; }
      set {
        _version = value ?? string.Empty;
        _refreshDefinedHeaders = true;
        _refreshCustomHeaders = true;
        _refreshBuilderIndex = 0;
        _refreshDefinedHeaderIndex = 0;
        _refreshCustomHeaderIndex = 0;
      }
    }
    /// <summary>
    /// Status code as part of the header prefix.
    /// </summary>
    public int StatusCode {
      get { return _statusCode; }
      set {
        _statusCode = value;
        _statusDescription = GetStatusDescription(_statusCode);
        _refreshDefinedHeaders = true;
        _refreshCustomHeaders = true;
        _refreshBuilderIndex = 0;
        _refreshDefinedHeaderIndex = 0;
        _refreshCustomHeaderIndex = 0;
      }
    }
    /// <summary>
    /// Description string related to the status code as part of the header prefix.
    /// </summary>
    public string StatusDescription {
      get { return _statusDescription; }
    }
    
    /// <summary>
    /// Get the headers as a collection of bytes.
    /// </summary>
    public byte[] Bytes {
      get {
        // return the header string converted to bytes
        return Encoding.UTF8.GetBytes(String);
      }
    }
    
    /// <summary>
    /// Get the headers as a complete string.
    /// </summary>
    public string String {
      get {
        
        // do the headers need to be refreshed? yes, run the refresh
        if(_refreshDefinedHeaders || _refreshCustomHeaders) RefreshHeaders();
        
        //Log.Debug("Built Headers : \n" + _builder.ToString());
        
        // return the complete headers
        return _builder.ToString();
      }
    }
    
    /// <summary>
    /// Get or set the specified header.
    /// </summary>
    public string this[HttpResponseHeader key] {
      get {
        // iterate the headers
        foreach(var header in _definedHeaders) {
          // does the header match?
          if(header.ArgA == key) {
            // yes, return the header value
            return header.ArgB.ArgA;
          }
        }
        // header not 'Null'
        return null;
      }
      set {
        Set(key, value);
      }
    }
    
    /// <summary>
    /// Get or set the specified header.
    /// </summary>
    public string this[string key] {
      get {
        // iterate the headers
        foreach(var header in _customHeaders) {
          // does the header match?
          if(header.ArgA == key) {
            // yes, return the header value
            return header.ArgB.ArgA;
          }
        }
        // header not 'Null'
        return null;
      }
      set {
        Set(key, value);
      }
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Inner mappings of status codes to descriptions.
    /// </summary>
    private static Dictionary<int, string> _statusDescriptions;
    
    /// <summary>
    /// Collection of header key-value pairs.
    /// </summary>
    protected ArrayRig<Teple<HttpResponseHeader, Teple<string, int>>> _definedHeaders;
    /// <summary>
    /// Collection of header key-value pairs.
    /// </summary>
    protected ArrayRig<Teple<string, Teple<string, int>>> _customHeaders;
    
    /// <summary>
    /// Header keys to be removed after the next build.
    /// </summary>
    protected Dictionary<HttpResponseHeader, string> _definedSingles;
    /// <summary>
    /// Header keys to be removed after the next build.
    /// </summary>
    protected Dictionary<string, string> _customSingles;
    
    /// <summary>
    /// Inner version string.
    /// </summary>
    protected string _version;
    /// <summary>
    /// Inner status code value;
    /// </summary>
    protected int _statusCode;
    /// <summary>
    /// Inner status description.
    /// </summary>
    protected string _statusDescription;
    
    /// <summary>
    /// The current complete headers string as a builder.
    /// </summary>
    protected StringBuilder _builder;
    
    /// <summary>
    /// Do the defined headers need to be refreshed?
    /// </summary>
    protected bool _refreshDefinedHeaders;
    /// <summary>
    /// Do the custom headers need to be refreshed?
    /// </summary>
    protected bool _refreshCustomHeaders;
    /// <summary>
    /// If the defined headers are to be refreshed, this index will indicate the header index
    /// to start at.
    /// </summary>
    protected int _refreshDefinedHeaderIndex;
    /// <summary>
    /// If the custom headers are to be refreshed, this index will indicate the header index
    /// to start at.
    /// </summary>
    protected int _refreshCustomHeaderIndex;
    /// <summary>
    /// If the headers are to be refreshed, this index will indicate the string index
    /// to start at.
    /// </summary>
    protected int _refreshBuilderIndex;
    
    //----------------------------------//
    
    static HttpResponseHeaders() {
      _statusDescriptions = BuildDescriptions();
    }
    
    /// <summary>
    /// Get a string description of a status code.
    /// </summary>
    public static string GetStatusDescription(int statusCode) {
      string description;
      return _statusDescriptions.TryGetValue(statusCode, out description) ? description : "Unknown";
    }
    
    /// <summary>
    /// Initialize a new set of headers.
    /// </summary>
    public HttpResponseHeaders(bool includeDefault) {
      
      // initialize the collection
      _definedHeaders = new ArrayRig<Teple<HttpResponseHeader, Teple<string, int>>>(12);
      _definedSingles = new Dictionary<HttpResponseHeader, string>();
      _customHeaders = new ArrayRig<Teple<string, Teple<string, int>>>();
      _customSingles = new Dictionary<string, string>();
      _builder = new StringBuilder();
      
      // should the default headers be added?
      if(includeDefault) {
        
        // set the default header prefix
        Version = "Http/1.1";
        StatusCode = 200;
        
        Set(HttpResponseHeader.Server, "Efz");
        Set(HttpResponseHeader.AcceptRanges, "bytes");
        Set(HttpResponseHeader.Vary, "Accept-Encoding");
        Set(HttpResponseHeader.Connection, "keep-alive");
        Set(HttpResponseHeader.ContentEncoding, "gzip");
        Set(HttpResponseHeader.ContentType, "text/html; charset=UTF-8");
        
      } else {
        _statusCode = 0;
        _version = _statusDescription = string.Empty;
      }
      
    }
    
    /// <summary>
    /// Set the specified header in the headers instance. If value is 'Null' the
    /// header will be removed.
    /// </summary>
    public void Set(HttpResponseHeader key, string value) {
      
      if(value == null) {
        Remove(key);
        return;
      }
      
      // has the singles array been assigned?
      if(_definedSingles.ContainsKey(key)) _definedSingles.Remove(key);
      
      // iterate the headers
      for (int i = _definedHeaders.Count-1; i >= 0; --i) {
        
        // does the header match?
        if (_definedHeaders[i].ArgA == key) {
          
          var header = _definedHeaders[i];
          
          // does the value match the existing header value?
          if(header.ArgB.ArgA.Equals(value, StringComparison.Ordinal)) {
            // yes, skip the assignment
            return;
          }
          
          // set the header value
          header.ArgB.ArgA = value;
          
          // flag a refresh of the headers
          if(_refreshDefinedHeaders) {
            if(_refreshDefinedHeaderIndex > i) _refreshDefinedHeaderIndex = i;
            if(_refreshBuilderIndex > header.ArgB.ArgB) _refreshBuilderIndex = header.ArgB.ArgB;
          } else {
            _refreshDefinedHeaders = true;
            _refreshDefinedHeaderIndex = i;
            _refreshBuilderIndex = header.ArgB.ArgB;
          }
          
          return;
        }
      }
      
      // is the refresh index after the index to be added?
      if(!_refreshDefinedHeaders || _definedHeaders.Count < _refreshDefinedHeaderIndex) {
        
        // yes, set the header index
        _refreshDefinedHeaderIndex = _definedHeaders.Count;
        
        // decrement the builder index
        _refreshBuilderIndex = _builder.Length-1;
        
        // flip the refresh flag
        _refreshDefinedHeaders = true;
      }
      
      // add the header value
      _definedHeaders.Add(new Teple<HttpResponseHeader, Teple<string, int>>(key, new Teple<string, int>(value, 0)));
      
    }
    
    /// <summary>
    /// Set the specified header in the headers instance. If value is 'Null' the
    /// header will be removed.
    /// </summary>
    public void Set(string key, string value) {
      
      if(value == null) {
        Remove(key);
        return;
      }
      
      // has the singles array been assigned?
      if(_customSingles.ContainsKey(key)) _customSingles.Remove(key);
      
      // iterate the headers
      for (int i = _customHeaders.Count-1; i >= 0; --i) {
        
        // does the header match?
        if (_customHeaders[i].ArgA == key) {
          
          var header = _customHeaders[i];
          
          // does the value match the existing header value?
          if(header.ArgB.ArgA.Equals(value, StringComparison.Ordinal)) {
            // yes, skip the assignment
            return;
          }
          
          // set the header value
          header.ArgB.ArgA = value;
          
          // flag a refresh of the headers
          if(_refreshCustomHeaders) {
            if(_refreshCustomHeaderIndex > i) _refreshCustomHeaderIndex = i;
            if(_refreshBuilderIndex > header.ArgB.ArgB) _refreshBuilderIndex = header.ArgB.ArgB;
          } else {
            _refreshCustomHeaders = true;
            _refreshCustomHeaderIndex = i;
            _refreshBuilderIndex = header.ArgB.ArgB;
          }
          
          return;
        }
      }
      
      // is the refresh index after the index to be added?
      if(!_refreshCustomHeaders || _definedHeaders.Count < _refreshCustomHeaderIndex) {
        
        // yes, set the header index
        _refreshCustomHeaderIndex = _definedHeaders.Count;
        
        // decrement the builder index
        _refreshBuilderIndex = _builder.Length-1;
        
        // flip the refresh flag
        _refreshCustomHeaders = true;
      }
      
      // add the header value
      _customHeaders.Add(new Teple<string, Teple<string, int>>(key, new Teple<string, int>(value, 0)));
      
    }
    
    /// <summary>
    /// Set a header key and value for a single request. The header
    /// is removed after a single build.
    /// </summary>
    public void SetSingle(HttpResponseHeader key, string value) {
      
      // add to the singles collection
      _definedSingles[key] = value;
      
      // iterate the headers
      for (int i = _definedHeaders.Count-1; i >= 0; --i) {
        
        // does the header match?
        if (_definedHeaders[i].ArgA.Equals(key)) {
          
          // is the refresh index after the index to be removed?
          if(!_refreshDefinedHeaders || i < _refreshDefinedHeaderIndex) {
            
            // yes, set the header index
            _refreshDefinedHeaderIndex = i;
            
            // decrement the builder index
            _refreshBuilderIndex = _definedHeaders[i].ArgB.ArgB;
            
            // flip the refresh flag
            _refreshDefinedHeaders = true;
          }
          
          return;
        }
      }
      
      // is the refresh index after the index to be removed?
      if(!_refreshDefinedHeaders && _definedHeaders.Count < _refreshDefinedHeaderIndex) {
        
        // yes, set the header index
        _refreshDefinedHeaderIndex = _definedHeaders.Count;
        
        _refreshBuilderIndex = _builder.Length-1;
        
        // flip the refresh flag
        _refreshDefinedHeaders = true;
      }
      
    }
    
    /// <summary>
    /// Set the specified header in the headers instance. If value is 'Null' the
    /// header will be removed.
    /// </summary>
    public void Remove(HttpResponseHeader key) {
      
      // has the singles array been assigned?
      if(_definedSingles.ContainsKey(key)) _definedSingles.Remove(key);
      
      // iterate the headers
      for (int i = _definedHeaders.Count-1; i >= 0; --i) {
        
        // does the header match?
        if (_definedHeaders[i].ArgA == key) {
          
          var header = _definedHeaders[i];
          
          // is the refresh index after the index to be removed?
          if(!_refreshDefinedHeaders || i < _refreshDefinedHeaderIndex) {
            
            // yes, set the header index
            _refreshDefinedHeaderIndex = i;
            _refreshDefinedHeaders = true;
            
            // decrement the builder index
            _refreshBuilderIndex = header.ArgB.ArgB;
            
            // flip the refresh flag
            _refreshDefinedHeaders = true;
          }
          
          // yes, remove the header value
          _definedHeaders.RemoveQuick(i);
          
          return;
        }
      }
      
    }
    
    /// <summary>
    /// Set the specified header in the headers instance. If value is 'Null' the
    /// header will be removed.
    /// </summary>
    public void Remove(string key) {
      
      // has the singles array been assigned?
      if(_customSingles.ContainsKey(key)) _customSingles.Remove(key);
      
      // iterate the headers
      for (int i = _customHeaders.Count-1; i >= 0; --i) {
        
        // does the header match?
        if (_customHeaders[i].ArgA == key) {
          
          var header = _customHeaders[i];
          
          // is the refresh index after the index to be removed?
          if(!_refreshCustomHeaders || i < _refreshCustomHeaderIndex) {
            
            // yes, set the header index
            _refreshCustomHeaderIndex = i;
            
            // decrement the builder index
            _refreshBuilderIndex = header.ArgB.ArgB;
            
            // flip the refresh flag
            _refreshCustomHeaders = true;
          }
          
          // yes, remove the header value
          _customHeaders.RemoveQuick(i);
          
          return;
        }
      }
      
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Refresh the headers from the header index.
    /// </summary>
    protected unsafe void RefreshHeaders() {
      
      // set the builder index
      _builder.Length = _refreshBuilderIndex;
      
      if(_refreshBuilderIndex == 0) {
        _builder.Append(_version);
        _builder.Append(Chars.Space);
        _builder.Append(_statusCode);
        _builder.Append(Chars.Space);
        _builder.AppendLine(_statusDescription);
      }
      
      if(_refreshDefinedHeaders) {
        _refreshDefinedHeaders = false;
        
        // iterate the headers from the index to be refreshed
        for (int i = _refreshDefinedHeaderIndex; i < _definedHeaders.Count; ++i) {
          
          // persist the header
          Teple<HttpResponseHeader, Teple<string, int>> header = _definedHeaders[i];
          
          header.ArgB.ArgB = _builder.Length;
          
          string value;
          if(_definedSingles.TryGetValue(header.ArgA, out value)) {
            // yes, check refresh
            if(!_refreshDefinedHeaders) {
              
              // yes, set the header index
              _refreshDefinedHeaderIndex = i;
              _refreshBuilderIndex = header.ArgB.ArgB;
              
              // flip the refresh flag
              _refreshDefinedHeaders = true;
            }
            
            // remove the single-shot header from the collection
            _definedSingles.Remove(header.ArgA);
            
            if(header.ArgB.ArgA == null) {
              // remove the header from the collection
              _definedHeaders.RemoveQuick(i);
              --i;
            }
            
            // append the header key and value
            _builder.Append(header.ArgA.GetString());
            _builder.Append(Chars.Colon);
            _builder.Append(Chars.Space);
            _builder.AppendLine(value);
            
          } else {
            
            // append the header key and value
            _builder.Append(header.ArgA.GetString());
            _builder.Append(Chars.Colon);
            _builder.Append(Chars.Space);
            _builder.AppendLine(header.ArgB.ArgA);
            
          }
          
        }
        
        _refreshCustomHeaders = true;
        _refreshCustomHeaderIndex = 0;
        
      }
      
      if(_refreshCustomHeaders) {
        _refreshCustomHeaders = false;
        
        // iterate the headers from the index to be refreshed
        for (int i = _refreshCustomHeaderIndex; i < _customHeaders.Count; ++i) {
          
          // persist the header
          Teple<string, Teple<string, int>> header = _customHeaders[i];
          
          header.ArgB.ArgB = _builder.Length;
          
          string value;
          if(_customSingles.TryGetValue(header.ArgA, out value)) {
            // yes, check refresh
            if(!_refreshCustomHeaders) {
              
              // yes, set the header index
              _refreshCustomHeaderIndex = i;
              _refreshBuilderIndex = header.ArgB.ArgB;
              
              // flip the refresh flag
              _refreshCustomHeaders = true;
            }
            
            // remove the single-shot header from the collection
            _customSingles.Remove(header.ArgA);
            
            if(header.ArgB.ArgA == null) {
              // remove the header from the collection
              _customHeaders.RemoveQuick(i);
              --i;
            }
            
            // append the header key and value
            _builder.Append(header.ArgA);
            _builder.Append(Chars.Colon);
            _builder.Append(Chars.Space);
            _builder.AppendLine(value);
            
          } else {
            
            // append the header key and value
            _builder.Append(header.ArgA);
            _builder.Append(Chars.Colon);
            _builder.Append(Chars.Space);
            _builder.AppendLine(header.ArgB.ArgA);
            
          }
          
        }
        
      }
      
      
      foreach(var single in _definedSingles) {
        // append the header key and value
        _builder.Append(single.Key.GetString());
        _builder.Append(Chars.Colon);
        _builder.Append(Chars.Space);
        _builder.AppendLine(single.Value);
      }
      
      _definedSingles.Clear();
      
      foreach(var single in _customSingles) {
        // append the header key and value
        _builder.Append(single.Key);
        _builder.Append(Chars.Colon);
        _builder.Append(Chars.Space);
        _builder.AppendLine(single.Value);
      }
      
      _customSingles.Clear();
      
      _builder.AppendLine();
      
      // should the status code be reset?
      if (_statusCode != 200) {
        // yes, reset it
        StatusCode = 200;
        // reset the status description
        _statusDescription = "OK";
      }
      
    }
    
    #region StatusDescriptions
    /// <summary>
    /// Inner method used to build the map of style keys.
    /// </summary>
    private static Dictionary<int, string> BuildDescriptions() {
      
      var map = new Dictionary<int, string> {
        { 100, "Continue" },
        { 101, "Switching Protocols" },
        { 102, "Processing" },
        { 200, "OK" },
        { 201, "Created" },
        { 202, "Accepted" },
        { 203, "Non-Authoritative Information" },
        { 204, "No Content" },
        { 205, "Reset Content" },
        { 206, "Partial Content" },
        { 207, "Multi-Status" },
        { 208, "Already Reported" },
        { 226, "IM Used" },
        { 300, "Multiple Choices" },
        { 301, "Moved Permanently" },
        { 302, "Found" },
        { 303, "See Other" },
        { 304, "Not Modified" },
        { 305, "Use Proxy" },
        { 306, "Switch Proxy" },
        { 307, "Temporary Redirect" },
        { 308, "Permanent Redirect" },
        { 400, "Bad Request" },
        { 401, "Unauthorized" },
        { 402, "Payment Required" },
        { 403, "Forbidden" },
        { 404, "Not Found" },
        { 405, "Method Not Allowed" },
        { 406, "Not Acceptable" },
        { 407, "Proxy Authentication Required" },
        { 408, "Request Timeout" },
        { 409, "Conflict" },
        { 410, "Gone" },
        { 411, "Length Required" },
        { 412, "Precondition Failed" },
        { 413, "Payload Too Large" },
        { 414, "URI Too Long" },
        { 415, "Unsupported Media Type" },
        { 416, "Range Not Satisfiable" },
        { 417, "Expectation Failed" },
        { 418, "I'm a teapot" },
        { 421, "Misdirected Request" },
        { 422, "Unprocessable Entity" },
        { 423, "Locked" },
        { 424, "Failed Dependency" },
        { 426, "Upgrade Required" },
        { 428, "Precondition Required" },
        { 429, "Too Many Requests" },
        { 431, "Request Header Fields Too Large" },
        { 451, "Unavailable For Legal Reasons" }
      };
      
      return map;
    }
    #endregion
    
  }
  
}

