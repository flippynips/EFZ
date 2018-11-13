/*
 * User: FloppyNipples
 * Date: 29/01/2017
 * Time: 12:11
 */
using System;
using System.IO;
using System.Text;

using Efz.Collections;

namespace Efz.Web.Display {
  
  /// <summary>
  /// Class that handles parsing a plain string web page into an element structure.
  /// </summary>
  public class ElementParser {
    
    //----------------------------------//
    
    /// <summary>
    /// Callback on the page being parsed.
    /// </summary>
    public IAction<ElementParser> OnParsed {
      get {
        return _onParsed;
      }
      set {
        _onParsed = value;
        if(_onParsed != null) _onParsed.ArgA = this;
      }
    }
    
    /// <summary>
    /// The element that contains all parsed elements.
    /// </summary>
    public Element Root;
    /// <summary>
    /// String that describes the error encountered if the page isn't parsed.
    /// </summary>
    public string Error;
    /// <summary>
    /// Path of the html being parsed.
    /// </summary>
    public string Path;
    
    public const string EscapeOpenTag = "&lt;";
    public const string EscapeCloseTag = "&gt;";
    public const string EscapeAnd = "&amp;";
    
    //----------------------------------//
    
    /// <summary>
    /// On the page parse completing.
    /// </summary>
    protected IAction<ElementParser> _onParsed;
    
    /// <summary>
    /// Stream to the file.
    /// </summary>
    protected Stream _stream;
    /// <summary>
    /// Reader of the page file.
    /// </summary>
    protected ByteBuffer _reader;
    
    /// <summary>
    /// Current stack of elements.
    /// </summary>
    protected Stack<Element> _elementStack;
    /// <summary>
    /// The current element being parsed.
    /// </summary>
    protected Element _element;
    
    /// <summary>
    /// Builder of all string sections.
    /// </summary>
    protected StringBuilder _builder;
    
    /// <summary>
    /// Current section being parsed.
    /// </summary>
    protected enum Section {
      Body = 1,
      Tag = 2,
    }
    
    /// <summary>
    /// Current section of the page.
    /// </summary>
    protected Section _section;
    /// <summary>
    /// Should the content of the current section be skipped?
    /// </summary>
    protected bool _skipContent;
    
    //----------------------------------//
    
    /// <summary>
    /// Start a new page parser for the specified local file.
    /// </summary>
    public ElementParser(string localFile, Action<ElementParser> onParsed) :
      this(localFile, new ActionSet<ElementParser>(onParsed)) {}
    
    /// <summary>
    /// Start a new page parser for the specified local file.
    /// </summary>
    public ElementParser(string localFile, IAction<ElementParser> onParsed = null) {
      
      Path = localFile;
      _stream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
      _reader = new ByteBuffer(_stream);
      
      _elementStack = new Stack<Element>();
      
      _builder = StringBuilderCache.Get();
      
      // perses the callback on parsed
      OnParsed = onParsed;
      
      // start as though reading an element body
      _section = Section.Body;
      _skipContent = true;
      
    }
    
    /// <summary>
    /// Start parsing.
    /// </summary>
    public ElementParser Run() {
      ManagerUpdate.Control.AddSingle(Parse, Global.BufferSizeLocal);
      return this;
    }
    
