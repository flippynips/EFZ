/*
 * User: Joshua
 * Date: 9/10/2016
 * Time: 7:23 PM
 */
using System;

namespace Efz.Cql {
  
  /// <summary>
  /// First component of an 'Update' command.
  /// </summary>
  public struct CqlUpdateWhere {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize the start of an 'Update' command.
    /// </summary>
    internal CqlUpdateWhere(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Add a column to the group of columns in the current conditional relation.
    /// </summary>
    public CqlUpdateWhere And(Column column) {
      _builder.Add(column);
      return this;
    }
    
    /// <summary>
    /// Add a column to the group of columns in the current conditional relation.
    /// </summary>
    public CqlUpdateWhere And(params Column[] columns) {
      _builder.Add(columns);
      return this;
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlUpdateWith GreaterThan(object value) {
      _builder.Add(Cql.GreaterThan);
      _builder.Add(value);
      return new CqlUpdateWith(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlUpdateWith GreaterThan(params object[] values) {
      _builder.Add(Cql.GreaterThan);
      _builder.Add(values);
      return new CqlUpdateWith(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlUpdateWith GreaterOrEqualThan(object value) {
      _builder.Add(Cql.GreaterOrEqual);
      _builder.Add(value);
      return new CqlUpdateWith(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlUpdateWith GreaterOrEqualThan(params object[] values) {
      _builder.Add(Cql.GreaterOrEqual);
      _builder.Add(values);
      return new CqlUpdateWith(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlUpdateWith LessThan(object value) {
      _builder.Add(Cql.LessThan);
      _builder.Add(value);
      return new CqlUpdateWith(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlUpdateWith LessThan(params object[] values) {
      _builder.Add(Cql.LessThan);
      _builder.Add(values);
      return new CqlUpdateWith(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlUpdateWith LessOrEqualThan(object value) {
      _builder.Add(Cql.LessOrEqual);
      _builder.Add(value);
      return new CqlUpdateWith(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlUpdateWith LessOrEqualThan(params object[] values) {
      _builder.Add(Cql.LessOrEqual);
      _builder.Add(values);
      return new CqlUpdateWith(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlUpdateWith EqualTo(object value) {
      _builder.Add(Cql.Equal);
      _builder.Add(value);
      return new CqlUpdateWith(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlUpdateWith EqualTo(params object[] values) {
      _builder.Add(Cql.Equal);
      _builder.Add(values);
      return new CqlUpdateWith(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlUpdateWith NotEqualTo(object value) {
      _builder.Add(Cql.NotEqual);
      _builder.Add(value);
      return new CqlUpdateWith(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlUpdateWith NotEqualTo(params object[] values) {
      _builder.Add(Cql.NotEqual);
      _builder.Add(values);
      return new CqlUpdateWith(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlUpdateWith Contains(object value) {
      _builder.Add(Cql.Contains);
      _builder.Add(value);
      return new CqlUpdateWith(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlUpdateWith Contains(params object[] values) {
      _builder.Add(Cql.Contains);
      _builder.Add(values);
      return new CqlUpdateWith(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlUpdateWith In(object value) {
      _builder.Add(Cql.In);
      _builder.Add(value);
      return new CqlUpdateWith(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlUpdateWith In(params object[] values) {
      _builder.Add(Cql.In);
      _builder.Add(values);
      return new CqlUpdateWith(_builder);
    }
    
    //----------------------------------//
    
    
  }
  
  
}
