/*
 * User: FloppyNipples
 * Date: 02/06/2017
 * Time: 00:43
 */
using System;
using System.Collections.Generic;
using System.IO;

using Efz.Threading;
using Efz.Tools;

namespace Efz.Data.Media {
  
  /// <summary>
  /// Media metadata action to remove metadata.
  /// </summary>
  public class MetaRemoval : IAction {
    
    //------------------------------//
    
    /// <summary>
    /// Supported file types.
    /// </summary>
    public static HashSet<string> Supported;
    
    /// <summary>
    /// Path to the temporary media file.
    /// </summary>
    public string Path;
    /// <summary>
    /// Output stream for the resulting cleared media.
    /// </summary>
    public Stream Output;
    /// <summary>
    /// Callback on metadata removal completion.
    /// </summary>
    public IAction<MetaRemoval> OnComplete;
    /// <summary>
    /// Flag indicating success.
    /// </summary>
    public bool Success;
    
    //------------------------------//
    
    /// <summary>
    /// Flag indicating the media file should be deleted once the operation is complete.
    /// </summary>
    protected bool _removeTempFile;
    /// <summary>
    /// Bytes representing flags used to remove metadata.
    /// </summary>
    protected static byte[] _bytesRemoveMetadata;
    /// <summary>
    /// Coordinator for the media meta removal process.
    /// </summary>
    protected MediaCoordinator _coordinator;
    
    /// <summary>
    /// Lock used to ensure the meta removal process isn't run until the temporary file is written.
    /// </summary>
    protected Lock _writingLock;
    
    //------------------------------//
    
    /// <summary>
    /// Initialize removal of metadata.
    /// </summary>
    static MetaRemoval() {
      // create the byte collection used to remove metadata
      _bytesRemoveMetadata = System.Text.Encoding.ASCII.GetBytes(
        "-overwrite_original"+SystemInformation.NewLine+
        "/"+SystemInformation.NewLine);
      
      // compile a collection of file types that exiftool can remove metadata from
      Supported = new HashSet<string> {
        "video/3gpp",
        "video/3gpp2",
        "audio/vnd.audible.aax",
        "application/postscript",
        "image/x-flif",
        "image/gif",
        "image/jpeg",
        "audio/m4a",
        "video/quicktime",
        //"video/mp4", causes the mp4 metadata to be corrupted
        "image/x-portable-bitmap",
        "application/pdf",
        "image/x-portable-graymap",
        "image/png",
        "image/x-portable-pixmap",
        "image/x-quicktime",
        "image/tiff",
        "image/vnd.ms-photo"
      };
    }
    
    /// <summary>
    /// Remove metadata from the media found in the input stream.
    /// </summary>
    public MetaRemoval(MediaCoordinator coordinator, Stream input, Stream output, IAction<MetaRemoval> onComplete, bool removeTempFile = true) {
      
      _coordinator = coordinator;
      _removeTempFile = removeTempFile;
      
      // create a name for the media
      Path = Fs.Combine(_coordinator.MediaPath, Guid.NewGuid().ToString());
      Output = output;
      OnComplete = onComplete;
      OnComplete.ArgA = this;
      
      try {
        
        // open a stream to the file
        FileStream file = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.Read);
        // copy the stream into a temporary file
        WriteTemporaryFile(file, input);
        
      } catch(IOException ex) {
        
        // if the file is still being used
        if(ex.HResult == -2147024864) {
          // try twice more
          ManagerUpdate.Iterant.AddSingle(TryWriteTemporaryFile, input, 0);
          return;
        }
        
      }
      
    }
    
    /// <summary>
    /// Remove metadata from the media found in the input stream.
    /// </summary>
    public MetaRemoval(MediaCoordinator coordinator, string path, Stream output, IAction<MetaRemoval> onComplete, bool removeFile = true) {
      
      _coordinator = coordinator;
      _removeTempFile = removeFile;
      
      // create a name for the media
      Path = path;
      Output = output;
      OnComplete = onComplete;
      OnComplete.ArgA = this;
      
      var process = _coordinator.Process.TakeItem();
      process.StandardInput.BaseStream.Write(_bytesRemoveMetadata, 0, _bytesRemoveMetadata.Length);
      byte[] pathBytes = System.Text.Encoding.ASCII.GetBytes(Path);
      process.StandardInput.BaseStream.Write(pathBytes, 0, pathBytes.Length);
      process.StandardInput.BaseStream.Write(_coordinator.BytesExecute, 0, _coordinator.BytesExecute.Length);
      process.StandardInput.BaseStream.Flush();
      _coordinator.Process.Release();
      
      // add this task to run with the coordinators process
      _coordinator.Add(this);
    }
    
