/*
 * User: Joshua
 * Date: 24/09/2016
 * Time: 6:27 PM
 */
using System;

namespace Efz.Cql {
  
  /// <summary>
  /// Struct to initialize a select command.
  /// </summary>
  public struct CqlInsertValues {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a new cql select component.
    /// </summary>
    internal CqlInsertValues(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Run the Insert on the condition that no matching primary
    /// column key is found. Returns if the Insert was completed.
    /// </summary>
    public bool RunReturnIfNoMatch() {
      _builder.Add(Cql.IfNotExists);
      return _builder.ExecuteBoolean();
    }
    
    /// <summary>
    /// Run the Insert on the condition that no matching primary
    /// column key is found.
    /// </summary>
    public void RunIfNoMatch() {
      _builder.Add(Cql.IfNotExists);
      _builder.ExecuteAsync();
    }
    
    /// <summary>
    /// Perform the Insert command synchronously.
    /// </summary>
    public void RunNow() {
      _builder.Execute();
    }
    
    /// <summary>
    /// Perform the Insert command syncronously.
    /// </summary>
    public void Run() {
      _builder.Execute();
    }
    
    /// <summary>
    /// Perform the Insert command asyncronously, making use of batching statements.
    /// </summary>
    public void RunAsync() {
      _builder.ExecuteAsync();
    }
    
    /// <summary>
    /// Perform the Insert command. Returns true if the Insert was completed.
    /// </summary>
    public static implicit operator bool(CqlInsertValues insert) {
      return insert._builder.ExecuteBoolean();
    }
    
    //----------------------------------//
    
    
    
  }
  
}
