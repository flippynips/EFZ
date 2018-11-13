/*
 * User: Joshua
 * Date: 22/10/2016
 * Time: 2:11 PM
 */
using System;
using System.Collections.Generic;

namespace Efz.Collections {
  
  /// <summary>
  /// A queue structure that uses a flexible array to store items. This structure
  /// is good for large numbers of items accessed like a queue.
  /// Not completely threadsafe however the 'Enqueue' and 'Next' methods should be
  /// safe to use in tandem.
  /// </summary>
  public class ArrayQueue<T> : IEnumerable<T>, IDisposable {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The current item in the queue.
    /// </summary>
    public T Current;
    /// <summary>
    /// Is the current collection empty?
    /// </summary>
    public bool Empty;
    /// <summary>
    /// Number of items in the queue.
    /// </summary>
    public int Count;
    
    //-------------------------------------------//
    
    /// <summary>
    /// The array rig used for the queue. The order of this collection
    /// does not reflect the order of the queue.
    /// </summary>
    private ArrayRig<T> _rig;
    /// <summary>
    /// The current index in the queue.
    /// </summary>
    private int _index;
    
    /// <summary>
    /// The first index in the loopback collection.
    /// </summary>
    private int _loopbackStart;
    /// <summary>
    /// The last index in the loopback collection.
    /// </summary>
    private int _loopbackEnd;
    /// <summary>
    /// Has the loopback collection been started?
    /// </summary>
    private bool _loopbackStarted;
    /// <summary>
    /// Has the loopback collection caught up with the last
    /// queue index?
    /// </summary>
    private bool _loopbackFilled;
    
    /// <summary>
    /// Is the loopback collection currently being iterated over?
    /// </summary>
    private bool _loopbackActive;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Construct a new empty array queue.
    /// </summary>
    public ArrayQueue() {
      _rig = new ArrayRig<T>();
      Empty = true;
    }
    
    /// <summary>
    /// Start with the content of an array.
    /// </summary>
    public ArrayQueue(T[] collection) {
      _rig = new ArrayRig<T>(collection);
      Empty = collection.Length == 0;
    }
    
    /// <summary>
    /// Start with the content of an array rig.
    /// </summary>
    public ArrayQueue(ArrayRig<T> rig) {
      _rig = rig;
      Empty = _rig.Count == 0;
    }
    
    /// <summary>
    /// Dispose of the array queue and the underlying array rig.
    /// </summary>
    public void Dispose() {
      _rig.Dispose();
      _rig = null;
      Count = -1;
    }
    
    /// <summary>
    /// Move to the next queued item.
    /// </summary>
    public bool Next() {
      
      // are there items? no, return negative
      if(Empty) return false;
      
      // get the next item
      Current = _rig[_index];
      
      // decrement the count
      --Count;
      
      // is the queue now empty?
      if(Count == 0) {
        
        // yes, quit
        Empty = true;
        _rig.Count = _index = _loopbackEnd = 0;
        _loopbackActive = _loopbackStarted = false;
        return true;
        
      }
      
      // increment the index
      ++_index;
      
      // is the loopback currently being iterated?
      if(_loopbackActive) {
        
        // has the end of the current loopback collection been reached?
        if(_index == _loopbackEnd) {
          
          // yes, return to the loopback start
          _index = _loopbackStart;
          _loopbackEnd = 0;
          _loopbackActive = false;
          
        }
        
      } else {
        
        // has the loopback collection been started?
        if(_loopbackStarted) {
          
          // has the loopback starting index been reached?
          if(_index == _loopbackStart) {
            
            // yes, start the loopback
            _index = 0;
            _loopbackActive = true;
            
          }
          
        } else if(_index == _rig.Count) {
          
          // the end of the collection has been reached
          _rig.Count = _index = 0;
          // the queue is empty
          Empty = true;
          
        }
      }
      
      return true;
    }
    
    /// <summary>
    /// Cause the next item to be moved to but not dequeued.
    /// </summary>
    public bool Peek() {
      // are there items? no, return negative
      if(Empty) return false;
      
      // get the next item
      Current = _rig[_index];
      
      return true;
    }
    
