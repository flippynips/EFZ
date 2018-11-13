using System;
using System.Collections.Generic;

using Efz.Collections;

namespace Efz.Data {
  
  /// <summary>
  /// Generic heirarchy data store.
  /// </summary>
  public class Node : IEnumerable<Node>, IEquatable<string>, IEquatable<Dictionary<string, Node>> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Get the value as an object.
    /// </summary>
    public object Object { get { return _value; } set { this._value = value; Set = this._value != null; } }
    /// <summary>
    /// Get a string representation of the node value.
    /// </summary>
    public string String { get { return Set ? _value.ToString() : string.Empty; } set { this._value = value; Set = this._value != null; } }
    /// <summary>
    /// Get a bool representation of the node value.
    /// </summary>
    public bool Bool { get { return Set && this.String.ToBool(); } set { this._value = value ? "true" : "false"; Set = true; } }
    /// <summary>
    /// Get a short representation of the node value.
    /// </summary>
    public short Int16 { get { return this.String.ToInt16(); } set { this._value = value; Set = true; } }
    /// <summary>
    /// Get a ushort representation of the node value.
    /// </summary>
    public ushort UInt16 { get { return this.String.ToUInt16(); } set { this._value = value; Set = true; } }
    /// <summary>
    /// Get a int representation of the node value.
    /// </summary>
    public int Int32 { get { return this.String.ToInt32(); } set { this._value = value; Set = true; } }
    /// <summary>
    /// Get a uint representation of the node value.
    /// </summary>
    public uint UInt32 { get { return this.String.ToUInt32(); } set { this._value = value; Set = true; } }
    /// <summary>
    /// Get a long representation of the node value.
    /// </summary>
    public long Int64 { get { return this.String.ToInt64(); } set { this._value = value; Set = true; } }
    /// <summary>
    /// Get a ulong representation of the node value.
    /// </summary>
    public ulong UInt64 { get { return this.String.ToUInt64(); } set { this._value = value; Set = true; } }
    /// <summary>
    /// Get a float representation of the node value.
    /// </summary>
    public float Float { get { return this.String.ToFloat(); } set { this._value = value; Set = true; } }
    /// <summary>
    /// Get a double representation of the node value.
    /// </summary>
    public double Double { get { return this.String.ToDouble(); } set { this._value = value; Set = true; } }
    /// <summary>
    /// Get a byte array representation of the node value.
    /// </summary>
    public byte[] Bytes { get { return Convert.FromBase64String(String); } set { this._value = Convert.ToBase64String(value); Set = true; } }
    
    /// <summary>
    /// Get the key or index of this node from its parent.
    /// </summary>
    public string Key {
      get {
        if(_parent == null) return string.Empty;
        if(_parent._dictionarySet) {
          foreach(KeyValuePair<string, Node> entry in _parent._dictionary) {
            if(entry.Value == this) {
              return entry.Key;
            }
          }
        }
        if(_parent._arraySet) {
          for(int i = 0; i < _parent._array.Count; ++i) {
            if(_parent.Array[i] == this) {
              return i.ToString();
            }
          }
        }
        return string.Empty;
      }
    }
    
    /// <summary>
    /// Returns the number of child nodes referenable with indices.
    /// </summary>
    public bool ArraySet { get { return _arraySet && _array.Count > 0; } }
    /// <summary>
    /// Returns the number of child nodes referenable with keys.
    /// </summary>
    public bool DictionarySet { get { return _dictionarySet && _dictionary.Count > 0; } }
    
    /// <summary>
    /// Returns if the value is not null.
    /// </summary>
    public bool Set { get; protected set; }
    
    /// <summary>
    /// The parent of this node.
    /// </summary>
    public Node Parent {
      get { return _parent; }
    }
    
    /// <summary>
    /// This nodes child node dictionary.
    /// </summary>
    public Dictionary<string, Node> Dictionary {
      get {
        if(!_dictionarySet) {
          _dictionary = new Dictionary<string, Node>();
          _dictionarySet = true;
        }
        return _dictionary;
      }
      protected set {
        _dictionary = value;
      }
    }
    /// <summary>
    /// This nodes child node array.
    /// </summary>
    public ArrayRig<Node> Array {
      get {
        if(!_arraySet) {
          _array = new ArrayRig<Node>();
          _arraySet = true;
        }
        return _array;
      }
      protected set {
        _array = value;
      }
    }
    
    /// <summary>
    /// Get whether this nodes value should be serialized when written and deserialized
    /// when read. When the value object cannot be serialized to a string.
    /// </summary>
    public bool Serialize { get; set; }
    
    //-------------------------------------------//
    
    protected object _value;
    protected Node _parent;
    
