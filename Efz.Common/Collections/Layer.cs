using System;

namespace Efz.Collections {
  /// <summary>
  /// A collection with an integer identifier included.
  /// </summary>
  public class Layer<T> where T : class {
    
    //-------------------------------------------//
    
    public int depth;
    public ArrayRig<T> elements;
        
    //-------------------------------------------//
        
    //-------------------------------------------//
        
    public Layer(int _depth = 0) {
      depth    = _depth;
      elements = new ArrayRig<T>();
    }

    public void Add(T _element) {
      elements.Add(_element);
    }
    
    public bool Remove(T _element) {
      elements.Remove(_element);
      if(elements.Count.Equals(0)) {
        return true;
      }
      return false;
    }

    public void Clear() {
      for(int i = elements.Count - 1; i >= 0; --i) {
        elements[i] = null;
      }
      elements.Reset();
    }
        
    //-------------------------------------------//
        
        
        
  }
}
