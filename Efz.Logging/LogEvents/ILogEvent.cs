/*
 * User: FloppyNipples
 * Date: 27/05/2017
 * Time: 21:17
 */
using System;

namespace Efz.Logs {
  
  /// <summary>
  /// Represents a single log event.
  /// </summary>
  public interface ILogEvent {
    
    //-------------------------------//
    
    /// <summary>
    /// Type of log event.
    /// </summary>
    LogType Type { get; }
    /// <summary>
    /// Message of the log event.
    /// </summary>
    string Message { get; }
    
    //-------------------------------//
    
    //-------------------------------//
    
    /// <summary>
    /// Write the log event to the console.
    /// </summary>
    void Write();
    
    //-------------------------------//
    
  }
  
}
