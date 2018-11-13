/*
 * User: Joshua
 * Date: 20/08/2016
 * Time: 10:03 AM
 */
using System;
using System.Runtime.InteropServices;

using Efz.Collections;
using Efz.Tools;

namespace Efz.Text {
  
  /// <summary>
  /// Instance of parsing a rule.
  /// </summary>
  internal class ParseSection : Parse {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Current matching sections. This is a nested collection as
    /// sections of text.
    /// </summary>
    public ArrayRig<ArrayRig<char>> Sections;
    
    //-------------------------------------------//
    
    /// <summary>
    /// The definition of the section of text.
    /// </summary>
    protected ExtractSection _extract;
    /// <summary>
    /// Active section of the parser.
    /// </summary>
    protected ArrayRig<char> _currentSection;
    
    /// <summary>
    /// All sub parsers.
    /// </summary>
    protected ArrayRig<Parse> _subParsers;
    /// <summary>
    /// All required parsers.
    /// </summary>
    protected ArrayRig<Parse> _reqParsers;
    
    /// <summary>
    /// Active collection of sub parsers.
    /// </summary>
    protected ArrayRig<Parse> _activeSubParsers;
    
    /// <summary>
    /// The prefix dynamic search.
    /// </summary>
    protected TreeSearch<char, string>.DynamicSearch _prefixSearch;
    /// <summary>
    /// The suffix dynamic search.
    /// </summary>
    protected TreeSearch<char, string>.DynamicSearch _suffixSearch;
    
    /// <summary>
    /// Are there any sub parsers?
    /// </summary>
    protected bool _subParsersSet;
    /// <summary>
    /// Are there any required sub parsers?
    /// </summary>
    protected bool _reqParsersSet;
    /// <summary>
    /// Are there any suffixes? If not the max characters will be returned after
    /// requirements are fulfilled.
    /// </summary>
    protected bool _suffixesSet;
    /// <summary>
    /// Are there any prefixes? If not then the max characters will be returned
    /// after the requirements are fulfilled.
    /// </summary>
    protected bool _prefixesSet;
    
    /// <summary>
    /// Are the sub parsers setup?
    /// </summary>
    protected bool _subParsersSetup;
    /// <summary>
    /// Are the req parsers setup?
    /// </summary>
    protected bool _reqParsersSetup;
    
    /// <summary>
    /// Have the requirements been satisfied.
    /// </summary>
    protected bool _runSubParsers;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new scheme parser instance.
    /// </summary>
    public ParseSection(ExtractSection extract) {
      _extract = extract;
      
      // initialize the dynamic searches
      if(_extract.Prefixes != null) _prefixSearch = new TreeSearch<char, string>.DynamicSearch(_extract.Prefixes);
      if(_extract.Suffixes != null) _suffixSearch = new TreeSearch<char, string>.DynamicSearch(_extract.Suffixes);
      
      _suffixesSet = _suffixSearch != null;
      _prefixesSet = _prefixSearch != null;
      
      // initialize the section stack
      Sections = new ArrayRig<ArrayRig<char>>();
      _currentSection = new ArrayRig<char>(_prefixesSet ^ _suffixesSet ? _extract.MaxCharacters : 2);
      _activeSubParsers = new ArrayRig<Parse>();
      
      _subParsersSet = _extract.SubExtracts != null && _extract.SubExtracts.Count != 0;
      _reqParsersSet = _extract.ReqExtracts != null && _extract.ReqExtracts.Count != 0;
      
      if(_subParsersSet) {
        _subParsers = new ArrayRig<Parse>();
        foreach(Extract ext in _extract.SubExtracts) {
          _subParsers.Add(ext.GetParser());
        }
      }
      if(_reqParsersSet) {
        _reqParsers = new ArrayRig<Parse>();
        foreach(Extract ext in _extract.ReqExtracts) {
          _reqParsers.Add(ext.GetParser());
        }
      }
    }
    
    /// <summary>
    /// Clear the state of this parser.
    /// </summary>
    public override void Reset() {
      if(_suffixesSet) _suffixSearch.Reset();
      if(_prefixesSet) _prefixSearch.Reset();
      Sections.Reset();
      _currentSection.Reset();
      
      if(_subParsersSetup) {
        foreach(Parse parser in _subParsers) {
          parser.Reset();
        }
        _subParsers.Reset();
        _subParsersSetup = false;
      }
      
      if(_reqParsersSetup) {
        foreach(Parse parser in _reqParsers) {
          parser.Reset();
        }
        _reqParsers.Reset();
        _reqParsersSetup = false;
      }
      
      Active = false;
    }
    
