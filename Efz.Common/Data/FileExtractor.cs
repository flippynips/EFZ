/*
 * User: Joshua
 * Date: 1/08/2016
 * Time: 7:29 PM
 */
using System;
using System.Text;

using Efz.Collections;
using Efz.Network;
using Efz.Threading;
using Efz.Tools;

namespace Efz.Text {
  
  /// <summary>
  /// Runs extractors on files.
  /// </summary>
  public class FileExtractor : IDisposable {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Timeout in milliseconds for files.
    /// </summary>
    public int Timeout    = 5000;
    
    /// <summary>
    /// Extractor for the file.
    /// </summary>
    public Extract Extractor;
    
    /// <summary>
    /// Is the file extractor running?
    /// </summary>
    public bool Running;
    /// <summary>
    /// When the file extractor finishes with all the files.
    /// </summary>
    public IAction OnComplete;
    /// <summary>
    /// When a file is parsed.
    /// </summary>
    public IAction<string> OnFile;
    /// <summary>
    /// The decoder to use for the current file.
    /// </summary>
    public Decoder Decoder;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Parser derived from the extractor.
    /// </summary>
    private Parse _parser;
    
    /// <summary>
    /// The full path to the files to be parsed.
    /// </summary>
    private Queue<string> _files;
    /// <summary>
    /// Byte buffer.
    /// </summary>
    private readonly byte[] _buffer;
    /// <summary>
    /// Character buffer.
    /// </summary>
    private readonly char[] _chars;
    /// <summary>
    /// Connection to the current file.
    /// </summary>
    private ConnectionLocal _connection;
    
    /// <summary>
    /// Timer used to timeout connections to files.
    /// </summary>
    private readonly Timer _timeout;
    
    /// <summary>
    /// Lock for external access to the file extractor.
    /// </summary>
    private Lock _lock;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a file extractor for the specified file with an extraction instance
    /// and the encoding for the file.
    /// </summary>
    public FileExtractor(string file, Extract extractor = null, Encoding encoding = null) :
      this(new ArrayRig<string>(new [] {file}), extractor, encoding) {}
    
    /// <summary>
    /// Initialize a file extractor with a collection of file paths, the extraction instance and the
    /// encoding of the file.
    /// </summary>
    public FileExtractor(ArrayRig<string> files, Extract extractor = null, Encoding encoding = null) {
      _files = new Queue<string>();
      
      // add the files to the queue
      foreach(string file in files) _files.Enqueue(file);
      
      // persist the extractor
      Extractor = extractor;
      Decoder = (encoding ?? Encoding.UTF8).GetDecoder();
      
      // initialize the buffers
      _buffer = BufferCache.Get();
      _chars = new char[(encoding ?? Encoding.UTF8).GetMaxCharCount(Global.BufferSizeLocal)];
      
      _timeout = new Timer(Timeout, Run, false);
      _lock = new Lock();
      
    }
    
    /// <summary>
    /// Dispose this file reader.
    /// </summary>
    public void Dispose() {
      Running = false;
      _timeout.Run = false;
      Extractor.Dispose();
      if(_connection != null) _connection.Dispose();
      BufferCache.Set(_buffer);
      Decoder = null;
      _parser = null;
      _files = null;
      _lock = null;
    }
    
    /// <summary>
    /// Begin reading the files.
    /// </summary>
    public void Run() {
      
      _lock.Take();
      
      // is the extraction already running? yes, skip
      if(Running) {
        
        // did a connection timeout?
        if(_timeout.Run) {
          // no, interrupted
          _lock.Release();
          return;
        }
        
        // nullify connection
        if(_connection != null) {
          _connection.Close();
          _connection = null;
        }
        
        // notify of the connection timeout
        Log.Warning("Connection to file was unable to be established '" + _files.Current + "'.");
      }
      
      // no, start running
      Running = true;
      
      // are there more files?
      if(_files.Dequeue()) {
        // yes, get a parser for the file
        _parser = Extractor.GetParser();
        // reset the decoder
        Decoder.Reset();
        
        // reset timeout
        _timeout.Reset(Timeout);
        
        _connection = null;
        _lock.Release();
        
        // get a reader for the next file
        ManagerConnections.Get<ConnectionLocal>(_files.Current, new Act<ConnectionLocal>(OnConnection));
        
      } else {
        
        // no, is the callback set? run
        if(OnComplete != null) OnComplete.Run();
        
        _lock.Release();
        
      }
      
    }
    
    /// <summary>
    /// Add a file to be extracted.
    /// </summary>
    public void AddFile(string path) {
      _lock.Take();
      _files.Enqueue(path);
      _lock.Release();
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// On connection.
    /// </summary>
    protected void OnConnection(ConnectionLocal connection) {
      
      // reset the timeout
      _timeout.Reset(Timeout);
      
      // persist the connection
      _connection = connection;
      
      // get a stream to the connection
      _connection.Get(new Act<ByteBuffer>(Read));
      
    }
    
    /// <summary>
    /// Read addresses into the buffer for the crawlers to comsume.
    /// </summary>
    protected void Read(ByteBuffer reader) {
      
      // stop the timeout
      _timeout.Run = false;
      
      // read the next buffer
      int count = reader.ReadBytes(_buffer, 0, Global.BufferSizeLocal);
      
      // get the characters5
      count = Decoder.GetChars(_buffer, 0, count, _chars, 0);
      
      // run the parser
      _parser.Next(_chars, 0, count);
      
      // was the file finished?
      if(reader.Empty) {
        
        // yes, no longer running
        Running = false;
        
        // is the 'OnFile' callback set?
        if(OnFile != null) {
          // yes, run
          OnFile.ArgA = _files.Current;
          OnFile.Run();
        }
        
        // read the next file
        Run();
        
      } else {
        
        // get the reader for the next buffer
        ManagerConnections.Get<ByteBuffer, ConnectionLocal>(_files.Current, new Act<ByteBuffer>(Read));
        
      }
      
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
  }
  
}
