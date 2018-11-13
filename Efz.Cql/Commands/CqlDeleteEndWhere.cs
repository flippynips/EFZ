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
  public struct CqlDeleteEndWhere {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a new cql where component to add to a cql statement.
    /// </summary>
    internal CqlDeleteEndWhere(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Impose an additional conditional predicate on the query.
    /// </summary>
    public CqlDeleteWhere Where(Column column) {
      _builder.Add(Cql.Where);
      _builder.Add(column);
      return new CqlDeleteWhere(_builder);
    }
    
    /// <summary>
    /// Impose an additional conditional predicate on the query.
    /// </summary>
    public CqlDeleteWhere Where(params Column[] columns) {
      _builder.Add(Cql.Where);
      // iterate columns
      foreach(Column column in columns) {
        _builder.Add(column);
      }
      return new CqlDeleteWhere(_builder);
    }
    
    /// <summary>
    /// Append another literal value to the where expression.
    /// </summary>
    public CqlDeleteEndWhere And(object value) {
      _builder.Add(value);
      return this;
    }
    
    /// <summary>
    /// Append other literal values to the where expression.
    /// </summary>
    public CqlDeleteEndWhere And(params object[] values) {
      _builder.Add(values);
      return this;
    }
    
    /// <summary>
    /// Limit the selected rows by the specified count.
    /// </summary>
    public CqlDeleteIf If(Column column){
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
    /// Run the delete query synchronously.
    /// </summary>
    public void Run() {
      _builder.Execute();
    }
    
    /// <summary>
    /// Run the delete query asynchronously.
    /// </summary>
    public void RunAsync() {
      _builder.ExecuteAsync();
    }
    
    /// <summary>
    /// Run the update query.
    /// </summary>
    public void RunAsync(IAction onExecuted) {
      _builder.ExecuteAsync(onExecuted);
    }
    
    //----------------------------------//
    
    
    
  }
  
}
