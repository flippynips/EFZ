using System;
using System.Collections.Generic;

using Efz.Tools;

namespace Efz.Collections {
  
  /// <summary>
  /// A fast and generic queue collection. Very lightweight (feature poor).
  /// Threadsafe to enqueue and dequeue using different threads.
  /// Not threadsave to multi-thread dequeue.
  /// </summary>
  public class Queue<T> : IEnumerable<T> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The current item.
    /// </summary>
    public T Current;
    /// <summary>
    /// Is the queue empty?
    /// </summary>
    public bool Empty;
    
    /// <summary>
    /// Number of items in the queue currently.
    /// </summary>
    public int Count;
    
    //-------------------------------------------//
    
    /// <summary>
    /// The current position in the queue.
    /// </summary>
    protected Link<T> _linkCurrent;
    /// <summary>
    /// The last link in the queue.
    /// </summary>
    protected Link<T> _linkLast;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new Belt.
    /// </summary>
    public Queue() {
      Empty = true;
    }
    
    /// <summary>
    /// Clear the content of the queue.
    /// </summary>
    public void Reset() {
      Empty = true;
      _linkCurrent = _linkLast = null;
      Count = 0;
    }
    
    /// <summary>
    /// Dequeue the an item in the queue, returning whether an item was successfully dequeued.
    /// </summary>
    public bool Dequeue() {
      
      // early out if empty
      if(Empty) return false;
      
      --Count;
      
      // set the current item
      Current = _linkCurrent.Item;
      
      // step forward
      _linkCurrent = _linkCurrent.Next;
      
      // flag if empty
      Empty |= _linkCurrent == null;
      
      return true;
    }
    
    /// <summary>
    /// Dequeue the an item in the queue, returning whether an item was successfully dequeued.
    /// </summary>
    public bool Dequeue(out T current) {
      
      // early out if empty
      if(Empty) {
        current = default(T);
        return false;
      }
      
      --Count;
      
      // set the current item
      current = Current = _linkCurrent.Item;
      
      // step forward
      _linkCurrent = _linkCurrent.Next;
      
      // flag if empty
      Empty |= _linkCurrent == null;
      
      return true;
    }
    
    /// <summary>
    /// Checks whether the item exists in the queue.
    /// </summary>
    public bool Contains(T item) {
      // early out if empty
      if(Empty) return false;
      
      // start with the current link
      Link<T> check = _linkCurrent;
      
      // run through the queue
      while(check != _linkLast) {
        if(check.Item.Equals(item)) return true;
        check = check.Next;
      }
      
      // check the final (or only) item
      return check.Item.Equals(item);
    }
    
    /// <summary>
    /// Add an item to the end of the belt.
    /// </summary>
    public void Enqueue(T item) {
      
      ++Count;
      
      if(Empty) {
        
        // enqueue single item
        _linkCurrent = new Link<T>(item);
        _linkLast = _linkCurrent;
        Empty = false;
        
      } else {
        
        // replace the last link with a new link
        _linkLast.Next = new Link<T>(item, null);
        _linkLast = _linkLast.Next;
        
      }
    }
    
    public IEnumerator<T> GetEnumerator() {
      return new Enumerator(this);
    }
    
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return new Enumerator(this);
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Provides enumeration of a queue.
    /// </summary>
    public class Enumerator : IEnumerator<T> {
      
      //-----------------------------//
      
      /// <summary>
      /// Required non-generic implementation.
      /// </summary>
      object System.Collections.IEnumerator.Current { get { return this.Current; } }
      
      /// <summary>
      /// Get the current enumeration item.
      /// </summary>
      public T Current { get { return _link.Item; } }
      
      //-----------------------------//
      
      /// <summary>
      /// The current link.
      /// </summary>
      private Link<T> _link;
      /// <summary>
      /// The queue being enumerated.
      /// </summary>
      private Queue<T> _queue;
      
      /// <summary>
      /// Flag for the enumeration being complete.
      /// </summary>
      private bool _last;
      /// <summary>
      /// Flag for the start of the enumeration.
      /// </summary>
      private bool _first;
      
      //-----------------------------//
      
      /// <summary>
      /// Initialize a new enumerator for the specified queue.
      /// </summary>
      public Enumerator(Queue<T> queue) {
        _queue = queue;
        
        _last = _queue.Empty;
        if(!_last) {
          _first = true;
          _link = _queue._linkCurrent;
        }
      }
      
      /// <summary>
      /// Move to the next link and item.
      /// </summary>
      public bool MoveNext() {
        // if done - no more elements
        if(_last) return false;
        
        if(_first) {
          // no longer the first item
          _first = false;
          // flag complete if the current link is the queues last
          _last = _link == _queue._linkLast;
        } else {
          // step the current link forward one
          _link = _link.Next;
          // flag complete if the current link is the queues last
          _last = _link == _queue._linkLast;
        }
        
        return true;
      }
      
      /// <summary>
      /// Reset the enumeration instance.
      /// </summary>
      public void Reset() {
        _last = _queue.Empty;
        if(!_last) {
          _link = _queue._linkCurrent;
        }
      }
      
      /// <summary>
      /// Dispose of the enumeration instance.
      /// </summary>
      public void Dispose() {
        _link = null;
        _queue = null;
      }
      
      //-----------------------------//
      
    }
    
  }

}