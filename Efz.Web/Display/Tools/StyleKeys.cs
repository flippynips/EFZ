/*
 * User: FloppyNipples
 * Date: 06/03/2017
 * Time: 21:06
 */
using System;
using System.Collections.Generic;

namespace Efz.Web.Display {
  
  /// <summary>
  /// Reference for common style keys.
  /// </summary>
  public static class StyleKeys {
    
    /// <summary>
    /// Maps of style keys to html representation.
    /// </summary>
    public readonly static Dictionary<StyleKey, string> Map;
    /// <summary>
    /// A hashset containing hashcodes of all string representations of the style keys.
    /// </summary>
    public readonly static HashSet<int> KeySet;
    /// <summary>
    /// A hashset containing style keys that are inherited by child elements.
    /// </summary>
    public readonly static HashSet<StyleKey> Inherited;
    
    static StyleKeys() {
      Map = BuildMap();
      Inherited = BuildInheritance();
      KeySet = BuildKeySet();
    }
    
    /// <summary>
    /// Get the StyleKey representation of the specified key.
    /// </summary>
    public static StyleKey GetKey(string key) {
      foreach(var entry in Map) if(entry.Value.Equals(key, StringComparison.Ordinal)) return entry.Key;
      return StyleKey.None;
    }
    
    /// <summary>
    /// Inner method used to build the set of key hashcodes.
    /// </summary>
    private static HashSet<int> BuildKeySet() {
      var keySet = new HashSet<int>();
      foreach(var entry in Map) keySet.Add(entry.Value.GetHashCode());
      return keySet;
    }
    
    /// <summary>
    /// Inner method used to build the set of style keys that should be inherited.
    /// </summary>
    private static HashSet<StyleKey> BuildInheritance() {
      var inherited = new HashSet<StyleKey>();
      #region inherited_keys
      inherited.Add(StyleKey.Azimuth);
      inherited.Add(StyleKey.BorderCollapse);
      inherited.Add(StyleKey.BorderSpacing);
      inherited.Add(StyleKey.CaptionSide);
      inherited.Add(StyleKey.Color);
      inherited.Add(StyleKey.Cursor);
      inherited.Add(StyleKey.Direction);
      inherited.Add(StyleKey.Elevation);
      inherited.Add(StyleKey.EmptyCells);
      inherited.Add(StyleKey.FontFamily);
      inherited.Add(StyleKey.FontSize);
      inherited.Add(StyleKey.FontStyle);
      inherited.Add(StyleKey.FontVariant);
      inherited.Add(StyleKey.FontWeight);
      inherited.Add(StyleKey.Font);
      inherited.Add(StyleKey.LetterSpacing);
      inherited.Add(StyleKey.LineHeight);
      inherited.Add(StyleKey.ListStyleImage);
      inherited.Add(StyleKey.ListStylePosition);
      inherited.Add(StyleKey.ListStyleType);
      inherited.Add(StyleKey.ListStyle);
      inherited.Add(StyleKey.Orphans);
      inherited.Add(StyleKey.PitchRange);
      inherited.Add(StyleKey.Pitch);
      inherited.Add(StyleKey.Quotes);
      inherited.Add(StyleKey.Richness);
      inherited.Add(StyleKey.SpeakHeader);
      inherited.Add(StyleKey.SpeakNumeral);
      inherited.Add(StyleKey.SpeakPunctuation);
      inherited.Add(StyleKey.Speak);
      inherited.Add(StyleKey.SpeechRate);
      inherited.Add(StyleKey.Stress);
      inherited.Add(StyleKey.TextAlign);
      inherited.Add(StyleKey.TextIndent);
      inherited.Add(StyleKey.TextTransform);
      inherited.Add(StyleKey.Visibility);
      inherited.Add(StyleKey.VoiceFamily);
      inherited.Add(StyleKey.Volume);
      inherited.Add(StyleKey.WhiteSpace);
      inherited.Add(StyleKey.Widows);
      inherited.Add(StyleKey.WordSpacing);
      inherited.Add(StyleKey.WordWrap);
      inherited.Add(StyleKey.WordBreak);
      #endregion
      return inherited;
    }
    
