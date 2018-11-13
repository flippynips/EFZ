/*
 * User: FloppyNipples
 * Date: 06/05/2017
 * Time: 18:03
 */
using System;

using System.IO;
using Efz.Threading;

namespace Efz.Web {
  
  /// <summary>
  /// Defines a class that represents a resizable circular byte queue.
  /// </summary>
  public class BufferQueue {
    
    //----------------------------------//
    
    /// <summary>
    /// Gets the length of the byte queue
    /// </summary>
    public int Length {
      get { return _count; }
    }
    
    //----------------------------------//
    
    /// <summary>
    /// The index of the first element in the buffer
    /// </summary>
    private int _start;
    /// <summary>
    /// The index of the last element in the buffer
    /// </summary>
    private int _end;
    
    /// <summary>
    /// Number of bytes in the queue.
    /// </summary>
    private volatile int _count;
    
    /// <summary>
    /// Number of contiguous free bytes in the buffer.
    /// </summary>
    private int _freeSection;
    
    /// <summary>
    /// The buffer
    /// </summary>
    private byte[] _buffer;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a new buffer queue.
    /// </summary>
    public BufferQueue() {
      _buffer = BufferCache.Get();
    }
    
    /// <summary>
    /// Clears the byte queue
    /// </summary>
    public void Clear(int size) {
      
      if (size > _count) size = _count;

      if (size == 0) {
        return;
      }

      _start = (_start + size) % _buffer.Length;
      _count -= size;

      if (_count == 0) {
        _start = 0;
        _end = 0;
      }
      
      _freeSection = _buffer.Length - _start;
      
    }

    /// <summary>
    /// Ensures the internal buffer is greater than or equal given size
    /// </summary>
    public void EnsureCapacity(int size) {
      if (size > _buffer.Length) SetCapacity(PowerOf2LargerThan(size));
    }
    
    /// <summary>
    /// Enqueue bytes in the specified stream into the queue. Returns the number of bytes
    /// in the queue.
    /// </summary>
    public int EnqueueGetCount(Stream stream, int length) {
      
      if (_count + length > _buffer.Length) SetCapacity(PowerOf2LargerThan(_count + length));
      
      if (_start < _end) {
        int rightLength = _buffer.Length - _end;
        
        if (rightLength >= length) {
          
          int count;
          while(length > 0) {
            
            count = stream.Read(_buffer, _end, Global.BufferSizeLocal > length ? length : Global.BufferSizeLocal);
            
            _end += count;
            _count += count;
            length -= count;
            
          }
          
        } else {
          
          int count;
          while(length > 0) {
            
            if(rightLength > 0) {
              int nextRead = Global.BufferSizeLocal > length ? length : Global.BufferSizeLocal;
              if(nextRead > rightLength) {
                count = stream.Read(_buffer, _end, rightLength);
                rightLength = 0;
              } else {
                count = stream.Read(_buffer, _end, nextRead);
                rightLength -= count;
              }
            } else {
              count = stream.Read(_buffer, _end, Global.BufferSizeLocal > length ? length : Global.BufferSizeLocal);
            }
            
            _end = (_end + count) % _buffer.Length;
            _count += count;
            length -= count;
            
          }
          
        }
      } else {
        
        int count;
        while(length > 0) {
          
          count = stream.Read(_buffer, _end, Global.BufferSizeLocal > length ? length : Global.BufferSizeLocal);
          
          _end = (_end + count) % _buffer.Length;
          _count += count;
          length -= count;
          
        }
        
      }
      
      _freeSection = _buffer.Length - _start;
      var total = _count;
      
      return total;
      
    }
    
