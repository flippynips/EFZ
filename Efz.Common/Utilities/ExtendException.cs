/*
 * User: Joshua
 * Date: 15/10/2016
 * Time: 7:01 PM
 */
using System;
using System.Text;

namespace Efz {
  
  /// <summary>
  /// Extension methods for Exception classes.
  /// </summary>
  public static class ExtendException {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Returns the exception messages including any sub-messages.
    /// </summary>
    public static string Messages(this Exception ex) {
      StringBuilder sb = StringBuilderCache.Get();
      while(ex != null) {
        sb.Append(ex.Message);
        ex = ex.InnerException;
      }
      return StringBuilderCache.SetAndGet(sb);
    }
    
    //-------------------------------------------//
    
  }
  
}
