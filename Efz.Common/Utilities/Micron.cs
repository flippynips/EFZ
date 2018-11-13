/*
 * User: Joshua
 * Date: 13/08/2016
 * Time: 8:20 PM
 */
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

using SharpDX;

namespace Efz {
  
  /// <summary>
  /// Micro management of memory. Here be dragons.
  /// </summary>
  [ComVisible(true)]
  public static class Micron {
    
    /// <summary>
    /// External memory copy method.
    /// </summary>
    [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false), SuppressUnmanagedCodeSecurity]
    private static unsafe extern void* memcpy(void* dest, void* src, ulong length);
    
    /// <summary>
    /// External memory move method.
    /// </summary>
    [DllImport("msvcrt.dll", EntryPoint = "memmove", CallingConvention = CallingConvention.Cdecl, SetLastError = false), SuppressUnmanagedCodeSecurity]
    private static unsafe extern void* memmove(void* dest, void* src, int length);
    
    /// <summary>
    /// External copy value method.
    /// </summary>
    [DllImport("mscorlib.dll", EntryPoint = "cpobj", CallingConvention = CallingConvention.Cdecl, SetLastError = false), SuppressUnmanagedCodeSecurity]
    private static unsafe extern void* cpobj(void* dest, void* src);
    
    /// <summary>
    /// TODO : Implement, Exploit, ???, Profit
    /// </summary>
    [DllImport("advapi32.dll", SetLastError=true)]
    static extern bool ImpersonateLoggedOnUser(IntPtr hToken);
    
    //-------------------------------------------//
    
    /// <summary>
    /// Block move a length of bytes from one location to another.
    /// </summary>
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static void CopyMemory(byte[] source, byte[] destination, int length) {
      
      // use the best method for the length of bytes to be copied
      if(length < 1048576) {
        // use buffer block copy for 1 Megabyte and less
        Buffer.BlockCopy(source, 0, destination, 0, (int)length);
      } else {
        // use Marshal Copy
        fixed (void* pDest = &destination[0]) {
          Marshal.Copy(source, 0, (IntPtr)pDest, (int)length);
        }
      }
      
    }
    
    /// <summary>
    /// Block move a length of bytes from one location to another.
    /// </summary>
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static void CopyMemory(byte[] source, int sourceIndex, byte[] destination, int destinationIndex, int length) {
      
      // use the best method for the length of bytes to be copied
      if(length < 220000) {
        // use buffer block copy
        Buffer.BlockCopy(source, sourceIndex, destination, destinationIndex, (int)length);
      } else {
        // use Marshal Copy
        fixed (void* pDest = &destination[destinationIndex]) {
          Marshal.Copy(source, sourceIndex, (IntPtr)pDest, (int)length);
        }
      }
      
    }
    
    /// <summary>
    /// Block move a series of pointer values from one location to another.
    /// </summary>
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static void CopyMemory(void* source, void* destination, int length) {
      
      // only an internal memcpy call
      Interop.memcpy(destination, source, length);
      //RtlMoveMemory((IntPtr)destination, (IntPtr)source, (uint)length);
      
    }
    
