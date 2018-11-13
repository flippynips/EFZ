/*
 * User: FloppyNipples
 * Date: 26/02/2017
 * Time: 19:09
 */
using System;
using System.Text;

using Efz.Data;

namespace Efz.Web.Display {
  
  /// <summary>
  /// Representation of a web color.
  /// </summary>
  public struct WebColor {
    
    //----------------------------------//
    
    /// <summary>
    /// Red byte channel.
    /// </summary>
    public byte R;
    /// <summary>
    /// Green byte channel.
    /// </summary>
    public byte G;
    /// <summary>
    /// Blue byte channel.
    /// </summary>
    public byte B;
    /// <summary>
    /// Alpha byte channel.
    /// </summary>
    public byte A;
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// Create a web color from the specified string representation. Will throw
    /// if the string isn't valid.
    /// </summary>
    public static WebColor Parse(string str) {
      if(string.IsNullOrEmpty(str)) {
        return WebColor.Black;
      }
      WebColor color;
      if(str[0] == Chars.Hash) {
        if(str.Length == 4) {
          
          byte bit = (byte)str[1];
          color.R = (byte)(bit < Ascii.Colon ? bit - Ascii.n0 : (bit < Ascii.G ? bit - 55 : bit - 87) * 16);
          bit = (byte)str[2];
          color.G = (byte)(bit < Ascii.Colon ? bit - Ascii.n0 : (bit < Ascii.G ? bit - 55 : bit - 87) * 16);
          bit = (byte)str[3];
          color.B = (byte)(bit < Ascii.Colon ? bit - Ascii.n0 : (bit < Ascii.G ? bit - 55 : bit - 87) * 16);
          color.A = 255;
        } else {
          var bytes = str.Substring(1).ToBytes();
          color.R = bytes[0];
          color.G = bytes[1];
          color.B = bytes[2];
          if(bytes.Length > 3) color.A = bytes[3];
          else color.A = 255;
        }
      } else {
        if(str.Length == 3) {
          byte bit = (byte)str[0];
          color.R = (byte)(bit < Ascii.Colon ? bit - Ascii.n0 : (bit < Ascii.G ? bit - 55 : bit - 87) * 16);
          bit = (byte)str[1];
          color.G = (byte)(bit < Ascii.Colon ? bit - Ascii.n0 : (bit < Ascii.G ? bit - 55 : bit - 87) * 16);
          bit = (byte)str[2];
          color.B = (byte)(bit < Ascii.Colon ? bit - Ascii.n0 : (bit < Ascii.G ? bit - 55 : bit - 87) * 16);
          color.A = 255;
        } else {
          var bytes = str.ToBytes();
          color.R = bytes[0];
          color.G = bytes[1];
          color.B = bytes[2];
          if(bytes.Length > 3) color.A = bytes[3];
          else color.A = 255;
        }
      }
      return color;
    }
    
    /// <summary>
    /// Create a web color from the specified string representation. Will throw
    /// if the string isn't valid.
    /// </summary>
    public static bool TryParse(string str, out WebColor color) {
      if(string.IsNullOrEmpty(str)) {
        color = WebColor.Black;
        return false;
      }
      if(str[0] == Chars.Hash) {
        if(str.Length == 4) {
          if(!Hex.IsHex(str[1]) ||
             !Hex.IsHex(str[2]) ||
             !Hex.IsHex(str[3])) {
            color = WebColor.Black;
            return false;
          }
          
          byte bit = (byte)str[1];
          color.R = (byte)(bit < Ascii.Colon ? bit - Ascii.n0 : (bit < Ascii.G ? bit - 55 : bit - 87) * 16);
          bit = (byte)str[2];
          color.G = (byte)(bit < Ascii.Colon ? bit - Ascii.n0 : (bit < Ascii.G ? bit - 55 : bit - 87) * 16);
          bit = (byte)str[3];
          color.B = (byte)(bit < Ascii.Colon ? bit - Ascii.n0 : (bit < Ascii.G ? bit - 55 : bit - 87) * 16);
          color.A = 255;
          
        } else if(str.Length == 7) {
          if(!Hex.IsHex(str[1]) ||
             !Hex.IsHex(str[2]) ||
             !Hex.IsHex(str[3]) ||
             !Hex.IsHex(str[4]) ||
             !Hex.IsHex(str[5]) ||
             !Hex.IsHex(str[6])) {
            
            color = WebColor.Black;
            return false;
          }
          
          var bytes = str.Substring(1).ToBytes();
          color.R = bytes[0];
          color.G = bytes[1];
          color.B = bytes[2];
          if(bytes.Length == 4) color.A = bytes[3];
          else color.A = 255;
        } else {
          color = WebColor.Black;
          return false;
        }
      } else {
        if(str.Length == 3) {
          if(!Hex.IsHex(str[0]) ||
             !Hex.IsHex(str[1]) ||
             !Hex.IsHex(str[2])) {
            
            color = WebColor.Black;
            return false;
          }
          
          byte bit = (byte)str[0];
          color.R = (byte)(bit < Ascii.Colon ? bit - Ascii.n0 : (bit < Ascii.G ? bit - 55 : bit - 87) * 16);
          bit = (byte)str[1];
          color.G = (byte)(bit < Ascii.Colon ? bit - Ascii.n0 : (bit < Ascii.G ? bit - 55 : bit - 87) * 16);
          bit = (byte)str[2];
          color.B = (byte)(bit < Ascii.Colon ? bit - Ascii.n0 : (bit < Ascii.G ? bit - 55 : bit - 87) * 16);
          color.A = 255;
        } else if(str.Length == 6) {
          if(!Hex.IsHex(str[0]) ||
             !Hex.IsHex(str[1]) ||
             !Hex.IsHex(str[2]) ||
             !Hex.IsHex(str[3]) ||
             !Hex.IsHex(str[4]) ||
             !Hex.IsHex(str[5])) {
            
            color = WebColor.Black;
            return false;
          }
          
          var bytes = str.ToBytes();
          color.R = bytes[0];
          color.G = bytes[1];
          color.B = bytes[2];
          if(bytes.Length > 3) color.A = bytes[3];
          else color.A = 255;
        } else {
          color = WebColor.Black;
          return false;
        }
      }
      return true;
    }
    
