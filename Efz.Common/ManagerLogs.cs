/*
 * User: FloppyNipples
 * Date: 27/05/2017
 * Time: 22:24
 */
using System;

using Efz.Data;
using Efz.Logs;

namespace Efz {
  
  /// <summary>
  /// Manager of logs.
  /// </summary>
  public class ManagerLogs : Singleton<ManagerLogs> {
    
    //-------------------------------//
    
    /// <summary>
    /// Log manager has high priority.
    /// </summary>
    protected override byte SingletonPriority { get { return 240; } }
    
    //-------------------------------//
    
    /// <summary>
    /// Queue of log events to be written.
    /// </summary>
    private static ActionSequence _sequence;
    /// <summary>
    /// Action roll of log events.
    /// </summary>
    private static ActionRoll<ILogEvent> _roll;
    
    //-------------------------------//
    
    //-------------------------------//
    
    /// <summary>
    /// On setup of the log manager.
    /// </summary>
    protected override void Start() {
      _sequence = new ActionSequence(ManagerUpdate.Polling);
      _roll = new ActionRoll<ILogEvent>(WriteLog);
      Log.OnLog += OnLog;
    }
    
    /// <summary>
    /// On log manager cleared.
    /// </summary>
    protected override void End(Node configuration) {
      Log.OnLog -= OnLog;
    }
    
    /// <summary>
    /// On a new log event.
    /// </summary>
    protected static void OnLog(ILogEvent log) {
      _roll.Add(log);
      _sequence.AddRun(_roll);
    }
    
    /// <summary>
    /// Write a log event.
    /// </summary>
    protected static void WriteLog(ILogEvent log) {
      log.Write();
    }
    
  }
  
}