    protected Dictionary<string,Node> _dictionary;
    protected ArrayRig<Node> _array;
    
    protected bool _dictionarySet;
    protected bool _arraySet;
    
    //-------------------------------------------//
    
    public Node(Node parent) {
      _parent = parent;
    }
    
    public Node(Node parent = null, object value = null) {
      _parent = parent;
      _value = value;
      Set = _value != null;
    }
    
    /// <summary>
    /// Clear the node collections.
    /// </summary>
    public void Clear() {
      if(ArraySet) _array.Reset();
      if(DictionarySet) _dictionary.Clear();
    }
    
    /// <summary>
    /// Set the parent node for this node. If the key is defined, this node is
    /// added to the parent nodes dictionary, else the parents array.
    /// Specify the parent as 'Null' to simply remove the parent reference.
    /// </summary>
    public void SetParent(Node parent, string key = null) {
      if(_parent != null) _parent.Remove(this);
      if(parent == null) {
        _parent = null;
      } else {
        _parent = parent;
        if(key == null) {
          _parent.Add(this);
        } else {
          _parent[key] = this;
        }
      }
    }
    
    /// <summary>
    /// Remove a child node by index.
    /// </summary>
    public void Remove(int index) {
      if(_arraySet && _array.Count > index) {
        var node = _array.RemoveReturn(index);
        node._parent = null;
      }
    }
    
    /// <summary>
    /// Remove a child node by key.
    /// </summary>
    public void Remove(string key) {
      Node node;
      if(_dictionarySet && _dictionary.TryGetValue(key, out node)) {
        _dictionary.Remove(key);
        node._parent = null;
      }
    }
    
    /// <summary>
    /// Remove a child node by key.
    /// </summary>
    public void Remove(params string[] keys) {
      Node node = this;
      // iterate the keys
      int length = keys.Length-1;
      for (int i = 0; i < length; i++) {
        // does the next node exist by key?
        if (node._dictionarySet && node._dictionary.TryGetValue(keys[i], out node)) {
        } else return;
      }
      // were any keys specified? yes, remove the child node
      if(length > 0) {
        if(node._dictionary.TryGetValue(keys[length], out node)) {
          node._parent._dictionary.Remove(keys[length]);
          node._parent = null;
        }
      } else if(_parent != null) {
        _parent.Remove(this);
      }
    }
    
    /// <summary>
    /// Remove a child node directly.
    /// </summary>
    public void Remove(Node child) {
      if(_dictionarySet && _dictionary.ContainsValue(child)) {
        foreach(KeyValuePair<string, Node> entry in Dictionary) {
          if(entry.Value == child) {
            _dictionary.Remove(entry.Key);
            child._parent = null;
            return;
          }
        }
      }
      if(_arraySet) {
        _array.Remove(child);
        child._parent = null;
      }
    }
    
    /// <summary>
    /// Adds non existing values and overrides existing values resulting in an updated
    /// node with historical values.
    /// </summary>
    public void Combine(Node node) {
      if(node.DictionarySet) {
        if(_dictionarySet) {
          foreach(KeyValuePair<string,Node> entry in node.Dictionary) {
            if(_dictionary.ContainsKey(entry.Key)) {
              _dictionary[entry.Key].Combine(entry.Value);
            } else {
              _dictionary[entry.Key] = entry.Value;
            }
          }
        } else {
          _dictionary = new Dictionary<string, Node>(node._dictionary);
          _dictionarySet = true;
        }
      }
      if(node.ArraySet) {
        if(_arraySet) {
          foreach(Node n in node._array) {
            int index = _array.IndexOf(n);
            if(index == -1) {
              _array.Add(n);
            } else {
              _array[index].Combine(n);
            }
          }
        } else {
          _arraySet = true;
          _array = new ArrayRig<Node>(node._array);
        }
      }
      if(node.Set) {
        _value = node._value;
      }
    }
    
    /// <summary>
    /// Checks if the specified node exists, matching all of its children and values.
    /// </summary>
    public bool Contains(Node node) {
      if(node == null) return true;
      if(node.Set) {
        if(!Set || !_value.Equals(node._value)) {
          return false;
        }
      }
      if(node.DictionarySet) {
        if(!DictionarySet) return false;
        Node thisNode;
        foreach(var otherNode in node._dictionary) {
          if(_dictionary.TryGetValue(otherNode.Key, out thisNode)) {
            if(!thisNode.Contains(otherNode.Value)) return false;
          } else return false;
        }
      }
      if(node.ArraySet) {
        if(!ArraySet) return false;
        foreach(var otherNode in node._array) {
          bool found = false;
          foreach(var thisNode in _array) {
            if(thisNode.Contains(otherNode)) {
              found = true;
              break;
            }
          }
          if(!found) return false;
        }
      }
      return true;
    }
    
