/*
 * User: FloppyNipples
 * Date: 02/01/2017
 * Time: 14:13
 */
using System;
using System.Text;

namespace Efz {
  
  /// <summary>
  /// Formatting helpers for global types.
  /// </summary>
  public static class Format {
    
    /// <summary>
    /// Format for a date time string. "dd/MM/yyyy HH:mm".
    /// </summary>
    public static void ToShortFormat(this DateTime dateTime, StringBuilder sb) {
      dateTime.Day.ToString(sb, 2);
      sb.Append(Chars.ForwardSlash);
      dateTime.Month.ToString(sb, 2);
      sb.Append(Chars.ForwardSlash);
      sb.Append(Convert.ToString(dateTime.Year));
      sb.Append(Chars.Space);
      dateTime.Hour.ToString(sb, 2);
      sb.Append(Chars.Colon);
      dateTime.Minute.ToString(sb, 2);
    }
    
    /// <summary>
    /// Format for a date time string. "dd/MM/yyyy HH:mm".
    /// </summary>
    public static string ToShortFormat(this DateTime dateTime) {
      var sb = StringBuilderCache.Get(16);
      dateTime.Day.ToString(sb, 2);
      sb.Append(Chars.ForwardSlash);
      dateTime.Month.ToString(sb, 2);
      sb.Append(Chars.ForwardSlash);
      sb.Append(Convert.ToString(dateTime.Year));
      sb.Append(Chars.Space);
      dateTime.Hour.ToString(sb, 2);
      sb.Append(Chars.Colon);
      dateTime.Minute.ToString(sb, 2);
      return StringBuilderCache.SetAndGet(sb);
    }
    
    /// <summary>
    /// Format for a date time string. "ddd, d MMM yyyy HH:mm:ss".
    /// </summary>
    public static string ToLongFormat(this DateTime dateTime) {
      const string format = "ddd, d MMM yyyy HH:mm:ss";
      return dateTime.ToString(format) + " UTC";
    }
    
    /// <summary>
    /// Returns a string of the specified integer filled with the specified number of '0's.
    /// If the integer will not be contained within the specified number of integers, it
    /// is returned as a string unchanged.
    /// </summary>
    public static void ToString(this int integer, StringBuilder sb, int numberOfCharacters) {
      // slightly different logic if the representation will include a '-' sign
      if(integer < 0) {
        if(--numberOfCharacters < 2) {
          sb.Append(Convert.ToString(integer));
        } else {
          if(Math.Pow(10, --numberOfCharacters) > integer) {
            integer = -integer;
            sb.Append(Chars.Dash);
            while(numberOfCharacters > 0 && Math.Pow(10, numberOfCharacters) > integer) {
              sb.Append(Chars.n0);
              --numberOfCharacters;
            }
            sb.Append(Convert.ToString(integer));
          } else {
            sb.Append(Convert.ToString(integer));
          }
        }
      } else {
        if(numberOfCharacters < 2) {
          sb.Append(Convert.ToString(integer));
        } else {
          if(Math.Pow(10, --numberOfCharacters) > integer) {
            while(numberOfCharacters > 0 && Math.Pow(10, numberOfCharacters) > integer) {
              sb.Append(Chars.n0);
              --numberOfCharacters;
            }
            sb.Append(Convert.ToString(integer));
          } else {
            sb.Append(Convert.ToString(integer));
          }
        }
      }
      
    }
    
    /// <summary>
    /// Appends a string of the specified integer filled with the specified number of '0's
    /// to the specified string builder.
    /// If the integer will not be contained within the specified number of integers, it
    /// is returned as a string unchanged.
    /// </summary>
    public static string ToString(this int integer, int numberOfCharacters) {
      // slightly different logic if the representation will include a '-' sign
      if(integer < 0) {
        if(--numberOfCharacters < 2) return Convert.ToString(integer);
        if(Math.Pow(10, --numberOfCharacters) > -integer) {
          var sb = StringBuilderCache.Get(numberOfCharacters);
          // invert the integer
          integer = -integer;
          // append the negative sign
          sb.Append(Chars.Dash);
          while(numberOfCharacters > 0 && Math.Pow(10, numberOfCharacters) > integer) {
            sb.Append(Chars.n0);
            --numberOfCharacters;
          }
          sb.Append(Convert.ToString(integer));
          return StringBuilderCache.SetAndGet(sb);
        }
      } else {
        if(numberOfCharacters < 2) return Convert.ToString(integer);
        if(Math.Pow(10, --numberOfCharacters) > integer) {
          var sb = StringBuilderCache.Get(numberOfCharacters);
          while(numberOfCharacters > 0 && Math.Pow(10, numberOfCharacters) > integer) {
            sb.Append(Chars.n0);
            --numberOfCharacters;
          }
          sb.Append(Convert.ToString(integer));
          return StringBuilderCache.SetAndGet(sb);
        }
      }
      return Convert.ToString(integer);
    }
    
    /// <summary>
    /// Get a byte count as a friendly string representation.
    /// </summary>
    public static string ToBytesString(this long bytes) {
      if(bytes > 999999) return ((double)bytes/Global.Megabyte).ToString("N2") + "MB";
      if(bytes > 9999) return ((bytes+512)/Global.Kilobyte) + "KB";
      return bytes + "B";
    }
    
    /// <summary>
    /// Get a byte count as a friendly string representation.
    /// </summary>
    public static string ToBytesString(this int bytes) {
      if(bytes > 999999) return ((double)bytes/Global.Megabyte).ToString("N2") + "MB";
      if(bytes > 9999) return ((bytes+512)/Global.Kilobyte) + "KB";
      return bytes + "B";
    }
    
    /// <summary>
    /// Get a string representing the specified milliseconds as a duration.
    /// </summary>
    public static string ToDuration(this long milliseconds) {
      string result = string.Empty;
      
      if(milliseconds > Time.Hour) {
        result = (milliseconds / Time.Hour) + ":";
        milliseconds = milliseconds % Time.Hour;
        
        result += (milliseconds > 540000 ? ((int)(milliseconds / Time.Minute)).ToString(2) : milliseconds.ToString()) + ":";
        milliseconds = milliseconds % Time.Minute;
      } else if(milliseconds < Time.Second) {
        return milliseconds + "ms";
      } else {
        result += (milliseconds / Time.Minute) + ":";
        milliseconds = milliseconds % Time.Minute;
      }
      
      result += ((int)(milliseconds / Time.Second)).ToString(2);
      
      return result;
    }
    
  }
  
}
