/*
 * User: FloppyNipples
 * Date: 14/08/2017
 * Time: 8:58 PM
 */
using System.Net;

namespace Efz.Web {
  
  /// <summary>
  /// UDP header helper.
  /// </summary>
  public class UdpHeader {
    
    //----------------------------------//
    
    /// <summary>
    /// Length of the body as indicated by the udp header.
    /// </summary>
    public int BodyLength;
    /// <summary>
    /// Length of the header.
    /// </summary>
    public int HeaderLength;
    
    /// <summary>
    /// Compression of the message.
    /// </summary>
    public DecompressionMethods Compression;
    
    /// <summary>
    /// Password used to encrypt the data.
    /// </summary>
    public string EncryptionPassword;
    
    /// <summary>
    /// If part of a sequence, the id of the sequence, else '0'. 
    /// </summary>
    public ushort SequenceId;
    /// <summary>
    /// If part of a sequence, the sequence byte index.
    /// </summary>
    public ushort SequenceIndex;
    /// <summary>
    /// If part of a sequence, the number of bytes in the complete sequence.
    /// </summary>
    public ushort SequenceLength;
    
    //----------------------------------//
    
    /// <summary>
    /// Encryption key used to encrypt passwords.
    /// </summary>
    private static readonly byte[] _cryptKey;
    /// <summary>
    /// Authentication key used to encrypt passwords.
    /// </summary>
    private static readonly byte[] _authKey;
    
    //----------------------------------//
    
    static UdpHeader() {
      _authKey  = System.Text.Encoding.ASCII.GetBytes("rGNgn(n$&iIu294.fu4992h248n3*!((");
      _cryptKey = System.Text.Encoding.ASCII.GetBytes("jf,:83f&f$(we@byju,wpy*{#&*23tj&");
    }
    
    /// <summary>
    /// Get a header representing the specified length.
    /// </summary>
    public unsafe void Serialize(ByteBuffer writer) {
      
      // serialize the body length
      SerializeBodyLength(writer);
      // serialize the compression
      SerializeCompression(writer);
      // serialize the encryption
      SerializeEncryption(writer);
      // serialize the sequence information
      SerializeSequence(writer);
      
    }
    
