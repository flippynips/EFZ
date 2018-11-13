/*
 * User: FloppyNipples
 * Date: 24/07/2017
 * Time: 21:26
 */
using Efz.Web.Display;

namespace Efz.Web.Display {
  
  /// <summary>
  /// Extension helper methods for common element properties.
  /// </summary>
  public static class ExtendElement {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Add a "text-align:center;" style attribute.
    /// </summary>
    public static void Center(this Element element) {
      element.Style[StyleKey.TextAlign] = "center";
    }
    
    //-------------------------------------------//
    
  }
  
}
