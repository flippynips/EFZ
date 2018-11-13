/*
 * User: FloppyNipples
 * Date: 11/01/2017
 * Time: 00:18
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Efz.Collections;

namespace Efz.Web {
  
  /// <summary>
  /// Represents a single web request that is be built lazily.
  /// Must be disposed of.
  /// </summary>
  public class HttpRequest : IDisposable {
    
    //----------------------------------//
    
    /// <summary>
    /// Maximum number of characters for a request path.
    /// </summary>
    public const int MaxRequestPath = 250;
    
    /// <summary>
    /// Type of web request.
    /// </summary>
    public HttpMethod Method;
    
    /// <summary>
    /// Collection of headers sent with the request.
    /// </summary>
    public readonly HttpRequestHeaders Headers;
    /// <summary>
    /// The web connection that received the request.
    /// </summary>
    public readonly HttpConnection Connection;
    /// <summary>
    /// The web client that received the request.
    /// </summary>
    public HttpClient Client { get { return Connection.Client; } }
    
    /// <summary>
    /// The payload if any that was sent with the web request.
    /// </summary>
    public ByteBuffer Stream { get { return _stream; } }
    /// <summary>
    /// Is the web request complete?
    /// </summary>
    public bool Complete { get { return _complete; } }
    
    /// <summary>
    /// Start of the body index in the byte stream.
    /// </summary>
    public int BodyIndex { get { return _bodyIndex; } }
    
    /// <summary>
    /// Current path of the request.
    /// </summary>
    public string RequestPath;
    
    /// <summary>
    /// If an error occured, this will represent the error message
    /// </summary>
    public string ErrorMessage { get { return _errorMessage; } }
    
    //----------------------------------//
    
    /// <summary>
    /// Has the request been completely received?
    /// </summary>
    protected bool _complete;
    
    /// <summary>
    /// Inner error message if there was a problem with the request.
    /// </summary>
    protected string _errorMessage;
    
    /// <summary>
    /// StreamBeing written to and read from for the request.
    /// </summary>
    protected ByteBuffer _stream;
    /// <summary>
    /// Inner start of the request body index.
    /// </summary>
    protected int _bodyIndex;
    
    /// <summary>
    /// Temporary character array used to store the characters in the
    /// header.
    /// </summary>
    protected ArrayRig<char> _chars;
    
    /// <summary>
    /// Remaining content lengthof the request.
    /// </summary>
    protected int _contentLength;
    /// <summary>
    /// Current request header key being processed.
    /// </summary>
    protected string _headerKey;
    /// <summary>
    /// Header used to read header names and values.
    /// </summary>
    protected int _headerIndex;
    
    /// <summary>
    /// Sections that are read.
    /// </summary>
    protected enum Section {
      Body = 0,
      Path = 1,
      Method = 2,
      HeaderKey = 3,
      HeaderValue = 4,
    }
    /// <summary>
    /// Current section being populated in the request.
    /// </summary>
    protected Section _section;
    
    //----------------------------------//
    
    internal HttpRequest(HttpConnection connection) {
      // reference the web client
      Connection = connection;
      
      // create the header collection
      Headers = new HttpRequestHeaders();
      _stream = new ByteBuffer(new MemoryStream(700), Global.BufferSizeLocal, System.Text.Encoding.UTF8);
      _chars = new ArrayRig<char>();
      _section = Section.Method;
    }
    
    /// <summary>
    /// Dispose of the web request and it's resources.
    /// </summary>
    public void Dispose() {
      _stream.Dispose();
      _stream = null;
    }
    
    /// <summary>
    /// Try add the specified bytes to the request.
    /// Returns the number of bytes read from the buffer.
    /// </summary>
    internal unsafe int TryAdd(byte[] buffer, int index, int count) {
      
      // is the request complete?
      if(_complete) return 0;
      
      // has the header been completely read?
      if(_section == Section.Body) {
        // does the current buffer contain the required bytes?
        if(_contentLength <= count) {
          
          // yes, write the remaining content length to the stream
          _stream.Write(buffer, index, _contentLength);
          count = _contentLength;
          _contentLength = 0;
          
          // the request is complete
          _complete = true;
          
          // move back to the start of the content body - this flushes the byte stream
          _stream.Position = _bodyIndex;
          
          // TODO : Auto decompression
          
          return count;
        }
        
        // no, write the buffer
        _stream.Write(buffer, index, count);
        // decrement the remaining content length
        _contentLength -= count;
        
        return count;
      }
      
      // persist the starting index of the stream
      long startIndex = _stream.WriteEnd;
      
      // write the buffer to the request
      _stream.Write(buffer, index, count);
      
      // return to the start of the buffer
      _stream.Position = startIndex;
      
      char c;
      
      // while characters can be read from the stream
      while((c = _stream.ReadChar()) != Chars.Null) {
        
        // should the character be skipped? yes, continue reading
        if(c == Chars.CarriageReturn) continue;
        
        // determine the state of the request method
        switch(_section) {
          case Section.Method:
            
            // read until a space is encountered
            if(c == Chars.Space) {
              // try determine the type of web request
              if(_chars.EndsWith(Chars.G, Chars.E, Chars.T)) {
                Method = HttpMethod.Get;
              } else if(_chars.EndsWith(Chars.P, Chars.O, Chars.S, Chars.T)) {
                Method = HttpMethod.Post;
              } else if(_chars.EndsWith(Chars.P, Chars.U, Chars.T, Chars.Space)) {
                Method = HttpMethod.Put;
              } else if(_chars.EndsWith(Chars.D, Chars.E, Chars.L, Chars.E, Chars.T, Chars.E)) {
                Method = HttpMethod.Delete;
              } else if(_chars.EndsWith(Chars.U, Chars.P, Chars.D, Chars.A, Chars.T, Chars.E)) {
                Method = HttpMethod.Update;
              }
              _section = Section.Path;
              _chars.Reset();
            } else {
              // add the current character to the collection
              _chars.Add(c);
            }
            
            break;
          case Section.Path:
            
            // yes, does the current character equal a new line?
            if(c == Chars.NewLine) {
              
              // yes, derive the request path and the http version
              index = _chars.Count;
              bool versionFound = false;
              
              // while the indices haven't been explored
              while(--index >= 0) {
                // does the current character equal a space?
                if(_chars[index] == Chars.Space) {
                  
                  // no, derive the request version
                  Headers[HttpRequestHeader.HttpVersion] = new string(_chars.Array, index + 1, _chars.Count - index - 1);
                  versionFound = true;
                  break;
                }
              }
              
              // yes, has the version been derived? yes, derive the request path
              if(versionFound) RequestPath = new string(_chars.Array, 0, index);
              else RequestPath = "/";
              
              _section = Section.HeaderKey;
              _chars.Reset();
              
            } else {
              
              // add the current character to the collection
              _chars.Add(c);
              
            }
            
            break;
            
          case Section.HeaderKey:
            
            // is the current character a separator between header names and values?
            if(c == Chars.Colon) {
              
              // get the name of the chracter header
              _headerKey = new string(_chars.Array, 0, _chars.Count);
              // read the header value
              _section = Section.HeaderValue;
              // clear the characters
              _chars.Reset();
              
            } else if(c == Chars.NewLine && _chars.Count == 0) {
              
              // yes, the request header has been completed
              _chars.Dispose();
              
              // determine whether to continue the request
              switch(Method) {
                case HttpMethod.Put:
                case HttpMethod.Post:
                case HttpMethod.Update:
                  
                  // set the start of the body index
                  _bodyIndex = (int)_stream.Position;
                  
                  // get the content length - was it able to be parsed?
                  if(!int.TryParse(Headers[HttpRequestHeader.ContentLength], out _contentLength)) {
                    // the content length wasn't able to be parsed
                    Log.Warning("Invalid content length parameter.");
                    // set the content length to '0'.
                    _contentLength = 0;
                    _complete = true;
                    return count;
                  }
                  
                  // the request contains a body
                  // should this request claim the remaining bytes written to the stream?
                  if(_stream.WriteEnd - _stream.Position >= _contentLength) {
                    
                    // no, get the excess number of bytes from the request
                    count = (int)(_stream.WriteEnd - startIndex);
                    _contentLength = 0;
                    _complete = true;
                    
                  } else {
                    
                    // decrement the content length
                    _contentLength -= (int)(_stream.WriteEnd - _stream.Position);
                    // flag the header as complete
                    _section = Section.Body;
                    // move the stream to the end of the written section
                    _stream.Position = _stream.WriteEnd;
                    
                  }
                  
                  // return the number of bytes that contribute to the request
                  return count;
                default:
                  
                  // the request doesn't contain a body, end reading the request
                  _complete = true;
                  // return the number of bytes read from the buffer
                  return (int)(_stream.Position - startIndex);
              }
              
            } else {
              // add the current character to the collection
              _chars.Add(c);
            }
            
            break;
          case Section.HeaderValue:
            
            // is the current character a new line?
            if(c == Chars.NewLine) {
              
              // add the header to the collection
              Headers[_headerKey] = new string(_chars.Array, 0, _chars.Count);
              _chars.Reset();
              _section = Section.HeaderKey;
              
            } else if(c != Chars.Space || _chars.Count != 0) {
              // add the current character to the collection
              _chars.Add(c);
            }
            break;
        }
      }
      
      // return the number of bytes read from the buffer
      return count;
    }
    
    /// <summary>
    /// Read post form data from the request stream and return the result as a collection
    /// of key-value pairs. Will return 'Null' if the data is not valid Optionally
    /// specify an expected number of fields.
    /// </summary>
    public HttpPostParams ReadParameters(int fields = -1) {
      
      HttpPostParams parameters = new HttpPostParams();
      
      // is GET?
      if(Method == HttpMethod.Get) {
        
        // try get the index separating the path from the parameters
        int paramsIndex = RequestPath.IndexOf(Chars.Question);
        
        // were parameters added?
        if(paramsIndex >= 0) {
          
          var split = System.Net.WebUtility.UrlDecode(RequestPath).Split(paramsIndex);
          
          RequestPath = split.ArgA;
          
          string key = null;
          int indexStart = 0;
          int index = 0;
          while(index < split.ArgB.Length) {
            
            switch(split.ArgB[index]) {
              case Chars.Equal:
                
                key = split.ArgB.Section(indexStart, index);
                indexStart = index + 1;
                
                break;
              case Chars.And:
                
                parameters.Add(key, split.ArgB.Section(indexStart, index));
                indexStart = index + 1;
                key = null;
                
                break;
            }
            
            ++index;
          }
          
          if(key != null) {
            if(indexStart == index) {
              parameters.Add(key.ToLowercase());
            } else {
              parameters.Add(key.ToLowercase(), split.ArgB.Section(indexStart, index));
            }
          }
          
        }
        
        return parameters;
      }
      
      // get the content type
      var contentType = Headers[HttpRequestHeader.ContentType].Split(Chars.SemiColon);
      
      // parse based off the content type
      switch(contentType[0].TrimSpace().ToLowercase()) {
        case "multipart/form-data":
          if(contentType.Length != 2) {
            Log.Warning("POST request for '"+RequestPath+"' wasn't parsed. Content Type '"+Headers[HttpRequestHeader.ContentType]+"' was malformed.");
            return null;
          }
          
          var boundarySplit = contentType[1].TrimSpace().Split(Chars.Equal);
          if(boundarySplit.Length != 2 || !boundarySplit[0].Equals("boundary", StringComparison.OrdinalIgnoreCase)) {
            Log.Warning("POST request for '"+RequestPath+"' wasn't parsed. Boundary specification '"+contentType[1]+"' was malformed.");
            return null;
          }
          
          try {
            
            // read the boundary bytes
            byte[] boundary = System.Text.Encoding.UTF8.GetBytes("--" + boundarySplit[1]);
            
            // read bytes until the specified boundary is read
            _stream.ReadTo(boundary);
            
            string values;
            
            // read the next line break
            if(_stream.ReadChar() == Chars.CarriageReturn) _stream.ReadChar();
            
            // did the stream end unexpectedly?
            if(_stream.Empty) {
              Log.Warning("POST request for '"+RequestPath+"' wasn't parsed. Stream ran out unexpectedly.");
              return null;
            }
            
            Dictionary<string, string> contentParams;
            string name = null;
            
            // iterate while the stream isn't empty
            while(!_stream.Empty) {
              
              contentParams = new Dictionary<string, string>();
              
              // ensure the content disposition is next
              values = _stream.ReadString(Chars.Colon);
              if(!values.Equals("content-disposition", StringComparison.OrdinalIgnoreCase)) {
                // were the parameters empty? yes, return the empty parameters
                if(values == "-\r\n") return parameters;
                Log.Warning("POST request for '"+RequestPath+"' wasn't parsed. Content disposition flag '"+values+"' was malformed.");
                return null;
              }
              
              // common character reference
              char c;
              // read to the next line break
              values = _stream.ReadString(out c, Chars.NewLine, Chars.CarriageReturn);
              
              // while the specified characters hasn't been read and the stream
              // isn't empty
              while(true) {
                
                if(values.Length == 0) break;
                
                // did the stream end unexpectedly?
                if(_stream.Empty) {
                  Log.Warning("POST request for '"+RequestPath+"' wasn't parsed. Stream ended unexpectedly.");
                  return null;
                }
                
                // split the read parameters into key-value pairs
                var keyValues = values.Split(Chars.SemiColon);
                
                // read each parameter into the content params collection
                foreach(var keyValue in keyValues) {
                  int index = keyValue.IndexOf(Chars.Equal);
                  if(index == -1) index = keyValue.IndexOf(Chars.Colon);
                  
                  if(index == -1) {
                    contentParams.Add(keyValue.TrimSpace().ToLowercase(), "");
                  } else {
                    var split = keyValue.Split(index);
                    if(split.ArgB[0] == Chars.DoubleQuote) {
                      contentParams.Add(split.ArgA.TrimSpace().ToLowercase(),
                        split.ArgB.TrimSpace().Substring(1, split.ArgB.Length-2));
                    } else {
                      contentParams.Add(split.ArgA.TrimSpace().ToLowercase(),
                        split.ArgB.TrimSpace());
                    }
                  }
                  
                }
                
                // was the content name specified? no, invalid
                if(!contentParams.TryGetValue("name", out name)) {
                  Log.Warning("POST request for '"+RequestPath+"' wasn't parsed. Post parameter was missing a 'name'.");
                  return null;
                }
                
                // read the next line break
                if(c == Chars.CarriageReturn) _stream.ReadChar();
                // read the next byte
                c = _stream.ReadChar();
                
                // does the character indicate another line break?
                if(c == Chars.CarriageReturn) {
                  // yes, read the next character (assumed '\n') and break to read the value
                  _stream.ReadChar();
                  break;
                }
                // does the character indicate another line breaks? yes, break
                if(c == Chars.NewLine) break;
                
                // read the next line as content params
                values = c + _stream.ReadString(out c, Chars.NewLine, Chars.CarriageReturn);
              }
              
              // read the content bytes
              ArrayRig<byte> bytes = _stream.ReadBytes(boundary);
              if(bytes.Count < 2) {
                // is the parameter a file reference?
                if(!contentParams.ContainsKey("filename")) {
                  // no, add an empty string value
                  parameters.Add(name, _stream.Encoding.GetString(bytes.Array, 0, bytes.Count), contentParams);
                }
              } else {
                
                // decrement the content byte count - the content should end in \n
                --bytes.Count;
                if(bytes[bytes.Count-1] == Ascii.CarriageReturn) --bytes.Count;
                
                // does the post parameter represent a file?
                if(contentParams.ContainsKey("filename")) {
                  // yes, assign the file bytes
                  if(bytes.Count != 0) parameters.Add(name, bytes, contentParams);
                } else {
                  // no, get the bytes as a string
                  parameters.Add(name, _stream.Encoding.GetString(bytes.Array, 0, bytes.Count), contentParams);
                }
                
              }
              
              // has the expected field count been reached? yes, return
              if(parameters.Count == fields) return parameters;
              
              // read the next character
              c = _stream.ReadChar();
              if(c == Chars.CarriageReturn) _stream.ReadChar();
              else if(c == Chars.Dash && _stream.ReadChar() == Chars.Dash) break;
              else if(c != Chars.NewLine) {
                Log.Warning("POST request for '"+RequestPath+"' wasn't parsed. Post parameter boundary ended with unexpected character '"+c+"'.");
                return null;
              }
              
            }
          #if DEBUG
          } catch(Exception ex) {
            
            Log.Error("There was an exception reading post parameters", ex);
            // return 'null' the parameters are invalid
            return null;
            
          }
          #else
          } catch {
            
            // return 'null' the parameters are invalid
            return null;
            
          }
          #endif
          
          break;
        case "application/x-www-form-urlencoded":
          while(!_stream.Empty) {
            try {
              
              // read the post fields
              char c;
              parameters.Add(_stream.ReadString(Chars.Equal).ToLowercase(),
                _stream.ReadString(out c, Chars.NewLine, Chars.CarriageReturn));
              
            } catch {
              
              // return 'null' - the parameters are invalid
              return null;
            }
            
            // has the expected fields count been reached?
            if(parameters.Count == fields) {
              // yes, return
              return parameters;
            }
          }
          break;
        case "text/plain":
          while(!_stream.Empty) {
            try {
              
              // read the key and un-escape
              var key = _stream.ReadString(Chars.Equal)
                .UnEscape(Chars.Plus, Chars.Space);
              
              if(key.Length == 0) return parameters;
              
              // read the value and un-escape
              char next;
              var value = _stream.ReadString(out next, Chars.CarriageReturn, Chars.NewLine)
                .UnEscape(Chars.Plus, Chars.Space);
              
              // should the next character be skipped?
              if(next == Chars.CarriageReturn) _stream.ReadChar();
              
              // add the post fields
              parameters.Add(key.ToLowercase(), value);
              
            } catch {
              
              // return 'null' the parameters are invalid
              return null;
              
            }
            
            // has the expected fields count been reached?
            if(parameters.Count == fields) {
              // yes, return
              return parameters;
            }
          }
          break;
        default:
          // unknown post parameter format
          return null;
      }
      
      return parameters;
    }
    
    /// <summary>
    /// Does the request indicate the client already has the most recent version of the resource?
    /// Returns true if the clients version of the resource is recent and a 304 response has been sent.
    /// </summary>
    public bool Cached(DateTime dateTime, long age) {
      
      // has the 'if-modified-since' header been specified?
      if(Headers.Headers.ContainsKey(HttpRequestHeader.IfModifiedSince)) {
        DateTime cached;
        
        if(DateTime.TryParseExact(Headers.Headers[HttpRequestHeader.IfModifiedSince],
          CultureInfo.CurrentCulture.DateTimeFormat.RFC1123Pattern,
          CultureInfo.InvariantCulture,
          DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out cached) &&
          cached < dateTime) {
          
          Connection.SendCacheUnchanged(age);
          return true;
        }
      }
      
      return false;
    }
    
    /// <summary>
    /// Get a string representation of the web request.
    /// </summary>
    public override string ToString() {
      return "[WebRequest Method="+Method+" From="+Connection+" Path="+RequestPath.Truncate(50, "...")+"]";
    }
    
    //----------------------------------//
    
  }
  
}
