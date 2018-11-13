/*
 * User: FloppyNipples
 * Date: 24/05/2017
 * Time: 17:38
 */
using System;

namespace Efz.Cql {
  
  /// <summary>
  /// Represents metadata relating to blob.
  /// </summary>
  public class Meta {
    
    /// <summary>
    /// Length of the metadata.
    /// </summary>
    public long Length;
    /// <summary>
    /// Number of sections in the blob.
    /// </summary>
    public int SectionCount;
    /// <summary>
    /// Maximum length of each section.
    /// </summary>
    public int SectionLength;
    
    /// <summary>
    /// Initialize a new metadata instance.
    /// </summary>
    public Meta() {
    }
    
    /// <summary>
    /// Initialize a new metadata instance.
    /// </summary>
    public Meta(long length, int sectionCount, int sectionLength) {
      Length = length;
      SectionCount = sectionCount;
      SectionLength = sectionLength;
    }
    
  }
  
}
