/*
 * User: Joshua
 * Date: 22/10/2016
 * Time: 2:11 PM
 */
using System;

using Efz.Threading;

namespace Efz.Collections {
  
  /// <summary>
  /// A queue structure that uses a flexible array to store items. This structure
  /// is good for large numbers of items accessed like a queue.
  /// Not threadsafe.
  /// </summary>
  public class Band<T> : IDisposable {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The current item in the band.
    /// </summary>
    public T Current;
    /// <summary>
    /// Is the current collection empty?
    /// </summary>
    public bool Empty;
    /// <summary>
    /// Number of items in the collection.
    /// </summary>
    public int Count;
    
    /// <summary>
    /// Current capacity of the array.
    /// </summary>
    public int Capacity;
    
    //-------------------------------------------//
    
    /// <summary>
    /// The array used for item storage.
    /// </summary>
    private T[] _array;
    
    /// <summary>
    /// The current index in the queue.
    /// </summary>
    private int _index;
    
    /// <summary>
    /// Is the minimum index now zero?
    /// </summary>
    private bool _minIsZero;
    /// <summary>
    /// The minimum index.
    /// </summary>
    private int _min;
    /// <summary>
    /// The maximum index.
    /// </summary>
    private int _max;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Construct a new empty band. Optionally specifying an expected capacity count (must be at least 1)
    /// </summary>
    public Band(int capacity = 40) {
      Capacity = capacity;
      
      _index = _min = _max = (int)(Capacity * .5);
      _array = new T[Capacity];
      
      Empty = true;
      _minIsZero = false;
    }
    
    /// <summary>
    /// Dispose of the bands resources.
    /// </summary>
    public void Dispose() {
      _array = null;
    }
    
    /// <summary>
    /// Clear the band of items.
    /// </summary>
    public void Clear() {
      Empty = true;
      _index = _min = _max = (int)(Capacity * .5);
      _minIsZero = false;
    }
    
    /// <summary>
    /// Add an item to the end of the band.
    /// </summary>
    public void Enqueue(T item) {
      
      // was the collection empty?
      if(Empty) {
        // yes, just set the index
        _array[_index] = item;
        ++_index;
        ++_max;
        ++Count;
        Empty = false;
        return;
      }
      
      // does the current index match the max?
      if(_index == _max) {
        _array[_index] = item;
        ++_index;
        ++_max;
        
        // yes, is the array full?
        if(Capacity.Equals(_max)) {
          // yes, resize
          T[] last = _array;
          // double array size
          Capacity += Capacity;
          _array = new T[Capacity];
          Array.Copy(last, _array, _max);
        }
        
        ++Count;
        return;
      }
      
      // has the minimum index become '0'?
      if(_minIsZero) {
        
        ++_max;
        // yes, is the array full?
        if(Capacity.Equals(_max)) {
          // yes, resize
          T[] last = _array;
          // double array size
          Capacity += Capacity;
          _array = new T[Capacity];
          Array.Copy(last, _array, _max);
        }
        
        _array[_index] = item;
        ++_index;
        
      } else {
        
        // no, is the index closer to the max index?
        if(_index - _min > _max - _index) {
          
          // yes, is the array full?
          if(Capacity.Equals(_max + 1)) {
            
            T[] last = _array;
            // double array size
            Capacity += Capacity;
            _array = new T[Capacity];
            Array.Copy(last, _array, _max);
          }
          
          // move all items up from the current index
          int current = _max;
          _array[current + 1] = _array[current];
          
          while(--current >= _index) {
            
            _array[current + 1] = _array[current];
            
          }
          
          ++_max;
          
          _array[_index] = item;
          ++_index;
          
        } else {
          
          // no, move all items down from the current index
          int current = _min;
          _array[current - 1] = _array[current];
          
          while(++current <= _index) {
            
            _array[current - 1] = _array[current];
            
          }
          
          _minIsZero = --_min == 0;
          
          _array[_index] = item;
          
        }
      }
      
      
      ++Count;
      
    }
    
    /// <summary>
    /// Move to the next queued item.
    /// </summary>
    public bool Next() {
      
      // if empty - return false
      if(Empty) return false;
      
      if(_index == _max) _index = _min;
      else ++_index;
      Current = _array[_index];
      
      return true;
      
    }
    
    /// <summary>
    /// Remove the current item in the collection.
    /// </summary>
    public void RemoveCurrent() {
      
      // early out if empty
      if(Empty) return;
      
      // is the band now empty?
      if(--Count == 0) {
        // yes, just flip the flag
        Empty = true;
        _min = _max = _index;
      } else {
        
        // is the index closer to the max index?
        if(_index - _min > _max - _index) {
          
          // yes, move all items down from the current index
          int current = _index;
          _array[current] = _array[current + 1];
          while(++current < _max) {
            _array[current] = _array[current + 1];
          }
          --_index;
          --_max;
          
        } else {
          
          // no, move all items up to the current index
          int current = _min;
          _array[current] = _array[current - 1];
          while(++current <= _index) {
            _array[current] = _array[current - 1];
          }
          
          _minIsZero = ++_min == 0;
          
        }
        
      }
      
    }
    
    //-------------------------------------------//
  
  
  }
  
}