    /// <summary>
    /// Inner method used to build the map of style keys.
    /// </summary>
    private static Dictionary<StyleKey, string> BuildMap() {
      
      var map = new Dictionary<StyleKey, string>();
      var builder = StringBuilderCache.Get();
      bool first;
      foreach(var value in (StyleKey[])Enum.GetValues(typeof(StyleKey))) {
        first = true;
        foreach(var c in value.ToString()) {
          if(Char.IsUpper(c)) {
            if(first) first = false;
            else builder.Append(Chars.Dash);
            builder.Append(Char.ToLower(c));
          } else {
            builder.Append(c);
          }
        }
        
        map.Add(value, builder.ToString());
        builder.Length = 0;
      }
      
      StringBuilderCache.Set(builder);
      
      return map;
    }
    
  }
  
  /// <summary>
  /// Common style property keys.
  /// </summary>
  public enum StyleKey {
    
    None                 = 0,
    
    // background properties
    /// <summary>
    /// Declares the attachment of a background image (to scroll with the page content or be in a fixed position).
    /// (fixed | scroll)
    /// </summary>
    BackgroundAttachment = 1,
    /// <summary>
    /// Declares the background color.
    /// (Valid color names, RGB values, hexidecimal notation)
    /// </summary>
    BackgroundColor      = 2,
    /// <summary>
    /// Declares the background image of an element.
    /// url(URL)
    /// </summary>
    BackgroundImage      = 3,
    /// <summary>
    /// Declares the position of a background image.
    /// Lengths or percentages for the x and y positions, or one of the predefined values:
    /// (top left | top center | top right | center left | center center | center right | bottom left | bottom center | bottom right)
    /// </summary>
    BackgroundPosition   = 4,
    /// <summary>
    /// Declares how and/or if a background image repeats.
    /// (repeat | repeat-x | repeat-y | no-repeat)
    /// </summary>
    BackgroundRepeat     = 5,
    /// <summary>
    /// Used as a shorthand property to set all the background properties at once.
    /// Separate values by a space in the following order (those that are not defined will use inherited or default initial values):
    /// (background-color background-image background-repeat background-attachment background-position)
    /// </summary>
    Background           = 6,
    
