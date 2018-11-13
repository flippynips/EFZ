/*
 * User: FloppyNipples
 * Date: 26/03/2017
 * Time: 15:47
 */
using System;
using System.Collections.Generic;

using Efz.Collections;

namespace Efz.Web.Display {
  
  /// <summary>
  /// Represents a form element with input controls.
  /// </summary>
  public class ElementForm : Element {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Input elements in the form.
    /// </summary>
    public Dictionary<string, ElementInput> Input {
      get { if(_update) Update(); return _input; }
    }
    /// <summary>
    /// Child forms.
    /// </summary>
    public ArrayRig<ElementForm> Forms {
      get { if(_update) Update(); return _forms; }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Is an update of the child elements required?
    /// </summary>
    protected bool _update;
    /// <summary>
    /// Inner collection of element inputs.
    /// </summary>
    protected Dictionary<string, ElementInput> _input;
    /// <summary>
    /// Inner collection of forms.
    /// </summary>
    protected ArrayRig<ElementForm> _forms;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Create a new form element.
    /// </summary>
    public ElementForm() {
      Tag = Tag.Form;
      _input = new Dictionary<string, ElementInput>();
      _forms = new ArrayRig<ElementForm>();
    }
    
    /// <summary>
    /// Clone the element.
    /// </summary>
    public override Element Clone() {
      
      // create a new element table
      ElementForm clone = new ElementForm();
      
      clone.Tag = Tag;
      if(_content != null) clone._content = new ArrayRig<string>(_content);
      if(_attributes != null) clone._attributes = new Dictionary<string, string>(_attributes);
      if(_style != null) clone._style = new Style(_style);
      
      if(Children != null) {
        // iterate the children of this element
        foreach(var child in Children) {
          clone.AddChild(child.Clone());
        }
      }
      
      clone._input = new Dictionary<string, ElementInput>(Input);
      clone._forms = new ArrayRig<ElementForm>(Forms);
      clone._update = _update;
      
      return clone;
    }
    
    public override void AddChild(Element element) {
      _update = true;
      base.AddChild(element);
    }
    
    public override void RemoveChild(Element element) {
      _update = true;
      base.RemoveChild(element);
    }
    
    public override void Replace(Element childA, Element childB) {
      _update = true;
      base.Replace(childA, childB);
    }
    
    public override void InsertChild(Element element, int index = 0) {
      _update = true;
      base.InsertChild(element, index);
    }
    
    /// <summary>
    /// Validate the form with the specified post parameters. Returns
    /// an empty array if validation succeeded or an array with user
    /// friendly error messages if the validation failed.
    /// </summary>
    public virtual ArrayRig<string> Validate(HttpPostParams parameters, ArrayRig<string> errors = null) {
      if(_update) Update();
      // create a collection of potential errors
      if(errors == null) errors = new ArrayRig<string>(1);
      if(parameters == null) {
        errors.Add("Malformed or missing request parameters.");
        return errors;
      }
      
      // iterate and validate the child forms
      foreach(var form in _forms) {
        form.Validate(parameters, errors);
      }
      
      // iterate and validate the input elements
      foreach(var input in _input) {
        IHttpPostParam parameter = parameters[input.Key];
        input.Value.Validate(parameter, errors);
      }
      
      // return the collection of error messages
      return errors;
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Update the form with children elements.
    /// </summary>
    protected void Update() {
      _update = false;
      _forms.Clear();
      _input.Clear();
      // iterate the elements
      HashSet<Element> hashSet = new HashSet<Element>(_elements.TakeItem());
      _elements.Release();
      foreach(var element in hashSet) {
        if(element.Tag == Tag.Input) {
          
          Element parent = element.Parent;
          while(parent != null && parent != this && parent.Tag != Tag.Form) {
            parent = parent.Parent;
          }
          
          if(parent == this) {
            var input = element as ElementInput;
            if(input == null || input["name"] == null) continue;
            if(!_input.ContainsKey(input["name"])) _input.Add(input["name"], input);
          }
          
        } else if(element.Tag == Tag.Form) {
          
          Element parent = element.Parent;
          while(parent != null && parent != this && parent.Tag != Tag.Form) {
            parent = parent.Parent;
          }
          
          if(parent == this) {
            var form = element as ElementForm;
            if(form != null && !_forms.Contains(form)) _forms.Add(form);
          }
        }
      }
      
    }
    
  }
}
