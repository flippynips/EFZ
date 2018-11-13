/*
 * User: Joshua
 * Date: 24/09/2016
 * Time: 6:27 PM
 */
using System;

namespace Efz.Cql {
  
  /// <summary>
  /// Struct to initialize a 'DELETE' command.
  /// </summary>
  public struct CqlDelete {
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// The query builder that collects the keywords, values and columns.
    /// </summary>
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct a new cql delete query.
    /// </summary>
    internal CqlDelete(Query builder) {
      _builder = builder;
      _builder.Add(Cql.Delete);
      _builder.Add(Cql.All);
    }
    
    /// <summary>
    /// Construct a new cql delete query.
    /// </summary>
    internal CqlDelete(Query builder, params Column[] columns) {
      _builder = builder;
      _builder.Add(Cql.Delete);
      bool identifier = false;
      foreach(Column column in columns) {
        if(column.IsIdentifier) {
          identifier = true;
          _builder.Add(Cql.All);
          break;
        }
      }
      if(!identifier) {
        foreach(Column column in columns) {
          _builder.Add(column);
        }
      }
    }
    
    /// <summary>
    /// Add column values to the insert command.
    /// </summary>
    public CqlDeleteWhere Where(Column column) {
      // append a keyword as a flag for the values
      _builder.Add(Cql.Where);
      _builder.Add(column);
      return new CqlDeleteWhere(_builder);
    }
    
    /// <summary>
    /// Add column values to the insert command.
    /// </summary>
    public CqlDeleteWhere Where(params Column[] columns) {
      // append a keyword as a flag for the values
      _builder.Add(Cql.Where);
      _builder.Add(columns);
      return new CqlDeleteWhere(_builder);
    }
    
    /// <summary>
    /// Add a conditional to the delete command.
    /// </summary>
    public CqlDeleteIf If(Column column) {
      _builder.Add(Cql.If);
      _builder.Add(column);
      return new CqlDeleteIf(_builder);
    }
    
    //----------------------------------//
    
  }
  
}
