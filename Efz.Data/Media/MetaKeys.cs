/*
 * User: FloppyNipples
 * Date: 02/06/2017
 * Time: 00:28
 */
using System;
using System.Collections.Generic;

namespace Efz.Data.Media {
  
  /// <summary>
  /// Collections and functionality surrounding media metadata keys.
  /// </summary>
  public static class MetaKeys {
    
    //--------------------------------//
    
    /// <summary>
    /// Mapping of exiftool output and metadata keys.
    /// </summary>
    public readonly static Dictionary<string, MetaKey> Map;
    
    //--------------------------------//
    
    //--------------------------------//
    
    static MetaKeys() {
      Map = BuildMap();
    }
    
    /// <summary>
    /// Build the mapping between exiftool metadata key strings and metakey enum values.
    /// </summary>
    private static Dictionary<string, MetaKey> BuildMap() {
      var map = new Dictionary<string, MetaKey> {
        {"Error", MetaKey.Error},
        {"MIME Type", MetaKey.MimeType},
        {"File Size", MetaKey.FileSize},
        {"Image Width", MetaKey.Width},
        {"Image Height", MetaKey.Height},
        {"Duration", MetaKey.Duration},
        {"Video Frame Rate", MetaKey.FrameRate},
        {"Audio Bitrate", MetaKey.Bitrate}
      };
      return map;
    }
    
    //--------------------------------//
    
  }
  
  /// <summary>
  /// Metadata keys.
  /// </summary>
  public enum MetaKey {
    
    //----------------------------//
    
    /// <summary>
    /// Key describing media metadata error.
    /// </summary>
    Error      = 0,
    /// <summary>
    /// Internet mime type of the media.
    /// </summary>
    MimeType   = 1,
    /// <summary>
    /// Size of the media item.
    /// </summary>
    FileSize   = 2,
    /// <summary>
    /// Width of the media. Image or video.
    /// </summary>
    Width      = 3,
    /// <summary>
    /// Height of the media. Image or video.
    /// </summary>
    Height     = 4,
    /// <summary>
    /// Duration of the media. Video or audio.
    /// </summary>
    Duration   = 5,
    /// <summary>
    /// Framerate of the media. Video.
    /// </summary>
    FrameRate  = 6,
    /// <summary>
    /// Bitrate of the media. Video or audio.
    /// </summary>
    Bitrate    = 7,
    
    //----------------------------//
    
  }
  
}
