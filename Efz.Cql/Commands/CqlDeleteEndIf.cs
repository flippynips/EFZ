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
  public struct CqlDeleteEndIf {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize the start of an 'Update' command.
    /// </summary>
    internal CqlDeleteEndIf(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Add a condition to the execution of the 'Update' query.
    /// If the confidition isn't met, the row is not updated or created.
    /// </summary>
    public CqlDeleteIf AndIf(Column column) {
      _builder.Add(Cql.If);
      _builder.Add(column);
      return new CqlDeleteIf(_builder);
    }
    
    /// <summary>
    /// Execute the query if the row is found to exist in the table.
    /// </summary>
    public void RunIfExists() {
      _builder.Add(Cql.IfExists);
      _builder.Execute();
    }
    
    /// <summary>
    /// Execute the query with the callback.
    /// </summary>
    public void RunIfExists(IAction onExecuted) {
      _builder.Add(Cql.IfExists);
      _builder.ExecuteAsync(onExecuted);
    }
    
    /// <summary>
    /// Run the update query.
    /// </summary>
    public void Run() {
      _builder.Execute();
    }
    
    /// <summary>
    /// Run the update query.
    /// </summary>
    public void Run(IAction onExecuted) {
      _builder.ExecuteAsync(onExecuted);
    }
    
    //----------------------------------//
    
  }
  
  
}
