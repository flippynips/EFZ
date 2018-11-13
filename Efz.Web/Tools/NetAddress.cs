/*
 * User: FloppyNipples
 * Date: 17/12/2016
 * Time: 11:07
 */
using System;

namespace Efz.Web {
  
  /// <summary>
  /// Represents an ip address and port.
  /// </summary>
  public class NetAddress {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty net address that represents an invalid address.
    /// </summary>
    public static NetAddress Empty { get { return new NetAddress(); } }
    
    /// <summary>
    /// Types of ip addresses.
    /// </summary>
    public enum AddressType : byte {
      Invalid = 0,
      Ipv4    = 1,
      Ipv6    = 2
    }
    
    /// <summary>
    /// Type of ip address.
    /// </summary>
    public AddressType Type;
    
    /// <summary>
    /// Ip address bytes.
    /// </summary>
    public byte[] Address;
    /// <summary>
    /// Port for this ip address if set. '-1' indicates no specific port.
    /// </summary>
    public int Port;
    
    /// <summary>
    /// Get a string representation of the ip address, excluding the port.
    /// </summary>
    public string IpAddress {
      get {
        
        System.Text.StringBuilder sb;
        // determine how to get a string representation of the address
        switch(Type) {
          case AddressType.Ipv4:
            // return the ipv4 address with dot notation
            sb = StringBuilderCache.Get(15);
            sb.Append(Address[0]);
            sb.Append(Chars.Stop);
            sb.Append(Address[1]);
            sb.Append(Chars.Stop);
            sb.Append(Address[2]);
            sb.Append(Chars.Stop);
            sb.Append(Address[3]);
            return sb.ToString();
            
          case AddressType.Ipv6:
            // get a builder for the address
            sb = StringBuilderCache.Get(45);
            bool zeros = true;
            
            // iterate the address bytes
            for(int i = 0; i < Address.Length; ++i) {
              
              // append a separator if needed
              if(i == 4 || i == 8 || i == 12 || i == 16 || i == 20 || i == 24 || i == 28) {
                zeros = true;
                sb.Append(Chars.Colon);
              }
              
              // append the values
              if(zeros && Address[i] == 0) continue;
              
              // no more leading zeros for the current section
              zeros = false;
              // is the address value a hex value
              if(Address[i] > 9) {
                switch(Address[i]) {
                  case 10:
                    sb.Append(Chars.a);
                    break;
                  case 11:
                    sb.Append(Chars.b);
                    break;
                  case 12:
                    sb.Append(Chars.c);
                    break;
                  case 13:
                    sb.Append(Chars.d);
                    break;
                  case 14:
                    sb.Append(Chars.e);
                    break;
                  case 15:
                    sb.Append(Chars.f);
                    break;
                  default:
                    Log.Warning("The ip address wasn't valid. A string representation could not be created.");
                    return null;
                }
              } else {
                sb.Append(Address[i]);
              }
            }
            
            return sb.ToString();
        }
        
        // the address is not valid - return null
        return null;
      }
    }
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Inner empty constructor.
    /// </summary>
    protected NetAddress() {
      Address = null;
      Port = -1;
      Type = AddressType.Invalid;
    }
    
    /// <summary>
    /// Create a new net address from the specified bytes.
    /// The type is set automatically.
    /// </summary>
    public NetAddress(byte[] address, int port = -1) {
      Address = address;
      Port = port;
      switch(address.Length) {
        case 4:
          Type = AddressType.Ipv4;
          break;
        case 16:
          Type = AddressType.Ipv6;
          break;
        default:
          Type = AddressType.Invalid;
          break;
      }
    }
    
    /// <summary>
    /// Create a new net address from the specified integers.
    /// The type is set automatically.
    /// </summary>
    public NetAddress(int[] address, int port = -1) {
      Port = port;
      
      // determine the address type
      switch(address.Length) {
        case 4:
          {
            Type = AddressType.Ipv4;
            Address = new byte[4];
            for(int i = address.Length-1; i > -1; --i) {
              if(address[i] < 256 && address[i] >= 0) {
                Address[i] = (byte)address[i];
              } else {
                Type = AddressType.Invalid;
                break;
              }
            }
          }
          break;
        case 8:
          {
            Type = AddressType.Ipv4;
            Address = new byte[16];
            for(int i = address.Length-1; i > -1; --i) {
              if(address[i] < 65536 && address[i] >= 0) {
                byte[] bytes = BitConverter.GetBytes((short)address[i]);
                Address[i*2] = bytes[0];
                Address[i*2+1] = bytes[1];
              } else {
                Type = AddressType.Invalid;
                break;
              }
            }
          }
          break;
      }
    }
    
