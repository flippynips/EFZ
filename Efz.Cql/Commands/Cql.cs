/*
 * User: Joshua
 * Date: 1/10/2016
 * Time: 2:18 AM
 */
using System;

namespace Efz.Cql {
  
  /// <summary>
  /// Types of query commands that may be associated with values.
  /// </summary>
  internal enum Cql : byte {
    
    Unknown     = 0,
    /// <summary>
    /// SELECT query to retrieve data from a Cassandra table.
    /// </summary>
    Select      = 1,
    /// <summary>
    /// UPDATE query to update columns in a row.
    /// </summary>
    Update      = 2,
    /// <summary>
    /// CREATE query to create a new table, keyspace, ect.
    /// </summary>
    Create      = 3,
    /// <summary>
    /// INSERT query to add or update columns.
    /// </summary>
    Insert      = 4,
    Delete      = 5,
    Drop        = 6,
    
    Table       = 7,
    
    Where       = 10,
    OrderBy     = 11,
    And         = 12,
    Operation   = 13,
    Item        = 14,
    Alias       = 15,
    Decending   = 16,
    Limit       = 17,
    IfNotExists = 18,
    Alter       = 19,
    Batch       = 20,
    Grant       = 21,
    Revoke      = 22,
    Truncate    = 23,
    Use         = 24,
    Set         = 25,
    If          = 26,
    IfExists    = 27,
    
    Count       = 30,
    All         = 32,
    Distinct    = 33,
    Index       = 34,
    Token       = 35,
    List        = 36,
    
    GreaterThan    = 50,
    LessThan       = 51,
    GreaterOrEqual = 52,
    LessOrEqual    = 53,
    Equal          = 54,
    NotEqual       = 55,
    In             = 56,
    Contains       = 57,
    
    
  }
  
}
