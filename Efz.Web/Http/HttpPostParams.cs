/*
 * User: FloppyNipples
 * Date: 20/03/2017
 * Time: 01:02
 */
using System;
using System.Collections.Generic;

using Efz.Collections;

namespace Efz.Web
{
  /// <summary>
  /// A collection of post parameters that can represent numerous value types.
  /// </summary>
  public class HttpPostParams :
    IEnumerable<KeyValuePair<string, HttpPostParam<string>>>,
    IEnumerable<KeyValuePair<string, HttpPostParam<ArrayRig<byte>>>>{
    
    //-------------------------------------------//
    
    /// <summary>
    /// Get the total number of parameters in this collection of post parameters.
    /// </summary>
    public int Count { get { return ParamsBytes.Count + ParamsStrings.Count; } }
    
    /// <summary>
    /// Map of params to bytes.
    /// </summary>
    public Dictionary<string, HttpPostParam<ArrayRig<byte>>> ParamsBytes;
    /// <summary>
    /// Map of params to strings.
    /// </summary>
    public Dictionary<string, HttpPostParam<string>> ParamsStrings;
    
    /// <summary>
    /// Get a post parameter by key. Returns 'Null' if no post parameter is found.
    /// </summary>
    public IHttpPostParam this[string key] {
      get {
        HttpPostParam<string> paramString;
        if(ParamsStrings.TryGetValue(key, out paramString)) {
          return paramString;
        }
        HttpPostParam<ArrayRig<byte>> paramBytes;
        if(ParamsBytes.TryGetValue(key, out paramBytes)) {
          return paramBytes;
        }
        return null;
      }
    }
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new collection of post parameters.
    /// </summary>
    public HttpPostParams() {
      ParamsBytes = new Dictionary<string, HttpPostParam<ArrayRig<byte>>>();
      ParamsStrings = new Dictionary<string, HttpPostParam<string>>();
    }
    
    /// <summary>
    /// Try get the string value of the specified key.
    /// </summary>
    public bool TryGet(string key, out string value) {
      HttpPostParam<string> val;
      if(ParamsStrings.TryGetValue(key, out val)) {
        value = val.Value;
        return true;
      }
      value = null;
      return false;
    }
    
    /// <summary>
    /// Try get the integer value of the specified key.
    /// </summary>
    public bool TryGet(string key, out int value) {
      HttpPostParam<string> val;
      if(ParamsStrings.TryGetValue(key, out val)) {
        string valueStr = val.Value;
        return int.TryParse(valueStr, out value);
      }
      value = 0;
      return false;
    }
    
    /// <summary>
    /// Try get the long value of the specified key.
    /// </summary>
    public bool TryGet(string key, out long value) {
      HttpPostParam<string> val;
      if(ParamsStrings.TryGetValue(key, out val)) {
        string valueStr = val.Value;
        return long.TryParse(valueStr, out value);
      }
      value = 0L;
      return false;
    }
    
    /// <summary>
    /// Try get the float value of the specified key.
    /// </summary>
    public bool TryGet(string key, out float value) {
      HttpPostParam<string> val;
      if(ParamsStrings.TryGetValue(key, out val)) {
        string valueStr = val.Value;
        return float.TryParse(valueStr, out value);
      }
      value = 0f;
      return false;
    }
    
    /// <summary>
    /// Try get the double value of the specified key.
    /// </summary>
    public bool TryGet(string key, out double value) {
      HttpPostParam<string> val;
      if(ParamsStrings.TryGetValue(key, out val)) {
        string valueStr = val.Value;
        return double.TryParse(valueStr, out value);
      }
      value = 0.0;
      return false;
    }
    
    /// <summary>
    /// Try get the string value of the specified key.
    /// </summary>
    public bool TryGet(string key, out ArrayRig<byte> value) {
      HttpPostParam<ArrayRig<byte>> val;
      if(ParamsBytes.TryGetValue(key, out val)) {
        value = val.Value;
        return true;
      }
      value = null;
      return false;
    }
    
    /// <summary>
    /// Try get the string value of the specified key.
    /// </summary>
    public bool TryGet(string key, out HttpPostParam<string> value) {
      return ParamsStrings.TryGetValue(key, out value);
    }
    
    /// <summary>
    /// Try get the string value of the specified key.
    /// </summary>
    public bool TryGet(string key, out HttpPostParam<ArrayRig<byte>> value) {
      return ParamsBytes.TryGetValue(key, out value);
    }
    
    
    /// <summary>
    /// Add a key value pair to the post params.
    /// </summary>
    public void Add(string key) {
      ParamsStrings.Add(key, new HttpPostParam<string>());
    }
    
    /// <summary>
    /// Add a key value pair to the post params.
    /// </summary>
    public void Add(string key, string value) {
      ParamsStrings.Add(key, new HttpPostParam<string>(value));
    }
    
    /// <summary>
    /// Add a key value pair to the post params.
    /// </summary>
    public void Add(string key, string value, Dictionary<string, string> parameters) {
      ParamsStrings.Add(key, new HttpPostParam<string>(value, parameters));
    }
    
    /// <summary>
    /// Add a key value pair to the post params.
    /// </summary>
    public void Add(string key, ArrayRig<byte> value) {
      ParamsBytes.Add(key, new HttpPostParam<ArrayRig<byte>>(value));
    }
    
    /// <summary>
    /// Add a key value pair to the post params.
    /// </summary>
    public void Add(string key, ArrayRig<byte> value, Dictionary<string, string> parameters) {
      ParamsBytes.Add(key, new HttpPostParam<ArrayRig<byte>>(value, parameters));
    }
    
    IEnumerator<KeyValuePair<string, HttpPostParam<string>>>
      IEnumerable<KeyValuePair<string, HttpPostParam<string>>>.GetEnumerator() {
      return ParamsStrings.GetEnumerator();
    }
    
    IEnumerator<KeyValuePair<string, HttpPostParam<ArrayRig<byte>>>>
      IEnumerable<KeyValuePair<string, HttpPostParam<ArrayRig<byte>>>>.GetEnumerator() {
      return ParamsBytes.GetEnumerator();
    }
    
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      var array = new ArrayRig<object>();
      foreach(KeyValuePair<string, HttpPostParam<string>> entry in ParamsStrings) {
        array.Add(entry);
      }
      foreach(KeyValuePair<string, HttpPostParam<ArrayRig<byte>>> entry in ParamsBytes) {
        array.Add(entry);
      }
      return array.GetEnumerator();
    }
    
