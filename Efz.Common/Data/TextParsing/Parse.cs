/*
 * User: Joshua
 * Date: 20/08/2016
 * Time: 10:03 AM
 */
using System;

namespace Efz.Text {
  
  /// <summary>
  /// Base class representing a parsing rule.
  /// </summary>
  public abstract class Parse {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Has the parser started parsing a section of text?
    /// </summary>
    public virtual bool Active { get; protected set; }
    /// <summary>
    /// The current index within the latest block.
    /// </summary>
    public int Index { get; protected set; }
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new text parser instance.
    /// </summary>
    protected Parse() {
    }
    
    /// <summary>
    /// Clear the state of this parser.
    /// </summary>
    public abstract void Reset();
    
    /// <summary>
    /// Parse the specified characters and test against the scheme and it's children. Returns if it's
    /// requirements are satisfied.
    /// </summary>
    public virtual bool Next(string str) {
      return Next(str.ToCharArray(), 0, str.Length);
    }
    
    /// <summary>
    /// Parse the specified characters and test against the scheme and it's children. Returns if it's
    /// requirements are satisfied.
    /// </summary>
    public abstract bool Next(char[] characters, int start, int end);
    
    //-------------------------------------------//
    
    
  }
}