    /// <summary>
    /// Cast one type as another in an unsafe manner.
    /// </summary>
//    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success),
//    MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public unsafe static TDst Cast<TSrc, TDst>(TSrc source)
//      where TSrc : struct
//      where TDst : struct {
//      
//      // get pointer to the source
//      fixed (void* pSource = &source) {
//        // perform and return the cast
//        TDst destination;
//        return (TDst)Interop.Read<TDst>(pSource, ref destination);
//      }
//      
//      
//      
//    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Validate a definition of a block of pointers.
    /// </summary>
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe void ValidateBlock(byte* _destination, byte* _source, uint _length) {
      if (_destination - _source >= _length) {
        switch (_length) {
          case 0u:
            return;
          case 1u:
            *_destination = *_source;
            return;
          case 2u:
            *(short*)_destination = *(short*)_source;
            return;
          case 3u:
            *(short*)_destination = *(short*)_source;
            _destination[2 / 1] = *(_source + 2 / 1);
            return;
          case 4u:
            *(int*)_destination = *(int*)_source;
            return;
          case 5u:
            *(int*)_destination = *(int*)_source;
            _destination[4 / 1] = *(_source + 4 / 1);
            return;
          case 6u:
            *(int*)_destination = *(int*)_source;
            *(short*)(_destination + 4 / 1) = *(short*)(_source + 4 / 1);
            return;
          case 7u:
            *(int*)_destination = *(int*)_source;
            *(short*)(_destination + 4 / 1) = *(short*)(_source + 4 / 1);
            _destination[6 / 1] = *(_source + 6 / 1);
            return;
          case 8u:
            *(int*)_destination = *(int*)_source;
            *(int*)(_destination + 4 / 1) = *(int*)(_source + 4 / 1);
            return;
          case 9u:
            *(int*)_destination = *(int*)_source;
            *(int*)(_destination + 4 / 1) = *(int*)(_source + 4 / 1);
            _destination[8 / 1] = *(_source + 8 / 1);
            return;
          case 10u:
            *(int*)_destination = *(int*)_source;
            *(int*)(_destination + 4 / 1) = *(int*)(_source + 4 / 1);
            *(short*)(_destination + 8 / 1) = *(short*)(_source + 8 / 1);
            return;
          case 11u:
            *(int*)_destination = *(int*)_source;
            *(int*)(_destination + 4 / 1) = *(int*)(_source + 4 / 1);
            *(short*)(_destination + 8 / 1) = *(short*)(_source + 8 / 1);
            _destination[10 / 1] = *(_source + 10 / 1);
            return;
          case 12u:
            *(int*)_destination = *(int*)_source;
            *(int*)(_destination + 4 / 1) = *(int*)(_source + 4 / 1);
            *(int*)(_destination + 8 / 1) = *(int*)(_source + 8 / 1);
            return;
          case 13u:
            *(int*)_destination = *(int*)_source;
            *(int*)(_destination + 4 / 1) = *(int*)(_source + 4 / 1);
            *(int*)(_destination + 8 / 1) = *(int*)(_source + 8 / 1);
            _destination[12 / 1] = *(_source + 12 / 1);
            return;
          case 14u:
            *(int*)_destination = *(int*)_source;
            *(int*)(_destination + 4 / 1) = *(int*)(_source + 4 / 1);
            *(int*)(_destination + 8 / 1) = *(int*)(_source + 8 / 1);
            *(short*)(_destination + 12 / 1) = *(short*)(_source + 12 / 1);
            return;
          case 15u:
            *(int*)_destination = *(int*)_source;
            *(int*)(_destination + 4 / 1) = *(int*)(_source + 4 / 1);
            *(int*)(_destination + 8 / 1) = *(int*)(_source + 8 / 1);
            *(short*)(_destination + 12 / 1) = *(short*)(_source + 12 / 1);
            _destination[14 / 1] = *(_source + 14 / 1);
            return;
          case 16u:
            *(int*)_destination = *(int*)_source;
            *(int*)(_destination + 4 / 1) = *(int*)(_source + 4 / 1);
            *(int*)(_destination + 8 / 1) = *(int*)(_source + 8 / 1);
            *(int*)(_destination + 12 / 1) = *(int*)(_source + 12 / 1);
            return;
          default:
            if (_length < 512u) {
              if ((*_destination & 3) != 0) {
                if ((*_destination & 1) != 0) {
                  *_destination = *_source;
                  _source++;
                  _destination++;
                  _length -= 1u;
                  if ((*_destination & 2) != 0) {
                    *(short*)_destination = *(short*)_source;
                    _source += 2 / 1;
                    _destination += 2 / 1;
                    _length -= 2u;
                  }
                } else {
                  *(short*)_destination = *(short*)_source;
                  _source += 2 / 1;
                  _destination += 2 / 1;
                  _length -= 2u;
                }
              }
              for (uint num = _length / 16u; num > 0u; num -= 1u) {
                *(int*)_destination = *(int*)_source;
                *(int*)(_destination + 4 / 1) = *(int*)(_source + 4 / 1);
                *(int*)(_destination + *(int*)2 * 4 / 1) = *(int*)(_source + *(int*)2 * 4 / 1); // 2s had (IntPtr)
                *(int*)(_destination + *(int*)3 * 4 / 1) = *(int*)(_source + *(int*)3 * 4 / 1); // 3s had (IntPtr)
                _destination += 16 / 1;
                _source += 16 / 1;
              }
              if ((_length & 8u) != 0u) {
                *(int*)_destination = *(int*)_source;
                *(int*)(_destination + 4 / 1) = *(int*)(_source + 4 / 1);
                _destination += 8 / 1;
                _source += 8 / 1;
              }
              if ((_length & 4u) != 0u) {
                *(int*)_destination = *(int*)_source;
                _destination += 4 / 1;
                _source += 4 / 1;
              }
              if ((_length & 2u) != 0u) {
                *(short*)_destination = *(short*)_source;
                _destination += 2 / 1;
                _source += 2 / 1;
              }
              if ((_length & 1u) != 0u) {
                *_destination = *_source;
              }
              return;
            }
            break;
        }
      }
    }
    
    
  }
}
