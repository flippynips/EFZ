/*
 * User: Joshua
 * Date: 9/10/2016
 * Time: 7:23 PM
 */
using System;

namespace Efz.Cql {
  
  /// <summary>
  /// First component of an 'Update' command.
  /// </summary>
  public struct CqlDeleteIf {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize the 'IF' component of the delete query.
    /// </summary>
    internal CqlDeleteIf(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Add the right hand side to the conditional with the literal value.
    /// </summary>
    public CqlDeleteEndIf EqualTo(object value) {
      _builder.Add(value);
      return new CqlDeleteEndIf(_builder);
    }
    
    //----------------------------------//
    
    
  }
  
  
}
