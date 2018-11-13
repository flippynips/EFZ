/*
 * User: FloppyNipples
 * Date: 26/03/2017
 * Time: 17:59
 */
using System;
using System.Text.RegularExpressions;

namespace Efz {
  
  /// <summary>
  /// Common validation rules.
  /// </summary>
  public static class Validate {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Regex expression for validating a password.
    /// </summary>
    public static Regex RegexEmail = new Regex("[A-Za-z0-9._%+-]+@[A-Za-z0-9-]+(\\.[A-Za-z]{2,}){1,3}",
      RegexOptions.Compiled, TimeSpan.FromMilliseconds(2));
    
    /// <summary>
    /// Html tag finder.
    /// </summary>
    public static Regex Html = new Regex(@"<(?<endTag>/)?(?<tagname>\w+)((\s+(?<attName>\w+)(\s*=\s*(?:""(?<attVal>" +
      @"[^""]*)""|'(?<attVal>[^']*)'|(?<attVal>[^'"">\s]+)))?)+\s*|\s*)(?<completeTag>/)?>",
      RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(200));
    
    /// <summary>
    /// Alpha numeric regex check.
    /// </summary>
    public static Regex RegexAlphaNumeric = new Regex("[0-9a-zA-Z]", RegexOptions.Compiled, TimeSpan.FromMilliseconds(2));
    
    /// <summary>
    /// Validate a string as an email address.
    /// </summary>
    public static bool Email(string str) {
      return RegexEmail.IsMatch(str);
    }
    
    /// <summary>
    /// Validate a phone number.
    /// </summary>
    public static bool PhoneNumber(string str) {
      // TODO : Better validation.
      var clean = str.Remove(Chars.Plus, Chars.Space, Chars.Colon, Chars.Stop, Chars.Comma);
      if(clean.Length == 10 || clean.Length == 8) {
        int number;
        if(int.TryParse(clean, out number)) return true;
      }
      return false;
    }
    
    /// <summary>
    /// Validate whether a string is a safe cql parameter. Ensures no injection could occur.
    /// </summary>
    public static bool SafeCql(string value) {
      // TODO : Better validation.
      return RegexAlphaNumeric.IsMatch(value);
    }
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
  }
}
