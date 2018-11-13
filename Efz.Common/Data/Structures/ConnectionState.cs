/*
 * User: Joshua
 * Date: 10/07/2016
 * Time: 3:12 PM
 */

namespace Efz {
  
  /// <summary>
  /// Common states of connections.
  /// </summary>
  [System.FlagsAttribute]
  public enum ConnectionState : byte {
    Closed   = 0x1,
    Openning = 0x2 | 0x1,
    Open     = 0x4,
    Used     = 0x8 | 0x4,
    Broken   = 0x64 | 0x0,
  }
  
  static public class ConnectionStateMeta {
    
    /// <summary>
    /// Shorthand for HasFlags.
    /// </summary>
    static public bool Is(this ConnectionState state, ConnectionState flags) {
      return (state & flags) == flags;
    }
    
  }
  
}