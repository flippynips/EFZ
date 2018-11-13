/*
 * User: Joshua
 * Date: 24/09/2016
 * Time: 6:27 PM
 */
using System;

using System.Collections.Generic;

namespace Efz.Cql {
  
  /// <summary>
  /// Base class of queries run on a Cassandra ISession.
  /// </summary>
  public struct CqlSelectOperation<TRow> : IEnumerable<TRow> where TRow : IRow, new() {
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// Builder of the query.
    /// </summary>
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a new cql where component to add to a cql statement.
    /// </summary>
    internal CqlSelectOperation(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Impose an additional conditional predicate on the query.
    /// </summary>
    public CqlSelectWhere<TRow> Where(Column column) {
      _builder.Add(Cql.Where);
      _builder.Add(column);
      return new CqlSelectWhere<TRow>(_builder);
    }
    
    /// <summary>
    /// Impose an additional conditional predicate on the query.
    /// </summary>
    public CqlSelectWhere<TRow> AndWhere(params Column[] columns) {
      _builder.Add(Cql.Where);
      // iterate columns
      foreach(Column column in columns) {
        _builder.Add(column);
      }
      return new CqlSelectWhere<TRow>(_builder);
    }
    
    /// <summary>
    /// Append another literal value to the where expression.
    /// </summary>
    public CqlSelectOperation<TRow> And(object value) {
      _builder.Add(value);
      return this;
    }
    
    /// <summary>
    /// Append other literal values to the where expression.
    /// </summary>
    public CqlSelectOperation<TRow> And(params object[] values) {
      _builder.Add(values);
      return this;
    }
    
    /// <summary>
    /// Limit the selected rows by the specified count.
    /// </summary>
    public CqlSelectOperation<TRow> Limit(int count){ 
      _builder.Limit = count;
      _builder.Add(Cql.Limit);
      return this;
    }
    
    /// <summary>
    /// Get the first row returned from the query. Returns 'Null' if no rows
    /// were found.
    /// </summary>
    public TRow First() {
      _builder.Limit = 1;
      _builder.Add(Cql.Limit);
      RowEnumerator<TRow> rows = _builder.ExecuteEnumerator<TRow>();
      return rows.MoveNext() ? rows.Current : default(TRow);
    }
    
    /// <summary>
    /// Get an enumerator of the results asynchronously.
    /// </summary>
    public void GetEnumerator(IAction<RowEnumerator<TRow>> onPage) {
      _builder.ExecuteEnumerator(onPage);
    }
    
    public IEnumerator<TRow> GetEnumerator() {
      return _builder.ExecuteEnumerator<TRow>();
    }
    
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }
    
    public static implicit operator RowEnumerator<TRow>(CqlSelectOperation<TRow> query) {
      return query._builder.ExecuteEnumerator<TRow>();
    }
    
    //----------------------------------//
    
    
    
  }
  
}
