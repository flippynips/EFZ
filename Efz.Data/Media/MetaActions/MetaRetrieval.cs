/*
 * User: FloppyNipples
 * Date: 02/06/2017
 * Time: 00:43
 */
using System;
using System.Collections.Generic;
using System.IO;
using Efz.Tools;

namespace Efz.Data.Media {
  
  /// <summary>
  /// Media metadata action to remove metadata.
  /// </summary>
  public class MetaRetrieval : IAction {
    
    //------------------------------//
    
    /// <summary>
    /// Path to the temporary media file.
    /// </summary>
    public string Path;
    /// <summary>
    /// Callback on metadata removal completion.
    /// </summary>
    public IAction<MetaRetrieval> OnComplete;
    /// <summary>
    /// Collection of metadata.
    /// </summary>
    public Dictionary<MetaKey, string> Metadata;
    
    /// <summary>
    /// Get the metadata value of the specified key.
    /// </summary>
    public string this[MetaKey key] {
      get {
        string value;
        return Metadata.TryGetValue(key, out value) ? value : string.Empty;
      }
    }
    
    //------------------------------//
    
    /// <summary>
    /// Flag to indicate the file is to be removed on metadata retrieval.
    /// </summary>
    protected bool _removeTempFile;
    
    /// <summary>
    /// Bytes representing flags used to remove metadata.
    /// </summary>
    protected static byte[] _bytesGetMetadata;
    /// <summary>
    /// Coordinator for the media meta removal process.
    /// </summary>
    protected MediaCoordinator _coordinator;
    
    //------------------------------//
    
    /// <summary>
    /// Initialize removal of metadata.
    /// </summary>
    static MetaRetrieval() {
      
      // create the byte collection used to retrieve metadata
      _bytesGetMetadata = System.Text.Encoding.ASCII.GetBytes(
        "-mimetype"+SystemInformation.NewLine+
        "-filesize"+SystemInformation.NewLine+
        "-imagewidth"+SystemInformation.NewLine+
        "-imageheight"+SystemInformation.NewLine+
        "-duration"+SystemInformation.NewLine+
        "-videoframerate"+SystemInformation.NewLine+
        "-audiobitrate"+SystemInformation.NewLine);
      
    }
    
    /// <summary>
    /// Remove metadata from the media found in the input stream.
    /// </summary>
    public MetaRetrieval(MediaCoordinator coordinator, Stream input, IAction<MetaRetrieval> onComplete, bool removeTempFile = true) {
      
      _coordinator = coordinator;
      _removeTempFile = removeTempFile;
      
      // create a name for the media
      Path = Fs.Combine(_coordinator.MediaPath, Guid.NewGuid().ToString());
      OnComplete = onComplete;
      OnComplete.ArgA = this;
      
      // open a stream to the file
      FileStream file = new FileStream(Path, FileMode.Create);
      
      // add a task to copy the stream into a temporary file
      // this then triggers the metadata retrieval
      WriteTemporaryFile(file, input);
    }
    
    /// <summary>
    /// Remove metadata from the media found in the input stream.
    /// </summary>
    public MetaRetrieval(MediaCoordinator coordinator, string path, IAction<MetaRetrieval> onComplete, bool removeFile = true) {
      
      _coordinator = coordinator;
      _removeTempFile = removeFile;
      
      // create a name for the media
      Path = path;
      OnComplete = onComplete;
      OnComplete.ArgA = this;
      
      var process = _coordinator.Process.TakeItem();
      process.StandardInput.BaseStream.Write(_bytesGetMetadata, 0, _bytesGetMetadata.Length);
      byte[] pathBytes = System.Text.Encoding.ASCII.GetBytes(Path);
      process.StandardInput.BaseStream.Write(pathBytes, 0, pathBytes.Length);
      process.StandardInput.BaseStream.Write(_coordinator.BytesExecute, 0, _coordinator.BytesExecute.Length);
      process.StandardInput.BaseStream.Flush();
      _coordinator.Process.Release();
      
      _coordinator.Add(this);
      
    }
    
    /// <summary>
    /// Run the medadata removal process.
    /// </summary>
    public void Run() {
      
      var stream = new ByteBuffer(new MemoryStream());
      
      bool found = false;
      if(_coordinator.Buffer != null) {
        while(_coordinator.BufferIndex < _coordinator.BufferCount) {
          var bit = _coordinator.Buffer[_coordinator.BufferIndex];
          stream.Write(bit);
          ++_coordinator.BufferIndex;
          if(_coordinator.ReadySearch.Next(bit)) {
            found = true;
            break;
          }
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
            var bit = buffer[index];
            stream.Write(bit);
            ++index;
            if(_coordinator.ReadySearch.Next(bit)) {
              // the flag has been found - are there bytes in the buffer?
              if(index == count) {
                BufferCache.Set(buffer);
              } else {
                // yes, remember the buffer
                _coordinator.Buffer = buffer;
                _coordinator.BufferIndex = index;
                _coordinator.BufferCount = count;
              }
              found = true;
              break;
            }
          }
          if(found) break;
          count = process.StandardOutput.BaseStream.Read(buffer, 0, Global.BufferSizeLocal);
        }
        _coordinator.Process.Release();
      }
      
