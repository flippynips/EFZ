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
  public struct CqlSelect<TRow> : IEnumerable<TRow> where TRow : IRow, new() {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a new cql select component.
    /// </summary>
    internal CqlSelect(Query builder, params Column[] columns) {
      _builder = builder;
      _builder.Add(Cql.Select);
      foreach(Column column in columns) {
        _builder.Add(column);
      }
    }
    
    /// <summary>
    /// Initialize a new cql select component.
    /// </summary>
    internal CqlSelect(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Impose a conditional on the query.
    /// </summary>
    public CqlSelectWhere<TRow> Where(Column column) {
      _builder.Add(Cql.Where);
      _builder.Add(column);
      return new CqlSelectWhere<TRow>(_builder);
    }
    
    /// <summary>
    /// Impose a conditional on the query.
    /// </summary>
    public CqlSelectWhere<TRow> Where(params Column[] columns) {
      _builder.Add(Cql.Where);
      foreach(Column column in columns) {
        _builder.Add(column);
      }
      return new CqlSelectWhere<TRow>(_builder);
    }
    
    /// <summary>
    /// Limit the selected rows by the specified count.
    /// </summary>
    public CqlSelectLimit<TRow> Limit(int count){ 
      _builder.Limit = count;
      return new CqlSelectLimit<TRow>(_builder);
    }
    
    /// <summary>
    /// Select the first row returned from the select method or 'Null' if none.
    /// </summary>
    public TRow First() {
      _builder.Add(Cql.Limit);
      _builder.Add(1);
      var enumerator = _builder.ExecuteEnumerator<TRow>();
      if(enumerator.MoveNext()) return enumerator.Current;
      return default(TRow);
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
