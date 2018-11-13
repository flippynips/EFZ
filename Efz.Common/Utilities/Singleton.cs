using System;

using Efz.Collections;
using Efz.Data;

namespace Efz {
  
  /// <summary>
  /// Interface to generic identification of Singleton classes.
  /// </summary>
  public interface ISingleton : IDisposable {}
  
  /// <summary>
  /// Classes deriving from Singleton have a single instance automatically
  /// instantiated on start with implied configuration saving and loading.
  /// </summary>
  public abstract class Singleton<TThis> : ISingleton where TThis : Singleton<TThis>, new() {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Explicitly prevent this class from making use of automatic configuration save and load.
    /// </summary>
    protected virtual bool SingletonConfiguration { get { return true; } }
    /// <summary>
    /// Define the priority of this Singleton class. Higher will load earlier in order.
    /// </summary>
    protected virtual byte SingletonPriority { get { return 100; } }
    
    /// <summary>
    /// The type of singleton.
    /// </summary>
    protected Type SingletonType { get; set; }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Singletons have empty constructors.
    /// </summary>
    protected Singleton() {
      SingletonType = this.GetType();
    }
    
    /// <summary>
    /// Don't call this.
    /// </summary>
    public virtual void Dispose() {}
    
    //-------------------------------------------//
    
    /// <summary>
    /// Called when the application starts with all Singletons are set up.
    /// </summary>
    protected virtual void Start() {}
    /// <summary>
    /// Runs when the application is initializes, with the last saved configuration Node.
    /// </summary>
    protected virtual void Setup(Node configuration) {}
    /// <summary>
    /// Runs on application close. Configure a node to load next run.
    /// </summary>
    protected virtual void End(Node configuration) {}
    
    /// <summary>
    /// Called automatically. Calls initialization functions.
    /// </summary>
    private void initialize() {
      
      // create a full path to the Efz configuration
      string path = Fs.Combine(AppDomain.CurrentDomain.BaseDirectory, ".Efz");
      
      // add tasks to load and save the configuration
      if(SingletonConfiguration) {
        
        // add a start event that will load the config with this type as the key and run the Setup method
        ManagerUpdate.OnStart.Add(
          SingletonType.Name,
          ActionSet.New(Setup, ManagerResources.LoadNode(path, SingletonType.Name.Replace("Manager", string.Empty)))
        );
        
        // add end event that will add the actual end event to ensure it's the last called
        ManagerUpdate.OnEnd.Add(
          new ActionSet<string, IAction>(
            onEndAdd,
            SingletonType.Name,
            ActionSet.New(End, ManagerResources.LoadNode(path, SingletonType.Name.Replace("Manager", string.Empty)))
          )
        );
        
      }
      
      // add on done event
      ManagerUpdate.OnStart.AddOnDone(Start);
      
    }
    
    private void onEndAdd(string message, IAction action) {
      ManagerUpdate.OnEnd.Add(message, action);
    }
    
  }
  
  /// <summary>
  /// Management of all singleton classes.
  /// </summary>
  public static class SingletonManager {
    
    /// <summary>
    /// Event called from the main thread after the singletons are disposed.
    /// </summary>
    public static event Action OnClose;
    
    /// <summary>
    /// All singletons in the program.
    /// </summary>
    private static ISingleton[] _singletons;
    /// <summary>
    /// Arguments collection passed to the current application.
    /// </summary>
    private static System.Collections.Generic.Dictionary<string, string> _arguments;
    
