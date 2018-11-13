/*
 * User: FloppyNipples
 * Date: 27/05/2017
 * Time: 21:25
 */
using System;

namespace Efz.Logs {
  
  /// <summary>
  /// Logging of a message centerised with a line.
  /// </summary>
  public struct LogStat : ILogEvent {
    
    //-------------------------------//
    
    /// <summary>
    /// Type of log event.
    /// </summary>
    public LogType Type { get { return _type; } }
    /// <summary>
    /// Message of the log event.
    /// </summary>
    public string Message { get { return _message; } }
    
    //-------------------------------//
    
    /// <summary>
    /// Inner type of log event.
    /// </summary>
    private LogType _type;
    /// <summary>
    /// Inner message of the log event.
    /// </summary>
    private readonly string _message;
    
    //-------------------------------//
    
    /// <summary>
    /// Initialize a new log line event.
    /// </summary>
    public LogStat(string message) {
      _type = LogType.Stat;
      _message = message;
    }
    
    /// <summary>
    /// Write the log line.
    /// </summary>
    public void Write() {
      Console.BackgroundColor = ConsoleColor.DarkBlue;
      Console.ForegroundColor = ConsoleColor.White;
      Log.StandardOutput.WriteLine(_message);
      Log.StandardOutput.Flush();
    }
    
    //-------------------------------//
    
  }
  
}