    /// <summary>
    /// Create a web color from the specified string representation.
    /// Returns '0' if the color isn't valid or the number of characters that constitute the color.
    /// </summary>
    public static int TryParse(string str, int index, out WebColor color) {
      if(str == null || str.Length - index < 3) {
        color = WebColor.Black;
        return 0;
      }
      
      byte[] bytes;
      if(str[index] == Chars.Hash) {
        if(str.Length - index < 4 ||
           !Hex.IsHex(str[index + 1]) ||
           !Hex.IsHex(str[index + 2]) ||
           !Hex.IsHex(str[index + 3])) {
          color = WebColor.Black;
          return 0;
        }
        
        if(str.Length - index < 7 ||
           !Hex.IsHex(str[index + 4]) ||
           !Hex.IsHex(str[index + 5]) ||
           !Hex.IsHex(str[index + 6])) {
          
          byte bit = (byte)str[index + 1];
          color.R = (byte)(bit < Ascii.Colon ? bit - Ascii.n0 : (bit < Ascii.G ? bit - 55 : bit - 87) * 16);
          bit = (byte)str[index + 2];
          color.G = (byte)(bit < Ascii.Colon ? bit - Ascii.n0 : (bit < Ascii.G ? bit - 55 : bit - 87) * 16);
          bit = (byte)str[index + 3];
          color.B = (byte)(bit < Ascii.Colon ? bit - Ascii.n0 : (bit < Ascii.G ? bit - 55 : bit - 87) * 16);
          color.A = 255;
          return 4;
        }
        
        if(str.Length - index < 9 ||
           !Hex.IsHex(str[index + 7]) ||
           !Hex.IsHex(str[index + 8])) {
          
          bytes = str.Substring(1, 6).ToBytes();
          color.R = bytes[0];
          color.G = bytes[1];
          color.B = bytes[2];
          color.A = 255;
          return 7;
        }
        
        bytes = str.Substring(1, 8).ToBytes();
        color.R = bytes[0];
        color.G = bytes[1];
        color.B = bytes[2];
        color.A = bytes[3];
        return 9;
        
      }
      
      if(str.Length - index < 3 ||
         !Hex.IsHex(str[index]) ||
         !Hex.IsHex(str[index + 1]) ||
         !Hex.IsHex(str[index + 2])) {
        
        color = WebColor.Black;
        return 0;
      }
      
      if(str.Length - index < 6 ||
         !Hex.IsHex(str[index + 3]) ||
         !Hex.IsHex(str[index + 4]) ||
         !Hex.IsHex(str[index + 5])) {
        
        byte bit = (byte)str[index + 1];
        color.R = (byte)(bit < Ascii.Colon ? bit - Ascii.n0 : (bit < Ascii.G ? bit - 55 : bit - 87) * 16);
        bit = (byte)str[index + 2];
        color.G = (byte)(bit < Ascii.Colon ? bit - Ascii.n0 : (bit < Ascii.G ? bit - 55 : bit - 87) * 16);
        bit = (byte)str[index + 3];
        color.B = (byte)(bit < Ascii.Colon ? bit - Ascii.n0 : (bit < Ascii.G ? bit - 55 : bit - 87) * 16);
        color.A = 255;
      }
      
      if(str.Length - index < 8 ||
         !Hex.IsHex(str[index + 6]) ||
         !Hex.IsHex(str[index + 7])) {
        
        bytes = str.Substring(index, 6).ToBytes();
        color.R = bytes[0];
        color.G = bytes[1];
        color.B = bytes[2];
        color.A = 255;
        return 6;
      }
      
      bytes = str.Substring(index, 8).ToBytes();
      color.R = bytes[0];
      color.G = bytes[1];
      color.B = bytes[2];
      color.A = bytes[3];
      
      return 8;
    }
    