    /// <summary>
    /// Run the medadata removal process.
    /// </summary>
    public void Run() {
      
      bool found = false;
      if(_coordinator.Buffer != null) {
        while(_coordinator.BufferIndex < _coordinator.BufferCount) {
          if(_coordinator.ReadySearch.Next(_coordinator.Buffer[_coordinator.BufferIndex])) {
            ++_coordinator.BufferIndex;
            found = true;
            break;
          }
          ++_coordinator.BufferIndex;
        }
        if(_coordinator.BufferIndex == _coordinator.BufferCount) _coordinator.Buffer = null;
      }
      
      if(!found) {
        
        // read the {ready} flag from the exiftool
        var buffer = BufferCache.Get();
        var process = _coordinator.Process.TakeItem();
        int count = process.StandardOutput.BaseStream.Read(buffer, 0, Global.BufferSizeLocal);
        
        // while there are bytes in the process standard output
        while(count > 0) {
          int index = 0;
          
          while(index < count) {
            // check the buffer for the {ready} flag
            if(_coordinator.ReadySearch.Next(buffer[index])) {
              ++index;
              // are there bytes in the buffer?
              if(index == count) {
                // no, persist the buffer
                BufferCache.Set(buffer);
              } else {
                // yes, remember the buffer and position
                _coordinator.Buffer = buffer;
                _coordinator.BufferIndex = index;
                _coordinator.BufferCount = count;
              }
              found = true;
              break;
            }
            ++index;
          }
          if(found) break;
          count = process.StandardOutput.BaseStream.Read(buffer, 0, Global.BufferSizeLocal);
        }
        _coordinator.Process.Release();
      }
      
      // should the file be read?
      if(Output == null) {
        // no, run callback
        OnComplete.Run();
        
        // should the file be removed?
        if(_removeTempFile) ManagerResources.RemoveFile(Path);
        return;
      }
      
      FileStream fileStream;
      try {
        fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
      } catch(IOException ex) {
        // if the file is still being used
        if(ex.HResult == -2147024864) {
          // try twice more
          ManagerUpdate.Iterant.AddSingle(TryReadTemporaryFile, 0);
          return;
        }
        throw;
      }
      
      // get a stream to the clean file
      ManagerUpdate.Control.AddSingle(ReadTemporaryFile, fileStream);
    }
    
    //------------------------------//
    
    /// <summary>
    /// Try reading the temporary file again.
    /// </summary>
    private void TryReadTemporaryFile(int index) {
      if(index == 3) {
        Log.Error("Temporary file '"+Path+"' couldn't be read.");
        return;
      }
      
      FileStream fileStream;
      try {
        fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
      } catch(IOException ex) {
        // if the file is still being used
        if(ex.HResult == -2147024864) {
          // try again
          ManagerUpdate.Iterant.AddSingle(TryReadTemporaryFile, ++index);
          return;
        }
        throw;
      }
      
      // get a stream to the clean file
      ManagerUpdate.Control.AddSingle(ReadTemporaryFile, fileStream);
    }
    
    /// <summary>
    /// Pass the 'inStream' through exiftool and write the output to the 'outStream'.
    /// </summary>
    private void ReadTemporaryFile(Stream file) {
      
      // get a buffer
      byte[] buffer = BufferCache.Get();
      // read from the stream
      int count = file.Read(buffer, 0, Global.BufferSizeLocal);
      
      // write the buffer to the output stream
      Output.Write(buffer, 0, count);
      
      // is this the final buffer?
      if(count < Global.BufferSizeLocal) {
        
        // yes, dispose of the file stream
        file.Dispose();
        
        // reset the buffer
        BufferCache.Set(buffer);
        
        Success = true;
        
        // run callback
        OnComplete.Run();
        
        // should the file be removed?
        if(_removeTempFile) ManagerResources.RemoveFile(Path);
        
      } else {
        
        // no, reset the buffer
        BufferCache.Set(buffer);
        
        // add a task to clear the metadata
        ManagerUpdate.Control.AddSingle(ReadTemporaryFile, file);
      }
      
    }
    
    /// <summary>
    /// Attempt to get write permissions for the file
    /// </summary>
    private void TryWriteTemporaryFile(Stream input, int index) {
      if(index == 3) {
        Log.Error("Temporary file '"+Path+"' couldn't be read.");
        return;
      }
      
      try {
        
        // open a stream to the file
        FileStream file = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.Read);
        // copy the stream into a temporary file
        WriteTemporaryFile(file, input);
        
      } catch(IOException ex) {
        // if the file is still being used
        if(ex.HResult == -2147024864) {
          // try twice more
          ManagerUpdate.Iterant.AddSingle(TryWriteTemporaryFile, input, ++index);
          return;
        }
      }
      
    }
    
    /// <summary>
    /// Pass the 'inStream' through exiftool and write the output to the 'outStream'.
    /// </summary>
    private void WriteTemporaryFile(Stream file, Stream input) {
      
      // get a buffer
      var buffer = BufferCache.Get();
      // read from the input stream
      int count = input.Read(buffer, 0, Global.BufferSizeLocal);
      // write the buffer to the file
      file.Write(buffer, 0, count);
      // reset the buffer
      BufferCache.Set(buffer);
      
      // is this the final buffer?
      if(count < Global.BufferSizeLocal) {
        
        // yes, dispose of the file stream
        file.Dispose();
        
        var process = _coordinator.Process.TakeItem();
        process.StandardInput.BaseStream.Write(_bytesRemoveMetadata, 0, _bytesRemoveMetadata.Length);
        byte[] pathBytes = System.Text.Encoding.ASCII.GetBytes(Path);
        process.StandardInput.BaseStream.Write(pathBytes, 0, pathBytes.Length);
        process.StandardInput.BaseStream.Write(_coordinator.BytesExecute, 0, _coordinator.BytesExecute.Length);
        process.StandardInput.BaseStream.Flush();
        _coordinator.Process.Release();
        
        // add this task to run with the coordinators process
        _coordinator.Add(this);
        
      } else {
        
        // continue writing the temporary file
        ManagerUpdate.Control.AddSingle(WriteTemporaryFile, file, input);
      }
    }
    
  }
}
