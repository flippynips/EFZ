/*
 * User: Joshua
 * Date: 10/07/2016
 * Time: 3:12 PM
 */

namespace Efz {
  
  /// <summary>
  /// Common states of loadable resources.
  /// </summary>
  [System.FlagsAttribute]
  public enum AssetState : byte {
    Unloaded  = 0x0,
    Loading   = 0x2 | Unloaded,
    Loaded    = 0x4,
    Unloading = 0x8,
    Saving    = 0x16 | Loaded,
    Broken    = 0x64 | Unloaded,
  }
  
  static public class AssetStateExtensions {
    
    /// <summary>
    /// Shorthand for HasFlags.
    /// </summary>
    static public bool Is(this AssetState state, AssetState flags) {
      return (state & flags) == flags;
    }
    
  }
  
}