using System;

namespace Efz {
  
  /// <summary>
  /// Helper class for easy access to various forms of random numbers.
  /// </summary>
  static public class Randomize {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Get a random double number.
    /// </summary>
    static public double Double {
      get {
        return _random.NextDouble();
      }
    }
    
    /// <summary>
    /// Get a random integer.
    /// </summary>
    static public int Integer {
      get {
        return (int)_random.NextDouble();
      }
    }
    
    /// <summary>
    /// Get a random character.
    /// </summary>
    static public char Char {
      get {
        return (char)(32 + _random.NextDouble() * 94);
      }
    }
    
    /// <summary>
    /// Get a random byte.
    /// </summary>
    static public byte Byte {
      get {
        // 2147483648
        // 8388608
        return (byte)(_random.Next() / _byteDivider);
      }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Random number generator.
    /// </summary>
    private static Random _random = new Random();
    
    /// <summary>
    /// Number the next random integer is divided by to make it a byte.
    /// </summary>
    private const int _byteDivider = 8388608;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Get a random integer between a minimum and maximum inclusive.
    /// </summary>
    public static int Range(int numberA, int numberB) {
      return (int)(numberA + _random.NextDouble() * (numberB - numberA));
    }
    
    /// <summary>
    /// Get a random double between a minimum and maximum inclusive.
    /// </summary>
    public static double Range(double numberA, double numberB) {
      return numberA + _random.NextDouble() * (numberB - numberA);
    }
    
    /// <summary>
    /// Get a random ulong between a minimum and maximum inclusive.
    /// </summary>
    public static ulong Range(ulong numberA, ulong numberB) {
      return (ulong)(numberA + _random.NextDouble() * (numberB - numberA));
    }
    
    /// <summary>
    /// Return a random string of the specified length.
    /// </summary>
    public static string String(int length) {
      char[] str = new char[length];
      for(int i = str.Length-1; i >= 0; --i) {
        str[i] = Char;
      }
      return new string(str);
    }
    
    //-------------------------------------------//
    
  }

}