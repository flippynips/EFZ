/*
 * User: Joshua
 * Date: 4/08/2016
 * Time: 6:19 PM
 */
using System;

namespace Efz {
  
  /// <summary>
  /// Protocols in an enum.
  /// </summary>
  public enum Protocol {
    Unknown = 0,
    File = 1,
    Http = 2,
    Https = 3,
    WebSocket = 4
  }
  
  /// <summary>
  /// Common protocol details.
  /// </summary>
  public static class Protocols {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Return a protocol string.
    /// </summary>
    public static string Get(Protocol protocol) {
      switch(protocol) {
        case Protocol.File:
          return "file://";
        case Protocol.Http:
          return "http://";
        case Protocol.Https:
          return "https://";
        case Protocol.WebSocket:
          return "ws://";
        default:
          return string.Empty;
      }
    }
    
    /// <summary>
    /// Local file path.
    /// </summary>
    public const string File = "file://";
    
    /// <summary>
    /// Http v1.1 or v2.0
    /// </summary>
    public const string Http = "http://";
    /// <summary>
    /// S-Http - Secure layer over http
    /// </summary>
    public const string Https = "https://";
    
    /// <summary>
    /// Web sockets (SOCKS) of any transport protocol.
    /// </summary>
    public const string Socket = "ws://";
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    //-------------------------------------------//
    
    
  }
  
  
  
}
