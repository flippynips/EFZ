using System;

namespace Efz.Collections {
  
  /// <summary>
  /// A queue that can accept any generic type. Useful for there are a few unique types possible in a queue.
  /// </summary>
  public class TypeQueue<A,B> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Defined types of items.
    /// </summary>
    public enum NextType { A, B }
    
    /// <summary>
    /// If the queue is empty.
    /// </summary>
    public bool Empty { get { return _queue.Empty; } }
    
    /// <summary>
    /// The type and associated collection of the next item.
    /// </summary>
    public NextType CurrentType;
    /// <summary>
    /// Collection of items of type A.
    /// </summary>
    public Queue<A> ItemsA;
    /// <summary>
    /// Collection of items of type B.
    /// </summary>
    public Queue<B> ItemsB;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Queue of flags indicating whether the next item is of type A.
    /// </summary>
    protected Queue<bool> _queue;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new TypeQueue.
    /// </summary>
    public TypeQueue() {
      ItemsA = new Queue<A>();
      ItemsB = new Queue<B>();
    }
    
    /// <summary>
    /// Enqueue the specified item.
    /// </summary>
    public void Enqueue(A item) {
      ItemsA.Enqueue(item);
      _queue.Enqueue(true);
    }
    
    /// <summary>
    /// Enqueue the specified item.
    /// </summary>
    public void Enqueue(B item) {
      ItemsB.Enqueue(item);
      _queue.Enqueue(false);
      
    }
    
    /// <summary>
    /// Move to and return whether there is another item.
    /// </summary>
    public bool Next() {
      if(_queue.Dequeue()) {
        if(_queue.Current) {
          CurrentType = NextType.A;
          ItemsA.Dequeue();
        } else {
          CurrentType = NextType.B;
          ItemsB.Dequeue();
        }
        return true;
      }
      return false;
    }
    
