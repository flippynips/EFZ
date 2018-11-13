/*
 * User: Joshua
 * Date: 20/08/2016
 * Time: 10:03 AM
 */
using System;
using System.Text;

using Efz.Collections;
using Efz.Threading;
using Efz.Tools;

namespace Efz.Text {
  
  /// <summary>
  /// Instance of parsing a rule.
  /// </summary>
  public class ExtractSection : Extract {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Prefixes to look for.
    /// </summary>
    public TreeSearch<char, string> Prefixes;
    /// <summary>
    /// Suffixes to look for.
    /// </summary>
    public TreeSearch<char, string> Suffixes;
    /// <summary>
    /// The encoder used to parse the section.
    /// </summary>
    public Encoder Encoder;
    
    /// <summary>
    /// Maximum length of the text section. Default is 20000.
    /// </summary>
    public int MaxCharacters = 20000;
    
    /// <summary>
    /// Has the on section method been set?
    /// </summary>
    internal bool OnSectionSet;
    /// <summary>
    /// Called when the parser gets a resulting section of text.
    /// </summary>
    internal Shared<IAction<char[]>> OnSection;
    
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
    public ExtractSection(Action<char[]> onSection) : this(new ActionSet<char[]>(onSection)) {}
    /// <summary>
    /// Initialize a new scheme parser instance.
    /// </summary>
    public ExtractSection(IAction<char[]> onSection = null) {
      if(onSection != null) {
        OnSection = new Shared<IAction<char[]>>(onSection);
        OnSectionSet = true;
      }
      Encoder = Encoding.GetEncoder();
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
      OnSection = null;
    }
    
    /// <summary>
    /// Add a prefix to this section extract.
    /// </summary>
    public void AddPrefix(string prefix) {
      if(Prefixes == null) Prefixes = new TreeSearch<char, string>();
      Prefixes.Add(prefix, prefix.ToCharArray());
    }
    
    /// <summary>
    /// Add a suffix to this section extract.
    /// </summary>
    public void AddSuffix(string suffix) {
      if(Suffixes == null) Suffixes = new TreeSearch<char, string>();
      Suffixes.Add(suffix, suffix.ToCharArray());
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
      return new ParseSection(this);
    }
    
    //-------------------------------------------//
    
  }
}