    /// <summary>
    /// Try deserialize the udp header. Returns whether the header is complete.
    /// </summary>
    public bool Deserialize(ByteBuffer reader) {
      
      // deserialze the body length
      if(!DeserializeBodyLength(reader)) return false;
      // deserialize the compression
      if(!DeserializeCompression(reader)) return false;
      // deserialize the encryption
      if(!DeserializeEncryption(reader)) return false;
      // deserialize the sequence
      if(!DeserializeSequence(reader)) return false;
      
      // set the header length
      HeaderLength = (int)reader.Position;
      
      return true;
      
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Serialize the body length into the specified buffer.
    /// </summary>
    private unsafe void SerializeBodyLength(ByteBuffer writer) {
      
      // create a byte array to contain the body length
      byte[] buffer = new byte[8];
      
      // print the body length in the buffer
      fixed (byte* byteP = &buffer[1]) {
        *(int*)byteP = BodyLength;
      }
      
      // quick shuffle
      buffer[5] = buffer[1];
      buffer[1] = buffer[2];
      buffer[7] = buffer[3];
      
      // randomize some
      buffer[0] = Randomize.Byte;
      buffer[3] = Randomize.Byte;
      buffer[6] = Randomize.Byte;
      
      // write the buffer
      writer.Write(buffer);
      
    }
    
    /// <summary>
    /// Deserialize the body length.
    /// </summary>
    private unsafe bool DeserializeBodyLength(ByteBuffer reader) {
      
      // does the buffer contain the required number of bytes?
      if(reader.WriteEnd - reader.Position < 8) {
        // no, return false
        return false;
      }
      
      // read the required
      var buffer = reader.ReadBytes(8);
      
      // quick shuffle backwards
      buffer[3] = buffer[7];
      buffer[2] = buffer[1];
      buffer[1] = buffer[5];
      
      // read the body length
      fixed(byte* intP = &buffer[1]) BodyLength = *(int*)intP;
      
      // return positive
      return true;
      
    }
    
    /// <summary>
    /// Serialize compression.
    /// </summary>
    private void SerializeCompression(ByteBuffer writer) {
      
      // determine the byte to write using compression
      switch(Compression) {
        case DecompressionMethods.GZip:
          writer.Write((byte)Randomize.Range(0, 35));
          break;
        case DecompressionMethods.None:
          writer.Write((byte)Randomize.Range(36, 177));
          break;
        case DecompressionMethods.Deflate:
          writer.Write((byte)Randomize.Range(178, 255));
          break;
      }
      
    }
    
    /// <summary>
    /// Deserialize the compression option from the specified reader.
    /// </summary>
    private bool DeserializeCompression(ByteBuffer reader) {
      
      // does the buffer contain the required buffer?
      if(reader.WriteEnd - reader.Position < 1) {
        // no, return negative
        return false;
      }
      
      // read a byte that will indicate the compression used
      byte compressionByte = reader.ReadByte();
      if(compressionByte < 36) {
        Compression = DecompressionMethods.GZip;
      } else if(compressionByte < 178) {
        Compression = DecompressionMethods.None;
      } else {
        Compression = DecompressionMethods.Deflate;
      }
      
      // return positive
      return true;
      
    }
    
    /// <summary>
    /// Serialize the encryption option.
    /// </summary>
    private unsafe void SerializeEncryption(ByteBuffer writer) {
      
      // is the header encrypted?
      if(EncryptionPassword == null) {
        
        // no, print negative
        writer.Write((byte)((Randomize.Range(0, 100) + 132) % 255));
        
      } else {
        
        // yes, print positive
        writer.Write((byte)((Randomize.Range(101, 255) + 132) % 255));
        
        // encrypt the password
        var encryptedPassword = Crypto.Encrypt(EncryptionPassword, _cryptKey, _authKey);
        
        // get the encrypted password as bytes
        var encryptedBytes = System.Text.Encoding.UTF8.GetBytes(encryptedPassword);
        
        // write the encrypted password length explicitly
        writer.Write((ushort)encryptedBytes.Length);
        
        // write the encrypted password bytes
        writer.Write(encryptedBytes);
        
      }
      
    }
    
    /// <summary>
    /// Deserializes the encryption password if specified.
    /// </summary>
    private unsafe bool DeserializeEncryption(ByteBuffer reader) {
      
      // does the buffer contain at least one byte? no, return negative
      if(reader.WriteEnd - reader.Position < 1) return false;
      
      int encryptionFlagByte = reader.ReadByte() - 132;
      if(encryptionFlagByte < 0) encryptionFlagByte += 255;
      
      // determine whether the buffer contains a password
      if(encryptionFlagByte < 101) return true;
      
      // does the buffer contain the required bytes? no, return negative
      if(reader.WriteEnd - reader.Position < 2) return false;
      
      // read the encrypted password length
      var length = reader.ReadUInt16();
      
      // does the buffer contain the required bytes? no, return negative
      if(reader.WriteEnd - reader.Position < length) return false;
      
      // read the encrypted password bytes
      var encryptedPasswordBytes = reader.ReadBytes(length);
      
      // get the bytes as an encrypted string
      EncryptionPassword = System.Text.Encoding.UTF8.GetString(encryptedPasswordBytes);
      
      // decrypt the password
      EncryptionPassword = Crypto.Decrypt(EncryptionPassword, _cryptKey, _authKey);
      
      return true;
      
    }
    
    /// <summary>
    /// Serialize sequence information.
    /// </summary>
    private unsafe void SerializeSequence(ByteBuffer writer) {
      
      // part of a sequence?
      if(SequenceId == 0) {
        
        // no, print negative
        writer.Write((byte)((Randomize.Range(0, 200) + 128) % 255));
        
      } else {
        
        // yes, print positive
        writer.Write((byte)((Randomize.Range(201, 255) + 128) % 255));
        
        // print the sequence id in the buffer
        writer.Write(SequenceId);
        
        // print the sequence length in the buffer
        writer.Write(SequenceLength);
        
        // print the sequence index in the buffer
        writer.Write(SequenceIndex);
        
      }
      
    }
    
    /// <summary>
    /// Deserialize the reader.
    /// </summary>
    private unsafe bool DeserializeSequence(ByteBuffer reader) {
      
      // does the buffer contain at least one byte?
      if(reader.WriteEnd - reader.Position < 1) return false;
      
      int sequenceFlagByte = reader.ReadByte() - 128;
      if(sequenceFlagByte < 0) sequenceFlagByte += 255;
      
      // yes, determine whether the sequence specifications exist
      if(sequenceFlagByte < 201) return true;
      
      // does the buffer contain the required number of bytes?
      if(reader.WriteEnd - reader.Position < 6) return false;
      
      // read the sequence id
      SequenceId = reader.ReadUInt16();
      // read the sequence length
      SequenceLength = reader.ReadUInt16();
      // read the sequence index
      SequenceIndex = reader.ReadUInt16();
      
      // the sequence options have been read
      return true;
      
    }
    
    
  }
  
}
