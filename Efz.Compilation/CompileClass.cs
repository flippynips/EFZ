using System;
using System.Text;

namespace Efz.Compilation {
  
  /// <summary>
  /// Compile a class at runtime.
  /// </summary>
  public class CompileClass : Compile {
  
    //-------------------------------------------//
    
    /// <summary>
    /// Name of the class to compile.
    /// </summary>
    public string Name;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Callback on compile completion.
    /// </summary>
    protected ActionActive<Type> _onCompile;
    
    /// <summary>
    /// Prefix to the class to be generated.
    /// </summary>
    protected const string _classPrefix  = "using System;namespace Efz.Runtime{";
    /// <summary>
    /// Suffix to the class to be generated.
    /// </summary>
    protected const string _classSuffix  = "}";
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a compilation constructor for a class.
    /// </summary>
    public CompileClass(string name, Action<Type> onCompile, Action<string> onError ) {
      Name      = name;
      _onCompile = new ActionActive<Type>(onCompile);
      _onError   = new ActionActive<string>(onError);
      Script     = new StringBuilder(_classPrefix);
    }
    
    /// <summary>
    /// Compile the class.
    /// </summary>
    override public void CompileAssembly() {
      Script.Append(_classSuffix);
      base.CompileAssembly();
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Method on compile.
    /// </summary>
    protected override void OnCompiled() {
      _onCompile.ArgA = _assembly.GetType("Efz.Runtime." + Name);
      _onCompile.Run();
    }
    
  }
}