    /// <summary>
    /// Add an item to the end of the queue.
    /// </summary>
    public void Enqueue(T item) {
      ++Count;
      
      // not empty
      Empty = false;
      
      // has the loopback been set?
      if(_loopbackStarted) {
        
        // yes, has the loopback been filled?
        if(_loopbackFilled) {
          
          // yes, add the item to the end of the collection
          _rig.Add(item);
          
        } else {
          
          // no, set the item at the loopback index and increment
          _rig[_loopbackEnd] = item;
          ++_loopbackEnd;
          _loopbackFilled = _loopbackEnd == _loopbackStart;
          
        }
      } else {
        // is the queue at a starting position?
        if(_index == 0) {
          
          // yes, add the item to the rig
          _rig.Add(item);
          
        } else {
          
          // no, start the loopback collection
          
          _loopbackStarted = true;
          _loopbackStart = _rig.Count;
          
          _rig[_loopbackEnd] = item;
          ++_loopbackEnd;
          
          _loopbackFilled = _loopbackEnd == _loopbackStart;
        }
      }
    }
    
    /// <summary>
    /// Add a collection to the end of the queue.
    /// </summary>
    public void Enqueue(T[] collection) {
      
      Count += collection.Length;
      // should the rig be resized?
      if(_rig.Capacity < Count * 2) {
        _rig.SetCapacity(Count);
      }
      
      // iterate and add the items in the collection.
      foreach(T item in collection) {
      
        // has the loopback been set?
        if(_loopbackStarted) {
          
          // yes, has the loopback been filled?
          if(_loopbackFilled) {
            // yes, add the item to the end of the collection
            _rig.Add(item);
          } else {
            // no, set the item at the loopback index and increment
            _rig[_loopbackEnd] = item;
            ++_loopbackEnd;
            _loopbackFilled = _loopbackEnd == _loopbackStart;
          }
          
        } else {
          // is the queue at a starting position?
          if(_index == 0) {
            
            // yes, just add the item
            _rig.Add(item);
            // not empty
            Empty = false;
            
          } else {
            
            // no, start the loopback collection
            
            _loopbackStarted = true;
            _loopbackStart = _rig.Count;
            
            _rig[_loopbackEnd] = item;
            ++_loopbackEnd;
            
            _loopbackFilled = _loopbackEnd == _loopbackStart;
            
          }
        }
      }
    }
    
    /// <summary>
    /// Add a collection to the end of the queue.
    /// </summary>
    public void Enqueue(ArrayRig<T> collection) {
      
      Count += collection.Count;
      // should the rig be resized?
      if(_rig.Capacity < Count * 2) {
        _rig.SetCapacity(Count);
      }
      
      // iterate and add the items in the collection.
      foreach(T item in collection) {
      
        // has the loopback been set?
        if(_loopbackStarted) {
          
          // yes, has the loopback been filled?
          if(_loopbackFilled) {
            // yes, add the item to the end of the collection
            _rig.Add(item);
          } else {
            // no, set the item at the loopback index and increment
            _rig[_loopbackEnd] = item;
            ++_loopbackEnd;
            _loopbackFilled = _loopbackEnd == _loopbackStart;
          }
          
        } else {
          // is the queue at a starting position?
          if(_index == 0) {
            
            // yes, just add the item
            _rig.Add(item);
            // not empty
            Empty = false;
            
          } else {
            
            // no, start the loopback collection
            
            _loopbackStarted = true;
            _loopbackStart = _rig.Count;
            
            _rig[_loopbackEnd] = item;
            ++_loopbackEnd;
            
            _loopbackFilled = _loopbackEnd == _loopbackStart;
            
          }
        }
      }
    }
    
    
    /// <summary>
    /// Enumerable implementation.
    /// </summary>
    public IEnumerator<T> GetEnumerator() {
      return new Enumerator(this);
    }
    
    /// <summary>
    /// Enumerable implementation.
    /// </summary>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this.GetEnumerator();
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Enumerator for an ArrayQueue collection.
    /// </summary>
    public class Enumerator : IEnumerator<T> {
      
      //---------------------------------//
      
      /// <summary>
      /// The current item in the ArrayQueue.
      /// </summary>
      public T Current { get { return _queue.Current; } }
      
      object System.Collections.IEnumerator.Current { get { return this.Current; } }
      
      //---------------------------------//
      
      /// <summary>
      /// The queue being enumerated.
      /// </summary>
      private ArrayQueue<T> _queue;
      
      //---------------------------------//
      
      /// <summary>
      /// Construct a new enumerator for the specified array queue.
      /// </summary>
      public Enumerator(ArrayQueue<T> queue) {
        _queue = queue;
      }
      
      public bool MoveNext() {
        return _queue.Next();
      }
      
      /// <summary>
      /// The ArrayQueue enumerator cannot be reset.
      /// </summary>
      public void Reset() {
      }
      
      /// <summary>
      /// The enumerator for the array queue is not disposed of.
      /// Dispose of the collection instead.
      /// </summary>
      public void Dispose() {
      }
      
      //---------------------------------//
      
    }
    
  }
  
}
