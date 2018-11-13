/*
 * User: Joshua
 * Date: 6/08/2016
 * Time: 9:48 PM
 */
using System;

using Efz.Collections;

namespace Efz.Text {
  
  /// <summary>
  /// A description of the processing a dom element on a html website.
  /// </summary>
  public class ParseAll : Parse {
    
    //-------------------------------------------//
    
    /// <summary>
    /// On a string being parsed.
    /// </summary>
    public IAction<string> OnParsed;
    
    //-------------------------------------------//
    
    /// <summary>
    /// The extract defining the elements to extract.
    /// </summary>
    protected ExtractAll _extract;
    
    /// <summary>
    /// Required parsers.
    /// </summary>
    protected ArrayRig<Parse> _reqParsers;
    /// <summary>
    /// Have the required parsers been set?
    /// </summary>
    protected bool _reqParsersSet;
    /// <summary>
    /// Sub parsers.
    /// </summary>
    protected ArrayRig<Parse> _subParsers;
    /// <summary>
    /// Have the sub parsers been set?
    /// </summary>
    protected bool _subParsersSet;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize an element rule with a tag to parse and parameters to return.
    /// </summary>
    public ParseAll(ExtractAll extract) {
      _extract = extract;
      
      _reqParsersSet = _extract.ReqExtracts != null;
      _subParsersSet = _extract.SubExtracts != null;
      if(_reqParsersSet) {
        _reqParsers = new ArrayRig<Parse>();
        foreach(Extract ext in _extract.ReqExtracts) {
          _reqParsers.Add(ext.GetParser());
        }
      }
      if(_subParsersSet) {
        _subParsers = new ArrayRig<Parse>();
        foreach(Extract ext in _extract.SubExtracts) {
          _subParsers.Add(ext.GetParser());
        }
      }
    }
    
    /// <summary>
    /// Clear the state of this parser.
    /// </summary>
    public override void Reset() {
    }
    
    /// <summary>
    /// Parse the specified characters and test against the scheme and it's children. Returns if it's
    /// requirements are satisfied.
    /// </summary>
    public override bool Next(char[] characters, int start, int end) {
      
      Active = true;
      // have the required parsers been set?
      if(_reqParsersSet) {
        // yes, iterate the parsers
        foreach(Parse parser in _reqParsers) {
          if(!parser.Next(characters, start, end)) {
            Active = false;
          }
        }
      }
      // were the required parsers successful and have the sub parsers been set?
      if(Active && _subParsersSet) {
        // yes, iterate the sub parsers
        foreach(Parse parser in _subParsers) {
          Active |= parser.Next(characters, start, end);
        }
      }
      return Active;
    }
    
    //-------------------------------------------//
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
  }
}
