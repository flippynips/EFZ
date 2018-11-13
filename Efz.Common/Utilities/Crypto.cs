/*
 * Created by SharpDevelop.
 * User: Pug
 * Date: 2/08/2017
 * Time: 9:24 PM
 */
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Efz {
  
  /// <summary>
  /// This work (Modern Encryption of a String C#, by James Tuley), 
  /// identified by James Tuley, is free of known copyright restrictions.
  /// https://gist.github.com/4336842
  /// http://creativecommons.org/publicdomain/mark/1.0/ 
  /// </summary>
  public static class Crypto {
    
    /// <summary>
    /// preconfigured encryption block size.
    /// </summary>
    public const int BlockBitSize = 128;
    /// <summary>
    /// preconfigured encryption key size.
    /// </summary>
    public const int KeyBitSize = 32;
    
    /// <summary>
    /// preconfigured password key derivation parameters.
    /// </summary>
    public const int SaltBitSize = 16;
    public const int Iterations = 2;
    
    /// <summary>
    /// Salt.
    /// </summary>
    private static readonly byte[] _salt = { 129, 021, 115, 224, 249, 249, 035, 006, 207, 074 };
    /// <summary>
    /// Sha256 hash algorithm.
    /// </summary>
    private static readonly SHA256 _sha256;
    
    static Crypto() {
      _sha256 = SHA256.Create();
    }
    
    /// <summary>
    /// Helper that generates a random key on each call.
    /// </summary>
    public static byte[] NewKey() {
      var key = new byte[KeyBitSize];
      for(int i = key.Length-1; i >= 0; --i) key[i] = Randomize.Byte;
      return key;
    }
    
    
    
    
    /// <summary>
    /// Simple Encryption (AES) then Authentication (HMAC) of a UTF8 message
    /// using Keys derived from a Password (PBKDF2).
    /// </summary>
    /// <param name="secretMessage">The secret message.</param>
    /// <param name="password">The password.</param>
    /// <param name="nonSecretPayload">The non secret payload.</param>
    /// <returns>
    /// Encrypted Message
    /// </returns>
    /// <exception cref="System.ArgumentException">password</exception>
    /// <remarks>
    /// Significantly less secure than using random binary keys.
    /// Adds additional non secret payload for key generation parameters.
    /// </remarks>
    public static string Encrypt(string secretMessage, string password, byte[] nonSecretPayload = null) {
      
      var plainText = Encoding.UTF8.GetBytes(secretMessage);
      var cipherText = EncryptWithPassword(plainText, password, nonSecretPayload);
      return Convert.ToBase64String(cipherText);
      
    }

    /// <summary>
    /// Simple Authentication (HMAC) and then Descryption (AES) of a UTF8 Message
    /// using keys derived from a password (PBKDF2). 
    /// </summary>
    /// <param name="encryptedMessage">The encrypted message.</param>
    /// <param name="password">The password.</param>
    /// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
    /// <returns>
    /// Decrypted Message
    /// </returns>
    /// <exception cref="System.ArgumentException">Encrypted Message Required!;encryptedMessage</exception>
    /// <remarks>
    /// Significantly less secure than using random binary keys.
    /// </remarks>
    public static string Decrypt(string encryptedMessage, string password, int nonSecretPayloadLength = 0) {
      
      var cipherText = Convert.FromBase64String(encryptedMessage);
      var plainText = DecryptWithPassword(cipherText, password, nonSecretPayloadLength);
      return plainText == null ? null : Encoding.UTF8.GetString(plainText);
      
    }
    
    
    
    
    /// <summary>
    /// Simple Encryption (AES) then Authentication (HMAC) for a UTF8 Message.
    /// </summary>
    /// <param name="secretMessage">The secret message.</param>
    /// <param name="cryptKey">The crypt key. Needs to be {KeyBitSize} bytes!</param>
    /// <param name="authKey">The auth key. Needs to be {KeyBitSize} bytes!</param>
    /// <param name="nonSecretPayload">(Optional) Non-Secret Payload.</param>
    /// <returns>
    /// Encrypted Message
    /// </returns>
    /// <exception cref="System.ArgumentException">Secret Message Required!;secretMessage</exception>
    /// <remarks>
    /// Adds overhead of (Optional-Payload + BlockSize(16) + Message-Padded-To-Blocksize +  HMac-Tag(32)) * 1.33 Base64
    /// </remarks>
    public static string Encrypt(string secretMessage, byte[] cryptKey, byte[] authKey, byte[] nonSecretPayload = null) {
      var plainText = Encoding.UTF8.GetBytes(secretMessage);
      var cipherText = Encrypt(plainText, cryptKey, authKey, nonSecretPayload);
      return Convert.ToBase64String(cipherText);
    }
    
    /// <summary>
    /// Simple Authentication (HMAC) then Decryption (AES) for a secrets UTF8 Message.
    /// </summary>
    /// <param name="encryptedMessage">The encrypted message.</param>
    /// <param name="cryptKey">The crypt key.</param>
    /// <param name="authKey">The auth key.</param>
    /// <param name="nonSecretPayloadLength">Length of the non secret payload.</param>
    /// <returns>
    /// Decrypted Message
    /// </returns>
    /// <exception cref="System.ArgumentException">Encrypted Message Required!;encryptedMessage</exception>
    public static string Decrypt(string encryptedMessage, byte[] cryptKey, byte[] authKey, int nonSecretPayloadLength = 0) {
      var cipherText = Convert.FromBase64String(encryptedMessage);
      var plainText = Decrypt(cipherText, cryptKey, authKey, nonSecretPayloadLength);
      return plainText == null ? null : Encoding.UTF8.GetString(plainText);
    }
    
    
    
    
    /// <summary>
    /// Encrypt bytes.
    /// NOTE : CryptKey needs to be {KeyBitSize} bits!
    /// NOTE : AuthKey needs to be {KeyBitSize} bits!
    /// </summary>
    public static byte[] Encrypt(byte[] secretMessage, byte[] cryptKey, byte[] authKey, byte[] nonSecretPayload = null) {
      
      //non-secret payload optional
      nonSecretPayload = nonSecretPayload ?? new byte[] { };
      
      byte[] cipherText;
      byte[] iv;

      using (var aes = new AesManaged {
          KeySize = KeyBitSize * 8,
          BlockSize = BlockBitSize,
          Mode = CipherMode.CBC,
          Padding = PaddingMode.PKCS7
        }) {
        
        //Use random IV
        aes.GenerateIV();
        iv = aes.IV;
        
        using (var encrypter = aes.CreateEncryptor(cryptKey, iv))
        using (var cipherStream = new MemoryStream()) {
          
          using (var cryptoStream = new CryptoStream(cipherStream, encrypter, CryptoStreamMode.Write))
          using (var binaryWriter = new BinaryWriter(cryptoStream)) {
            
            // encrypt Data
            binaryWriter.Write(secretMessage);
            
          }
          
          cipherText = cipherStream.ToArray();
        }
        
      }
      
      //Assemble encrypted message and add authentication
      using (var hmac = new HMACSHA256(authKey))
      using (var encryptedStream = new MemoryStream()) {
        
        using (var binaryWriter = new BinaryWriter(encryptedStream)) {
          
          //Prepend non-secret payload if any
          binaryWriter.Write(nonSecretPayload);
          //Prepend IV
          binaryWriter.Write(iv);
          //Write Ciphertext
          binaryWriter.Write(cipherText);
          binaryWriter.Flush();
          
          // authenticate all data
          encryptedStream.Position = 0;
          var tag = hmac.ComputeHash(encryptedStream);
          // postpend tag
          binaryWriter.Write(tag);
          
        }
        
        return encryptedStream.ToArray();
      }
      
    }
    
    /// <summary>
    /// Decript the specified message.
    /// NOTE : CryptKey needs to be {KeyBitSize} bits!
    /// NOTE : AuthKey needs to be {KeyBitSize} bits!
    /// </summary>
    public static byte[] Decrypt(byte[] encryptedMessage, byte[] cryptKey, byte[] authKey, int nonSecretPayloadLength = 0) {
      
      using (var hmac = new HMACSHA256(authKey)) {
        
        int sentTagLength = hmac.HashSize / 8;
        byte[] sentTag = BufferCache.Get(sentTagLength);
        // calculate tag
        byte[] calcTag = hmac.ComputeHash(encryptedMessage, 0, encryptedMessage.Length - sentTagLength);
        const int ivLength = BlockBitSize / 8;
        
        // if message length is to small just return null
        if (encryptedMessage.Length < sentTagLength + nonSecretPayloadLength + ivLength)
          return null;
        
        // grab sent
        Array.Copy(encryptedMessage, encryptedMessage.Length - sentTagLength, sentTag, 0, sentTagLength);

        //Compare Tag with constant time comparison
        var compare = 0;
        for (var i = 0; i < sentTagLength; ++i) compare |= sentTag[i] ^ calcTag[i]; 

        //if message doesn't authenticate return null
        if (compare != 0) return null;

        using (var aes = new AesManaged {
          KeySize = KeyBitSize * 8,
          BlockSize = BlockBitSize,
          Mode = CipherMode.CBC,
          Padding = PaddingMode.PKCS7
        }) {

          //Grab IV from message
          var iv = new byte[ivLength];
          Array.Copy(encryptedMessage, nonSecretPayloadLength, iv, 0, ivLength);

          using (var decrypter = aes.CreateDecryptor(cryptKey, iv))
          using (var decrypted = new MemoryStream()) {
            using (var decrypterStream = new CryptoStream(decrypted, decrypter, CryptoStreamMode.Write))
            using (var binaryWriter = new BinaryWriter(decrypterStream)) {
              //Decrypt Cipher Text from Message
              binaryWriter.Write(
                encryptedMessage,
                nonSecretPayloadLength + ivLength,
                encryptedMessage.Length - nonSecretPayloadLength - ivLength - sentTagLength
              );
            }
            
            // return the decrypted bytes as an array
            return decrypted.ToArray();
          }
        }
      }
      
    }
    
    
    
    
    /// <summary>
    /// Encrypt bytes with a string password.
    /// </summary>
    public static byte[] EncryptWithPassword(byte[] secretMessage, string password, byte[] nonSecretPayload = null) {
      nonSecretPayload = nonSecretPayload ?? new byte[] {};
      
      var payload = BufferCache.Get((SaltBitSize * 2) + nonSecretPayload.Length);
      
      Array.Copy(nonSecretPayload, payload, nonSecretPayload.Length);
      int payloadIndex = nonSecretPayload.Length;
      
      byte[] cryptKey;
      byte[] authKey;
      //Use Random Salt to prevent pre-generated weak password attacks.
      using (var generator = new Rfc2898DeriveBytes(password, SaltBitSize, Iterations)) {
        var salt = generator.Salt;
        
        //Generate Keys
        cryptKey = generator.GetBytes(KeyBitSize);
        
        //Create Non Secret Payload
        Array.Copy(salt, 0, payload, payloadIndex, salt.Length);
        payloadIndex += salt.Length;
      }
      
      //Deriving separate key, might be less efficient than using HKDF, 
      //but now compatible with RNEncryptor which had a very similar wireformat and requires less code than HKDF.
      using (var generator = new Rfc2898DeriveBytes(password, SaltBitSize, Iterations)) {
        var salt = generator.Salt;
        
        //Generate Keys
        authKey = generator.GetBytes(KeyBitSize);
        
        //Create Rest of Non Secret Payload
        Array.Copy(salt, 0, payload, payloadIndex, salt.Length);
      }
      
      return Encrypt(secretMessage, cryptKey, authKey, payload);
    }
    
    /// <summary>
    /// Decrypt with password.
    /// </summary>
    public static byte[] DecryptWithPassword(byte[] encryptedMessage, string password, int nonSecretPayloadLength = 0) {
      
      var cryptSalt = new byte[SaltBitSize];
      var authSalt = new byte[SaltBitSize];
      
      // grab Salt from Non-Secret Payload
      Array.Copy(encryptedMessage, nonSecretPayloadLength, cryptSalt, 0, SaltBitSize);
      Array.Copy(encryptedMessage, nonSecretPayloadLength + SaltBitSize, authSalt, 0, SaltBitSize);
      
      byte[] cryptKey;
      byte[] authKey;
      
      // generate crypt key
      using (var generator = new Rfc2898DeriveBytes(password, cryptSalt, Iterations)) {
        cryptKey = generator.GetBytes(KeyBitSize);
      }
      // generate auth key
      using (var generator = new Rfc2898DeriveBytes(password, authSalt, Iterations)) {
        authKey = generator.GetBytes(KeyBitSize);
      }
      
      return Decrypt(encryptedMessage, cryptKey, authKey, SaltBitSize + SaltBitSize + nonSecretPayloadLength);
      
    }
    
    
    
    /// <summary>
    /// Encrypt bytes from the stream using a string password.
    /// </summary>
    public static MemoryStream EncryptWithPassword(Stream stream, string password, byte[] nonSecretPayload = null) {
      
      byte[] payload;
      int payloadIndex;
      if(nonSecretPayload == null) {
        
        payload = BufferCache.Get(SaltBitSize * 2);
        
        payloadIndex = 0;
        
      } else {
        
        payload = BufferCache.Get((SaltBitSize * 2) + nonSecretPayload.Length);
        
        Array.Copy(nonSecretPayload, payload, nonSecretPayload.Length);
        payloadIndex = nonSecretPayload.Length;
        
      }
      
      byte[] cryptKey;
      byte[] authKey;
      //Use Random Salt to prevent pre-generated weak password attacks.
      using (var generator = new Rfc2898DeriveBytes(password, SaltBitSize, Iterations)) {
        var salt = generator.Salt;
        
        // generate the cryptography key
        cryptKey = generator.GetBytes(KeyBitSize);
        
        // copy the salt
        Array.Copy(salt, 0, payload, payloadIndex, salt.Length);
        payloadIndex += salt.Length;
      }
      
      //Deriving separate key, might be less efficient than using HKDF, 
      //but now compatible with RNEncryptor which had a very similar wireformat and requires less code than HKDF.
      using (var generator = new Rfc2898DeriveBytes(password, SaltBitSize, Iterations)) {
        var salt = generator.Salt;
        
        // generate the auth key
        authKey = generator.GetBytes(KeyBitSize);
        
        // create the rest of the non-secret payload
        Array.Copy(salt, 0, payload, payloadIndex, salt.Length);
        payloadIndex += salt.Length;
      }
      
      byte[] iv;
      byte[] buffer = BufferCache.Get();
      
      using (var aes = new AesManaged {
          Key = cryptKey,
          BlockSize = BlockBitSize,
          Mode = CipherMode.CBC,
          Padding = PaddingMode.Zeros
        }) {
        
        // use random IV
        aes.GenerateIV();
        iv = aes.IV;
        
        using (var encrypter = aes.CreateEncryptor())
        using (var cipherStream = new MemoryStream())
        using (var cryptoStream = new CryptoStream(cipherStream, encrypter, CryptoStreamMode.Write)) {
          
          // read from the stream
          int count = stream.Read(buffer, 0, Global.BufferSizeLocal);
          
          // while the buffer is filled
          while(count == Global.BufferSizeLocal) {
            
            // write to the crypto stream
            cryptoStream.Write(buffer, 0, count);
            
            // read from the stream
            count = stream.Read(buffer, 0, Global.BufferSizeLocal);
            
          }
          
          // write the final buffer
          cryptoStream.Write(buffer, 0, count);
          
          // flush the encryped stream
          cryptoStream.Flush();
          
          cipherStream.Position = 0;
          
          // assemble encrypted message and add authentication
          var encryptedStream = new MemoryStream();
          
          // prepend payload
          encryptedStream.Write(payload, 0, payloadIndex);
          // prepend IV
          encryptedStream.Write(iv, 0, iv.Length);
          
          // read from the stream
          count = cipherStream.Read(buffer, 0, Global.BufferSizeLocal);
          
          while(count == Global.BufferSizeLocal) {
            
            // write to the crypto stream
            encryptedStream.Write(buffer, 0, count);
            
            // read from the stream
            count = cipherStream.Read(buffer, 0, Global.BufferSizeLocal);
            
          }
          
          // write the final buffer
          encryptedStream.Write(buffer, 0, count);
          
          BufferCache.Set(buffer);
          
          
          // authenticate all data by postpending a hash of all data
          encryptedStream.Position = payloadIndex + iv.Length;
          
          using (var hmac = new HMACSHA256(authKey)) {
            
            // calculate a hash of the entire stream excluding the payload and iv
            // that can be used to authenticate the received data
            byte[] tag = hmac.ComputeHash(encryptedStream);
            
            // write the tag to the end of the encrypted stream
            encryptedStream.Write(tag, 0, tag.Length);
            
          }
          
          // reset the stream position
          encryptedStream.Position = 0;
          
          // return the complete, encrypted stream
          return encryptedStream;
          
        }
        
      }
      
    }
    
    /// <summary>
    /// Decrypt with password. The stream must be navigatable.
    /// </summary>
    public static Stream DecryptWithPassword(Stream stream, string password, int nonSecretPayloadLength = 0) {
      
      const int ivLength = BlockBitSize / 8;
      
      var cryptSalt = new byte[SaltBitSize];
      var authSalt = new byte[SaltBitSize];
      
      var streamStartPosition = stream.Position;
      
      // grab Salt from Non-Secret Payload
      if(nonSecretPayloadLength != 0) stream.Position = streamStartPosition + nonSecretPayloadLength;
      stream.Read(cryptSalt, 0, SaltBitSize);
      stream.Read(authSalt, 0, SaltBitSize);
      
      // grab IV from message
      var iv = new byte[ivLength];
      stream.Read(iv, 0, ivLength);
      
      
      byte[] cryptKey;
      byte[] authKey;
      
      // generate crypt key
      using (var generator = new Rfc2898DeriveBytes(password, cryptSalt, Iterations)) {
        cryptKey = generator.GetBytes(KeyBitSize);
      }
      // generate auth key
      using (var generator = new Rfc2898DeriveBytes(password, authSalt, Iterations)) {
        authKey = generator.GetBytes(KeyBitSize);
      }
      
      using (var hmac = new HMACSHA256(authKey)) {
        
        int sentTagLength = hmac.HashSize / 8;
        
        // if message length is to small just return null
        if (stream.Length - streamStartPosition < sentTagLength + nonSecretPayloadLength + ivLength)
          throw new CryptographicException("Insufficient bytes in stream.");
        
        // get the number of encrypted bytes that constitute the message
        int count = (int)(stream.Length -
          streamStartPosition -
          sentTagLength -
          nonSecretPayloadLength -
          ivLength -
          SaltBitSize - SaltBitSize);
        
        byte[] tagContent = BufferCache.Get(count);
        
        // read the bytes required for the tag hash
        count = stream.Read(tagContent, 0, count);
        
        // calculate the tag hash
        byte[] calcTag = hmac.ComputeHash(tagContent, 0, count);
        
        // read sent tag
        stream.Read(tagContent, 0, sentTagLength);
        
        // compare tag with constant time comparison
        var compare = 0;
        for (var i = 0; i < sentTagLength; ++i) compare |= tagContent[i] ^ calcTag[i]; 
        
        BufferCache.Set(tagContent);
        
        // if message doesn't authenticate, throw
        if (compare != 0) throw new CryptographicException("Data hash was not correct.");
        
        using (var aes = new AesManaged {
          Key = cryptKey,
          IV = iv,
          BlockSize = BlockBitSize,
          Mode = CipherMode.CBC,
          Padding = PaddingMode.Zeros
        }) {
          
          
          // create a memory stream for the decrypted bytes
          var decrypted = new MemoryStream();
          
          using (var decrypter = aes.CreateDecryptor()) {
            
            var decrypterStream = new CryptoStream(decrypted, decrypter, CryptoStreamMode.Write);
            
            byte[] buffer = BufferCache.Get();
            
            // move to the start of the encrypted data
            stream.Position = streamStartPosition +
              nonSecretPayloadLength +
              SaltBitSize + SaltBitSize +
              ivLength;
            
            // determine the bytes to be written to the decrypter stream 
            int remaining = (int)(stream.Length - stream.Position - sentTagLength);
            
            // read the first buffer from the stream
            count = stream.Read(buffer, 0, remaining > Global.BufferSizeLocal ? Global.BufferSizeLocal : remaining);
            
            // while the buffer is full
            while(count == Global.BufferSizeLocal) {
              
              // decrement the remaining bytes
              remaining -= count;
              
              // write the buffer
              decrypterStream.Write(buffer, 0, count);
              
              // read a new buffer
              count = stream.Read(buffer, 0, Global.BufferSizeLocal);
              
            }
            
            // if the stream ended prematurely, throw
            if(count != remaining) throw new EndOfStreamException();
            
            // write final buffer
            decrypterStream.Write(buffer, 0, count);
            
            BufferCache.Set(buffer);
            
            // flush the decryption stream
            decrypterStream.FlushFinalBlock();
            
            // set the decrpted memory stream position
            decrypted.Position = 0;
            
            // return the decrypted stream
            return decrypted;
          }
          
        }
      }
      
    }
    
    
    
    
    
    /// <summary>
    /// Fast insecure murmur3 hash of the specified string into base 64.
    /// </summary>
    public static string FastHash(string text, bool ascii = true) {
      return Convert.ToBase64String(FastHashBytes(text, ascii));
    }
    
    /// <summary>
    /// Fast insecure murmur3 hash of the specified string to a 64bit int.
    /// </summary>
    public unsafe static ulong FashHashUInt64(string text, bool ascii = true) {
      
      // get the text bytes
      byte[] bytes = ascii ? Encoding.ASCII.GetBytes(text) : Encoding.UTF8.GetBytes(text);
      
      // length of the string in bytes
      int curLength = bytes.Length;
      int length = curLength;
      // the seed of the hash
      ulong h1 = (ulong)(text.Length % 53298);
      ulong h2 = 0;
      
      // body, eat stream a 64-bit int at a time
      Int64 currentIndex = -1;
      while (curLength >= 8) {
        // Get eight bytes from the input into an UInt64
        h2 = ((ulong)bytes[++currentIndex] << 56
            | (ulong)bytes[++currentIndex] << 48
            | (ulong)bytes[++currentIndex] << 40
            | (ulong)bytes[++currentIndex] << 32
            | (ulong)bytes[++currentIndex] << 24
            | (ulong)bytes[++currentIndex] << 16
            | (ulong)bytes[++currentIndex] << 8
            | (ulong)bytes[++currentIndex]);
        
        // bitmagic hash
        h2 *= 0xcc9e2d51b8735923;
        h2 = (h2 << 33) | (h2 >> (64 - 33));
        h2 *= 0x1b8c9e2d482c9193;
        
        h1 ^= h2;
        h2 = (h1 << 33) | (h1 >> (64 - 33));
        h1 = h1 * 5 + 0xe6546b685f2c32b7;
        curLength -= 8;
      }
   
      // tail, the reminder bytes that fell through the iteration
      switch (curLength) {
        case 7:
          h2 = (ulong)(bytes[++currentIndex]
            | (ulong)bytes[++currentIndex] << 8
            | (ulong)bytes[++currentIndex] << 16
            | (ulong)bytes[++currentIndex] << 32
            | (ulong)bytes[++currentIndex] << 40
            | (ulong)bytes[++currentIndex] << 48
            | (ulong)bytes[++currentIndex] << 56);
          h2 *= 0xcc9e2d51b8735923;
          h2 = (h2 << 33) | (h2 >> (64 - 33));
          h2 *= 0x1b8c9e2d482c9193;
          h1 ^= h2;
          break;
        case 6:
          h2 = (ulong)(bytes[++currentIndex]
            | (ulong)bytes[++currentIndex] << 8
            | (ulong)bytes[++currentIndex] << 16
            | (ulong)bytes[++currentIndex] << 32
            | (ulong)bytes[++currentIndex] << 40
            | (ulong)bytes[++currentIndex] << 48);
          h2 *= 0xcc9e2d51b8735923;
          h2 = (h2 << 33) | (h2 >> (64 - 33));
          h2 *= 0x1b8c9e2d482c9193;
          h1 ^= h2;
          break;
        case 5:
          h2 = (ulong)(bytes[++currentIndex]
            | (ulong)bytes[++currentIndex] << 8
            | (ulong)bytes[++currentIndex] << 16
            | (ulong)bytes[++currentIndex] << 32
            | (ulong)bytes[++currentIndex] << 40);
          h2 *= 0xcc9e2d51b8735923;
          h2 = (h2 << 33) | (h2 >> (64 - 33));
          h2 *= 0x1b8c9e2d482c9193;
          h1 ^= h2;
          break;
        case 4:
          h2 = (ulong)(bytes[++currentIndex]
            | (ulong)bytes[++currentIndex] << 8
            | (ulong)bytes[++currentIndex] << 16
            | (ulong)bytes[++currentIndex] << 32);
          h2 *= 0xcc9e2d51b8735923;
          h2 = (h2 << 33) | (h2 >> (64 - 33));
          h2 *= 0x1b8c9e2d482c9193;
          h1 ^= h2;
          break;
        case 3:
          h2 = (UInt32)(bytes[++currentIndex]
            | (ulong)bytes[++currentIndex] << 8
            | (ulong)bytes[++currentIndex] << 16);
          h2 *= 0xcc9e2d51b8735923;
          h2 = (h2 << 33) | (h2 >> (64 - 33));
          h2 *= 0x1b8c9e2d482c9193;
          h1 ^= h2;
          break;
        case 2:
          h2 = (UInt32)(bytes[++currentIndex]
            | (ulong)bytes[++currentIndex] << 8);
          h2 *= 0xcc9e2d51b8735923;
          h2 = (h2 << 33) | (h2 >> (64 - 33));
          h2 *= 0x1b8c9e2d482c9193;
          h1 ^= h2;
          break;
        case 1:
          h2 = (UInt32)(bytes[++currentIndex]);
          h2 *= 0xcc9e2d51b8735923;
          h2 = (h2 << 33) | (h2 >> (64 - 33));
          h2 *= 0x1b8c9e2d482c9193;
          h1 ^= h2;
          break;
      }
      
      // finalization
      h1 ^= (ulong)length;
      h1 ^= h1 >> 33;
      h1 *= 0x85ebca6b04a2f214;
      h1 ^= h1 >> 33;
      h1 *= 0xc2b2ae356b04a2f3;
      h1 ^= h1 >> 33;
      
      // return the bytes
      return h1 ^ h2;
      
    }
    
    /// <summary>
    /// Fast insecure murmur3 hash of the specified string to a 16 byte array.
    /// </summary>
    public unsafe static byte[] FastHashBytes(string text, bool ascii = true) {
      
      // get the text bytes
      byte[] bytes = ascii ? Encoding.ASCII.GetBytes(text) : Encoding.UTF8.GetBytes(text);
      
      // length of the string in bytes
      int curLength = bytes.Length;
      int length = curLength;
      // the seed of the hash
      ulong h1 = (ulong)(text.Length % 53298);
      ulong h2 = 0;
      
      // body, eat stream a 64-bit int at a time
      Int64 currentIndex = -1;
      while (curLength >= 8) {
        // Get eight bytes from the input into an UInt64
        h2 = ((ulong)bytes[++currentIndex] << 56
            | (ulong)bytes[++currentIndex] << 48
            | (ulong)bytes[++currentIndex] << 40
            | (ulong)bytes[++currentIndex] << 32
            | (ulong)bytes[++currentIndex] << 24
            | (ulong)bytes[++currentIndex] << 16
            | (ulong)bytes[++currentIndex] << 8
            | (ulong)bytes[++currentIndex]);
        
        // bitmagic hash
        h2 *= 0xcc9e2d51b8735923;
        h2 = (h2 << 33) | (h2 >> (64 - 33));
        h2 *= 0x1b8c9e2d482c9193;
        
        h1 ^= h2;
        h2 = (h1 << 33) | (h1 >> (64 - 33));
        h1 = h1 * 5 + 0xe6546b685f2c32b7;
        curLength -= 8;
      }
   
      // tail, the reminder bytes that fell through the iteration
      switch (curLength) {
        case 7:
          h2 = (ulong)(bytes[++currentIndex]
            | (ulong)bytes[++currentIndex] << 8
            | (ulong)bytes[++currentIndex] << 16
            | (ulong)bytes[++currentIndex] << 32
            | (ulong)bytes[++currentIndex] << 40
            | (ulong)bytes[++currentIndex] << 48
            | (ulong)bytes[++currentIndex] << 56);
          h2 *= 0xcc9e2d51b8735923;
          h2 = (h2 << 33) | (h2 >> (64 - 33));
          h2 *= 0x1b8c9e2d482c9193;
          h1 ^= h2;
          break;
        case 6:
          h2 = (ulong)(bytes[++currentIndex]
            | (ulong)bytes[++currentIndex] << 8
            | (ulong)bytes[++currentIndex] << 16
            | (ulong)bytes[++currentIndex] << 32
            | (ulong)bytes[++currentIndex] << 40
            | (ulong)bytes[++currentIndex] << 48);
          h2 *= 0xcc9e2d51b8735923;
          h2 = (h2 << 33) | (h2 >> (64 - 33));
          h2 *= 0x1b8c9e2d482c9193;
          h1 ^= h2;
          break;
        case 5:
          h2 = (ulong)(bytes[++currentIndex]
            | (ulong)bytes[++currentIndex] << 8
            | (ulong)bytes[++currentIndex] << 16
            | (ulong)bytes[++currentIndex] << 32
            | (ulong)bytes[++currentIndex] << 40);
          h2 *= 0xcc9e2d51b8735923;
          h2 = (h2 << 33) | (h2 >> (64 - 33));
          h2 *= 0x1b8c9e2d482c9193;
          h1 ^= h2;
          break;
        case 4:
          h2 = (ulong)(bytes[++currentIndex]
            | (ulong)bytes[++currentIndex] << 8
            | (ulong)bytes[++currentIndex] << 16
            | (ulong)bytes[++currentIndex] << 32);
          h2 *= 0xcc9e2d51b8735923;
          h2 = (h2 << 33) | (h2 >> (64 - 33));
          h2 *= 0x1b8c9e2d482c9193;
          h1 ^= h2;
          break;
        case 3:
          h2 = (UInt32)(bytes[++currentIndex]
            | (ulong)bytes[++currentIndex] << 8
            | (ulong)bytes[++currentIndex] << 16);
          h2 *= 0xcc9e2d51b8735923;
          h2 = (h2 << 33) | (h2 >> (64 - 33));
          h2 *= 0x1b8c9e2d482c9193;
          h1 ^= h2;
          break;
        case 2:
          h2 = (UInt32)(bytes[++currentIndex]
            | (ulong)bytes[++currentIndex] << 8);
          h2 *= 0xcc9e2d51b8735923;
          h2 = (h2 << 33) | (h2 >> (64 - 33));
          h2 *= 0x1b8c9e2d482c9193;
          h1 ^= h2;
          break;
        case 1:
          h2 = (UInt32)(bytes[++currentIndex]);
          h2 *= 0xcc9e2d51b8735923;
          h2 = (h2 << 33) | (h2 >> (64 - 33));
          h2 *= 0x1b8c9e2d482c9193;
          h1 ^= h2;
          break;
      }
      
      // finalization
      h1 ^= (ulong)length;
      h1 ^= h1 >> 33;
      h1 *= 0x85ebca6b04a2f214;
      h1 ^= h1 >> 33;
      h1 *= 0xc2b2ae356b04a2f3;
      h1 ^= h1 >> 33;
      
      // return the bytes
      return new [] {
        (byte)((ulong)((long)h1 & -72057594037927936L) >> 56),
        (byte)((h1 & 71776119061217280L) >> 48),
        (byte)((h1 & 280375465082880L) >> 40),
        (byte)((h1 & 1095216660480L) >> 32),
        (byte)(((long)h1 & (long)-16777216) >> 24),
        (byte)((h1 & 16711680L) >> 16),
        (byte)((h1 & 65280L) >> 8),
        (byte)(h1 & 255L),
        (byte)((ulong)((long)h2 & -72057594037927936L) >> 56),
        (byte)((h2 & 71776119061217280L) >> 48),
        (byte)((h2 & 280375465082880L) >> 40),
        (byte)((h2 & 1095216660480L) >> 32),
        (byte)(((long)h2 & (long)-16777216) >> 24),
        (byte)((h2 & 16711680L) >> 16),
        (byte)((h2 & 65280L) >> 8),
        (byte)(h2 & 255L)
      };
      
    }
    
    /// <summary>
    /// Hash the content of the stream.
    /// </summary>
    public static int Hash(Stream stream) {
      
      const uint c1 = 0xcc9e2d51;
      const uint c2 = 0x1b873593;
      
      uint h1 = 349;
      uint k1 = 0;
      int streamLength = 0;
      
      using(ByteBuffer reader = new ByteBuffer(stream)) {
        byte[] chunk = new byte[4];
        int length = reader.ReadBytes(chunk, 0, 4);
        while (!reader.Empty) {
          streamLength += length;
          switch(length) {
            case 4:
              /* Get four bytes from the input into an uint */
              k1 = (uint)
                 (chunk[0]
                | chunk[1] << 8
                | chunk[2] << 16
                | chunk[3] << 24);
  
              /* bitmagic hash */
              k1 *= c1;
              k1 = (k1 << 15) | (k1 >> (32 - 15));
              k1 *= c2;
  
              h1 ^= k1;
              h1 = (h1 << 13) | (h1 >> (32 - 13));
              h1 = h1 * 5 + 0xe6546b64;
              break;
            case 3:
              k1 = (uint)
                 (chunk[0]
                | chunk[1] << 8
                | chunk[2] << 16);
              k1 *= c1;
              k1 = (k1 << 15) | (k1 >> (32 - 15));
              k1 *= c2;
              h1 ^= k1;
              break;
            case 2:
              k1 = (uint)
                 (chunk[0]
                | chunk[1] << 8);
              k1 *= c1;
              k1 = (k1 << 15) | (k1 >> (32 - 15));
              k1 *= c2;
              h1 ^= k1;
              break;
            case 1:
              k1 = (uint)(chunk[0]);
              k1 *= c1;
              k1 = (k1 << 15) | (k1 >> (32 - 15));
              k1 *= c2;
              h1 ^= k1;
              break;
          }
          length = reader.ReadBytes(chunk, 0, 4);
        }
      }
          
      // finalization, magic chants to wrap it all up
      h1 ^= (uint)streamLength;
      h1 ^= h1 >> 16;
      h1 *= 0x85ebca6b;
      h1 ^= h1 >> 13;
      h1 *= 0xc2b2ae35;
      h1 ^= h1 >> 16;
      
      unchecked {
        return (int)h1;
      }
    }
    
    /// <summary>
    /// Secure hash of the specified text into base 64.
    /// </summary>
    public static string Hash(string text, bool ascii = true) {
      byte[] textBytes = ascii ? Encoding.ASCII.GetBytes(text) : Encoding.UTF8.GetBytes(text);
      byte[] bytes = BufferCache.Get(textBytes.Length + _salt.Length);
      Micron.CopyMemory(textBytes, bytes, textBytes.Length);
      Micron.CopyMemory(_salt, 0, bytes, textBytes.Length, _salt.Length);
      return Convert.ToBase64String(_sha256.ComputeHash(bytes, 0, textBytes.Length + _salt.Length));
    }
    
    
  }
  
}
