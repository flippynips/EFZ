/*
 * User: FloppyNipples
 * Date: 19/02/2017
 * Time: 22:45
 */
using System;
using System.IO;
using Efz.Collections;
using Efz.Threading;

namespace Efz.Cql {
  
  /// <summary>
  /// Blob management.
  /// </summary>
  public class BlobStream : Stream {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Id of the blob stream.
    /// </summary>
    public readonly string Id;
    
    public override bool CanRead {
      get { return true; }
    }
    public override bool CanWrite {
      get { return true; }
    }
    public override bool CanSeek {
      get { return true; }
    }
    
    /// <summary>
    /// Length of the blob stream.
    /// </summary>
    public override long Length {
      get { return _length; }
    }
    /// <summary>
    /// Current position within the stream.
    /// </summary>
    public override long Position {
      get { return _position + _sectionPosition; }
      set { Seek(value, SeekOrigin.Begin); }
    }
    
    /// <summary>
    /// Bytes to keep in the blob stream.
    /// </summary>
    public int BufferSize = (int)Global.Megabyte * 2;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Blobs instance the stream makes use of.
    /// </summary>
    protected Blobs _blobs;
    /// <summary>
    /// Byte count loaded into the blob stream.
    /// </summary>
    protected int _bytes;
    
    /// <summary>
    /// Number of sections in the blob.
    /// </summary>
    protected int _sectionCount;
    /// <summary>
    /// Size of each full section in the blob.
    /// </summary>
    protected int _sectionSize = (int)Global.Megabyte;
    
    /// <summary>
    /// Inner length of the blob stream.
    /// </summary>
    protected long _length;
    /// <summary>
    /// Inner position of the blob stream.
    /// </summary>
    protected long _position;
    
    /// <summary>
    /// Current section index.
    /// </summary>
    protected int _sectionIndex;
    /// <summary>
    /// Current index within the current section.
    /// </summary>
    protected int _sectionPosition;
    /// <summary>
    /// Current section being written to or read from.
    /// </summary>
    protected BlobSection _section;
    
    /// <summary>
    /// Has the stream been written to.
    /// </summary>
    protected bool _updated;
    /// <summary>
    /// Flag for the stream to be initialized.
    /// </summary>
    protected bool _initialize;
    
    /// <summary>
    /// Sections of the blob stream.
    /// </summary>
    protected ArrayRig<BlobSection> _sections;
    
    /// <summary>
    /// Represents a single row.
    /// </summary>
    protected class BlobSection {
      /// <summary>
      /// Bytes that represent the section.
      /// </summary>
      public byte[] Bytes;
      /// <summary>
      /// Length of the bytes written.
      /// </summary>
      public int Length;
      /// <summary>
      /// Has the section been written to.
      /// </summary>
      public bool Updated;
    }
    
    /// <summary>
    /// Lock used for disposal of the blob stream.
    /// </summary>
    protected Lock _lock;
    
    //-------------------------------------------//
    
    /// <summary>
    /// New blob stream for a new record.
    /// </summary>
    internal BlobStream(string id, Blobs blobs) {
      Id = id;
      _blobs = blobs;
      
      _lock = new Lock();
      _initialize = true;
    }
    
    /// <summary>
    /// New blob stream from an existing record.
    /// </summary>
    internal BlobStream(string id, Blobs blobs, long length, int sectionCount, int sectionSize) {
      Id = id;
      _blobs = blobs;
      
      _length = length;
      _sectionCount = sectionCount;
      _sectionSize = sectionSize;
      
      _initialize = true;
    }
    
    /// <summary>
    /// Flush the content of the stream.
    /// </summary>
    public override void Flush() {
      if(_lock.TryTake) {
        
        // have any of the sections been written to?
        if(_updated) {
          _updated = false;
          
          // yes, iterate the sections
          for(int i = _sections.Count-1; i >= 0; --i) {
            _section = _sections[i];
            if(_section != null && _section.Updated) {
              SaveSection(_section, i);
            }
          }
          
          _blobs.BlobMeta.Set(Id, _length, _sectionCount, _sectionSize);
          
        }
        
        _lock.Release();
      }
    }
    
    /// <summary>
    /// Dispose of the blob stream.
    /// </summary>
    protected override void Dispose(bool disposing) {
      if(!disposing) return;
      
      _lock.Take();
      FlushInternal();
      
      if(_sections != null) {
        // dispose of the sections collection
        for(int i = _sections.Count-1; i >= 0; --i) {
          if(_sections[i] == null) continue;
          _sections[i].Bytes = null;
        }
        _sections.Dispose();
        _sections = null;
      }
      
      _lock.Release();
      
    }
    
