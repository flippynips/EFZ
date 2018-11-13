/*
 * User: FloppyNipples
 * Date: 24/05/2017
 * Time: 17:38
 */
using System;

namespace Efz.Cql {
  
  /// <summary>
  /// Represents a section of data.
  /// </summary>
  public class Data {
    
    /// <summary>
    /// Index of the blob section.
    /// </summary>
    public int SectionIndex;
    /// <summary>
    /// Bytes of the section.
    /// </summary>
    public byte[] Bytes;
    
    /// <summary>
    /// Initialize a new data section instance.
    /// </summary>
    public Data() {
    }
    
    /// <summary>
    /// Initialize a new data section instance.
    /// </summary>
    public Data(int sectionIndex, byte[] bytes) {
      SectionIndex = sectionIndex;
      Bytes = bytes;
    }
    
  }
  
}
