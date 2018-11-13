/*
 * User: Joshua
 * Date: 20/10/2016
 * Time: 11:48 PM
 */
using System;

namespace Efz.Cql
{
  /// <summary>
  /// Description of CqlUpdateWith.
  /// </summary>
  public struct CqlUpdateWith {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize the start of an 'Update' command.
    /// </summary>
    internal CqlUpdateWith(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Filter the affected rows of the query using a 'Where' expression.
    /// </summary>
    public CqlUpdateWhere Where(Column column) {
      _builder.Add(Cql.Where);
      _builder.Add(column);
      return new CqlUpdateWhere(_builder);
    }
    
    /// <summary>
    /// Filter the affected rows of the query using a 'Where' expression.
    /// </summary>
    public CqlUpdateWhere Where(params Column[] columns) {
      _builder.Add(Cql.Where);
      _builder.Add(columns);
      return new CqlUpdateWhere(_builder);
    }
    
    /// <summary>
    /// Add a condition to the execution of the 'Update' query.
    /// If the confidition isn't met, the row is not updated or created.
    /// </summary>
    public CqlUpdateIf If(Column column) {
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
    /// Run the update query asynchronously, making use of
    /// batch statements.
    /// </summary>
    public void RunAsync() {
      _builder.ExecuteAsync();
    }
    
    /// <summary>
    /// Run asynchronously without using the batch capabilities of the keyspace.
    /// </summary>
    public void RunAsyncNoBatch() {
      _builder.ExecuteAsyncNoBatch();
    }
    
    /// <summary>
    /// Execute the query if the row is found to exist in the table.
    /// </summary>
    public CqlUpdateWith IfExists() {
      _builder.Add(Cql.IfExists);
      return this;
    }
    
  }
}
