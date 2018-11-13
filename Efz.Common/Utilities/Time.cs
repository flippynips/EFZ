using System;
using System.Threading;
using Efz.Threading;

namespace Efz {
  
  /// <summary>
  /// Structure containing thread static time information.
  /// </summary>
  public static class Time {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Number of milliseconds in a second.
    /// </summary>
    public const long Second = 1000;
    /// <summary>
    /// Number of milliseconds in a minute.
    /// </summary>
    public const long Minute = 60000;
    /// <summary>
    /// Number of milliseconds in an hour.
    /// </summary>
    public const long Hour = Minute * 60;
    /// <summary>
    /// Number of milliseconds in a day.
    /// </summary>
    public const long Day = Hour * 24;
    
    /// <summary>
    /// Number of ticks per millisecond.
    /// </summary>
    public readonly static long Frequency;
    /// <summary>
    /// Number of ticks per second.
    /// </summary>
    public readonly static long SecondFrequency;
    
    /// <summary>
    /// Current utc datetime.
    /// </summary>
    public static DateTime Utc {
      get { return new DateTime(_timestamp); }
    }
    
    /// <summary>
    /// Get the current UTC timestamp.
    /// </summary>
    public static long Timestamp {
      get { return _timestamp; }
    }
    
    /// <summary>
    /// Get a unique timestamp.
    /// </summary>
    public static long UniqueTimestamp {
      get {
        _uniqueOffsetUsed = true;
        return _timestamp + Interlocked.Increment(ref _uniqueOffset);
      }
    }
    
    /// <summary>
    /// Get the current timestamp in milliseconds.
    /// </summary>
    public static long Milliseconds {
      get { return _timestamp / Frequency; }
    }
    
    /// <summary>
    /// Number of milliseconds this application has been running.
    /// </summary>
    public static long ApplicationTime {
      get { return (_timestamp - _timeStart) / Frequency; }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Inner timestamp value.
    /// </summary>
    private static long _timestamp;
    /// <summary>
    /// Is the current datetime and timestamp valid?
    /// </summary>
    private static readonly Lock _refresh;
    
    /// <summary>
    /// Timestamp representing initialization.
    /// </summary>
    private readonly static long _timeStart;
    
    /// <summary>
    /// Offset from the current timestamp used for unique timestamps.
    /// </summary>
    private static int _uniqueOffset;
    /// <summary>
    /// Flag to indicate the unique offset has been incremented and should be reset.
    /// </summary>
    private static bool _uniqueOffsetUsed;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize the time class.
    /// </summary>
    static Time() {
      _refresh = new Lock();
      
      SecondFrequency = 10000000L;
      Frequency = (long)(SecondFrequency / 1000.0);
      _timestamp = _timeStart = DateTime.UtcNow.Ticks;
    }
    
    /// <summary>
    /// Update the datetime and timestamp values.
    /// </summary>
    public static void Update() {
      if(_refresh.TryTake) {
        Interlocked.Exchange(ref _timestamp, DateTime.UtcNow.Ticks);
        if(_uniqueOffsetUsed && _uniqueOffset > 5000) {
          _uniqueOffsetUsed = false;
          Interlocked.Exchange(ref _uniqueOffset, 0);
        }
        _refresh.Release();
      }
    }
    
    //-------------------------------------------//
    
  }

}