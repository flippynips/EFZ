/*
 * User: FloppyNipples
 * Date: 18/03/2017
 * Time: 21:18
 */
using System;

namespace Efz.Web.Display {
  
  /// <summary>
  /// Extension methods for styles.
  /// </summary>
  public static class ExtendStyle {
    
    /// <summary>
    /// Shorthand for adding complete justification for text content.
    /// </summary>
    public static void Justify(this Style style) {
      style[StyleKey.TextAlign] = "justify";
      
      Style justify = new Style();
      justify.Class = ":after";
      justify[StyleKey.Display] = "inline-block";
      justify[StyleKey.Content] = "''";
      justify[StyleKey.Width] = "100%";
      style.Add(justify);
    }
    
    /// <summary>
    /// Add style components in order to disable text selection.
    /// </summary>
    public static void DisableSelection(this Style style) {
      style[StyleKey.Cursor] = "default";
      style["user-select"] = "none";
      style["-o-user-select"] = "none";
      style["-ms-user-select"] = "none";
      style["-moz-user-select"] = "none";
      style["-webkit-user-select"] = "none";
    }
    
    /// <summary>
    /// Add a style component that will cause the element to fade in over the specified time.
    /// </summary>
    public static void FadeIn(this Style style) {
      Style fadeIn = new Style();
      fadeIn.Class = ":before";
      fadeIn[StyleKey.Visibility] = "visible";
      fadeIn[StyleKey.Opacity] = "1";
      fadeIn[StyleKey.Transition] = "opacity 2s linear";
      style.Add(fadeIn);
    }
    
    /// <summary>
    /// Add a style component that will cause the element to fade out over the specified time.
    /// </summary>
    public static void FadeOut(this Style style) {
      Style fadeOut = new Style();
      fadeOut.Class = ":after";
      fadeOut[StyleKey.Visibility] = "hidden";
      fadeOut[StyleKey.Opacity] = "0";
      fadeOut[StyleKey.Transition] = "visibility 0s 2s, opacity 2s linear";
      style.Add(fadeOut);
    }
    
  }
}
