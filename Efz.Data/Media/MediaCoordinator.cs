/*
 * User: FloppyNipples
 * Date: 02/06/2017
 * Time: 01:20
 */
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

using Efz.Threading;
using Efz.Tools;

namespace Efz.Data.Media {
  
  /// <summary>
  /// Coordinator for tasks relating to the exiftool process.
  /// </summary>
  public class MediaCoordinator {
    
    //------------------------------//
    
    /// <summary>
    /// Path to the directory used for media manipulation.
    /// </summary>
    public string MediaPath;
    
    /// <summary>
    /// Process this coordinator manages.
    /// </summary>
    public Shared<Process> Process;
    
    /// <summary>
    /// Bytes to be written to the stream after each command.
    /// </summary>
    internal readonly byte[] BytesExecute;
    /// <summary>
    /// Tree used to find the {ready} flag to indicate a command is complete.
    /// </summary>
    internal readonly TreeSearch<byte, bool> ReadyTree;
    /// <summary>
    /// Bytes in the ready flag.
    /// </summary>
    internal readonly int ReadyLength;
    /// <summary>
    /// Dynamic search.
    /// </summary>
    internal readonly TreeSearch<byte, bool>.DynamicSearch ReadySearch;
    
    /// <summary>
    /// Temporary buffer used when reading from the exiftool process.
    /// </summary>
    internal byte[] Buffer;
    /// <summary>
    /// Temporary buffer index used when reading from the exiftool process.
    /// </summary>
    internal int BufferIndex;
    /// <summary>
    /// Temporary buffer count used when reading from the exiftool process.
    /// </summary>
    internal int BufferCount;
    
    //------------------------------//
    
    /// <summary>
    /// Sequence of actions with meta tasks.
    /// </summary>
    private ActionSequence _sequencer;
    
    //------------------------------//
    
    public MediaCoordinator() {
      MediaPath = Path.GetTempPath();
      
      // create the sequencer which will run meta tasks
      _sequencer = new ActionSequence();
      
      // create the byte collection to execute the prior commands
      BytesExecute = System.Text.Encoding.ASCII.GetBytes(SystemInformation.NewLine + "-execute" + SystemInformation.NewLine);
      
      // create the tree search for the {ready} delimiter
      ReadyTree = new TreeSearch<byte, bool>();
      var readyBytes = System.Text.Encoding.ASCII.GetBytes("{ready}");
      ReadyLength = readyBytes.Length;
      ReadyTree.Add(true, readyBytes);
      ReadySearch = ReadyTree.SearchDynamic();
      
      try {
        StartExifTool();
      } catch(Exception ex) {
        Log.Error("There was a problem starting the exif tool.", ex);
      }
    }
    
    /// <summary>
    /// Remove metadata from the media contained in the input stream.
    /// </summary>
    public void RemoveMetadata(Stream input, Stream output, IAction<MetaRemoval> onComplete, bool removeTempFile = true) {
      new MetaRemoval(this, input, output, onComplete, removeTempFile);
    }
    
    /// <summary>
    /// Remove metadata from the media contained in the media at the specified path.
    /// </summary>
    public void RemoveMetadata(string path, Stream output, IAction<MetaRemoval> onComplete, bool removeFile = true) {
      new MetaRemoval(this, path, output, onComplete, removeFile);
    }
    
    /// <summary>
    /// Retrieve standard metadata from the media in the input stream.
    /// </summary>
    public void RetrieveMetadata(Stream input, IAction<MetaRetrieval> onComplete, bool removeTempFile = true) {
      new MetaRetrieval(this, input, onComplete, removeTempFile);
    }
    
    /// <summary>
    /// Retrieve standard metadata from the media at the specified path.
    /// </summary>
    public void RetrieveMetadata(string path, IAction<MetaRetrieval> onComplete, bool removeFile = true) {
      new MetaRetrieval(this, path, onComplete, removeFile);
    }
    