    /// <summary>
    /// Get whether the specified keys exist in sequence within the node.
    /// </summary>
    public bool Contains(params string[] keys) {
      Node node = this;
      foreach(var key in keys) {
        if(!node._dictionarySet || !node._dictionary.TryGetValue(key, out node)) {
          return false;
        }
      }
      return node.Set;
    }
    
    /// <summary>
    /// Return the object reference as a specified type.
    /// </summary>
    public T As<T>() {
      if(Object == null) return default(T);
      return (T)Object;
    }
    
    /// <summary>
    /// Get the value of the specified node if it exists, else return default.
    /// The node value and the specified type must be compatible.
    /// </summary>
    public short Default(short @default, params string[] keys) {
      Node node = this;
      foreach(string key in keys) node = node[key];
      return node.Set ? node.Int16 : node.Int16 = @default;
    }
    
    /// <summary>
    /// Get the value of the specified node if it exists, else return default.
    /// The node value and the specified type must be compatible.
    /// </summary>
    public int Default(int @default, params string[] keys) {
      Node node = this;
      foreach(string key in keys) node = node[key];
      return node.Set ? node.Int32 : node.Int32 = @default;
    }
    
    /// <summary>
    /// Get the value of the specified node if it exists, else return default.
    /// The node value and the specified type must be compatible.
    /// </summary>
    public long Default(long @default, params string[] keys) {
      Node node = this;
      foreach(string key in keys) node = node[key];
      return node.Set ? node.Int64 : node.Int64 = @default;
    }
    
    /// <summary>
    /// Get the value of the specified node if it exists, else return default.
    /// The node value and the specified type must be compatible.
    /// </summary>
    public ushort Default(ushort @default, params string[] keys) {
      Node node = this;
      foreach(string key in keys) node = node[key];
      return node.Set ? node.UInt16 : node.UInt16 = @default;
    }
    
    /// <summary>
    /// Get the value of the specified node if it exists, else return default.
    /// The node value and the specified type must be compatible.
    /// </summary>
    public uint Default(uint @default, params string[] keys) {
      Node node = this;
      foreach(string key in keys) node = node[key];
      return node.Set ? node.UInt32 : node.UInt32 = @default;
    }
    
    /// <summary>
    /// Get the value of the specified node if it exists, else return default.
    /// The node value and the specified type must be compatible.
    /// </summary>
    public ulong Default(ulong @default, params string[] keys) {
      Node node = this;
      foreach(string key in keys) node = node[key];
      return node.Set ? node.UInt64 : node.UInt64 = @default;
    }
    
    /// <summary>
    /// Get the value of the specified node if it exists, else return default.
    /// The node value and the specified type must be compatible.
    /// </summary>
    public string Default(string @default, params string[] keys) {
      Node node = this;
      foreach(string key in keys) node = node[key];
      return node.Set ? node.String : node.String = @default;
    }
    
    /// <summary>
    /// Get the value of the specified node if it exists, else return default.
    /// The node value and the specified type must be compatible.
    /// </summary>
    public double Default(double @default, params string[] keys) {
      Node node = this;
      foreach(string key in keys) node = node[key];
      return node.Set ? node.Double : node.Double = @default;
    }
    
    /// <summary>
    /// Get the value of the specified node if it exists, else return default.
    /// The node value and the specified type must be compatible.
    /// </summary>
    public float Default(float @default, params string[] keys) {
      Node node = this;
      foreach(string key in keys) node = node[key];
      return node.Set ? node.Float : node.Float = @default;
    }
    
    /// <summary>
    /// Get the value of the specified node if it exists, else return default.
    /// The node value and the specified type must be compatible.
    /// </summary>
    public T Default<T>(T @default, params string[] keys) {
      Node node = this;
      foreach(string key in keys) node = node[key];
      return (T)(node.Set ? node.Object : node.Object = @default);
    }
    
    /// <summary>
    /// Get a json representation of the node.
    /// </summary>
    public string ToJson() {
      return NodeSerialization.SerializeJson(this);
    }
    
    public override string ToString() {
      return "[Key=" + Key + ", Value=" + _value + ", Children=" + ((_dictionarySet ? _dictionary.Count : 0) + (_arraySet ? Array.Count : 0)) + "]";
    }
    
    public bool Equals(string str) {
      return _value.Equals(str);
    }
    
    public bool Equals(Dictionary<string,Node> dictionary) {
      return _dictionarySet ? _dictionary.Equals(dictionary) : dictionary == null;
    }
    
