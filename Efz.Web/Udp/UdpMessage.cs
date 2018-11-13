/*
 * User: FloppyNipples
 * Date: 11/01/2017
 * Time: 00:18
 */
using System;
using System.IO;
using ProtoBuf;

namespace Efz.Web {
  
  
  /// <summary>
  /// Represents a single web request that is be built lazily.
  /// Must be disposed of.
  /// </summary>
  public class UdpMessage : IDisposable {
    
    //----------------------------------//
    
    /// <summary>
    /// The web connection that received the request.
    /// </summary>
    public readonly UdpConnection Connection;
    
    /// <summary>
    /// The payload if any that was sent with the web request.
    /// </summary>
    public ByteBuffer Buffer { get { return _streamBuffer; } }
    /// <summary>
    /// Flag indicating whether the message contains all data it should.
    /// </summary>
    public bool Complete { get { return _remainingBodyLength == 0; } }
    
    /// <summary>
    /// If an error occured, this will represent the error message
    /// </summary>
    public string ErrorMessage { get { return _errorMessage; } }
    
    /// <summary>
    /// Number of bytes in the body of the message.
    /// </summary>
    public UdpHeader Header;
    /// <summary>
    /// Number of bytes in the header of the message.
    /// </summary>
    public int HeaderLength;
    
    //----------------------------------//
    
    /// <summary>
    /// Inner error message if there was a problem with the request.
    /// </summary>
    protected string _errorMessage;
    
    /// <summary>
    /// StreamBeing written to and read from for the request.
    /// </summary>
    protected ByteBuffer _streamBuffer;
    
    /// <summary>
    /// Header of the message.
    /// </summary>
    protected UdpHeader _header;
    /// <summary>
    /// Has the header been completely read?
    /// </summary>
    protected bool _headerComplete;
    /// <summary>
    /// Remaining bytes in the body of the message.
    /// </summary>
    protected int _remainingBodyLength;
    
    //----------------------------------//
    
    internal UdpMessage(UdpConnection connection) {
      
      // reference the web client
      Connection = connection;
      
      _streamBuffer = new ByteBuffer(new MemoryStream(200));
      
      Header = new UdpHeader();
      
      _remainingBodyLength = -1;
      
    }
    
    /// <summary>
    /// Dispose of the web request and it's resources.
    /// </summary>
    public void Dispose() {
      
      if(_streamBuffer == null) return;
      _streamBuffer.Dispose();
      _streamBuffer = null;
      
    }
    