    // border properties
    /// <summary>
    /// Declares the color of the top border.
    /// Valid color names, RGB values, hexidecimal notation, or the predefined value 'transparent'.
    /// </summary>
    BorderTopColor       = 10,
    /// <summary>
    /// Declares the style of the top border.
    /// (none | hidden | dotted | dashed | solid | double | groove | ridge | inset | outset)
    /// </summary>
    BorderTopStyle       = 11,
    /// <summary>
    /// Declares the width of the top border. Pixels or the following predefined values:
    /// (thin | medium | thick)
    /// </summary>
    BorderTopWidth       = 12,
    /// <summary>
    /// Used as a shorthand property to set all the border-top properties at once.
    /// Separate values by a space in the following order (those that are not defined will use inherited or default initial values):
    /// (border-top-width | border-top-style | border-top-color)
    /// </summary>
    BorderTop            = 13,
    /// <summary>
    /// Declares the color of the right border.
    /// Valid color names, RGB values, hexidecimal notation, or the predefined value 'transparent'.
    /// </summary>
    BorderRightColor     = 14,
    /// <summary>
    /// Declares the style of the right border.
    /// (none | hidden | dotted | dashed | solid | double | groove | ridge | inset | outset)
    /// </summary>
    BorderRightStyle     = 15,
    /// <summary>
    /// Declares the width of the right border. Pixels or the following predefined values:
    /// (thin | medium | thick)
    /// </summary>
    BorderRightWidth     = 16,
    /// <summary>
    /// Used as a shorthand property to set all the border-right properties at once.
    /// Separate values by a space in the following order (those that are not defined will use inherited or default initial values):
    /// (border-right-width | border-right-style | border-right-color)
    /// </summary>
    BorderRight          = 17,
    /// <summary>
    /// Declares the color of the bottom border.
    /// Valid color names, RGB values, hexidecimal notation, or the predefined value 'transparent'.
    /// </summary>
    BorderBottomColor    = 18,
    /// <summary>
    /// Declares the style of the bottom border.
    /// (none | hidden | dotted | dashed | solid | double | groove | ridge | inset | outset)
    /// </summary>
    BorderBottomStyle    = 19,
    /// <summary>
    /// Declares the width of the bottom border. Pixels or the following predefined values:
    /// (thin | medium | thick)
    /// </summary>
    BorderBottomWidth    = 20,
    /// <summary>
    /// Used as a shorthand property to set all the border-bottom properties at once.
    /// Separate values by a space in the following order (those that are not defined will use inherited or default initial values):
    /// (border-bottom-width | border-bottom-style | border-bottom-color)
    /// </summary>
    BorderBottom         = 21,
    /// <summary>
    /// Declares the color of the left border.
    /// Valid color names, RGB values, hexidecimal notation, or the predefined value 'transparent'.
    /// </summary>
    BorderLeftColor      = 22,
    /// <summary>
    /// Declares the style of the left border.
    /// (none | hidden | dotted | dashed | solid | double | groove | ridge | inset | outset)
    /// </summary>
    BorderLeftStyle      = 23,
    /// <summary>
    /// Declares the width of the left border. Pixels or the following predefined values:
    /// (thin | medium | thick)
    /// </summary>
    BorderLeftWidth      = 24,
    /// <summary>
    /// Used as a shorthand property to set all the border-left properties at once.
    /// Separate values by a space in the following order (those that are not defined will use inherited or default initial values):
    /// (border-left-width | border-left-style | border-left-color)
    /// </summary>
    BorderLeft           = 25,
    /// <summary>
    /// Declares the color of the border.
    /// Valid color names, RGB values, hexidecimal notation, or the predefined value 'transparent'.
    /// </summary>
    BorderColor          = 26,
    /// <summary>
    /// Declares the style of the border.
    /// (none | hidden | dotted | dashed | solid | double | groove | ridge | inset | outset)
    /// </summary>
    BorderStyle          = 27,
    /// <summary>
    /// Declares the width of the border. Pixels or the following predefined values:
    /// (thin | medium | thick)
    /// </summary>
    BorderWidth          = 28,
    /// <summary>
    /// Used as a shorthand property to set all the border properties at once.
    /// Separate values by a space in the following order (those that are not defined will use inherited or default initial values):
    /// (border-width | border-style | border-color)
    /// </summary>
    Border               = 29,
    /// <summary>
    /// Defines rounded corner size for content within the element.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// </summary>
    BorderRadius         = 201,
    
