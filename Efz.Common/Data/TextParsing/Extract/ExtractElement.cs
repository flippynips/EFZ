/*
 * User: Joshua
 * Date: 6/08/2016
 * Time: 9:48 PM
 */
using System;

using Efz.Collections;
using Efz.Threading;
using Efz.Tools;

namespace Efz.Text {
  
  /// <summary>
  /// A description for finding html dom elements.
  /// </summary>
  public class ExtractElement : Extract {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Whether to parse the body of the element. Default is 'true'.
    /// </summary>
    public bool ParseBody = true;
    
    /// <summary>
    /// Is the on element callback set?
    /// </summary>
    internal bool OnElementSet;
    /// <summary>
    /// Run when an element is found.
    /// </summary>
    internal Shared<IAction<Element>> OnElement;
    
    /// <summary>
    /// Tags that will be parsed as a part of this element extraction.
    /// </summary>
    internal ArrayRig<string> Tags;
    
    /// <summary>
    /// Attribute keys of which values are extracted.
    /// </summary>
    internal TreeSearch<char, string> GetAttributes;
    /// <summary>
    /// Attribute key-value pairs that are required for the element to be parsed.
    /// </summary>
    internal TreeSearch<char, string> FilterAttributes;
    /// <summary>
    /// Number of required attributes.
    /// </summary>
    internal int FilterAttributesCount;
    
    /// <summary>
    /// Body extracts.
    /// </summary>
    internal ArrayRig<Extract> BodyExtracts;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize an element rule with a tag to parse and parameters to return.
    /// </summary>
    public ExtractElement(Action<Element> onElement = null) : this(new ActionSet<Element>(onElement)) {
    }
    
    /// <summary>
    /// Initialize an element rule with a tag to parse and parameters to return.
    /// </summary>
    public ExtractElement(IAction<Element> onElement = null) {
      if(onElement != null) {
        OnElement = new Shared<IAction<Element>>(onElement);
        OnElementSet = true;
      }
      
      Tags = new ArrayRig<string>();
    }
    
    /// <summary>
    /// Dispose of this Extract instance.
    /// </summary>
    public override void Dispose() {
      Tags.Dispose();
      GetAttributes.Dispose();
      BodyExtracts.Dispose();
    }
    
    /// <summary>
    /// Add a tag for this element extraction.
    /// </summary>
    public void AddTag(string tag) {
      // add the tag to the section extraction
      Tags.Add(tag);
    }
    
    /// <summary>
    /// Add an attribute to filter.
    /// </summary>
    public void GetAttribute(string key) {
      if(GetAttributes == null) GetAttributes = new TreeSearch<char, string>();
      // add the parameter key
      GetAttributes.Add(key, (Chars.Space + key + Chars.Equal).ToCharArray());
    }
    
    /// <summary>
    /// Filter by the specified attribute.
    /// </summary>
    public void FilterAttribute(string key, string value = null) {
      if(FilterAttributes == null) FilterAttributes = new TreeSearch<char, string>();
      ++FilterAttributesCount;
      FilterAttributes.Add(value, key.ToCharArray());
    }
    
    /// <summary>
    /// Add a sub extract to act on the body of the element if it exists.
    /// </summary>
    public void AddBodyExtract(Extract extract) {
      if(BodyExtracts == null) BodyExtracts = new ArrayRig<Extract>();
      BodyExtracts.Add(extract);
    }
    
    /// <summary>
    /// Retrieve a new instance of the parser associated with this text entity.
    /// </summary>
    public override Parse GetParser() {
      return new ParseElement(this);
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Represents an extracted element.
    /// </summary>
    public struct Element {
      public ArrayRig<Struct<string,string>> Parameters;
      public string Body;
      public string Tag;
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
  }
}
