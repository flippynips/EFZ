/*
 * User: Joshua
 * Date: 7/08/2016
 * Time: 6:37 PM
 */
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Efz.Collections;
using Efz.Tools;

namespace Efz {
  
  /// <summary>
  /// Wrapper for reading and writing streams of bytes with a buffer.
  /// Allows efficient reading, writing and enumeration of a stream.
  /// </summary>
  public class ByteBuffer : IEnumerable<byte>, IEnumerator<byte> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The current position in the stream taking the buffer into account
    /// </summary>
    public long Position {
      get {
        // write pending buffer
        if(_writing) FlushWrite(true);
        
        // calculate position with reading or writing buffer
        return _position + _bufferIndex;
      }
      set {
        
        // write pending buffer
        if(_writing) FlushWrite(true);
        
        if(_empty) _empty = value > (_position + _bufferIndex);
        
        // set reference position
        _position = value;
        
        // normalize buffer indices
        _bufferIndex = _bufferCount = _charBufferIndex = _charBufferCount = 0;
        
        // set stream position if available
        if(_stream.CanSeek) {
          if(_last) _last = _position > _stream.Position;
          _stream.Position = _position;
        }
        
      }
    }
    
    /// <summary>
    /// The byte at the current position when reading.
    /// </summary>
    public byte Current { get { return _current; } }
    /// <summary>
    /// The first known written index in the stream.
    /// </summary>
    public long WriteStart { get { if(_writing) FlushWrite(false); return _writtenStart; } }
    /// <summary>
    /// The last known written index in the stream.
    /// </summary>
    public long WriteEnd { get { if(_writing) FlushWrite(false); return _writtenEnd; } }
    /// <summary>
    /// Do the start and end indexes represent a single section of the stream?
    /// </summary>
    public bool SingleSection { get { if(_writing) FlushWrite(false); return _writtenSingleSection; } }
    
    /// <summary>
    /// Number of bytes in the buffer for both reading and writing.
    /// </summary>
    public int BufferCount {
      get {
        return _bufferSize;
      }
      set {
        _bufferSize = value;
        Array.Resize(ref _buffer, _bufferSize);
        _encoderNull = true;
      }
    }
    
    /// <summary>
    /// Get the base Stream being consumed.
    /// </summary>
    public Stream Stream {
      get {
        // flush the buffer
        if(_writing) FlushWrite(true);
        
        // return the underlying stream
        return _stream;
      }
      set {
        if(_writing) FlushWrite(true);
        
        // set the stream reference
        _stream = value;
        
        // initialize with reading flag
        _reading = true;
        
        // match the current stream position if possible
        _position = _stream.CanSeek ? _stream.Position : 0;
        _writtenSingleSection = true;
        _writtenStart = 0;
        _writtenEnd = _stream.CanSeek ? _stream.Length : 0;
        
        // normalize buffer indices
        _bufferIndex = _bufferCount = _charBufferIndex = _charBufferCount = 0;
        
      }
    }
    
    /// <summary>
    /// Encoding used to read and write strings. Default is ASCII.
    /// </summary>
    public Encoding Encoding {
      get {
        return _encoding;
      }
      set {
        if(_encoding != value) {
          _encoding = value;
          _encoderNull = true;
        }
      }
    }
    
    /// <summary>
    /// Both stream and buffer are empty.
    /// </summary>
    public bool Empty { get { return _empty; } }
    
    //-------------------------------------------//
    
    /// <summary>
    /// The base stream that is read from and written to.
    /// </summary>
    protected Stream _stream;
    
    /// <summary>
    /// Current buffer being read or written.
    /// </summary>
    protected byte[] _buffer;
    /// <summary>
    /// The current number of bytes assigned to the buffer.
    /// </summary>
    protected int _bufferCount;
    /// <summary>
    /// The current position in the buffer.
    /// </summary>
    protected int _bufferIndex;
    
    /// <summary>
    /// Both stream and buffer is empty.
    /// </summary>
    protected bool _empty;
    /// <summary>
    /// Buffer contains bytes to be written.
    /// </summary>
    protected bool _writing;
    /// <summary>
    /// Buffer contains bytes read from the stream.
    /// </summary>
    protected bool _reading;
    /// <summary>
    /// Encoder is not set up.
    /// </summary>
    protected bool _encoderNull;
    
    /// <summary>
    /// Position within the stream.
    /// </summary>
    protected long _position;
    
    /// <summary>
    /// The start index of the known written section.
    /// </summary>
    protected long _writtenStart;
    /// <summary>
    /// Integers representing the start and end indexes of known written sections of the
    /// stream.
    /// </summary>
    protected long _writtenEnd;
    /// <summary>
    /// Does the current written size represent a single block within the stream.
    /// </summary>
    protected bool _writtenSingleSection;
    
    /// <summary>
    /// Number of bytes in the buffer.
    /// </summary>6
    protected int _bufferSize;
    /// <summary>
    /// The current byte or last byte read.
    /// </summary>
    protected byte _current;
    
    /// <summary>
    /// Character buffer used to read string values.
    /// </summary>
    protected char[] _charBuffer;
    /// <summary>
    /// The current character buffer index.
    /// </summary>
    protected int _charBufferIndex;
    /// <summary>
    /// The number of characters in the character buffer.
    /// </summary>
    protected int _charBufferCount;
    /// <summary>
    /// The maximum number of characters per buffer.
    /// </summary>
    protected int _charBufferSize;
    /// <summary>
    /// Max number of bytes per single character of the current encoding.
    /// </summary>
    protected int _charMaxBytes;
    /// <summary>
    /// Flag to indicate the last read was from the character buffer.
    /// </summary>
    protected bool _charRead;
    
    /// <summary>
    /// Inner encoding value for string values.
    /// </summary>
    protected Encoding _encoding;
    /// <summary>
    /// Encoder instance for the specified encoding.
    /// </summary>
    protected Encoder _encoder;
    /// <summary>
    /// Decoder instance for the specified encoding.
    /// </summary>
    protected Decoder _decoder;
    
    /// <summary>
    /// Flag for the current buffer being the last to be read.
    /// </summary>
    protected bool _last;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize the bytestream with the specified stream.
    /// </summary>
    public ByteBuffer(Stream stream, int buffer = Global.BufferSizeLocal, Encoding encoding = null) {
      _stream = stream;
      _encoding = encoding ?? Encoding.UTF8;
      _bufferSize = buffer;
      
      // initialize with reading flag
      _reading = true;
      
      _position = stream.CanSeek ? stream.Position : 0;
      _writtenSingleSection = true;
      _writtenStart = 0;
      _writtenEnd = stream.CanSeek ? stream.Length : 0;
      
      // set default encoding
      _encoderNull = true;
      
      // byte buffer read from the stream
      _buffer = BufferCache.Get(buffer);
    }
    
    /// <summary>
    /// Dispose gets called after every enumeration - in order
    /// to maintain continuation this method is empty.
    /// Call close to property dispose of the stream and resources.
    /// </summary>
    public void Dispose() {
      if(_writing) {
        FlushWrite(false);
        _stream.Flush();
      }
    }
    
    /// <summary>
    /// Resets the current position and enumerator. This does not dispose 
    /// of the stream. Call 'Close' to properly dispose of the stream and
    /// other resources.
    /// </summary>
    public void Reset() {
      
      // flush buffer
      if(_writing) {
        FlushWrite(true);
        _stream.Flush();
      }
      
      _writtenEnd = _writtenStart = 0;
      
      // reset parameters
      _position = 0;
      
      // reset position if stream can seek
      if(_stream.CanSeek) {
        _stream.Position = _position;
        _stream.SetLength(0L);
      }
      
      if(!_encoderNull) {
        // reset the decoders
        _encoder.Reset();
        _decoder.Reset();
      }
      
      // reset the flags
      _last = _empty = false;
      _charRead = false;
      _writing = false;
      _reading = true;
      
    }
    
