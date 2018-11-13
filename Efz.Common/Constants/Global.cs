/*
 * User: Joshua
 * Date: 21/06/2016
 * Time: 11:43 PM
 */

namespace Efz {
  
  /// <summary>
  /// Common, application-wide definitions.
  /// </summary>
  public static class Global {
    
    /// <summary>
    /// Debug flag.
    /// </summary>
    public const bool Debug = true;
    
    /// <summary>
    /// Whether the system uses a little endian data structure.
    /// </summary>
    public static readonly bool LittleEndian = System.BitConverter.IsLittleEndian;
    /// <summary>
    /// The system specific offset to string data.
    /// </summary>
    public static readonly int OffsetToStringData = System.Runtime.CompilerServices.RuntimeHelpers.OffsetToStringData;
    /// <summary>
    /// The system specific offset to string data divided by two. Common use.
    /// </summary>
    public static readonly int HalfOffsetToStringData = OffsetToStringData / 2;
    
    /// <summary>
    /// The Efz assembly.
    /// </summary>
    public static readonly System.Reflection.Assembly Assembly = typeof(Global).Assembly;
    
    /// <summary>
    /// Bytes in a megabyte.
    /// </summary>
    public const long Gigabyte = 1073741824;
    /// <summary>
    /// Bytes in a megabyte.
    /// </summary>
    public const long Megabyte = 1048576;
    /// <summary>
    /// Bytes in a kilobyte.
    /// </summary>
    public const long Kilobyte = 1024;
    
    /// <summary>
    /// Global buffer size to be used for local transfers.
    /// </summary>
    public const int BufferSizeLocal = 8192;
    /// <summary>
    /// Global buffer size to be used for network transfers.
    /// </summary>
    public const int BufferSizeNet = BufferSizeLocal * 2;
    
  }
}