    // positioning and visibility
    /// <summary>
    /// Declares the side(s) of an element where no previous floating elements are allowed to be adjacent.
    /// (left | right | both | none)
    /// </summary>
    Clear                = 30,
    /// <summary>
    /// Declares the type of cursor to be displayed. URL values, and the following prefefined values:
    /// (auto | crosshair | default | pointer | move | e-resize | ne-resize | nw-resize | n-resize | se-resize | sw-resize | s-resize | w-resize | text | wait | help)
    /// </summary>
    Cursor               = 31,
    /// <summary>
    /// Declares if/how the element displays.
    /// (none | inline-block | list-item | run-in | compact | marker | table-inline-table | table-row-group | table-header-group | table-footer-group | table-row | table-column-group | table-column | table-cell | table-caption)
    /// </summary>
    Display              = 32,
    /// <summary>
    /// Declares whether a box should float to the left or right of other content, or whether it should not be floated at all.
    /// (left | right | none)
    /// </summary>
    Float                = 33,
    /// <summary>
    /// Declares the visibility of boxes generated by an element.
    /// (visible | hidden | collapse)
    /// </summary>
    Visibility           = 34,
    /// <summary>
    /// Declares the distance that the top content edge of the element is offset below the top edge of its containing block. The position property of the element must also be set to a value other than static.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// </summary>
    Top                  = 35,
    /// <summary>
    /// Declares the distance that the right content edge of the element is offset to the left of the right edge of its containing block. The position property of the element must also be set to a value other than static. 
    /// Lengths, percentages, and the predefined value 'auto'.
    /// </summary>
    Right                = 36,
    /// <summary>
    /// Declares the distance that the bottom content edge of the element is offset above the bottom edge of its containing block. The position property of the element must also be set to a value other than static.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// </summary>
    Bottom               = 37,
    /// <summary>
    /// Declares the distance that the left content edge of the element is offset to the right of the left edge of its containing block. The position property of the element must also be set to a value other than static.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// </summary>
    Left                 = 38,
    /// <summary>
    /// Declares the type of positioning of an element.
    /// (static | relative | absolute | fixed)
    /// </summary>
    Position             = 39,
    /// <summary>
    /// Declares the shape of a clipped region when the value of the overflow property is set to a value other than visible.
    /// (rect(top, right, bottom, left) | auto)
    /// </summary>
    Clip                 = 40,
    /// <summary>
    /// Declares how content that overflows the element's box is handled.
    /// (visible | hidden | scroll | auto)
    /// </summary>
    Overflow             = 41,
    /// <summary>
    /// Declares the vertical alignment of an inline-level element or a table cell.
    /// Lengths, percentages, and the following predefined values:
    /// (baseline | sub | super | top | text-top | middle | bottom | text-bottom)
    /// </summary>
    VerticalAlign        = 42,
    /// <summary>
    /// Declares the stack order of the element.
    /// Integer values and the predefined value 'auto'.
    /// </summary>
    ZIndex               = 43,
    /// <summary>
    /// Alpha opacity of the element.
    /// Numeric value between 0 and 1.
    /// </summary>
    Opacity              = 44,
    /// <summary>
    /// Change property values smoothly (from one value to another), over a given duration.
    /// ({property} {time in seconds}[, further properties])
    /// </summary>
    Transition           = 45,
    
    // dimension properties
    /// <summary>
    /// Declares the height of the element.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// </summary>
    Height               = 50,
    /// <summary>
    /// Declares the maximum height of the element.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// </summary>
    MaxHeight            = 51,
    /// <summary>
    /// Declares the minimum height of the element.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// </summary>
    MinHeight            = 52,
    /// <summary>
    /// Declares the width of the element.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// </summary>
    Width                = 53,
    /// <summary>
    /// Declares the maximum width of the element.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// </summary>
    MaxWidth             = 54,
    /// <summary>
    /// Declares the minimum width of the element.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// </summary>
    MinWidth             = 55,
    
