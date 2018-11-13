/*
 * User: Floppynipples
 * Date: 13/11/2016
 * Time: 10:53
 */

using System;
using System.Text;
using System.Collections.Generic;

using Efz.Collections;
using Efz.Threading;

namespace Efz.Web.Display {
  
  /// <summary>
  /// A building block on a web page being constructed.
  /// </summary>
  public partial class Element {
    
    //----------------------------------//
    
    /// <summary>
    /// Flag to indicate the elements body content is to be html encoded.
    /// </summary>
    public bool EncodeContent = true;
    
    /// <summary>
    /// Tag type of the element if set.
    /// </summary>
    public Tag Tag;
    /// <summary>
    /// Get or set the content of the element as a whole.
    /// </summary>
    public virtual string ContentString {
      get {
        if(_content == null) return string.Empty;
        if(_content.Count == 1) return _content[0];
        var builder = StringBuilderCache.Get();
        foreach(var content in _content) builder.Append(content);
        return StringBuilderCache.SetAndGet(builder);
      }
      set {
        if(_content == null) _content = new ArrayRig<string>(1);
        else _content.Clear();
        _content.Add(value);
      }
    }
    
    /// <summary>
    /// Get or set the collection of content strings with indexes interleaved
    /// between the corresponding child elements.
    /// </summary>
    public virtual ArrayRig<string> Content {
      get {
        if(_content == null) _content = new ArrayRig<string>(1);
        return _content;
      }
      set {
        _content = value;
      }
    }
    
    /// <summary>
    /// Get or set the parent element of this one.
    /// </summary>
    public Element Parent {
      get {
        return _parent;
      }
      set {
        if(_parent == value) return;
        if(_parent != null) _parent.RemoveChild(this);
        if(value != null) value.AddChild(this);
      }
    }
    
    /// <summary>
    /// Collection of direct child elements. Manipulate with care.
    /// </summary>
    public ArrayRig<Element> Children;
    
    /// <summary>
    /// Get the number of direct children this element has.
    /// </summary>
    public int ChildCount { get { return Children == null ? 0 : Children.Count; } }
    
    /// <summary>
    /// Get or set the style of the element.
    /// </summary>
    public Style Style {
      get {
        if(_style == null) _style = new Style();
        return _style;
      }
      set {
        _style = value;
      }
    }
    
