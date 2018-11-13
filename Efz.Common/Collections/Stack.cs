using System;

using Efz.Tools;

namespace Efz.Collections {
  
  /// <summary>
  /// A fast and generic stack of items. Very lightweight (feature poor).
  /// </summary>
  public class Stack<T> {
        
    //-------------------------------------------//
    
    /// <summary>
    /// The current top item.
    /// </summary>
    public T Current;
    /// <summary>
    /// Is the stack empty?
    /// </summary>
    public bool Empty;
    
    /// <summary>
    /// Number of items in the stack.
    /// </summary>
    public int Count;
    
    //-------------------------------------------//
    
    /// <summary>
    /// The current position in the stack.
    /// </summary>
    protected Link<T> _linkCurrent;
    /// <summary>
    /// The last link in the stack.
    /// </summary>
    protected Link<T> _linkLast;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new Stack.
    /// </summary>
    public Stack() {
      Empty = true;
    }
    
    /// <summary>
    /// Clear the content of the stack.
    /// </summary>
    public void Reset() {
      Empty = true;
      _linkCurrent = _linkLast = null;
      Count = 0;
    }
    
    /// <summary>
    /// Pop an item off the stack, returning whether there was an item to pop.
    /// </summary>
    public bool Pop() {
      // early out if empty
      if(Empty) return false;
      
      // set the current item
      Current = _linkCurrent.Item;
      
      // flop flag if last
      Empty |= _linkCurrent == _linkLast;
      
      // step forward
      _linkCurrent = _linkCurrent.Next;
      --Count;
      
      return true;
    }
    
    /// <summary>
    /// Checks whether the item exists in the stack. Slow.
    /// </summary>
    public bool Contains(T item) {
      // early out if empty
      if(Empty) return false;
      
      // start with the current link
      Link<T> check = _linkCurrent;
      // run through the queue
      while(check != _linkLast) {
        if(check.Item.Equals(item)) {
          return true;
        }
        check = check.Next;
      }
      
      // check the final (or only) item
      return check.Item.Equals(item);
    }
    
    /// <summary>
    /// Removes the specified item from the stack if found.
    /// </summary>
    public void Remove(T item) {
      // early out if empty
      if(Empty) return;
      
      // if the current link is the link to remove
      if(_linkCurrent.Item.Equals(item)) {
        // set the next current item
        Current = _linkCurrent.Item;
        
        // flag if empty
        Empty |= _linkCurrent == _linkLast;
        
        // step forward
        _linkCurrent = _linkCurrent.Next;
        --Count;
        
        // early out
        return;
      }
      
      // start with the next link
      Link<T> prev = _linkCurrent;
      Link<T> check = _linkCurrent.Next;
      
      // run through the queue
      while(check != _linkLast) {
        if(check.Item.Equals(item)) {
          // skip the link
          prev.Next = check.Next;
          // decrement count
          --Count;
          return;
        }
        // step forward
        prev = check;
        check = check.Next;
      }
      
      // check if last link
      if(check.Item.Equals(item)) {
        // skip the link
        prev.Next = check.Next;
        // decrement count
        --Count;
      }
      
    }
    
    /// <summary>
    /// Push an item onto the stack.
    /// </summary>
    public void Push(T item) {
      if(Empty) {
        Empty = false;
        // set single item
        _linkLast = _linkCurrent = new Link<T>(item, null);
        _linkCurrent.Next = _linkCurrent;
      } else {
        // replace the current link with a new link
        _linkCurrent = new Link<T>(item, _linkCurrent);
      }
      ++Count;
    }
    
    /// <summary>
    /// Add an item to the end of the stack.
    /// </summary>
    public void Enqueue(T item) {
      if(Empty) {
        Empty = false;
        // set single item
        _linkLast = _linkCurrent = new Link<T>(item, null);
        _linkCurrent.Next = _linkCurrent;
      } else {
        // replace the last link with the new link
        _linkLast = _linkLast.Next = new Link<T>(item, null);
      }
      ++Count;
    }
    
    //-------------------------------------------//
    
  }

}