/*
 * User: FloppyNipples
 * Date: 27/04/2017
 * Time: 23:09
 */
using System;
using System.Text;

using Efz.Tools;

namespace Efz.Data {
  
  /// <summary>
  /// Text template parsing. Will dynamically replace predetermined keys within a string
  /// with other values.
  /// </summary>
  public class TextTemplate {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Tree of character collections to replacement strings.
    /// </summary>
    protected TreeSearch<char, Teple<string, ActionRoll<StringBuilder, string, int>>> _tree;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new template.
    /// </summary>
    public TextTemplate() {
      _tree = new TreeSearch<char, Teple<string, ActionRoll<StringBuilder, string, int>>>();
    }
    
    /// <summary>
    /// Add a key to replace in strings with the specified value.
    /// </summary>
    public void Add(string key, Action<StringBuilder, string, int> onParsed) {
      _tree.Add(new Teple<string, ActionRoll<StringBuilder, string, int>>(
        null, new ActionRoll<StringBuilder, string, int>(onParsed)),
        key.ToCharArray());
    }
    
    /// <summary>
    /// Add a key to replace in strings with the specified value.
    /// </summary>
    public void Add(string key, ActionRoll<StringBuilder, string, int> onParsed) {
      _tree.Add(new Teple<string, ActionRoll<StringBuilder, string, int>>(
        null, new ActionRoll<StringBuilder, string, int>(onParsed)), key.ToCharArray());
    }
    
    /// <summary>
    /// Add a key to replace in strings with the specified value.
    /// </summary>
    public void Add(string key, string value, Action<StringBuilder, string, int> onParsed) {
      _tree.Add(new Teple<string, ActionRoll<StringBuilder, string, int>>(
        value, new ActionRoll<StringBuilder, string, int>(onParsed)), key.ToCharArray());
    }
    
    /// <summary>
    /// Add a key to replace in strings with the specified value.
    /// </summary>
    public void Add(string key, string value, ActionRoll<StringBuilder, string, int> onParsed) {
      _tree.Add(new Teple<string, ActionRoll<StringBuilder, string, int>>(value, onParsed), key.ToCharArray());
    }
    
    /// <summary>
    /// Add a key to replace in strings with the specified value.
    /// </summary>
    public void Add(string key, string value) {
      _tree.Add(new Teple<string, ActionRoll<StringBuilder, string, int>>(value, null), key.ToCharArray());
    }
    
    /// <summary>
    /// Remove a key.
    /// </summary>
    public void Remove(string key) {
      _tree.Remove(key.ToCharArray());
    }
    
    /// <summary>
    /// Run the template on the specified string.
    /// </summary>
    public void Run(string str, IAction<StringBuilder> onRan) {
      
      onRan.ArgA = StringBuilderCache.Get();
      ManagerUpdate.Control.AddSingle(Run, str, 0, onRan, _tree.SearchDynamic());
      
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Run loop of the text template.
    /// </summary>
    protected void Run(string str, int index, IAction<StringBuilder> onRan,
      TreeSearch<char, Teple<string, ActionRoll<StringBuilder, string, int>>>.DynamicSearch search) {
      
      int end = index + Global.BufferSizeLocal;
      if(end > str.Length) end = str.Length;
      
      Teple<string, ActionRoll<StringBuilder, string, int>> result = null;
      
      // iterate the string
      for(int i = index; i < end; ++i) {
        
        // has a replacement string been found?
        if (search.Next(str[i])) {
          
          // yes, set the current result
          result = search.Values.Pop();
          
        } else {
          
          // no, has a replacement been found?
          if (result != null) {
            // yes, append the replacement
            if(result.ArgA != null) onRan.ArgA.Append(result.ArgA);
            if(result.ArgB != null) result.ArgB.Run(onRan.ArgA, str, str.Length-1);
            result = null;
          }
          
          // append the non-matching character
          onRan.ArgA.Append(str[i]);
        }
      }
      
      // was the last character to be replaced?
      if(result != null) {
        // yes, append the replacement
        if(result.ArgA != null) onRan.ArgA.Append(result.ArgA);
        if(result.ArgB != null) result.ArgB.Run(onRan.ArgA, str, str.Length-1);
      }
      
      if(index < end) ManagerUpdate.Control.AddSingle(Run, str, index, onRan, search);
      else onRan.Run();
      
    }
    
  }
  
}
