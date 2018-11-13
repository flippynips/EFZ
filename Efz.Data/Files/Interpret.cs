/*
 * User: Bob
 * Date: 26/11/2016
 * Time: 13:02
 */
using System;

namespace Efz.Data.Files {
  
  /// <summary>
  /// Interpret bytes as another type, potentially over a number of reads.
  /// </summary>
  public abstract class Interpret {
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// The current number of prepared bytes in the prepared array.
    /// </summary>
    protected int _bufferCount;
    /// <summary>
    /// Current index within the prepared bytes collection.
    /// </summary>
    protected int _bufferIndex;
    /// <summary>
    /// The length target of prepared bytes.
    /// </summary>
    protected int _bufferTarget;
    /// <summary>
    /// The length of the prepared byte array.
    /// </summary>
    protected int _bufferCapacity;
    /// <summary>
    /// The prepared collection of bytes.
    /// </summary>
    protected byte[] _bufferBytes;
    
    /// <summary>
    /// Should a certain number of bytes be prepared?
    /// </summary>
    protected bool _buffer;
    
    //----------------------------------//
    
    protected Interpret() {
      _bufferCapacity = Global.BufferSizeLocal;
      _bufferBytes = BufferCache.Get(_bufferCapacity);
    }
    
    /// <summary>
    /// Clear the interpreter of the current read.
    /// </summary>
    public virtual void Clear() {
      Reset();
      _bufferCount = 0;
      _bufferIndex = 0;
      _bufferTarget = 0;
      _buffer = false;
    }
    
    /// <summary>
    /// Read a single block of data, interpreting the bytes as a meaningful type.
    /// </summary>
    public virtual bool Read(byte[] bytes, int offset, int length) {
      
      // reset local buffers
      Reset();
      
      // are some required bytes being prepared?
      if(_buffer) {
        
        bool result = false;
        
        // iterate while the buffer requirement is filled by the new bytes
        while(_bufferIndex + length - offset >= _bufferTarget) {
          
          // iterate while the prepared buffer contains enough bytes
          while(_bufferCount - _bufferIndex > _bufferTarget) {
            
            // send the prepared bytes
            int target = _bufferTarget;
            result |= Next(_bufferBytes, _bufferIndex, target);
            _bufferIndex += target;
            
          }
          
          // any prepared bytes remaining?
          if(_bufferCount > _bufferIndex) {
            
            // yes, copy the bytes to the start of the prepared array
            Micron.CopyMemory(_bufferBytes, _bufferIndex, _bufferBytes, 0, _bufferCount - _bufferIndex);
            // update the prepared count
            _bufferCount -= _bufferIndex;
            _bufferIndex = 0;
            
          } else {
            
            // no, copy any new bytes to the start of the buffer
            Micron.CopyMemory(bytes, offset, _bufferBytes, 0, length - offset);
            // reset the buffer parameters
            _bufferIndex = _bufferCount = 0;
            
            break;
            
          }
          
          // append new bytes to the buffer
          Micron.CopyMemory(bytes, offset, _bufferBytes, _bufferIndex, _bufferCapacity - _bufferIndex);
          offset += _bufferCount - _bufferIndex;
          
        }
        
        // iterate new buffer
        while(length - offset > _bufferTarget) {
          
          result |= Next(bytes, offset, _bufferTarget);
          offset += _bufferTarget;
        }
        
        // copy the remaining new bytes to the buffer
        Micron.CopyMemory(bytes, offset, _bufferBytes, _bufferIndex, length);
        
        return result;
      }
      
      // any bytes in the buffer?
      if(_bufferIndex < _bufferCount) {
        
        // yes, will the new bytes fit in the buffer?
        if(length > _bufferCapacity - _bufferCount) {
          // no, resize the buffer
          Array.Resize(ref _bufferBytes, _bufferCapacity + length);
        }
        
        // copy the new bytes into the buffer
        Micron.CopyMemory(bytes, offset, _bufferBytes, _bufferIndex, length);
        
        int index = _bufferIndex;
        _bufferIndex += _bufferCount;
        
        // read the buffer bytes
        return Next(_bufferBytes, index, _bufferCount);
      }
      
      // read all bytes passed
      return Next(bytes, offset, length);
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Read the next set of bytes.
    /// </summary>
    protected abstract bool Next(byte[] bytes, int offset, int length);
    /// <summary>
    /// Refresh the Current instance.
    /// </summary>
    protected abstract void Reset();
    
    /// <summary>
    /// Prepare the specified number of bytes. This ensures that at least the specified
    /// number of bytes are sent to the 'Next' method each call.
    /// </summary>
    protected void BufferCount(int length) {
      
      if(length == 0) {
        _buffer = false;
      } else {
        _buffer = true;
        _bufferTarget = length;
        if(_bufferBytes == null || _bufferBytes.Length < _bufferTarget) _bufferBytes = BufferCache.Get(length);
      }
      
    }
    
    /// <summary>
    /// Append the specified bytes to the next read.
    /// </summary>
    protected void Buffer(byte[] bytes, int offset, int length) {
      
      _buffer = true;
      
      // does the buffer have room for the specified bytes?
      if(length > _bufferTarget - _bufferIndex) {
        // no, resize the prepared byte array
        _bufferBytes = BufferCache.Get(_bufferTarget + length);
      }
      
      // copy the byte buffer into the prepared collection
      Micron.CopyMemory(bytes, offset, _bufferBytes, _bufferIndex, length);
      // increment the index
      _bufferIndex += length;
      
    }
    
  }
  
}
