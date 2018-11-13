/*
 * User: FloppyNipples
 * Date: 26/09/2017
 * Time: 8:47 PM
 */
using System;
using System.Net;

namespace Efz {
  
  /// <summary>
  /// Extend IPEndPoint functionality.
  /// </summary>
  public static class ExtendIPEndPoint {
    
    /// <summary>
    /// Serialize an ip end point.
    /// </summary>
    public static void Serialize(this IPEndPoint endPoint, ByteBuffer writer) {
      
      // write the end point
      writer.Write(endPoint.Port);
      // write the address type
      writer.Write((byte)endPoint.AddressFamily);
      // write the address bytes
      writer.Write(endPoint.Address.GetAddressBytes());
      
    }
    
    /// <summary>
    /// Deserialize an ip end point.
    /// </summary>
    public static IPEndPoint Deserialize(ByteBuffer reader) {
      
      // read the port
      var port = reader.ReadInt32();
      // read the address type
      var addressFamily = (System.Net.Sockets.AddressFamily)reader.ReadByte();
      byte[] addressBytes;
      // read the required bytes
      addressBytes = addressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 ?
        reader.ReadBytes(16) :
        reader.ReadBytes(4);
      
      // return the end point
      return new IPEndPoint(new IPAddress(addressBytes), port);
      
    }
    
    /// <summary>
    /// Try parse an ip address and port from the string.
    /// </summary>
    public static bool TryParseEndPoint(this string address, out IPEndPoint endpoint) {
      
      int port;
      IPAddress ipAddress;
      
      // iterate the address in reverse
      for(int i = address.Length-1; i >= 0; --i) {
        
        if(!Char.IsDigit(address[i])) {
          
          if(address.Split(address[i]).Length == 2 &&
             IPAddress.TryParse(address.Substring(0, i), out ipAddress) &&
             int.TryParse(address.Substring(i+1), out port)) {
            
            endpoint = new IPEndPoint(ipAddress, port);
            return true;
            
          }
          
          endpoint = null;
          return false;
          
        }
        
      }
      
      endpoint = null;
      return false;
      
    }
    
  }
  
}
