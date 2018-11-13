/*
 * User: Joshua
 * Date: 6/08/2016
 * Time: 4:00 PM
 */
using System;
using System.IO;
using System.IO.Compression;

using Efz;

namespace Efz {
  
  /// <summary>
  /// Helper methods relating to streaming data.
  /// </summary>
  public static class StreamHelper {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Reads the specified number of bytes and returns whether they were all displayable ascii.
    /// This is a quick and dirty Ascii text encoding check.
    /// </summary>
    public static bool IsAscii(Stream stream, byte length = 50) {
      if(!stream.CanSeek) throw new ArgumentException("StreamHelper.IsAscii > Stream must be able to seek.");
      long position = stream.Position;
      byte[] bytes;
      
      using(BinaryReader reader = new BinaryReader(stream)) {
        try {
          bytes = reader.ReadBytes(length);
        } catch {
          // skip
          stream.Position = position;
          return false;
        } 
      }
      stream.Position = position;
      
      // iterate read bytes and check if in visible range of Ascii.
      foreach(byte bit in bytes) {
        if(bit < 33) return false;
      }
      
      return true;
    }
    
    /// <summary>
    /// Get a stream as the result of the specified stream being compressed using the specified method. Optionally specify a length
    /// for the number of bytes to read, and dispose of the original stream. The decompressed stream is returned using the specified
    /// callback method.
    /// </summary>
    public static void Compress(IAction<MemoryStream> callback, System.Net.DecompressionMethods method,
      CompressionLevel level, Stream source, int length = -1, bool dispose = true) {
      
      // create a new memory stream to receive the compressed stream
      MemoryStream memStream = new MemoryStream();
      // set the callback parameter
      callback.ArgA = memStream;
      
      // resolve the compression stream
      Stream compressionStream;
      switch(method) {
        case System.Net.DecompressionMethods.Deflate:
          compressionStream = new DeflateStream(memStream, level, true);
          break;
        case System.Net.DecompressionMethods.GZip:
          compressionStream = new GZipStream(memStream, level, true);
          break;
        default:
          // no compression - copy unchanged
          source.CopyTo(memStream);
          // run the callback
          callback.Run();
          return;
      }
      
      // add a compress task
      ManagerUpdate.Control.AddSingle(ActionSet.New(Compress, callback, memStream, compressionStream, source, length, dispose));
      
    }
    
    /// <summary>
    /// Inner method compress stream method.
    /// </summary>
    private static void Compress(IRun callback, Stream result, Stream destination, Stream source,
      int length, bool dispose) {
      
      // reset the buffer
      byte[] buffer = BufferCache.Get();
      
      int count;
      if(length == -1) {
        
        // read from the stream
        count = source.Read(buffer, 0, Global.BufferSizeLocal);
        
      } else {
        
        // read from the stream
        count = length < Global.BufferSizeLocal ?
          source.Read(buffer, 0, length) :
          source.Read(buffer, 0, Global.BufferSizeLocal);
        
        // decrement the remaining bytes
        length -= count;
        
      }
      
      // write to the compression stream
      destination.Write(buffer, 0, count);
      
      // reset the buffer
      BufferCache.Set(buffer);
      
      // has the compression been completed?
      if(count == Global.BufferSizeLocal) {
        
        // no, run a new compression iteration
        ManagerUpdate.Control.AddSingle(Compress, callback, result, destination, source, length, dispose);
        
      } else {
        
        // yes, dispose of the compression stream
        destination.Dispose();
        
        // should the source be disposed? yes
        if(dispose) source.Dispose();
        
        // reset the memory stream position
        result.Position = 0L;
        
        // yes, run the callback
        callback.Run();
        
      }
      
    }
    
    /// <summary>
    /// Get a stream as the result of the specified stream being compressed using the specified method. Optionally specify a length
    /// for the number of bytes to read.
    /// </summary>
    public static MemoryStream Compress(System.Net.DecompressionMethods method, CompressionLevel level, Stream source, int length = -1) {
      
      // create a new memory stream to receive the compressed stream
      MemoryStream memStream = new MemoryStream(Global.BufferSizeLocal);
      
      // resolve the compression stream
      Stream compressionStream;
      switch(method) {
        case System.Net.DecompressionMethods.Deflate:
          compressionStream = new GZipStream(memStream, level, true);
          break;
        case System.Net.DecompressionMethods.GZip:
          compressionStream = new DeflateStream(memStream, level, true);
          break;
        default:
          // no compression - return uncompressed
          source.CopyTo(memStream);
          return memStream;
      }
      
      // was the length specifed?
      if(length == -1) {
        
        // no, write the buffer to the compression
        source.CopyTo(compressionStream, Global.BufferSizeLocal);
        
      } else {
        
        // yes, create a buffer for the copy
        byte[] buffer = BufferCache.Get();
        int count = Global.BufferSizeLocal;
        
        // while the buffer is filled by reading from the stream
        while(count == Global.BufferSizeLocal) {
          // read from the stream
          count = length < Global.BufferSizeLocal ?
            source.Read(buffer, 0, length) :
            source.Read(buffer, 0, Global.BufferSizeLocal);
          
          // write to the compression stream
          compressionStream.Write(buffer, 0, count);
          // decrement the remaining bytes
          length -= count;
        }
        
        BufferCache.Set(buffer);
        
      }
      
      // close the compression stream
      compressionStream.Close();
      
      // return the memory stream
      return memStream;
    }
    
