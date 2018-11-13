/*
 * User: FloppyNipples
 * Date: 23/04/2017
 * Time: 21:07
 */
using System;
using System.Collections.Generic;

using Efz;
using Efz.Collections;
using Efz.Tools;
using Efz.Web.Display;

namespace Efz.Web.Display {
  
  /// <summary>
  /// Modifications that can be made to an element. Useful for updating content.
  /// </summary>
  public class ElementCoat {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Actions that are called when the element is modified.
    /// </summary>
    protected ArrayRig<Teple<string, IAction<Element>>> _actions;
    /// <summary>
    /// Attributes that are applied.
    /// </summary>
    protected Dictionary<string, Teple<string,string>> _attributes;
    
    //-------------------------------------------//
    
    public ElementCoat() {
      
    }
    
    public void Set(string key, IAction action) {
      
    }
    
    //-------------------------------------------//
    
  }
  
  
}