    // font properties
    /// <summary>
    /// Declares the name of the font to be used.
    /// Valid font family names or generic family names,
    /// i.e. Arial, Verdana, sans-serif, "Times New Roman", Times, serif, etc.
    /// </summary>
    FontFamily           = 60,
    /// <summary>
    /// Declares the size of the font.
    /// Lengths (number and unit type— i.e. 1em, 12pt, 10px, 80%) or one of the following predefined values:
    /// (xx-small | x-small | small | medium | large | x-large | xx-large | smaller | larger)
    /// </summary>
    FontSize             = 61,
    /// <summary>
    /// Declares the aspect value (font size divided by x-height).
    /// Numeric value
    /// </summary>
    FontSizeAdjust       = 62,
    /// <summary>
    /// Declares the stretch of the font face.
    /// (normal | wider | narrower | ultra-condensed | extra-condensed | condensed | semi-condensed |
    /// semi-expanded | expanded | extra-expanded | ultra-expanded)
    /// </summary>
    FontStretch          = 63,
    /// <summary>
    /// Declares the font style.
    /// (normal | italic | oblique)
    /// </summary>
    FontStyle            = 64,
    /// <summary>
    /// Declares the font variant.
    /// (normal | small-caps)
    /// </summary>
    FontVariant          = 65,
    /// <summary>
    /// Declares the font weight (lightness or boldness)
    /// (normal | bold | bolder | lighter | 100 | 200 | 300 | 400 | 500 | 600 | 700 | 800 | 900)
    /// </summary>
    FontWeight           = 66,
    /// <summary>
    /// Used as a shorthand property to declare all of the font properties at
    /// once (except font-size-adjust and font-stretch).
    /// Separate values by a space in the following order (those that are not
    /// defined will use inherited or default initial values): 
    /// {font-style font-variant font-weight font-size line-height font-family}
    /// </summary>
    Font                 = 67,
    
    // content properties
    /// <summary>
    /// Generates content in the document in conjunction with the :before and :after pseudo-elements.
    /// String values, URL values, and predefined value formats:
    /// (counter(name) | counter(name, list-style-type) | counters(name, string) |
    /// counters(name, string, list-style-type) | attr(X) | open-quote | close-quote |
    /// no-open-quote | no-close-quote)
    /// </summary>
    Content              = 70,
    /// <summary>
    /// Declares the counter increment for each instance of a selector.
    /// Integers and the predefined value 'none'.
    /// </summary>
    CounterIncrement     = 71,
    /// <summary>
    /// Declares the value the counter is set to on each instance of a selector.
    /// Integers and the predefined value 'none'.
    /// </summary>
    CounterReset         = 72,
    /// <summary>
    /// Declares the type of quotation marks to use for quotations and embedded quotations.
    /// String values and the predefined value 'none'.
    /// </summary>
    Quotes               = 73,
    
    // list properties
    /// <summary>
    /// Declares the type of list marker used.
    /// (disc | circle | square | decimal | decimal-leading-zero | lower-roman | upper-roman |
    /// lower-alpha | upper-alpha | lower-greek | lower-latin | upper-latin | hebrew |
    /// georgian | cjk-ideographic | hiragana | katakana | hiragana-iroha | katakana-iroha)
    /// </summary>
    ListStyleType        = 80,
    /// <summary>
    /// Declares the position of the list marker.
    /// (inside | outside)
    /// </summary>
    ListStylePosition    = 81,
    /// <summary>
    /// Declares an image to be used as the list marker.
    /// URL values.
    /// </summary>
    ListStyleImage       = 82,
    /// <summary>
    /// Shorthand property to declare three list properties at once.
    /// Separate values by a space in the following order (those that are not defined
    /// will use inherited or default initial values):
    /// (list-style-type | list-style-position | list-style-image)
    /// </summary>
    ListStyle            = 83,
    /// <summary>
    /// Declares the marker offset for elements with a value of 'marker' set for the 'display'
    /// property.
    /// Lengths and the predefined value 'auto'.
    /// </summary>
    MarkerOffset         = 84,
    
    // margin properties
    /// <summary>
    /// Declares the top margin for the element.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// </summary>
    MarginTop            = 90,
    /// <summary>
    /// Declares the right margin for the element.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// </summary>
    MarginRight          = 91,
    /// <summary>
    /// Declares the bottom margin for the element.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// </summary>
    MarginBottom         = 92,
    /// <summary>
    /// Declares the left margin for the element.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// </summary>
    MarginLeft           = 93,
    /// <summary>
    /// Declares the all margin values for the element.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// {margin-top margin-right margin-bottom margin-left}
    /// 
    /// Undeclared values work as further shorthand notation. If only one length value
    /// is declared, all four margins will use that length. If two lengths are declared,
    /// the top and bottom margins will use the first length while the right and left
    /// margins will use the second length. If three lengths are declared, the top
    /// margin will use the first length, the right and left margins will use the second
    /// length, and the bottom margin will use the third length.
    /// </summary>
    Margin               = 94,
    
