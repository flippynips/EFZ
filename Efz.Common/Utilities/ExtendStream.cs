/*
 * User: Joshua
 * Date: 29/10/2016
 * Time: 1:03 PM
 */
using System;
using System.IO;

namespace Efz {
  
  /// <summary>
  /// Extensions of stream classes.
  /// </summary>
  public static class ExtendStream {
    
    /// <summary>
    /// Copy the content of a stream to another over a series of async tasks.
    /// </summary>
    public static void BufferedCopy(this Stream inStream, Stream outStream, IAction onComplete = null) {
      
      var buffer = BufferCache.Get();
      int count = inStream.Read(buffer, 0, Global.BufferSizeLocal);
      
      if(count == 0) {
        outStream.Flush();
        BufferCache.Set(buffer);
        if(onComplete != null) onComplete.Run();
      } else {
        outStream.Write(buffer, 0, count);
        BufferCache.Set(buffer);
        ManagerUpdate.Control.AddSingle(BufferedCopy, inStream, outStream, onComplete);
      }
      
    }
    
    
  }
}