    /// <summary>
    /// Get a string representation of the net address.
    /// </summary>
    public override string ToString() {
      
      System.Text.StringBuilder sb;
      // determine how to get a string representation of the address
      switch(Type) {
        case AddressType.Ipv4:
          
          sb = StringBuilderCache.Get(21);
          sb.Append(Address[0]);
          sb.Append(Chars.Stop);
          sb.Append(Address[1]);
          sb.Append(Chars.Stop);
          sb.Append(Address[2]);
          sb.Append(Chars.Stop);
          sb.Append(Address[3]);
          if(Port > -1) {
            sb.Append(Chars.Colon);
            sb.Append(Port);
          }
          
          // return the ipv4 address with dot notation
          return sb.ToString();
          
        case AddressType.Ipv6:
          
          // get a builder for the address
          sb = StringBuilderCache.Get(45);
          bool zeros = true;
          
          // iterate the address bytes
          for(int i = 0; i < Address.Length; ++i) {
            
            // append a separator if needed
            if(i == 4 || i == 8 || i == 12 || i == 16 || i == 20 || i == 24 || i == 28) {
              zeros = true;
              sb.Append(Chars.Colon);
            }
            
            // append the values
            if(zeros && Address[i] == 0) continue;
            
            // no more leading zeros for the current section
            zeros = false;
            // is the address value a hex value
            if(Address[i] > 9) {
              switch(Address[i]) {
                case 10:
                  sb.Append(Chars.A);
                  break;
                case 11:
                  sb.Append(Chars.B);
                  break;
                case 12:
                  sb.Append(Chars.C);
                  break;
                case 13:
                  sb.Append(Chars.D);
                  break;
                case 14:
                  sb.Append(Chars.E);
                  break;
                case 15:
                  sb.Append(Chars.F);
                  break;
                default:
                  Log.Warning("The ip address wasn't valid. A string representation could not be created.");
                  return null;
              }
            } else {
              sb.Append(Address[i]);
            }
          }
          
          if(Port > -1) {
            sb.Append("::");
            sb.Append(Port);
          }
          
          return sb.ToString();
      }
      
      // invalid ip address
      return null;
    }
    
