/*
 * User: FloppyNipples
 * Date: 24/01/2017
 * Time: 22:03
 */
using System;
using System.Text;
using Efz.Collections;

namespace Efz {
  
  /// <summary>
  /// File system helper methods.
  /// </summary>
  public static class Fs {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Path segment that indicates a parent folder to the indicated path.
    /// </summary>
    public const string ParentSegment = "/../";
    /// <summary>
    /// Characters that are invalid
    /// </summary>
    public readonly static char[] InvalidFileNameChars = System.IO.Path.GetInvalidFileNameChars();
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Combine the specified path segments into a single path.
    /// </summary>
    public static string Combine(ArrayRig<string> segments) {
      
      // was the segments collection assigned? return empty string
      if(segments == null || segments.Count == 0) return string.Empty;
      // is there only one segment? yes, return the single segment
      if(segments.Count == 1) return segments[0].Swap(Chars.BackSlash, Chars.ForwardSlash);
      
      // get a string builder
      StringBuilder builder = StringBuilderCache.Get();
      
      // get the first segment
      string segment = segments[0];
      bool skipSeparator = segment != null && segment.Length > 2 && segment[1] == Chars.Colon &&
        (segment[2] == Chars.ForwardSlash || segment[2] == Chars.BackSlash);
      
      // iterate the path segments
      for (int i = 0; i < segments.Count; ++i) {
        segment = segments[i];
        
        // is the segment empty? skip if so
        if(string.IsNullOrEmpty(segment)) continue;
        
        // replace any backslashes with forward slashes
        segment = segment.Swap(Chars.BackSlash, Chars.ForwardSlash);
        
        if(segment.Length == 1 && segment[0] == Chars.ForwardSlash) {
          continue;
        }
        
        // should the addition of a separator be skipped this iteration?
        if(skipSeparator) {
          
          // yes, does the segment include a separator?
          if(segment[0] == Chars.ForwardSlash) {
            // yes, append the segment without the separator
            builder.Append(segment, 1, segment.Length-1);
          } else {
            builder.Append(segment);
          }
          
        } else {
          
          // no, does the segment include a separator?
          if(segment[0] == Chars.ForwardSlash) {
            // yes, append the segment
            builder.Append(segment);
          } else {
            // no, append a separator then the segment
            builder.Append(Chars.ForwardSlash);
            builder.Append(segment);
          }
          
        }
        
        skipSeparator = segment[segment.Length-1] == Chars.ForwardSlash;
        
      }
      
      string path;
      
      // did the last segment end in a separator?
      if(skipSeparator) {
        // yes, remove the last character and return
        path = StringBuilderCache.SetAndGet(builder, 0, builder.Length-1);
      } else {
        // no, return the complete string
        path = StringBuilderCache.SetAndGet(builder);
      }
      
      // get an index of a parent separator
      int parentSeparator = path.IndexOf(ParentSegment, StringComparison.Ordinal);
      // while there are parent separators
      while (parentSeparator > 0) {
        int lastSeparator = path.LastIndexOf(Chars.ForwardSlash, parentSeparator - 1);
        if(lastSeparator > -1) {
          var sections = path.Split(lastSeparator, 1, parentSeparator - lastSeparator + 3);
          path = sections.ArgA + sections.ArgB;
          parentSeparator = lastSeparator - 1;
        }
        parentSeparator = path.IndexOf(ParentSegment, parentSeparator + 1, StringComparison.Ordinal);
      }
      
      return path;
      
    }
    
