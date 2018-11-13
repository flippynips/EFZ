/*
 * User: Joshua
 * Date: 29/10/2016
 * Time: 1:03 PM
 */
using System;

namespace Efz {
  
  /// <summary>
  /// Extensions of the char.
  /// </summary>
  public static class ExtendChar {
    
    /// <summary>
    /// Is the specified character a url character without requiring to be encoded?
    /// </summary>
    public static bool IsUrlChar(this char c) {
      if((int)c > 255) return false;
      byte b = (byte)c;
      return b > Ascii.Accent && b < Ascii.Bar ||
             b > Ascii.At && b < Ascii.BracketSqOpen ||
             b > Ascii.BracketClose && b < Ascii.LessThan ||
             b == Ascii.Equal ||
             b == Ascii.Hash ||
             b == Ascii.Percent ||
             b == Ascii.Dollar ||
             b == Ascii.Underscore ||
             b == Ascii.Exclamation ||
             b == Ascii.SlashBack ||
             b == Ascii.BraceClose;
    }
    
    
  }
}