    /// <summary>
    /// Seek the stream.
    /// </summary>
    public override long Seek(long offset, SeekOrigin origin) {
      if(_initialize) Initialize(true);
      
      switch(origin) {
        case SeekOrigin.Begin:
          if(_position + _sectionPosition == offset) return offset;
          _position = offset;
          break;
        case SeekOrigin.Current:
          _position += offset;
          break;
        case SeekOrigin.End:
          if(_position + _sectionPosition == _length - offset) return _position + _sectionPosition;
          _position = _length - offset;
          break;
      }
      
      // determine the section index
      var sectionIndex = (int)(_position / _sectionSize);
      if(sectionIndex != _sectionIndex) {
        _sectionIndex = sectionIndex;
        if(_sectionIndex >= _sectionCount) {
          while(_sectionIndex >= _sections.Count) _sections.Add(new BlobSection());
          _sectionCount = _sectionIndex+1;
        }
        _section = _sections[_sectionIndex];
      }
      
      // update the position within the current section
      _sectionPosition = (int)(_position % _sectionSize);
      // update the overrall section position
      _position -= _sectionPosition;
      
      return _position + _sectionPosition;
    }
    
    /// <summary>
    /// Does nothing. The length is dynamic.
    /// </summary>
    public override void SetLength(long value) {
      //throw new NotImplementedException();
    }
    
    /// <summary>
    /// Read and fill the byte buffer from the stream.
    /// </summary>
    public override int Read(byte[] buffer, int offset, int count) {
      
      if(_initialize) Initialize(true);
      
      int start = offset;
      
      // get the required sections
      while(count > 0) {
        
        if(_section == null) {
          
          if(_sectionIndex == _sectionCount) {
            Log.Error(Position + "- Read Last " + (offset - start) + " bytes. Was hoping for " + count);
            return offset - start;
          }
          
          // load the required section
          SetSection(_sectionIndex);
          
          if(count >= _section.Length - _sectionPosition) {
            
            var copySize = _section.Length - _sectionPosition;
            Micron.CopyMemory(_section.Bytes, _sectionPosition, buffer, offset, copySize);
            count -= copySize;
            offset += copySize;
            
            NextSection();
            
          } else {
            
            Micron.CopyMemory(_section.Bytes, _sectionPosition, buffer, offset, count);
            offset += count;
            _sectionPosition += count;
            count = 0;
          }
          
        } else {
          // will the buffer be filled with the current section?
          if(count >= _section.Length - _sectionPosition) {
            
            // no, copy bytes from the current section
            int bufferSize = _section.Length - _sectionPosition;
            
            Micron.CopyMemory(_section.Bytes, _sectionPosition, buffer, offset, bufferSize);
            
            offset += bufferSize;
            count -= bufferSize;
            
            // increment the section
            NextSection();
            
          } else {
            
            // yes, copy bytes from the current section
            Micron.CopyMemory(_section.Bytes, _sectionPosition, buffer, offset, count);
            
            // increment the section position
            _sectionPosition += count;
            offset += count;
            count = 0;
            if(_sectionPosition > _length) _length = _sectionPosition;
          }
        }
      }
      
      // all required bytes were read
      return offset - start;
    }
    
    /// <summary>
    /// Read a single byte from the stream.
    /// </summary>
    public override int ReadByte() {
      if(_initialize) Initialize(true);
      
      // has the current section been loaded?
      if(_section == null) {
        if(_sectionIndex == _sectionCount) return -1;
        SetSection(_sectionIndex);
      }
      
      // has the end of the current section been reached?
      if(_sectionPosition == _section.Length) {
        // increment the section
        NextSection();
      }
      
      // reference the current byte from the current section
      var bit = _section.Bytes[_sectionPosition];
      // increment the section position
      ++_sectionPosition;
      
      return bit;
    }
    
    /// <summary>
    /// Write the specified byte buffer.
    /// </summary>
    public override void Write(byte[] buffer, int offset, int count) {
      if(_initialize) Initialize(false);
      
      _updated |= count > 0;
      
      // iterate while there are more bytes to write
      while(count > 0) {
        
        if(_section == null) SetSection(_sectionIndex);
        
        // will the section be filled by the section?
        if(count > _sectionSize - _sectionPosition) {
          
          // yes, fill the current section
          var bytes = _sectionSize - _sectionPosition;
          Micron.CopyMemory(buffer, offset, _section.Bytes, _sectionPosition, bytes);
          
          offset += bytes;
          count -= bytes;
          
          _section.Updated = true;
          _section.Length += bytes;
          
          // increment the section
          NextSection();
          
          // add the bytes count
          AddByteCount(bytes);
          
        } else {
          
          // no, copy the buffer bytes
          Micron.CopyMemory(buffer, offset, _section.Bytes, _sectionPosition, count);
          
          // increment the section position
          _sectionPosition += count;
          _section.Length += count;
          _section.Updated = true;
          
          AddByteCount(count);
          count = 0;
          
        }
      }
      
      if(_position + _sectionPosition > _length) _length = _position + _sectionPosition;
    }
    
