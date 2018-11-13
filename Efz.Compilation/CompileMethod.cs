using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Efz.Compilation {
  
  /// <summary>
  /// Compile a method.
  /// </summary>
  public class CompileMethod : Compile {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Name of the method.
    /// </summary>
    public string Name;
    /// <summary>
    /// Name of the class into which the method should be compiled.
    /// If not specified, the method will be compiled into a general
    /// class.
    /// </summary>
    public string Class = "Methods";
    
    //-------------------------------------------//
    
    /// <summary>
    /// Callback on method compilation.
    /// </summary>
    protected ActionActive<MethodInfo> _onCompile;
    
    /// <summary>
    /// The prefix for any compiled method.
    /// </summary>
    private const string _methodPrefix = "using System; namespace Efz.Runtime{public partial class ";
    /// <summary>
    /// The suffix for any compiled method.
    /// </summary>
    private const string _methodSuffix = "}}";
    
    //-------------------------------------------//
    
    public CompileMethod(string name, Action<MethodInfo> onCompile, Action<string> onError) {
      Name = name;
      _onCompile = new ActionActive<MethodInfo>(onCompile);
      _onError = new ActionActive<string>(onError);
      
      Script = new StringBuilder(_methodPrefix);
      Script.Append(Class);
      Script.Append('{');
    }
    
    /// <summary>
    /// Start compiling the assembly.
    /// </summary>
    override public void CompileAssembly() {
      Script.Append(_methodSuffix);
      base.CompileAssembly();
    }                                                                  
        
    //-------------------------------------------//
        
    protected override void OnCompiled() {
      foreach(Type t in _assembly.GetTypes()) {
        if(t.Name.Equals("Scripts")) {
          _onCompile.ArgA = t.GetMethod(Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
          _onCompile.Run();
          break;
        }
      }
    }
    
  }
    
}