    // outline properties
    /// <summary>
    /// Declares the outline color.
    /// Valid color names, RGB values, hexidecimal notation.
    /// </summary>
    OutlineColor         = 100,
    /// <summary>
    /// Declares the style of the outline.
    /// (none | dotted | dashed | solid | double | groove | ridge | inset | outset)
    /// </summary>
    OutlineStyle         = 101,
    /// <summary>
    /// Declares the width of the outline.
    /// (thin | medium | thick)
    /// </summary>
    OutlineWidth         = 102,
    /// <summary>
    /// Used as a shorthand property to set all the outline properties at once.
    /// Separate values by a space in the following order (those that are not defined will
    /// use inherited or default initial values):
    /// {outline-color | outline-style | outline-width}
    /// </summary>
    Outline              = 103,
    
    // padding properties
    /// <summary>
    /// Declares the top padding for the element.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// </summary>
    PaddingTop           = 110,
    /// <summary>
    /// Declares the right padding for the element.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// </summary>
    PaddingRight         = 111,
    /// <summary>
    /// Declares the bottom padding for the element.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// </summary>
    PaddingBottom        = 112,
    /// <summary>
    /// Declares the left padding for the element.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// </summary>
    PaddingLeft          = 113,
    /// <summary>
    /// Declares all padding values for the element.
    /// Lengths, percentages, and the predefined value 'auto'.
    /// {padding-top padding-right padding-bottom padding-left}
    /// 
    /// Undeclared values work as further shorthand notation. If only one length value
    /// is declared, all four sides will use that length. If two lengths are declared,
    /// the top and bottom sides will use the first length while the right and left
    /// sides will use the second length. If three lengths are declared, the top
    /// side will use the first length, the right and left sides will use the second
    /// length, and the bottom side will use the third length.
    /// </summary>
    Padding              = 114,
    
    // page properties
    /// <summary>
    /// Declares the type of marks to display outside the page box.
    /// (crop | cross)
    /// </summary>
    Marks                = 120,
    /// <summary>
    /// Declares the minimum number of lines of a paragraph that must be left at the
    /// bottom of a page.
    /// Integer value.
    /// </summary>
    Orphans              = 121,
    /// <summary>
    /// Declares the type of page where an element should be displayed.
    /// Indentifiers.
    /// </summary>
    Page                 = 122,
    /// <summary>
    /// Declares a page break.
    /// (auto | always | avoid | left | right)
    /// </summary>
    PageBreakAfter       = 123,
    /// <summary>
    /// Declares a page break.
    /// (auto | always | avoid | left | right)
    /// </summary>
    PageBreakBefore      = 124,
    /// <summary>
    /// Declares a page break.
    /// (auto | avoid)
    /// </summary>
    PageBreakInside      = 125,
    /// <summary>
    /// Declares the size and orientation of a page box.
    /// Lengths, and the following predefined values:
    /// (auto | landscape | potrait)
    /// </summary>
    Size                 = 126,
    /// <summary>
    /// Declares the minimum number of lines of a paragraph that must be left at
    /// the top of a page.
    /// Integer values.
    /// </summary>
    Widows               = 127,
    
