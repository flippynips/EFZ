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
  public class TransportOptions {
    
    //----------------------------------//
    
    /// <summary>
    /// Compression of the data to be sent.
    /// Default : Gzip.
    /// </summary>
    public DecompressionMethods Compression;
    /// <summary>
    /// If set, the data will be encrypted using the password.
    /// </summary>
    public string EncryptionPassword;
    
    //----------------------------------//
    
    /// <summary>
    /// Serialize the transport options into the specified byte buffer.
    /// </summary>
    public void Serialize(ByteBuffer writer) {
      
      // write a positive flag
      writer.Write(true);
      
      // write the compression
      writer.Write((byte)Compression);
      // write the encryption flag
      writer.Write(EncryptionPassword);
      
    }
    
    /// <summary>
    /// Deserialize transport options. Returns 'Null' if the transport options weren't serialized.
    /// </summary>
    public static TransportOptions Deserialize(ByteBuffer reader) {
      
      // read a flag to indicate a transport option instance follows
      if(!reader.ReadBoolean()) return null;
      
      // create the transport options
      TransportOptions options = new TransportOptions();
      
      // read the compression enum
      options.Compression = (DecompressionMethods)reader.ReadByte();
      // read the encryption flag
      options.EncryptionPassword = reader.ReadString();
      
      return options;
      
    }
    
    //----------------------------------//
    
    //----------------------------------//
    
  }
  
}