    /// <summary>
    /// Try add the specified bytes to the request.
    /// Returns the number of bytes read from the buffer.
    /// </summary>
    internal unsafe int TryAdd(byte[] buffer, int index, int count) {
      
      // is the request complete?
      if(_remainingBodyLength == 0) return 0;
      
      // has the header been read?
      if(_headerComplete) {
        
        // will the message be complete with the current buffer?
        if(count >= _remainingBodyLength) {
          
          // yes, write the remaining bytes
          _streamBuffer.Write(buffer, index, _remainingBodyLength);
          
          count = _remainingBodyLength;
          _remainingBodyLength = 0;
          
          // process the complete message body
          PostprocessBody();
          
          return count;
          
        }
        
        // write the entire buffer
        _streamBuffer.Write(buffer, index, count);
        _remainingBodyLength -= count;
        
        return count;
      }
      
      int writtenCount = 0;
      
      // while there are bytes to be read from the buffer
      while(count > 0) {
        
        // write 32 bytes at a time until the header is complete
        int writeCount = count > 32 ? 32 : count;
        // write a chunk of bytes into the buffer
        _streamBuffer.Write(buffer, index, writeCount);
        
        // increment the total written count
        writtenCount += writeCount;
        index += writeCount;
        count -= writeCount;
        
        _streamBuffer.Position = 0;
        
        // was the header able to be deserialized?
        if(Header.Deserialize(_streamBuffer)) {
          
          // yes, flag as complete
          _headerComplete = true;
          
          // get the number of bytes that have overflowed from deserializing the header
          writeCount = (int)(_streamBuffer.WriteEnd - Header.HeaderLength);
          
          if(writeCount > 0) {
            
            // get the bytes that overflowed from the header
            var overflow = _streamBuffer.ReadBytes(writeCount);
            
            // reset the stream buffer position to write the body
            _streamBuffer.Position = 0;
            
            // write the bytes the overflowed from deserializing the header
            _streamBuffer.Write(overflow);
            
          } else {
            
            // reset the stream buffer position to write the body
            _streamBuffer.Position = 0;
            
          }
          
          // set the length of the stream to the expected message length
          _streamBuffer.SetLength(Header.BodyLength);
          
          // set the remaining body length
          _remainingBodyLength = (int)(Header.BodyLength - writeCount);
          
          // does the buffer contain the body of the message?
          if(count >= _remainingBodyLength) {
            
            // yes, write the required bytes in the buffer
            if(_remainingBodyLength > 0) {
              
              // write the remainder of the message body
              _streamBuffer.Write(buffer, index, _remainingBodyLength);
              
            }
            
            // increment the written count
            writtenCount += _remainingBodyLength;
            
            // no more body to read
            _remainingBodyLength = 0;
            
            // process the complete message body
            PostprocessBody();
            
            // return the number of bytes read
            return writtenCount;
            
          }
          
          // no, write the remainder of the buffer
          _streamBuffer.Write(buffer, index, count);
          
          // decrement the remaining body count
          _remainingBodyLength -= count;
          
          // return the number of bytes read
          return writtenCount + count;
          
        } else {
          
          // move to the end of the written section
          _streamBuffer.Position = _streamBuffer.WriteEnd;
          
        }
        
      }
      
      return writtenCount;
      
    }
    
    /// <summary>
    /// Get the send options (if any) serialized within the message. Will return null
    /// and reset the position if the stream doesn't contain udp send options.
    /// </summary>
    public TransportOptions GetTransportOptions() {
      
      // deserialize the udp options
      var position = Buffer.Position;
      
      try {
        return Serializer.DeserializeWithLengthPrefix<TransportOptions>(Buffer.Stream, PrefixStyle.Base128);
      } catch {
        Buffer.Position = position;
        return null;
      }
      
    }
    
    /// <summary>
    /// Get a string representation of the web request.
    /// </summary>
    public override string ToString() {
      return "[UDP Message From="+Connection+" Length="+_streamBuffer.WriteEnd+"]";
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Process the completed message. This can involve decryption and decompression.
    /// </summary>
    private void PostprocessBody() {
      
      // move to the start of the stream
      _streamBuffer.Position = 0;
      
      // is the message encrypted?
      if(Header.EncryptionPassword != null) {
        
        // yes, decrypt using the password
        var stream = Crypto.DecryptWithPassword(_streamBuffer.Stream, Header.EncryptionPassword);
        
        // if stream is null, the data wasn't able to be decrypted - throw
        if(stream == null) throw new InvalidDataException("Stream was unable to be decrypted.");
        
        // dispose of the encrypted stream
        _streamBuffer.Stream.Dispose();
        
        // create a new buffer from the decrypted stream
        _streamBuffer.Stream = stream;
        
        // reset buffer position
        _streamBuffer.Position = 0;
        
        // set the adjusted body length
        Header.BodyLength = (int)_streamBuffer.WriteEnd;
        
      }
      
      // is the message compressed?
      if(Header.Compression != System.Net.DecompressionMethods.None) {
        
        // yes, decompress the message
        var stream = StreamHelper.Decompress(Header.Compression, _streamBuffer.Stream, ref Header.BodyLength);
        // dispose of the compressed stream
        _streamBuffer.Close();
        
        // create a new buffer from the decompressed stream
        _streamBuffer.Stream = stream;
        
        // reset buffer position
        _streamBuffer.Position = 0;
        
        // set the adjusted body length
        Header.BodyLength = (int)_streamBuffer.WriteEnd;
        
      }
      
    }
    
  }
  
}
