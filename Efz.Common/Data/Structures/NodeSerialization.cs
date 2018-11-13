using System;
using System.Collections.Generic;
using System.Text;

namespace Efz.Data {
  
  /// <summary>
  /// Serialize and parse nodes to and from strings.
  /// </summary>
  public class NodeSerialization {
    
    //-------------------------------------------//
    //------------  Serialization  --------------//
    //-------------------------------------------//
    
    /// <summary>
    /// Serialize a node with the specified writer formatted.
    /// </summary>
    public static bool Serialize(Node node, ByteBuffer writer) {
      bool success = Serialize(node, writer, string.Empty);
      writer.Flush();
      return success;
    }
    
    /// <summary>
    /// Serialize a node with the specified writer formatted.
    /// </summary>
    private static bool Serialize(Node node, ByteBuffer stream, string indent) {
      if(node == null) {
        stream.Write(Chars.Tilde);
        return true;
      }
      
      bool success = true;
      const string indentSpace = "  ";
      
      // does the node have more than one value type?
      bool multivalue = node.Set && node.DictionarySet || node.Set && node.ArraySet || node.DictionarySet && node.Set;
      if(multivalue) {
        // yes, write the open node symbol
        stream.Write(Chars.BracketOpen);
        indent = indent + indentSpace;
      }
      
      // has the Nodes value been set?
      if(node.Set) {
        // yes, is the value to be serialized?
        if(node.Serialize) {
          throw new NotImplementedException("low priority");
        } else {
          // if the string contains any token values
          // or greater than 20 characters (not worth checking)
          if(node.String.Length > 20 ||
             node.String.Contains(
            Chars.Comma,
            Chars.BracketSqClose,
            Chars.BraceOpen,
            Chars.NewLine,
            Chars.Space,
            Chars.Tab)) {
            // write string with quotes
            stream.Write(Chars.DoubleQuote);
            stream.Write(node.String);
            stream.Write(Chars.DoubleQuote);
          } else {
            // write string without quotes
            stream.Write(node.String);
          }
        }
      } else {
        if(!node.ArraySet && !node.DictionarySet) {
          stream.Write(Chars.Tilde);
          return true;
        }
      }
      
      // write node key-value pairs
      if(node.DictionarySet) {
        bool first = true;
        
        if(node.Set) {
          stream.Write(Chars.Comma);
          stream.Write(Chars.NewLine);
          stream.Write(indent);
        }
        
        // start a new dictionary
        stream.Write(Chars.BraceOpen);
        stream.Write(Chars.NewLine);
        stream.Write(indent + indentSpace);
        
        // iterate the nodes dictionary values
        foreach(KeyValuePair<string,Node> entry in node.Dictionary) {
          
          if(first) {
            first = false;
          } else {
            // indent
            stream.Write(Chars.Comma);
            stream.Write(Chars.NewLine);
            stream.Write(indent + indentSpace);
          }
          
          // write key
          stream.Write(entry.Key);
          // colon
          stream.Write(Chars.Colon);
          // write the node value
          Serialize(entry.Value, stream, indent + indentSpace);
        }
        
        stream.Write(Chars.NewLine);
        // indent
        stream.Write(indent);
        stream.Write(Chars.BraceClose);
      }
      
      if(node.ArraySet) {
        
        if(node.DictionarySet || node.Set) {
          stream.Write(Chars.Comma);
          stream.Write(Chars.NewLine);
          stream.Write(indent);
        }
        
        // write the opening of a new node list
        stream.Write(Chars.BracketSqOpen);
        stream.Write(Chars.NewLine);
        stream.Write(indent + indentSpace);
        
        bool first = true;
        for(int i = 0; i < node.Array.Count; ++i) {
          if(first) {
            first = false;
          } else {
            stream.Write(Chars.Comma);
            // indent
            stream.Write(Chars.NewLine);
            stream.Write(indent + indentSpace);
          }
          success = Serialize(node.Array[i], stream, indent + indentSpace);
        }
        // indent
        stream.Write(Chars.NewLine);
        stream.Write(indent);
        stream.Write(Chars.BracketSqClose);
      }
      
      // did the node have more than one value type set?
      if(multivalue) {
        // yes, write the close node symbol
        stream.Write(Chars.NewLine);
        stream.Write(indent.Substring(0, indent.Length-indentSpace.Length));
        stream.Write(Chars.BracketClose);
      }
      
      return success;
    }
    
    /// <summary>
    /// Get a json representation of a node.
    /// </summary>
    public static string SerializeJson(Node node) {
      var writer = StringBuilderCache.Get();
      bool success = SerializeJson(node, writer, string.Empty);
      return StringBuilderCache.SetAndGet(writer);
    }
    