      // should the file be removed?
      if(_removeTempFile) ManagerResources.RemoveFile(Path);
      
      // reset the stream position
      stream.Position = 0;
      
      // create the metadata dictionary for the callback
      Metadata = new Dictionary<MetaKey, string>();
      
      // while there are more bytes to read
      while(stream.Position < stream.WriteEnd) {
        
        string key;
        try {
          
          // read the key of the key value pair
          key = stream.ReadString(Chars.Colon);
          
        } catch {
          
          // ignore and break
          break;
          
        }
        
        // trim the key
        key = key.TrimSpace();
        
        // no, read the value of the key-value pair
        var value = stream.ReadString(Chars.NewLine).TrimSpace();
        
        if(string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)) {
          Log.Debug("Missing metadata '"+key+"' : '"+value+"'.");
          continue;
        }
        
        // parse the key and assign key-value pair
        MetaKey metaKey;
        if(MetaKeys.Map.TryGetValue(key, out metaKey)) {
          
          // determine how to parse the metadata value
          switch(metaKey) {
            case MetaKey.FileSize:
              {
                // determine the scale of the file size
                var splitFileSize = value.Split(value.IndexOf(Chars.Space));
                switch(splitFileSize.ArgB) {
                  case "bytes":
                    Metadata[metaKey] = splitFileSize.ArgA;
                    break;
                  case "MB":
                    Metadata[metaKey] = ((int)(splitFileSize.ArgA.ToDouble() * Global.Megabyte)).ToString();
                    break;
                  case "kB":
                    Metadata[metaKey] = ((int)(splitFileSize.ArgA.ToDouble() * Global.Kilobyte)).ToString();
                    break;
                  case "GB":
                    Metadata[metaKey] = ((int)(splitFileSize.ArgA.ToDouble() * Global.Gigabyte)).ToString();
                    break;
                  default:
                    Log.Error("Unknown file size scale of media '"+value+"'.");
                    break;
                }
              }
              break;
            case MetaKey.Duration:
              {
                // check for (approx) suffix
                int index = value.IndexOf(Chars.Space);
                if(index != -1) value = value.Substring(0, index);
                
                var splitDuration = value.Split(Chars.Colon);
                
                // determine the type of duration
                if(splitDuration.Length == 1) {
                  
                  Metadata[metaKey] = ((int)(splitDuration[0].ToDouble() * 1000)).ToString();
                  
                } else {
                  // parse each component
                  int time = splitDuration[0].ToInt32() * 60 * 60 * 1000;
                  time += splitDuration[1].ToInt32() * 60 * 1000;
                  time += splitDuration[2].ToInt32() * 1000;
                  
                  // assign the time value
                  Metadata[metaKey] = time.ToString();
                }
              }
              break;
            case MetaKey.Bitrate:
              {
                // determine the scale of the file size
                var splitBitrate = value.Split(value.IndexOf(Chars.Space));
                switch(splitBitrate.ArgB) {
                  case "bps":
                    Metadata[metaKey] = splitBitrate.ArgA;
                    break;
                  case "kbps":
                    Metadata[metaKey] = ((int)(splitBitrate.ArgA.ToDouble() * Global.Kilobyte)).ToString();
                    break;
                  case "Mbps":
                    Metadata[metaKey] = ((int)(splitBitrate.ArgA.ToDouble() * Global.Megabyte)).ToString();
                    break;
                  default:
                    Log.Warning("Unknown bitrate scale of media '"+value+"'.");
                    break;
                }
              }
              break;
            default:
              // assign the mime type directly
              Metadata[metaKey] = value;
              break;
          }
        } else {
          Log.Warning("Unrecognized metadata key '"+key+" : "+value+"'.");
        }
        
      }
      
      // dispose of the byte stream
      stream.Close();
      
      // run callback
      OnComplete.Run();
    }
    
    //------------------------------//
    
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
        process.StandardInput.BaseStream.Write(_bytesGetMetadata, 0, _bytesGetMetadata.Length);
        byte[] pathBytes = System.Text.Encoding.ASCII.GetBytes(Path);
        process.StandardInput.BaseStream.Write(pathBytes, 0, pathBytes.Length);
        process.StandardInput.BaseStream.Write(_coordinator.BytesExecute, 0, _coordinator.BytesExecute.Length);
        process.StandardInput.BaseStream.Flush();
        _coordinator.Process.Release();
        
        // add the task to the coordinator in order to be processed
        _coordinator.Add(this);
        
      } else {
        
        ManagerUpdate.Control.AddSingle(WriteTemporaryFile, file, input);
      }
    }
    
  }
}
