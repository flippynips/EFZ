/*
 * User: Joshua
 * Date: 20/10/2016
 * Time: 11:08 PM
 */
using System;

using Cassandra;

namespace Efz.Cql {
  
  /// <summary>
  /// Non-generic interface for all row definitions.
  /// </summary>
  public interface IRow {
    
    //----------------------------------//
    
    /// <summary>
    /// True when the row has been flagged to potentially be updated with a 'SubmitChanges'
    /// on the owning 'RowEnumerator' instance.
    /// </summary>
    bool Updated { get; set; }
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// Map the values of the cassandra row to this row instance.
    /// </summary>
    void Create(Row row);
    /// <summary>
    /// Populate the values of the row cells.
    /// </summary>
    void GetValues(object[] values);
    /// <summary>
    /// Populate the flags indicating which column indexes have been updated.
    /// </summary>
    void GetUpdated(bool[] updated);
    
    //----------------------------------//
    
  }
  
}