    /// <summary>
    /// Serialize a node with the specified writer formatted.
    /// </summary>
    private static bool SerializeJson(Node node, StringBuilder writer, string indent) {
      if(node == null) {
        writer.Append("\"\"");
        return true;
      }
      
      bool success = true;
      const string indentSpace = "  ";
      
      // does the node have more than one value type?
      bool multivalue = node.Set && node.DictionarySet || node.Set && node.ArraySet || node.DictionarySet && node.Set;
      if(multivalue) {
        // yes, write the open node symbol
        writer.Append(Chars.BracketOpen);
        indent = indent + indentSpace;
      }
      
      // has the Nodes value been set?
      if(node.Set) {
        // yes, is the value to be serialized?
        if(node.Serialize) {
          throw new NotImplementedException("low priority");
        }
        
        // write string value with quotes
        writer.Append(Chars.DoubleQuote);
        writer.Append(node.String);
        writer.Append(Chars.DoubleQuote);
        
      } else {
        
        if(!node.ArraySet && !node.DictionarySet) {
          writer.Append("\"\"");
          return true;
        }
        
      }
      
      // write node key-value pairs
      if(node.DictionarySet) {
        bool first = true;
        
        if(node.Set) {
          writer.Append(Chars.Comma);
          writer.Append(Chars.NewLine);
          writer.Append(indent);
        }
        
        // start a new dictionary
        writer.Append(Chars.BraceOpen);
        writer.Append(Chars.NewLine);
        writer.Append(indent + indentSpace);
        
        // iterate the nodes dictionary values
        foreach(KeyValuePair<string,Node> entry in node.Dictionary) {
          
          if(first) {
            first = false;
          } else {
            // indent
            writer.Append(Chars.Comma);
            writer.Append(Chars.NewLine);
            writer.Append(indent + indentSpace);
          }
          
          // write key
          writer.Append(Chars.DoubleQuote);
          writer.Append(entry.Key);
          writer.Append(Chars.DoubleQuote);
          // colon
          writer.Append(Chars.Colon);
          // write the node value
          SerializeJson(entry.Value, writer, indent + indentSpace);
        }
        
        writer.Append(Chars.NewLine);
        // indent
        writer.Append(indent);
        writer.Append(Chars.BraceClose);
      }
      
      if(node.ArraySet) {
        
        if(node.DictionarySet || node.Set) {
          writer.Append(Chars.Comma);
          writer.Append(Chars.NewLine);
          writer.Append(indent);
        }
        
        // write the opening of a new node list
        writer.Append(Chars.BracketSqOpen);
        writer.Append(Chars.NewLine);
        writer.Append(indent + indentSpace);
        
        bool first = true;
        for(int i = 0; i < node.Array.Count; ++i) {
          if(first) {
            first = false;
          } else {
            writer.Append(Chars.Comma);
            // indent
            writer.Append(Chars.NewLine);
            writer.Append(indent + indentSpace);
          }
          success = SerializeJson(node.Array[i], writer, indent + indentSpace);
        }
        // indent
        writer.Append(Chars.NewLine);
        writer.Append(indent);
        writer.Append(Chars.BracketSqClose);
      }
      
      // did the node have more than one value type set?
      if(multivalue) {
        // yes, write the close node symbol
        writer.Append(Chars.NewLine);
        writer.Append(indent.Substring(0, indent.Length-indentSpace.Length));
        writer.Append(Chars.BracketClose);
      }
      
      return success;
    }
    
    //-------------------------------------------//
    //---------------  Parsing  -----------------//
    //-------------------------------------------//
    
    public Node Node;
    
    //-------------------------------------------//
    
    private char _token;
    private StringBuilder _builder;
    private object _value;
    private bool _serialized;
    
    private readonly ByteBuffer _stream;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Parse a node from a binary reader.
    /// </summary>
    public static Node Parse(ByteBuffer stream) {
      return new NodeSerialization(stream).Node;
    }
    
    /// <summary>
    /// Parse a node from a string.
    /// </summary>
    public static Node Parse(string json) {
      return new NodeSerialization(json).Node;
    }
    
    /// <summary>
    /// Parse a node from the specified reader.
    /// </summary>
    protected NodeSerialization(ByteBuffer stream) {
      _stream = stream;
      _builder = new StringBuilder();
      Node = ParseNode(new Node());
    }
    