    /// <summary>
    /// Get a string key and value dictionary.
    /// </summary>
    public Dictionary<string, string> ToDictionary() {
      Dictionary<string, string> dictionaryString = new Dictionary<string, string>();
      foreach(KeyValuePair<string, Node> entry in Dictionary) {
        dictionaryString.Add(entry.Key, entry.Value.String);
      }
      return dictionaryString;
    }
    
    /// <summary>
    /// Add a node to this nodes array collection.
    /// </summary>
    public void Add(Node node) {
      if(!_arraySet) {
        Array = new ArrayRig<Node>();
        _arraySet = true;
      }
      node._parent = this;
      _array.Add(node);
    }
    
    /// <summary>
    /// Add a node to this nodes dictionary collection with the specified key.
    /// </summary>
    public Node Add(string key) {
      if(!_dictionarySet) {
        _dictionary = new Dictionary<string, Node>();
        _dictionarySet = true;
      }
      var node = new Node(this);
      _dictionary.Add(key, node);
      return node;
    }
    
    /// <summary>
    /// Get the Node at the end of the specified path.
    /// </summary>
    public static Node Get(Node node, string path) {
      if(string.IsNullOrEmpty(path)) return node;
      foreach(string key in path.Split(Chars.ForwardSlash)) {
        switch(key) {
          case "..":
            node = GetRoot(node);
            break;
          case ".":
            node = node._parent;
            break;
          default:
            node = node[key];
            break;
        }
      }
      return node;
    }
    
    /// <summary>
    /// Get the root node in the heirarchy.
    /// </summary>
    public static Node GetRoot(Node node) {
      while(node.Parent != null) {
        node = node.Parent;
      }
      return node;
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Access a child node by index.
    /// </summary>
    public Node this[int index] {
      get {
        // set list if not set
        if(!_arraySet) {
          _array = new ArrayRig<Node>();
          _arraySet = true;
        }
        // add items if index not set
        if(index > _array.Count-1) {
          while(index > _array.Count) {
            _array.Add(new Node(this));
          }
          _array.Add(new Node(this));
        }
        return _array[index];
      }
      set {
        // set list if not set
        if(!_arraySet) {
          _arraySet = true;
          _array = new ArrayRig<Node>();
        }
        value._parent = this;
        // add items if index not set
        if(index > _array.Count-1) {
          while(index > _array.Count) {
            _array.Add(new Node(this));
          }
          _array.Add(value);
        } else {
          _array[index] = value;
        }
      }
    }
    
    /// <summary>
    /// Access a child node by key.
    /// </summary>
    public Node this[string key] {
      get {
        // set dictionary if not set
        Node node;
        if(!_dictionarySet) {
          _dictionarySet = true;
          _dictionary = new Dictionary<string,Node>();
          node = new Node(this);
          _dictionary.Add(key, node);
          return node;
        }
        
        if(_dictionary.TryGetValue(key, out node)) {
          return node;
        }
        
        node = new Node(this);
        _dictionary.Add(key, node);
        return node;
      }
      set {
        // set dictionary if not set
        if(!_dictionarySet) {
          _dictionarySet = true;
          _dictionary = new Dictionary<string,Node>();
        }
        value._parent = this;
        _dictionary[key] = value;
      }
    }
    
    /// <summary>
    /// Access a node via a key path.
    /// </summary>
    public Node this[params string[] keys] {
      get {
        Node node = this;
        foreach(string key in keys) node = node[key];
        return node;
      }
      set {
        Node node = this;
        int index = keys.Length-1;
        for(int i = 0; i < index; ++i) {
          node = node[keys[i]];
        }
        value._parent = node;
        node[keys[index]] = value;
      }
    }
    
    /// <summary>
    /// Access a child node via an index path.
    /// </summary>
    public Node this[params int[] indicies] {
      get {
        Node node = this;
        foreach(int index in indicies) {
          node = node[index];
        }
        return node;
      }
      set {
        Node node = this;
        for(int i = 0; i < indicies.Length-1; ++i) {
          node = node[indicies[i]];
        }
        value._parent = node;
        node[indicies[indicies.Length-1]] = value;
      }
    }
    
    public IEnumerator<Node> GetEnumerator() {
      return new NodeEnumerator(this);
    }
    
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
    
    /// <summary>
    /// Clone the values and children values of the specified node.
    /// </summary>
    public static Node Clone(Node node, Node parent = null) {
      var n = new Node(parent);
      if(node.Set) n.Object = node.Object;
      if(node.DictionarySet) {
        foreach(var entry in node._dictionary) {
          n[entry.Key] = Clone(entry.Value, n);
        }
      }
      if(node.ArraySet) {
        foreach(var entry in node._array) {
          n.Add(Clone(entry, n));
        }
      }
      return n;
    }
    
    //-------------------------------------------//
    
  }

}