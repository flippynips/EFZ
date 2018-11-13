/*
 * User: Joshua
 * Date: 6/08/2016
 * Time: 9:48 PM
 */
using System;

using Efz.Collections;
using Efz.Tools;

namespace Efz.Text {
  
  /// <summary>
  /// A description of the processing a dom element on a html website.
  /// </summary>
  public class ParseUrl : Parse {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Action called on successful parse.
    /// </summary>
    public IAction<string> OnUrl {
      get {
        return _onUrl;
      }
      set {
        _onUrl = value;
        _onUrlSet = _onUrl != null;
      }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// The extract defining the elements to extract.
    /// </summary>
    protected ExtractUrl _extract;
    
    /// <summary>
    /// Is the on parse callback set?
    /// </summary>
    protected bool _onUrlSet;
    /// <summary>
    /// Inner action called on successful parse.
    /// </summary>
    protected IAction<string> _onUrl;
    
    /// <summary>
    /// The string builder for the urls.
    /// </summary>
    protected char[] _chars;
    /// <summary>
    /// Current index within the buffer.
    /// </summary>
    protected int _charsIndex;
    
    /// <summary>
    /// Last string parsed.
    /// </summary>
    protected string _lastUrl;
    
    /// <summary>
    /// Dynamic search of parameter keys.
    /// </summary>
    protected TreeSearch<char, char[]>.ProgressiveSearch _protocolSearch;
    
    /// <summary>
    /// The collection of sub extracts.
    /// </summary>
    protected ArrayRig<Parse> _subExtracts;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize an element rule with a tag to parse and parameters to return.
    /// </summary>
    public ParseUrl(ExtractUrl extract) {
      _extract = extract;
      
      // setup the parameter dynamic search
      _protocolSearch = _extract.ProtocolSearch.SearchProgressive();
      
      // setup the sub extracts
      if(_extract.SubExtractSet) {
        _subExtracts = new ArrayRig<Parse>();
        foreach(Extract subExtract in _extract.SubExtract) {
          _subExtracts.Add(subExtract.GetParser());
        }
      }
      
      // setup the buffer
      _chars = new char[extract.MaxLength];
    }
    
    /// <summary>
    /// Clear the state of this parser.
    /// </summary>
    public override void Reset() {
      _protocolSearch.Reset();
    }
    
    /// <summary>
    /// Parse the specified characters and test against the scheme and it's children. Returns if it's
    /// requirements are satisfied.
    /// </summary>
    public override bool Next(char[] characters, int start, int end) {
      // whether the urls have been found
      bool foundUrl = false;
      // read address until a non url allowed character is read
      bool gotDot = false;
      // get if character is part of the url
      bool readUrl = false;
      
      // index within the current buffer
      int index = start-1;
      
      char character;
      // read until eof or protocol found
      while(++index != end) {
        
        // get the next character
        character = characters[index];
        
        // if reading url
        if(readUrl) {
          // if character is an unencoded url character
          if(character.IsUrlChar()) {
            
            // is the character a dot?
            if(character == Chars.Stop) {
              // yes, set flag
              gotDot = true;
            }
            
            // continue reading
            _chars[_charsIndex] = character;
            // has the max length been reached?
            if(++_charsIndex == _extract.MaxLength) {
              // yes, end
              readUrl = false;
            }
            
          } else if(gotDot) {
            
            // does the url pass dirty validation?
            if(_chars[_charsIndex-1] == Ascii.Comma ||
               _chars[_charsIndex-1] == Ascii.Stop) {
              // no, ignore
              readUrl = false;
              continue;
            }
            
            string urlStr = new string(_chars, 0, _charsIndex);
            // has the url already been parsed?
            if(!urlStr.Equals(_lastUrl, StringComparison.Ordinal)) {
              // set the last parsed
              _lastUrl = urlStr;
              
              // are there sub extracts?
              if(_extract.SubExtractSet) {
                // run the sub extractors
                foreach(Parse parser in _subExtracts) {
                  parser.Next(_chars, 0, _charsIndex);
                }
              }
              
              if(_onUrlSet) {
                _onUrl.ArgA = urlStr;
                _onUrl.Run();
              }
              
              // is the callback set?
              if(_extract.OnUrlSet) {
                // yes, run the callback
                _extract.OnUrl.Take();
                _extract.OnUrl.Item.ArgA = urlStr;
                _extract.OnUrl.Item.Run();
                _extract.OnUrl.Release();
              }
              
              // return positive
              foundUrl = true;
            }
            
            // end url parsing
            readUrl = false;
            
          } else {
            // no, end url parsing
            readUrl = false;
          }
          
        } else if(_protocolSearch.Next(character)) {
          // the search tree has arrived at a protocol node
          
          // start the buffer
          _charsIndex = _protocolSearch.Value.Length;
          // add the protocol to the buffer
          Array.Copy(_protocolSearch.Value, _chars, _charsIndex);
          
          // has the max length been reached?
          if(_charsIndex > _extract.MaxLength) {
            // yes, end
            readUrl = false;
          } else {
            _protocolSearch.Reset();
            gotDot = false;
            readUrl = true;
          }
          
        }
      }
      
      // return whether a url was found
      return foundUrl;
    }
    
    //-------------------------------------------//
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
  }
}
