/*
 * User: FloppyNipples
 * Date: 02/01/2017
 * Time: 00:52
 */
using System;
using System.IO;
using System.Collections.Concurrent;

using Efz.Logs;
using Efz.Threading;

namespace Efz {
  
  /// <summary>
  /// Handler to log to a file.
  /// </summary>
  public class LogToFile : IDisposable {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The file being logged to.
    /// </summary>
    public string File {
      get {
        return _file;
      }
      set {
        _file = value;
        if(_writer != null) {
          _writer.Close();
          _writer.Dispose();
        }
        _writer = new StreamWriter(new FileStream(_file, _truncate && System.IO.File.Exists(_file) ? FileMode.Truncate : FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read));
      }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// File path the logs are written to.
    /// </summary>
    protected string _file;
    /// <summary>
    /// Should the log file be truncated?
    /// </summary>
    protected bool _truncate;
    
    /// <summary>
    /// Log writer.
    /// </summary>
    protected StreamWriter _writer;
    /// <summary>
    /// Lock for the writer.
    /// </summary>
    protected Lock _lock;
    /// <summary>
    /// Queue of logs.
    /// </summary>
    protected ConcurrentQueue<ILogEvent> _logs;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Create a log to file instance.
    /// </summary>
    public LogToFile(string file, bool truncate = true) {
      _truncate = truncate;
      File = file;
      // create the logs collection
      _logs = new ConcurrentQueue<ILogEvent>();
      _lock = new Lock();
      // subscribe to logging
      Log.OnLog += OnLog;
    }
    
    /// <summary>
    /// Dispose of the resources used by the logger.
    /// </summary>
    public void Dispose() {
      Log.OnLog -= OnLog;
      _lock.Take();
      ILogEvent log;
      while(_logs.TryDequeue(out log)) {
        _writer.Write(log);
        _writer.WriteLine();
      }
      _writer.Close();
      _writer = null;
      _lock.Release();
      _lock = null;
      _logs = null;
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Update.
    /// </summary>
    protected void Update() {
      
      // take the lock
      if(_lock.TryTake) {
        // dequeue all log messages and write them to the log file
        ILogEvent log;
        while(_logs.TryDequeue(out log)) {
          _writer.Write(log.Type);
          _writer.Write(Chars.Space);
          _writer.Write(log.Message);
          _writer.WriteLine();
        }
        
        _lock.Release();
      }
      
    }
    
    /// <summary>
    /// Callback on logging.
    /// </summary>
    protected void OnLog(ILogEvent log) {
      
      // was the lock taken?
      if(_lock.TryTake) {
        // yes, write pending log messages
        while(_logs.TryDequeue(out log)) {
          _writer.Write(log);
          _writer.WriteLine();
        }
        // write the current log message
        _writer.Write(log.Type);
        _writer.Write(Chars.Space);
        _writer.Write(log.Message);
        _writer.WriteLine();
        
        _lock.Release();
        
      } else {
        
        // enqueue the message
        _logs.Enqueue(log);
        ManagerUpdate.Control.AddSingle(Update);
        
      }
      
    }
    
  }
  
}
