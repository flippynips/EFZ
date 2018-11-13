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
  public class ExtractLine : Extract {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The size of the first character buffer. Defaults to 50.
    /// The buffer is doubled when resizing is required.
    /// </summary>
    public int CharBuffer = 50;
    /// <summary>
    /// The maximum number of characters allowed in the line.
    /// If exceeded, lines beyond this number are shortenned.
    /// </summary>
    public int MaxChars = 10000;
    
    /// <summary>
    /// Is the on line callback set?
    /// </summary>
    internal bool OnLineSet;
    /// <summary>
    /// Callback on each line.
    /// </summary>
    internal Shared<IAction> OnLine;
    /// <summary>
    /// Is the on line callback set?
    /// </summary>
    internal bool OnLineCharsSet;
    /// <summary>
    /// Callback on each line with the line characters.
    /// </summary>
    internal Shared<IAction<char[]>> OnLineChars;
    
    /// <summary>
    /// Children entities of this extract.
    /// </summary>
    internal ArrayRig<Extract> SubExtracts;
    /// <summary>
    /// Required extracts to exist.
    /// </summary>
    internal ArrayRig<Extract> ReqExtracts;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Empty constructor.
    /// </summary>
    public ExtractLine() {}
    /// <summary>
    /// Initialize with a callback on a line being parsed.
    /// </summary>
    public ExtractLine(Action onLine) : this(new ActionSet(onLine)) {}
    /// <summary>
    /// Initialize with a callback on a line being parsed.
    /// </summary>
    public ExtractLine(Action<char[]> onLine) : this(new ActionSet<char[]>(onLine)) {}
    
    /// <summary>
    /// Initialize with an optional callback.
    /// </summary>
    public ExtractLine(IAction onLine) {
      if(onLine != null) {
        OnLine = new Shared<IAction>(onLine);
        OnLineSet = true;
      }
    }
    
    /// <summary>
    /// Initialize with the specified callbacks, with or without the line characters.
    /// </summary>
    public ExtractLine(IAction<char[]> onLineChars, IAction onLine = null) {
      if(onLineChars != null) {
        OnLineChars = new Shared<IAction<char[]>>(onLineChars);
        OnLineCharsSet = true;
      }
      if(onLine != null) {
        OnLine = new Shared<IAction>(onLine);
        OnLineSet = true;
      }
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
    public void AddRequirement(Extract extract) {
      if(ReqExtracts == null) ReqExtracts = new ArrayRig<Extract>();
      ReqExtracts.Add(extract);
    }
    
    /// <summary>
    /// Retrieve a new instance of the parser associated with this text entity.
    /// </summary>
    public override Parse GetParser() {
      return new ParseLine(this);
    }
    
    //-------------------------------------------//
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
  }
}
