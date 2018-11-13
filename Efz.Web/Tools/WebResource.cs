/*
 * User: FloppyNipples
 * Date: 04/02/2017
 * Time: 14:30
 */
using System;
using System.IO;

using Efz.Threading;

namespace Efz.Web {
  
  /// <summary>
  /// Description of a resource that is passed as a web page or part of one.
  /// </summary>
  public class WebResource {
    
    //----------------------------------//
    
    /// <summary>
    /// The mime type the web resource represents.
    /// </summary>
    public string MimeType;
    /// <summary>
    /// File extension of the specified resource.
    /// </summary>
    public string Extension {
      get { return Mime.GetExtension(MimeType); }
      set { MimeType = Mime.GetType(value); }
    }
    
    /// <summary>
    /// Path to the resource.
    /// </summary>
    public string FullPath;
    /// <summary>
    /// Local path to the resource if the full path specifies a local resource.
    /// </summary>
    public string Path;
    
    /// <summary>
    /// Size in bytes of the web resource. Returns '0' if the size can't be determined.
    /// </summary>
    public long Size {
      get {
        // has the resource been loaded? no, load it
        if(!_valid && _reset) {
          Load();
          if(_reset) return 0;
        }
        
        // can the stream length be determined?
        if(_stream.CanSeek) {
          // yes, return the stream length
          return _stream.Length;
        }
        
        // return 0
        return 0;
      }
    }
    
    /// <summary>
    /// Is the web resource able to be loaded?
    /// </summary>
    public bool Valid {
      get {
        if(_valid) return true;
        if(_reset) Load();
        return _valid;
      }
    }
    
