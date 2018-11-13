/*
 * User: Joshua
 * Date: 24/09/2016
 * Time: 6:27 PM
 */
using System;

namespace Efz.Cql {
  
  /// <summary>
  /// Represents a selector of column names or records.
  /// </summary>
  internal struct CqlCreateIndex<TRow> where TRow : IRow, new() {
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// Builder of the index query.
    /// </summary>
    private Query _builder;
    
    //----------------------------------//
    
    public CqlCreateIndex(Query builder, string indexName) {
      _builder = builder;
      _builder.Add(Cql.Create);
      _builder.Add(Cql.Index);
      _builder.Add(indexName);
    }
    
    /// <summary>
    /// Include the specified column in the index.
    /// </summary>
    public void OfColumn(Column column) {
      _builder.Add(column);
    }
    
    /// <summary>
    /// Include the specified columns in the index.
    /// </summary>
    public void OfColumns(params Column[] columns) {
      foreach(Column column in columns) {
        _builder.Add(column);
      }
    }
    
    //----------------------------------//
    
    
    
  }
  
}
