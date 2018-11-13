/*
 * User: Joshua
 * Date: 26/06/2016
 * Time: 3:15 PM
 */

using System;

namespace Efz.Tools {
  
  public class Link<T> {
    
    //-------------------------------------------//
    
    public T Item;
    public Link<T> Next;
    
    //-------------------------------------------//
    
    public Link(T item) {
      Item = item;
    }
    
    public Link(T item, Link<T> next) {
      Item = item;
      Next = next;
    }
    
    //-------------------------------------------//
    
  }
  
}
