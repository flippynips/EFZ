/*
 * User: Joshua
 * Date: 3/06/2016
 * Time: 8:49 PM
 */
using System;

using System.Runtime.CompilerServices;
using System.Text;
using Efz.Collections;

namespace Efz {
  
  /// <summary>
  /// Basic helper string extensions.
  /// </summary>
  public static class ExtendString {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Fast replacement of characters.
    /// </summary>
    public unsafe static string Swap(this string str, char existing, char replace) {
      char* ptr;
      int index = str.Length;
      fixed(char* strPtr = str) {
        ptr = strPtr;
        // iterate the string
        while(--index >= 0) {
          if(*ptr == existing) {
            *ptr = replace;
          }
          ++ptr;
        }
      }
      return str;
    }
    
    /// <summary>
    /// Get a sub-section of a string from character indexStart to indexEnd
    /// exclusive.
    /// </summary>
    public static string Section(this string str, int indexStart, int indexEnd) {
      return str.Substring(indexStart, indexEnd - indexStart);
    }
    
    /// <summary>
    /// Retrieve the index of the specified string within the character array if it exists.
    /// Returns the starting index of the string or '-1' if it doesn't exist.
    /// </summary>
    public unsafe static int IndexOf(this char[] chars, string str) {
      int index = 0;
      int strIndex = 0;
      int last = chars.Length - str.Length;
      
      char* charA;
      char* charB;
      fixed(char* ptrA = chars)
      fixed(char* ptrB = str) {
        charA = ptrA;
        charB = ptrB;
        while(index < last) {
          // increment the indices
          ++index;
          ++strIndex;
          
          // does the character match?
          if(charA == charB) {
            // yes, is this the last character?
            if(strIndex == str.Length) {
              // yes, return the index of the string
              return index - strIndex;
            }
          } else {
            // no, reset the string index
            strIndex = 0;
            // reset the character pointer
            charB = ptrB;
          }
          
          // increment the character pointers
          ++charA;
          ++charB;
        }
      }
      
      // return no index
      return -1;
    }
    
    /// <summary>
    /// Retrieve the first index of any of the specified characters in the string from the index specified
    /// if it exists or -1 if none of the characters are found.
    /// </summary>
    public unsafe static int IndexOf(this string str, int indexStart, params char[] characters) {
      
      int last = str.Length;
      
      char* charA;
      fixed(char* ptrA = str) {
        charA = ptrA + indexStart;
        
        // iterate characters in the string
        while(indexStart < last) {
          
          // does the character match?
          foreach(var charB in characters) {
            if(charB == *charA) {
              // yes
              return indexStart;
            }
          }
          
          // increment the index and pointer
          ++indexStart;
          ++charA;
        }
      }
      
      // return no index
      return -1;
      
    }
    
    /// <summary>
    /// Retrieve the last index of any of the specified characters in the string iterating backward from the index specified
    /// if it exists or -1 if none of the characters are found.
    /// </summary>
    public unsafe static int LastIndexOf(this string str, int indexEnd, params char[] characters) {
      
      char* charA;
      fixed(char* ptrA = str) {
        charA = ptrA + indexEnd;
        
        // iterate characters in the string
        while(indexEnd >= 0) {
          
          // does the character match?
          foreach(var charB in characters) {
            if(charB == *charA) {
              // yes
              return indexEnd;
            }
          }
          
          // increment the index and pointer
          --indexEnd;
          --charA;
        }
      }
      
      // return no index
      return -1;
      
    }
    
    /// <summary>
    /// Split a string into two sections and return each side of the result. Optionally specify
    /// character offsets from the specified index for each section.
    /// </summary>
    public unsafe static Struct<string,string> Split(this string str, int index, int offsetA = 0, int offsetB = 0) {
      fixed(char* ptr = str) {
        return new Struct<string, string>(
          new string(ptr, 0, index + offsetA),
          new string(ptr, index + offsetB + 1, str.Length - index - offsetB - 1));
      }
    }
    
    /// <summary>
    /// Simple check for if the specified string contains any of the specified characters.
    /// </summary>
    public static bool Contains(this string str, params char[] characters) {
      foreach(char strChar in str) foreach(char character in characters) {
        if(strChar == character) return true;
      }
      return false;
    }
    
