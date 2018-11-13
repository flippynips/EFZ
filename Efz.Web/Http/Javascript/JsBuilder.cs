/*
 * User: FloppyNipples
 * Date: 17/07/2017
 * Time: 12:55
 */
using System;
using System.Text;
using Efz.Collections;

namespace Efz.Web.Javascript {
  
  /// <summary>
  /// Builder of all things javascript.
  /// </summary>
  public class JsBuilder {
    
    //----------------------------------//
    
    /// <summary>
    /// Collection of commands which constitutes the complete javascript.
    /// </summary>
    internal ArrayRig<Js> Components;
    /// <summary>
    /// Collection of prototypes required by scripts.
    /// </summary>
    internal ArrayRig<JsPrototype> Prototypes;
    
    /// <summary>
    /// String builder of this javascript.
    /// </summary>
    internal StringBuilder String;
    
    /// <summary>
    /// Next unique name.
    /// </summary>
    internal string NextName {
      get { return System.Threading.Interlocked.Increment(ref _nameIndex).AsLetters(); }
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Current name index.
    /// </summary>
    protected int _nameIndex;
    /// <summary>
    /// Last javascript element added.
    /// </summary>
    protected Js _last;
    
    //----------------------------------//
    
    public JsBuilder() {
      Components = new ArrayRig<Js>();
      Prototypes = new ArrayRig<JsPrototype>();
    }
    
    /// <summary>
    /// Build the javascript.
    /// </summary>
    public string Build() {
      String = StringBuilderCache.Get();
      foreach(var component in Components) component.Build(this);
      var componentsStr = String.ToString();
      String.Clear();
      foreach(var prototype in Prototypes) prototype.Build(this);
      return StringBuilderCache.SetAndGet(String) + componentsStr;
    }
    
    /// <summary>
    /// Add a raw string as javascript.
    /// </summary>
    public void Add(string command) {
      _last = null;
      Components.Add(new JsCommandString(command));
    }
    
    /// <summary>
    /// Add a custom command to the builder.
    /// </summary>
    public void Add(JsCommand command) {
      _last = command;
      Components.Add(command);
    }
    
    /// <summary>
    /// Add a new global function.
    /// </summary>
    public void Add(JsMethod jsMethod) {
      _last = jsMethod;
      Components.Add(jsMethod);
    }
    
    /// <summary>
    /// Add a new prototype.
    /// </summary>
    internal void Add(JsPrototype prototype) {
      if(!Prototypes.Contains(prototype)) Prototypes.Add(prototype);
    }
    
    /// <summary>
    /// Create a new object of the specified type.
    /// </summary>
    internal void Add(JsClass jsClass, Js[] parameters) {
      _last = jsClass;
      Components.Add(new JsCommandNew(jsClass, parameters));
    }
    
    /// <summary>
    /// Create a new variable in the current context.
    /// </summary>
    public void Add(JsVar jsVar) {
      if(jsVar.Value == _last) Components.Pop();
      if(jsVar.Name == null) jsVar.Name = NextName;
      _last = jsVar;
      Components.Add(new JsCommandVar(jsVar));
    }
    
    //----------------------------------//
    
  }
  
}