    /// <summary>
    /// Compress the specified byte buffer.
    /// </summary>
    public static byte[] Compress(System.Net.DecompressionMethods method, CompressionLevel level, byte[] buffer, int offset, ref int count) {
      
      // create a new memory stream to receive the compressed stream
      MemoryStream memStream = new MemoryStream(count);
      
      // resolve the compression stream
      Stream compressionStream;
      switch(method) {
        case System.Net.DecompressionMethods.Deflate:
          compressionStream = new DeflateStream(memStream, level, true);
          break;
        case System.Net.DecompressionMethods.GZip:
          compressionStream = new GZipStream(memStream, level, true);
          break;
        default:
          // no compression - return uncompressed
          return buffer;
      }
      
      // write the buffer to the compression
      compressionStream.Write(buffer, offset, count);
      // close the stream
      compressionStream.Dispose();
      
      // get the count
      count = (int)memStream.Length;
      
      // get the bytes from the memory stream
      buffer = memStream.ToArray();
      
      // dispose of the memory stream
      memStream.Dispose();
      
      // return the bytes
      return buffer;
      
    }
    
    /// <summary>
    /// Compress the specified byte buffer.
    /// </summary>
    public static byte[] Decompress(System.Net.DecompressionMethods method, byte[] buffer, int offset, ref int count) {
      
      // create a new memory stream to receive the compressed stream
      using(MemoryStream memStream = new MemoryStream()) {
        
        // resolve the compression stream
        Stream compressionStream;
        switch(method) {
          case System.Net.DecompressionMethods.Deflate:
            compressionStream = new DeflateStream(memStream, CompressionMode.Decompress, true);
            break;
          case System.Net.DecompressionMethods.GZip:
            compressionStream = new GZipStream(memStream, CompressionMode.Decompress, true);
            break;
          default:
            // no compression - return uncompressed
            return buffer;
        }
        
        // write the buffer to the compression
        compressionStream.Write(buffer, offset, count);
        // close the stream
        compressionStream.Dispose();
        
        // get the count
        memStream.Position = 0L;
        count = (int)memStream.Length;
        
        // get the bytes from the memory stream
        buffer = memStream.ToArray();
        
        // dispose of the memory stream
        memStream.Dispose();
        
        // return the bytes
        return buffer;
      }
    }
    
    /// <summary>
    /// Compress the specified byte buffer.
    /// </summary>
    public static Stream Decompress(System.Net.DecompressionMethods method, Stream source, ref int count) {
      
      // create a new memory stream to receive the compressed stream
      MemoryStream memStream = new MemoryStream();
      
      // resolve the compression stream
      Stream compressionStream;
      switch(method) {
        case System.Net.DecompressionMethods.Deflate:
          compressionStream = new DeflateStream(memStream, CompressionMode.Decompress, true);
          break;
        case System.Net.DecompressionMethods.GZip:
          compressionStream = new GZipStream(memStream, CompressionMode.Decompress, true);
          break;
        default:
          // no compression - return uncompressed
          return source;
      }
      
      // write the buffer to the compression
      var buffer = BufferCache.Get();
      int bufferCount = Global.BufferSizeLocal;
      while(bufferCount == Global.BufferSizeLocal) {
        bufferCount = source.Read(buffer, 0, count > Global.BufferSizeLocal ? Global.BufferSizeLocal : count);
        count -= bufferCount;
        compressionStream.Write(buffer, 0, bufferCount);
      }
      
      // pass the buffer back to the cache
      BufferCache.Set(buffer);
      
      // close the stream
      compressionStream.Dispose();
      
      // get the count
      memStream.Position = 0L;
      count = (int)memStream.Length;
      
      // get the bytes from the memory stream
      return memStream;
      
    }
    
    /// <summary>
    /// Read all bytes from the specified stream and return it as an array.
    /// </summary>
    public static byte[] ReadAll(Stream stream, out int count) {
      
      // create the byte collection for the bytes
      byte[] result = BufferCache.Get();
      
      // create a buffer components
      int length = Global.BufferSizeLocal;
      count = 0;
      
      // iterate while there are more bytes to be read
      while(length == Global.BufferSizeLocal) {
        
        // does the result buffer require resizing?
        if(count + Global.BufferSizeLocal > result.Length) {
          // yes, resize
          Array.Resize(ref result, count + count);
        }
        
        // read the next buffer
        length = stream.Read(result, count, Global.BufferSizeLocal);
        count += length;
        
      }
      
      // return the byte buffer
      return result;
    }
    
    //-------------------------------------------//
    
    
    
    
    
    
    
    
    
    
  }
  
}
