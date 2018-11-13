/*
 * User: Joshua
 * Date: 6/08/2016
 * Time: 6:11 PM
 */
using System;
using System.Text.RegularExpressions;

namespace Efz.Web {
  
  /// <summary>
  /// Common helper methods related to Web access.
  /// </summary>
  public static class Common {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Does the extension likely represent a web page?
    /// </summary>
    public static bool IsWebPage(string extension) {
      switch(extension.ToUpper()) {
        case "":
        case "HTM":
        case "HTML":
        case "JHTML":
        case "XHTML":
        case "CSS":
        case "ASP":
        case "ASPX":
        case "AXD":
        case "ASMX":
        case "ASHX":
        case "CFM":
        case "SHTML":
        case "PHP":
        case "PHP4":
        case "PHP3":
        case "PHTML":
        case "PY":
        case "RHTML":
        case "RB":
        case "XML":
        case "RSS":
        case "SVG":
        case "CGI":
        case "DLL":
        case "PL":
        case "JS":
        case "JSP":
        case "JSPX":
        case "WSS":
        case "DO":
        case "ACTION":
          return true;
      }
      return false;
    }
    
    //-------------------------------------------//
    
  }
  
}
