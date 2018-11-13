// Helper class to aid in concurrent
// collection use.

namespace Efz.Tools {
  
  /// <summary>
  /// Helper class for rotating items back and forth. Useful for threading.
  /// </summary>
  public class Flipper<T> where T : new() {
    
    //-------------------------------------------//
    
    public T A;
    public T B;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    public Flipper() {
      A = new T();
      B = new T();
    }
    
    /// <summary>
    /// New flipper with a and b set explicitly.
    /// </summary>
    public Flipper(T a, T b) {
      A = a;
      B = b;
    }
    
    /// <summary>
    /// Flips item a and item b's references.
    /// </summary>
    public void Flip() {
      T c = A;
      A = B;
      B = c;
    }

    //-------------------------------------------//


  }
  
  /// <summary>
  /// Structure for rotating through multiple items.
  /// </summary>
  public class FlipperMulti<T> where T : new() {
        
    //-------------------------------------------//
    
    public T[] items;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new instance of the FlipperMulti class with specified number of items.
    /// </summary>
    public FlipperMulti(int itemNumber) {
      items = new T[itemNumber];
      for(int i = itemNumber-1; i >= 0; --i) {
        items[i] = new T();
      }
    }
    
    /// <summary>
    /// Rotates the items upward one place. item in position 0 is now item n.
    /// </summary>
    public void Flip() {
      T shiftA = default(T);
      T shiftB = items[items.Length-1];
      bool flip = false;
      for(int i = 0; i < items.Length; ++i) {
        if(flip) {
          shiftB = items[i];
          items[i] = shiftA;
        } else {
          shiftA = items[i];
          items[i] = shiftB;
        }
        flip = !flip;
      }
    }

    //-------------------------------------------//


  }
  
}