    /// <summary>
    /// Parse the specified characters and test against the scheme and it's children. Returns if it's
    /// requirements are satisfied.
    /// </summary>
    public unsafe override bool Next(char[] characters, int start, int end) {
      
      // if the result isn't going anywhere - skip
      if(!_extract.OnSectionSet && _extract.SubExtracts.Count == 0) {
        // no result method or sub extracts
        return false;
      }
      
      // have the results of this parser been satisfied?
      if(_runSubParsers) {
        // yes, pass any characters to the sub-parsers
        foreach(Parse parser in _activeSubParsers) {
          if(!parser.Next(characters, start, end)) {
            _activeSubParsers.RemoveQuick(parser);
            _runSubParsers = _activeSubParsers.Count > 0;
          }
        }
      }
      
      // the ultimate result of this parse is whether a result is achieved and
      // passed to a return method or a sub extract method is similarly successful
      bool result = false;
      // the last index to be copied into the current section
      int lastIndex = start;
      
      // are there prefixes and suffixes?
      if(_prefixesSet && _suffixesSet) {
        // yes, both prefixes and suffixes
        
        // set the current index within the block
        Index = lastIndex - 1;
        
        // loop until the block is complete
        while(Index <= end) {
          
          // are there prefixes or has a match been found?
          if(Active) {
            
            // iterate characters in current block
            while(++Index < end) {
              char character = characters[Index];
              
              // if a suffix has been found
              if(_suffixSearch.Next(character)) {
                
                // if current section count + current block is greater than max allowed
                if(_currentSection.Count + Index - lastIndex > _extract.MaxCharacters) {
                  // overflow - reset section stack
                  Reset();
                  
                  // decrement the index to start searching for the prefix
                  --Index;
                  
                  // revert to prefix search
                  break;
                }
                
                ++Index;
                
                // is the current section able to contain the next block of characters?
                if(_currentSection.Capacity - _currentSection.Count < Index - lastIndex) {
                  // no, resize
                  _currentSection.SetCapacity(_currentSection.Capacity + Index - lastIndex);
                }
                
                // append the block up to the current index
                fixed(void* dst = &_currentSection.Array[_currentSection.Count]) {
                  Marshal.Copy(characters, lastIndex, (IntPtr)dst, Index - lastIndex);
                }
                
                // update the current section count
                _currentSection.Count += Index - lastIndex;
                // update the last index
                --Index;
                lastIndex = Index;
                
                // if there are nested requirements
                if(_reqParsersSet) {
                  // run required nested extracts
                  foreach(Parse reqParser in _reqParsers) {
                    // if required extract wasn't satisfied then not active
                    Active &= reqParser.Next(_currentSection.Array, 0, _currentSection.Count);
                  }
                }
                
                // if any requirements were fulfilled
                if(Active) {
                  // if there are sub parsers
                  if(_subParsersSet) {
                    
                    // begin running sub parsers
                    for(int i = _subParsers.Count-1; i >= 0; --i) {
                      if(_subParsers[i].Next(_currentSection.Array, 0, _currentSection.Count)) {
                        _activeSubParsers.Add(_subParsers[i]);
                      } else {
                        _activeSubParsers.RemoveQuick(_subParsers[i]);
                      }
                    }
                    _runSubParsers = _activeSubParsers.Count != 0;
                    
                  }
                  
                  // if section callback is set
                  if(_extract.OnSectionSet) {
                    // run complete section callback
                    _extract.OnSection.Take();
                    _extract.OnSection.Item.ArgA = _currentSection.ToArray();
                    _extract.OnSection.Item.Run();
                    _extract.OnSection.Release();
                    result = true;
                  }
                }
                
                // pop the last section and check max length
                if(Sections.Count > 1 && Sections[Sections.Count-1].Count + _currentSection.Count - 1 < _extract.MaxCharacters) {
                  // get the next session
                  Sections.Remove(Sections.Count-1);
                  ArrayRig<char> nextSection = Sections[Sections.Count-1];
                  
                  // will the next section contain the current sections characters?
                  if(nextSection.Capacity - nextSection.Count < _currentSection.Count) {
                    // no, resize
                    nextSection.SetCapacity(nextSection.Capacity + _currentSection.Count);
                  }
                  
                  // append the last section
                  fixed(void* dst = &nextSection.Array[0]) {
                    Marshal.Copy(_currentSection.Array, 0, (IntPtr)dst, _currentSection.Count);
                  }
                  
                  // update the next section count
                  nextSection.Count += _currentSection.Count;
                  
                  // set current section
                  _currentSection = nextSection;
                  
                  // flag for suffix search
                  Active = true;
                  
                } else {
                  // reset the section stack
                  Reset();
                  
                  // decrement index
                  --Index;
                  
                  // move back to prefix search
                  break;
                }
                
              }
              
              // has the prefix been found?
              if(_prefixSearch.Next(character)) {
                // nested prefix has been found
                
                // check max length of current section
                if(_currentSection.Count + Index - lastIndex > _extract.MaxCharacters) {
                  
                  // overflow - reset the sections stack
                  Reset();
                  
                } else {
                  // add the current section to the stack
                  Sections.Add(_currentSection);
                  
                  // initialize new nested section
                  _currentSection = new ArrayRig<char>();
                }
                
                // get the prefix char array
                char[] prefix = _prefixSearch.Values[0].ToCharArray();
                
                // if the current section won't contain the prefix
                if(_currentSection.Capacity - _currentSection.Count < prefix.Length) {
                  // resize
                  _currentSection.SetCapacity(_currentSection.Capacity + prefix.Length);
                }
                
                // copy the prefix charcters to the current section
                fixed(void* dst = &_currentSection.Array[_currentSection.Count]) {
                  Marshal.Copy(prefix, 0, (IntPtr)dst, prefix.Length);
                }
                
                // update the section count
                _currentSection.Count += prefix.Length;
                
              }
              
            }
            
            // if block is finished
            if(Index == end - 1) {
              // check max length
              if(_currentSection.Count + Index - lastIndex > _extract.MaxCharacters) {
                
                // overflow - reset
                Reset();
                
              } else {
                
                // if the current section won't contain the block
                if(_currentSection.Capacity - _currentSection.Count < Index - lastIndex) {
                  // resize
                  _currentSection.SetCapacity(_currentSection.Capacity + Index - lastIndex);
                }
                
                // copy the block charcters to the current section
                fixed(void* dst = &_currentSection.Array[_currentSection.Count]) {
                  Marshal.Copy(characters, lastIndex, (IntPtr)dst, Index - lastIndex);
                }
                
                // update the section count
                _currentSection.Count += Index - lastIndex;
                lastIndex = Index;
              }
              
              // return the result of the current block
              return result;
            }
            
          }
          
          // search for a prefix
          
          // iterate chacters in current block
          while(++Index < end) {
            char character = characters[Index];
            
            // if a prefix has been found
            if(_prefixSearch.Next(character)) {
              
              // get the prefix char array
              char[] prefix = _prefixSearch.Values[0].ToCharArray();
              
              // is the current section collection large enough for the prefix?
              if(_currentSection.Capacity < prefix.Length) {
                // no, resize
                _currentSection.SetCapacity(_currentSection.Capacity + prefix.Length);
              }
              
              // copy the prefix charcters to the current section
              fixed(void* dst = &_currentSection.Array[_currentSection.Count]) {
                Marshal.Copy(prefix, 0, (IntPtr)dst, prefix.Length);
              }
              
              // update the section count
              _currentSection.Count += prefix.Length;
              // update the last index
              lastIndex = Index + 1;
              
              // catch up the suffix search (for partially overlapping prefixes and suffixes)
              foreach(char c in prefix) {
                _suffixSearch.Next(c);
              }
              
              // set to look for suffix
              Active = true;
              
              // go on to find a suffix
              break;
            }
            
          }
          
          // if block is finished
          if(!Active) {
            return result;
          }
          
        }
        
      } else if(_prefixesSet) {
        // no, prefixes only
        
        // iterate chacters in current block
        while(++Index < end) {
          char character = characters[Index];
          
          // if a prefix has been found
          if(_prefixSearch.Next(character)) {
            
            // get the prefix
            char[] prefix = _prefixSearch.Values[0].ToCharArray();
            
            // is the parser already active?
            if(Active) {
              // yes, a nested prefix has been found
              
              // create a new section
              var newSection = new ArrayRig<char>(_extract.MaxCharacters < prefix.Length ? prefix.Length : _extract.MaxCharacters);
              
              // copy the prefix charcters to the new section
              fixed(void* dst = &newSection.Array[0]) {
                Marshal.Copy(prefix, 0, (IntPtr)dst, prefix.Length);
              }
              
              // update the new section count
              newSection.Count += prefix.Length;
              
              /////// copy the current block to the current sections
              
              // is the current section going to be full?
              while(_currentSection.Count + Index - lastIndex >= _extract.MaxCharacters) {
                
                /////// yes, complete the current section
                
                // resize the current section if needed
                if(_extract.MaxCharacters > _currentSection.Capacity) {
                  _currentSection.SetCapacity(_extract.MaxCharacters);
                }
                
                // are there null characters in the current section?
                if(_extract.MaxCharacters > _currentSection.Count) {
                  // yes, append the block to the current section
                  fixed(void* dst = &_currentSection.Array[_currentSection.Count]) {
                    Marshal.Copy(characters, lastIndex, (IntPtr)dst, _extract.MaxCharacters - _currentSection.Count);
                  }
                  
                  // update the current section count
                  _currentSection.Count = _extract.MaxCharacters;
                }
                
                // are there requirements?
                if(_reqParsersSet) {
                  // yes, run the required parsers
                  foreach(Parse parser in _reqParsers) {
                    result |= parser.Next(_currentSection.Array, 0, _currentSection.Count);
                  }
                } else {
                  // no, the parse was successful
                  result = true;
                }
                
                // have the requirements been fulfilled?
                if(result) {
                  
                  // are there sub parsers?
                  if(_subParsersSet) {
                    
                    // yes, begin running sub parsers
                    for(int i = _subParsers.Count-1; i >= 0; --i) {
                      if(_subParsers[i].Next(_currentSection.Array, 0, _currentSection.Count)) {
                        _activeSubParsers.Add(_subParsers[i]);
                      } else {
                        _activeSubParsers.RemoveQuick(_subParsers[i]);
                      }
                    }
                    _runSubParsers = _activeSubParsers.Count != 0;
                    
                  }
                  
                  // is the section callback set?
                  if(_extract.OnSectionSet) {
                    // yes, run complete section callback
                    _extract.OnSection.Take();
                    _extract.OnSection.Item.ArgA = _currentSection.ToArray();
                    _extract.OnSection.Item.Run();
                    _extract.OnSection.Release();
                    result = true;
                  }
                  
                }
                
                // are there more sections?
                if(Sections.Count == 0) {
                  // no, exit the iteration
                  _currentSection.Reset();
                  break;
                }
                // yes, get the next section
                _currentSection.Dispose();
                _currentSection = Sections[0];
                Sections.Remove(0);
                
              }
              
              // are there any partial sections apart from the new section?
              if(_currentSection.Count != 0) {
                
                ////// yes, copy the current block to all other sections that will not be filled
                
                // resize the current section if needed
                if(_currentSection.Count + Index - lastIndex > _currentSection.Capacity) {
                  _currentSection.SetCapacity(_currentSection.Count + Index - lastIndex);
                }
                
                // append the block to the current section
                fixed(void* dst = &_currentSection.Array[_currentSection.Count]) {
                  Marshal.Copy(characters, lastIndex, (IntPtr)dst, Index - lastIndex);
                }
                
                // update the section count
                _currentSection.Count += Index - lastIndex;
                
                // iterate all sections
                foreach(ArrayRig<char> section in Sections) {
                  
                  // append the block to each section
                  fixed(void* dst = &section.Array[section.Count]) {
                    Marshal.Copy(characters, lastIndex, (IntPtr)dst, Index - lastIndex);
                  }
                  
                  // update the section count
                  section.Count += Index - lastIndex;
                }
                
                // add the new section
                Sections.Add(newSection);
              } else {
                
                // no, the new section is the only section left
                _currentSection = newSection;
                
              }
              
              // update the last index
              lastIndex = Index;
              
            } else {
              // no, start the first section
              
              // will the section contain the prefix?
              if(prefix.Length > _currentSection.Capacity) {
                // nope, resize
                _currentSection.SetCapacity(prefix.Length);
              }
              
              // copy the characters to the current section
              fixed(void* dst = &_currentSection.Array[0]) {
                Marshal.Copy(prefix, 0, (IntPtr)dst, prefix.Length);
              }
              
              // update the section count
              _currentSection.Count = prefix.Length;
              
              // update the last index
              lastIndex = Index;
            }
            
            // set parser active
            Active = true;
            
          }
          
        }
        
        ////// copy the remaining characters to the current sections if any
        
        // is there a prefix and is the current section going to be filled by the current block?
        while(Active && _currentSection.Count + Index - lastIndex >= _extract.MaxCharacters) {
          
          /////// yes, complete the current section
          
          // resize the current section if needed
          if(_extract.MaxCharacters > _currentSection.Capacity) {
            _currentSection.SetCapacity(_extract.MaxCharacters);
          }
          
          // is there space in the current section?
          if(_extract.MaxCharacters > _currentSection.Count) {
            // yes, fill the block to the current section
            fixed(void* dst = &_currentSection.Array[_currentSection.Count]) {
              Marshal.Copy(characters, lastIndex, (IntPtr)dst, _extract.MaxCharacters - _currentSection.Count);
            }
            
            // update the current section count
            _currentSection.Count = _extract.MaxCharacters;
          }
          
          // are there requirements?
          if(_reqParsersSet) {
            // yes, run the required parsers
            foreach(Parse parser in _reqParsers) {
              result |= parser.Next(_currentSection.Array, 0, _currentSection.Count);
            }
          } else {
            // no, the parse was successful
            result = true;
          }
          
          // have the requirements been fulfilled?
          if(result) {
            
            // are there sub parsers?
            if(_subParsersSet) {
              
              // yes, begin running sub parsers
              for(int i = _subParsers.Count-1; i >= 0; --i) {
                if(_subParsers[i].Next(_currentSection.Array, 0, _currentSection.Count)) {
                  _activeSubParsers.Add(_subParsers[i]);
                } else {
                  _activeSubParsers.RemoveQuick(_subParsers[i]);
                }
              }
              _runSubParsers = _activeSubParsers.Count != 0;
              
            }
            
            // is the section callback set?
            if(_extract.OnSectionSet) {
              // yes, run complete section callback
              _extract.OnSection.Take();
              _extract.OnSection.Item.ArgA = _currentSection.ToArray();
              _extract.OnSection.Item.Run();
              _extract.OnSection.Release();
              result = true;
            }
            
          }
          
          // are there more sections?
          if(Sections.Count == 0) {
            // no, exit the iteration
            _currentSection.Reset();
            break;
          }
          
          // yes, get the next section
          _currentSection.Dispose();
          _currentSection = Sections[0];
          Sections.Remove(0);
          
        }
        
        // are there any partial sections left?
        if(_currentSection.Count != 0) {
          
          ////// yes, copy the current block to all other sections thatill not be filled w
          
          // resize the current section if needed
          if(_currentSection.Count + Index - lastIndex > _currentSection.Capacity) {
            _currentSection.SetCapacity(_currentSection.Count + Index - lastIndex);
          }
          
          // append the block to the current section
          fixed(void* dst = &_currentSection.Array[_currentSection.Count]) {
            Marshal.Copy(characters, lastIndex, (IntPtr)dst, Index - lastIndex);
          }
          
          // update the section count
          _currentSection.Count += Index - lastIndex;
          
          // iterate all sections
          foreach(ArrayRig<char> section in Sections) {
            
            // append the block to each section
            fixed(void* dst = &section.Array[section.Count]) {
              Marshal.Copy(characters, lastIndex, (IntPtr)dst, Index - lastIndex);
            }
            
            // update the section count
            section.Count += Index - lastIndex;
          }
        }
        
      } else if(_suffixesSet) {
        // no, suffixes only
        
        // iterate characters in current block
        while(++Index < end) {
          char character = characters[Index];
          
          // does the current character complete a suffix?
          if(_suffixSearch.Next(character)) {
            // yes, push current character block
            
            // if current section count + current block is greater than max allowed
            if(_currentSection.Count + Index - lastIndex > _extract.MaxCharacters) {
              // overflow - move the current section
              
              // shift characters from the current section
              fixed(void* dst = &_currentSection.Array[0]) {
                Marshal.Copy(_currentSection.Array, Index - lastIndex, (IntPtr)dst, _currentSection.Count - Index - lastIndex);
              }
              
              // set the new count
              _currentSection.Count = _currentSection.Count - Index - lastIndex;
            }
            
            // resize the current section if needed
            if(_currentSection.Count + Index - lastIndex > _currentSection.Capacity) {
              _currentSection.SetCapacity(_currentSection.Count + Index - lastIndex);
            }
            
            // append the block up to the current index
            fixed(void* dst = &_currentSection.Array[_currentSection.Count]) {
              Marshal.Copy(characters, lastIndex, (IntPtr)dst, Index - lastIndex);
            }
            
            // update the current section count
            _currentSection.Count += Index - lastIndex;
            // update the last index
            lastIndex = Index;
            
            // are there requirements?
            if(_reqParsersSet) {
              // yes, run the required parsers
              foreach(Parse parser in _reqParsers) {
                result |= parser.Next(_currentSection.Array, 0, _currentSection.Count);
              }
            } else result = true;
            
            // have the requirements been fulfilled?
            if(result) {
              
              // are there sub parsers?
              if(_subParsersSet) {
                
                // yes, begin running sub parsers
                for(int i = _subParsers.Count-1; i >= 0; --i) {
                  if(_subParsers[i].Next(_currentSection.Array, 0, _currentSection.Count)) {
                    _activeSubParsers.Add(_subParsers[i]);
                  } else {
                    _activeSubParsers.RemoveQuick(_subParsers[i]);
                  }
                }
                _runSubParsers = _activeSubParsers.Count != 0;
                
              }
              
              // is the section callback set?
              if(_extract.OnSectionSet) {
                // yes, run complete section callback
                _extract.OnSection.Take();
                _extract.OnSection.Item.ArgA = _currentSection.ToArray();
                _extract.OnSection.Item.Run();
                _extract.OnSection.Release();
                result = true;
              }
              
            }
            
          }
          
        }
        
        // copy the current character block
        
        // if current section count + current block is greater than max allowed
        if(_currentSection.Count + Index - lastIndex > _extract.MaxCharacters) {
          // overflow - move the current section
          
          // shift characters in the current section
          fixed(void* dst = &_currentSection.Array[0]) {
            Marshal.Copy(_currentSection.Array, Index - lastIndex, (IntPtr)dst, _currentSection.Count - Index - lastIndex);
          }
          
          // set the new count
          _currentSection.Count = _currentSection.Count - Index - lastIndex;
        }
        
        // resize the current section if needed
        if(_currentSection.Count + Index - lastIndex > _currentSection.Capacity) {
          _currentSection.SetCapacity(_currentSection.Count + Index - lastIndex);
        }
        
        // append the block up to the current index
        fixed(void* dst = &_currentSection.Array[_currentSection.Count]) {
          Marshal.Copy(characters, lastIndex, (IntPtr)dst, Index - lastIndex);
        }
        
        // update the current section count
        _currentSection.Count += Index - lastIndex;
        
      } else {
        // no prefixes or suffixes
        
        // are there requirements?
        if(_reqParsersSet) {
          // yes, check requirements
          foreach(Parse parser in _reqParsers) {
            result |= parser.Next(characters, start, end);
          }
        } else {
          // no, the result is positive
          result = true;
        }
        
        // is the parser becoming active?
        if(result && !Active) {
          // yes, update state
          Active = true;
          
          // run sub parsers
          if(!_runSubParsers) {
            _runSubParsers = true;
            // iterate sub parsers
            for(int i = _subParsers.Count-1; i >= 0; --i) {
              if(_subParsers[i].Next(_currentSection.Array, 0, _currentSection.Count)) {
                _activeSubParsers.Add(_subParsers[i]);
              } else {
                _activeSubParsers.RemoveQuick(_subParsers[i]);
              }
            }
          }
        }
      }
      
      // return the result
      return result;
      
    }
    
    //-------------------------------------------//
    
  }
}
