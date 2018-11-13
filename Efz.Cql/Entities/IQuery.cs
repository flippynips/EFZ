/*
 * User: Joshua
 * Date: 3/10/2016
 * Time: 6:53 PM
 */
using System;

namespace Efz.Cql.Entities {
  
  /// <summary>
  /// Non-generic interface for queries.
  /// </summary>
  internal interface IQuery {
    
    //----------------------------------//
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// Add a cql keyword to the query.
    /// </summary>
    void Add(Cql cql);
    /// <summary>
    /// Add a column definition to the query.
    /// </summary>
    void Add(Column column);
    /// <summary>
    /// Add a set of columns to the query.
    /// </summary>
    void Add(params Column[] columns);
    /// <summary>
    /// Add a string literal to the query.
    /// </summary>
    void Add(object value);
    /// <summary>
    /// Add a string literals to the query.
    /// </summary>
    void Add(params object[] values);
    
    //----------------------------------//
    
  }
  
  
  
}