    // table properties
    /// <summary>
    /// Declares the way borders are displayed.
    /// (collapse | separate)
    /// </summary>
    BorderCollapse       = 130,
    /// <summary>
    /// Declares the distance separating borders (if border-collapse is separate).
    /// Lengths for the horizontal and vertical spacing, separated by a space. If one
    /// length is value is declared, that length is used for both the horizontal and
    /// vertical spacing. If two lengths are declared, the first one is used for
    /// horizontal spacing and the second one is used for vertical spacing. 
    /// </summary>
    BorderSpacing        = 131,
    /// <summary>
    /// Declares where the table caption is displayed in relation to the table.
    /// (top | bottom | left | right)
    /// </summary>
    CaptionSide          = 132,
    /// <summary>
    /// Declares the way empty cells are displayed (if border-collapse is separate).
    /// (show | hide)
    /// </summary>
    EmptyCells           = 133,
    /// <summary>
    /// Declares the type of table layout.
    /// (auto | fixed)
    /// </summary>
    TableLayout          = 134,
    
    // text properties
    /// <summary>
    /// Declares the color of the text and foreground elements.
    /// Valid color names, RGB values, hexidecimal notation.
    /// (aqua | black | blue | fuchsia | gray | green | lime | maroon | navy | olive |
    /// purple | red | silver | teal | white | yellow)
    /// </summary>
    Color                = 140,
    /// <summary>
    /// Declares the reading direction of the text.
    /// (ltr | rtl)
    /// </summary>
    Direction            = 141,
    /// <summary>
    /// Declares the distance between lines.
    /// Numbers, percentages, lengths, and the predefined value of normal.
    /// </summary>
    LineHeight           = 142,
    /// <summary>
    /// Declares the amount of space between text characters.
    /// A length (in addition to the default space) or the predefined value of normal.
    /// </summary>
    LetterSpacing        = 143,
    /// <summary>
    /// Declares the horizontal alignment of inline content.
    /// (left | right | center | justify)
    /// </summary>
    TextAlign            = 144,
    /// <summary>
    /// Declares the text decoration.
    /// (none | underline | overline | line-through | blink)
    /// </summary>
    TextDecoration       = 145,
    /// <summary>
    /// Declares the indentation of the first line of text.
    /// Lengths and percentages.
    /// </summary>
    TextIndent           = 146,
    /// <summary>
    /// Declares shadow effects on the text.
    /// A list containg a color followed by numeric values:
    /// {color horizontal-offset vertical-offset blur-radius}
    /// </summary>
    TextShadow           = 147,
    /// <summary>
    /// Declares the capitalization effects on the letters in the text.
    /// (none | capitalize | uppercase | lowercase)
    /// </summary>
    TextTransform        = 148,
    /// <summary>
    /// Declares values relating to bidirectional text. May be used in conjunction with
    /// the the direction property.
    /// (normal | embed | bidi-override)
    /// </summary>
    UnicodeBidi          = 149,
    /// <summary>
    /// Declares how white space is handled in an element.
    /// (normal | pre | nowrap)
    /// </summary>
    WhiteSpace           = 150,
    /// <summary>
    /// Declares the space between words in the text.
    /// A length (in addition to the default space) or the predefined value of 'normal'.
    /// </summary>
    WordSpacing          = 151,
    /// <summary>
    /// Determines how word overflow is handled for the element.
    /// (normal | break-word | initial | inherit)
    /// </summary>
    WordBreak            = 152,
    /// <summary>
    /// Determines how text overflow is handled for the element.
    /// (normal | break-all | keep-all | initial | inherit)
    /// </summary>
    WordWrap             = 153,
    /// <summary>
    /// Apply a filter to the element content.
    /// none | blur() | brightness() | contrast() | drop-shadow() | grayscale() | hue-rotate() | invert() | opacity() | saturate() | sepia() | url()
    /// </summary>
    Filter               = 154,
    
