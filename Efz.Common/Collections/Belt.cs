using System;

using Efz.Tools;

namespace Efz.Collections {
  
  /// <summary>
  /// A linked list of items that loops. Facilitates removal of items during dequeueing.
  /// </summary>
  public class Belt<T> {
        
    //-------------------------------------------//
    
    /// <summary>
    /// The current position in the chain.
    /// </summary>
    public Link<T> LinkCurrent;
    /// <summary>
    /// The link preceeding the current link. Used to remove links.
    /// </summary>
    public Link<T> LinkLast;
    
    /// <summary>
    /// The current item.
    /// </summary>
    public T Current;
    /// <summary>
    /// Is the belt empty?
    /// </summary>
    public bool Empty;
    /// <summary>
    /// Number of items in the belt.
    /// </summary>
    public int Count;
    
    /// <summary>
    /// If the belt has just completed a loop.
    /// </summary>
    public bool Loop;
    /// <summary>
    /// Current index of the loop.
    /// </summary>
    public int Index;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Array of items to be removed.
    /// </summary>
    private ArrayRig<T> _remove;
    /// <summary>
    /// Are there items to be removed?
    /// </summary>
    private bool _removeActive;
    /// <summary>
    /// Track each loop?
    /// </summary>
    private bool _loopSignal;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new Belt. Optional loop signal causes the 'Loop' flag to be updated
    /// each loop cycle of the belt.
    /// </summary>
    public Belt() {
      Empty = true;
      Loop = true;
      Count = 0;
      _remove = new ArrayRig<T>();
    }
    
    /// <summary>
    /// Initialize a new Belt. Optional loop signal will flag each loop completion and return false on loops.
    /// </summary>
    public Belt(bool loopSignal) {
      _loopSignal = loopSignal;
      Empty = true;
      Loop = true;
      Count = 0;
      _remove = new ArrayRig<T>();
    }
    
    /// <summary>
    /// Clear the content of the chain.
    /// </summary>
    public void Clear() {
      Empty = true;
      Count = 0;
      LinkLast = LinkCurrent = null;
    }
    
    /// <summary>
    /// Move to the next item in the belt, returning whether there was an item.
    /// </summary>
    public bool Next() {
      
      // early out if empty
      if(Empty) return false;
      
      // remove items as required
      if(_removeActive) {
        while(_remove.RemoveCheckQuick(LinkCurrent.Item)) {
          
          // remove current
          LinkLast.Next = LinkCurrent = LinkCurrent.Next;
          
          // update remove collection state
          _removeActive = _remove.Count != 0;
          
          // is this empty
          if(--Count == 0) {
            _removeActive = false;
            Empty = Loop = true;
            return false;
          }
          
        }
      }
      
      // if required, flag a loop of the belt being completed -
      // this is done with a 'virtual' index
      if(_loopSignal) {
        ++Index;
        if(Index > Count) {
          Loop = true;
          Index = 0;
        } else {
          Loop = false;
        }
      }
      
      // step forward
      LinkLast = LinkCurrent;
      LinkCurrent = LinkCurrent.Next;
      
      // set the current item
      Current = LinkCurrent.Item;
      
      return true;
    }
    
    /// <summary>
    /// Move to the next item in the belt, returning whether there was an item.
    /// </summary>
    public bool Next(out T next) {
      
      // early out if empty
      if(Empty) {
        next = default(T);
        return false;
      }
      
      // remove items as required
      if(_removeActive) {
        while(_remove.RemoveCheckQuick(LinkCurrent.Item)) {
          // remove current
          LinkLast.Next = LinkCurrent = LinkCurrent.Next;
          // update remove collection state
          _removeActive = _remove.Count != 0;
          // is this empty
          if(--Count == 0) {
            Empty = Loop = true;
            next = default(T);
            return false;
          }
        }
      }
      
      // if required, flag a loop of the belt being completed -
      // this is done with a 'virtual' index
      if(_loopSignal) {
        ++Index;
        if(Index > Count) {
          Loop = true;
          Index = 0;
        } else {
          Loop = false;
        }
      }
      
      // step forward
      LinkLast = LinkCurrent;
      LinkCurrent = LinkCurrent.Next;
      
      // set the current item
      next = Current = LinkCurrent.Item;
      
      return true;
    }
    
    /// <summary>
    /// Remove the current item in the belt. Prefered removal method.
    /// </summary>
    public void RemoveCurrent() {
      
      // skip empty
      if(Empty) return;
      
      // check if empty now
      if(--Count == 0) {
        Empty = Loop = true;
        LinkLast = LinkCurrent = null;
      } else {
        // remove current link
        LinkLast.Next = LinkCurrent = LinkCurrent.Next;
      }
      
    }
    
    /// <summary>
    /// Remove the specified item from the belt.
    /// </summary>
    public void Remove(T item) {
      if(Empty) return;
      _remove.Add(item);
      _removeActive = true;
    }
    
    /// <summary>
    /// Checks whether the item exists in the belt.
    /// Fairly long operation. To be avoided where possible.
    /// </summary>
    public bool Contains(T item) {
      // early out if empty
      if(Empty) return false;
      
      // start with the current link
      Link<T> check = LinkCurrent;
      Link<T> checkLast = LinkLast;
      
      // loop the belt
      while(check != LinkLast) {
        
        // is the current link to be removed?
        if(_removeActive && _remove.RemoveCheckQuick(check.Item)) {
          
          // yes, remove the current link
          checkLast.Next = check = check.Next;
          
          // update remove collection state
          _removeActive = _remove.Count != 0;
          
          // is this collection empty?
          if(--Count == 0) {
            Empty = Loop = true;
            return false;
          }
          
        } else if(check.Item.Equals(item)) {
          return true;
        }
        
        checkLast = check;
        check = check.Next;
        
      }
      
      // check the final (or only) item
      // is the current link to be removed?
      if(_removeActive && _remove.RemoveCheckQuick(check.Item)) {
        
        // yes, remove the current link
        checkLast.Next = check = check.Next;
        
        // update remove collection state
        _removeActive = _remove.Count != 0;
        
        // is this collection empty?
        if(--Count == 0) {
          Empty = Loop = true;
        }
        
        return false;
        
      }
      
      return check.Item.Equals(item);
    }
    
    /// <summary>
    /// Add an item to the end of the belt.
    /// </summary>
    public void Enqueue(T item) {
      ++Count;
      
      if(Empty) {
        
        Loop = false;
        Empty = false;
        
        // enqueue single item
        LinkCurrent = new Link<T>(item);;
        LinkCurrent.Next = LinkLast = LinkCurrent;
        
      } else {
        
        // the previous link steps forward once
        LinkLast = LinkLast.Next = new Link<T>(item, LinkCurrent);
        //LinkLast = LinkCurrent;
        // replace the current link with the new link
        //LinkCurrent = new Link<T>(item, LinkCurrent.Next);
        
      }
    }
    
    //-------------------------------------------//
    
  }

}