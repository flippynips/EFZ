/*
 * User: FloppyNipples
 * Date: 26/02/2017
 * Time: 19:09
 */
using System;
using System.Collections.Generic;
using System.Text;
using Efz.Collections;

namespace Efz.Web.Display {
  
  /// <summary>
  /// Style management for an element.
  /// </summary>
  public class Style : IEquatable<Style> {
    
    //----------------------------------//
    
    /// <summary>
    /// Class string that identifies this style and allows elements to reference
    /// it.
    /// </summary>
    public string Class;
    
    /// <summary>
    /// Get a flag indicating the style is empty and contains no values.
    /// </summary>
    public bool Empty {
      get {
        return (_params == null || _params.Count == 0) &&
          _properties.Count == 0 &&
          (_children == null || _children.Count == 0);
      }
    }
    
    /// <summary>
    /// Get or set a style parameter.
    /// </summary>
    public string this[StyleKey key] {
      get {
        string value;
        if(_properties.TryGetValue(key, out value)) return value;
        return null;
      }
      set {
        _properties[key] = value;
      }
    }
    
    /// <summary>
    /// Get or set a style parameter.
    /// </summary>
    public string this[string key] {
      get {
        string value;
        if(StyleKeys.KeySet.Contains(key.ToLowercase().GetHashCode())) {
          StyleKey styleKey = StyleKeys.GetKey(key);
          if(styleKey != StyleKey.None) {
            _properties.TryGetValue(styleKey, out value);
            return value;
          }
        }
        if(_params == null) return null;
        _params.TryGetValue(key, out value);
        return value;
      }
      set {
        if(StyleKeys.KeySet.Contains(key.ToLowercase().GetHashCode())) {
          StyleKey styleKey = StyleKeys.GetKey(key);
          if(styleKey != StyleKey.None) {
            _properties[styleKey] = value;
            return;
          }
        }
        if(_params == null) _params = new Dictionary<string, string>();
        _params[key] = value;
      }
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Custom style parameters.
    /// </summary>
    protected Dictionary<string, string> _params;
    /// <summary>
    /// Style parameters.
    /// </summary>
    protected Dictionary<StyleKey, string> _properties;
    /// <summary>
    /// Children styles.
    /// </summary>
    protected ArrayRig<Style> _children;
    
    //----------------------------------//
    
    /// <summary>
    /// Create a new, empty style.
    /// </summary>
    public Style() {
      _properties = new Dictionary<StyleKey, string>();
    }
    
    /// <summary>
    /// Create a style from the attribute string representation.
    /// </summary>
    public Style(string style) : this() {
      var values = style.Split(Chars.SemiColon);
      foreach(var value in values) {
        if(value.Length == 0) continue;
        var keyValue = value.TrimSpace().Split(Chars.Colon);
        if(keyValue.Length == 2) {
          this[keyValue[0]] = keyValue[1];
        } else {
          Log.Warning("Incorrect style specification parsed '"+value+"'.");
        }
      }
    }
    
    /// <summary>
    /// Initialize a style with the properties of the specified style,
    /// effectively cloning the style.
    /// </summary>
    public Style(Style style) {
      if(style._params != null) _params = new Dictionary<string, string>(style._params);
      _properties = new Dictionary<StyleKey, string>(style._properties);
      if(style._children != null) _children = new ArrayRig<Style>(style._children);
      Class = style.Class;
    }
    
    /// <summary>
    /// Add a child style.
    /// </summary>
    public void Add(Style child) {
      if(_children == null) _children = new ArrayRig<Style>();
      _children.Add(child);
    }
    
    /// <summary>
    /// Remove the specified child style.
    /// </summary>
    public void Remove(Style child) {
      if(_children != null) {
        _children.Remove(child);
        if(_children.Count == 0) _children = null;
      }
    }
    
    /// <summary>
    /// Add the specified styles properties to this instance and return a style
    /// that is a compilation of the differences between this and the specified
    /// style.
    /// </summary>
    public Style AddAndGetDifferences(Style other) {
      Style differences = new Style();
      // iterate the style entries
      foreach(var entry in other._properties) {
        string value;
        if(_properties.TryGetValue(entry.Key, out value)) {
          if(!StyleKeys.Inherited.Contains(entry.Key) || !entry.Value.Equals(value, StringComparison.Ordinal)) {
            differences[entry.Key] = entry.Value;
            _properties[entry.Key] = entry.Value;
          }
        } else {
          differences[entry.Key] = entry.Value;
          _properties[entry.Key] = entry.Value;
        }
      }
      if(other._params != null) {
        _params = new Dictionary<string, string>();
        // iterate the custom parameters
        foreach(var entry in other._params) {
          string value;
          if(_params.TryGetValue(entry.Key, out value)) {
            if(!entry.Value.Equals(value, StringComparison.Ordinal)) {
              differences[entry.Key] = entry.Value;
              _params[entry.Key] = entry.Value;
            }
          } else {
            differences[entry.Key] = entry.Value;
            _params[entry.Key] = entry.Value;
          }
        }
      }
      
      // iterate children
      if(other._children != null) {
        if(_children == null) {
          for(int i = 0; i < other._children.Count; ++i) {
            differences.Add(new Style(other._children[i]));
          }
        } else {
          for(int i = 0; i < other._children.Count; ++i) {
            if(_children.Contains(other._children[i])) continue;
            differences.Add(new Style(other._children[i]));
          }
        }
      }
      
      return differences;
    }
    
    /// <summary>
    /// Get the style as a complete attribute string.
    /// </summary>
    public string ToAttributeValue() {
      string result = null;
      bool first = true;
      if(_params != null) {
        foreach(var param in _params) {
          if(first) {
            first = false;
            result = String.Join(Chars.Colon.ToString(), param.Key, param.Value);
          } else {
            result = String.Join(Chars.SemiColon.ToString(), result,
              String.Join(Chars.Colon.ToString(), param.Key, param.Value));
          }
        }
      }
      foreach(var param in _properties) {
        if(first) {
          first = false;
          result = String.Join(Chars.Colon.ToString(), StyleKeys.Map[param.Key], param.Value);
        } else {
          result = String.Join(Chars.SemiColon.ToString(), result,
            String.Join(Chars.Colon.ToString(), StyleKeys.Map[param.Key], param.Value));
        }
      }
      return result;
    }
    
    /// <summary>
    /// Get the style as a CSS class.
    /// </summary>
    public string ToCss(StringBuilder builder, char prefix) {
      if(prefix != Chars.Null) builder.Append(prefix);
      
      builder.Append(Class);
      builder.Append(Chars.BraceOpen);
      bool first = true;
      if(_params != null) {
        foreach(var param in _params) {
          if(first) {
            first = false;
            builder.Append(param.Key);
            builder.Append(Chars.Colon);
            builder.Append(param.Value);
          } else {
            builder.Append(Chars.SemiColon);
            builder.Append(param.Key);
            builder.Append(Chars.Colon);
            builder.Append(param.Value);
          }
        }
      }
      foreach(var param in _properties) {
        if(first) {
          first = false;
          builder.Append(StyleKeys.Map[param.Key]);
          builder.Append(Chars.Colon);
          builder.Append(param.Value);
        } else {
          builder.Append(Chars.SemiColon);
          builder.Append(StyleKeys.Map[param.Key]);
          builder.Append(Chars.Colon);
          builder.Append(param.Value);
        }
      }
      builder.Append(Chars.SemiColon);
      builder.Append(Chars.BraceClose);
      
      if(_children != null) {
        foreach(var style in _children) {
          builder.Append(Chars.NewLine);
          if(prefix != Chars.Null) builder.Append(prefix);
          builder.Append(Class);
          style.ToCss(builder);
        }
      }
      
      return StringBuilderCache.SetAndGet(builder);
    }
    
    /// <summary>
    /// Get the style as a CSS class.
    /// </summary>
    public string ToCss(StringBuilder builder) {
      
      builder.Append(Class);
      builder.Append(Chars.BraceOpen);
      bool first = true;
      if(_params != null) {
        foreach(var param in _params) {
          if(first) {
            first = false;
            builder.Append(param.Key);
            builder.Append(Chars.Colon);
            builder.Append(param.Value);
          } else {
            builder.Append(Chars.SemiColon);
            builder.Append(param.Key);
            builder.Append(Chars.Colon);
            builder.Append(param.Value);
          }
        }
      }
      foreach(var param in _properties) {
        if(first) {
          first = false;
          builder.Append(StyleKeys.Map[param.Key]);
          builder.Append(Chars.Colon);
          builder.Append(param.Value);
        } else {
          builder.Append(Chars.SemiColon);
          builder.Append(StyleKeys.Map[param.Key]);
          builder.Append(Chars.Colon);
          builder.Append(param.Value);
        }
      }
      builder.Append(Chars.SemiColon);
      builder.Append(Chars.BraceClose);
      
      if(_children != null) {
        foreach(var style in _children) {
          builder.Append(Chars.NewLine);
          builder.Append(Class);
          style.ToCss(builder);
        }
      }
      
      return StringBuilderCache.SetAndGet(builder);
    }
    
    /// <summary>
    /// Equality check with another style.
    /// </summary>
    public bool Equals(Style other) {
      if(_properties.Count != other._properties.Count ||
        (other._params == null ? _params != null :
        _params == null || _params.Count != other._params.Count)) {
        return false;
      }
      string value;
      HashSet<StyleKey> keys = new HashSet<StyleKey>();
      foreach(var param in _properties) {
        if(!other._properties.TryGetValue(param.Key, out value) ||
           !param.Value.Equals(value, StringComparison.Ordinal)) {
          return false;
        }
        keys.Add(param.Key);
      }
      foreach(var param in other._properties) {
        if(keys.Contains(param.Key)) continue;
        if(!_properties.TryGetValue(param.Key, out value) ||
           !param.Value.Equals(value, StringComparison.Ordinal)) {
          return false;
        }
      }
      if(_params == null) {
        if(other._params != null) return false;
      } else {
        if(other._params == null) return false;
        
        HashSet<string> paramKeys = new HashSet<string>();
        foreach(var param in _params) {
          if(!other._params.TryGetValue(param.Key, out value) || !param.Value.Equals(value, StringComparison.Ordinal)) {
            return false;
          }
          paramKeys.Add(param.Key);
        }
        foreach(var param in other._params) {
          if(paramKeys.Contains(param.Key)) continue;
          if(!_params.TryGetValue(param.Key, out value) ||
          !param.Value.Equals(value, StringComparison.Ordinal)) {
            return false;
          }
        }
      }
      if(_children == null) {
        if(other._children != null) return false;
      } else {
        if(other._children == null) return false;
        foreach(var child in _children) {
          if(!other._children.Contains(child)) return false;
        }
      }
      return true;
    }
    
    public override bool Equals(object obj) {
      Style other = obj as Style;
      if (other == null) return false;
      return this.Equals(other);
    }
    
    public override int GetHashCode() {
      return (Class == null ? 0 : Class.GetHashCode()) ^
        _properties.GetHashCode() ^
        (_params == null ? 0 : _params.GetHashCode()) ^
        (_children == null ? 0 : _children.GetHashCode());
    }
    
    public static bool operator ==(Style lhs, Style rhs) {
      if(ReferenceEquals(lhs, rhs)) return true;
      if(ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null)) return false;
      return lhs.Equals(rhs);
    }
    
    public static bool operator !=(Style lhs, Style rhs) {
      if(ReferenceEquals(lhs, rhs)) return false;
      if(ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null)) return true;
      return !lhs.Equals(rhs);
    }
    
    //----------------------------------//
    
  }
  
  
}
