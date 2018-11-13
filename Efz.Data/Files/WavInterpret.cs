/*
 * User: Bob
 * Date: 26/11/2016
 * Time: 13:09
 */
using Efz.Collections;

namespace Efz.Data.Files {
  
  /// <summary>
  /// Read a wav file into an array of frequencies.
  /// </summary>
  public class WavInterpret : Interpret {
    
    //----------------------------------//
    
    /// <summary>
    /// The left channel or mono channel if the Wav has a single channel.
    /// </summary>
    public ArrayRig<double> Left;
    /// <summary>
    /// The right channel or 'Null' if the Wav has a single channel.
    /// </summary>
    public ArrayRig<double> Right;
    
    /// <summary>
    /// Number of samples in the Wav file. Populated after metadata is populated.
    /// </summary>
    public int Samples;
    
    /// <summary>
    /// Is the Wav file representative of a single channel?
    /// </summary>
    public bool SingleChannel;
    
    //----------------------------------//
    
    /// <summary>
    /// Step 1 : Have the number of channels been read?
    /// </summary>
    protected bool _channels;
    /// <summary>
    /// Step 2 : Have the chunks been read?
    /// </summary>
    protected bool _chunks;
    /// <summary>
    /// Step 3 : Has the metadata been read?
    /// </summary>
    protected bool _metadata;
    
    /// <summary>
    /// Chunk sizes to skip before the data begins.
    /// </summary>
    protected int  _chunkSize;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a wav file reader.
    /// </summary>
    public WavInterpret() {
      BufferCount(24);
      
      Left = new ArrayRig<double>(1000);
      Right = new ArrayRig<double>(1000);
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Reset the double arrays.
    /// </summary>
    protected override void Reset() {
      Left.Clear();
      if(Right != null) Right.Clear();
    }
    
    /// <summary>
    /// Read from the specified bytes.
    /// </summary>
    protected override bool Next(byte[] bytes, int offset, int length) {
      
      int position = offset;
      
      // read the chunks?
      if(_chunks) {
        // read the channels?
        if(_channels) {
          
          _channels = false;
          
          // determine if mono or stereo
          // skip byte 23 as 99.999% of WAVs are 1 or 2 channels
          SingleChannel = bytes[22] == 1;
          
          // get past all the other sub chunks to get to the data subchunk:
          position += 12; // First Subchunk ID from 12 to 16
          
        }
        
        if(_chunks) {
          
          position += 4 + _chunkSize;
          
          // keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
          while (bytes[position] != 100 || bytes[position + 1] != 97 || bytes[position + 2] != 116 || bytes[position + 3] != 97) {
            position += 4;
            
            // get the next chunk size to skip
            _chunkSize = bytes[position] + bytes[position + 1] * 256 + bytes[position + 2] * 65536 + bytes[position + 3] * 16777216;
            
            // if the current buffer won't contain the next chunk and bytes
            if (16 + _chunkSize > offset + length) {
              
              // buffer the remaining
              Buffer(bytes, offset, length - position - offset);
              
              // end the current read of the metadata
              return false;
            }
            
            position += 4 + _chunkSize;
          }
          
          // chunks have been read
          _chunks = false;
        }
        
        // enough bytes remaining to skip to the data? yes increment the position
        if(position - offset + 8 <= length) {
          position += 8;
          // change the buffer bytes size
          BufferCount(SingleChannel ? 2 : 4);
          _metadata = false;
        } else {
          // no, ensure enough bytes next time
          BufferCount(8);
        }
        
        // buffer the remaining bytes
        Buffer(bytes, position, length - position - offset);
        
        return false;
      }
      
      // read wav data
      while (position < length) {
        // read the left channel bytes
        Left.Add(BytesToDouble(bytes[position], bytes[++position]));
        if (!SingleChannel) {
          // read the right channel bytes
          Right.Add(BytesToDouble(bytes[++position], bytes[++position]));
        }
        ++position;
      }
      
      return true;
    }
    
    /// <summary>
    /// Convert two bytes into one double.
    /// </summary>
    private static unsafe double BytesToDouble(byte a, byte b) {
      int s = (b << 8) | a;
      return s / 32768.0;
    }
    
  }
  
}
