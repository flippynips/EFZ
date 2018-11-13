/*
 * User: FloppyNipples
 * Date: 13/03/2017
 * Time: 19:24
 */
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Efz.Web.Display {
  
  /// <summary>
  /// Static helper functions for Elements.
  /// </summary>
  public partial class Element {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Regex expression for validating a month input type.
    /// </summary>
    public static Regex RegexMonth = new Regex("[0-9]{4}-[0-9]{2}|[0-9]{2}-[0-9]{4}",
      RegexOptions.Compiled, TimeSpan.FromMilliseconds(2));
    
    /// <summary>
    /// Regex expression for validating a password.
    /// </summary>
    public static Regex RegexPassword = new Regex("(?=.*[!@#$%^&*()0-9a-zA-Z]).{4,}",
      RegexOptions.Compiled, TimeSpan.FromMilliseconds(2));
    
    /// <summary>
    /// Regex expression for validating 24 hour time as per an input element e.g. 13-23.
    /// </summary>
    public static Regex RegexTime = new Regex("[0-2][0-9]-[0-5][0-9]",
      RegexOptions.Compiled, TimeSpan.FromMilliseconds(2));
    
    /// <summary>
    /// Regex expression for validating a password.
    /// </summary>
    public static Regex RegexUrl = new Regex(".+\\.\\D{2,}",
      RegexOptions.Compiled, TimeSpan.FromMilliseconds(2));
    
    /// <summary>
    /// Regex expression for validating a password.
    /// </summary>
    public static Regex RegexWeek = new Regex("\\d{4}-W\\d{2}",
      RegexOptions.Compiled, TimeSpan.FromMilliseconds(2));
    
    /// <summary>
    /// Create a link element with the specified text to the specified url.
    /// </summary>
    public static Element CreateLink(string text, string url) {
      var link = new Element {
        Tag = Tag.Link,
        ContentString = text
      };
      link.SetAttribute("href", url);
      return link;
    }
    
    /// <summary>
    /// Create a link element with the specified text to the specified url.
    /// </summary>
    public static Element CreateImage(string source, string link, int maxWidth, int maxHeight) {
      var linkElement = new Element(Tag.Link);
      linkElement.SetAttribute("href", link);
      var image = new Element(Tag.Image);
      image.SetAttribute("src", source);
      image.Style[StyleKey.MaxWidth] = maxWidth + "px";
      image.Style[StyleKey.MaxHeight] = maxHeight + "px";
      linkElement.AddChild(image);
      return linkElement;
    }
    
    
    /// <summary>
    /// Create a link element with the specified text to the specified url.
    /// </summary>
    public static Element CreateImage(string source, int maxWidth, int maxHeight) {
      var image = new Element(Tag.Image);
      image.SetAttribute("src", source);
      image.Style[StyleKey.MaxWidth] = maxWidth + "px";
      image.Style[StyleKey.MaxHeight] = maxHeight + "px";
      return image;
    }
    
    /// <summary>
    /// Create a link element as the child of an element of the specified type.
    /// </summary>
    public static Element CreateLink(Tag parent, string text, string url) {
      var link = new Element {
        Tag = Tag.Link,
        ContentString = text
      };
      link.SetAttribute("href", url);
      var parentElement = new Element {
        Tag = parent
      };
      parentElement.AddChild(link);
      return parentElement;
    }
    
    /// <summary>
    /// Create a paragraph element with the specified text content.
    /// </summary>
    public static Element CreateText(string text) {
      return new Element {
        Tag = Tag.Paragraph,
        ContentString = text
      };
    }
    
    /// <summary>
    /// Create an element with the specified tag and text content.
    /// </summary>
    public static Element CreateText(Tag tag, string text) {
      return new Element {
        Tag = tag,
        ContentString = text
      };
    }
    
    /// <summary>
    /// Create a paragraph element with the specified text content.
    /// </summary>
    public static Element CreateText(string text, WebColor color) {
      var textElement = new Element {
        Tag = Tag.Paragraph,
        ContentString = text
      };
      textElement.Style[StyleKey.Color] = color;
      return textElement;
    }
    
    /// <summary>
    /// Create an error element with the specified text.
    /// </summary>
    public static Element CreateError(string text) {
      var errorElement = new Element {
        Tag = Tag.Paragraph,
        ContentString = text
      };
      errorElement.Style[StyleKey.Color] = WebColor.Orange.Shade(1.4f).ToString();
      return errorElement;
    }
    
    /// <summary>
    /// Create an element that contains a list of errors.
    /// </summary>
    public static Element CreateError(IEnumerable<string> text) {
      var errorList = new Element();
      errorList.Tag = Tag.ListItem;
      errorList.Style[StyleKey.Color] = WebColor.Orange.Shade(1.4f).ToString();
      foreach(var error in text) {
        var item = new Element {
          Tag = Tag.List,
          ContentString = error
        };
        errorList.AddChild(item);
      }
      return errorList;
    }
    
    /// <summary>
    /// Create a file browse element.
    /// </summary>
    public static Element CreateBrowse(string label, string name, string accept = null, bool required = false) {
      
      Element container = new Element(Tag.Division);
      
      // add the browse button
      Element buttonElement = new Element(Tag.Input);
      buttonElement.Style[StyleKey.MaxWidth] = "73px";
      buttonElement.Style[StyleKey.MarginRight] = "1px";
      buttonElement.Style[StyleKey.MarginTop] = "2px";
      buttonElement["name"] = name;
      buttonElement["type"] = "file";
      if(accept != null) buttonElement["accept"] = accept;
      if(required) buttonElement["required"] = null;
      
      container.AddChild(buttonElement);
      
      // add the label
      Element labelElement = new Element(Tag.Paragraph);
      labelElement.Style[StyleKey.Display] = "inline";
      labelElement.Style[StyleKey.Margin] = "1px auto";
      labelElement.Style[StyleKey.MinWidth] = "65px";
      labelElement.Style[StyleKey.FontSize] = "14px";
      labelElement.ContentString = label;
      
      container.AddChild(labelElement);
      
      return container;
    }
    
    //-------------------------------------------//
    
  }
}
