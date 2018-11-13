using System.Runtime.InteropServices;

namespace Efz.Tools {
  
  public struct Size<T> {
    
    //-------------------------------------------//
    
    public uint Get {
      get {
        if( set ) {
          return size;
        }
        set = true;
        return size = (uint)Marshal.SizeOf(typeof(T));
      }
    }
    
    //-------------------------------------------//
    
    private bool set;
    private uint size;
    
    //-------------------------------------------//
    
    static public implicit operator uint( Size<T> _size ) {
      return _size.Get;
    }
    
    //-------------------------------------------//
    
  }
  
}
