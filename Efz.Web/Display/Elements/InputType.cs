/*
 * User: FloppyNipples
 * Date: 26/03/2017
 * Time: 16:42
 */
using System;

namespace Efz.Web.Display {
  
  /// <summary>
  /// Types of input elements.
  /// </summary>
  public enum InputType {
    None = 0,
    Button = 1,
    Checkbox = 2,
    Color = 3,
    Date = 4,
    DatetimeLocal = 5,
    Email = 6,
    File = 7,
    Hidden = 8,
    Image = 9,
    Month = 10,
    Number = 11,
    Password = 12,
    Radio = 13,
    Range = 14,
    Reset = 15,
    Search = 16,
    Submit = 17,
    Tel = 18,
    Text = 19,
    Time = 20,
    Url = 21,
    Week = 22,
  }
  
  /// <summary>
  /// Extension methdods for the input type enum.
  /// </summary>
  public static class ExtendInputType {
    
    /// <summary>
    /// Get a string representation of the input type.
    /// </summary>
    public static string GetString(this InputType type) {
      switch(type) {
        case InputType.DatetimeLocal:
          return "datetime-local";
        default:
          return type.ToString().ToLowercase();
      }
    }
    
    /// <summary>
    /// Get the input type representation of the string type.
    /// </summary>
    public static InputType GetInputType(string type) {
      if(type == null) return InputType.None;
      if(type.Equals("datetime-local", StringComparison.OrdinalIgnoreCase)) {
        return InputType.DatetimeLocal;
      }
      InputType result;
      if(Enum.TryParse<InputType>(type, true, out result)) {
        return result;
      }
      return InputType.None;
    }
    
  }
  
}