    /// <summary>
    /// Create a web color of the specified values.
    /// </summary>
    public WebColor(byte r, byte g, byte b, byte a = (byte)255) {
      R = r;
      G = g;
      B = b;
      A = a;
    }
    
    /// <summary>
    /// Create a web color of the specified values.
    /// </summary>
    public WebColor(int r, int g, int b, int a = 255) {
      R = (byte)r;
      G = (byte)g;
      B = (byte)b;
      A = (byte)a;
    }
    
    /// <summary>
    /// Multiply the web color by the specified modifier. Optionally include the
    /// alpha channel.
    /// </summary>
    public WebColor Multiply(float modifier, bool alpha = false) {
      return new WebColor(
        R = (byte)Meth.MinMax(R * modifier, 0, 255),
        G = (byte)Meth.MinMax(G * modifier, 0, 255),
        B = (byte)Meth.MinMax(B * modifier, 0, 255),
        alpha ? A = (byte)Meth.MinMax(A * modifier, 0, 255) : A
      );
    }
    
    /// <summary>
    /// Modify the shade of the web color by maintaining the ratio of
    /// hues while either making the color lighter or darker. Values
    /// between 0.0-1.0 to darken and 1.0-2.0 to lighten.
    /// </summary>
    public WebColor Shade(float shade, bool alpha = false) {
      if(shade > 1.0f) {
        shade -= 1.0f;
        return new WebColor(
          R = (byte)(R + (255 - R) * shade),
          G = (byte)(G + (255 - G) * shade),
          B = (byte)(B + (255 - B) * shade),
          alpha ? A = (byte)(A + (255 - A) * shade) : A
        );
      }
      
      return new WebColor(
        R = (byte)(R * shade),
        G = (byte)(G * shade),
        B = (byte)(B * shade),
        alpha ? A = (byte)(A * shade) : A
      );
    }
    
    /// <summary>
    /// Append the web color to the specified string builder, ignoring the alpha
    /// channel.
    /// </summary>
    public void ToString(StringBuilder builder) {
      builder.Append(Chars.Hash);
      Hex.ToColor(builder, R, G, B);
    }
    
    /// <summary>
    /// Append the web color to the specified string builder, ignoring the alpha
    /// channel.
    /// </summary>
    public void ToStringAlpha(StringBuilder builder) {
      builder.Append(Chars.Hash);
      Hex.ToColor(builder, R, G, B, A);
    }
    
    /// <summary>
    /// Get the web color as a hex string representation, ignoring the alpha
    /// channel.
    /// </summary>
    public override string ToString() {
      return Chars.Hash + Hex.ToColor(R, G, B);
    }
    
    /// <summary>
    /// Get the web color as a hex string representation.
    /// </summary>
    public string ToStringAlpha() {
      return Chars.Hash + Hex.ToColor(R, G, B, A);
    }
    
    /// <summary>
    /// Get a web color implicitly as a string. This doesn't include the alpha
    /// channel.
    /// </summary>
    public static implicit operator string(WebColor webColor) {
      return webColor.ToString();
    }
    
    /// <summary>
    /// Add one web color to another.
    /// </summary>
    public static WebColor operator +(WebColor valueA, WebColor valueB) {
      return new WebColor(
        valueA.R + valueB.R,
        valueA.G + valueB.G,
        valueA.B + valueB.B,
        valueA.A + valueB.A);
    }
    
    /// <summary>
    /// Subtract one web color from another.
    /// </summary>
    public static WebColor operator -(WebColor valueA, WebColor valueB) {
      return new WebColor(
        valueA.R - valueB.R,
        valueA.G - valueB.G,
        valueA.B - valueB.B,
        valueA.A - valueB.A);
    }
    
    /// <summary>
    /// Multiply a web color with another.
    /// </summary>
    public static WebColor operator *(WebColor valueA, WebColor valueB) {
      return new WebColor(
        valueA.R * valueB.R,
        valueA.G * valueB.G,
        valueA.B * valueB.B,
        valueA.A * valueB.A);
    }
    
