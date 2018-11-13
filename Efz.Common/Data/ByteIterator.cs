/*
 * User: FloppyNipples
 * Date: 13/03/2017
 * Time: 23:10
 */
using System;

namespace Efz.Data {
  
  /// <summary>
  /// Iteration of a byte array that automatically resizes.
  /// </summary>
  public class ByteIterator {
    
    //----------------------------------//
    
    /// <summary>
    /// Move to the next byte array configuration and return a
    /// string representation of the byte array.
    /// </summary>
    public string NextStr {
      get {
        if(_id[0] == 255) {
          // iterate the id bytes
          for(int i = 1; i < _id.Length; ++i) {
            if(_id[i] == 255) {
              if(i == _id.Length-1) {
                Array.Resize(ref _id, _id.Length + 1);
                _id[_id.Length-1] = 1;
                while(--i >= 0) {
                  _id[i] = 0;
                }
                break;
              }
            } else {
              ++_id[i];
              while(--i >= 0) {
                _id[i] = 0;
              }
              break;
            }
          }
        } else {
          ++_id[0];
        }
        return Convert.ToBase64String(_id);
      }
    }
    
    /// <summary>
    /// Iterate and move to the next byte collection.
    /// </summary>
    public byte[] NextBytes {
      get {
        if(_id[0] == 255) {
          // iterate the id bytes
          for(int i = 1; i < _id.Length; ++i) {
            if(_id[i] == 255) {
              if(i == _id.Length-1) {
                Array.Resize(ref _id, _id.Length + 1);
                _id[_id.Length-1] = 1;
                while(--i >= 0) {
                  _id[i] = 0;
                }
                break;
              }
            } else {
              ++_id[i];
              while(--i >= 0) {
                _id[i] = 0;
              }
              break;
            }
          }
        } else {
          ++_id[0];
        }
        return _id;
      }
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Inner byte array.
    /// </summary>
    protected byte[] _id;
    
    //----------------------------------//
    
    public ByteIterator(byte[] id) {
      _id = id == null || id.Length == 0 ? new byte[1] : id;
    }
    
    public ByteIterator(int byteCount) {
      if(byteCount < 1) byteCount = 1;
      _id = new byte[byteCount];
    }
    
    public ByteIterator() {
      _id = new byte[1];
    }
    
    //----------------------------------//
    
  }
  
}
