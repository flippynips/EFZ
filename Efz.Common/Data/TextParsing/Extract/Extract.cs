/*
 * User: Joshua
 * Date: 6/08/2016
 * Time: 9:48 PM
 */
using System;
using System.Text;

namespace Efz.Text {
  
  /// <summary>
  /// Requirements for and methods of parsing a section of text.
  /// </summary>
  public abstract class Extract : IDisposable {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The encoding used to extract the text. This is used in cases
    /// where the byte count for characters is needed.
    /// Default us utf8.
    /// </summary>
    public Encoding Encoding = Encoding.UTF8;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize this text entity.
    /// </summary>
    protected Extract() { }
    
    /// <summary>
    /// Dispose of all resources used by this extractor and sub-extractors.
    /// </summary>
    public abstract void Dispose();
    /// <summary>
    /// Retrieve a new instance of the parser associated with this text entity.
    /// </summary>
    public abstract Parse GetParser();
    
    //-------------------------------------------//
    
    
    
    
  }
  
}
