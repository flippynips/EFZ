/*
 * User: Joshua
 * Date: 20/08/2016
 * Time: 10:03 AM
 */
using System;

using Efz.Collections;

namespace Efz.Text {
  
  /// <summary>
  /// Instance of parsing a rule.
  /// </summary>
  public class ExtractAll : Extract {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Children entities of this extract section.
    /// </summary>
    internal ArrayRig<Extract> SubExtracts;
    /// <summary>
    /// Required extracts to exist within this section for the section to be processed.
    /// </summary>
    internal ArrayRig<Extract> ReqExtracts;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new scheme parser instance.
    /// </summary>
    public ExtractAll() {
    }
    
    /// <summary>
    /// Dispose of this Extract instance.
    /// </summary>
    public override void Dispose() {
      foreach(Extract extract in 
      SubExtracts) extract.Dispose();
      foreach(Extract extract in ReqExtracts) extract.Dispose();
      SubExtracts.Dispose();
      ReqExtracts.Dispose();
    }
    
    /// <summary>
    /// Add a sub extract that will be parsed on a successful
    /// section.
    /// </summary>
    public void AddSubExtract(Extract extract) {
      if(SubExtracts == null) SubExtracts = new ArrayRig<Extract>();
      SubExtracts.Add(extract);
    }
    
    /// <summary>
    /// Add a sub extract that must exist within this section for it to be
    /// parsed successfully.
    /// </summary>
    public void AddReqExtract(Extract extract) {
      if(ReqExtracts == null) ReqExtracts = new ArrayRig<Extract>();
      ReqExtracts.Add(extract);
    }
    
    /// <summary>
    /// On a parsed section.
    /// </summary>
    public ParseAll GetParser(Action<string> onParsed) {
      return GetParser(new ActionSet<string>(onParsed));
    }
    
    /// <summary>
    /// On a parsed section.
    /// </summary>
    public ParseAll GetParser(IAction<string> onParsed) {
      return GetParser(onParsed); 
    }
    
    /// <summary>
    /// Retrieve a new instance of the parser associated with this text entity.
    /// </summary>
    public override Parse GetParser() {
      return new ParseAll(this);
    }
    
    //-------------------------------------------//
    
  }
}