    /// <summary>
    /// Dispose of resources associated with this byte stream including the underlying stream.
    /// </summary>
    public void Close() {
      
      // if pending bytes
      if(_writing) FlushWrite(false);
      
      // dispose of the buffer
      _buffer = null;
      // dispose of the underlying stream
      _stream.Dispose();
      
    }
    
    /// <summary>
    /// Set the length of the underlying stream. Also sets the write end to
    /// accomadate the length if a single section.
    /// </summary>
    public void SetLength(long length) {
      
      // set the underlying stream length
      _stream.SetLength(length);
      
      // single section? yes, set the end of the written section to accomadate the length
      if(_writtenSingleSection) _writtenEnd = _writtenStart + length;
      
    }
    
    /// <summary>
    /// Write any pending bytes to the stream.
    /// </summary>
    protected void FlushWrite(bool reading) {
      
      // flush buffer
      if(_bufferIndex != _bufferCount) {
        
        // get the number of bytes that will be written
        _bufferCount = _bufferCount - _bufferIndex;
        
        // write to stream
        _stream.Write(_buffer, _bufferIndex, _bufferCount);
        
        _last = false;
        _empty = false;
        
        // update position in stream
        if(_position < _writtenStart) {
          if(_position + _bufferCount < _writtenStart) {
            _writtenSingleSection = false;
          }
          _writtenStart = _position;
        }
        if(_position + _bufferCount > _writtenEnd) {
          if(_writtenEnd < _position) _writtenSingleSection = false;
          _writtenEnd = _position + _bufferCount;
        }
        
        // increment the position
        _position += _bufferCount;
        
        // reset the params
        _bufferIndex = _bufferCount = _charBufferIndex = _charBufferCount = 0;
        
      }
      
      _charRead = false;
      if(reading) {
        _reading = true;
        _writing = false;
      }
      
    }
    
    /// <summary>
    /// Write the buffer if full.
    /// </summary>
    protected void FlushWrite(int free) {
      
      _charRead = false;
      
      // if buffer is will be filled by potential bytes
      if(_bufferSize - _bufferCount < free) {
        
        _bufferCount = _bufferCount - _bufferIndex;
        
        // update position in stream
        if(_position < _writtenStart) {
          if(_position + _bufferCount < _writtenStart) {
            _writtenSingleSection = false;
          }
          _writtenStart = _position;
        }
        if(_position + _bufferCount > _writtenEnd) {
          if(_writtenEnd < _position) _writtenSingleSection = false;
          _writtenEnd = _position + _bufferCount;
        }
        
        // write to stream
        _stream.Write(_buffer, _bufferIndex, _bufferCount);
        
        _last = false;
        _empty = false;
        
        // update position in stream
        _position += _bufferCount;
        // reset buffer
        _bufferIndex = _bufferCount = _charBufferIndex = _charBufferCount = 0;
        
      }
      
    }
    
    /// <summary>
    /// Flush the read buffer.
    /// </summary>
    protected void FlushRead() {
      // reset index and set stream position
      if(_bufferIndex != _bufferCount) {
        if(_stream.CanSeek) _stream.Position -= _bufferCount - _bufferIndex;
        _position += _bufferCount - _bufferIndex;
        _bufferIndex = _bufferCount = _charBufferIndex = _charBufferCount = 0;
      }
      _charRead = false;
      _reading = false;
      _writing = true;
    }
    
    /// <summary>
    /// Flush the stream write or read buffer.
    /// </summary>
    public void Flush() {
      
      if(_writing) {
        FlushWrite(false);
        _writing = false;
        
        // flush the underlying stream
        _stream.Flush();
        
      } else if(_reading) {
        FlushRead();
        _reading = false;
      }
      
    }
    
    
    /// <summary>
    /// Read a boolean value from the stream.
    /// </summary>
    public bool ReadBoolean() {
      
      // write pending bytes
      if(_writing) FlushWrite(true);
      
      // the character buffer doesn't match the byte buffer
      _charRead = false;
      
      // move to the next byte
      if(MoveNext()) {
        // boolean is true if the current byte is 1
        if(_current > 1) throw new InvalidDataException("Boolean value not within expected range.");
        return _current == 1;
      }
      
      // stream is empty
      throw new EndOfStreamException();
    }
    
    /// <summary>
    /// Read a single byte from the stream.
    /// </summary>
    public byte ReadByte() {
      
      // write pending bytes
      if(_writing) FlushWrite(true);
      
      // the character buffer doesn't match the byte buffer
      _charRead = false;
      
      // move to and return the next byte
      if(MoveNext()) return _current;
      
      // the stream is empty
      throw new EndOfStreamException();
    }
    
    /// <summary>
    /// Read a number of bytes from the stream and populate a byte buffer. Returns
    /// the number of bytes read which may be less than the specified count.
    /// </summary>
    public int ReadBytes(byte[] bytes, int startIndex, int length) {
      
      // write pending bytes
      if(_writing) FlushWrite(true);
      
      if(_empty) throw new EndOfStreamException("Stream was empty while reading '"+length+"' bytes.");
      
      // the character buffer doesn't match the byte buffer
      _charRead = false;
      
      // if there aren't enough bytes in the current buffer
      if(length > _bufferCount - _bufferIndex) {
        
        // the next number of bytes to copy
        int count = _bufferCount - _bufferIndex;
        int remaining = length;
        
        // copy the remaining current buffer into the target byte array
        if(count != 0) {
          
          // copy the current buffer into the target byte array
          Micron.CopyMemory(_buffer, _bufferIndex, bytes, startIndex, count);
          
          // update position in stream
          _position += count;
          
          // reset buffer
          _bufferIndex = _bufferCount = _charBufferIndex = _charBufferCount = 0;
          
          // increment start index
          startIndex += count;
          // decrement length
          remaining -= count;
          
        }
        
        // while more bytes need to be read
        while(remaining != 0) {
          
          // empty check
          if(_last) {
            _empty = true;
            throw new EndOfStreamException("ReadBytes failed to read '"+length+"' bytes at index '"+(count - remaining)+"'.");
          }
          
          // read some bytes
          if(remaining > _bufferSize) {
            count = _stream.Read(bytes, startIndex, _bufferSize);
            if(count < _bufferSize) {
              _empty = true;
              throw new EndOfStreamException("ReadBytes failed to read '"+length+"' bytes at index '"+(count - remaining)+"'.");
            }
          } else {
            count = _stream.Read(bytes, startIndex, remaining);
            if(count < remaining) {
              _empty = true;
              throw new EndOfStreamException("ReadBytes failed to read '"+length+"' bytes at index '"+(count - remaining)+"'.");
            }
          }
          
          // update position in stream
          _position += count;
          
          // increment the start index
          startIndex += count;
          // decrement length
          remaining -= count;
          
        }
        
      } else {
        
        // copy the current buffer into the target byte array
        Micron.CopyMemory(_buffer, _bufferIndex, bytes, startIndex, length);
        
        // increment the buffer index the number of bytes
        _bufferIndex += length;
        
      }
      
      // if we got here, the buffer read was completed
      return length;
    }
    
    /// <summary>
    /// Read bytes until either the specified sequence of bytes are read or the stream
    /// ends.
    /// </summary>
    public void ReadTo(byte[] sequence) {
      
      // write pending bytes
      if(_writing) FlushWrite(true);
      
      // the character buffer doesn't match the byte buffer
      _charRead = false;
      
      // create a tree structure for the byte sequence to search for
      TreeSearch<byte, bool> treeSearch = new TreeSearch<byte, bool>();
      treeSearch.Add(false, sequence);
      var search = treeSearch.SearchDynamic();
      
      // if there aren't enough bytes in the current buffer
      while(true) {
        
        // while the current buffer isn't complete
        while(_bufferIndex < _bufferCount) {
          // does the next byte complete the sequence required?
          if(search.Next(_buffer[_bufferIndex])) {
            // yes, increment the buffer index to read the next byte
            ++_bufferIndex;
            // complete, return
            return;
          }
          ++_bufferIndex;
        }
        
        // empty check
        if(_last) {
          _empty = true;
          throw new EndOfStreamException("Stream was not able to read to sequence of '"+sequence.Length+"' bytes, starting with '"+(sequence.Length == 0 ? "na" : sequence[0].ToString())+"'.");
        }
        
        // increment the current position
        _position += _bufferCount;
        // read the next buffer
        _bufferCount = _stream.Read(_buffer, 0, _bufferSize);
        _bufferIndex = _charBufferIndex = _charBufferCount = 0;
        
        // was a full buffer read? no, this is the last read
        _last |= _bufferCount != _bufferSize;
      }
      
    }
    
