using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

using Efz.Network;
using Efz.Threading;

namespace Efz.Tools {
  
  /// <summary>
  /// Platform.
  /// </summary>
  public enum Platform : byte {
    Unknown  = 0,
    Windows  = 1,
    Linux    = 2,
    Mac      = 3,
    Android  = 4,
  }
  
  /// <summary>
  /// Singleton responsible for retrieving and maintaining useful system information.
  /// This includes memory and processing capabilities.
  /// </summary>
  public class SystemInformation : Singleton<SystemInformation> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Current platform of the application.
    /// </summary>
    public static Platform Platform = Platform.Unknown;
    
    /// <summary>
    /// Reference to the current process.
    /// </summary>
    public static Process Process;
    
    /// <summary>
    /// New line character local to the platform.
    /// </summary>
    public static string NewLine;
    
    /// <summary>
    /// Estimate of total memory.
    /// </summary>
    public static ulong MemoryTotal;
    /// <summary>
    /// Estimate of available memory.
    /// </summary>
    public static ulong MemoryAvailable;
    /// <summary>
    /// Memory paging size.
    /// </summary>
    public static int PageSize;
    
    /// <summary>
    /// Number of processors available.
    /// </summary>
    public static int ProcessorCount;
    /// <summary>
    /// Is the application running for a 64 bit process?
    /// </summary>
    public static bool Processor64Bit;
    
    /// <summary>
    /// Name of the user that ran the application.
    /// </summary>
    public static string NameUser;
    /// <summary>
    /// Name of the current machine.
    /// </summary>
    public static string NameMachine;
    /// <summary>
    /// Name of this process instance.
    /// </summary>
    public static string NameInstance;
    /// <summary>
    /// Id of the current process.
    /// </summary>
    public static int ProcessId;
    
    //-------------------------------------------//
    
    private static MemoryStatus _memoryStatus;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    public SystemInformation() {
      
      // get environment information
      PageSize       = Environment.SystemPageSize;
      ProcessorCount = Environment.ProcessorCount;
      Processor64Bit = Environment.Is64BitProcess;
      NameUser       = Environment.UserName;
      NameMachine    = Environment.MachineName;
      NameInstance   = Process.GetCurrentProcess().Id.ToString();
      
      // validate processor count
      if(ProcessorCount < 1) ProcessorCount = 1;
      
      // get current process
      Process = Process.GetCurrentProcess();
      // get process id
      ProcessId = Process.Id;
      
      // determine the platform
      int platformId = (int)Environment.OSVersion.Platform;
      if(platformId == 4 || platformId == 6 || platformId == 128) {
        Platform = Platform.Linux;
      } else {
        switch(Environment.OSVersion.Platform) {
          case PlatformID.MacOSX:
            Platform = Platform.Mac;
            break;
          case PlatformID.Unix:
            Platform = Platform.Linux;
            break;
          case PlatformID.Win32NT:
          case PlatformID.Win32S:
          case PlatformID.Win32Windows:
          case PlatformID.WinCE:
            Platform = Platform.Windows;
            break;
          default:
            Platform = Platform.Unknown;
            break;
        }
      }
      
      switch(Platform) {
        case Platform.Windows:
          NewLine = "\r\n";
          break;
        default:
          NewLine = "\n";
          break;
      }
      
      // memory flag
      bool memorySet = false;
      
      // attempt to get memory state using kernal32.dll method
      try {
        _memoryStatus = new MemoryStatus();
        if(MemoryStatus.GlobalMemoryStatusEx(_memoryStatus)) {
          MemoryTotal = _memoryStatus.ullTotalPhys;
          MemoryAvailable = _memoryStatus.ullAvailPhys;
          memorySet = MemoryAvailable > 0;
        }
        // disable once EmptyGeneralCatchClause
      } catch {
        // ignore
      }
      
      // attempt to get available memory through null allocation
      if(!memorySet) {
        // Approximate the available memory by subtracting the current processes resource use.
        MemoryAvailable = MemoryTotal = (ulong)Process.PeakWorkingSet64 - (ulong)Process.WorkingSet64;
        memorySet = MemoryAvailable > 0;
      }
      
    }
    
    /// <summary>
    /// Get average statistics of the drive of the specified directory path. Resulting value units
    /// are bytes and per second.
    /// Note : This method writes a Megabyte to the Stream and then clears it.
    /// </summary>
    public static void GetStreamStats(string directory, out uint seeks, out uint writes, out uint reads) {
      // create a test file
      string filePath = ManagerResources.CreateFilePath(directory, ".drivetest");
      
      // get a stream to the created test file
      Teple<LockShared, ByteBuffer> resource;
      ManagerConnections.Get<ByteBuffer, ConnectionLocal>(filePath, out resource);
      
      ByteBuffer stream = resource.ArgB;
      
      // get a seek, read and write speed for the Author
      Timekeeper time = new Timekeeper();
      
      // write a Megabyte
      time.Start();
      stream.Write(Generic.Series<byte>((int)Global.Megabyte, 50));
      stream.Stream.Flush();
      time.Stop();
      
      // derive the number of bytes written per second
      writes = (uint)(Global.Megabyte / (time.Milliseconds / 1000));
      
      // perform a number of seeks
      time.Start();
      for(int i = 1000; i >= 0; --i) {
        stream.Position = 0;
        stream.Write((byte)0);
        stream.Stream.Flush();
        stream.Position = (long)Global.Megabyte - 1L;
        stream.Write((byte)0);
        stream.Stream.Flush();
      }
      time.Stop();
      
      // derive the number of seeks per second
      seeks = (uint)(1000 / (time.Milliseconds / 1000));
      stream.Position = 0;
      
      // read the Megabyte
      time.Start();
      stream.ReadBytes((int)Global.Megabyte);
      stream.Stream.Flush();
      time.Stop();
      
      // derive the number of bytes read per second
      reads = (uint)(Global.Megabyte / (time.Milliseconds / 1000));
      
      // release the connection
      resource.ArgA.Release();
      
      // remove the files
      File.Delete(filePath);
    }
    
    //-------------------------------------------//
    
  }
  
}

/// <summary>
/// Class required to receive system information from a kernal.dll method.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class MemoryStatus {
  
  //-------------------------------------------//
  
  public uint dwLength;
  public uint dwMemoryLoad;
  public ulong ullTotalPhys;
  public ulong ullAvailPhys;
  public ulong ullTotalPageFile;
  public ulong ullAvailPageFile;
  public ulong ullTotalVirtual;
  public ulong ullAvailVirtual;
  public ulong ullAvailExtendedVirtual;
  
  //-------------------------------------------//
  
  [return: MarshalAs(UnmanagedType.Bool)]
  [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
  public static extern bool GlobalMemoryStatusEx([In, Out] MemoryStatus lpBuffer);
  
  //-------------------------------------------//

  public MemoryStatus() {
    this.dwLength = (uint)Marshal.SizeOf(typeof(MemoryStatus));
  }

  //-------------------------------------------//


  //-------------------------------------------//


}