    /// <summary>
    /// Divide a web color with another.
    /// </summary>
    public static WebColor operator /(WebColor valueA, WebColor valueB) {
      return new WebColor(
        valueA.R / valueB.R,
        valueA.G / valueB.G,
        valueA.B / valueB.B,
        valueA.A / valueB.A);
    }
    
    /// <summary>
    /// Add a floating point value to a web color.
    /// </summary>
    public static WebColor operator +(WebColor valueA, float valueB) {
      return new WebColor(
        (byte)(valueA.R + valueB),
        (byte)(valueA.G + valueB),
        (byte)(valueA.B + valueB),
        (byte)(valueA.A + valueB));
    }
    
    /// <summary>
    /// Subtract one scalar value from another.
    /// </summary>
    public static WebColor operator -(WebColor valueA, float valueB) {
      return new WebColor(
        (byte)(valueA.R - valueB),
        (byte)(valueA.G - valueB),
        (byte)(valueA.B - valueB),
        (byte)(valueA.A - valueB));
    }
    
    /// <summary>
    /// Subtract one scalar value from another.
    /// </summary>
    public static WebColor operator *(WebColor valueA, float valueB) {
      return new WebColor(
        (byte)(valueA.R * valueB),
        (byte)(valueA.G * valueB),
        (byte)(valueA.B * valueB),
        (byte)(valueA.A * valueB));
    }
    
    /// <summary>
    /// Subtract one scalar value from another.
    /// </summary>
    public static WebColor operator /(WebColor valueA, float valueB) {
      return new WebColor(
        (byte)(valueA.R / valueB),
        (byte)(valueA.G / valueB),
        (byte)(valueA.B / valueB),
        (byte)(valueA.A / valueB));
    }
    
    /// <summary>
    /// Subtract one scalar value from another.
    /// </summary>
    public static WebColor operator +(float valueA, WebColor valueB) {
      return new WebColor(
        (byte)(valueA + valueB.R),
        (byte)(valueA + valueB.G),
        (byte)(valueA + valueB.B),
        (byte)(valueA + valueB.A));
    }
    
    /// <summary>
    /// Subtract one scalar value from another.
    /// </summary>
    public static WebColor operator -(float valueA, WebColor valueB) {
      return new WebColor(
        (byte)(valueA - valueB.R),
        (byte)(valueA - valueB.G),
        (byte)(valueA - valueB.B),
        (byte)(valueA - valueB.A));
    }
    
    /// <summary>
    /// Subtract one scalar value from another.
    /// </summary>
    public static WebColor operator *(float valueA, WebColor valueB) {
      return new WebColor(
        (byte)(valueA * valueB.R),
        (byte)(valueA * valueB.G),
        (byte)(valueA * valueB.B),
        (byte)(valueA * valueB.A));
    }
    
    /// <summary>
    /// Subtract one scalar value from another.
    /// </summary>
    public static WebColor operator /(float valueA, WebColor valueB) {
      return new WebColor(
        (byte)(valueA / valueB.R),
        (byte)(valueA / valueB.G),
        (byte)(valueA / valueB.B),
        (byte)(valueA / valueB.A));
    }
    
    #region colors
    public static WebColor Black = new WebColor((byte)0, (byte)0, (byte)0);
    public static WebColor DarkGrey = new WebColor((byte)50, (byte)50, (byte)50);
    public static WebColor Grey = new WebColor((byte)125, (byte)125, (byte)125);
    public static WebColor LightGrey = new WebColor((byte)200, (byte)200, (byte)200);
    public static WebColor White = new WebColor((byte)0, (byte)0, (byte)0);
    
    public static WebColor Red = new WebColor((byte)255, (byte)0, (byte)0);
    public static WebColor Blue = new WebColor((byte)0, (byte)255, (byte)0);
    public static WebColor Green = new WebColor((byte)0, (byte)0, (byte)255);
    
    public static WebColor Cyan = new WebColor((byte)0, (byte)200, (byte)200);
    public static WebColor Magenta = new WebColor((byte)200, (byte)0, (byte)200);
    public static WebColor Yellow = new WebColor((byte)200, (byte)200, (byte)0);
    
    public static WebColor Orange = new WebColor((byte)255, (byte)136, (byte)0);
    public static WebColor Lime = new WebColor((byte)136, (byte)255, (byte)0);
    public static WebColor Pink = new WebColor((byte)255, (byte)0, (byte)136);
    public static WebColor Purple = new WebColor((byte)136, (byte)0, (byte)255);
    public static WebColor Azure = new WebColor((byte)0, (byte)136, (byte)255);
    public static WebColor Emerald = new WebColor((byte)0, (byte)255, (byte)136);
    #endregion
    
    //----------------------------------//
    
  }
  
}