    //-------------------------------------------//
    
    
  }
  
  /// <summary>
  /// A queue that can accept any generic type. Useful for there are a few unique types possible in a queue.
  /// </summary>
  public class TypeQueue<A,B,C> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Defined types of items.
    /// </summary>
    public enum NextType { A, B, C }
    
    /// <summary>
    /// Get the number of elements in the queue.
    /// </summary>
    public int Count { get { return _queue.Count; } }
    /// <summary>
    /// If the queue is empty.
    /// </summary>
    public bool Empty { get { return _queue.Empty; } }
    
    /// <summary>
    /// The type and associated collection of the next item.
    /// </summary>
    public NextType CurrentType;
    /// <summary>
    /// Collection of items of type A.
    /// </summary>
    public Queue<A> ItemsA;
    /// <summary>
    /// Collection of items of type B.
    /// </summary>
    public Queue<B> ItemsB;
    /// <summary>
    /// Collection of items of type C.
    /// </summary>
    public Queue<C> ItemsC;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Queue of flags indicating which type represents the next item.
    /// </summary>
    protected Queue<NextType> _queue;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new TypeQueue.
    /// </summary>
    public TypeQueue() {
      ItemsA = new Queue<A>();
      ItemsB = new Queue<B>();
      ItemsC = new Queue<C>();
    }
    
    /// <summary>
    /// Enqueue the specified item.
    /// </summary>
    public void Enqueue(A item) {
      ItemsA.Enqueue(item);
      _queue.Enqueue(NextType.A);
    }
    
    /// <summary>
    /// Enqueue the specified item.
    /// </summary>
    public void Enqueue(B item) {
      ItemsB.Enqueue(item);
      _queue.Enqueue(NextType.B);
    }
    
    /// <summary>
    /// Enqueue the specified item.
    /// </summary>
    public void Enqueue(C item) {
      ItemsC.Enqueue(item);
      _queue.Enqueue(NextType.C);
    }
    
    /// <summary>
    /// Move to and return whether there is another item.
    /// </summary>
    public bool Next() {
      if(_queue.Dequeue()) {
        CurrentType = _queue.Current;
        switch(CurrentType) {
          case NextType.A:
            ItemsA.Dequeue();
            break;
          case NextType.B:
            ItemsB.Dequeue();
            break;
          case NextType.C:
            ItemsC.Dequeue();
            break;
        }
        return true;
      }
      return false;
    }
    
    //-------------------------------------------//
    
    
  }
  
  /// <summary>
  /// A queue that can accept any generic type. Useful for there are a few unique types possible in a queue.
  /// </summary>
  public class TypeQueue<A,B,C,D> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Defined types of items.
    /// </summary>
    public enum NextType { A, B, C, D }
    
    /// <summary>
    /// If the queue is empty.
    /// </summary>
    public bool Empty { get { return _queue.Empty; } }
    
    /// <summary>
    /// The type and associated collection of the next item.
    /// </summary>
    public NextType CurrentType;
    /// <summary>
    /// Collection of items of type A.
    /// </summary>
    public Queue<A> ItemsA;
    /// <summary>
    /// Collection of items of type B.
    /// </summary>
    public Queue<B> ItemsB;
    /// <summary>
    /// Collection of items of type C.
    /// </summary>
    public Queue<C> ItemsC;
    /// <summary>
    /// Collection of items of type D.
    /// </summary>
    public Queue<D> ItemsD;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Queue of flags indicating which type represents the next item.
    /// </summary>
    protected Queue<NextType> _queue;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new TypeQueue.
    /// </summary>
    public TypeQueue() {
      ItemsA = new Queue<A>();
      ItemsB = new Queue<B>();
      ItemsC = new Queue<C>();
      ItemsD = new Queue<D>();
    }
    
    /// <summary>
    /// Enqueue the specified item.
    /// </summary>
    public void Enqueue(A item) {
      ItemsA.Enqueue(item);
      _queue.Enqueue(NextType.A);
    }
    
    /// <summary>
    /// Enqueue the specified item.
    /// </summary>
    public void Enqueue(B item) {
      ItemsB.Enqueue(item);
      _queue.Enqueue(NextType.B);
    }
    
    /// <summary>
    /// Enqueue the specified item.
    /// </summary>
    public void Enqueue(C item) {
      ItemsC.Enqueue(item);
      _queue.Enqueue(NextType.C);
    }
    
    /// <summary>
    /// Enqueue the specified item.
    /// </summary>
    public void Enqueue(D item) {
      ItemsD.Enqueue(item);
      _queue.Enqueue(NextType.D);
    }
    
    /// <summary>
    /// Move to and return whether there is another item.
    /// </summary>
    public bool Next() {
      if(_queue.Dequeue()) {
        CurrentType = _queue.Current;
        switch(CurrentType) {
          case NextType.A:
            ItemsA.Dequeue();
            break;
          case NextType.B:
            ItemsB.Dequeue();
            break;
          case NextType.C:
            ItemsC.Dequeue();
            break;
          case NextType.D:
            ItemsD.Dequeue();
            break;
        }
        return true;
      }
      return false;
    }
    
    //-------------------------------------------//
    
    
  }
  
  /// <summary>
  /// A queue that can accept any generic type. Useful for there are a few unique types possible in a queue.
  /// </summary>
  public class TypeQueue<A,B,C,D,E> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Defined types of items.
    /// </summary>
    public enum NextType { A, B, C, D, E }
    
    /// <summary>
    /// If the queue is empty.
    /// </summary>
    public bool Empty { get { return _queue.Empty; } }
    
    /// <summary>
    /// The type and associated collection of the next item.
    /// </summary>
    public NextType CurrentType;
    /// <summary>
    /// Collection of items of type A.
    /// </summary>
    public Queue<A> ItemsA;
    /// <summary>
    /// Collection of items of type B.
    /// </summary>
    public Queue<B> ItemsB;
    /// <summary>
    /// Collection of items of type C.
    /// </summary>
    public Queue<C> ItemsC;
    /// <summary>
    /// Collection of items of type D.
    /// </summary>
    public Queue<D> ItemsD;
    /// <summary>
    /// Collection of items of type E.
    /// </summary>
    public Queue<E> ItemsE;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Queue of flags indicating which type represents the next item.
    /// </summary>
    protected Queue<NextType> _queue;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new TypeQueue.
    /// </summary>
    public TypeQueue() {
      ItemsA = new Queue<A>();
      ItemsB = new Queue<B>();
      ItemsC = new Queue<C>();
      ItemsD = new Queue<D>();
      ItemsE = new Queue<E>();
    }
    
    /// <summary>
    /// Enqueue the specified item.
    /// </summary>
    public void Enqueue(A item) {
      ItemsA.Enqueue(item);
      _queue.Enqueue(NextType.A);
    }
    
    /// <summary>
    /// Enqueue the specified item.
    /// </summary>
    public void Enqueue(B item) {
      ItemsB.Enqueue(item);
      _queue.Enqueue(NextType.B);
    }
    
    /// <summary>
    /// Enqueue the specified item.
    /// </summary>
    public void Enqueue(C item) {
      ItemsC.Enqueue(item);
      _queue.Enqueue(NextType.C);
    }
    
    /// <summary>
    /// Enqueue the specified item.
    /// </summary>
    public void Enqueue(D item) {
      ItemsD.Enqueue(item);
      _queue.Enqueue(NextType.D);
    }
    
    /// <summary>
    /// Enqueue the specified item.
    /// </summary>
    public void Enqueue(E item) {
      ItemsE.Enqueue(item);
      _queue.Enqueue(NextType.E);
    }
    
    /// <summary>
    /// Move to and return whether there is another item.
    /// </summary>
    public bool Next() {
      if(_queue.Dequeue()) {
        CurrentType = _queue.Current;
        switch(CurrentType) {
          case NextType.A:
            ItemsA.Dequeue();
            break;
          case NextType.B:
            ItemsB.Dequeue();
            break;
          case NextType.C:
            ItemsC.Dequeue();
            break;
          case NextType.D:
            ItemsD.Dequeue();
            break;
          case NextType.E:
            ItemsE.Dequeue();
            break;
        }
        return true;
      }
      return false;
    }
    
    //-------------------------------------------//
    
    
  }

}