    /// <summary>
    /// Combine the specified path segments into a single path.
    /// </summary>
    public static string Combine(params string[] segments) {
      
      // was the segments collection assigned? return empty string
      if(segments == null || segments.Length == 0) return string.Empty;
      // is there only one segment? yes, return the single segment
      if(segments.Length == 1) return segments[0].Swap(Chars.BackSlash, Chars.ForwardSlash);
      
      // get a string builder
      StringBuilder builder = StringBuilderCache.Get();
      
      // get the first segment
      string segment = segments[0];
      bool skipSeparator = segment != null && segment.Length > 2 && segment[1] == Chars.Colon &&
        (segment[2] == Chars.ForwardSlash || segment[2] == Chars.BackSlash);
      
      // iterate the path segments
      for (int i = 0; i < segments.Length; ++i) {
        segment = segments[i];
        
        // is the segment empty? skip if so
        if(string.IsNullOrEmpty(segment)) continue;
        
        // replace any backslashes with forward slashes
        segment = segment.Swap(Chars.BackSlash, Chars.ForwardSlash);
        
        if(segment.Length == 1 && segment[0] == Chars.ForwardSlash) {
          continue;
        }
        
        // should the addition of a separator be skipped this iteration?
        if(skipSeparator) {
          
          // yes, does the segment include a separator?
          if(segment[0] == Chars.ForwardSlash) {
            // yes, append the segment without the separator
            builder.Append(segment, 1, segment.Length-1);
          } else {
            builder.Append(segment);
          }
          
        } else {
          
          // no, does the segment include a separator?
          if(segment[0] == Chars.ForwardSlash) {
            // yes, append the segment
            builder.Append(segment);
          } else {
            // no, append a separator then the segment
            builder.Append(Chars.ForwardSlash);
            builder.Append(segment);
          }
          
        }
        
        skipSeparator = segment[segment.Length-1] == Chars.ForwardSlash;
        
      }
      
      string path;
      
      // did the last segment end in a separator?
      if(skipSeparator) {
        // yes, remove the last character and return
        path = StringBuilderCache.SetAndGet(builder, 0, builder.Length-1);
      } else {
        // no, return the complete string
        path = StringBuilderCache.SetAndGet(builder);
      }
      
      // get an index of a parent separator
      int parentSeparator = path.IndexOf(ParentSegment, StringComparison.Ordinal);
      // while there are parent separators
      while (parentSeparator > 0) {
        int lastSeparator = path.LastIndexOf(Chars.ForwardSlash, parentSeparator - 1);
        if(lastSeparator > -1) {
          var sections = path.Split(lastSeparator, 1, parentSeparator - lastSeparator + 3);
          path = sections.ArgA + sections.ArgB;
          parentSeparator = lastSeparator - 1;
        }
        parentSeparator = path.IndexOf(ParentSegment, parentSeparator + 1, StringComparison.Ordinal);
      }
      
      return path;
      
    }
    
    /// <summary>
    /// Get the file extension of the specified path if it exists.
    /// </summary>
    public static string GetExtension(string path) {
      
      // get a possible index of the extension
      int extensionIndex = path.LastIndexOf(Chars.Stop);
      
      // get the last path separator
      int separatorIndex = path.LastIndexOf(Chars.ForwardSlash);
      
      // is the last extension index after the last path separator
      if(separatorIndex >= extensionIndex) {
        // no, return empty
        return string.Empty;
      }
      
      // return the extension
      return path.Substring(extensionIndex + 1, path.Length - extensionIndex - 1);
      
    }
    
    /// <summary>
    /// Get a file name from the specified path. The file name will
    /// not include an extension.
    /// </summary>
    public static string GetFileName(string path) {
      int index = path.LastIndexOf(Chars.ForwardSlash);
      if(index >= 0) path = path.Substring(index + 1);
      index = path.LastIndexOf(Chars.BackSlash);
      if(index >= 0) path = path.Substring(index + 1);
      index = path.LastIndexOf(Chars.Stop);
      if(index == -1) return path;
      return path.Substring(0, index);
    }
    
    /// <summary>
    /// Get a file name and extension from the specified path.
    /// </summary>
    public static string GetFileNameAndExtension(string path) {
      int index = path.LastIndexOf(Chars.ForwardSlash);
      if(index >= 0) path = path.Substring(index + 1);
      index = path.LastIndexOf(Chars.BackSlash);
      if(index >= 0) return path.Substring(index + 1);
      return path;
    }
    