    /// <summary>
    /// Parse a node from the specified string.
    /// </summary>
    protected NodeSerialization(string json) {
      var ms = MemoryStreamCache.Get(json.Length);
      var bytes = Encoding.UTF8.GetBytes(json);
      ms.Write(bytes, 0, bytes.Length);
      _stream = new ByteBuffer(ms);
      _builder = new StringBuilder();
      Node = ParseNode(new Node());
      MemoryStreamCache.Set(ms);
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Get the node.
    /// </summary>
    private Node ParseNode(Node node) {
      // get the next token
      TokenValue();
      switch(_token) {
        case Chars.BraceOpen:
          _token = Chars.Null;
          ParseDictionary(node);
          return node;
        case Chars.BracketSqOpen:
          _token = Chars.Null;
          ParseList(node);
          return node;
        case Chars.BracketOpen:
          // parse a multi-value node
          _token = Chars.Null;
          ParseNode(node);
          TokenValue();
          if(_token == Chars.Comma) {
            NextToken();
            if(_token == Chars.BraceOpen) {
              ParseNode(node);
              TokenValue();
              if(_token == Chars.BracketSqOpen) {
                ParseNode(node);
              }
            } else if(_token == Chars.BracketSqOpen) {
              ParseNode(node);
            }
          }
          TokenValue();
          if(_token != Chars.BracketClose) {
            Log.Warning("Unexpected token value '" + _token + "' on Node deserialization.");
          }
          _token = Chars.Null;
          return node;
        case Chars.Null:
        case Chars.Comma:
        case Chars.BraceClose:
        case Chars.BracketSqClose:
          // null node value
          return node;
        default:
          // read the node value
          ParseValue();
          node.Object = _value;
          node.Serialize = _serialized;
          _serialized = false;
          return node;
      }
      //throw new Exception("Node Parser : Unexpected token " + (char)token + " at index " + stream.Stream.Position);
    }
    
    /// <summary>
    /// Parse key-value pairs into the node structure.
    /// </summary>
    private void ParseDictionary(Node node) {
      while(!_stream.Empty) {
        TokenValue();
        switch(_token) {
        case Chars.Comma:
          _token = Chars.Null;
          break;
        case Chars.BraceClose:
          _token = Chars.Null;
          // return
          return;
        case Chars.DoubleQuote:
          {
          // parse the key
          string key = _stream.ReadString(Chars.DoubleQuote);
          // nullify the colon
          _token = Chars.Null;
          // read the node value
          node[key] = ParseNode(new Node(node));
          }
          break;
        default:
          {
          // parse the key
          string key = (char)_token + _stream.ReadString(Chars.Colon);
          // nullify the colon
          _token = Chars.Null;
          // read the node value
          node[key] = ParseNode(new Node(node));
          }
          break;
        }
      }
    }
    
    /// <summary>
    /// Parse a list of nodes.
    /// </summary>
    private void ParseList(Node node) {
      while(!_stream.Empty) {
        TokenValue();
        switch(_token) {
        case Chars.Comma:
          _token = Chars.Null;
          break;
        case Chars.BraceClose:
        case Chars.BracketSqClose:
          _token = Chars.Null;
          return;
        default:
          node.Add(ParseNode(new Node(node)));
          break;
        }
      }
    }
    
    /// <summary>
    /// Parse a value by character until a token is hit.
    /// </summary>
    private void ParseValue() {
      switch(_token) {
        case Chars.Tilde:
          NextToken();
          _value = null;
          return;
        case Chars.StartText:
          throw new NotImplementedException ("low priority");
        case Chars.DoubleQuote:
          // get string literal
          _value = _stream.Empty ? null : _stream.ReadString(Chars.DoubleQuote);
          NextToken();
          return;
        default:
          // clear the last string
          _builder.Length = 0;
          // add the current token
          _builder.Append((char)_token);
          // read text until a token is hit
          _token = _stream.ReadChar();
          while(!_stream.Empty) {
            switch(_token) {
              case Chars.BraceClose:
              case Chars.Comma:
              case Chars.BracketSqClose:
              case Chars.BraceOpen:
                // heed tokens outside quotes
                _value = _builder.ToString();
                return;
              case Chars.NewLine:
              case Chars.Space:
              case Chars.Tab:
                // outside quotes - whitespace indicates end of value
                _value = _builder.ToString();
                _token = Chars.Null;
                return;
              default:
                _builder.Append((char)_token);
                break;
            }
            _token = _stream.ReadChar();
          }
          break;
      }
      throw new Exception("Unexpectedly reached end of stream.");
    }
    
    /// <summary>
    /// Read the next token or value.
    /// </summary>
    private void TokenValue() {
      // if the token already has a value - skip
      if(_token != Chars.Null) return;
      
      try {
        
        // while not-null chars are read from the stream
        while(!_stream.Empty && (_token = _stream.ReadChar()) != Chars.Null) {
          switch(_token) {
            case Chars.BraceOpen:
            case Chars.BraceClose:
            case Chars.BracketSqOpen:
            case Chars.BracketSqClose:
            case Chars.Comma:
            case Chars.Colon:
            case Chars.DoubleQuote:
              // got a token
              return;
            case Chars.Space:
            case Chars.Tab:
            case Chars.NewLine:
            case Chars.CarriageReturn:
              // ignore whitespace
              break;
            default:
              // whoooops hit a value
              return;
          }
        }
      } catch {
        _token = Chars.Null;
        // ignore //
      }
      
    }
    
    /// <summary>
    /// Read the next token ignoring characters that aren't tokens.
    /// </summary>
    private void NextToken() {
      try {
        while(!_stream.Empty && (_token = _stream.ReadChar()) != Chars.Null) {
          switch(_token) {
            case Chars.BraceOpen:
            case Chars.BraceClose:
            case Chars.BracketSqOpen:
            case Chars.BracketSqClose:
            case Chars.BracketOpen:
            case Chars.BracketClose:
            case Chars.Comma:
            case Chars.Colon:
            case Chars.DoubleQuote:
              return;
          }
        }
        _token = Chars.Null;
      } catch {
        _token = Chars.Null;
        // ignore //
      }
    }
    
  }

}