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
  public interface IDeserializable<TReader> {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Deserialize using the specified reader. The serialized
    /// byte size is provided.
    /// </summary>
    void Deserialize(TReader reader);
    
    //-------------------------------------------//
    
  }
  
}