    /// <summary>
    /// Get or set the compression method of the web resource. This should be
    /// the actual compression, not the desired compression. The resource compression
    /// will match the client compression.
    /// </summary>
    public System.Net.DecompressionMethods Compression {
      get { return _compression; }
      set { _compression = value; }
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Has the web resource been loaded?
    /// </summary>
    protected bool _reset;
    /// <summary>
    /// Is the web resource valid?
    /// </summary>
    protected bool _valid;
    
    /// <summary>
    /// Stream to the web resource if loaded.
    /// </summary>
    protected Stream _stream;
    
    /// <summary>
    /// Is the stream already compressed?
    /// </summary>
    protected System.Net.DecompressionMethods _compression;
    
    /// <summary>
    /// Lock used for external access to the resource.
    /// </summary>
    protected LockShared _lock;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a web resource.
    /// </summary>
    public WebResource(HttpSite site, string path, string mime = null) {
      
      _lock = new LockShared();
      
      // persist the local path
      Path = path;
      
      // does the path indicate a web resource?
      if(Fs.IsWebPath(Path)) {
        // persist the path
        FullPath = Path;
      } else {
        // persist the path
        FullPath = Fs.Combine(site.Path, Path);
      }
      
      _compression = System.Net.DecompressionMethods.None;
      
      // get the web resource extension if set
      MimeType = mime ?? Mime.GetType(Fs.GetExtension(FullPath));
      
      // the resource must be loaded to begin with
      _reset = true;
      
    }
    
    /// <summary>
    /// Append the resource to the specified stream.
    /// </summary>
    public void CopyTo(Stream stream) {
      _lock.Take();
      
      if(_reset) {
        Load();
        if(_reset) {
          _lock.Release();
          Log.Warning("Stream wasn't able to be resolved '"+FullPath+"'.");
          return;
        }
      }
      
      // copy the stream content
      _stream.CopyTo(stream);
      
      // the stream must be reloaded
      _reset = true;
      // release the lock
      _lock.Release();
    }
    
    /// <summary>
    /// Get a stream to the resource. Returns 'Null' if the stream can't be resolved.
    /// This invalidates the resource stream.
    /// </summary>
    public Stream GetStream(out Lock @lock) {
      _lock.Take();
      if(_reset) {
        Load();
        if(_reset) {
          _lock.Release();
          Log.Warning("Stream wasn't able to be resolved '"+FullPath+"'.");
          @lock = null;
          return null;
        }
      }
      @lock = _lock;
      _reset = true;
      return _stream;
    }
    
    /// <summary>
    /// Non-blocking method of getting the web resource stream. Only use immediate IActions.
    /// </summary>
    public void GetStream(IAction<Stream, Lock> onStream) {
      _lock.TryLock(ActionSet.New(OnGetStream, onStream), null, false);
    }
    
    /// <summary>
    /// Replace the web resource stream.
    /// </summary>
    public void ReplaceStream(Stream stream, System.Net.DecompressionMethods compression) {
      _lock.Take();
      if(_stream != null) {
        _stream.Close();
        _stream = null;
      }
      _stream = stream;
      _compression = compression;
      _reset = true;
      _lock.Release();
    }
    
    /// <summary>
    /// Replace the web resource stream. Lock should be taken before this method is called.
    /// </summary>
    public void ReplaceStreamLocked(Stream stream, System.Net.DecompressionMethods compression) {
      if(_stream != null) {
        _stream.Close();
        _stream = null;
      }
      _stream = stream;
      _compression = compression;
      _reset = true;
    }
    
    /// <summary>
    /// Get a string representation of the resource. This is either the complete content
    /// or a source string.
    /// </summary>
    public string GetString() {
      _lock.Take();
      if(_reset) {
        Load();
        if(_reset) {
          _lock.Release();
          Log.Warning("Stream wasn't able to be resolved '"+FullPath+"'.");
          return null;
        }
      }
      
      // determine the string representation based on the mime type
      switch(Mime.GetCategory(MimeType)) {
        case Mime.Category.Text:
          StreamReader reader = new StreamReader(_stream);
          var content = reader.ReadToEnd();
          _reset = true;
          _lock.Release();
          return content;
        
        case Mime.Category.Audio:
        case Mime.Category.Video:
        case Mime.Category.Image:
        case Mime.Category.Document:
        case Mime.Category.Unknown:
          _lock.Release();
          return Path; 
      }
      
      _lock.Release();
      return string.Empty;
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Load the resource.
    /// </summary>
    protected void Load() {
      // open a stream to the resource
      if(_stream != null && _stream.CanSeek) {
        // yes, reset the position
        _stream.Position = 0;
        // the stream was loaded
        _reset = false;
      } else {
        _stream = GetStream(FullPath);
        _reset = _stream == null;
        _valid = !_reset;
      }
    }
    
    /// <summary>
    /// On the lock being successfully attained as a result of a GetStream request
    /// with a callback.
    /// </summary>
    protected void OnGetStream(IAction<Stream, Lock> onStream) {
      if(_reset) {
        Load();
        if(_reset) {
          Log.Warning("Stream wasn't able to be resolved '"+FullPath+"'.");
          onStream.ArgA = null;
          onStream.ArgB = _lock;
          onStream.Run();
          return;
        }
        _reset = true;
      }
      onStream.ArgA = _stream;
      onStream.ArgB = _lock;
      onStream.Run();
    }
    
    /// <summary>
    /// Get a stream to the specified resource path. Optionally open a reader or writer stream.
    /// Will return 'Null' if the file stream can't be created.
    /// </summary>
    public static Stream GetStream(string path, bool reader = true) {
      
      // does the path start with a local file path?
      if(path[1] == Chars.Colon && path[2] == Chars.ForwardSlash || path.StartsWith(Protocols.File, StringComparison.OrdinalIgnoreCase)) {
        // yes, is a reader required?
        if(reader) {
          
          // yes, does the file exist?
          if(File.Exists(path)) {
            // yes, open a file stream
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
          }
          
          // no, return null
          return null;
        }
        
        // no, open a file stream
        return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
        
      }
      
      // does the path start with a http resource?
      if(path.StartsWith(Protocols.Http, StringComparison.OrdinalIgnoreCase) || path.StartsWith(Protocols.Https, StringComparison.OrdinalIgnoreCase)) {
        
        // yes, open a http stream
        return new WebStream(path);
      }
      
      // does the path start with a socket protocol?
      if(path.StartsWith(Protocols.Socket, StringComparison.OrdinalIgnoreCase)) {
        throw new NotImplementedException("Sockets aren't implemented yet. Low priority.");
      }
      
      return null;
    }
    
  }
  
}