    /// <summary>
    /// Enqueue bytes in the specified stream into the queue.
    /// </summary>
    public void Enqueue(Stream stream, int length) {
      
      if (_count + length > _buffer.Length) SetCapacity(PowerOf2LargerThan(_count + length));
      
      if (_start < _end) {
        int rightLength = _buffer.Length - _end;
        
        if (rightLength >= length) {
          
          int count;
          while(length > 0) {
            
            count = stream.Read(_buffer, _end, Global.BufferSizeLocal > length ? length : Global.BufferSizeLocal);
            
            _end += count;
            _count += count;
            length -= count;
            
          }
          
        } else {
          
          int count;
          while(length > 0) {
            
            if(rightLength > 0) {
              int nextRead = Global.BufferSizeLocal > length ? length : Global.BufferSizeLocal;
              if(nextRead > rightLength) {
                count = stream.Read(_buffer, _end, rightLength);
                rightLength = 0;
              } else {
                count = stream.Read(_buffer, _end, nextRead);
                rightLength -= count;
              }
            } else {
              count = stream.Read(_buffer, _end, Global.BufferSizeLocal > length ? length : Global.BufferSizeLocal);
            }
            
            _end = (_end + count) % _buffer.Length;
            _count += count;
            length -= count;
            
          }
          
        }
      } else {
        
        int count;
        while(length > 0) {
          
          count = stream.Read(_buffer, _end, Global.BufferSizeLocal > length ? length : Global.BufferSizeLocal);
          
          _end = (_end + count) % _buffer.Length;
          _count += count;
          length -= count;
          
        }
        
      }
      
      _freeSection = _buffer.Length - _start;
      
    }
    
    /// <summary>
    /// Enqueues a buffer to the queue and inserts it to a correct position. Returns
    /// the total number of bytes in the queue.
    /// </summary>
    /// <param name="buffer">Buffer to enqueue</param>
    /// <param name="offset">The zero-based byte offset in the buffer</param>
    /// <param name="size">The number of bytes to enqueue</param>
    public int EnqueueGetCount(byte[] buffer, int offset, int size) {
      if (size == 0) return _count;
      
      if ((_count + size) > _buffer.Length) {
        SetCapacity(PowerOf2LargerThan(_count + size));
      }
      
      if (_start < _end) {
        int rightLength = _buffer.Length - _end;
        
        if (rightLength >= size) {
          Buffer.BlockCopy(buffer, offset, _buffer, _end, size);
        } else {
          Buffer.BlockCopy(buffer, offset, _buffer, _end, rightLength);
          Buffer.BlockCopy(buffer, offset + rightLength, _buffer, 0, size - rightLength);
        }
      } else {
        Buffer.BlockCopy(buffer, offset, _buffer, _end, size);
      }
      
      _end = (_end + size) % _buffer.Length;
      _count += size;
      _freeSection = _buffer.Length - _start;
      
      var total = _count;
      
      return total;
      
    }
    
    /// <summary>
    /// Enqueues a buffer to the queue and inserts it to a correct position
    /// </summary>
    /// <param name="buffer">Buffer to enqueue</param>
    /// <param name="offset">The zero-based byte offset in the buffer</param>
    /// <param name="size">The number of bytes to enqueue</param>
    public void Enqueue(byte[] buffer, int offset, int size) {
      if (size == 0) return;
      
      if ((_count + size) > _buffer.Length) {
        SetCapacity(PowerOf2LargerThan(_count + size));
      }
      
      if (_start < _end) {
        int rightLength = _buffer.Length - _end;
        
        if (rightLength >= size) {
          Buffer.BlockCopy(buffer, offset, _buffer, _end, size);
        } else {
          Buffer.BlockCopy(buffer, offset, _buffer, _end, rightLength);
          Buffer.BlockCopy(buffer, offset + rightLength, _buffer, 0, size - rightLength);
        }
      } else {
        Buffer.BlockCopy(buffer, offset, _buffer, _end, size);
      }
      
      _end = (_end + size) % _buffer.Length;
      _count += size;
      _freeSection = _buffer.Length - _start;
      
    }

    /// <summary>
    /// Dequeues a buffer from the queue
    /// </summary>
    /// <param name="buffer">Buffer to enqueue</param>
    /// <param name="offset">The zero-based byte offset in the buffer</param>
    /// <param name="size">The number of bytes to dequeue</param>
    /// <returns>Number of bytes dequeued</returns>
    public int Dequeue(byte[] buffer, int offset, int size) {
      
      if (size > _count) size = _count;
      
      if (size == 0) {
        return 0;
      }
      
      if (_start < _end) {
        Buffer.BlockCopy(_buffer, _start, buffer, offset, size);
      } else {
        int rightLength = (_buffer.Length - _start);
        
        if (rightLength >= size) {
          Buffer.BlockCopy(_buffer, _start, buffer, offset, size);
        } else {
          Buffer.BlockCopy(_buffer, _start, buffer, offset, rightLength);
          Buffer.BlockCopy(_buffer, 0, buffer, offset + rightLength, size - rightLength);
        }
      }
      
      _start = (_start + size) % _buffer.Length;
      _count -= size;
      
      if (_count == 0) {
        _start = 0;
        _end = 0;
      }
      
      _freeSection = _buffer.Length - _start;
      ShrinkIfPossible();
      
      return size;
    }

