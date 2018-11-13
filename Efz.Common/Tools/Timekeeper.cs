/*
 * User: Joshua
 * Date: 20/07/2016
 * Time: 8:03 PM
 */
using System;

namespace Efz.Tools {
  
  /// <summary>
  /// Helper class for accurate measurements of time segments.
  /// </summary>
  public class Timekeeper {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Is this time keeper running?
    /// </summary>
    public bool Running { get { return _running; } }
    /// <summary>
    /// Number of milliseconds that have ellapsed since 'Start' was called on this Time instance.
    /// </summary>
    public long Milliseconds {
      get {
        return _running ?
          (Time.Timestamp - _timestampStart) / Time.Frequency :
          (_timestampEnd - _timestampStart) / Time.Frequency;
      }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Inner value for whether the time keeper is running.
    /// </summary>
    protected bool _running;
    /// <summary>
    /// Timestamp that the time keeper was started.
    /// </summary>
    protected long _timestampStart;
    /// <summary>
    /// Timestamp the time keeper was stopped.
    /// </summary>
    protected long _timestampEnd;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Start the timekeeper instance.
    /// </summary>
    public void Start() {
      _running = true;
      _timestampStart = Time.Timestamp;
    }
    
    /// <summary>
    /// Stop the timekeeper instance.
    /// </summary>
    public void Stop() {
      _running = false;
      _timestampEnd = Time.Timestamp;
    }
    
    //-------------------------------------------//
    
  }
  
}
