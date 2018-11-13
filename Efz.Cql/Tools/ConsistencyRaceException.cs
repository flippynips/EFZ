/*
 * User: FloppyNipples
 * Date: 9/11/2017
 * Time: 9:18 PM
 */
using System;

namespace Efz.Cql {
  
  /// <summary>
  /// Exception that can be thrown if aggregated data from multiple Cassandra
  /// queries end up misaligned or invalid.
  /// </summary>
  public class ConsistencyRaceException : Exception {
    
    //-----------------------------------//
    
    //-----------------------------------//
    
    //-----------------------------------//
    
    /// <summary>
    /// Create a new consistency exception.
    /// </summary>
    public ConsistencyRaceException(string message) : base(message) {
    }
    
    //-----------------------------------//
      
  }
  
}