    /// <summary>
    /// This function will find all Singleton classes, instantiate them
    /// and call their 'initialize' methods in order of Priority.
    /// </summary>
    public static void Initialize() {
      
      // load assemblies - only once
      LoadAssemblies();
      
      // get all singleton types
      ArrayRig<Type> singletonTypes = new ArrayRig<Type>();
      foreach(var type in Generic.GetTypes(typeof(ISingleton))) singletonTypes.Add(type);
      
      FuncSet<ISingleton>[] constructors = new FuncSet<ISingleton>[singletonTypes.Count];
      ArrayRig<ISingleton> singles = new ArrayRig<ISingleton>(singletonTypes.Count);
      
      // get singleton constructors
      for(int i = 0; i < singletonTypes.Count; ++i) {
        constructors[i] = Dynamic.Constructor<FuncSet<ISingleton>>(singletonTypes[i]);
      }
      // create all singletons
      for(int i = 0; i < constructors.Length; ++i) {
        singles.Add(constructors[i].Func());
      }
      
      // sort singletons in order of decending priority
      singles.Sort((a, b) => Generic.GetValue<byte>(a, "SingletonPriority") > Generic.GetValue<byte>(b, "SingletonPriority"));
      
      // get the array
      _singletons = singles.ToArray();
      
      // initialize in decending order of priority
      for(int i = 0; i < _singletons.Length; ++i) {
        
        // invoke generic initialize method
        Type type = _singletons[i].GetType();
        type.BaseType.GetMethod("initialize", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(_singletons[i], null);
        Log.Info("Created " + type.Name);
      }
      
    }
    
    /// <summary>
    /// Dispose of all singletons.
    /// </summary>
    public static void Dispose() {
      
      // dispose of all singletons in order of ascending priority
      for(int i = _singletons.Length-1; i >= 0; --i) {
        _singletons[i].Dispose();
        Log.Info("Disposed of " + _singletons[i].GetType().Name);
      }
      
      // call the close event
      if(OnClose != null) OnClose();
      
    }
    
    /// <summary>
    /// Get the value of the specified key. Will return an empty string if not specified.
    /// </summary>
    public static string GetArgument(string key) {
      string value;
      return _arguments.TryGetValue(key, out value) ? value : string.Empty;
    }
    
    /// <summary>
    /// Get the collection of arguments.
    /// </summary>
    public static System.Collections.Generic.Dictionary<string,string> GetArguments() {
      return new System.Collections.Generic.Dictionary<string, string>(_arguments);
    }
    
    /// <summary>
    /// Set the current arguments.
    /// </summary>
    public static void SetArguments(string[] arguments) {
      
      // initialize the arguments collection
      _arguments = new System.Collections.Generic.Dictionary<string, string>(arguments.Length);
      // iterate the passed arguments
      foreach(string argument in arguments) {
        string[] split;
        // split each argument as a key-value pair
        if(argument[0] == Chars.Quote && argument[argument.Length-1] == Chars.Quote && argument.Length > 1) {
          split = argument.Substring(1, argument.Length-2).Split(Chars.Equal);
        } else {
          split = argument.Split(Chars.Equal);
        }
        if(split.Length == 2) _arguments[split[0]] = split[1];
        else if(split.Length == 1) _arguments[split[0]] = "true";
      }
      
    }
    
    /// <summary>
    /// Load all referenced assemblies that could include a Singleton.
    /// </summary>
    private static void LoadAssemblies() {
      // load assemblies that reference the Efz.Common assembly ignoring the .Net assemblies
      ArrayRig<System.Reflection.Assembly> loadedAssemblies = new ArrayRig<System.Reflection.Assembly>();
      loadedAssemblies.Add(System.Reflection.Assembly.GetCallingAssembly());
      ArrayRig<System.Reflection.Assembly> nextAssemblies = new ArrayRig<System.Reflection.Assembly>(AppDomain.CurrentDomain.GetAssemblies());
      
      // while there are more assemblies to load
      while(nextAssemblies.Count != 0) {
        
        // iterate the next assemblies to load
        for(int i = nextAssemblies.Count-1; i >= 0; --i) {
          var assembly = nextAssemblies[i];
          
          // remove the assembly
          nextAssemblies.Remove(i);
          
          // is it a system assembly?
          if((assembly.FullName[6] == Chars.Stop ||
            assembly.FullName[6] == Chars.Comma) &&
            assembly.FullName.StartsWith("System", StringComparison.Ordinal) ||
            assembly.FullName.StartsWith("mscorlib", StringComparison.Ordinal)) {
            // yes, skip loading referenced assemblies
            continue;
          }
          
          // get the referenced assemblies
          var referenced = assembly.GetReferencedAssemblies();
          
          // iterate the referenced assemblies
          foreach(var reference in referenced) {
            
            // is the referenced assembly a system assembly?
            if(reference.Name.StartsWith("System", StringComparison.Ordinal) || reference.Name.Equals("mscorlib")) {
              // yes, skip it
              continue;
            }
            
            // does the current loaded assemblies contain the referenced assembly?
            bool found = false;
            foreach(var a in loadedAssemblies) {
              if(a.FullName == reference.FullName) {
                found = true;
                break;
              }
            }
            
            // was the assembly already loaded? yes, skip
            if(found) continue;
            
            // ensure each reference is loaded
            System.Reflection.Assembly next;
            try {
              next = AppDomain.CurrentDomain.Load(reference);
            } catch {
              // gracefully ignore
              continue;
            }
            loadedAssemblies.Add(next);
            nextAssemblies.Add(next);
          }
          
        }
      }
      
      loadedAssemblies.Dispose();
      nextAssemblies.Dispose();
    }
    
  }

}