/*
 * User: Joshua
 * Date: 11/10/2016
 * Time: 11:07 PM
 */
using System;

using Efz.Collections;

namespace Efz.Cql {
  
  /// <summary>
  /// Represents a collection of rows for a set of columns.
  /// </summary>
  public class RowCollection {
    
    //----------------------------------//
    
    /// <summary>
    /// The collection of columns associated with the rows.
    /// </summary>
    public ArrayRig<Column> Columns;
    /// <summary>
    /// The collection of rows.
    /// </summary>
    public ArrayQueue<IRow> Rows;
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a new empty row collection.
    /// </summary>
    internal RowCollection() {
      Rows = new ArrayQueue<IRow>();
      Columns = new ArrayRig<Column>();
    }
    
    /// <summary>
    /// Initialize a new row collection for the specified columns.
    /// </summary>
    public RowCollection(params Column[] columns) {
      Rows = new ArrayQueue<IRow>();
      Columns = new ArrayRig<Column>(columns);
    }
    
    //----------------------------------//
    
  }
  
}
