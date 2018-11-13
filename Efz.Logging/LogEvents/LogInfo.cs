/*
 * User: FloppyNipples
 * Date: 27/05/2017
 * Time: 21:25
 */
using System;
using System.Text;

namespace Efz.Logs {
  
  /// <summary>
  /// Logging of a message centerised with a line.
  /// </summary>
  public struct LogInfo : ILogEvent {
    
    //-------------------------------//
    
    /// <summary>
    /// Type of log event.
    /// </summary>
    public LogType Type { get { return _type; } }
    /// <summary>
    /// Message of the log event.
    /// </summary>
    public string Message { get { return _message; } }
    
    /// <summary>
    /// Prefix for an info log message.
    /// </summary>
    internal const string Prefix = "-- INFO ";
    
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
    public LogInfo(string message) {
      _type = LogType.Info;
      _message = message;
    }
    
    /// <summary>
    /// Write the log line.
    /// </summary>
    public void Write() {
      Console.BackgroundColor = ConsoleColor.DarkBlue;
      Console.ForegroundColor = ConsoleColor.Blue;
      Log.StandardOutput.Write(Prefix);
      Log.StandardOutput.Flush();
      Console.ResetColor();
      Log.StandardOutput.WriteLine(_message);
      Log.StandardOutput.Flush();
    }
    
    //-------------------------------//
    
  }
  
}