    /// <summary>
    /// Read bytes until either the specified sequence of bytes are read or the stream
    /// ends.
    /// </summary>
    public ArrayRig<byte> ReadBytes(byte[] sequence, bool include = false) {
      
      // write pending bytes
      if(_writing) FlushWrite(true);
      
      // the character buffer doesn't match the byte buffer
      _charRead = false;
      
      // create a tree structure for the byte sequence to search for
      TreeSearch<byte, bool> treeSearch = new TreeSearch<byte, bool>();
      treeSearch.Add(false, sequence);
      var search = treeSearch.SearchDynamic();
      
      int index = _bufferIndex;
      ArrayRig<byte> result;
      
      // are there bytes in the buffer?
      if(_bufferIndex == _bufferCount) {
        
        // no, create a collection for the result set
        result = new ArrayRig<byte>(_bufferSize);
        
      } else {
        
        // yes, while the current buffer isn't complete
        while(_bufferIndex < _bufferCount) {
          // does the next byte complete the sequence required?
          if(search.Next(_buffer[_bufferIndex])) {
            // yes, increment the buffer index to move past the last matching byte
            ++_bufferIndex;
            // return an array rig of the sub-section of the buffer
            return new ArrayRig<byte>(_buffer, index, include ? _bufferIndex - index : (_bufferIndex - index) - sequence.Length);
          }
          ++_bufferIndex;
        }
        
        // create a collection for the result set
        result = new ArrayRig<byte>(_bufferCount - index + _bufferSize);
        result.Count = _bufferCount - index;
        Array.Copy(_buffer, index, result.Array, 0, result.Count);
        
      }
      
      // increment the current position
      _position += _bufferCount;
      // read the next buffer
      _bufferCount = _stream.Read(result.Array, result.Count, _bufferSize);
      // reset the current index
      _bufferIndex = _charBufferIndex = _charBufferCount = 0;
      
      // was the buffer filled?
      if(_bufferCount != _bufferSize) {
        // no, decrement the result collection capacity to reflect the remaining bytes
        result.Capacity -= _bufferSize - _bufferCount;
        _last = true;
      }
      
      // while the boundary sequence has not been read
      while(true) {
        
        // while the current buffer isn't complete
        while(result.Count < result.Capacity) {
          // does the next byte complete the sequence required?
          if(search.Next(result.Array[result.Count])) {
            
            // yes, adjust the buffer index
            ++result.Count;
            // update the number of bytes in the buffer
            _bufferCount = result.Capacity - result.Count;
            
            // any bytes remaining in the current read?
            if (_bufferCount > 0) {
              // yes, copy the tail of the bytes read
              Array.Copy(result.Array, result.Count, _buffer, 0, _bufferCount);
            }
            
            // should the sequence be included? yes, increment the index to match the byte count
            if(!include) result.Count -= sequence.Length;
            
            // return the complete collection of bytes
            return result;
          }
          ++result.Count;
        }
        
        if(_last) {
          _empty = true;
          // throw
          throw new EndOfStreamException("Stream was not able to read to sequence of '"+sequence.Length+"' bytes, starting with '"+(sequence.Length == 0 ? "na" : sequence[0].ToString())+"'.");
        }
        
        // resize the result array
        result.SetCapacity(result.Capacity + _bufferSize);
        
        // increment the current position
        _position += _bufferCount;
        
        // read the next buffer directly into the result collection
        _bufferCount = _stream.Read(result.Array, result.Count, _bufferSize);
        
        // was a full buffer read?
        if(_bufferCount != _bufferSize) {
          // no, this is the last read
          _last = true;
          // decrement the result collections capacity to reflect the remaining count
          result.Capacity -= _bufferSize - _bufferCount;
        }
      }
      
    }
    
    /// <summary>
    /// Read a number of bytes from the stream. Returns a shorter collection if the specified count isn't available.
    /// </summary>
    public byte[] ReadBytes(int toRead) {
      
      // skip reading 0 bytes
      if(toRead == 0) return new byte[0];
      
      // write pending bytes
      if(_writing) FlushWrite(true);
      
      // is the stream empty? yes, throw
      if(_empty) {
        throw new EndOfStreamException("Stream was empty, could not read'"+toRead+"' bytes.");
      }
      
      // the character buffer doesn't match the byte buffer
      _charRead = false;
      
      // the bytes to return
      byte[] bytes = new byte[toRead];
      
      // are the required bytes in the current buffer?
      if(toRead > _bufferCount - _bufferIndex) {
        
        int index;
        // no, get the number of bytes to copy
        int count = index = _bufferCount - _bufferIndex;
        
        // copy the remaining current buffer into the target byte array
        if(count != 0) {
          
          // copy the current buffer into the target byte array
          Micron.CopyMemory(_buffer, _bufferIndex, bytes, 0, count);
          
          // update position in stream
          _position += count;
          
          // reset buffer
          _bufferIndex = _bufferCount = _charBufferIndex = _charBufferCount = 0;
          
          // decrement count
          toRead -= count;
          
        }
        
        // are there more bytes?
        if(_last) {
          // no, throw
          _empty = true;
          throw new EndOfStreamException("Stream ended unexpectedly while reading '"+toRead+"' bytes.");
        }
        
        // while more bytes need to be read
        while(toRead != 0) {
          
          // read the remaining bytes
          count = _stream.Read(bytes, index, toRead);
          
          // is the stream empty?
          if(count == 0) {
            // yes, throw
            _empty = true;
            throw new EndOfStreamException("Stream ended unexpectedly while reading '"+toRead+"' bytes.");
          }
          
          // update position in stream
          _position += count;
          
          // decrement the total count
          toRead -= count;
          // increment the current index
          index += count;
          
        }
        
      } else {
        
        // yes, copy the current buffer into the target byte array
        Micron.CopyMemory(_buffer, _bufferIndex, bytes, 0, toRead);
        
        // increment the buffer index by the number of bytes copied
        _bufferIndex += toRead;
        
      }
      
      return bytes;
      
    }
    
    /// <summary>
    /// Read a single character from the stream.
    /// </summary>
    public char ReadChar() {
      
      // write pending bytes
      if(_writing) FlushWrite(true);
      
      // if empty throw
      if(_empty) throw new EndOfStreamException("Stream was empty while reading a chacter.");
      
      // get encoder reference if not set
      if(_encoderNull) {
        _encoderNull = false;
        _charRead = false;
        _encoder = _encoding.GetEncoder();
        _decoder = _encoding.GetDecoder();
        
        // get character/byte relationships
        _charMaxBytes = _encoding.GetMaxByteCount(1);
        _charBufferSize = _charMaxBytes * _bufferSize;
        
        // setup character buffer
        _charBuffer = new char[_charBufferSize];
        _charBufferIndex = _charBufferCount = 0;
      }
      
      // if current buffer has characters
      if(_charRead) {
        if(_charBufferIndex < _charBufferCount) {
          // increment the buffer index
          _bufferIndex += _encoder.GetByteCount(_charBuffer, _charBufferIndex, 1, false);
          // increment the character index
          ++_charBufferIndex;
          
          return _charBuffer[_charBufferIndex-1];
        }
      } else {
        _encoder.Reset();
        _decoder.Reset();
      }
      
      // the character buffer will match the byte buffer
      _charRead = true;
      
      // is the buffer empty?
      if(_bufferIndex >= _bufferCount) {
        // update position in stream
        _position += _bufferCount;
        
        // read bytes from stream
        _bufferCount = _stream.Read(_buffer, 0, _bufferSize);
        // reset the buffer index
        _bufferIndex = _charBufferIndex = _charBufferCount = 0;
        
        // is the buffer empty?
        if(_bufferCount == 0) {
          // yes, the stream is empty
          _empty = true;
          throw new EndOfStreamException("Stream was empty while reading a chacter.");
        }
        
        // populate character buffer
        _charBufferCount = _decoder.GetChars(_buffer, _bufferIndex, _bufferCount, _charBuffer, 0);
        
      } else {
        
        // get the character from the buffer
        _charBufferCount = _decoder.GetChars(_buffer, _bufferIndex, _bufferCount - _bufferIndex, _charBuffer, 0);
        _charBufferIndex = 0;
        
      }
      
      // increment the buffer index
      _bufferIndex += _encoder.GetByteCount(_charBuffer, _charBufferIndex, 1, false);
      // set the character buffer index
      _charBufferIndex = 1;
      
      // return the single character
      return _charBuffer[_charBufferIndex-1];
    }
    
