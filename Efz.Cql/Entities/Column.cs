/*
 * User: Joshua
 * Date: 29/09/2016
 * Time: 10:09 PM
 */
using System;

using Efz.Cql;

namespace Efz.Cql {
  
  /// <summary>
  /// Represents a Column in a Table.
  /// </summary>
  public sealed class Column<T> : Column {
    
    //----------------------------------//
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// Internal constructor called when its owning table initializes.
    /// </summary>
    internal Column(ColumnAttribute attribute) : base(attribute) {
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Used internally as a non-generic column interaction during cql
  /// query construction. DO NOT INHERIT FROM THIS CLASS.
  /// </summary>
  public abstract class Column {
    
    //----------------------------------//
    
    /// <summary>
    /// The class of column.
    /// </summary>
    public enum ColumnClass : byte {
      Primary,
      Cluster,
      Data
    }
    
    /// <summary>
    /// The index of this Column. This is assigned using metadata
    /// when the columns table is created.
    /// </summary>
    internal int Index;
    /// <summary>
    /// String representation of the Cassandra data type this column
    /// represents.
    /// </summary>
    internal string DataType;
    
    /// <summary>
    /// The name of the Column.
    /// </summary>
    public readonly string Name;
    /// <summary>
    /// Is this column a component of the partition key.
    /// </summary>
    public readonly ColumnClass Class;
    
    /// <summary>
    /// Flags the column for as a unique identifier for update commands?
    /// This is true for partition and cluster keys.
    /// </summary>
    public readonly bool IsIdentifier;
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// Setup the properties of the column.
    /// </summary>
    internal Column(ColumnAttribute attribute) {
      Name = attribute.Name.ToLowercase();
      Class = attribute.Class;
      IsIdentifier = attribute.IsIdentifier || Class == ColumnClass.Primary || Class == ColumnClass.Cluster;
      DataType = attribute.DataType;
      
      // if the type wasn't explicitly defined
      if(DataType == null) {
        // derive the type of Cassandra column this represents
        DataType = Common.GetDataTypeString(GetType().GetGenericArguments()[0]);
      }
    }
    
    //----------------------------------//
    
  }
  
}