    /// <summary>
    /// Get a string representation of the post parameters.
    /// </summary>
    public override string ToString() {
      var builder = StringBuilderCache.Get();
      builder.Append("Post parameters [");
      // iterate the string parameters
      foreach(var parameter in ParamsStrings) {
        builder.Append(Chars.NewLine);
        builder.Append(parameter.Key);
        builder.Append(Chars.Equal);
        builder.Append(parameter.Value.Value);
        builder.Append(Chars.NewLine);
        foreach(var attribute in parameter.Value.Params) {
          builder.Append(attribute.Key);
          builder.Append(Chars.Colon);
          builder.Append(attribute.Value);
          builder.Append(Chars.SemiColon);
          builder.Append(Chars.Space);
        }
      }
      // iterate the byte parameters
      foreach(var parameter in ParamsBytes) {
        builder.Append(Chars.NewLine);
        builder.Append(parameter.Key);
        builder.Append(Chars.Equal);
        builder.Append(parameter.Value.Value.Count);
        builder.Append("bytes");
        builder.Append(Chars.NewLine);
        foreach(var attribute in parameter.Value.Params) {
          builder.Append(attribute.Key);
          builder.Append(Chars.Colon);
          builder.Append(attribute.Value);
          builder.Append(Chars.SemiColon);
          builder.Append(Chars.Space);
        }
      }
      builder.Append(Chars.NewLine);
      builder.Append(Chars.BracketSqClose);
      return StringBuilderCache.SetAndGet(builder);
    }

    
    //-------------------------------------------//
    
    
  }
}