    /// <summary>
    /// Read a string from the stream that must have been prefixed with a 7 byte length.
    /// </summary>
    public string ReadString() {
      
      // write pending bytes
      if(_writing) FlushWrite(true);
      
      // throw if empty
      if(_empty) throw new EndOfStreamException("Stream was empty reading a string.");
      
      _charRead = false;
      
      // get encoder reference if not set
      if(_encoderNull) {
        _encoderNull = false;
        
        _encoder = _encoding.GetEncoder();
        _decoder = _encoding.GetDecoder();
        
        // get character/byte relationships
        _charMaxBytes = _encoding.GetMaxByteCount(1);
        _charBufferSize = _charMaxBytes * _bufferSize;
        
        // setup character buffer
        _charBuffer = new char[_charBufferSize];
        _charBufferIndex = _charBufferCount = 0;
      }
      
      // read the string byte length
      int byteLength = 0;
      int byteBuffer = 0;
      // while not the end of the integer
      while (byteBuffer != 35) {
        byte b = ReadByte();
        byteLength |= (int)(b & 127) << byteBuffer;
        byteBuffer += 7;
        if ((b & 128) == 0) {
          // if empty string was written
          if(byteLength == 0) return string.Empty;
          // got the string length
          break;
        }
      }
      
      // initialize the chracter array to create the string from
      char[] stringChars;
      // use the character buffer if it's large enough
      if(byteLength > _charBufferSize) {
        stringChars = new char[byteLength];
      } else {
        stringChars = _charBuffer;
      }
      
      // if the current buffer contains enough bytes to read the whole string
      if(_bufferCount - _bufferIndex > byteLength) {
        // get the byte length
        byteBuffer = byteLength;
        
        // convert bytes to characters and append to string characters
        _charBufferCount = _decoder.GetChars(_buffer, _bufferIndex, byteBuffer, stringChars, 0);
        
      } else {
        // set the number of bytes to read from the current buffer
        byteBuffer = _bufferCount - _bufferIndex;
        
        // convert current buffer bytes to characters and append to string characters
        _charBufferCount = _decoder.GetChars(_buffer, _bufferIndex, byteBuffer, stringChars, 0);
        
        // decrease the remaining bytes to read
        byteLength -= byteBuffer;
        
        // set the number of bytes to read as the buffer count
        byteBuffer = _bufferCount;
        
        // reset buffer index
        _bufferIndex = _charBufferIndex = 0;
        
        // while there are more bytes to read
        while(byteLength != 0) {
          
          // update position in stream
          _position += _bufferCount;
          // read bytes from stream
          _bufferCount = _stream.Read(_buffer, 0, _bufferSize);
          
          _bufferIndex = _charBufferIndex = 0;
          
          if(_bufferCount == 0) {
            _empty = true;
            throw new EndOfStreamException("Stream was empty reading a string.");
          }
          
          // ensure byteCount is the remaining bytes to read or the size of the buffer
          if(_bufferCount > byteLength) byteBuffer = byteLength;
          
          // decrease the remaining bytes to read
          byteLength -= byteBuffer;
          
          // convert bytes to characters and append to string characters
          _charBufferCount += _decoder.GetChars(_buffer, _bufferIndex, byteBuffer, stringChars, _charBufferCount);
          
        }
        
      }
      
      // set buffer index
      _bufferIndex += byteBuffer;
      
      byteLength = _charBufferCount;
      
      // return the string that was constructed
      return new string(stringChars, 0, byteLength);
      
    }
    
    /// <summary>
    /// Read a string from the stream. The specified characters indicates the end of
    /// the string.
    /// </summary>
    public string ReadString(out char found, params char[] eos) {
      
      // write pending bytes
      if(_writing) FlushWrite(true);
      
      // skip if empty
      if(_empty) {
        throw new EndOfStreamException("Stream was empty reading a string.");
      }
      
      // get encoder reference if not set
      if(_encoderNull) {
        _encoderNull = false;
        _charRead = false;
        _encoder = _encoding.GetEncoder();
        _decoder = _encoding.GetDecoder();
        
        // get character/byte relationships
        _charMaxBytes = _encoding.GetMaxByteCount(1);
        _charBufferSize = _charMaxBytes * _bufferSize;
        
        // setup character buffer
        _charBuffer = new char[_charBufferSize];
        _charBufferIndex = _charBufferCount = 0;
      } else if(!_charRead) {
        // reset the encoder and decoder
        _encoder.Reset();
        _decoder.Reset();
      }
      
      // initialize the character block index
      int stringCurrentIndex = 0;
      // are there characters in the buffer?
      if(!_charRead || _charBufferCount == 0) {
        
        // the character buffer will match the byte buffer
        _charRead = true;
        
        // no, are there bytes in the buffer?
        if(_bufferIndex < _bufferCount) {
          
          // yes, get the next character buffer
          _charBufferCount = _decoder.GetChars(_buffer, _bufferIndex, _bufferCount - _bufferIndex, _charBuffer, 0);
          
        } else {
          
          // no, was the last buffer read?
          if(_last) {
            // yes, return empty
            _empty = true;
            found = Chars.Null;
            return string.Empty;
          }
          
          // update position in stream
          _position += _bufferCount;
          // read next byte buffer
          _bufferCount = _stream.Read(_buffer, 0, _bufferSize);
          _bufferIndex = 0;
          
          // set last buffer flag
          _last = _bufferCount != _bufferSize;
          
          // get the next character buffer
          _charBufferCount = _decoder.GetChars(_buffer, _bufferIndex, _bufferCount, _charBuffer, 0);
          
        }
        
        _charBufferIndex = 0;
      }
      
      // initialize the chracter array to create the string from
      char[] stringChars = _charBuffer;
      
      while(true) {
        
        // check for the end character in the current character buffer
        for(int i = _charBufferIndex; i < _charBufferCount; ++i) {
          
          // has the end character been found?
          if(eos.Contains(stringChars[stringCurrentIndex + i])) {
            
            // yes, reference the character found
            found = stringChars[stringCurrentIndex + i];
            
            string result;
            
            // create the complete string from the character buffer
            result = new string(stringChars, _charBufferIndex, stringCurrentIndex + i - _charBufferIndex);
            // increment the buffer index by the byte count of the string
            ++i;
            _bufferIndex += _encoding.GetByteCount(stringChars, _charBufferIndex, i - _charBufferIndex);
            
            // was the string completed by the first character buffer?
            if(stringCurrentIndex != 0) _charRead = false;
            
            // set the character buffer index
            _charBufferIndex = i;
            
            return result;
          }
        }
        
        // if stream ended unexpectedly
        if(_last) {
          _empty = true;
          found = Chars.Null;
          return new string(stringChars, _charBufferIndex, _charBufferCount - _charBufferIndex);
        }
        
        // resize the character buffer
        if(stringCurrentIndex == 0) {
          stringChars = new char[stringChars.Length + _charBufferSize];
          Array.Copy(_charBuffer, _charBufferIndex, stringChars, 0, _charBufferCount - _charBufferIndex);
        } else {
          Array.Resize(ref stringChars, stringChars.Length + _charBufferSize);
        }
        
        // increment string character index
        stringCurrentIndex += _charBufferCount - _charBufferIndex;
        
        // update position in stream
        _position += _bufferCount;
        // read next byte buffer
        _bufferCount = _stream.Read(_buffer, 0, _bufferSize);
        _bufferIndex = 0;
        
        // set last buffer flag
        if(_bufferCount != _bufferSize) {
          _last = true;
          // has the stream been ended?
          if(_bufferCount == 0) {
            // yes, throw
            _empty = true;
            throw new EndOfStreamException("Stream was empty while reading a string '"+new string(stringChars, _charBufferIndex, stringCurrentIndex)+"' was read.");
          }
        }
        
        // get next character buffer
        _charBufferCount = _decoder.GetChars(_buffer, _bufferIndex, _bufferCount, stringChars, stringCurrentIndex);
        _charBufferIndex = 0;
      }
      
    }
    
