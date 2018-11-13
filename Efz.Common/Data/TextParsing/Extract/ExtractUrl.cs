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
  public class ExtractUrl : Extract {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Max length of urls to parse.
    /// </summary>
    public int MaxLength = 200;
    
    /// <summary>
    /// Run when an element is found.
    /// </summary>
    internal Shared<IAction<string>> OnUrl;
    /// <summary>
    /// Is the on element callback set?
    /// </summary>
    internal bool OnUrlSet;
    
    /// <summary>
    /// Sub url extracts.
    /// </summary>
    internal ArrayRig<Extract> SubExtract;
    /// <summary>
    /// Are there any body extracts.
    /// </summary>
    internal bool SubExtractSet;
    
    /// <summary>
    /// The protocol search.
    /// </summary>
    internal TreeSearch<char, char[]> ProtocolSearch;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Task to run when an element that matches this rule is found.
    /// Contains the element body and parameters.
    /// </summary>
    protected IAction<string> _onUrl;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize an element rule with a tag to parse and parameters to return.
    /// </summary>
    public ExtractUrl(IAction<string> onUrl = null, ArrayRig<Protocol> protocols = null) {
      if(onUrl != null) {
        OnUrl = new Shared<IAction<string>>(onUrl);
        OnUrlSet = true;
      }
      
      // initialize the tree search instance
      ProtocolSearch = new TreeSearch<char, char[]>();
      
      // were the protocols specified?
      if(protocols == null) {
        // no, set default
        protocols = new ArrayRig<Protocol>(new [] { Protocol.Http, Protocol.Https });
      }
      
      // iterate the protocols
      foreach(var protocol in protocols) {
        char[] chars = Protocols.Get(protocol).ToCharArray();
        ProtocolSearch.Add(chars, new ArrayRig<char>(chars));
      }
    }
    
    /// <summary>
    /// Dispose of this Extract instance.
    /// </summary>
    public override void Dispose() {
      if(SubExtractSet) SubExtract.Dispose();
      ProtocolSearch.Dispose();
    }
    
    /// <summary>
    /// Add a tag for this element extraction.
    /// </summary>
    public void AddProtocol(string protocol) {
      char[] chars = protocol.ToCharArray();
      ProtocolSearch.Add(chars, new ArrayRig<char>(chars));
    }
    
    /// <summary>
    /// Add a sub extract to act on the body of the element if it exists.
    /// </summary>
    public void AddSubExtract(Extract extract) {
      if(!SubExtractSet) {
        SubExtractSet = true;
        SubExtract = new ArrayRig<Extract>();
      }
      SubExtract.Add(extract);
    }
    
    /// <summary>
    /// Retrieve a new instance of the parser associated with this text entity.
    /// </summary>
    public override Parse GetParser() {
      return new ParseUrl(this);
    }
    
    //-------------------------------------------//
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
  }
}
