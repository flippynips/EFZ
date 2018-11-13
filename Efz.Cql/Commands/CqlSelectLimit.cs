/*
 * User: Joshua
 * Date: 24/09/2016
 * Time: 6:27 PM
 */
using System;
using System.Collections.Generic;

namespace Efz.Cql {
  
  /// <summary>
  /// Struct to initialize a select command.
  /// </summary>
  public struct CqlSelectLimit<TRow> : IEnumerable<TRow> where TRow : IRow, new() {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a new cql select component.
    /// </summary>
    internal CqlSelectLimit(Query builder) {
      _builder = builder;
    }
    
    public IEnumerator<TRow> GetEnumerator() {
      return _builder.ExecuteEnumerator<TRow>();
    }
    
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }
    
    //----------------------------------//
    
    
    
  }
  
}
