/*
 * User: FloppyNipples
 * Date: 12/05/2017
 * Time: 22:33
 */
using System;
using System.IO;

namespace Efz {
  
  /// <summary>
  /// Murmer3 hash.
  /// </summary>
  public class Murmur3 {
    
    public static int ReadSize = 16;
    private const ulong _c1 = 0x7bc04b128042a315L;
    private const ulong _c2 = 0x8cfbbd4321151a7fL;
    
    /// <summary>
    /// Length read for the hash.
    /// </summary>
    private int _length;
    /// <summary>
    /// Seed used for the murmer3 hash.
    /// </summary>
    private uint _seed;
    private ulong _h1;
    private ulong _h2;
    
    /// <summary>
    /// Initialize a new murmur3 hash handler.
    /// </summary>
    public Murmur3(int seed) {
      _seed = (uint)seed;
    }
    
    /// <summary>
    /// Compute a hash of the specified stream.
    /// </summary>
    public void ComputeHash(Stream stream, IAction<byte[]> onComplete) {
      byte[] buffer = BufferCache.Get();
      
      int index = 0;
      int count = stream.Read(buffer, 0, Global.BufferSizeLocal);
      bool last = count != Global.BufferSizeLocal;
      
      _h1 = _seed;
      _length = 0;
      
      // read 128 bits, 16 bytes, 2 longs in eacy cycle
      while (index + 16 < count) {
        ulong k1 = GetUInt64(buffer, index);
        ulong k2 = GetUInt64(buffer, index + 8);
        
        _length += ReadSize;
        MixBody(k1, k2);
        
        index += 16;
      }
      
      if (index + 16 == count) {
        BufferCache.Set(buffer);
        buffer = null;
      } else {
        if(last) {
          // if the input MOD 16 != 0
          if (count > 0) ProcessBytesRemaining(buffer, count - index, index);
          onComplete.ArgA = Hash;
          onComplete.Run();
          return;
        }
        Micron.CopyMemory(buffer, index, buffer, 0, count - index);
        index -= count;
      }
      
      ManagerUpdate.Control.AddSingle(ComputeHashInner, stream, onComplete, buffer, index);
    }
    
    /// <summary>
    /// Continuing compute of a hash of the specified stream.
    /// </summary>
    protected void ComputeHashInner(Stream stream, IAction<byte[]> onComplete, byte[] buffer, int index) {
      if(buffer == null) {
        buffer = BufferCache.Get();
        index = 0;
      }
      int count = stream.Read(buffer, index, Global.BufferSizeLocal);
      bool last = count != Global.BufferSizeLocal;
      
      _h1 = _seed;
      _length = 0;
      
      // read 128 bits, 16 bytes, 2 longs in eacy cycle
      while (index + 16 <= count) {
        ulong k1 = GetUInt64(buffer, index);
        ulong k2 = GetUInt64(buffer, index + 8);
        
        _length += ReadSize;
        MixBody(k1, k2);
        
        index += 16;
      }
      
      if (index + 16 == count) {
        BufferCache.Set(buffer);
        buffer = null;
      } else {
        if(last) {
          // if the input MOD 16 != 0
          if (index < count) ProcessBytesRemaining(buffer, count - index, index);
          onComplete.ArgA = Hash;
          onComplete.Run();
          return;
        }
        Micron.CopyMemory(buffer, index, buffer, 0, count - index);
        index -= count;
      }
      
      ManagerUpdate.Control.AddSingle(ComputeHashInner, stream, onComplete, buffer, index);
    }
    
    /// <summary>
    /// Compute a hash of the specified stream.
    /// </summary>
    public byte[] ComputeHash(Stream stream) {
      _h1 = _seed;
      _length = 0;
      
      int pos = 0;
      byte[] bytes = new byte[16];
      int count = stream.Read(bytes, 0, 16);
      
      // read 128 bits, 16 bytes, 2 longs in eacy cycle
      while (count >= (int)ReadSize) {
        ulong k1 = GetUInt64(bytes, 0);
        pos += 8;
        
        ulong k2 = GetUInt64(bytes, 8);
        pos += 8;
        
        _length += ReadSize;
        
        MixBody(k1, k2);
        
        count = stream.Read(bytes, 0, 16);
      }

      // if the input MOD 16 != 0
      if (count > 0) ProcessBytesRemaining(bytes, count, 0);
      return Hash;
    }
    
