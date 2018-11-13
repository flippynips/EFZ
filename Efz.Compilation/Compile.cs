using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;

using Efz.Tools;

namespace Efz.Compilation {
  
  /// <summary>
  /// Base class for compiling various structures at runtime.
  /// </summary>
  public class Compile {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The script to compile.
    /// </summary>
    public StringBuilder Script;
    
    //-------------------------------------------//
    
    /// <summary>
    /// The Assembly into which the script will be compiled.
    /// </summary>
    protected Assembly _assembly;
    /// <summary>
    /// Callback on the script compilation causing an error.
    /// </summary>
    protected ActionActive<string> _onError;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Create an empty compile instance with an unassigned string builder instance.
    /// </summary>
    public Compile() {}
    
    /// <summary>
    /// Create a compiler instance with the specified callback on compilation errors.
    /// </summary>
    public Compile(Action<string> onError) {
      _onError = onError;
      Script   = new StringBuilder();
    }
    
    /// <summary>
    /// Start compiling the script.
    /// </summary>
    public virtual void CompileAssembly() {
      
      CompilerParameters parameters;
      CodeDomProvider    provider;
      CompilerResults    results;
      
      provider   = CodeDomProvider.CreateProvider("CSharp");
      parameters = new CompilerParameters {
        GenerateExecutable    = false,
        GenerateInMemory      = true,
        TreatWarningsAsErrors = false
      };
      
      // add references to all the assemblies we might need
      _assembly = Assembly.GetExecutingAssembly();
      parameters.ReferencedAssemblies.Add(_assembly.Location);
      
      // iterate the referenced assemblies to find the target
      foreach(AssemblyName assemblyName in _assembly.GetReferencedAssemblies()) {
        // add each referenced assembly
        parameters.ReferencedAssemblies.Add(Assembly.Load(assemblyName).Location);
      }
      
      // invoke compilation of the source file
      results = provider.CompileAssemblyFromSource(parameters, Script.ToString());
      
      // were there any errors with the compilation?
      if(results.Errors.Count > 0) {
        
        // get a builder for the cache
        StringBuilder builder = StringBuilderCache.Get();
        // append all compilation errors
        foreach(CompilerError error in results.Errors) {
          builder.Append(error.ToString());
          builder.Append('\n');
        }
        
        // run the error callback
        _onError.ArgA = builder.ToString();
        _onError.Run();
      } else {
        
        // persist the resulting assembly
        _assembly = results.CompiledAssembly;
        
        // run the compilation complete method
        OnCompiled();
      }
      
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Method on compile.
    /// </summary>
    protected virtual void OnCompiled() { }
    
  }
  
}
