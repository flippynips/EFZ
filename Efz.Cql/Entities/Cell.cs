/*
 * User: Joshua
 * Date: 1/10/2016
 * Time: 2:19 PM
 */
using System;

namespace Efz.Cql {
  
  /// <summary>
  /// Represents the value of a specific row column.
  /// </summary>
  public class Cell<T> {
    
    //----------------------------------//
    
    /// <summary>
    /// Get or set the value this column represents.
    /// </summary>
    public T Value {
      get {
        return InnerValue;
      }
      set {
        // early out if the value isn't changed
        InnerValue = value;
        Updated = true;
      }
    }
    
    /// <summary>
    /// The column value has been updated.
    /// </summary>
    public bool Updated { get; internal set; }
    /// <summary>
    /// Inner value of the column.
    /// </summary>
    internal T InnerValue;
    
    //----------------------------------//
    
    //----------------------------------//
    
    public Cell(T value) {
      InnerValue = value;
    }
    
    //----------------------------------//
    
  }
  
}
