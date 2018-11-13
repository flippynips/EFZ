/*
 * User: Joshua
 * Date: 29/09/2016
 * Time: 10:09 PM
 */
using System;

namespace Efz.Cql {
  
  /// <summary>
  /// Defines a column in a table. These attributes cannot be changed once a
  /// table has been created.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  public sealed class ColumnAttribute : Attribute {
    
    //----------------------------------//
    
    /// <summary>
    /// The name of the column in its data table. If not set, the name will be taken
    /// from the field name.
    /// </summary>
    public string Name;
    /// <summary>
    /// Is this column a component of the partition key.
    /// </summary>
    public Column.ColumnClass Class;
    
    /// <summary>
    /// Can the column be used as an identifier?
    /// This is true for partition and cluster keys.
    /// </summary>
    public bool IsIdentifier;
    
    /// <summary>
    /// Optionally specify the Cassandra data type this column represents.
    /// If not specified the type will be derived.
    /// </summary>
    public string DataType;
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize the details of a column.
    /// </summary>
    public ColumnAttribute(Column.ColumnClass columnClass, string name = null) {
      Class = columnClass;
      Name = name;
    }
    
    //----------------------------------//
    
  }
  
}
