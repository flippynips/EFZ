using System;

using Efz.Threading;
using Efz.Tools;

namespace Efz.Collections {
  
  /// <summary>
  /// Threadsafe queue with concurrent reading and writing.
  /// </summary>
  public class SafeQueue<T> : IDisposable {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Current number of items in the rig.
    /// </summary>
    public int Count {
      get { return _queues.A.Count + _queues.B.Count; }
    }
    
    /// <summary>
    /// The current item in the queue.
    /// </summary>
    public T Current { get; protected set; }
    
    //-------------------------------------------//
    
    /// <summary>
    /// The collections this rig modifies.
    /// </summary>
    private readonly Flipper<ArrayQueue<T>> _queues;
    /// <summary>
    /// The locks used when accessing the queue collections.
    /// </summary>
    private Flipper<Lock> _locks;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initializes a new ArrayRig with default starting size 8.
    /// </summary>
    public SafeQueue() {
      _queues = new Flipper<ArrayQueue<T>>(new ArrayQueue<T>(), new ArrayQueue<T>());
      _locks = new Flipper<Lock>(new Lock(), new Lock());
    }
    
    /// <summary>
    /// Initializes with the content of the specified array.
    /// </summary>
    public SafeQueue(T[] array, int count = -1) {
      _queues = new Flipper<ArrayQueue<T>>(new ArrayQueue<T>(new ArrayRig<T>(array, count)), new ArrayQueue<T>());
      _locks = new Flipper<Lock>(new Lock(), new Lock());
    }
    
    
    /// <summary>
    /// Dispose of the safe rig.
    /// </summary>
    public void Dispose() {
      _queues.A.Dispose();
      _queues.B.Dispose();
      _locks = null;
    }
    
    /// <summary>
    /// Move to the next item if available.
    /// </summary>
    public bool Dequeue() {
      _locks.A.Take();
      if(_queues.A.Next()) {
        Current = _queues.A.Current;
        _locks.A.Release();
        return true;
      }
      _locks.B.Take();
      _queues.Flip();
      _locks.B.Release();
      if(_queues.A.Next()) {
        Current = _queues.A.Current;
        _locks.A.Release();
        return true;
      }
      _locks.A.Release();
      return false;
    }
    
    /// <summary>
    /// Dequeue an item from the queue if possible.
    /// </summary>
    public bool Dequeue(out T item) {
      _locks.A.Take();
      if(_queues.A.Next()) {
        Current = item = _queues.A.Current;
        _locks.A.Release();
        return true;
      }
      _locks.B.Take();
      _queues.Flip();
      _locks.B.Release();
      if(_queues.A.Next()) {
        Current = item = _queues.A.Current;
        _locks.A.Release();
        return true;
      }
      _locks.A.Release();
      item = default(T);
      return false;
    }
    
    /// <summary>
    /// Enqueue an item.
    /// </summary>
    public void Enqueue(T item) {
      _locks.B.Take();
      _queues.B.Enqueue(item);
      _locks.B.Release();
    }
    
    /// <summary>
    /// Enqueue a collection.
    /// </summary>
    public void Enqueue(T[] collection) {
      _locks.B.Take();
      _queues.B.Enqueue(collection);
      _locks.B.Release();
    }
    
    /// <summary>
    /// Enqueue a collection.
    /// </summary>
    public void Enqueue(ArrayRig<T> collection) {
      _locks.B.Take();
      _queues.B.Enqueue(collection);
      _locks.B.Release();
    }
    
    //-------------------------------------------//
    
  }

}