    /// <summary>
    /// Removes data from the queue
    /// </summary>
    /// <param name="size">The number of bytes to remove</param>
    /// <returns>Number of bytes removed</returns>
    public int Remove(int size) {
      
      if (size > _count) size = _count;
      
      if (size == 0) {
        return 0;
      }
      
      _start = (_start + size) % _buffer.Length;
      _count -= size;
      
      if (_count == 0) {
        _start = 0;
        _end = 0;
      }
      
      _freeSection = _buffer.Length - _start;
      ShrinkIfPossible();
      
      return size;
    }
    
    /// <summary>
    /// Peeks a byte with a relative index to the fHead        
    /// Note: should be used for special cases only, as it is rather slow
    /// </summary>
    /// <param name="index">A relative index</param>
    /// <returns>The byte peeked</returns>
    public byte PeekOne(int index) {
      byte bit = index >= _freeSection
          ? _buffer[index - _freeSection]
          : _buffer[_start + index];
      return bit;
    }
    
    /// <summary>
    /// Dequeues a buffer from the queue
    /// </summary>
    /// <param name="buffer">Buffer to peek into</param>
    /// <param name="offset">The zero-based byte offset in the buffer</param>
    /// <param name="size">The number of bytes to peek</param>
    /// <returns>Number of bytes returned</returns>
    public int PeekMultiple(byte[] buffer, int offset, int size) {
      
      if (size > _count) size = _count;
      
      if (size == 0) {
        return 0;
      }
      
      if (_start < _end) {
        Buffer.BlockCopy(_buffer, _start, buffer, offset, size);
      } else {
        int rightLength = (_buffer.Length - _start);
        
        if (rightLength >= size) {
          Buffer.BlockCopy(_buffer, _start, buffer, offset, size);
        } else {
          Buffer.BlockCopy(_buffer, _start, buffer, offset, rightLength);
          Buffer.BlockCopy(_buffer, 0, buffer, offset + rightLength, size - rightLength);
        }
      }
      
      return size;
    }
    
    /// <summary>
    /// Clears the byte queue
    /// </summary>
    internal void Clear() {
      _start = 0;
      _end = 0;
      _count = 0;
      _freeSection = _buffer.Length;
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Extends or shrinks the capacity of the bytequeue
    /// </summary>
    private void SetCapacity(int capacity) {
      byte[] newBuffer = new byte[capacity];
      
      if (_count > 0) {
        if (_start < _end) {
          Buffer.BlockCopy(_buffer, _start, newBuffer, 0, _count);
        } else {
          Buffer.BlockCopy(_buffer, _start, newBuffer, 0, _buffer.Length - _start);
          Buffer.BlockCopy(_buffer, 0, newBuffer, _buffer.Length - _start, _end);
        }
      }
      
      _start = 0;
      _end = _count;
      _buffer = newBuffer;
    }
    
    /// <summary>
    /// Shrinks the internal buffer if possible
    /// </summary>
    private void ShrinkIfPossible() {
      var minRequiredSize = PowerOf2LargerThan(_count);
      if (minRequiredSize < _buffer.Length && minRequiredSize >= Global.BufferSizeLocal) {
        SetCapacity(minRequiredSize);
      }
    }
    
    /// <summary>
    /// Returns the smallest power of two greater than or equal given value
    /// </summary>
    private int PowerOf2LargerThan(int x) {
      if (x < 0) return 0;
      --x;
      
      x |= x >> 1;
      x |= x >> 2;
      x |= x >> 4;
      x |= x >> 8;
      x |= x >> 16;
      
      return x + 1;
    }
  }
  
}
