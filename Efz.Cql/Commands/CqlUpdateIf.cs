/*
 * User: Joshua
 * Date: 9/10/2016
 * Time: 7:23 PM
 */
using System;

namespace Efz.Cql {
  
  /// <summary>
  /// IF constraint component of an 'Update' command.
  /// </summary>
  public struct CqlUpdateIf {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize the start of an 'Update' command.
    /// </summary>
    internal CqlUpdateIf(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Add the right hand side to the conditional with the literal value.
    /// </summary>
    public CqlUpdateEndIf EqualTo(object value) {
      _builder.Add(value);
      return new CqlUpdateEndIf(_builder);
    }
    
    //----------------------------------//
    
    
  }
  
  
}
