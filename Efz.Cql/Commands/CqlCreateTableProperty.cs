/*
 * User: Joshua
 * Date: 12/10/2016
 * Time: 12:37 AM
 */
using System;
using Cassandra;

namespace Efz.Cql {
  
  /// <summary>
  /// The first component in a 'CREATE TABLE' cql command.
  /// </summary>
  public struct CqlCreateTableProperty {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize the start of an 'Update' command.
    /// </summary>
    internal CqlCreateTableProperty(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Add a partition key to the table.
    /// </summary>
    public CqlCreateTableProperty WithProperty(string property) {
      _builder.Add(property);
      return this;
    }
    
    /// <summary>
    /// Run the create table query. Returns the associated metadata for the table.
    /// </summary>
    public TableMetadata Run() {
      _builder.Execute();
      return _builder.Keyspace.Metadata.GetTableMetadata(_builder.Values[0].ToString());
    }
    
    /// <summary>
    /// Implicitly return the created Table metadata.
    /// </summary>
    public static implicit operator TableMetadata(CqlCreateTableProperty command) {
      command._builder.Execute();
      return command._builder.Keyspace.Metadata.GetTableMetadata(command._builder.Values[0].ToString());
    }
    
    //----------------------------------//
    
  }
  
}
