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
  public struct CqlUpdateEndIf {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize the start of an 'Update' command.
    /// </summary>
    internal CqlUpdateEndIf(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Add a condition to the execution of the 'Update' query.
    /// If the confidition isn't met, the row is not updated or created.
    /// </summary>
    public CqlUpdateIf And(Column column) {
      _builder.Add(Cql.If);
      _builder.Add(column);
      return new CqlUpdateIf(_builder);
    }
    
    /// <summary>
    /// Run the update query.
    /// </summary>
    public void Run() {
      _builder.Execute();
    }
    
    /// <summary>
    /// Execute the query if the row is found to exist in the table.
    /// </summary>
    public void IfExists() {
      _builder.Add(Cql.IfExists);
    }
    
    //----------------------------------//
    
    
  }
  
  
}