    /// <summary>
    /// Compare strings.
    /// </summary>
    public static int Compare(this string strA, string strB) {
      return string.CompareOrdinal(strA, strB);
    }
    
    /// <summary>
    /// Escape the specified separator in the string.
    /// </summary>
    public static string Escape(this string str, char separator) {
      
      // get a builder for the escaped string
      var sb = StringBuilderCache.Get();
      // iterate the string to be escaped
      foreach(char c in str) {
        sb.Append(c);
        if(c == separator) sb.Append(c);
      }
      
      // return the completed string
      return StringBuilderCache.SetAndGet(sb);
      
    }
    
    /// <summary>
    /// Escape the specified separator in the string.
    /// </summary>
    public static string Escape(this string str, params char[] separators) {
      
      // get a builder for the escaped string
      var sb = StringBuilderCache.Get();
      // iterate the string to be escaped
      foreach(char c in str) {
        sb.Append(c);
        if(separators.Contains(c)) sb.Append(c);
      }
      
      // return the completed string
      return StringBuilderCache.SetAndGet(sb);
      
    }
    
    /// <summary>
    /// Creates an iterator for split string with the possibility of escaping
    /// with two separators which will be reduced to one.
    /// </summary>
    public static ArrayRig<string> SplitEscaped(this string str, char separator) {
      // the collection of strings
      ArrayRig<string> split = new ArrayRig<string>();
      int indexStart = 0;
      int indexEnd   = 0;
      bool removeEscape = false;
      
      while(true) {
        // get the next index
        indexEnd = str.IndexOf(separator, indexStart);
        if(indexEnd > -1 && indexEnd+1 < str.Length) {
          // check the next character
          if(str[indexEnd+1] == separator) {
            removeEscape = true;
          } else {
            // do we have to escape double characters in this string?
            if(removeEscape) {
              split.Add(str.Section(indexStart, indexEnd).Replace(separator + separator.ToString(), separator.ToString()));
            } else {
              split.Add(str.Section(indexStart, indexEnd));
            }
          }
          indexStart = indexEnd + 1;
        } else {
          // add the last string to the collection
          if(removeEscape) {
            split.Add(str.Section(indexStart, str.Length).Replace(separator + separator.ToString(), separator.ToString()));
          } else {
            split.Add(str.Section(indexStart, str.Length));
          }
          break;
        }
      }
      return split;
    }
    
    /// <summary>
    /// Returns whether the specified string is an integer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static bool IsInt(this string str) {
      int index;
      fixed(char* digits = str) {
        if(*digits == Chars.Dash) {
          index = str.Length-2;
          ++*digits;
        } else {
          index = str.Length-1;
        }
        while (--index > -1) {
          if (*digits > 57 || *digits < 48) {
            return false;
          }
          ++*digits;
        }
      }
      return true;
    }
    
    /// <summary>
    /// Read an ascii encoded string from a binary reader until the specified breaker byte is hit.
    /// </summary>
    static public string ReadString(this System.IO.BinaryReader reader, byte separator) {
     StringBuilder sb = StringBuilderCache.Get();
      byte b = reader.ReadByte();
      while(b != separator) {
        sb.Append((char)b);
        b = reader.ReadByte();
      }
      return StringBuilderCache.SetAndGet(sb);
    }
    
    /// <summary>
    /// Join a collection of items into a string with an optional delimiter.
    /// </summary>
    static public string Join(this object[] items, char delimiter = Chars.Null, int count = -1) {
      StringBuilder sb;
      if(count == -1) {
        sb = StringBuilderCache.Get();
        if(delimiter == Chars.Null) {
          foreach(object item in items) {
            sb.Append(item.ToString());
          }
        } else {
          bool first = true;
            foreach(object item in items) {
              if(first) first = false;
              else sb.Append(delimiter);
              sb.Append(item.ToString());
            }
          }
      } else {
        if(count == 0) return string.Empty;
        sb = StringBuilderCache.Get();
        if(delimiter == Chars.Null) {
          foreach(object item in items) {
            sb.Append(item.ToString());
            if(--count == 0) return StringBuilderCache.SetAndGet(sb);
          }
        } else {
          bool first = true;
          foreach(object item in items) {
            if(first) first = false;
            else sb.Append(delimiter);
            sb.Append(item.ToString());
            if(--count == 0) return StringBuilderCache.SetAndGet(sb);
          }
        }
      }
      
      return StringBuilderCache.SetAndGet(sb);
    }
    
