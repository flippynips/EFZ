/*
 * User: Joshua
 * Date: 28/08/2016
 * Time: 9:34 PM
 */
using System;

namespace Efz {
  
  /// <summary>
  /// Description of IDeserializable.
  /// </summary>
  public interface ISerializable<TWriter> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Serialize using the specified writer.
    /// </summary>
    void Serialize(TWriter reader);
    
    //-------------------------------------------//
    
  }
  
}
