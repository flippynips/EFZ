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
  public struct CqlDeleteWhere {
    
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
    internal CqlDeleteWhere(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Add a column to the group of columns in the current conditional relation.
    /// </summary>
    public CqlDeleteWhere And(Column column) {
      _builder.Add(column);
      return this;
    }
    
    /// <summary>
    /// Add a column to the group of columns in the current conditional relation.
    /// </summary>
    public CqlDeleteWhere And(params Column[] columns) {
      _builder.Add(columns);
      return this;
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlDeleteEndWhere EqualTo(object value) {
      _builder.Add(Cql.Equal);
      _builder.Add(value);
      return new CqlDeleteEndWhere(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlDeleteEndWhere EqualTo(params object[] values) {
      _builder.Add(Cql.Equal);
      _builder.Add(values);
      return new CqlDeleteEndWhere(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlDeleteEndWhere In(object value) {
      _builder.Add(Cql.In);
      _builder.Add(value);
      return new CqlDeleteEndWhere(_builder);
    }
    
    /// <summary>
    /// Impose a conditional predicate on the query.
    /// </summary>
    public CqlDeleteEndWhere In(params object[] values) {
      _builder.Add(Cql.In);
      _builder.Add(values);
      return new CqlDeleteEndWhere(_builder);
    }
    
    //----------------------------------//
    
    
    
  }
  
}