    /// <summary>
    /// Join a collection of items into a string with an optional delimiter.
    /// </summary>
    static public string Join(this object[] items, string delimiter, int count = -1) {
      StringBuilder sb;
      if(count == -1) {
        sb = StringBuilderCache.Get();
        if(delimiter == null) {
          foreach(object item in items) {
            sb.Append(item.ToString());
          }
        } else {
          bool first = true;
          foreach(object item in items) {
            if(first) first = false;
            else sb.Append(delimiter);
            sb.Append(item.ToString());
          }
        }
      } else {
        if(count == 0) return string.Empty;
        sb = StringBuilderCache.Get();
        if(delimiter == null) {
          foreach(object item in items) {
            sb.Append(item.ToString());
          }
        } else {
          bool first = true;
          foreach(object item in items) {
            if(first) first = false;
            else sb.Append(delimiter);
            sb.Append(item.ToString());
          }
        }
      }
      
      return StringBuilderCache.SetAndGet(sb);
    }
    
    /// <summary>
    /// Set all the characters as the uppercase version. Faster than ToUpper().
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static string ToUppercase(this string str) {
      int index = str.Length;
      char* ptr;
      fixed(char* ch = str) {
        ptr = ch;
        while (--index >= 0) {
          // is the character a lower case alphabetic?
          if (*ptr > 96 && *ptr < 123) {
            // yes, make the character its uppercase version
            *ptr = (char)(*ptr - 32);
          }
          ++ptr;
        }
      }
      return str;
    }
    
    /// <summary>
    /// Set all the characters as the lowercase version. Faster than ToLower().
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static string ToLowercase(this string str) {
      int index = str.Length;
      char* ptr;
      fixed(char* ch = str) {
        ptr = ch;
        while (--index >= 0) {
          // is the character a upper case alphabetic?
          if (*ptr > 64 && *ptr < 91) {
            // yes, make the character its lower case version
            *ptr = (char)(*ptr + 32);
          }
          ++ptr;
        }
      }
      return str;
    }
    
    // ----------------------------------------------------- Conversions //
    