    /// <summary>
    /// Get or set an attribute by key.
    /// </summary>
    public virtual string this[string key] {
      get {
        if(key.Equals("style", StringComparison.OrdinalIgnoreCase)) {
          return _style == null ? null : _style.ToAttributeValue();
        }
        if(_attributes == null) return null;
        string value;
        _attributes.TryGetValue(key, out value);
        return value;
      }
      set {
        SetAttribute(key, value);
      }
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Attribute key used when the element has a custom tag.
    /// </summary>
    internal const string _customTagKey   = "_custom_tag_";
    
    /// <summary>
    /// Attributes of the Element.
    /// </summary>
    internal Dictionary<string, string> _attributes;
    /// <summary>
    /// Inner parent element.
    /// </summary>
    internal Element _parent;
    
    /// <summary>
    /// Inner content collection.
    /// </summary>
    internal ArrayRig<string> _content;
    
    /// <summary>
    /// Inner style of the element.
    /// </summary>
    internal Style _style;
    
    /// <summary>
    /// Collection of elements by id. Shared by elements in the same tree.
    /// </summary>
    internal Shared<Dictionary<string, ArrayRig<Element>>> _elementsById;
    /// <summary>
    /// Collection of all elements. Shared by elements in the same tree.
    /// </summary>
    internal Shared<HashSet<Element>> _elements;
    
    /// <summary>
    /// Lock used for external access.
    /// </summary>
    internal Lock _lock;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a new element.
    /// </summary>
    public Element() {
      _lock = new Lock();
      var elementsById = new Dictionary<string, ArrayRig<Element>>();
      _elementsById = new Shared<Dictionary<string, ArrayRig<Element>>>(elementsById);
      var elements = new HashSet<Element>();
      elements.Add(this);
      _elements = new Shared<HashSet<Element>>(elements);
    }
    
    /// <summary>
    /// Initialize a new element.
    /// </summary>
    public Element(Tag tag) : this() {
      Tag = tag;
    }
    
    /// <summary>
    /// Clone the element.
    /// </summary>
    public virtual Element Clone() {
      
      // create the element to be the clone
      Element clone = new Element();
      clone.Tag = Tag;
      clone.EncodeContent = EncodeContent;
      if(_content != null) clone._content = new ArrayRig<string>(_content);
      if(_attributes != null) clone._attributes = new Dictionary<string, string>(_attributes);
      if(_style != null) clone._style = new Style(_style);
      
      if(Children != null) {
        // iterate the children of this element
        foreach(var child in Children) clone.AddChild(child.Clone());
      }
      
      // return the clone
      return clone;
    }
    
    /// <summary>
    /// Get elements by id.
    /// </summary>
    public ArrayRig<Element> Find(string id) {
      ArrayRig<Element> elements;
      if(_elementsById.TakeItem().TryGetValue(id, out elements)) {
        _elementsById.Release();
        return elements;
      }
      _elementsById.Release();
      return new ArrayRig<Element>();
    }
    
    /// <summary>
    /// Get children elements that have the specified id.
    /// </summary>
    public ArrayRig<Element> FindChildren(string id) {
      // create the results collection
      ArrayRig<Element> results = new ArrayRig<Element>();
      if(id == null) return results;
      // iterate elements that match the requested id
      foreach(Element found in Find(id)) {
        var element = found;
        while(element != null) {
          if(element == this) {
            results.Add(found);
            break;
          }
          element = element._parent;
        }
      }
      return results;
    }
    
    /// <summary>
    /// Get a single child element by id. Returns this element if the id matches.
    /// Returns 'Null' if the element isn't found.
    /// </summary>
    public Element FindChild(string id) {
      if(id == null) return this;
      // iterate elements that match the requested id
      foreach(Element found in Find(id)) {
        var element = found;
        while(element != null) {
          if(element == this) return found;
          element = element._parent;
        }
      }
      return null;
    }
    
    /// <summary>
    /// Assign a collection of attributes to override the existing attribute collection.
    /// </summary>
    public virtual void SetAttributes(Dictionary<string,string> attributes) {
      string current;
      
      _lock.Take();
      if(_attributes != null && _attributes.TryGetValue("id", out current)) {
        string next;
        ArrayRig<Element> elements;
        if(attributes.TryGetValue("id", out next)) {
          if(current.Equals(next, StringComparison.Ordinal)) {
            _attributes = attributes;
            _lock.Release();
            return;
          }
          
          // assign the new collection of attributes
          _attributes = attributes;
          _lock.Release();
          
          // get the current collection of elements
          elements = _elementsById.TakeItem()[current];
          if(elements.Count == 1) _elementsById.Item.Remove(current);
          else elements.RemoveQuick(this);
          
          if(_elementsById.Item.TryGetValue(next, out elements)) {
            elements.Add(this);
          } else {
            elements = new ArrayRig<Element>();
            elements.Add(this);
            _elementsById.Item.Add(next, elements);
          }
          
          _elementsById.Release();
          return;
          
        }
        
        // assign the new collection of attributes
        _attributes = attributes;
        _lock.Release();
        
        elements = _elementsById.TakeItem()[current];
        if(elements.Count == 1) _elementsById.Item.Remove(current);
        else elements.RemoveQuick(this);
        _elementsById.Release();
        
        return;
      }
      
      // assign the new collection of attributes
      _attributes = attributes;
      _lock.Release();
    }
    
    /// <summary>
    /// Set an attribute.
    /// </summary>
    public virtual void SetAttribute(string key, string value) {
      
      if(key.Equals("style", StringComparison.OrdinalIgnoreCase)) {
        Style = new Style(value);
        return;
      }
      
      // is the id being changed?
      if(key.Equals("id", StringComparison.OrdinalIgnoreCase)) {
        // yes, update the attribute
        ArrayRig<Element> elements;
        string currentId = null;
        
        _lock.Take();
        bool currentIdSet = _attributes != null && _attributes.TryGetValue("id", out currentId);
        _lock.Release();
        
        // do the current attributes contain an id different to the one being assigned?
        if(currentIdSet && !currentId.Equals(value, StringComparison.Ordinal)) {
          
          // yes, replace this element in the elements by id collection
          
          elements = _elementsById.TakeItem()[currentId];
          if(elements.Count == 1) _elementsById.Item.Remove(currentId);
          else elements.RemoveQuick(this);
          
          if(_elementsById.Item.TryGetValue(value, out elements)) {
            elements.Add(this);
          } else {
            elements = new ArrayRig<Element>();
            elements.Add(this);
            _elementsById.Item.Add(value, elements);
          }
          
          _elementsById.Release();
          
        } else {
          
          if(_elementsById.TakeItem().TryGetValue(value, out elements)) {
            elements.Add(this);
          } else {
            elements = new ArrayRig<Element>();
            elements.Add(this);
            _elementsById.Item.Add(value, elements);
          }
          _elementsById.Release();
          
        }
        
      }
      
      _lock.Take();
      if(_attributes == null) _attributes = new Dictionary<string, string>();
      _attributes[key] = value;
      _lock.Release();
    }
    
    /// <summary>
    /// Remove an attribute from an element by key if it exists.
    /// </summary>
    public virtual void RemoveAttribute(string key) {
      _lock.Take();
      if(_attributes != null && _attributes.ContainsKey(key)) {
        
        if(key == "id") {
          string id = _attributes[key];
          ArrayRig<Element> elements = _elementsById.TakeItem()[id];
          if(elements.Count == 1) _elementsById.Item.Remove(id);
          else elements.RemoveQuick(this);
          _elementsById.Release();
        }
        
        _attributes.Remove(key);
      }
      _lock.Release();
    }
    
    /// <summary>
    /// Get the string representation of the element and all children elements.
    /// </summary>
    public string Build() {
      StringBuilder builder = StringBuilderCache.Get();
      Build(builder);
      return StringBuilderCache.SetAndGet(builder);
    }
    
    /// <summary>
    /// Build the css classes for this element and it's children.
    /// </summary>
    public string BuildCss() {
      ArrayRig<Style> styles = new ArrayRig<Style>();
      BuildCss(styles, new Style());
      StringBuilder builder = StringBuilderCache.Get();
      foreach(var style in styles) {
        builder.Append(Chars.NewLine);
        style.ToCss(builder, Chars.Stop);
      }
      builder.Append(Chars.NewLine);
      return StringBuilderCache.SetAndGet(builder);
    }
    
    /// <summary>
    /// Set a tag by string value.
    /// </summary>
    public void SetTag(string tag) {
      if(string.IsNullOrEmpty(tag)) {
        Tag = Tag.None;
        return;
      }
      
      foreach(var entry in Tags.Html) {
        if(entry.Value.Equals(tag, StringComparison.OrdinalIgnoreCase)) {
          Tag = entry.Key;
          return;
        }
      }
      
      _lock.Take();
      
      // initialize the attributes collection if required
      if(_attributes == null) _attributes = new Dictionary<string, string>();
      
      // set a custom tag
      _attributes[_customTagKey] = tag;
      
      _lock.Release();
      
      Tag = Tag.Custom;
    }
    
    /// <summary>
    /// Get the tag representation of the specified string.
    /// </summary>
    public static Tag GetTag(string tag) {
      if(string.IsNullOrEmpty(tag)) {
        return Tag.None;
      }
      
      foreach(var entry in Tags.Html) {
        if(entry.Value.Equals(tag, StringComparison.OrdinalIgnoreCase)) {
          return entry.Key;
        }
      }
      
      return Tag.Custom;
    }
    
    /// <summary>
    /// Add the specified element as a child of this element.
    /// </summary>
    public virtual void AddChild(Element element) {
      _lock.Take();
      if(element == null) Log.Error("Null Element Added.");
      if(Children == null) Children = new ArrayRig<Element>();
      Children.Add(element);
      _lock.Release();
      if(element._parent == null) {
        element._parent = this;
        element.SetElementCollections();
      } else {
        element._parent = this;
        element.ReplaceChildElementCollections();
      }
    }
    
    /// <summary>
    /// Add the specified element as a child of this element in the specified index.
    /// </summary>
    public virtual void InsertChild(Element element, int index = 0) {
      _lock.Take();
      if(element == null) Log.Error("Null Element Added.");
      if(Children == null) Children = new ArrayRig<Element>();
      if(Children.Count <= index) {
        while(Children.Count < index) Children.Add(null);
        Children.Add(element);
      } else {
        Children.Insert(element, index);
      }
      _lock.Release();
      if(element._parent == null) {
        element._parent = this;
        element.SetElementCollections();
      } else {
        element._parent = this;
        element.ReplaceChildElementCollections();
      }
    }
    
    /// <summary>
    /// Add the specified element as a child of this element at the specified index.
    /// </summary>
    public virtual void SetChild(Element element, int index) {
      _lock.Take();
      if(element == null) Log.Error("Null Element Added.");
      if(Children == null) Children = new ArrayRig<Element>();
      if(Children.Count <= index) {
        while(Children.Count < index) Children.Add(null);
        Children.Add(element);
      } else {
        Children[index] = element;
      }
      _lock.Release();
      if(element._parent == null) {
        element._parent = this;
        element.SetElementCollections();
      } else {
        element._parent = this;
        element.ReplaceChildElementCollections();
      }
    }
    
    /// <summary>
    /// Remove the specified element from the collection of children.
    /// </summary>
    public virtual void RemoveChild(Element element) {
      if(Children == null) return;
      element.RemoveElementCollections();
      element._parent = null;
      _lock.Take();
      Children.Remove(element);
      _lock.Release();
    }
    
    /// <summary>
    /// Separate this element and its children from it's parent.
    /// </summary>
    public virtual void Remove() {
      if(_parent != null) _parent.RemoveChild(this);
    }
    
    /// <summary>
    /// Replace the element with the other specified element. If 'Null' this element
    /// and its children are removed.
    /// </summary>
    public virtual void Replace(Element replacement) {
      
      // does this element have a parent?
      if(_parent != null) {
        
        // perform the switch
        _parent.Replace(this, replacement);
        
      } else {
        
        // remove this element from the collections
        this.RemoveElementCollections();
        replacement._parent = null;
        replacement.RemoveElementCollections();
      }
    }
    
    /// <summary>
    /// Replace one child element with another. Woo..
    /// </summary>
    public virtual void Replace(Element childA, Element childB) {
      
      if(Children != null) {
        
        childA._parent = null;
        childA.RemoveElementCollections();
        
        if(childB == null) {
          _lock.Take();
          Children.RemoveQuick(childA);
          _lock.Release();
        } else {
          _lock.Take();
          Children.Replace(childA, childB);
          _lock.Release();
          
          childB._parent = this;
          childB.SetElementCollections();
        }
      }
    }
    
    /// <summary>
    /// Truncate content of the element and its children to the specified length. Elements
    /// will be removed beyond the specified length.
    /// </summary>
    public virtual void Truncate(int length) {
      
      int index = 0;
      if(Children != null) {
        while(index < Children.Count) {
          
          if(_content != null && index < _content.Count) {
            if(_content[index].Length > length) {
              _content[index] = _content[index].Truncate(length, "...");
              ++index;
              while(Children.Count >= index) Children.Pop();
              while(_content.Count > index) _content.Pop();
              return;
            }
            length -= _content[index].Length;
          }
          
          if(Children[index].Truncate(ref length)) {
            ++index;
            while(Children.Count > index) Children.Pop();
            if(_content != null) while(_content.Count > index) _content.Pop();
            return;
          }
          
          ++index;
        }
      }
      
      if(_content != null) {
        while(index < _content.Count) {
          
          if(_content != null && index < _content.Count) {
            if(_content[index].Length > length) {
              _content[index] = _content[index].Truncate(length, "...");
              ++index;
              while(_content.Count > index) _content.Pop();
              return;
            }
            length -= _content[index].Length;
          }
          
          ++index;
        }
      }
      
      return;
    }
    
    /// <summary>
    /// Inner truncate helper.
    /// </summary>
    protected bool Truncate(ref int length) {
      
      int index = 0;
      if(Children != null) {
        while(index < Children.Count) {
          
          if(_content != null && index < _content.Count) {
            if(_content[index].Length > length) {
              _content[index] = _content[index].Truncate(length, "...");
              ++index;
              while(Children.Count >= index) Children.Pop();
              while(_content.Count > index) _content.Pop();
              return true;
            }
            length -= _content[index].Length;
          }
          
          if(Children[index].Truncate(ref length)) {
            ++index;
            while(Children.Count > index) Children.Pop();
            if(_content != null) while(_content.Count > index) _content.Pop();
            return true;
          }
          
        }
      }
      
      if(_content != null) {
        while(index < _content.Count) {
          
          if(_content[index].Length > length) {
            _content[index] = _content[index].Truncate(length, "...");
            while(_content.Count > index) _content.Pop();
            return true;
          }
          length -= _content[index].Length;
          
          ++index;
        }
      }
      
      return false;
    }
    
    /// <summary>
    /// Get the top level element of the tree this element belongs to.
    /// </summary>
    public Element GetRoot() {
      Element element = this;
      while(element._parent != null) element = element._parent;
      return element;
    }
    
    /// <summary>
    /// Write only the content of the element.
    /// </summary>
    public virtual void BuildContent(StringBuilder builder) {
      if(Children == null || Children.Count == 0) {
        if(_content == null || _content.Count == 0) return;
        // append the encoded body content
        foreach(var content in _content) {
          builder.Append(System.Net.WebUtility.HtmlEncode(content));
        }
      } else if(_content == null || _content.Count == 0) {
        // build and append the children elements
        for (int i = 0; i < Children.Count; ++i) Children[i].Build(builder);
      } else {
        int index = -1;
        if(_content.Count > Children.Count) {
          // build and append the children elements
          while(++index < Children.Count) {
            builder.Append(_content[index]);
            Children[index].Build(builder);
          }
          while(index < _content.Count) {
            builder.Append(_content[index]);
            ++index;
          }
        } else {
          // build and append the children elements
          while(++index < _content.Count) {
            builder.Append(_content[index]);
            Children[index].Build(builder);
          }
          while(index < Children.Count) {
            Children[index].Build(builder);
            ++index;
          }
        }
      }
    }
    
    /// <summary>
    /// Write the open tag symbol.
    /// </summary>
    public virtual void BuildTagOpen(StringBuilder builder) {
      if(Tag == Tag.None) return;
      
      // open the element tag
      builder.Append(Chars.LessThan);
      // write the tag type
      if(Tag == Tag.Custom) {
        builder.Append(_attributes[_customTagKey]);
      } else {
        builder.Append(Tags.Html[Tag]);
      }
      
      // are any attributes set?
      if(_attributes != null && _attributes.Count != 0) {
        // yes, write all the attributes
        builder.Append(Chars.Space);
        bool first = true;
        if(Tag == Tag.Custom) {
          string customTag = _attributes[_customTagKey];
          _attributes.Remove(_customTagKey);
          
          foreach(KeyValuePair<string,string> entry in _attributes) {
            if(first) first = false;
            else builder.Append(Chars.Space);
            
            builder.Append(entry.Key);
            if(string.IsNullOrEmpty(entry.Value)) continue;
            builder.Append(Chars.Equal);
            builder.Append(Chars.DoubleQuote);
            builder.Append(entry.Value);
            builder.Append(Chars.DoubleQuote);
          }
          
          _attributes[_customTagKey] = customTag;
        } else {
          foreach(KeyValuePair<string,string> entry in _attributes) {
            if(first) first = false;
            else builder.Append(Chars.Space);
            
            builder.Append(entry.Key);
            if(string.IsNullOrEmpty(entry.Value)) continue;
            builder.Append(Chars.Equal);
            builder.Append(Chars.DoubleQuote);
            builder.Append(entry.Value);
            builder.Append(Chars.DoubleQuote);
          }
        }
        
        // has this element been assigned a style?
        if(_style != null) {
          // yes, does the style have a class?
          if(_style.Class == null) {
            // no, append the style in-line
            if(first) first = false;
            else builder.Append(Chars.Space);
            
            builder.Append("style=\"");
            builder.Append(_style.ToAttributeValue());
            builder.Append(Chars.DoubleQuote);
          } else {
            // yes, add the style as a class reference
            if(first) first = false;
            else builder.Append(Chars.Space);
            
            builder.Append("class=\"");
            builder.Append(_style.Class);
            builder.Append(Chars.DoubleQuote);
          }
        }
      } else if(_style != null) {
        // yes, does the style have a class?
        builder.Append(Chars.Space);
        if(_style.Class == null) {
          // no, append the style in-line
          builder.Append("style=\"");
          builder.Append(_style.ToAttributeValue());
          builder.Append(Chars.DoubleQuote);
        } else {
          // yes, add the style as a class reference
          builder.Append("class=\"");
          builder.Append(_style.Class);
          builder.Append(Chars.DoubleQuote);
        }
      }
      
      // end the html tag
      builder.Append(Chars.GreaterThan);
      
    }
    
    /// <summary>
    /// Write the close tag symbol.
    /// </summary>
    public virtual void BuildTagClose(StringBuilder builder) {
      
      // open the end tag
      builder.Append(Chars.LessThan);
      
      // add a forward slash
      builder.Append(Chars.ForwardSlash);
      // if the tag isn't set
      if(Tag == Tag.Custom) builder.Append(_attributes[_customTagKey]);
      else builder.Append(Tags.Html[Tag]);
      
      // close the element
      builder.Append(Chars.GreaterThan);
    }
    
    /// <summary>
    /// Get a string representation of the element.
    /// </summary>
    public override string ToString() {
      var count = _elements.TakeItem().Count;
      _elements.Release();
      string id;
      if(_attributes != null && _attributes.TryGetValue("id", out id)) {
        return "[Element tag="+Tag+" id=" + id + " tree count="+count+" children="+(Children == null ? 0 : Children.Count)+"]";
      }
      return "[Element tag="+Tag+" tree count="+count+" children="+(Children == null ? 0 : Children.Count)+"]";
    }
    
    /// <summary>
    /// Append the element to the specified string builder.
    /// </summary>
    public virtual void Build(StringBuilder builder) {
      
      if(Tag == Tag.None) {
        BuildContent(builder);
        return;
      }
      
      // open the element tag
      builder.Append(Chars.LessThan);
      
      // write the tag type
      if(Tag == Tag.Custom) {
        builder.Append(_attributes[_customTagKey]);
      } else {
        builder.Append(Tags.Html[Tag]);
      }
      
      // are any attributes set?
      if(_attributes != null && _attributes.Count != 0) {
        // yes, write all the attributes
        builder.Append(Chars.Space);
        bool first = true;
        if(Tag == Tag.Custom) {
          string customTag = _attributes[_customTagKey];
          _attributes.Remove(_customTagKey);
          
          // iterate the attributes
          foreach(KeyValuePair<string,string> entry in _attributes) {
            if(first) first = false;
            else builder.Append(Chars.Space);
            
            builder.Append(entry.Key);
            if(string.IsNullOrEmpty(entry.Value)) continue;
            builder.Append(Chars.Equal);
            builder.Append(Chars.DoubleQuote);
            builder.Append(entry.Value);
            builder.Append(Chars.DoubleQuote);
          }
          
          _attributes[_customTagKey] = customTag;
        } else {
          foreach(KeyValuePair<string,string> entry in _attributes) {
            if(first) first = false;
            else builder.Append(Chars.Space);
            
            builder.Append(entry.Key);
            if(string.IsNullOrEmpty(entry.Value)) continue;
            builder.Append(Chars.Equal);
            builder.Append(Chars.DoubleQuote);
            builder.Append(entry.Value);
            builder.Append(Chars.DoubleQuote);
          }
        }
        
        // has this element been assigned a style?
        if(_style != null) {
          // yes, does the style have a class?
          if(_style.Class == null) {
            // no, append the style in-line
            if(first) first = false;
            else builder.Append(Chars.Space);
            
            builder.Append("style=\"");
            builder.Append(_style.ToAttributeValue());
            builder.Append(Chars.DoubleQuote);
          } else {
            // yes, add the style as a class reference
            if(first) first = false;
            else builder.Append(Chars.Space);
            
            builder.Append("class=\"");
            builder.Append(_style.Class);
            builder.Append(Chars.DoubleQuote);
          }
        }
      } else if(_style != null) {
        // yes, does the style have a class?
        builder.Append(Chars.Space);
        if(_style.Class == null) {
          // no, append the style in-line
          builder.Append("style=\"");
          builder.Append(_style.ToAttributeValue());
          builder.Append(Chars.DoubleQuote);
        } else {
          // yes, add the style as a class reference
          builder.Append("class=\"");
          builder.Append(_style.Class);
          builder.Append(Chars.DoubleQuote);
        }
      }
      
      // any children assigned?
      if(Children == null || Children.Count == 0) {
        if(_content == null || _content.Count == 0) {
          
          // is the tag potentially standalone?
          if(!Tags.Standalone.Contains(Tag)) {
            // no, build the close tag
            builder.Append(Chars.GreaterThan);
            builder.Append(Chars.LessThan);
            builder.Append(Chars.ForwardSlash);
            if(Tag == Tag.Custom) {
              builder.Append(_attributes[_customTagKey]);
            } else {
              builder.Append(Tags.Html[Tag]);
            }
          }
          
        } else {
          
          // end the open tag
          builder.Append(Chars.GreaterThan);
          
          if(EncodeContent) {
            // append the encoded body content
            foreach(var content in _content) {
              builder.Append(System.Net.WebUtility.HtmlEncode(content));
            }
          } else {
            // append the encoded body content
            foreach(var content in _content) {
              builder.Append(content);
            }
          }
          
          // open the end tag
          builder.Append(Chars.LessThan);
          builder.Append(Chars.ForwardSlash);
          if(Tag == Tag.Custom) {
            builder.Append(_attributes[_customTagKey]);
          } else {
            builder.Append(Tags.Html[Tag]);
          }
        }
      } else if(_content == null || _content.Count == 0) {
        // end the open tag
        builder.Append(Chars.GreaterThan);
        
        // build and append the children elements
        for (int i = 0; i < Children.Count; ++i) {
          Children[i].Build(builder);
        }
        
        // open the end tag
        builder.Append(Chars.LessThan);
        builder.Append(Chars.ForwardSlash);
        if(Tag == Tag.Custom) {
          builder.Append(_attributes[_customTagKey]);
        } else {
          builder.Append(Tags.Html[Tag]);
        }
      } else {
        // end the open tag
        builder.Append(Chars.GreaterThan);
        
        int index = -1;
        if(_content.Count > Children.Count) {
          // build and append the children elements
          if(EncodeContent) {
            while(++index < Children.Count) {
              builder.Append(System.Net.WebUtility.HtmlEncode(_content[index]));
              Children[index].Build(builder);
            }
            while(index < _content.Count) {
              builder.Append(System.Net.WebUtility.HtmlEncode(_content[index]));
              ++index;
            }
          } else {
            while(++index < Children.Count) {
              builder.Append(_content[index]);
              Children[index].Build(builder);
            }
            while(index < _content.Count) {
              builder.Append(_content[index]);
              ++index;
            }
          }
        } else {
          // build and append the children elements
          if(EncodeContent) {
            while(++index < _content.Count) {
              builder.Append(System.Net.WebUtility.HtmlEncode(_content[index]));
              Children[index].Build(builder);
            }
          } else {
            while(++index < _content.Count) {
              builder.Append(_content[index]);
              Children[index].Build(builder);
            }
          }
          while(index < Children.Count) {
            Children[index].Build(builder);
            ++index;
          }
        }
        
        // open the end tag
        builder.Append(Chars.LessThan);
        builder.Append(Chars.ForwardSlash);
        if(Tag == Tag.Custom) {
          builder.Append(_attributes[_customTagKey]);
        } else {
          builder.Append(Tags.Html[Tag]);
        }
      }
      
      // close the element
      builder.Append(Chars.GreaterThan);
      
    }
    
    /// <summary>
    /// Compile and append the necessary css classes for the element tree.
    /// </summary>
    public virtual void BuildCss(ArrayRig<Style> styles, Style style) {
      
      // has the style been assigned?
      if(_style != null && !_style.Empty) {
        // yes, get the index of the style if it's already been assigned
        int index = styles.IndexOf(_style);
        // does the style already exist?
        if(index == -1) {
          // no, get the differences between this elements style and it's
          // parents style
          style = new Style(style);
          Style differences = style.AddAndGetDifferences(_style);
          
          // yes, get the index of the difference style in the current collection
          // if it's been added already
          index = styles.IndexOf(differences);
          
          // has the style to be assigned been added?
          if(index == -1) {
            // no, add the differences style
            styles.Add(differences);
            _style.Class = differences.Class = (styles.Count - 1).AsLetters();
          } else {
            // yes, set the current styles class
            _style.Class = index.AsLetters();
          }
        } else {
          // yes, assign this elements style class
          _style.Class = index.AsLetters();
        }
      }
      
      // does this element have children?
      if(Children != null && Children.Count > 0) {
        // yes, build the css of this elements children
        foreach(var child in Children) {
          child.BuildCss(styles, style);
        }
      }
    }
    
    /// <summary>
    /// Combine elements. If either element has a Div tag, the other element is added as a
    /// child. If neither element has a Div tag, a new element with a div tag is created
    /// and both elements are added as children.
    /// Returns the resulting container element.
    /// </summary>
    public static Element operator +(Element elementA, Element elementB) {
      // does elementA have a Div tag?
      if(elementA.Tag == Tag.Division) {
        elementB.Parent = elementA;
        return elementA;
      }
      if(elementB.Tag == Tag.Division) {
        elementA.Parent = elementB;
        return elementB;
      }
      var container = new Element(Tag.Division);
      container.AddChild(elementA);
      container.AddChild(elementB);
      return container;
    }
    
    /// <summary>
    /// If elementB is a child of elementA, it is removed. Returns elementA.
    /// </summary>
    public static Element operator -(Element elementA, Element elementB) {
      elementA.RemoveChild(elementB);
      return elementA;
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Set the this element and its childrens collections.
    /// </summary>
    protected virtual void SetElementCollections() {
      
      if(_parent != null) {
        _elementsById = _parent._elementsById;
        _elements = _parent._elements;
      }
      
      _elements.TakeItem().Add(this);
      _elements.Release();
      
      string id = this["id"];
      if(id != null) {
        ArrayRig<Element> elements;
        if(_elementsById.TakeItem().TryGetValue(id, out elements)) {
          elements.Add(this);
        } else {
          elements = new ArrayRig<Element>();
          elements.Add(this);
          _elementsById.Item.Add(id, elements);
        }
        _elementsById.Release();
      }
      
      // has the child collection been specified?
      if(Children != null) {
        // update the children collections
        for(int i = Children.Count-1; i >= 0; --i) {
          // update the child element collections
          Children[i].SetElementCollections();
        }
      }
    }
    
    /// <summary>
    /// Removes this element from the current assigned collections and adds
    /// it the new parent collection.
    /// </summary>
    protected void ReplaceChildElementCollections() {
      
      _lock.Take();
      // try get the element id
      string id = this["id"];
      _lock.Release();
      
      // has the id been assigned?
      if(id != null) {
        
        // yes, does the element exist in the collection?
        ArrayRig<Element> existing = _elementsById.TakeItem()[id];
        
        if(existing.Count == 1) _elementsById.Item.Remove(id);
        else existing.Remove(this);
        
        _elementsById.Release();
        
        // reference parents collection
        _elementsById = _parent._elementsById;
        
        if(_elementsById.TakeItem().TryGetValue(id, out existing)) {
          existing.Add(this);
        } else {
          existing = new ArrayRig<Element>();
          existing.Add(this);
          _elementsById.Item.Add(id, existing);
        }
        
        _elementsById.Release();
        
      } else {
        
        // reference parents collection
        _elementsById = _parent._elementsById;
        
      }
      
      // remove from the hash set collection
      _elements.TakeItem().Remove(this);
      _elements.Release();
      
      // reference parents collection
      _elements = _parent._elements;
      
      _elements.TakeItem().Add(this);
      _elements.Release();
      
      if(Children != null) {
        // iterate children and remove them from the collections
        for(int i = Children.Count-1; i >= 0; --i) {
          Children[i].ReplaceChildElementCollections();
        }
      }
      
    }
    
    /// <summary>
    /// Remove this element from assigned element collections.
    /// </summary>
    protected virtual void RemoveElementCollections() {
      ArrayRig<Element> existing;
      _lock.Take();
      // try get the element id
      string id = this["id"];
      _lock.Release();
      
      // has the id been assigned?
      if(id != null) {
        
        // yes, does the element exist in the collection?
        if(_elementsById.TakeItem().TryGetValue(id, out existing)) {
          if(existing.Count == 1) _elementsById.Item.Remove(id);
          else existing.RemoveQuick(this);
        }
        _elementsById.Release();
      }
      
      // remove from the hash set collection
      _elements.TakeItem().Remove(this);
      _elements.Release();
      
      _lock.Take();
      _elements = new Shared<HashSet<Element>>(new HashSet<Element>());
      _elementsById = new Shared<Dictionary<string, ArrayRig<Element>>>(new Dictionary<string, ArrayRig<Element>>());
      _lock.Release();
      
      // has the id been assigned?
      if(id != null) {
        // yes, add to the elements by id collection
        if(_elementsById.TakeItem().TryGetValue(id, out existing)) {
          existing.Add(this);
        } else {
          existing = new ArrayRig<Element>();
          existing.Add(this);
          _elementsById.Item.Add(id, existing);
        }
        _elementsById.Release();
      }
      
      if(Children != null) {
        // iterate children and remove them from the current collections and update
        for(int i = Children.Count-1; i >= 0; --i) {
          Children[i].ReplaceChildElementCollections();
        }
      }
    }
    
  }
  
}