    /// <summary>
    /// Get the name of the last folder name of the specified path.
    /// </summary>
    public static string GetFolderName(string path) {
      int index = path.LastIndexOf(Chars.ForwardSlash);
      if(index == -1) index = path.LastIndexOf(Chars.BackSlash);
      return index == -1 ? path : GetFileNameAndExtension(path.Substring(0, index));
    }
    
    /// <summary>
    /// Get the directory of a file specified by path. If no directory
    /// separator is found, the path is returned unchanged.
    /// </summary>
    public static string GetDirectory(string path) {
      int index = path.LastIndexOf(Chars.ForwardSlash);
      if(index == -1) index = path.LastIndexOf(Chars.BackSlash);
      if(index == -1) return path;
      return path.Substring(0, index);
    }
    
    /// <summary>
    /// Get the full parent directory of the specified directory.
    /// </summary>
    public static string GetParentDirectory(string directory) {
      int index = directory.LastIndexOf(Chars.ForwardSlash, directory.Length-1);
      if(index == -1) {
        index = directory.LastIndexOf(Chars.BackSlash, directory.Length-1);
        if(index == -1) return directory;
        int nextIndex = directory.LastIndexOf(Chars.BackSlash, index-1);
        return nextIndex == -1 ?
          directory.Substring(0, index) :
          directory.Substring(0, nextIndex);
      } else {
        int nextIndex = directory.LastIndexOf(Chars.BackSlash, index-1);
        return nextIndex == -1 ?
          directory.Substring(0, index) :
          directory.Substring(0, nextIndex);
      }
    }
    
    /// <summary>
    /// Ensure validity of the specified file name while 
    /// </summary>
    public static bool ParseFileName(ref string fileName) {
      if(fileName.Contains(InvalidFileNameChars) || fileName.Length > 256) return false;
      int index = fileName.LastIndexOf(Chars.Stop);
      if(index >= 0) fileName = fileName.Substring(0, index);
      return true;
    }
    
    /// <summary>
    /// Does the specified path indicate a web resource?
    /// </summary>
    public static bool IsWebPath(string path) {
      return path.StartsWith(Protocols.Http, StringComparison.OrdinalIgnoreCase) || path.StartsWith(Protocols.Https, StringComparison.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// Split a path into its components.
    /// </summary>
    public static ArrayRig<string> Split(string path) {
      
      ArrayRig<string> split;
      
      if(path.Contains(Chars.ForwardSlash)) {
        
        split = new ArrayRig<string>(path.Split(Chars.ForwardSlash));
        for(int i = split.Count-1; i >= 0; --i) {
          if(string.IsNullOrEmpty(split[i])) {
            split.Remove(i);
            if(i > 0) {
              string part = split[i-1];
              if(part.Length > 1 && part[part.Length-1] == Chars.Colon) {
                split[i-1] = part + Chars.ForwardSlash;
              }
            }
          }
        }
        
      } else if(path.Contains(Chars.BackSlash)) {
        
        split = new ArrayRig<string>(path.Split(Chars.BackSlash));
        for(int i = split.Count-1; i >= 0; --i) {
          if(string.IsNullOrEmpty(split[i])) {
            split.Remove(i);
            if(i > 0) {
              string part = split[i-1];
              if(part.Length > 1 && part[part.Length-1] == Chars.Colon) {
                split[i-1] = part + Chars.BackSlash;
              }
            }
          }
        }
        
      } else {
        
        split = new ArrayRig<string>();
        split.Add(path);
        
      }
      
      return split;
      
    }
    
    /// <summary>
    /// Attempts to get whether the path represents a full file path on the current platform.
    /// This will also return true in the case of a web path.
    /// </summary>
    public static bool IsFullPath(string path) {
      
      if(Efz.Tools.SystemInformation.Platform == Efz.Tools.Platform.Windows) {
        return path[1] == ':' && path[2] == '\\' ||
          path.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
          path.StartsWith("http://", StringComparison.OrdinalIgnoreCase);
      }
      
      return path[0] == '/' ||
        path.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
        path.StartsWith("http://", StringComparison.OrdinalIgnoreCase);
      
    }
    
    //-------------------------------------------//
    
  }
  
}
