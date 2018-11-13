/*
 * User: FloppyNipples
 * Date: 13/03/2017
 * Time: 23:53
 */
using System;
using System.Text;

namespace Efz.Data {
  
  /// <summary>
  /// Hexidecimal data handling.
  /// </summary>
  public static class Hex {
    
    //----------------------------------//
    
    /// <summary>
    /// Hex string lookup table.
    /// </summary>
    private static readonly string[] HexStringTable = {
      "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F",
      "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F",
      "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F",
      "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F",
      "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F",
      "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5A", "5B", "5C", "5D", "5E", "5F",
      "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D", "6E", "6F",
      "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7A", "7B", "7C", "7D", "7E", "7F",
      "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B", "8C", "8D", "8E", "8F",
      "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F",
      "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF",
      "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF",
      "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF",
      "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF",
      "E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF",
      "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF"
    };
    
    public static readonly char[] HexCharTable = {
      '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
    };
    
    //----------------------------------//
    
    /// <summary>
    /// Get a byte collection of the specified string representation of a
    /// hexidecimal number.
    /// </summary>
    public static byte[] ToBytes(this string str) {
      // is the string the correct length?
      if(str.Length == 0 || str.Length % 2 != 0) return new byte[0];
      
      byte[] buffer = BufferCache.Get();
      int length = str.Length / 2;
      char c;
      for (int bx = 0, sx = 0; bx < length; ++bx, ++sx) {
        // convert first half of byte
        c = str[sx];
        buffer[bx] = (byte)((c > Chars.n9 ? (c > Chars.Z ? (c - Chars.a + 10) : (c - Chars.A + 10)) : (c - Chars.n0)) << 4);
        
        // convert second half of byte
        c = str[++sx];
        buffer[bx] |= (byte)(c > Chars.n9 ? (c > Chars.Z ? (c - Chars.a + 10) : (c - Chars.A + 10)) : (c - Chars.n0));
      }
      
      return buffer;
    }
    
    /// <summary>
    /// Get whether the specified character is within the hexadecimal ranges.
    /// </summary>
    public static bool IsHex(char character) {
      int integer = (int)character;
      return integer < 103 && integer > 47 && (
        integer < 58 || integer > 96 ||
        integer > 64 && integer < 71);
    }
    
    /// <summary>
    /// Get a hex string representation of this array of bytes.
    /// </summary>
    public static string ToString(this byte[] bytes) {
      var builder = StringBuilderCache.Get(bytes.Length * 2);
      if (bytes != null) {
        foreach (byte bit in bytes) {
          builder.Append(HexStringTable[bit]);
        }
      }
      return StringBuilderCache.SetAndGet(builder);
    }
    
    /// <summary>
    /// Get a hex string representation.
    /// </summary>
    public static string ToString(byte a) {
      return HexStringTable[a];
    }
    
    /// <summary>
    /// Get a hex string representation.
    /// </summary>
    public static string ToString(byte a, byte b) {
      return HexStringTable[a] + HexStringTable[b];
    }
    
    /// <summary>
    /// Get a hex string representation.
    /// </summary>
    public static string ToString(byte a, byte b, byte c) {
      return String.Join(string.Empty, HexStringTable[a], HexStringTable[b], HexStringTable[c]);
    }
    
    /// <summary>
    /// Get a hex string representation.
    /// </summary>
    public static string ToString(byte a, byte b, byte c, byte d) {
      return String.Join(string.Empty,
        HexStringTable[a],
        HexStringTable[b],
        HexStringTable[c],
        HexStringTable[d]);
    }
    
    /// <summary>
    /// Get a hex string representation.
    /// </summary>
    public static void ToString(StringBuilder builder, byte a) {
      builder.Append(HexStringTable[a]);
    }
    
    /// <summary>
    /// Get a hex string representation.
    /// </summary>
    public static void ToString(StringBuilder builder, byte a, byte b) {
      builder.Append(HexStringTable[a]);
      builder.Append(HexStringTable[b]);
    }
    
    /// <summary>
    /// Get a hex string representation.
    /// </summary>
    public static void ToString(StringBuilder builder, byte a, byte b, byte c) {
      builder.Append(HexStringTable[a]);
      builder.Append(HexStringTable[b]);
      builder.Append(HexStringTable[c]);
    }
    
    /// <summary>
    /// Get a hex string representation.
    /// </summary>
    public static void ToString(StringBuilder builder, byte a, byte b, byte c, byte d) {
      builder.Append(HexStringTable[a]);
      builder.Append(HexStringTable[b]);
      builder.Append(HexStringTable[c]);
      builder.Append(HexStringTable[d]);
    }
    
    /// <summary>
    /// Get a hex color string representation.
    /// </summary>
    public static string ToColor(byte a, byte b, byte c) {
      return IsDouble(a) && IsDouble(b) && IsDouble(c) ?
        String.Join(string.Empty, HexCharTable[a/16], HexCharTable[b/16], HexCharTable[c/16]) :
        String.Join(string.Empty, HexStringTable[a], HexStringTable[b], HexStringTable[c]);
    }
    
    /// <summary>
    /// Get a hex color string representation.
    /// </summary>
    public static string ToColor(byte a, byte b, byte c, byte d) {
      return String.Join(string.Empty,
        HexStringTable[a],
        HexStringTable[b],
        HexStringTable[c],
        HexStringTable[d]);
    }
    
    /// <summary>
    /// Get a hex color string representation.
    /// </summary>
    public static void ToColor(StringBuilder builder, byte a, byte b, byte c) {
      if(IsDouble(a) && IsDouble(b) && IsDouble(c)) {
        builder.Append(HexCharTable[a/16]);
        builder.Append(HexCharTable[b/16]);
        builder.Append(HexCharTable[c/16]);
      } else {
        builder.Append(HexStringTable[a]);
        builder.Append(HexStringTable[b]);
        builder.Append(HexStringTable[c]);
      }
    }
    
    /// <summary>
    /// Get a hex color string representation.
    /// </summary>
    public static void ToColor(StringBuilder builder, byte a, byte b, byte c, byte d) {
      builder.Append(HexStringTable[a]);
      builder.Append(HexStringTable[b]);
      builder.Append(HexStringTable[c]);
      builder.Append(HexStringTable[d]);
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Is the hexidecimal representation of the specified byte two identical characters?
    /// </summary>
    private static bool IsDouble(byte value) {
      switch(value) {
        case 0:
        case 17:
        case 34:
        case 51:
        case 68:
        case 85:
        case 102:
        case 119:
        case 136:
        case 153:
        case 170:
        case 187:
        case 204:
        case 221:
        case 238:
        case 255:
          return true;
        default:
          return false;
      }
    }
    
  }
}