    /// <summary>
    /// Write a byte.
    /// </summary>
    public override void WriteByte(byte value) {
      if(_initialize) Initialize(false);
      
      _updated = true;
      
      // is the current section full?
      if(_sectionPosition == _sectionSize) {
        
        // yes, move to the next section
        NextSection();
        // set the current section
        SetSection(_sectionIndex);
        
        // the section has been written to
        _section.Updated = true;
      }
      
      // set the current byte
      _section.Bytes[_sectionPosition] = value;
      ++_section.Length;
      // increment the current position
      ++_sectionPosition;
      
      if(_position + _sectionPosition > _length) _length = _position + _sectionPosition;
      
      AddByteCount(1);
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize the blob stream, optionally as a reader.
    /// </summary>
    protected void Initialize(bool reader) {
      
      _initialize = false;
      
      if(reader) {
        if(_sectionCount == 0) {
          // get the blob metadata
          var metadata = _blobs.BlobMeta.Get(Id);
          
          // was the metadata retrieved?
          if(metadata == null) {
            // no, initialize as a new blob
            _sectionCount = 1;
            _sections = new ArrayRig<BlobSection>(_sectionCount);
            _sections.Add(_section = new BlobSection { Bytes = new byte[_sectionSize], Updated = true });
          } else {
            // yes, persist blob metadata
            _length = metadata.Length;
            _sectionCount = metadata.SectionCount;
            _sectionSize = metadata.SectionLength;
            _sections = new ArrayRig<BlobSection>(_sectionCount);
          }
          
        } else {
          
          // initialize the sections collection with the correct count
          _sections = new ArrayRig<BlobSection>(_sectionCount);
          
        }
        
      } else {
        _sectionCount = 1;
        _sections = new ArrayRig<BlobSection>(_sectionCount);
        _sections.Add(_section = new BlobSection { Bytes = new byte[_sectionSize], Updated = true });
      }
      
    }
    
    /// <summary>
    /// Move to the next section.
    /// </summary>
    protected void NextSection() {
      
      // increment the section index
      ++_sectionIndex;
      
      // increment the current position
      _position = _sectionIndex * _sectionSize;
      
      _section = null;
    }
    
    /// <summary>
    /// Load or create the section specified by index.
    /// </summary>
    protected void SetSection(int index) {
      
      _sectionPosition = 0;
      
      if(index == _sectionIndex) {
        if(_sectionIndex == _sectionCount) {
          
          // create a new section
          _sections.Add(_section = new BlobSection());
          _section.Bytes = new byte[_sectionSize];
          _section.Updated = true;
          ++_sectionCount;
          
        } else if(_section == null) {
          
          if(_sections[index] == null) {
            _sections[index] = _section = new BlobSection();
          } else _section = _sections[index];
          
          // get the byte collection from the data table
          _section.Bytes = _blobs.BlobData.Get(Id, index);
          _section.Length = _section.Bytes.Length;
          
          // add the section byte count
          AddByteCount(_section.Length);
        }
        return;
      }
      
      if(index >= _sectionCount) {
        
        while(index >= _sectionCount) {
          
          // create a new section
          _sections.Add(_section = new BlobSection());
          _section.Bytes = new byte[_sectionSize];
          _section.Updated = true;
          ++_sectionCount;
          
        }
      } else {
        
        // create the section
        if(_sections[index] == null) {
          _sections[index] = _section = new BlobSection();
          // get the byte collection from the data table
          _section.Bytes = _blobs.BlobData.Get(Id, index);
          _section.Length = _section.Bytes.Length;
          // add the section byte count
          AddByteCount(_section.Length);
        } else {
          _section = _sections[index];
        }
        
      }
      
    }
    
    /// <summary>
    /// Add the specified byte count and unload sections if the buffer size
    /// has been reached.
    /// </summary>
    protected void AddByteCount(int count) {
      _bytes += count;
      
      // has the buffer size been exceeded?
      if(_bytes >= BufferSize) {
        // yes, remove any unrequired sections
        for(int i = _sections.Count-1; i >= 0; --i) {
          // if the section is active, skip
          if(i == _sectionIndex) continue;
          
          // has the section been written to? yes, save the section
          if(_sections[i].Updated) SaveSection(_sections[i], i);
          _bytes -= _sections[i].Length;
          _sections[i] = null;
          
          // is the current count less than the buffer size? yea, break
          if(_bytes < BufferSize) break;
        }
      }
    }
    
    /// <summary>
    /// Save the section.
    /// </summary>
    protected void SaveSection(BlobSection section, int sectionIndex) {
      section.Updated = false;
      if(section.Length != _sectionSize) {
        byte[] bytes = new byte[section.Length];
        Micron.CopyMemory(section.Bytes, bytes, section.Length);
        _blobs.BlobData.Set(Id, sectionIndex, bytes);
      } else {
        _blobs.BlobData.Set(Id, sectionIndex, section.Bytes);
      }
    }
    
    /// <summary>
    /// Flush the content of the stream without taking the internal lock.
    /// </summary>
    public void FlushInternal() {
      
      // have any of the sections been written to?
      if(_updated) {
        _updated = false;
        // yes, iterate the sections
        for(int i = _sections.Count-1; i >= 0; --i) {
          _section = _sections[i];
          if(_section != null && _section.Updated) {
            SaveSection(_section, i);
          }
        }
        
        _blobs.BlobMeta.Set(Id, _length, _sectionCount, _sectionSize);
        
      }
    }
    
  }
  
}
