/*
 * User: FloppyNipples
 * Date: 28/04/2017
 * Time: 21:35
 */
using System;

using Efz.Tools;

namespace Efz.Web.Display {
  
  /// <summary>
  /// Evaluation, validation and markup of post content.
  /// </summary>
  public class MarkdownParser<T> {
    
    //-------------------------------------------//
    
    public delegate bool OnParseDelegate(ParseInfo info, Element element);
    
    public class Marker {
      public string Prefix;
      public OnParseDelegate OnParse;
    }
    
    public class MarkerInfo {
      public Marker Marker;
      public int StartIndex;
      public int EndIndex;
      public Element Element;
    }
    
    public class ParseInfo {
      public string Source;
      public int Length;
      public int Index;
      public T Metadata;
      public TreeSearch<char, Marker>.DynamicSearch Search;
    }
    
    /// <summary>
    /// Tree search of markers.
    /// </summary>
    internal TreeSearch<char, Marker> Markers;
    /// <summary>
    /// Suffix to end all markers.
    /// </summary>
    internal string Suffix;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Parse the specified string.
    /// </summary>
    public MarkdownParser(string suffix) {
      Suffix = suffix;
      Markers = new TreeSearch<char, Marker>();
      Markers.Add(null, Suffix);
    }
    
    /// <summary>
    /// Add the specified prefix and parse delegate.
    /// </summary>
    public void Add(string prefix, OnParseDelegate onParse) {
      Marker marker = new Marker();
      marker.Prefix = prefix;
      marker.OnParse = onParse;
      Markers.Add(marker, marker.Prefix);
    }
    
    /// <summary>
    /// Add the specified marker to the tree search.
    /// </summary>
    public void Add(Marker marker) {
      Markers.Add(marker, marker.Prefix);
    }
    
    /// <summary>
    /// Parse the specified source and run the specified callback on complete.
    /// </summary>
    public Element Parse(string source, T metadata) {
      
      var info = new ParseInfo();
      info.Source = source;
      info.Length = source.Length;
      info.Metadata = metadata;
      info.Search = Markers.SearchDynamic();
      
      return Parse(info);
      
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Parse the content of a marker and any contained child markers.
    /// </summary>
    protected bool Parse(ParseInfo info, MarkerInfo marker) {
      
      // parse the marker
      if(marker.Element == null) marker.Element = new Element();
      if(!marker.Marker.OnParse(info, marker.Element)) return false;
      
      // set the end index to the current index
      marker.EndIndex = info.Index;
      
      // while there are more characters to parse
      while(info.Index < info.Length) {
        
        // move to the next character is there a marker?
        if(info.Search.Next(info.Source[info.Index])) {
          
          var mark = info.Search.Values[0];
          
          // is the marker a closer?
          if(mark == null) {
            
            // yes, append the remaining content to the element
            if(info.Index - Suffix.Length > marker.EndIndex) {
              marker.Element.Content.Add(info.Source.Substring(marker.EndIndex, info.Index - Suffix.Length - marker.EndIndex + 1));
            }
            marker.EndIndex = info.Index + 1;
            
            // return success
            return true;
            
          } else {
            
            // no, create a child marker
            MarkerInfo child = new MarkerInfo();
            child.StartIndex = info.Index - mark.Prefix.Length + 1;
            child.Marker = mark;
            
            // attempt to parse the content of the marker
            if(Parse(info, child)) {
              if(child.StartIndex > marker.EndIndex) {
                marker.Element.Content.Add(info.Source.Substring(marker.EndIndex, child.StartIndex - marker.EndIndex + 1));
              } else {
                marker.Element.Content.Add(string.Empty);
              }
              marker.Element.AddChild(child.Element);
              marker.EndIndex = info.Index + 1;
            }
            
          }
          
        }
        
        ++info.Index;
      }
      
      // return negative - the marker was missing a closer
      return false;
      
    }
    
    /// <summary>
    /// Parse the source of a parse job.
    /// </summary>
    protected Element Parse(ParseInfo info) {
      
      MarkerInfo marker = new MarkerInfo();
      marker.Element = new Element(Tag.Paragraph);
      marker.Element.Style[StyleKey.WordWrap] = "break-word";
      marker.Element.Style[StyleKey.WhiteSpace] = "pre-wrap";
      marker.EndIndex = marker.StartIndex = 0;
      
      int length = info.Source.Length;
      
      // while there are more characters to read
      while(info.Index < info.Length) {
        
        // move to the next character is there a marker?
        if(info.Search.Next(info.Source[info.Index])) {
          
          var mark = info.Search.Values[0];
          if(mark == null) {
            ++info.Index;
            continue;
          }
          
          // no, attempt to parse the content of the marker
          var child = new MarkerInfo();
          child.StartIndex = info.Index - mark.Prefix.Length + 1;
          child.Marker = mark;
          
          // parse the info
          if(Parse(info, child)) {
            if(child.StartIndex > marker.EndIndex) {
              marker.Element.Content.Add(info.Source.Substring(marker.EndIndex, child.StartIndex - marker.EndIndex));
            } else {
              marker.Element.Content.Add(string.Empty);
            }
            marker.Element.AddChild(child.Element);
            marker.EndIndex = info.Index + 1;
          }
          
        }
        
        ++info.Index;
      }
      
      if(marker.EndIndex < info.Length) {
        // append any remaining content to the base element
        marker.Element.Content.Add(info.Source.Substring(marker.EndIndex, info.Length - marker.EndIndex));
      }
      
      // run the parse callback
      return marker.Element;
      
    }
    
    
  }
  
}
