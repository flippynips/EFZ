/*
 * User: Joshua
 * Date: 24/09/2016
 * Time: 6:27 PM
 */
using System;

namespace Efz.Cql {
  
  /// <summary>
  /// Base class of queries run on a Cassandra ISession.
  /// </summary>
  public struct CqlSelectWhere<TRow> where TRow : IRow, new() {
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// Query builder instance this where statement will append.
    /// </summary>
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a new cql component to conditionally filter rows.
    /// </summary>
    internal CqlSelectWhere(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Add a column to the group of columns in the current conditional relation.
    /// </summary>
    public CqlSelectWhere<TRow> And(Column column) {
      _builder.Add(column);
      return this;
    }
    
    /// <summary>
    /// Add a column to the group of columns in the current conditional relation.
    /// </summary>
    public CqlSelectWhere<TRow> And(params Column[] columns) {
      _builder.Add(columns);
      return this;
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlSelectOperation<TRow> GreaterThan(object value) {
      _builder.Add(Cql.GreaterThan);
      _builder.Add(value);
      return new CqlSelectOperation<TRow>(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlSelectOperation<TRow> GreaterThan(params object[] values) {
      _builder.Add(Cql.GreaterThan);
      _builder.Add(values);
      return new CqlSelectOperation<TRow>(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlSelectOperation<TRow> GreaterOrEqualThan(object value) {
      _builder.Add(Cql.GreaterOrEqual);
      _builder.Add(value);
      return new CqlSelectOperation<TRow>(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlSelectOperation<TRow> GreaterOrEqualThan(params object[] values) {
      _builder.Add(Cql.GreaterOrEqual);
      _builder.Add(values);
      return new CqlSelectOperation<TRow>(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlSelectOperation<TRow> LessThan(object value) {
      _builder.Add(Cql.LessThan);
      _builder.Add(value);
      return new CqlSelectOperation<TRow>(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlSelectOperation<TRow> LessThan(params object[] values) {
      _builder.Add(Cql.LessThan);
      _builder.Add(values);
      return new CqlSelectOperation<TRow>(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlSelectOperation<TRow> LessOrEqualThan(object value) {
      _builder.Add(Cql.LessOrEqual);
      _builder.Add(value);
      return new CqlSelectOperation<TRow>(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlSelectOperation<TRow> LessOrEqualThan(params object[] values) {
      _builder.Add(Cql.LessOrEqual);
      _builder.Add(values);
      return new CqlSelectOperation<TRow>(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlSelectOperation<TRow> EqualTo(object value) {
      _builder.Add(Cql.Equal);
      _builder.Add(value);
      return new CqlSelectOperation<TRow>(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlSelectOperation<TRow> EqualTo(params object[] values) {
      _builder.Add(Cql.Equal);
      _builder.Add(values);
      return new CqlSelectOperation<TRow>(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlSelectOperation<TRow> NotEqualTo(object value) {
      _builder.Add(Cql.NotEqual);
      _builder.Add(value);
      return new CqlSelectOperation<TRow>(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlSelectOperation<TRow> NotEqualTo(params object[] values) {
      _builder.Add(Cql.NotEqual);
      _builder.Add(values);
      return new CqlSelectOperation<TRow>(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlSelectOperation<TRow> Contains(object value) {
      _builder.Add(Cql.Contains);
      _builder.Add(value);
      return new CqlSelectOperation<TRow>(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlSelectOperation<TRow> Contains(params object[] values) {
      _builder.Add(Cql.Contains);
      _builder.Add(values);
      return new CqlSelectOperation<TRow>(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlSelectOperation<TRow> In(object value) {
      _builder.Add(Cql.In);
      _builder.Add(value);
      return new CqlSelectOperation<TRow>(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlSelectOperation<TRow> In(params object[] values) {
      _builder.Add(Cql.In);
      _builder.Add(values);
      return new CqlSelectOperation<TRow>(_builder);
    }
    
    //----------------------------------//
    
    
    
  }
  
}