    /// <summary>
    /// Simple determination if a string represents a boolean value of false.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ToBool(this string str) {
      return !(string.IsNullOrEmpty(str) || string.Equals(str, "false", StringComparison.OrdinalIgnoreCase));
    }
    
    /// <summary>
    /// Get a 16bit integer value represented by a string.
    /// The string must be correctly formatted.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static short ToInt16(this string str) {
      bool negative = false;
      int scale;
      short result = 0;
      char* digits;
      fixed(char* pointer = str) {
        digits = pointer;
        if(*digits == 45) {
          ++digits;
          negative = true;
          scale = str.Length-1;
        } else {
          scale = str.Length;
        }
        while (--scale > -1) {
          result *= 10;
          result += (short)(*digits - 48);
          ++digits;
        }
      }
      // calculate the sign
      if(negative) {
        result = (short)-result;
        // precision catch
        if (result > 0) {
          return 0;
        }
      } else {
        // precision catch
        if (result < 0) {
          return 0;
        }
      }
      return result;
    }
    
    /// <summary>
    /// Get an unsigned 16bit integer value represented by a string.
    /// The string must be correctly formatted.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static ushort ToUInt16(this string str) {
      int scale = str.Length-1;
      ushort result = 0;
      int integer = 0;
      char* digits;
      fixed(char* pointer = str) {
        digits = pointer;
        while (--scale >= 0) {
          result *= 10;
          integer += *digits - 48;
          ++digits;
          if(integer > ExtendMath.IntMaxBuffer) {
            result += (ushort)integer;
            integer = 0;
          }
        }
      }
      return (ushort)(result + (ushort)integer);
    }
    
    /// <summary>
    /// Get a 32bit integer value represented by a string.
    /// The string must be correctly formatted.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static int ToInt32(this string str) {
      bool negative = false;
      int scale;
      int result = 0;
      char* digits;
      fixed(char* pointer = str) {
        digits = pointer;
        if(*digits == 45) {
          ++digits;
          negative = true;
          scale = str.Length-1;
        } else {
          scale = str.Length;
        }
        while (--scale >= 0) {
          result *= 10;
          result += *digits - 48;
          ++digits;
        }
      }
      // calculate the sign
      if(negative) {
        result = -result;
        // precision catch
        if (result > 0) return 0;
      } else {
        // precision catch
        if (result < 0) return 0;
      }
      return result;
    }
    
    /// <summary>
    /// Get an unsigned 32bit integer value represented by a string.
    /// The string must be correctly formatted.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static uint ToUInt32(this string str) {
      int scale = str.Length;
      uint result = 0u;
      int integer = 0;
      char* digits;
      fixed(char* pointer = str) {
        digits = pointer;
        while (--scale >= 0) {
          result *= 10;
          integer += *digits - 48;
          ++digits;
          if(integer > ExtendMath.IntMaxBuffer) {
            result += (uint)integer;
            integer = 0;
          }
        }
      }
      return result + (uint)integer;
    }
    
    /// <summary>
    /// Get a 64bit integer value represented by a string.
    /// The string must be correctly formatted.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static long ToInt64(this string str) {
      bool negative = false;
      int scale;
      long result = 0L;
      char* digits;
      fixed(char* pointer = str) {
        digits = pointer;
        if(*digits == 45) {
          ++digits;
          negative = true;
          scale = str.Length-1;
        } else {
          scale = str.Length;
        }
        while (--scale >= 0) {
          result *= 10;
          result += *digits - 48;
          ++digits;
        }
      }
      // calculate the sign
      if(negative) {
        result = -result;
        // precision catch
        if (result > 0) return 0;
      } else {
        // precision catch
        if (result < 0) return 0;
      }
      return result;
    }
    
    /// <summary>
    /// Get an unsigned 64bit integer value represented by a string.
    /// The string must be correctly formatted.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static ulong ToUInt64(this string str) {
      int length = str.Length;
      ulong result = 0UL;
      char* digits;
      fixed(char* pointer = str) {
        digits = pointer;
        while (--length >= 0) {
          result *= 10;
          result += (ulong)(*digits - 48);
          ++digits;
        }
      }
      return result;
    }
    
    /// <summary>
    /// Get a floating point value represented by a string.
    /// The string must be correctly formatted.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static float ToFloat(this string str) {
     return (float)ToDouble(str);
    }
    
    /// <summary>
    /// Get a double value represented by a string.
    /// The string must be correctly formatted.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static double ToDouble(this string str) {
      bool negative = false;
      bool exponent = false;
      bool fraction = false;
      int fractionScale = 10;
      int scale;
      double result = 0;
      double exp = 0;
      char* digits;
      fixed(char* pointer = str) {
        digits = pointer;
        if(*digits == 45) {
          ++digits;
          negative = true;
          scale = str.Length-1;
        } else {
          scale = str.Length;
        }
        while (--scale >= 0) {
          switch(*digits) {
            case Chars.Stop:
              fraction = true;
              fractionScale = 10;
              break;
            case Chars.e:
            case Chars.E:
              exponent = true;
              fraction = false;
              break;
            default:
              if(exponent) {
                if(fraction) {
                  exp += (*digits - 48) / fractionScale;
                  fractionScale += 10;
                } else {
                  exp *= 10;
                  exp += *digits - 48;
                }
              } else {
                if(fraction) {
                  result += (*digits - 48) / fractionScale;
                  fractionScale += 10;
                } else {
                  result *= 10;
                  result += *digits - 48;
                }
              }
              break;
          }
          ++digits;
        }
      }
      // calculate the sign
      if(negative) {
        result = -result;
        // precision catch
        if (result > 0) {
          return 0;
        }
      } else {
        // precision catch
        if (result < 0) {
          return 0;
        }
      }
      return Math.Pow(result, exp);
    }
    
    /// <summary>
    /// Does the string represent a guid?
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static bool IsGuid(this string str) {
      
      // check the structure of the guid
      if(str == null || str.Length != 36 ||
         str[8] != Chars.Dash || str[13] != Chars.Dash ||
         str[18] != Chars.Dash || str[23] != Chars.Dash) return false;
      
      // iterate characters in the string
      foreach(var c in str) {
        // is the character in the hexidecimal range
        if((c != Chars.Dash) &&
           (c < Ascii.n0 || c > Ascii.n9) &&
           (c < Ascii.A || c > Ascii.F) &&
           (c < Ascii.a || c > Ascii.f)) {
          return false;
        }
      }
      
      return true;
    }
    
    /// <summary>
    /// Get a representation of an integer as a-z letters only.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static string AsLetters(this int integer) {
      
      if(integer < 26) return ((char)(integer + 65)).ToString();
      
      string values = string.Empty;
      while(integer > 25) {
        int mod = integer % 26;
        integer = (integer-mod) / 26 - 1;
        values = (char)(mod + 65) + values;
      }
      
      return (char)(integer + 65) + values;
    }
    
    /// <summary>
    /// Truncate the string to the specified length. If the string is shorter
    /// than the specified length, nothing happens.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static string Truncate(this string str, int length) {
      return str.Length > length ? str.Substring(0, length) : str;
    }
    
    /// <summary>
    /// Truncate the string to the specified length. If the string is truncated,
    /// the specified suffix is added.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static string Truncate(this string str, int length, string suffix) {
      return str.Length > length ? str.Substring(0, length)+suffix : str;
    }
    
    /// <summary>
    /// Parse the escaped string for the escaped characters, swapping single instances
    /// with the replacement character and reducing double instances with a single
    /// escaped character.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static string UnEscape(this string str, char escaped, char replacement) {
      StringBuilder builder = StringBuilderCache.Get();
      int index = str.Length;
      char* pointer;
      bool gotEscaped = false;
      fixed(char* strPtr = str) {
        pointer = strPtr;
        while(--index >= 0) {
          if(gotEscaped) {
            if(*pointer == escaped) {
              builder.Append(escaped);
            } else {
              builder.Append(replacement);
              builder.Append(*pointer);
            }
          } else if(*pointer == escaped) {
            gotEscaped = true;
          } else {
            builder.Append(*pointer);
          }
          ++pointer;
        }
      }
      return StringBuilderCache.SetAndGet(builder);
    }
    
    /// <summary>
    /// Remove excess spaces and line breaks from the start and end of the specified string.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static string TrimSpace(this string str) {
      int length = str.Length;
      if(length == 0) return str;
      int index = 0;
      char* pointer;
      fixed(char* strPtr = str) {
        pointer = strPtr;
        while(index < length && *pointer == Chars.Space || *pointer == Chars.NewLine || *pointer == Chars.CarriageReturn) {
          ++index;
          ++pointer;
        }
        if(index == length) return string.Empty;
        int end = length;
        pointer = strPtr + length - 1;
        while(*pointer == Chars.Space || *pointer == Chars.NewLine || *pointer == Chars.CarriageReturn) {
          --end;
          --pointer;
        }
        // return a trimmed string
        return new string(strPtr, index, end - index);
      }
    }
    
    /// <summary>
    /// Remove the specified set of characters from the string.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static string Remove(this string str, params char[] characters) {
      StringBuilder builder = StringBuilderCache.Get();
      int index = str.Length;
      char* pointer;
      fixed(char* strPtr = str) {
        pointer = strPtr;
        while(index >= 0) {
          if(!characters.Contains(*pointer)) {
            builder.Append(*pointer);
          }
          --index;
          ++pointer;
        }
      }
      return builder.ToString();
    }
    
    /// <summary>
    /// Get a byte array representing the specified string. It is assumed the string
    /// is ASCII only. Use an encoder for all other strings.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static byte[] GetBytes(this string str) {
      if(string.IsNullOrEmpty(str)) return new byte[0];
      int length = str.Length;
      int index = 0;
      byte[] bytes = new byte[length];
      while(index < length) {
        bytes[index] = (byte)str[index];
        ++index;
      }
      return bytes;
    }
    
    /// <summary>
    /// Get the string as an integer array.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static int[] GetInts(this string str) {
      if(string.IsNullOrEmpty(str)) return new int[0];
      int length = str.Length;
      int index = 0;
      int[] ints = new int[length];
      while(index < length) {
        ints[index] = (int)str[index];
        ++index;
      }
      return ints;
    }
    
    //-------------------------------------------//
    
  }
  
}
