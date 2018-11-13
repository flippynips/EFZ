/*
 * User: Joshua
 * Date: 6/08/2016
 * Time: 9:48 PM
 */
using System;

using System.Runtime.InteropServices;
using Efz.Collections;

namespace Efz.Text {
  
  /// <summary>
  /// Parser of individual lines.
  /// </summary>
  public class ParseLine : Parse {
    
    //-------------------------------------------//
    
    public IAction<char[]> OnLine {
      get {
        return _onLine;
      }
      set {
        _onLine = value;
        _onLineSet = _onLine != null;
      }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// The extract defining the elements to extract.
    /// </summary>
    protected ExtractLine _extract;
    
    /// <summary>
    /// All sub parsers.
    /// </summary>
    protected ArrayRig<Parse> _subParsers;
    /// <summary>
    /// All required parsers.
    /// </summary>
    protected ArrayRig<Parse> _reqParsers;
    
    /// <summary>
    /// Are there any sub parsers?
    /// </summary>
    protected bool _subParsersSet;
    /// <summary>
    /// Are there any required sub parsers?
    /// </summary>
    protected bool _reqParsersSet;
    
    /// <summary>
    /// Inner line callback.
    /// </summary>
    protected IAction<char[]> _onLine;
    /// <summary>
    /// Has the on line callback been set?
    /// </summary>
    protected bool _onLineSet;
    
    /// <summary>
    /// Character collection for the current line.
    /// </summary>
    protected char[] _line;
    /// <summary>
    /// The index in the current line.
    /// </summary>
    protected int _lineCount;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize an element rule with a tag to parse and parameters to return.
    /// </summary>
    public ParseLine(ExtractLine extract) {
      _extract = extract;
      _line = new char[_extract.CharBuffer];
      
      _reqParsersSet = extract.ReqExtracts != null;
      if(_reqParsersSet) {
        _reqParsers = new ArrayRig<Parse>();
        foreach(Extract req in extract.ReqExtracts) {
          _reqParsers.Add(req.GetParser());
        }
      }
      
      _subParsersSet = extract.SubExtracts != null;
      if(_subParsersSet) {
        _subParsers = new ArrayRig<Parse>();
        foreach(Extract sub in extract.SubExtracts) {
          _subParsers.Add(sub.GetParser());
        }
      }
    }
    
    /// <summary>
    /// Clear the state of this parser.
    /// </summary>
    public override void Reset() {
      _line = new char[_extract.CharBuffer];
    }
    
    /// <summary>
    /// Parse the specified characters and test against the scheme and it's children. Returns if it's
    /// requirements are satisfied.
    /// </summary>
    public unsafe override bool Next(char[] characters, int start, int end) {
      
      bool result = false;
      int index = start - 1;
      int count;
      
      // iterate
      while(++index < end) {
        
        // is the current character a new line?
        if(characters[index] == Chars.NewLine) {
          
          // yes, pass the last line to the sub parsers and callbacks
          
          // get the number of characters to copy
          count = index - start;
          
          // will the line buffer contain the next section?
          if(count > _line.Length - _lineCount) {
            // no, resize the line buffer
            Array.Resize(ref _line, _line.Length + count);
          }
          
          // copy the current section into the line buffer
          fixed(char* dst = &_line[_lineCount]) {
            
            // copy the current characters to the buffer
            Marshal.Copy(characters, start, (IntPtr)dst, count);
          }
          
          // increment the number of characters in the line
          _lineCount += count;
          // increment the start index
          start = index;
          
          bool active = true;
          // are the extract requirements fulfilled?
          if(_reqParsersSet) {
            foreach(Parse req in _reqParsers) {
              if (req.Next(_line, 0, _lineCount)) continue;
              active = false;
              break;
            }
          }
          
          if(active) {
            result = true;
            
            if(_subParsersSet) {
              foreach(Parse sub in _subParsers) {
                sub.Next(_line, 0, _lineCount);
              }
            }
            
            if(_onLineSet || _extract.OnLineCharsSet) {
              char[] charsCallback = new char[_lineCount];
              
              // copy the current line into the callback array
              fixed(char* dst = &charsCallback[0]) {
                
                // copy the current characters to the buffer
                Marshal.Copy(_line, 0, (IntPtr)dst, _lineCount);
              }
              
              if(_onLineSet) {
                _onLine.ArgA = charsCallback;
                _onLine.Run();
              }
              if(_extract.OnLineCharsSet) {
                _extract.OnLineChars.Take();
                _extract.OnLineChars.Item.ArgA = charsCallback;
                _extract.OnLineChars.Item.Run();
                _extract.OnLineChars.Release();
              }
            }
            
            if(_extract.OnLineSet) {
              _extract.OnLine.Take();
              _extract.OnLine.Item.Run();
              _extract.OnLine.Release();
            }
            
          }
          
          _lineCount = 0;
        }
        
      }
      
      count = end - start;
      
      // will the line buffer contain the next section?
      if(_line.Length - _lineCount < count) {
        
        // no, resize the line buffer
        Array.Resize(ref _line, _line.Length + count);
      }
      
      fixed(char* dst = &_line[_lineCount]) {
        
        // copy the remaining characters to the buffer
        Marshal.Copy(characters, start, (IntPtr)dst, count);
      }
      
      _lineCount += count;
      
      return result;
      
    }
    
    //-------------------------------------------//
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
  }
}