    /// <summary>
    /// Compute a hash of the specified array.
    /// </summary>
    public byte[] ComputeHash(byte[] bytes) {
      _h1 = _seed;
      _length = 0;
      
      int pos = 0;
      int remaining = (int)bytes.Length;
      
      // read 128 bits, 16 bytes, 2 longs in eacy cycle
      while (remaining >= ReadSize) {
        ulong k1 = GetUInt64(bytes, pos);
        pos += 8;
        
        ulong k2 = GetUInt64(bytes, pos);
        pos += 8;
        
        _length += ReadSize;
        remaining -= ReadSize;
        
        MixBody(k1, k2);
      }

      // if the input MOD 16 != 0
      if (remaining > 0) ProcessBytesRemaining(bytes, remaining, pos);
      return Hash;
    }
    
    private void ProcessBytesRemaining(byte[] bytes, int remaining, int index) {
      ulong k1 = 0;
      ulong k2 = 0;
      _length += remaining;

      // little endian (x86) processing
      switch (remaining) {
        case 15:
          k2 ^= (ulong)bytes[index + 14] << 48;
          goto case 14;
        case 14:
          k2 ^= (ulong)bytes[index + 13] << 40;
          goto case 13;
        case 13:
          k2 ^= (ulong)bytes[index + 12] << 32;
          goto case 12;
        case 12:
          k2 ^= (ulong)bytes[index + 11] << 24;
          goto case 11;
        case 11:
          k2 ^= (ulong)bytes[index + 10] << 16;
          goto case 10;
        case 10:
          k2 ^= (ulong)bytes[index + 9] << 8;
          goto case 9;
        case 9:
          k2 ^= (ulong)bytes[index + 8];
          goto case 8;
        case 8:
          k1 ^= GetUInt64(bytes, index);
          break;
        case 7:
          k1 ^= (ulong)bytes[index + 6] << 48;
          goto case 6;
        case 6:
          k1 ^= (ulong)bytes[index + 5] << 40;
          goto case 5;
        case 5:
          k1 ^= (ulong)bytes[index + 4] << 32;
          goto case 4;
        case 4:
          k1 ^= (ulong)bytes[index + 3] << 24;
          goto case 3;
        case 3:
          k1 ^= (ulong)bytes[index + 2] << 16;
          goto case 2;
        case 2:
          k1 ^= (ulong)bytes[index + 1] << 8;
          goto case 1;
        case 1:
          k1 ^= (ulong)bytes[index];
          break;
        default:
          throw new Exception("Something went wrong with remaining bytes calculation.");
      }
      
      _h1 ^= MixKey1(k1);
      _h2 ^= MixKey2(k2);
    }
    
    /// <summary>
    /// Get the complete hash byte array.
    /// </summary>
    public byte[] Hash {
      get {
        _h1 ^= (ulong)_length;
        _h2 ^= (ulong)_length;
        
        _h1 += _h2;
        _h2 += _h1;
        
        _h1 = Murmur3.MixFinal(_h1);
        _h2 = Murmur3.MixFinal(_h2);
        
        _h1 += _h2;
        _h2 += _h1;
        
        var hash = new byte[Murmur3.ReadSize];
        
        Array.Copy(BitConverter.GetBytes(_h1), 0, hash, 0, 8);
        Array.Copy(BitConverter.GetBytes(_h2), 0, hash, 8, 8);
        
        return hash;
      }
    }
    
    private void MixBody(ulong k1, ulong k2) {
      _h1 ^= MixKey1(k1);
      
      _h1 = (_h1 << 27) | (_h1 >> (64 - 27));
      _h1 += _h2;
      _h1 = _h1 * 5 + 0x52dce729;
      
      _h2 ^= MixKey2(k2);
      
      _h2 = (_h2 << 31) | (_h2 >> (64 - 31));
      _h2 += _h1;
      _h2 = _h2 * 5 + 0x38495ab5;
    }
    
    private static ulong MixKey1(ulong k1) {
      k1 *= _c1;
      k1 = (k1 << 31) | (k1 >> (64 - 31));
      k1 *= _c2;
      return k1;
    }
    
    private static ulong MixKey2(ulong k2) {
      k2 *= _c2;
      k2 = (k2 << 33) | (k2 >> (64 - 33));
      k2 *= _c1;
      return k2;
    }
    
    private static ulong MixFinal(ulong k) {
      // avalanche bits
      k ^= k >> 33;
      k *= 0xff51afd7ed558ccdL;
      k ^= k >> 33;
      k *= 0xc4ceb9fe1a85ec53L;
      k ^= k >> 33;
      return k;
    }
    
    /// <summary>
    /// Get the value of a ulong value at the specified index within the byte array.
    /// </summary>
    private unsafe static ulong GetUInt64(byte[] bb, int index) {
      // we only read aligned longs, so a simple casting is enough
      fixed (byte* pbyte = &bb[index]) {
        return *((ulong*)pbyte);
      }
    }
  }
}
