/*
 * User: FloppyNipples
 * Date: 18/05/2017
 * Time: 18:33
 */
using System;
using System.Net;

namespace Efz.Web {
  
  /// <summary>
  /// Options for sending data to clients.
  /// </summary>
  public class HttpSendOptions {
    
    //----------------------------------//
    
    /// <summary>
    /// Content type header.
    /// Default : null
    /// </summary>
    public string ContentType;
    /// <summary>
    /// Time that the data can be cached in seconds. '0' seconds indicates no cache.
    /// Default : null.
    /// </summary>
    public int? CacheTime = null;
    /// <summary>
    /// Status to assign to the data.
    /// </summary>
    public int StatusCode = 200;
    /// <summary>
    /// Compression of the data to be sent.
    /// Default : Gzip.
    /// </summary>
    public DecompressionMethods Compression = DecompressionMethods.GZip;
    
    //----------------------------------//
    
    //----------------------------------//
    
    //----------------------------------//
    
  }
  
}
