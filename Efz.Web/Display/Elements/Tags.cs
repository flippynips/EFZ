/*
 * User: Bob
 * Date: 13/11/2016
 * Time: 11:10
 */
/*
 * User: Joshua
 * Date: 6/08/2016
 * Time: 6:11 PM
 */
using System;
using System.Collections.Generic;

namespace Efz.Web.Display {
  
  public static class Tags {
    
    /// <summary>
    /// Map of enum representations to html representations.
    /// </summary>
    public static readonly Dictionary<Tag, string> Html;
    
    /// <summary>
    /// Collection of tags that do not have content and therefore don't require forward slash
    /// tag suffixes.
    /// </summary>
    public static readonly HashSet<Tag> Standalone;
    
    /// <summary>
    /// Static initialization.
    /// </summary>
    static Tags() {
      Html = new Dictionary<Tag, string> {
        // handling
        { Tag.Custom, string.Empty },
        { Tag.Html, "html" },
        { Tag.Head, "head" },
        { Tag.Title, "title" },
        { Tag.Body, "body" },
        { Tag.Paragraph, "p" },
        { Tag.HorizontalLine, "hr" },
        { Tag.Preserve, "pre" },
        { Tag.Division, "div" },
        { Tag.Link, "a" },
        { Tag.Meta, "meta" },
        { Tag.ListItem, "li" },
        { Tag.List, "lu" },
        { Tag.Table, "table" },
        { Tag.TableRow, "tr" },
        { Tag.TableHeadCell, "th" },
        { Tag.TableCell, "td" },
        { Tag.TableHead, "thead" },
        { Tag.TableBody, "tbody" },
        { Tag.TableFoot, "tfoot" },
        { Tag.Span, "span" },
        { Tag.Break, "br" },
        { Tag.Form, "form" },
        { Tag.Input, "input" },
        { Tag.TextArea, "textarea" },
        { Tag.Column, "col" },
        { Tag.ColumnGroup, "colgroup" },
        { Tag.Select, "select" },
        { Tag.Option, "option" },
        
        // media
        { Tag.Image, "img" },
        { Tag.Audio, "audio" },
        { Tag.Video, "video" },
        
        // size
        { Tag.Heading1, "h1" },
        { Tag.Heading2, "h2" },
        { Tag.Heading3, "h3" },
        { Tag.Heading4, "h4" },
        
        // meta
        { Tag.NoScript, "noscript" },
        { Tag.Script, "script" },
        { Tag.LinkResource, "link" },
        
        // style
        { Tag.Italic, "i" },
        { Tag.Bold, "b" },
        { Tag.Style, "style"},
        
        // positioning
        { Tag.Center, "center" },
        
        // placeholder
        { Tag.Prefab, "prefab" }
        
      };
      
      Standalone = new HashSet<Tag>();
      
      Standalone.Add(Tag.None);
      Standalone.Add(Tag.Body);
      Standalone.Add(Tag.Head);
      Standalone.Add(Tag.Html);
      Standalone.Add(Tag.ListItem);
      Standalone.Add(Tag.Paragraph);
      Standalone.Add(Tag.Link);
      Standalone.Add(Tag.Break);
      Standalone.Add(Tag.HorizontalLine);
      Standalone.Add(Tag.Meta);
      Standalone.Add(Tag.Input);
      Standalone.Add(Tag.Image);
      Standalone.Add(Tag.LinkResource);
      
      Standalone.Add(Tag.Column);
      Standalone.Add(Tag.ColumnGroup);
      Standalone.Add(Tag.TableHeadCell);
      Standalone.Add(Tag.TableCell);
      Standalone.Add(Tag.TableRow);
      Standalone.Add(Tag.TableHead);
      Standalone.Add(Tag.TableBody);
      Standalone.Add(Tag.TableFoot);
    }
    
  }
  
  /// <summary>
  /// Possible html tag types.
  /// </summary>
  public enum Tag : byte {
    
    // handling
    None           = 0, // no tag will be added
    Custom         = 1, // will by default get the tag from the attribute 'tag'
    Html           = 2,
    Head           = 3,
    Title          = 4,
    Body           = 5,
    Paragraph      = 6,
    HorizontalLine = 7,
    Preserve       = 8,
    Division       = 9,
    Link           = 10,
    Meta           = 11,
    ListItem       = 12,
    List           = 13,
    Table          = 14,
    TableRow       = 15,
    TableHeadCell  = 16,
    TableCell      = 17,
    TableHead      = 18,
    TableBody      = 19,
    TableFoot      = 20,
    Span           = 21,
    Break          = 22,
    Form           = 23,
    Input          = 24,
    TextArea       = 25,
    Column         = 26,
    ColumnGroup    = 27,
    Select         = 28,
    Option         = 29,
    
    // media
    Image          = 80,
    Video          = 81,
    Audio          = 82,
    
    // size
    Heading1       = 110,
    Heading2       = 111,
    Heading3       = 112,
    Heading4       = 113,
    
    // meta
    NoScript       = 150,
    Script         = 151,
    LinkResource   = 152,
    
    // style
    Italic         = 200,
    Bold           = 201,
    Style          = 202,
    
    // positioning
    Center         = 250,
    
    // placeholder for a replacement element
    Prefab         = 255
    
  }
  
}