    // other
    /// <summary>
    /// Declares the angle that sound travels to the listener.
    /// Angle values in degrees (deg), or one of the following predefined values:
    /// (left-side | far-left | left | center-left | center | center-right | right |
    /// far-right | right-side | behind | leftwards | rightwards)
    /// </summary>
    Azimuth              = 180,
    /// <summary>
    /// Declares an audio cue to play after an element.
    /// URL values and the predefined value 'none'.
    /// </summary>
    CueAfter             = 181,
    /// <summary>
    /// Declares an audio cue to play before an element.
    /// URL values and the predefined value 'none'.
    /// </summary>
    CueBefore            = 182,
    /// <summary>
    /// Shorthand proerty to set both cue values at once.
    /// URL values and the predefined value none. Separate the values by a space in
    /// the following order:
    /// {cue-before cue-after}
    /// 
    /// If only one cue value is declared, it is used for both before and after.
    /// </summary>
    Cue                  = 183,
    /// <summary>
    /// Declares the elevation of a sound.
    /// Angle values in degrees (deg), or one of the following predefined values:
    /// (below | level | above | higher | lower)
    /// </summary>
    Elevation            = 184,
    /// <summary>
    /// Declares the amount of time to pause after an element.
    /// Time in milliseconds (ms) or percentages.
    /// </summary>
    PauseAfter           = 185,
    /// <summary>
    /// Declares the amount of time to pause before an element.
    /// Time in milliseconds (ms) or percentages.
    /// </summary>
    PauseBefore          = 186,
    /// <summary>
    /// Shorthand proerty to set both pause values at once.
    /// Separate the values by a space in the following order:
    /// {pause-before | pause-after}
    /// If only one pause value is declared, it is used for both before and after. 
    /// </summary>
    Pause                = 187,
    /// <summary>
    /// Declares the average speaking pitch of a voice.
    /// Frequencies in hertz (Hz) or the following predefined values:
    /// (x-low | low | medium | high | x-high)
    /// </summary>
    Pitch                = 188,
    /// <summary>
    /// Declares a change in the pitch range of a voice. 
    /// Number values between 0 and 100 (lower values indicate a flat voice while
    /// higher values indicate an animated voice). 
    /// </summary>
    PitchRange           = 189,
    /// <summary>
    /// Declares a background sound to be played while the current element is spoken.
    /// URL value, followed by one or more of the following keywords, separated by spaces:
    /// {mix | repeat}
    /// Alternatley, one of the following keywords:
    /// (auto | none)
    /// </summary>
    PlayDuring           = 190,
    /// <summary>
    /// Declares the richness of the voice in spoken text.
    /// Numeric values between 0 and 100 (lower values have less richness and higher
    /// values have more richness).
    /// </summary>
    Richness             = 191,
    /// <summary>
    /// Declares if/how text is spoken.
    /// (normal | none | spell-out)
    /// </summary>
    Speak                = 192,
    /// <summary>
    /// Declares how often table header cells are spoken.
    /// (once | always)
    /// </summary>
    SpeakHeader          = 193,
    /// <summary>
    /// Declares how numerals are spoken.
    /// (digits | continuous)
    /// </summary>
    SpeakNumeral         = 194,
    /// <summary>
    /// Declares how punctuation is spoken.
    /// (code | none)
    /// </summary>
    SpeakPunctuation     = 195,
    /// <summary>
    /// Declares the speech rate of spoken text.
    /// A number indicating the number of words per minute, or one of the following
    /// predefined values:
    /// (x-slow | slow | medium | fast | x-fast | faster | slower)
    /// </summary>
    SpeechRate           = 196,
    /// <summary>
    /// Declares the stress of the voice on spoken text.
    /// Numeric values between 0 and 100 (lower values have less stress and higher
    /// values have more stress).
    /// </summary>
    Stress               = 197,
    /// <summary>
    /// Declares the voice family of spoken text.
    /// Generic or specific voice family names.
    /// </summary>
    VoiceFamily          = 198,
    /// <summary>
    /// Declares the median volume.
    /// Numbers between 0 and 100, percentages, or one of the following predefined values:
    /// (silent | x-soft | soft | medium | loud | x-loud)
    /// </summary>
    Volume               = 199
    
  }
}
