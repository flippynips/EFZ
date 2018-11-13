/*
 * User: Joshua
 * Date: 10/07/2016
 * Time: 3:12 PM
 */

namespace Efz {
  
  /// <summary>
  /// Common states of data streams.
  /// </summary>
  [System.FlagsAttribute]
  public enum StreamState : byte {
    Closed    = 0x000,
    Idle      = 0x001 & 0x008 & 0x016,
    WriterSet = 0x002,
    ReaderSet = 0x004,
    // reading and writing are exclusive of each other
    Writing   = 0x008 | 0x002 & 0x016 & 0x001,
    Reading   = 0x016 | 0x004 & 0x008 & 0x001
  }
  
  static public class StreamStateMeta {
    
    /// <summary>
    /// Shorthand for HasFlags.
    /// </summary>
    static public bool Is(this StreamState state, StreamState flags) {
      return (state & flags) == flags;
    }
    
  }
  
}