    /// <summary>
    /// Read a string from the stream. The specified character indicates the end of
    /// the string and is optionally included in the string.
    /// </summary>
    public string ReadString(char endOfString, bool include = false) {
      
      // write pending bytes
      if(_writing) FlushWrite(true);
      
      // skip if empty
      if(_empty) throw new EndOfStreamException("Stream was empty reading a string to "+endOfString+".");
      
      // get encoder reference if not set
      if(_encoderNull) {
        _encoderNull = false;
        _charRead = false;
        _encoder = _encoding.GetEncoder();
        _decoder = _encoding.GetDecoder();
        
        // get character/byte relationships
        _charMaxBytes = _encoding.GetMaxByteCount(1);
        _charBufferSize = _charMaxBytes * _bufferSize;
        
        // setup character buffer
        _charBuffer = new char[_charBufferSize];
        _charBufferIndex = _charBufferCount = 0;
      } else if(!_charRead) {
        // reset the encoder and decoder
        _encoder.Reset();
        _decoder.Reset();
      }
      
      // initialize the character block index
      int stringCurrentIndex = 0;
      // are there characters in the buffer?
      if(!_charRead || _charBufferCount == 0) {
        
        // the character buffer will match the byte buffer
        _charRead = true;
        
        // no, are there bytes in the buffer?
        if(_bufferIndex < _bufferCount) {
          
          // yes, get the next character buffer
          _charBufferCount = _decoder.GetChars(_buffer, _bufferIndex, _bufferCount - _bufferIndex, _charBuffer, 0);
          
        } else {
          
          // no, was the last buffer read?
          if(_last) {
            // yes, throw
            _empty = true;
            throw new EndOfStreamException("Stream was empty reading a string to "+endOfString+".");
          }
          
          // update position in stream
          _position += _bufferCount;
          // read next byte buffer
          _bufferCount = _stream.Read(_buffer, 0, _bufferSize);
          _bufferIndex = 0;
          
          // set last buffer flag
          _last = _bufferCount != _bufferSize;
          
          // get the next character buffer
          _charBufferCount = _decoder.GetChars(_buffer, 0, _bufferCount, _charBuffer, 0);
          
        }
        
        _charBufferIndex = 0;
      }
      
      // initialize the chracter array to create the string from
      char[] stringChars = _charBuffer;
      
      while(true) {
        
        // check for the end character in the current character buffer
        for(int i = _charBufferIndex; i < _charBufferCount; ++i) {
          
          // has the end character been found?
          if(stringChars[stringCurrentIndex + i] == endOfString) {
            
            // yes
            string result;
            
            ++i;
            // create the complete string from the character buffer
            result = include ?
              // include the end of string character
              new string(stringChars, _charBufferIndex, stringCurrentIndex + i - _charBufferIndex) :
              // don't include the end of string character
              new string(stringChars, _charBufferIndex, stringCurrentIndex + i - 1 - _charBufferIndex);
            
            // increment the buffer index by the byte count of the string
            
            _bufferIndex += _encoding.GetByteCount(stringChars, _charBufferIndex, i - _charBufferIndex);
            
            // was the string completed by the first character buffer?
            if(stringCurrentIndex != 0) _charRead = false;
            
            // set the character buffer index
            _charBufferIndex = i;
            
            return result;
          }
        }
        
        // is last?
        if(_last) {
          // yes, throw
          _empty = true;
          throw new EndOfStreamException("Stream was empty reading a string to "+endOfString+".");
        }
        
        // resize the character buffer
        if(stringCurrentIndex == 0) {
          stringChars = new char[_charBufferSize + _charBufferSize];
          Array.Copy(_charBuffer, _charBufferIndex, stringChars, 0, _charBufferCount - _charBufferIndex);
        } else {
          Array.Resize(ref stringChars, stringChars.Length + _charBufferSize);
        }
        
        // increment string character index
        stringCurrentIndex += _charBufferCount - _charBufferIndex;
        
        // update position in stream
        _position += _bufferCount;
        // read next byte buffer
        _bufferCount = _stream.Read(_buffer, 0, _bufferSize);
        _bufferIndex = 0;
        
        // set last buffer flag
        if(_bufferCount != _bufferSize) {
          _last = true;
          // has the stream been ended?
          if(_bufferCount == 0) {
            // yes, set the flag
            _empty = true;
            return new string(stringChars, 0, stringCurrentIndex);
          }
        }
        
        // get next character buffer
        _charBufferCount = _decoder.GetChars(_buffer, _bufferIndex, _bufferCount, stringChars, stringCurrentIndex);
        _charBufferIndex = 0;
      }
      
    }
    
    /// <summary>
    /// Read a string from the stream. The specified character indicates the end of
    /// the string and is optionally included in the string.
    /// </summary>
    public string ReadString(bool include, params char[] endChars) {
      
      // write pending bytes
      if(_writing) FlushWrite(true);
      
      // skip if empty
      if(_empty) return string.Empty;
      
      // get encoder reference if not set
      if(_encoderNull) {
        _encoderNull = false;
        _charRead = false;
        _encoder = _encoding.GetEncoder();
        _decoder = _encoding.GetDecoder();
        
        // get character/byte relationships
        _charMaxBytes = _encoding.GetMaxByteCount(1);
        _charBufferSize = _charMaxBytes * _bufferSize;
        
        // setup character buffer
        _charBuffer = new char[_charBufferSize];
        _charBufferIndex = _charBufferCount = 0;
      } else if(!_charRead) {
        // reset the encoder and decoder
        _encoder.Reset();
        _decoder.Reset();
      }
      
      // initialize the character block index
      int stringCurrentIndex = 0;
      
      // are there characters in the buffer?
      if(!_charRead || _charBufferCount == 0) {
        
        // no, the character buffer will match the byte buffer
        _charRead = true;
        
        // are there bytes in the buffer?
        if(_bufferIndex < _bufferCount) {
          
          // yes, get the next character buffer
          _charBufferCount = _decoder.GetChars(_buffer, _bufferIndex, _bufferCount - _bufferIndex, _charBuffer, 0);
          
        } else {
          
          // is last?
          if(_last) {
            // yes, throw
            _empty = true;
            throw new EndOfStreamException("Stream was empty reading a string to ["+endChars.ToString(',')+"].");
          }
          
          // update position in stream
          _position += _bufferCount;
          // read next byte buffer
          _bufferCount = _stream.Read(_buffer, 0, _bufferSize);
          _bufferIndex = 0;
          
          // set last buffer flag
          _last = _bufferCount != _bufferSize;
          
          // get the next character buffer
          _charBufferCount = _decoder.GetChars(_buffer, 0, _bufferCount, _charBuffer, 0);
          
        }
        
        _charBufferIndex = 0;
      }
      
      // initialize the chracter array to create the string from
      char[] stringChars = _charBuffer;
      
      while(true) {
        
        // check for the end character in the current character buffer
        for(int i = _charBufferIndex; i < _charBufferCount; ++i) {
          
          // has the end character been found?
          if(endChars.Contains(stringChars[i + stringCurrentIndex])) {
            
            ++i;
            // yes, increment the buffer index by the byte count of the string
            _bufferIndex += _encoding.GetByteCount(stringChars, _charBufferIndex, i + stringCurrentIndex - _charBufferIndex);
            
            string result;
            
            // create the complete string from the character buffer
            result = include ?
              // include the end of string character
              new string(stringChars, _charBufferIndex, stringCurrentIndex + i - _charBufferIndex) :
              // don't include the end of string character
              new string(stringChars, _charBufferIndex, stringCurrentIndex + i - 1 - _charBufferIndex);
            
            // was the string completed by the first character buffer?
            if(stringCurrentIndex != 0) _charRead = false;
            
            // set the character buffer index
            _charBufferIndex = i;
            
            return result;
          }
        }
        
        // if stream ended unexpectedly
        if(_last) {
          _empty = true;
          throw new EndOfStreamException("Stream was empty reading a string to ["+endChars.ToString(',')+"].");
        }
        
        // resize the character buffer
        if(stringCurrentIndex == 0) {
          stringChars = new char[_charBufferCount + _charBufferSize];
          Array.Copy(_charBuffer, _charBufferIndex, stringChars, 0, _charBufferCount - _charBufferIndex);
        } else {
          Array.Resize(ref stringChars, stringChars.Length + _charBufferCount);
        }
        
        // increment string character index
        stringCurrentIndex += _charBufferCount - _charBufferIndex;
        
        // update position in stream
        _position += _bufferCount;
        // read next byte buffer
        _bufferCount = _stream.Read(_buffer, 0, _bufferSize);
        _bufferIndex = 0;
        
        // set last buffer flag
        if(_bufferCount != _bufferSize) {
          _last = true;
          // has the stream been ended?
          if(_bufferCount == 0) {
            // yes, throw
            _empty = true;
            throw new EndOfStreamException("Stream was empty reading a string to ["+endChars.ToString(',')+"].");
          }
        }
        
        // get next character buffer
        _charBufferCount = _decoder.GetChars(_buffer, _bufferIndex, _bufferCount, stringChars, stringCurrentIndex);
        _charBufferIndex = 0;
      }
      
    }
    