    /// <summary>
    /// Create a thumbnail of the media file if possible. Callback flag indicates success.
    /// </summary>
    public static void CreateThumbnail(string mimeType, Stream input, Stream output, IAction<bool> onComplete, int size) {
      
      // determine action based on type of image
      switch(mimeType) {
        case "image/bmp":
        case "image/jpeg":
        case "image/png":
        case "image/gif":
        case "image/tiff":
          
          Bitmap source;
          int width;
          int height;
          try {
            source = new Bitmap(input);
            width = source.Width;
            height = source.Height;
          } catch {
            // image not valid for thumbnail creation
            onComplete.ArgA = false;
            onComplete.Run();
            return;
          }
          
          if(width == 0 || height == 0) {
            onComplete.ArgA = false;
            onComplete.Run();
            return;
          }
          
          int newWidth;
          int newHeight;
          
          if(width > height) {
            newWidth = size;
            newHeight = (int)(size * ((double)height/width));
          } else {
            newWidth = (int)(size * ((double)width/height));
            newHeight = size;
          }
          
          try {
            
            if(mimeType == "image/gif" && source.GetFrameCount(System.Drawing.Imaging.FrameDimension.Time) > 1) {
              source.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Time, 0);
            }
            
            using(source)
            using(Bitmap thumb = new Bitmap(newWidth, newHeight))
            using(Graphics graphics = Graphics.FromImage(thumb)) {
              graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
              graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
              graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
              graphics.DrawImage(source, 0, 0, newWidth, newHeight);
              
              // save the thumbnail as a jpeg
              thumb.Save(output, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            
          } catch(Exception ex) {
            
            // log the exception
            Log.Error("Error creating thumbnail.", ex);
            
            // callback fail
            onComplete.ArgA = false;
            onComplete.Run();
            return;
            
          }
          break;
        default:
          
          // unhandled mime type
          onComplete.ArgA = false;
          onComplete.Run();
          
          return;
      }
      
      onComplete.ArgA = true;
      onComplete.Run();
    }
    
    /// <summary>
    /// Internal addition of a metadata task when the task is ready to run.
    /// </summary>
    internal void Add(IAction metaTask) {
      _sequencer.AddRun(metaTask);
    }
    
    //------------------------------//
    
    /// <summary>
    /// Start the exiftool process.
    /// </summary>
    private void StartExifTool() {
      
      var startInfo = new ProcessStartInfo {
        #if DEBUG
        RedirectStandardError = true,
        #endif
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        UseShellExecute = false,
        FileName = SystemInformation.Platform == Platform.Windows ? "Tools/exiftool.exe" : "exiftool",
        Arguments = "-stay_open 1 -@ - -api largefilesupport=1"
      };
      
      // log the initialization of exiftool
      Log.Info("Starting exiftool '"+startInfo.FileName+"'.");
      
      // start exiftool
      Process = new Shared<Process>();
      Process.Take();
      Process.Item = System.Diagnostics.Process.Start(startInfo);
      Process.Release();
      
      #if DEBUG
      // subscribe to errors from the exiftool process
      Process.TakeItem().ErrorDataReceived += OnError;
      Process.Item.BeginErrorReadLine();
      Process.Release();
      #endif
      
      ManagerUpdate.OnEnd.Add("ExifTool", Dispose);
    }
    
    /// <summary>
    /// Dispose of the media control components.
    /// </summary>
    private void Dispose() {
      
      var closeArgs = System.Text.Encoding.ASCII.GetBytes("-stay_open"+SystemInformation.NewLine+"0"+SystemInformation.NewLine+"-execute");
      
      try {
        
        Process.Take();
        #if DEBUG
        // cancel debug error information
        Process.Item.CancelErrorRead();
        Process.Item.ErrorDataReceived -= OnError;
        #endif
        // write closing arguments
        Process.Item.StandardInput.BaseStream.Write(closeArgs, 0, closeArgs.Length);
        Process.Item.StandardInput.BaseStream.Flush();
        
        // has the process exited?
        if(!Process.Item.HasExited) {
          // no, wait for the process to exit
          Process.Item.WaitForExit(500);
          // has the process exited?
          if(!Process.Item.HasExited) {
            // no, kill the process
            Process.Item.Kill();
            // wait for the process to exit
            Process.Item.WaitForExit(500);
            // has the process exited? no, log
            if(!Process.Item.HasExited) Log.Warning("Exiftool didn't exit.");
          }
          // dispose of the process instance
          Process.Item.Dispose();
        }
        
      } catch(Exception ex) {
        Log.Error("Failed to close exiftool.", ex);
        return;
      } finally {
        // release the process
        Process.Release();
      }
      
    }
    
    #if DEBUG
    /// <summary>
    /// Log errors from exiftool.
    /// </summary>
    private void OnError(object sender, DataReceivedEventArgs args) {
      if(!string.IsNullOrEmpty(args.Data)) Log.Error("Exiftool error : " + args.Data);
    }
    #endif
    
  }
  
}
