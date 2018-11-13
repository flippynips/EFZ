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
  public class ParseElement : Parse {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// The extract defining the elements to extract.
    /// </summary>
    protected ExtractElement _extract;
    /// <summary>
    /// Definition of the section to extract.
    /// </summary>
    protected ExtractSection _section;
    /// <summary>
    /// The section parse class to find the main element.
    /// </summary>
    internal ParseSection _sectionParse;
    
    /// <summary>
    /// Dynamic search of parameter keys.
    /// </summary>
    protected TreeSearch<char, string>.DynamicSearch _attributeGetSearch;
    /// <summary>
    /// Dynamic search of parameter keys.
    /// </summary>
    protected TreeSearch<char, string>.DynamicSearch _attributeFilterSearch;
    /// <summary>
    /// Number of attribute filters.
    /// </summary>
    protected int _attributeFilterCount;
    /// <summary>
    /// Body parsers.
    /// </summary>
    protected ArrayRig<Parse> _bodyParsers;
    
    protected string _getKey = null;
    protected bool _getSet = false;
    protected bool _getQuoteOpen = false;
    protected int _getQuoteStart = 0;
    protected bool _filterSet = false;
    protected bool _filterQuoteOpen = false;
    protected int _filterQuoteStart = 0;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize an element rule with a tag to parse and parameters to return.
    /// </summary>
    public ParseElement(ExtractElement extract) {
      _extract = extract;
      
      // set up the tag section search
      _section = new ExtractSection(new ActionSet<char[]>(OnSection));
      foreach(string tag in _extract.Tags) {
        _section.AddPrefix(Chars.LessThan + tag);
        _section.AddSuffix(Chars.ForwardSlash + tag + Chars.GreaterThan);
      }
      
      // setup the parsing
      _sectionParse = new ParseSection(_section);
      
      // setup the parameter dynamic search
      if(_extract.GetAttributes != null) {
        _attributeGetSearch = new TreeSearch<char, string>.DynamicSearch(_extract.GetAttributes);
      }
      if(_extract.FilterAttributes != null) {
        _attributeFilterSearch = new TreeSearch<char, string>.DynamicSearch(_extract.FilterAttributes);
        _attributeFilterCount = _extract.FilterAttributesCount;
      }
    }
    
    /// <summary>
    /// Clear the state of this parser.
    /// </summary>
    public override void Reset() {
      _sectionParse.Reset();
      _attributeGetSearch.Reset();
      _attributeFilterSearch.Reset();
    }
    
    /// <summary>
    /// Parse the specified characters and test against the scheme and it's children. Returns if it's
    /// requirements are satisfied.
    /// </summary>
    public override bool Next(char[] characters, int start, int end) {
      return _sectionParse.Next(characters, start, end);
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// When an element is found. Parses the element.
    /// </summary>
    protected void OnSection(char[] characters) {
      
      // if the callback has been set
      if(_extract.OnElementSet) {
        
        ExtractElement.Element element = new ExtractElement.Element();
        int index = -1;
        // get the parameters before the body
        if(_attributeGetSearch != null) {
          element.Parameters = new ArrayRig<Struct<string, string>>();
          
          while(++index < characters.Length) {
            char character = characters[index];
            
            // if the end of tag
            if(character == Chars.GreaterThan) {
               // finished parameters
              break;
            }
            
            // if found a parameter key
            if(_attributeFilterSearch != null && _attributeFilterSearch.Next(character)) {
              
              // if parameter value wasn't found
              if(_filterSet) {
                _filterSet = false;
                // was the value null?
                if(_attributeFilterSearch.Values[0] == null) --_attributeFilterCount;
              } else {
                // the filter key has been found
                _filterSet = true;
              }
              
            } else if(_filterSet) {
              
              // get the parameter value
              if(_filterQuoteOpen) {
                if(character == Chars.DoubleQuote) {
                  // got the key value
                  if(_attributeFilterSearch.Values[0] == null ||
                    _attributeFilterSearch.Values.Contains(new string(characters, _filterQuoteStart, index - _filterQuoteStart - 1))) {
                    --_attributeFilterCount;
                  }
                  _filterQuoteOpen = false;
                  _filterSet = false;
                }
              } else if(character == Chars.DoubleQuote) {
                _filterQuoteOpen = true;
                _filterQuoteStart = index + 1;
              }
              
            }
            
            // if found a parameter key
            if(_attributeGetSearch.Next(character)) {
              
              // if parameter value wasn't found
              if(_getSet) {
                _getSet = false;
                // add null parameter
                element.Parameters.Add(new Struct<string, string>(_getKey, null));
              } else {
                // set key value
                _getKey = _attributeGetSearch.Values[0];
                _getSet = true;
              }
              
            } else if(_getSet) {
              
              // get the parameter value
              if(_getQuoteOpen) {
                if(character == Chars.DoubleQuote) {
                  // got the key value
                  element.Parameters.Add(new Struct<string, string>(_getKey, new string(characters, _getQuoteStart, index - _getQuoteStart - 1)));
                  _getQuoteOpen = false;
                  _getSet = false;
                }
              } else if(character == Chars.DoubleQuote) {
                _getQuoteOpen = true;
                _getQuoteStart = index + 1;
              }
              
            }
            
          }
          
          // if the filter parameter wasn't found
          if(_filterSet) {
            _filterSet = false;
            // was the value null?
            if(_attributeFilterSearch.Values[0] == null) --_attributeFilterCount;
          }
          
          // if parameter value wasn't found
          if(_getSet) {
            _getSet = false;
            // add null parameter
            element.Parameters.Add(new Struct<string, string>(_getKey, null));
          }
          
        } else {
          // move past the tag element
          while(++index < characters.Length && characters[index] != Chars.GreaterThan) { }
        }
        
        // if element contains body
        if(_extract.ParseBody && index < characters.Length) {
          
          // read until the end of the body
          int bodyStart = index + 1;
          while(++index < characters.Length) {
            if(characters[index] == Chars.LessThan) {
              // set the element body
              element.Body = new string(characters, bodyStart, index - bodyStart - 1);
              
              // should the body extractors be run?
              if(_attributeFilterSearch == null || _attributeFilterCount == 0) {
                
                if(_extract.BodyExtracts != null) {
                  // run the body extractors
                  if(_bodyParsers == null) {
                    _bodyParsers = new ArrayRig<Parse>();
                    foreach(Extract extract in _extract.BodyExtracts) {
                      _bodyParsers.Add(extract.GetParser());
                    }
                  }
                  foreach(Parse parser in _bodyParsers) {
                    parser.Next(characters, bodyStart, index - bodyStart - 1);
                  }
                }
                
              }
              
              _attributeFilterCount = _extract.FilterAttributesCount;
              
              break;
            }
          }
          
        }
        
      } else if(_extract.ParseBody && _extract.BodyExtracts != null) {
        
        // move past the tag element
        int index = -1;
        while(++index < characters.Length && characters[index] != Chars.GreaterThan) { }
        
        // read until the end of the body
        int bodyStart = index + 1;
        while(++index < characters.Length) {
          if(characters[index] == Chars.LessThan) {
            
            // should the body extractors be run?
            if(_attributeFilterSearch == null || _attributeFilterCount == 0) {
              
              // run the body extractors
              if(_bodyParsers == null) {
                _bodyParsers = new ArrayRig<Parse>();
                foreach(Extract extract in _extract.BodyExtracts) {
                  _bodyParsers.Add(extract.GetParser());
                }
              }
              foreach(Parse parser in _bodyParsers) {
                parser.Next(characters, bodyStart, index - bodyStart - 1);
              }
              
            }
            
            _attributeFilterCount = _extract.FilterAttributesCount;
            
            break;
            
          }
        }
        
      }
      
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
  }
}
