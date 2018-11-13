/*
 * User: Joshua
 * Date: 23/10/2016
 * Time: 1:40 PM
 */
using System;
using System.Collections.Generic;
using Efz.Collections;

namespace Efz {
  
  /// <summary>
  /// Extension methods for common collection types.
  /// </summary>
  public static class ExtendCollections {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Derive a collection of one type from another, optionally with a predicate.
    /// </summary>
    public static ArrayRig<B> Derive<A,B>(this IEnumerable<A> collection, Func<A,B> mapper, Func<A,bool> predicate = null) {
      ArrayRig<B> rig = new ArrayRig<B>();
      
      // was the predicate supplied?
      if(predicate == null) {
        // no, perform the mapping without
        foreach(A item in collection) {
          rig.Add(mapper(item));
        }
      } else {
        // yes, perform the mapping with the predicate
        foreach(A item in collection) {
          if(predicate(item)) rig.Add(mapper(item));
        }
      }
      
      return rig;
    }
    
    /// <summary>
    /// Get a random item from a collection.
    /// Returns 'default(T)' if the array contains no items.
    /// </summary>
    public static T Random<T>(this T[] array) {
      if(array.Length == 0) return default(T);
      return array[Randomize.Range(0, array.Length)];
    }
    
    /// <summary>
    /// Get the dictionary content as a single string.
    /// </summary>
    public static string Join(this Dictionary<string, string> dictionary, char delimiter = ',', char separator = '=') {
      var builder = StringBuilderCache.Get();
      bool first = true;
      foreach(var entry in dictionary) {
        if(first) first = false;
        else builder.Append(delimiter);
        builder.Append(entry.Key.Escape(delimiter, separator));
        builder.Append(separator);
        builder.Append(entry.Value.Escape(delimiter, separator));
      }
      return builder.ToString();
    }
    
    /// <summary>
    /// Get a string representation of the collection.
    /// </summary>
    public static string ToString<T>(this IEnumerable<T> collection, char separator = Chars.Comma) {
      
      // get a builder of the concatination
      var builder = StringBuilderCache.Get();
      if(separator == Chars.Null) {
        foreach(var item in collection) builder.Append(item);
      } else {
        bool first = true;
        foreach(var item in collection) {
          if(first) first = false;
          else builder.Append(separator);
          builder.Append(item);
        }
      }
      
      // return the complete build
      return builder.ToString();
    }
    
    /// <summary>
    /// Get a string representation a section of the collection.
    /// </summary>
    public static string ToString<T>(this T[] collection, int index, int count, char separator = Chars.Null) {
      
      // get the end index
      count += index;
      
      // get a builder of the concatination
      var builder = StringBuilderCache.Get();
      if(separator == Chars.Null) {
        for(int i = index; i < count; ++i) {
          builder.Append(collection[i]);
        }
      } else {
        bool first = true;
        for(int i = index; i < count; ++i) {
          if(first) first = false;
          else builder.Append(separator);
          builder.Append(collection[i]);
        }
      }
      
      // return the complete build
      return builder.ToString();
    }
    
    /// <summary>
    /// Get a string representation a section of the collection.
    /// </summary>
    public static bool SequenceEqual<T>(this T[] collectionA, T[] collectionB, int startA, int startB, int length) {
      while(--length >= 0) if(!collectionA[startA + length].Equals(collectionB[startB + length])) return false;
      return true;
    }
    
    /// <summary>
    /// Get a string representation a section of the collection.
    /// </summary>
    public static bool SequenceEqual<T>(this T[] collectionA, T[] collectionB, int start, int length) {
      length = start + length;
      while(--length >= start) if(!collectionA[length].Equals(collectionB[length])) return false;
      return true;
    }
    
    //-------------------------------------------//
    
  }
  
}
