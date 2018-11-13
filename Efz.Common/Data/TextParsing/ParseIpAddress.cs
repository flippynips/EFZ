/*
 * User: Joshua
 * Date: 6/08/2016
 * Time: 9:48 PM
 */
using System;

using Efz.Collections;

namespace Efz.Text {
  
  /// <summary>
  /// A parsing module for ip addresses in blocks of text.
  /// </summary>
  public class ParseIpAddress : Parse {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Action called on successful parse.
    /// Returns secondary value as 'True' if ipv4.
    /// </summary>
    public IAction<string, bool> OnIpAddress {
      get {
        return _onIpAddress;
      }
      set {
        _onIpAddress = value;
        _onIpAddressSet = _onIpAddress != null;
      }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// The extract defining the elements to extract.
    /// </summary>
    protected ExtractIpAddress _extract;
    
    /// <summary>
    /// Is the on parse callback set?
    /// </summary>
    protected bool _onIpAddressSet;
    /// <summary>
    /// Inner action called on successful parse.
    /// </summary>
    protected IAction<string, bool> _onIpAddress;
    
    /// <summary>
    /// The string builder for the ip addresses.
    /// </summary>
    protected char[] _chars;
    /// <summary>
    /// Current index within the buffer.
    /// </summary>
    protected int _charsIndex;
    
    /// <summary>
    /// Last string parsed.
    /// </summary>
    protected string _lastIpAddress;
    /// <summary>
    /// The collection of sub parsers.
    /// </summary>
    protected ArrayRig<Parse> _subParsers;
    
    /// <summary>
    /// Current index within the structure.
    /// </summary>
    protected int _structureIndex;
    
    /// <summary>
    /// Is an ip address potentially being read?
    /// </summary>
    protected bool _readingIpAddress;
    
    /// <summary>
    /// The current index value in the ip address being read.
    /// </summary>
    protected int _valueIndex;
    /// <summary>
    /// The current section being read from the ip address.
    /// Ipv4 contains 4 and Ipv6 contains 6.
    /// </summary>
    protected int _sectionIndex;
    /// <summary>
    /// Is the ip address potentially ipv4?
    /// </summary>
    protected bool _isIpv4;
    /// <summary>
    /// Is the ip address potentially ipv6?
    /// </summary>
    protected bool _isIpv6;
    
    /// <summary>
    /// Seprarators for segments of ip addresses.
    /// </summary>
    protected static readonly char[] _separators = { Chars.Stop, Chars.Colon };
    protected static readonly char[] _hex = {'a','b','c','d','e','f','A','B','C','D','E','F'};
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize an element rule with a tag to parse and parameters to return.
    /// </summary>
    public ParseIpAddress(ExtractIpAddress extract) {
      _extract = extract;
      
      // setup the sub extracts
      if(_extract.SubExtractSet) {
        _subParsers = new ArrayRig<Parse>();
        foreach(Extract subExtract in _extract.SubExtract) {
          _subParsers.Add(subExtract.GetParser());
        }
      }
      
      // setup the buffer
      _chars = new char[extract.MaxLength];
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
      // whether an ip address has been found
      bool foundAddress = false;
      
      // index within the current buffer
      int index = start-1;
      
      char character;
      // read until eob or protocol found
      while(++index < end) {
        
        character = Chars.Null;
        try {
          // get the next character
          character = characters[index];
        } catch {
          throw new Exception("Yeah ParseIpAddress needs checking.");
        }
        
        // is an ip address being read?
        if(_readingIpAddress) {
          
          // has the ip address type been determined?
          if(_isIpv4 && _isIpv6) {
            // no, continue
            if(Char.IsDigit(character)) {
              
              _chars[_charsIndex] = character;
              ++_charsIndex;
              ++_valueIndex;
              
              if(_valueIndex == 4) _isIpv4 = false;
              
            } else if(_hex.Contains(character)) {
              _isIpv4 = false;
              _chars[_charsIndex] = character;
              ++_charsIndex;
              ++_valueIndex;
              
              if(_valueIndex == 4) _isIpv4 = false;
              
            } else if(_separators.Contains(character)) {
              
              _chars[_charsIndex] = character;
              ++_charsIndex;
              ++_sectionIndex;
              _valueIndex = 0;
              
              if(_sectionIndex == 10) {
                // clear the read
                _readingIpAddress = false;
              }
              
            } else {
              
              // is the ip address of valid length?
              if(_sectionIndex == 4 || _sectionIndex == 5 || _sectionIndex == 8 || _sectionIndex == 9) {
                // yes, pass it
                foundAddress = true;
                
                // the ip address was read
                _lastIpAddress = new string(_chars, 0, _charsIndex);
                
                // any sub parsers to run?
                if(_subParsers != null) {
                  // yes, run them
                  foreach(Parse parse in _subParsers) {
                    parse.Next(_chars, 0, _charsIndex);
                  }
                }
                
                // is the callback set?
                if(_onIpAddressSet) {
                  // yes, run it with the parsed address
                  _onIpAddress.ArgB = _sectionIndex < 8;
                  _onIpAddress.ArgA = _lastIpAddress;
                  _onIpAddress.Run();
                }
                
                // is the extract callback set?
                if(_extract.OnIpAddressSet) {
                  // yes, run the callback
                  _extract.OnIpAddress.Take();
                  _extract.OnIpAddress.Item.ArgA = _lastIpAddress;
                  _extract.OnIpAddress.Item.ArgB = _sectionIndex < 8;
                  _extract.OnIpAddress.Item.Run();
                  _extract.OnIpAddress.Release();
                }
                
              }
              
              // clear the read
              _readingIpAddress = false;
            }
            
          } else if(_isIpv4) {
            
            if(Char.IsDigit(character)) {
              _chars[_charsIndex] = character;
              ++_charsIndex;
              ++_valueIndex;
              
              if(_valueIndex > 3) {
                // ip address isn't ipv4
                _readingIpAddress = false;
              }
            } else if(_separators.Contains(character)) {
              
              _chars[_charsIndex] = character;
              ++_charsIndex;
              ++_sectionIndex;
              _valueIndex = 0;
              
              if(_valueIndex == 4) {
                // stop reading - value out of range
                _readingIpAddress = false;
              }
              
            } else {
              // is the ip address of valid length?
              if(_sectionIndex == 4 || _sectionIndex == 5) {
                // yes, pass it
                foundAddress = true;
                
                _lastIpAddress = new string(_chars);
                
                // any sub parsers to run?
                if(_subParsers != null) {
                  // yes, run them
                  foreach(Parse parse in _subParsers) {
                    parse.Next(_chars, 0, _charsIndex);
                  }
                }
                
                // run callback
                if(_onIpAddressSet) {
                  _onIpAddress.ArgB = true;
                  _onIpAddress.ArgA = _lastIpAddress;
                  _onIpAddress.Run();
                }
              }
              
              // clear the read
              _readingIpAddress = false;
            }
            
          } else {
            
            if(Char.IsDigit(character) || _hex.Contains(character)) {
              _chars[_charsIndex] = character;
              ++_charsIndex;
              ++_valueIndex;
              
              if(_valueIndex == 5) {
                // stop reading - value out of range
                _readingIpAddress = false;
              }
              
            } else if(_separators.Contains(character)) {
              
              _chars[_charsIndex] = character;
              ++_charsIndex;
              ++_sectionIndex;
              _valueIndex = 0;
              
              if(_sectionIndex == 10) {
                // clear the read
                _readingIpAddress = false;
              }
              
            } else {
              
              // is the ip address of valid length?
              if(_sectionIndex == 8 || _sectionIndex == 9) {
                // yes, pass it
                foundAddress = true;
                
                _lastIpAddress = new string(_chars, 0, _charsIndex);
                
                // any sub parsers to run?
                if(_subParsers != null) {
                  // yes, run them
                  foreach(Parse parse in _subParsers) {
                    parse.Next(_chars, 0, _charsIndex);
                  }
                }
                
                // run callback
                if(_onIpAddressSet) {
                  _onIpAddress.ArgB = false;
                  _onIpAddress.ArgA = _lastIpAddress;
                  _onIpAddress.Run();
                }
              }
              
              // clear the read
              _readingIpAddress = false;
              
            }
          }
          
        } else if(Char.IsDigit(character)) {
          
          _readingIpAddress = true;
          _isIpv4 = true;
          _isIpv6 = true;
          
          _sectionIndex = 1;
          
          _chars[0] = character;
          _charsIndex = 1;
          _valueIndex = 1;
          
        } else if(_hex.Contains(character)) {
          
          _readingIpAddress = true;
          _isIpv4 = false;
          _isIpv6 = true;
          
          _sectionIndex = 1;
          
          _chars[0] = character;
          _charsIndex = 1;
          _valueIndex = 1;
          
        }
      }
      
      // return whether a url was found
      return foundAddress;
    }
    
    //-------------------------------------------//
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
  }
}
