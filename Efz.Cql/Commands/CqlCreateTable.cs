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
  public struct CqlCreateTable {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize the start of an 'Update' command.
    /// </summary>
    internal CqlCreateTable(Query builder) {
      _builder = builder;
      
      _builder.Add(Cql.Create);
      _builder.Add(Cql.Table);
    }
    
    /// <summary>
    /// Add the specified column to the table construction.
    /// </summary>
    public CqlCreateTable With(Column column) {
      _builder.Add(column);
      return this;
    }
    
    /// <summary>
    /// Add the specified columns to the table construction.
    /// </summary>
    public CqlCreateTable With(params Column[] columns) {
      _builder.Add(columns);
      return this;
    }
    
    /// <summary>
    /// With the specified table property. Find property definitions here :
    /// http://docs.datastax.com/en/cql/3.1/cql/cql_reference/tabProp.html
    /// </summary>
    public CqlCreateTableProperty WithProperty(string property) {
      _builder.Add(Cql.Where);
      _builder.Add(property);
      return new CqlCreateTableProperty(_builder);
    }
    
    /// <summary>
    /// Run the create table query. Returns the associated metadata for the table.
    /// </summary>
    public TableMetadata Run() {
      _builder.Execute();
      return _builder.Keyspace.Metadata.GetTableMetadata(_builder.Table.Name);
    }
    
    /// <summary>
    /// Implicitly return the created Table metadata.
    /// </summary>
    public static implicit operator TableMetadata(CqlCreateTable command) {
      command._builder.Execute();
      return command._builder.Keyspace.Metadata.GetTableMetadata(command._builder.Table.Name);
    }
    
    //----------------------------------//
    
  }
  
}
