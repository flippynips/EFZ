/*
 * User: FloppyNipples
 * Date: 04/02/2017
 * Time: 17:33
 */
using System;
using System.IO;
using System.Net;

namespace Efz.Web {
  
  /// <summary>
  /// A web based stream.
  /// </summary>
  public class WebStream : Stream {
    
    //-------------------------------------------//
    
    public override long Length {
      get { return _stream.Length; }
    }
    
    public override long Position {
      get {
        return _stream.Position;
      }
      set {
        _stream.Position = value;
      }
    }
    
    /// <summary>
    /// Can the stream be read from?
    /// </summary>
    public override bool CanRead {
      get { return _canRead; }
    }
    /// <summary>
    /// Can the stream be written to?
    /// </summary>
    public override bool CanWrite {
      get { return _canWrite; }
    }
    /// <summary>
    /// Can the current stream be seeked?
    /// </summary>
    public override bool CanSeek {
      get { return false; }
    }
    
    /// <summary>
    /// Get or set the time a write operation will consume before timing out.
    /// </summary>
    public override int WriteTimeout {
      get { return _stream.WriteTimeout; }
      set { _stream.WriteTimeout = value; }
    }
    
    /// <summary>
    /// Get this stream timeout?
    /// </summary>
    public override bool CanTimeout {
      get { return _stream.CanTimeout; }
    }
    
    /// <summary>
    /// The path of the web stream.
    /// </summary>
    public string Path;
    
    /// <summary>
    /// The request used for the stream once initialized.
    /// </summary>
    protected HttpWebRequest Request;
    /// <summary>
    /// The web response
    /// </summary>
    protected HttpWebResponse Response {
      get {
        // is the stream being written to and has the request not been completed? yes, flush it
        if(_canWrite && _response == null) Flush();
        return _response;
      }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Can the web stream be read from?
    /// </summary>
    protected bool _canRead;
    /// <summary>
    /// Can be web stream be written to?
    /// </summary>
    protected bool _canWrite;
    /// <summary>
    /// The web identity used for the web request.
    /// </summary>
    protected Identity _identity;
    
    /// <summary>
    /// The request stream.
    /// </summary>
    protected Stream _stream;
    /// <summary>
    /// Inner reference to the web response.
    /// </summary>
    protected HttpWebResponse _response;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a web stream to the specified path.
    /// </summary>
    public WebStream(string path, Identity identity = null) {
      
      Path = path;
      _identity = identity ?? Identity.Next;
      
      _canRead = _canWrite = true;
    }
    
    /// <summary>
    /// Flush the stream of underlying byte collections.
    /// </summary>
    public override void Flush() {
      
      // was the stream a writer?
      if(_canRead) {
        
        // no, nevertheless flush the underlying stream
        _stream.Flush();
        
      } else {
        
        // yes, complete the post request
        try {
          _response = (HttpWebResponse)Request.GetResponse();
        } catch {
          _response = null;
          throw;
        }
        
      }
      
    }
    
    /// <summary>
    /// Dispose of the web stream.
    /// </summary>
    protected override void Dispose(bool disposing) {
      if(_response != null) _response.Dispose();
      if(Request != null) {
        try {
          Request.Abort();
        } catch { /* ignore gracefully */ }
      }
      if(_stream != null) _stream.Dispose();
    }
    
    /// <summary>
    /// Seeking in a web stream is invalid.
    /// </summary>
    public override long Seek(long offset, SeekOrigin origin) {
      return _stream.Seek(offset, origin);
    }
    
    /// <summary>
    /// Setting the length of a web stream is invalid.
    /// </summary>
    public override void SetLength(long value) {
      throw new InvalidOperationException();
    }
    
    /// <summary>
    /// Write the specified bytes to the web resource.
    /// </summary>
    public override void Write(byte[] buffer, int offset, int count) {
      if(!_canWrite) throw new InvalidOperationException("Web stream cannot be written to.");
      if(Request == null) BeginRequest(false);
      _stream.Write(buffer, offset, count);
    }
    
    /// <summary>
    /// Write a single byte to the web request stream.
    /// </summary>
    public override void WriteByte(byte value) {
      if(!_canWrite) throw new InvalidOperationException("Web stream cannot be written to.");
      if(Request == null) BeginRequest(false);
      _stream.WriteByte(value);
    }
    
    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
      if(!_canWrite) throw new InvalidOperationException("Web stream cannot be written to.");
      if(Request == null) BeginRequest(false);
      return _stream.BeginWrite(buffer, offset, count, callback, state);
    }
    
    /// <summary>
    /// End an asynchronous write to the request stream.
    /// </summary>
    public override void EndWrite(IAsyncResult asyncResult) {
      if(!_canWrite) throw new InvalidOperationException("Web stream cannot be written to.");
      _stream.EndWrite(asyncResult);
    }
    
    /// <summary>
    /// Read the specified byte buffer from the stream.
    /// </summary>
    public override int Read(byte[] buffer, int offset, int count) {
      if(!_canRead) throw new InvalidOperationException("Web stream cannot be read from.");
      if(Request == null) BeginRequest(true);
      return _stream.Read(buffer, offset, count);
    }
    
    /// <summary>
    /// Read a single byte from the web resource.
    /// </summary>
    public override int ReadByte() {
      if(!_canRead) throw new InvalidOperationException("Web stream cannot be read from.");
      if(Request == null) BeginRequest(true);
      return _stream.ReadByte();
    }
    
    /// <summary>
    /// Asynchronously begin reading from the stream.
    /// </summary>
    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
      if(!_canRead) throw new InvalidOperationException("Web stream cannot be read from.");
      if(Request == null) BeginRequest(true);
      return _stream.BeginRead(buffer, offset, count, callback, state);
    }
    
    /// <summary>
    /// End an asynchronous read from the stream.
    /// </summary>
    public override int EndRead(IAsyncResult asyncResult) {
      if(!_canRead) throw new InvalidOperationException("Web stream cannot be read from.");
      return _stream.EndRead(asyncResult);
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Start the request as either a reader or writer.
    /// </summary>
    protected void BeginRequest(bool reader) {
      
      // create the web request
      Request = System.Net.WebRequest.CreateHttp(Path);
      // add the identity headers to the web request
      _identity.UpdateRequest(Request);
      
      // is the request to read a resource?
      if(reader) {
        // yes, make the request a getter
        Request.Method = "GET";
        
        _canWrite = false;
        
        try {
          // get the request response
          _response = (HttpWebResponse)Request.GetResponse();
          
          // get a stream from the response
          _stream = _response.GetResponseStream();
          
        } catch {
          
          // reset the parameters
          Request = null;
          _canWrite = true;
          
          // rethrow
          throw;
          
        }
        
      } else {
        // no, make the request a post
        Request.Method = "POST";
        
        _canRead = false;
        
        try {
          // get a request stream
          _stream = Request.GetRequestStream();
        } catch {
          
          // reset the parameters
          Request = null;
          _canRead = true;
          
          // rethrow
          throw;
        }
        
      }
      
    }
    
  }
  
}