    /// <summary>
    /// Read the specified number of characters.
    /// Throws an exception if the required characters cannot be read.
    /// </summary>
    public char[] ReadChars(int count) {
      
      char[] chars = new char[count];
      int readCount = ReadChars(chars, 0, count);
      if(readCount != count) throw new EndOfStreamException("Stream ended before '"+count+"' characters could be read.");
      return chars;
      
    }
    
    /// <summary>
    /// Read a collection of characters into a character buffer.
    /// </summary>
    public unsafe int ReadChars(char[] chars, int offset, int count) {
      
      // write pending bytes
      if(_writing) FlushWrite(true);
      
      // skip if empty
      if(_empty) throw new EndOfStreamException("Stream was empty while reading '"+count+"' chars.");
      
      // get encoder reference if not set
      if(_encoderNull) {
        _encoderNull = false;
        _charRead = false;
        _encoder = _encoding.GetEncoder();
        _decoder = _encoding.GetDecoder();
        
        // get character/byte relationships
        _charMaxBytes = _encoding.GetMaxByteCount(1);
        _charBufferSize = _charMaxBytes * _bufferSize;
        
        // setup character buffer
        _charBuffer = new char[_charBufferSize];
        _charBufferIndex = _charBufferCount = 0;
      }
      
      int charCount = _charBufferCount - _charBufferIndex;
      int start = offset;
      
      // does the current character buffer contain any characters?
      if(_charRead) {
        if(charCount > 0) {
          // yeah, get the number of characters that should be copied
          charCount = charCount > count ? count : charCount;
          
          // copy the required characters from the character buffer
          fixed(void* dst = &chars[offset]) {
            Marshal.Copy(_charBuffer, _charBufferIndex, (IntPtr)dst, charCount);
          }
          
          count -= charCount;
          offset += charCount;
          
          // increment the buffer index
          _bufferIndex += _encoder.GetByteCount(chars, start, charCount, true);
          
          // is the char read complete?
          if(count == 0) {
            
            // yes, increment the char buffer index
            _charBufferIndex += charCount;
            
            return offset - start;
            
          }
          
          // set the char buffer indexes
          _charBufferIndex = _charBufferCount = 0;
          
        }
      } else {
        
        // reset the encoder and decoder
        _encoder.Reset();
        _decoder.Reset();
        
      }
      
      // the char buffer will match the byte buffer
      _charRead = true;
      
      // get the max number of bytes required
      charCount = _decoder.GetCharCount(_buffer, _bufferIndex, _bufferCount - _bufferIndex);
      
      // will the current buffer contain enough bytes to read the required characters?
      if(charCount > count) {
        
        // yes, decode the remainder of the buffer as characters
        _charBufferCount = _decoder.GetChars(_buffer, _bufferIndex, _bufferCount - _bufferIndex, _charBuffer, 0);
        
        // copy the required characters
        fixed(void* dst = &chars[offset]) {
          Marshal.Copy(_charBuffer, 0, (IntPtr)dst, count);
        }
        
        _charBufferIndex += count;
        offset += count;
        
        // increment the buffer index by the number of bytes the char buffer represents
        _bufferIndex += _encoder.GetByteCount(chars, start, offset - start, true);
        
      } else {
        
        // convert current buffer bytes to characters and append to string characters
        charCount = _decoder.GetChars(_buffer, _bufferIndex, _bufferCount - _bufferIndex, chars, offset);
        
        // decrease the remaining chars to read
        count -= charCount;
        offset += charCount;
        
        // iterate while chars need to be read
        while(count != 0) {
          
          // update the position in the stream
          _position += _bufferCount;
          
          // is stream empty?
          if(_last) {
            _empty = true;
            return offset - start;
          }
          
          // read bytes from the stream
          _bufferCount = _stream.Read(_buffer, 0, _bufferSize);
          _bufferIndex = 0;
          
          _last |= _bufferCount != _bufferSize;
          
          // decode the the buffer as characters
          _charBufferCount = _decoder.GetChars(_buffer, _bufferIndex, _bufferCount - _bufferIndex, _charBuffer, 0);
          
          // determine the number of characters to copy from the character buffer
          if(count > _charBufferCount) charCount = _charBufferCount;
          else charCount = count;
          
          // copy the required characters
          fixed(void* dst = &chars[offset]) Marshal.Copy(_charBuffer, 0, (IntPtr)dst, charCount);
          
          count -= charCount;
          
          // has the buffer been completed?
          if(count == 0) {
            // yes, increment the buffer index by the number of bytes the last read char buffer represents
            _bufferIndex += _encoder.GetByteCount(chars, offset, charCount, true);
            _charBufferIndex = charCount;
          }
          
          offset += charCount;
          
        }
        
      }
      
      return offset - start;
      
    }
    
    /// <summary>
    /// Read a ushort value from the stream.
    /// </summary>
    public ushort ReadUInt16() {
      return (ushort)ReadInt16();
    }
    
    /// <summary>
    /// Read a short value from the stream.
    /// </summary>
    public unsafe short ReadInt16() {
      
      // write pending bytes
      if(_writing) FlushWrite(true);
      
      if(_empty) throw new EndOfStreamException("Stream was empty while reading.");
      
      // the char buffer wont match the byte buffer
      _charRead = false;
      
      // check if enough bytes exist in the buffer
      if(_bufferCount - _bufferIndex < 2) {
        // read the required bytes and convert to a short
        fixed(byte* shortP = &ReadBytes(2)[0]) return *(short*)shortP;
      }
      
      // get a pointer to the first byte
      fixed(byte* shortP = &_buffer[_bufferIndex]) {
        
        // increment buffer index
        _bufferIndex += 2;
        
        if (_bufferCount % 2 == 0) {
          return *(short*)shortP;
        }
        
        // If the system architecture is little-endian reverse the byte array
        if (Global.LittleEndian) {
          return (short)(
            (int)(*shortP) |
            (int)(shortP)[1] << 8);
        }
        
        return (short)(
          (int)(*shortP) << 8 |
          (int)(shortP)[1]);
        
      }
      
    }
    
