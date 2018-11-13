/*
 * User: Joshua
 * Date: 29/10/2016
 * Time: 1:03 PM
 */
using System;

namespace Efz {
  
  /// <summary>
  /// Extensions of the date time.
  /// </summary>
  public static class ExtendDateTime {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Get an index that represents the week of the specified date time.
    /// </summary>
    public static int GetWeekIndex(this DateTime dateTime) {
      // get the week index from the date time tick count
      return (int)(dateTime.Ticks / 6048000000000L);
    }
    
    //-------------------------------------------//
    
  }
  
}