    /// <summary>
    /// Complete the parse synchronously.
    /// </summary>
    public ElementParser RunSync() {
      Parse(int.MaxValue);
      return this;
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Main parse iteration method. Reads from the stream and
    /// populates the elements that are read.
    /// </summary>
    protected void Parse(int remainingBuffer) {
      
      int lastPosition = (int)_reader.Position;
      char c;
      
      // while there are more bytes to read this iteration
      while(remainingBuffer >= 0) {
        
        switch(_section) {
          case Section.Body:
            
            if(_skipContent) {
              // read to the openning of the first element
              _reader.ReadString(Chars.LessThan);
              _skipContent = false;
            } else {
              // read the body to the next tag open character
              _element.Content.Add(_reader.ReadString(Chars.LessThan).Trim(Chars.NewLine, Chars.CarriageReturn));
            }
            
            // read the character following the tag openner
            char tagStart = _reader.ReadChar();
            
            // does the tag represent the end of an element?
            if(tagStart == Chars.ForwardSlash) {
              
              string tagStr = _reader.ReadString(Chars.GreaterThan);
              
              // is the tag a match for the current element?
              while(!tagStr.Equals(_element.Tag == Tag.Custom ? _element[Element._customTagKey] : Tags.Html[_element.Tag],
                StringComparison.OrdinalIgnoreCase)) {
                
                // is the tag standalone?
                if(!Tags.Standalone.Contains(_element.Tag)) {
                  
                  // yes, the parse was unsuccessful
                  Error = "Invalid element structure. Missing element close tag for '"+tagStr+"' in file '"+Path+"'.";
                  if(_onParsed != null) _onParsed.Run();
                  StringBuilderCache.Set(_builder);
                  _reader.Close();
                  return;
                  
                }
                
                // are there more elements on the stack?
                if(_elementStack.Pop()) {
                  
                  // assign the next element
                  _element = _elementStack.Current;
                  
                } else {
                  
                  // the parse was unsuccessful
                  Error = "Invalid element structure. Missing element close tag for '"+tagStr+"' element in file '"+Path+"'.";
                  if(_onParsed != null) _onParsed.Run();
                  StringBuilderCache.Set(_builder);
                  _reader.Close();
                  return;
                  
                }
              }
              
              // yes, is there a parent element?
              if(_elementStack.Pop()) {
                
                // yes, set the current element
                _element = _elementStack.Current;
                
              } else {
                
                // the stack is empty, end the read
                if(_onParsed != null) _onParsed.Run();
                StringBuilderCache.Set(_builder);
                _reader.Close();
                return;
                
              }
              
            } else {
              
              // no, does the tag indicate a comment?
              if(tagStart == Chars.Exclamation) {
                
                // yes, construct a new comment element
                Element element;
                c = _reader.ReadChar();
                if(c == Chars.Dash) {
                  element = new ElementComment();
                  var str = tagStart + c + _reader.ReadString(Chars.GreaterThan);
                  element.ContentString = str.Substring(3, str.Length - 3);
                } else if(c == Chars.D) {
                  element = new ElementComment(false);
                  element.ContentString = tagStart + c + _reader.ReadString(Chars.GreaterThan);
                } else {
                  Error = "Unhandled element parsed '"+_reader.ReadString(Chars.GreaterThan)+"' in file '"+Path+"'.";
                  if(_onParsed != null) _onParsed.Run();
                  StringBuilderCache.Set(_builder);
                  _reader.Close();
                  break;
                }
                
                if(_element == null) {
                  _element = element;
                } else {
                  // add the comment to the current element
                  _element.AddChild(element);
                }
                
              } else {
                
                // no, it's a child element - read until a space or the end of tag
                string tagStr = tagStart + _reader.ReadString(out c, Chars.Space, Chars.GreaterThan, Chars.ForwardSlash);
                // persist the current element
                Element parent = _element;
                
                // should the parsed element be popped immediately?
                bool popElement = false;
                
                switch(c) {
                  case Chars.Null:
                    // unexpected end of stream
                    Root = null;
                    Error = "Unexpected end of stream. Expected end of tag '"+_element.Tag+"' was not found in file '"+Path+"'.";
                    if(_onParsed != null) _onParsed.Run();
                    StringBuilderCache.Set(_builder);
                    _reader.Close();
                    return;
                  case Chars.Space:
                    _section = Section.Tag;
                    break;
                  case Chars.ForwardSlash:
                    popElement = true;
                    c = _reader.ReadChar();
                    if(c != Chars.GreaterThan) {
                      // unexpected end of stream
                      Root = null;
                      Error = "Unexpected end of stream. Expected end of tag '"+_element.Tag+"' was not found in file '"+Path+"'.";
                      if(_onParsed != null) _onParsed.Run();
                      StringBuilderCache.Set(_builder);
                      _reader.Close();
                      return;
                    }
                    break;
                }
                
                // get the tag of the element
                var tag = Element.GetTag(tagStr);
                
                // determine action for certain element types
                switch(tag) {
                  case Tag.Table:
                    _element = new ElementTable();
                    break;
                  case Tag.Input:
                  case Tag.TextArea:
                    _element = new ElementInput(tag);
                    break;
                  case Tag.Form:
                    _element = new ElementForm();
                    break;
                  case Tag.Break:
                    // init a new element
                    _element = new Element();
                    _element.Tag = tag;
                    _section = Section.Body;
                    
                    popElement = true;
                    break;
                  case Tag.Custom:
                    // init a new element
                    _element = new Element();
                    _element.SetTag(tagStr);
                    break;
                  case Tag.Style:
                    _element = new Element();
                    _element.Tag = tag;
                    _element.EncodeContent = false;
                    break;
                  default:
                    // init a new element
                    _element = new Element();
                    _element.Tag = tag;
                    break;
                }
                
                // will this element have a parent? no, must be root
                if(parent == null) Root = _element;
                else {
                  // yes, add the element to the parents child collection
                  parent.AddChild(_element);
                  
                  // should the current element be popped?
                  if(popElement) {
                    // yes, assign the parent element
                    _element = parent;
                  } else {
                    // no, push the parent element onto the stack
                    _elementStack.Push(parent);
                  }
                }
                
              }
            }
            
            break;
          case Section.Tag:
            
            c = _reader.ReadChar();
            while(Chars.Whitespace.Contains(c)) {
              
              // has the end of stream been reached?
              if(c == Chars.Null) {
                
                // yes, is the stack empty?
                if(_elementStack.Count == 0) {
                  
                  // yes, the read is complete
                  if(_onParsed != null) _onParsed.Run();
                  StringBuilderCache.Set(_builder);
                  _reader.Close();
                  return;
                }
                
                // unexpected end of stream
                Root = null;
                Error = "Parse failed. Some elements are missing closing tags in file '"+Path+"'.";
                if(_onParsed != null) _onParsed.Run();
                StringBuilderCache.Set(_builder);
                _reader.Close();
                return;
              }
              
              // read the next character
              c = _reader.ReadChar();
            }
            
            if(c == Chars.GreaterThan) {
              _section = Section.Body;
              if(_element.Tag == Tag.Break || _element.Tag == Tag.HorizontalLine) {
                if(_elementStack.Pop()) {
                  _element = _elementStack.Current;
                } else {
                  // the read is complete
                  if(_onParsed != null) _onParsed.Run();
                  StringBuilderCache.Set(_builder);
                  _reader.Close();
                  return;
                }
              }
              break;
            }
            
            string key;
            char end;
            // flag to indicate the tag was ended and the current element should be popped
            bool pop = false;
            
            if(c == Chars.ForwardSlash) {
              c = _reader.ReadChar();
              if(c == Chars.GreaterThan) {
                
                // pop the current element from the stack
                if(_elementStack.Pop()) {
                  _element = _elementStack.Current;
                  _section = Section.Body;
                  break;
                }
                
                if(_onParsed != null) _onParsed.Run();
                StringBuilderCache.Set(_builder);
                _reader.Close();
                return;
              }
              
              // read the next attribute key and potentially value
              key = Chars.ForwardSlash + c + _reader.ReadString(out end, Chars.Equal, Chars.Space, Chars.GreaterThan);
            } else {
              
              // read the next attribute key and potentially value
              key = c + _reader.ReadString(out end, Chars.Equal, Chars.Space, Chars.GreaterThan);
            }
            
            // is the attribute just whitespace?
            if(string.IsNullOrWhiteSpace(key)) {
              
              // yes, has the stream ended?
              if(_reader.Empty) {
                // yes, unexpected end of stream
                Root = null;
                Error = "Parse failed. A tag was unfinished.";
                if(_onParsed != null) _onParsed.Run();
                StringBuilderCache.Set(_builder);
                _reader.Close();
                return;
              }
              
            } else {
              
              // no, has the end of the tag been read?
              if(end == Chars.GreaterThan) {
                
                // yes, was the tag closed?
                if (key[key.Length - 1] == Chars.ForwardSlash) {
                  
                  // yes, adjust the attribute key
                  key = key.Substring(0, key.Length - 1);
                  
                  // flag the current element is to be popped
                  pop = true;
                  
                } else {
                  pop |= _element.Tag == Tag.Break || _element.Tag == Tag.HorizontalLine;
                }
                
                // swap section
                _section = Section.Body;
              }
              
              // is the attribute empty?
              if(key.Length != 0) {
                
                // no, is the attribute a key-value pair?
                if(end == Chars.Equal) {
                  
                  // yes, read the value
                  string value;
                  
                  c = _reader.ReadChar();
                  if(c == Chars.DoubleQuote) {
                    value = _reader.ReadString(Chars.DoubleQuote);
                    while(value.Length > 0 && value[value.Length-1] == Chars.BackSlash) {
                      value = value + Chars.DoubleQuote + _reader.ReadString(Chars.DoubleQuote);
                    }
                  } else {
                    value = c + _reader.ReadString(out end, Chars.GreaterThan, Chars.Space);
                    
                    // has the tag been ended?
                    if(end == Chars.GreaterThan) {
                      if(value[value.Length-1] == Chars.ForwardSlash) {
                        value = value.Substring(0, value.Length-1);
                        // skip the end of tag
                        _reader.ReadString(Chars.GreaterThan);
                        pop = true;
                      }
                      _section = Section.Body;
                    }
                    
                  }
                  
                  // is the parsed attribute the style?
                  if(key.Equals("style")) {
                    _element.Style = new Style(value);
                  } else {
                    // add the attribute key and value
                    _element[key] = value;
                  }
                  
                } else {
                  
                  // no, add the attribute as a flag value
                  if(key.Equals("style")) {
                    _element.Style = new Style();
                  } else {
                    // add the attribute key and value
                    _element[key] = string.Empty;
                  }
                  
                }
              }
              
              // is the element to be popped?
              if(pop) {
                // yes, pop the current element and check for the end of page
                if(_elementStack.Pop()) {
                  _element = _elementStack.Current;
                } else {
                  // the stack is empty, end the parse
                  if(_onParsed != null) _onParsed.Run();
                  StringBuilderCache.Set(_builder);
                  _reader.Close();
                  return;
                }
              }
              
            }
            
            break;
        }
        
        // decrement the remaining buffer
        remainingBuffer -= (int)(_stream.Position - lastPosition);
        
      }
      
      if(_reader.Empty) {
        StringBuilderCache.Set(_builder);
        _reader.Close();
      } else {
        // add another parse
        ManagerUpdate.Control.AddSingle(Parse, Global.BufferSizeLocal);
      }
      
    }
    
    
  }
  
}
