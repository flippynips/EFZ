/*
 * User: Joshua
 * Date: 6/08/2016
 * Time: 9:48 PM
 */
using System;

using Efz.Collections;
using Efz.Threading;

namespace Efz.Text {
  
  /// <summary>
  /// A description for finding html dom elements.
  /// </summary>
  public class ExtractIpAddress : Extract {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Max length of ip addresses can be.
    /// </summary>
    public int MaxLength = 46;
    
    /// <summary>
    /// Run when an element is found.
    /// </summary>
    internal Shared<IAction<string, bool>> OnIpAddress;
    /// <summary>
    /// Is the on element callback set?
    /// </summary>
    internal bool OnIpAddressSet;
    
    /// <summary>
    /// Sub url extracts.
    /// </summary>
    internal ArrayRig<Extract> SubExtract;
    /// <summary>
    /// Are there any body extracts.
    /// </summary>
    internal bool SubExtractSet;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize an element rule with a tag to parse and parameters to return.
    /// </summary>
    public ExtractIpAddress(Action<string, bool> onIpAddress) : this(new ActionSet<string, bool>(onIpAddress)) { }
    /// <summary>
    /// Initialize an element rule with a tag to parse and parameters to return.
    /// </summary>
    public ExtractIpAddress(IAction<string, bool> onIpAddress = null) {
      if(onIpAddress != null) {
        OnIpAddress = new Shared<IAction<string, bool>>(onIpAddress);
        OnIpAddressSet = true;
      }
    }
    
    /// <summary>
    /// Dispose of this Extract instance.
    /// </summary>
    public override void Dispose() {
      if(SubExtractSet) SubExtract.Dispose();
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
      return new ParseIpAddress(this);
    }
    
    //-------------------------------------------//
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
  }
}