    /// <summary>
    /// Attempt to parse the specified string and generate a net address with defined
    /// address and port parameters. Returns if the parse was successful.
    /// </summary>
    public static bool TryParse(string ipAddress, out NetAddress netAddress) {
      
      // current address being read
      byte[][] address = new byte[9][];
      byte[] section = {16,16,16,16};
      
      // current index within the value representation
      int sIndex = 0;
      int aIndex = 0;
      
      // iterate the ip address string
      for(int i = 0; i < ipAddress.Length; ++i) {
        
        switch(ipAddress[i]) {
          case Chars.Space:
          case Chars.Tab:
          case Chars.Dollar:
          case Chars.BracketSqClose:
          case Chars.BraceClose:
          case Chars.BracketClose:
            // skip whitespace
            break;
          case Chars.Hash:
          case Chars.Asterisk:
          case Chars.Equal:
          case Chars.Underscore:
          case Chars.Dash:
          case Chars.Colon:
          case Chars.Stop:
          case Chars.BraceOpen:
          case Chars.BracketOpen:
          case Chars.BracketSqOpen:
            // add the last value to the address
            address[aIndex] = section;
            ++aIndex;
            
            // should this be the end of the address?
            if(aIndex == 9) {
              // yes, break
              i = ipAddress.Length;
              break;
            }
            
            // refresh the section
            section = new byte[] {16,16,16,16};
            sIndex = 0;
            break;
          case Chars.n0:
            if(sIndex > 3) {
              netAddress = NetAddress.Empty;
              return false;
            }
            section[sIndex] = 0;
            ++sIndex;
            break;
          case Chars.n1:
            if(sIndex > 3) {
              netAddress = NetAddress.Empty;
              return false;
            }
            section[sIndex] = 1;
            ++sIndex;
            break;
          case Chars.n2:
            if(sIndex > 3) {
              netAddress = NetAddress.Empty;
              return false;
            }
            section[sIndex] = 2;
            ++sIndex;
            break;
          case Chars.n3:
            if(sIndex > 3) {
              netAddress = NetAddress.Empty;
              return false;
            }
            section[sIndex] = 3;
            ++sIndex;
            break;
          case Chars.n4:
            if(sIndex > 3) {
              netAddress = NetAddress.Empty;
              return false;
            }
            section[sIndex] = 4;
            ++sIndex;
            break;
          case Chars.n5:
            if(sIndex > 3) {
              netAddress = NetAddress.Empty;
              return false;
            }
            section[sIndex] = 5;
            ++sIndex;
            break;
          case Chars.n6:
            if(sIndex > 3) {
              netAddress = NetAddress.Empty;
              return false;
            }
            section[sIndex] = 6;
            ++sIndex;
            break;
          case Chars.n7:
            if(sIndex > 3) {
              netAddress = NetAddress.Empty;
              return false;
            }
            section[sIndex] = 7;
            ++sIndex;
            break;
          case Chars.n8:
            if(sIndex > 3) {
              netAddress = NetAddress.Empty;
              return false;
            }
            section[sIndex] = 8;
            ++sIndex;
            break;
          case Chars.n9:
            if(sIndex > 3) {
              netAddress = NetAddress.Empty;
              return false;
            }
            section[sIndex] = 9;
            ++sIndex;
            break;
          case Chars.a:
            if(sIndex > 3) {
              netAddress = NetAddress.Empty;
              return false;
            }
            section[sIndex] = 10;
            ++sIndex;
            break;
          case Chars.b:
            if(sIndex > 3) {
              netAddress = NetAddress.Empty;
              return false;
            }
            section[sIndex] = 11;
            ++sIndex;
            break;
          case Chars.c:
            if(sIndex > 3) {
              netAddress = NetAddress.Empty;
              return false;
            }
            section[sIndex] = 12;
            ++sIndex;
            break;
          case Chars.d:
            if(sIndex > 3) {
              netAddress = NetAddress.Empty;
              return false;
            }
            section[sIndex] = 13;
            ++sIndex;
            break;
          case Chars.e:
            if(sIndex > 3) {
              netAddress = NetAddress.Empty;
              return false;
            }
            section[sIndex] = 14;
            ++sIndex;
            break;
          case Chars.f:
            if(sIndex > 3) {
              netAddress = NetAddress.Empty;
              return false;
            }
            section[sIndex] = 15;
            ++sIndex;
            break;
        }
        
      }
      
      // add the section to the address
      address[aIndex] = section;
      ++aIndex;
      
      byte[] result;
      int port = -1;
      // common number of numbers reference
      int numbers;
      
      // determine the type of ip address
      switch(aIndex) {
        case 4:
          // ipv4 with no port
          result = new byte[4];
          
          // add each address segment to the result
          for(int a = 0; a < aIndex; ++a) {
            
            numbers = 1;
            
            // iterate the segment turning the bytes into integers
            for(int b = 3; b >= 0; --b) {
              if(address[a][b] == 16) continue;
              result[a] += (byte)(address[a][b] * numbers);
              // increment the number of numbers
              numbers *= 10;
            }
            
          }
          break;
        case 5:
          // ipv4 with a port
          result = new byte[4];
          
          --aIndex;
          
          // add each address segment to the result
          for(int a = 0; a < aIndex; ++a) {
            
            numbers = 1;
            
            // iterate the segment turning the bytes into integers
            for(int b = 3; b >= 0; --b) {
              if(address[a][b] == 16) continue;
              result[a] += (byte)(address[a][b] * numbers);
              numbers *= 10;
            }
            
          }
          
          numbers = 1;
          
          // derive the port
          for(int b = 3; b >= 0; --b) {
            if(address[aIndex][b] == 16) continue;
            result[aIndex] += (byte)(address[aIndex][b] * numbers);
            numbers *= 10;
          }
          
          break;
        case 8:
          // ipv6 with no port
          result = new byte[16];
          
          // add each address segment to the result
          for(int a = 0; a < aIndex; ++a) {
            
            // get the number of '0' bytes that need to prefix the actual segment
            numbers = 4;
            foreach(byte bit in address[a]) {
              if(bit != 16) --numbers;
            }
            
            // iterate the segment adding '0' byte prefixes where required
            for(int b = 0; b < 4; ++b) {
              
              if(address[a][b] == 16) continue;
              
              result[a * 4 + b + numbers] = address[a][b];
              
            }
            
          }
          break;
        case 9:
          // ipv6 with a port
          result = new byte[16];
          --aIndex;
          
          // add each address segment to the result
          for(int a = 0; a < aIndex-1; ++a) {
            
            // get the number of '0' bytes that need to prefix the actual segment
            numbers = 4;
            foreach(byte bit in address[a]) {
              if(bit != 16) --numbers;
            }
            
            // iterate the segment adding '0' byte prefixes where required
            for(int b = 0; b < 4; ++b) {
              
              if(address[a][b] == 16) continue;
              
              result[a * 4 + b + numbers] = address[a][b];
              
            }
            
          }
          
          numbers = 1;
          
          // derive the port
          for(int b = 3; b >= 0; --b) {
            if(address[aIndex][b] == 16) continue;
            result[aIndex] += (byte)(address[aIndex][b] * numbers);
            numbers *= 10;
          }
          
          break;
        default:
          // there were an unexpected number of sections
          netAddress = NetAddress.Empty;
          return false;
      }
      
      // create the resulting net address
      netAddress = new NetAddress(result, port);
      
      return true;
    }
    
    //-------------------------------------------//
    
  }
  
}