    /// <summary>
    /// Read a uint value from the stream.
    /// </summary>
    public uint ReadUInt32() {
      return (uint)ReadInt32();
    }
    
    /// <summary>
    /// Read an int value from the stream.
    /// </summary>
    public unsafe int ReadInt32() {
      
      // write pending bytes
      if(_writing) FlushWrite(true);
      
      if(_empty) throw new EndOfStreamException("Stream was empty while reading.");
      
      // the char buffer wont match the byte buffer
      _charRead = false;
      
      // check if enough bytes exist in the buffer
      if(_bufferCount - _bufferIndex < 4) {
        // read the required bytes and convert to int
        fixed(byte* intP = &ReadBytes(4)[0]) return *(int*)intP;
      }
      
      fixed(byte* intP = &_buffer[_bufferIndex]) {
        
        // increment buffer index
        _bufferIndex += 4;
        
        if (_bufferCount % 4 == 0) {
          return *(int*)intP;
        }
        
        // If the system architecture is little-endian reverse the byte array
        if (Global.LittleEndian) {
          return
            (int)(*intP)
            | (int)(intP)[1] << 8
            | (int)(intP)[2] << 16
            | (int)(intP)[3] << 24;
        }
        
        return
          (int)(*intP) << 24
          | (int)(intP)[1] << 16
          | (int)(intP)[2] << 8
          | (int)(intP)[3];
        
      }
      
    }
    
    /// <summary>
    /// Read a single value from the stream.
    /// </summary>
    public unsafe float ReadSingle() {
      int integer = ReadInt32();
      return *(float*)(&integer);
    }
    
    /// <summary>
    /// Read a single value from the stream.
    /// </summary>
    public unsafe double ReadDouble() {
      long number = ReadInt64();
      return *(double*)(&number);
    }
    
    /// <summary>
    /// Read a long value from the stream.
    /// </summary>
    public unsafe long ReadInt64() {
      
      // write pending bytes
      if(_writing) FlushWrite(true);
      
      if(_empty) throw new EndOfStreamException("Stream was empty while reading.");
      
      // the char buffer wont match the byte buffer
      _charRead = false;
      
      // check if enough bytes exist in the buffer
      if(_bufferCount - _bufferIndex < 8) {
        // read the required bytes and convert to int
        var bytes = ReadBytes(8);
        fixed(byte* longP = &bytes[0]) return *(long*)longP;
      }
      
      fixed(byte* longP = &_buffer[_bufferIndex]) {
        
        // increment buffer index
        _bufferIndex += 8;
        
        // return the data
        return *(long*)longP;
        
        /* TODO : Determine if required
        if (_bufferCount % 8 == 0) return *(long*)longP;
        
        // If the system architecture is little-endian - reverse
        if (Global.LittleEndian) {
          ulong part =
            (ulong)((int)(*longP) |
            (int)(longP)[1] << 8 |
            (int)(longP)[2] << 16 |
            (int)(longP)[3] << 24);
          
          int num =
            (int)(longP)[4] |
            (int)(longP)[5] << 8 |
            (int)(longP)[6] << 16 |
            (int)(longP)[7] << 24;
          
          return (long)(part | ((ulong)num << 32));
        }
        
        int inv =
          (int)(*longP) << 24 |
          (int)(longP)[1] << 16 |
          (int)(longP)[2] << 8 |
          (int)(longP)[3];
        
        return
          (long)((ulong)(
          (int)(longP)[4] << 24 |
          (int)(longP)[5] << 16 |
          (int)(longP)[6] << 8 |
          (int)(longP)[7]) |
          (ulong)(inv << 32));
        */
      }
      
    }
    
    /// <summary>
    /// Read a single value from the stream.
    /// </summary>
    public ulong ReadUInt64() {
      return (ulong)ReadInt64();
    }





    // ---------------------------------------------------------------------------<><><>
    
    
    
    
    /// <summary>
    /// Write the byte array to the buffered stream.
    /// </summary>
    public void Write(byte[] bytes) {
      Write(bytes, 0, bytes.Length);
    }
    
    /// <summary>
    /// Write the byte array to the buffered stream.
    /// </summary>
    public unsafe void Write(byte[] bytes, int index, int count) {
      
      // skip writing no bytes
      if(count == 0) return;
      
      // flush any current read buffer
      if(_reading) FlushRead();
      
      // will the buffer overflow?
      if(count + _bufferCount > _bufferSize) {
        
        // yes, does the buffer have unwritten bytes?
        if(_bufferIndex != _bufferCount) {
          // yes, write the current buffer
          _stream.Write(_buffer, _bufferIndex, _bufferCount - _bufferIndex);
        }
        
        // write the specified bytes
        _stream.Write(bytes, index, count);
        
        _last = false;
        _empty = false;
        
        // temporarily set 'count' to the number of bytes written to the stream
        count = _bufferCount - _bufferIndex + count;
        
        // update position in stream
        if(_position < _writtenStart) {
          if(_position + count < _writtenStart) {
            _writtenSingleSection = false;
          }
          _writtenStart = _position;
        }
        if(_position + count > _writtenEnd) {
          if(_writtenEnd < _position) _writtenSingleSection = false;
          _writtenEnd = _position + count;
        }
        
        // increment stream position
        _position += count;
        
        // reset buffer parameters
        _bufferIndex = _bufferCount = _charBufferIndex = _charBufferCount = 0;
        
      } else {
        
        // copy bytes to the buffer
        Micron.CopyMemory(bytes, index, _buffer, _bufferCount, count);
        
        // update the current position in the buffer
        _bufferCount += count;
        
      }
      
    }
    
    /// <summary>
    /// Write a string to the stream. Optionally add a prefix of the length of the string to be used
    /// when reading.
    /// </summary>
    public unsafe void Write(string str, bool prefixLength = false) {
      // skip null
      if(string.IsNullOrEmpty(str)) return;
      
      // set writing state
      if(_reading) {
        FlushRead();
      }
      
      // get encoder reference if not set
      if(_encoderNull) {
        _encoderNull = false;
        _charRead = false;
        _encoder = _encoding.GetEncoder();
        _decoder = _encoding.GetDecoder();
        
        // get character/byte relationships
        _charMaxBytes = _encoding.GetMaxByteCount(1);
        _charBufferSize = _charMaxBytes * _bufferSize;
        
        // setup character buffer
        _charBuffer = new char[_charBufferSize];
        _charBufferIndex = _charBufferCount = 0;
      }
      
      // write string byte length as 7 bytes
      int charIndex = 0;
      if(prefixLength) {
        for (charIndex = _encoding.GetByteCount(str); charIndex >= 128u; charIndex >>= 7) {
          Write((byte)(charIndex | 128u));
        }
        Write((byte)charIndex);
      }
      
      // iterate through string incrementaly writing buffer to stream
      charIndex = 0;
      
      // initialize the remaining chracter count
      int charCount;
      int strLength = str.Length;
      
      fixed (char* ptr = str) {
        
        // will the buffer be filled with the first set of characters?
        if(_charMaxBytes * strLength > _bufferSize - _bufferCount) {
          
          // yes, write current buffer to stream
          FlushWrite(false);
          
          // determine the first char write length
          charCount = strLength > _charBufferSize ? _charBufferSize : strLength;
          
        } else {
          
          // the entire string can be appended to the buffer
          charCount = strLength;
        }
        
        bool increment = false;
        
        // get a pointer to the string and a pointer to the current buffer position
        fixed (byte* bufferPtr = _buffer) {
        
          // stop when there are no more characters to write
          while(charIndex != strLength) {
            
            // if not first iteration
            if(increment) {
              
              // flush the last potentially full buffer
              FlushWrite(false);
              
              // check the number of chars to write
              if(charIndex + charCount > strLength) {
                charCount = strLength - charIndex;
              }
              
            } else {
              
              // flag not first iteration
              increment = true;
              
            }
            
            // encode and append the character buffer to the byte buffer
            int current = _encoder.GetBytes(
              ptr + charIndex,
              charCount,
              bufferPtr + _bufferCount,
              _bufferSize - _bufferCount,
              charIndex + charCount == strLength);
            
            _bufferCount += current;
            
            // increment the character index
            charIndex += charCount;
            
          }
          
        }
      }
      
    }
    
