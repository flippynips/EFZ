using System;
using System.Collections.Generic;

using Efz.Collections;
using Efz.Tools;

namespace Efz.Data {
  
  public class NodeEnumerator : IEnumerator<Node>, IEnumerable<Node> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The current node.
    /// </summary>
    public Node Current {
      get { return _node; }
    }
    
    /// <summary>
    /// Flag to skip iteration of the current nodes children.
    /// </summary>
    public bool Skip {
      get { return !_first; }
      set { _first = !value; }
    }
    
    //-------------------------------------------//
    
    object System.Collections.IEnumerator.Current { get { return _node; } }
    
    protected Node _node;
    protected Node _root;
    
    protected bool _skip;
    protected bool _list;
    protected bool _first;
    
    protected ArrayRig<Teple<IEnumerator<Node>, bool>> _nodeEnumerators;
    protected IEnumerator<Node> _currentEnumerator;
    
    //-------------------------------------------//
    
    public IEnumerator<Node> GetEnumerator() {
      return this;
    }
    
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this;
    }
    
    public NodeEnumerator(Node node) {
      _node = _root = node;
      _first = true;
      _nodeEnumerators = new ArrayRig<Teple<IEnumerator<Node>, bool>>();
      _currentEnumerator = new ArrayRig<Node>(1).GetEnumerator();
    }
    
    public void Dispose() {
      foreach(Teple<IEnumerator<Node>, bool> e in _nodeEnumerators) {
        e.ArgA.Dispose();
      }
      _nodeEnumerators.Clear();
      _currentEnumerator.Dispose();
    }
    
    public void Reset() {
      foreach(Teple<IEnumerator<Node>, bool> e in _nodeEnumerators) {
        e.ArgA.Dispose();
      }
      _nodeEnumerators.Clear();
      _currentEnumerator.Dispose();
      
      _node = _root;
      _list = false;
      _first = true;
      _currentEnumerator = new ArrayRig<Node>(1).GetEnumerator();
    }
    
    public bool MoveNext() {
      if(_first) {
        _first = false;
        return _node != null;
      }
      
      while(true) {
        // is there the potential to iterate over the current nodes children?
        if(_node.ArraySet) {
          // push current enumerator onto the stack
          _nodeEnumerators.Add(new Teple<IEnumerator<Node>, bool>(_currentEnumerator, _list));
          _currentEnumerator = _node.Array.GetEnumerator();
          if(_currentEnumerator.MoveNext()) {
            _list = true;
            _node = _currentEnumerator.Current;
            return true;
          }
        }
        if(_node.DictionarySet) {
          // push current enumerator onto the stack
          _nodeEnumerators.Add(new Teple<IEnumerator<Node>, bool>(_currentEnumerator, _list));
          _currentEnumerator = _node.Dictionary.Values.GetEnumerator();
          if(_currentEnumerator.MoveNext()) {
            _list = false;
            _node = _currentEnumerator.Current;
            return true;
          }
        }
        
        while(true) {
          
          // move the current enumerator
          if(_currentEnumerator.MoveNext()) {
            _node = _currentEnumerator.Current;
            return true;
          }
          
          if(_list) {
            _list = false;
            if(_node.DictionarySet) {
              // set the first enumerator as this nodes list
              _currentEnumerator = _node.Dictionary.Values.GetEnumerator();
              if(_currentEnumerator.MoveNext()) {
                _node = _currentEnumerator.Current;
                return true;
              }
            }
          }
          
          // any node enumerators to pop?
          if(_nodeEnumerators.Count == 0) return false;
          
          // pop last enumerator off stack if it exists
          Teple<IEnumerator<Node>, bool> next = _nodeEnumerators.Pop();
          _currentEnumerator = next.ArgA;
          _list = next.ArgB;
          
          // move up a node
          _node = _node.Parent;
        }
      }
    }
    
    //-------------------------------------------//
      
    
  }
  
}