    /// <summary>
    /// Write a single character to the stream using the specified encoding.
    /// </summary>
    public void Write(char character) {
      
      // set writing state
      if(_reading) {
        FlushRead();
      } else {
        FlushWrite(_charMaxBytes);
      }
      
      // get encoder reference if not set
      if(_encoderNull) {
        _encoderNull = false;
        _charRead = false;
        _encoder = _encoding.GetEncoder();
        _decoder = _encoding.GetDecoder();
        
        // get character/byte relationships
        _charMaxBytes = _encoding.GetMaxByteCount(1);
        _charBufferSize = _charMaxBytes * _bufferSize;
        
        // setup character buffer
        _charBuffer = new char[_charBufferSize];
        _charBufferIndex = _charBufferCount = 0;
      }
      
      _charBuffer[_charBufferCount] = character;
      
      // copy character to reader buffer
      int current = _encoder.GetBytes(_charBuffer, _charBufferCount, 1, _buffer, _bufferCount, false);
      
      _bufferCount += current;
      ++_charBufferCount;
      
    }
    
    /// <summary>
    /// Write a single character to the stream using the specified encoding.
    /// </summary>
    public void Write(char character, int count) {
      
      // set writing state
      if(_reading) {
        FlushRead();
      }
      
      // get encoder reference if not set
      if(_encoderNull) {
        _encoderNull = false;
        _charRead = false;
        _encoder = _encoding.GetEncoder();
        _decoder = _encoding.GetDecoder();
        
        // get character/byte relationships
        _charMaxBytes = _encoding.GetMaxByteCount(1);
        _charBufferSize = _charMaxBytes * _bufferSize;
        
        // setup character buffer
        _charBuffer = new char[_charBufferSize];
        _charBufferIndex = _charBufferCount = 0;
      }
      
      while(--count >= 0) {
        
        FlushWrite(_charMaxBytes);
        
        _charBuffer[_charBufferCount] = character;
        
        // copy character to reader buffer
        int current = _encoder.GetBytes(_charBuffer, _charBufferCount, 1, _buffer, _bufferCount, false);
        
        _bufferCount += current;
        
        ++_charBufferCount;
        
      }
      
      
      
    }
    
    /// <summary>
    /// Write a single byte to the stream.
    /// </summary>
    public void Write(bool boolean) {
      
      // set writing state
      if(_reading) {
        FlushRead();
      } else {
        FlushWrite(1);
      }
      
      // add to buffer
      _buffer[_bufferCount] = boolean ? (byte)1 : (byte)0;
      ++_bufferCount;
      
    }
    
    /// <summary>
    /// Write a single byte to the stream.
    /// </summary>
    public void Write(byte bit) {
      
      // set writing state
      if(_reading) {
        FlushRead();
      } else {
        FlushWrite(1);
      }
      
      // append to buffer
      _buffer[_bufferCount] = bit;
      ++_bufferCount;
      
    }
    
    /// <summary>
    /// Write a 16 bit integer to the stream.
    /// </summary>
    public unsafe void Write(short number) {
      
      // set writing state
      if(_reading) {
        FlushRead();
      } else {
        FlushWrite(2);
      }
      
      // add to buffer
      fixed (byte* byteP = &_buffer[_bufferCount]) {
        *(short*)byteP = number;
      }
      
      // increment buffer count
      _bufferCount += 2;
      
    }
    
    /// <summary>
    /// Write an unsigned short to the stream.
    /// </summary>
    public unsafe void Write(ushort number) {
      
      // set writing state
      if(_reading) {
        FlushRead();
      } else {
        FlushWrite(2);
      }
      
      // add to buffer
      fixed (byte* byteP = &_buffer[_bufferCount]) {
        *(ushort*)byteP = number;
      }
      
      // increment buffer count
      _bufferCount += 2;
      
    }
    
    /// <summary>
    /// Write an integer to the stream.
    /// </summary>
    public unsafe void Write(int number) {
      
      // set writing state
      if(_reading) {
        FlushRead();
      } else {
        FlushWrite(4);
      }
      
      // add to buffer
      fixed (byte* byteP = &_buffer[_bufferCount]) {
        *(int*)byteP = number;
      }
      
      // increment buffer count
      _bufferCount += 4;
      
    }
    
    /// <summary>
    /// Write an unsigned integer to the stream.
    /// </summary>
    public unsafe void Write(uint number) {
      
      // set writing state
      if(_reading) {
        FlushRead();
      } else {
        FlushWrite(4);
      }
      
      // add to buffer
      fixed (byte* byteP = &_buffer[_bufferCount]) {
        *(uint*)byteP = number;
      }
      
      // increment buffer count
      _bufferCount += 4;
      
    }
    
    /// <summary>
    /// Write an long number to the stream.
    /// </summary>
    public unsafe void Write(long number) {
      
      // set writing state
      if(_reading) {
        FlushRead();
      } else {
        FlushWrite(8);
      }
      
      // add to buffer
      fixed (byte* byteP = &_buffer[_bufferCount]) {
        *(long*)byteP = number;
      }
      
      // increment buffer count
      _bufferCount += 8;
      
    }
    
    /// <summary>
    /// Write an unsigned long integer to the stream.
    /// </summary>
    public unsafe void Write(ulong number) {
      
      // set writing state
      if(_reading) {
        FlushRead();
      } else {
        FlushWrite(8);
      }
      
      // add to buffer
      fixed (byte* byteP = &_buffer[_bufferCount]) {
        *(ulong*)byteP = number;
      }
      
      // increment buffer count
      _bufferCount += 8;
      
    }
    
    /// <summary>
    /// Write an unsigned integer to the stream.
    /// </summary>
    public unsafe void Write(float number) {
      
      // set writing state
      if(_reading) {
        FlushRead();
      } else {
        FlushWrite(4);
      }
      
      // add to buffer
      fixed (byte* byteP = &_buffer[_bufferCount]) {
        *(float*)byteP = number;
      }
      
      // increment buffer index
      _bufferCount += 4;
      
    }
    
    /// <summary>
    /// Write an unsigned integer to the stream.
    /// </summary>
    public unsafe void Write(double number) {
      
      // set writing state
      if(_reading) {
        FlushRead();
      } else {
        FlushWrite(8);
      }
      
      // add to buffer
      fixed (byte* byteP = &_buffer[_bufferCount]) {
        *(double*)byteP = number;
      }
      
      // increment buffer index
      _bufferCount += 8;
      
    }
    
    /// <summary>
    /// Step the enumerator forward a single byte.
    /// </summary>
    public bool MoveNext() {
      // if reading already - fast 
      if(_reading) {
        
        // move to the next byte
        if(_bufferIndex != _bufferCount) {
          // set the current byte
          _current = _buffer[_bufferIndex];
          // increment the current index
          ++_bufferIndex;
          return true;
        }
        
        // if last buffer
        if(_last) {
          _empty = true;
          return false;
        }
        
      } else {
        
        FlushWrite(true);
        
      }
      
      // skip if empty
      if(_empty) return false;
      
      // update position in stream
      _position += _bufferCount;
      // read the next buffer
      _bufferCount = _stream.Read(_buffer, 0, _bufferSize);
      
      // check if buffer wasn't filled
      if(_bufferCount != _bufferSize) {
        // flag as last buffer
        _last = true;
        // check if empty
        if(_bufferCount == 0) {
          _bufferCount = _bufferIndex = _charBufferIndex = _charBufferCount = 0;
          _empty = true;
          return false;
        }
      }
      
      // set the current byte
      _current = _buffer[0];
      // set the current index
      _bufferIndex = 1;
      
      return true;
    }
    
    object System.Collections.IEnumerator.Current { get { return this.Current; } }
    
    public IEnumerator<byte> GetEnumerator() {
      // just return this as the current enumerator
      return this;
    }
    
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
    
    //-------------------------------------------//
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
  }
}
