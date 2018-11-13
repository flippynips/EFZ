using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Efz.Data;
using Efz.Threading;
using Efz.Tools;

namespace Efz {
  
  /// <summary>
  /// Manager for common configurations. Ensures threadsafe configuration access and creation.
  /// </summary>
  public class ManagerResources : Singleton<ManagerResources> {
    
    //-------------------------------------------//
		
		/// <summary>
    /// Manager of resources will be initialized third in likely all cases.
    /// </summary>
    protected override byte SingletonPriority { get { return 240; } }
		
		/// <summary>
		/// Local resources directory.
		/// </summary>
		public static readonly string ResourcePath = ".Resources";
		/// <summary>
		/// Full path to the local program directory.
		/// </summary>
		public static readonly string ProgramPath;
		/// <summary>
		/// Name of the program which can generally found at the program directory.
		/// </summary>
		public static readonly string ProgramName;
		
    //-------------------------------------------//
		
		/// <summary>
		/// Roll collection of local file paths to be removed.
		/// </summary>
    private static ActionRoll<string, int> _toRemove;
    /// <summary>
    /// Sequence of actions to remove local files;
    /// </summary>
    private static ActionSequence _removeSequence;
		
		/// <summary>
		/// Current active configurations.
		/// </summary>
    private static Dictionary<string, WeakReferencer<Configuration>> _configurations;
    /// <summary>
    /// Lock used for external access to un-threadsafe methods of the resource manager.
    /// </summary>
    private static LockShared _lock;
    
    //-------------------------------------------//
		
    static ManagerResources() {
      ProgramPath = AppDomain.CurrentDomain.BaseDirectory;
      ProgramName = AppDomain.CurrentDomain.FriendlyName;
      
      ResourcePath = Fs.Combine(ProgramPath, ResourcePath);
      
      _configurations = new Dictionary<string, WeakReferencer<Configuration>>();
      _lock = new LockShared();
    }
		
    /// <summary>
    /// Get or create the file specified by the path parameters.
    /// </summary>
    public static string CreateFilePath(params string[] path) {
      StringBuilder builder = new StringBuilder();
      for(int i = 0; i < path.Length-1; ++i) {
        builder.Append(path[i]);
        builder.Append(Chars.ForwardSlash);
        if(!Directory.Exists(builder.ToString())) {
          Directory.CreateDirectory(builder.ToString());
        }
      }
      builder.Append(path[path.Length-1]);
      return builder.ToString();
    }
    
    /// <summary>
    /// Get or create the directory specified by path.
    /// </summary>
    public static DirectoryInfo CreatePath(string directory, bool file) {
      string[] paths = directory.Split(Path.DirectorySeparatorChar);
      StringBuilder builder = new StringBuilder(directory.Length);
      DirectoryInfo dir = null;
      for(int i = 0; i < paths.Length; ++i) {
        builder.Append(paths[i]);
        builder.Append(Path.DirectorySeparatorChar);
        dir = new DirectoryInfo(builder.ToString());
        if(file) {
          if(i != paths.Length-1 && !dir.Exists) {
            dir.Create();
          }
        } else if(!dir.Exists) {
          dir.Create();
        }
      }
      return dir;
    }
    
    /// <summary>
    /// Remove a local file will fail after the specified successive attempts.
    /// </summary>
    public static void RemoveFile(string path, int attempts = 3) {
      TryRemoveFile(path, attempts);
    }
		
		/// <summary>
    /// Get a configuration by path synchronously/asynchronously.
    /// </summary>
    public static Configuration LoadConfig(string path) {
      // get the lock
      _lock.Take();
      
      // get the reference to the configuration
      WeakReferencer<Configuration> reference;
      if(!_configurations.TryGetValue(path, out reference)) {
        reference = _configurations[path] = new WeakReferencer<Configuration>(new FuncSet<string, Configuration>(GetConfiguration, path));
      }
      
      Configuration config = reference.Item;
      
      // check if the configuration was loaded
      if(config.State != AssetState.Loaded) config.Load();
      
      _lock.Release();
      return config;
    }
		
    /// <summary>
    /// Get a configuration by path.
    /// </summary>
    public static void LoadConfig(string path, Act<Configuration> onLoad) {
      LoadConfig(path, onLoad, true);
    }
    private static void LoadConfig(string path, Act<Configuration> onLoad, bool takeLock) {
      // get the lock
      if(takeLock && !(takeLock = _lock.TryTake)) {
        _lock.TryLock(new Act<string, Act<Configuration>, bool>(LoadConfig, path, onLoad, false));
        return;
      }
      
      // get the reference to the configuration
      WeakReferencer<Configuration> reference;
      if(!_configurations.TryGetValue(path, out reference)) {
        reference = _configurations[path] = new WeakReferencer<Configuration>(new FuncSet<string, Configuration>(GetConfiguration, path));
      }
      
      Configuration config = reference.Item;
      
      // check if the configuration was loaded
      if(config.State == AssetState.Loaded) {
        onLoad.ArgA = config;
      } else {
        config.Load(onLoad);
      }
      
      if(takeLock) _lock.Release();
    }
    
    /// <summary>
    /// Get a configuration by path.
    /// </summary>
    public static void SaveConfig(string path, Node node, Act<Configuration> onSave = null) {
      SaveConfig(path, node, onSave, true);
    }
    private static void SaveConfig(string path, Node node, Act<Configuration> onSave, bool takeLock) {
      // get the lock
      if(takeLock && !(takeLock = _lock.TryTake)) {
        _lock.TryLock(new Act<string, Node, Act<Configuration>, bool>(SaveConfig, path, node, onSave, false));
        return;
      }
      
      WeakReferencer<Configuration> reference;
      if(!_configurations.TryGetValue(path, out reference)) {
        reference = _configurations[path] = new WeakReferencer<Configuration>(new FuncSet<string, Configuration>(GetConfiguration, path));
      }
      
      // set node and save
      Configuration config = reference.Item;
      config.Node = node;
      config.Save(onSave);
      
      if(takeLock) _lock.Release();
    }
    
    /// <summary>
    /// Get configuration node by path and key.
    /// </summary>
    public static void LoadNode(Act<Node> onLoad, string path, params string[] keys) {
      LoadNode(onLoad, path, keys, true);
    }
    private static void LoadNode(Act<Node> onLoad, string path, string[] keys, bool takeLock) {
      // get the lock
      if(takeLock && !(takeLock = _lock.TryTake)) {
        _lock.TryLock(new Act<Act<Node>, string, string[], bool>(LoadNode, onLoad, path, keys, false));
        return;
      }
      
      // get the configuration referencer
      WeakReferencer<Configuration> reference; 
      if(!_configurations.TryGetValue(path, out reference)) {
        reference = new WeakReferencer<Configuration>(new FuncSet<string, Configuration>(GetConfiguration, path));
        _configurations.Add(path, reference);
      }
      
      Configuration config = reference.Item;
      
      // check if configuration was loaded
      if(config.State == AssetState.Loaded) {
        onLoad.ArgA = config.Node[keys];
        onLoad.Run();
      } else {
        // the task will retrieve the node and run the on load function when the configuration loads
        config.Load(new Act<Configuration>(c => {
          onLoad.ArgA = c.Node[keys];
          onLoad.Action.Run();
        }, onLoad.Needle));
      }
      
      // unlock if locked
      if(takeLock) _lock.Release();
    }
    
    /// <summary>
    /// Load a node immediately and return it. Returns 'Null' if the Node could not be loaded.
    /// </summary>
    public static Node LoadNode(string path, params string[] keys) {
      
      // get the lock
      _lock.Take();
      
      // get the configuration referencer
      WeakReferencer<Configuration> reference;
      if(!_configurations.TryGetValue(path, out reference)) {
        reference = new WeakReferencer<Configuration>(new FuncSet<string, Configuration>(GetConfiguration, path));
        _configurations.Add(path, reference);
      }
      Configuration config = reference.Item;
      
      // check if configuration was loaded
      if(!config.State.Is(AssetState.Loaded)) {
        // the pocket will retrieve the node and run the on load function when the configuration loads
        config.Load();
      }
      
      // unlock
      _lock.Release();
      
      // if loaded successfully return the node else null
      return config.State.Is(AssetState.Loaded) ? config.Node[keys] : null;
    }
    
    /// <summary>
    /// Save configuration aynchronously by path and keys.
    /// </summary>
    public static void SaveNode(Node node, string path, params string[] keys) {
      // skip null nodes
      if(node == null) return;
      SaveNode(node, path, keys, true);
    }
    private static void SaveNode(Node node, string path, string[] keys, bool takeLock) {
      // get the lock
      if(takeLock && !(takeLock = _lock.TryTake)) {
        _lock.TryLock(new Act<Node, string, string[], bool>(SaveNode, node, path, keys, false));
        return;
      }
      
      WeakReferencer<Configuration> reference;
      if(!_configurations.TryGetValue(path, out reference)) {
        _configurations[path] = reference = new WeakReferencer<Configuration>(new FuncSet<string, Configuration>(GetConfiguration, path));
      }
      
      Configuration config = reference.Item;
      
      if(keys.Length == 0) config.Node = node;
      else config.Node[keys] = node;
      
      reference.Item.Save();
      
      if(takeLock) _lock.Release();
    }
    
    /// <summary>
    /// Save configuration by path and key. If there is no callback on save, the node is saved inline.
    /// </summary>
    public static void SaveNode(Node node, Act<Configuration> onSave, string path, params string[] keys) {
      // skip null nodes
      if(node == null) return;
      SaveNode(node, onSave, path, keys, true);
    }
    private static void SaveNode(Node node, Act<Configuration> onSave, string path, string[] keys, bool takeLock) {
      // get the lock
      if(takeLock && !(takeLock = _lock.TryTake)) {
        _lock.TryLock(new Act<Node, Act<Configuration>, string, string[], bool>(SaveNode, node, onSave, path, keys, false));
        return;
      }
      
      WeakReferencer<Configuration> reference;
      if(!_configurations.TryGetValue(path, out reference)) {
        _configurations[path] = reference = new WeakReferencer<Configuration>(new FuncSet<string, Configuration>(GetConfiguration, path));
      }
      
      Configuration config = reference.Item;
      
      config.Node[keys] = node;
      reference.Item.Save(onSave);
      
      if(takeLock) _lock.Release();
    }
    
    //-------------------------------------------//
		
		/// <summary>
		/// Try remove a local file.
		/// </summary>
    protected static void TryRemoveFile(string path, int count) {
      
		  try {
		    File.Delete(path);
		    return;
		  } catch(Exception ex) {
		    Log.Debug("Couldn't delete file. " + ex);
		  }
      
		  if(--count == 0) {
		    Log.Error("Couldn't remove file '"+path+"'.");
		    return;
		  }
      
      _toRemove.Add(path, count);
      new Timer(2000, ActionSet.New(_removeSequence.AddRun, _toRemove));
      
    }
		
		/// <summary>
		/// Method used by weak references to re-create configurations.
		/// </summary>
    protected static Configuration GetConfiguration(string path) {
		  return new Configuration(path);
    }
		
		/// <summary>
		/// Start the resource manager.
		/// </summary>
    protected override void Setup(Node configuration) {
      
      _toRemove = new ActionRoll<string, int>(TryRemoveFile);
      _removeSequence = new ActionSequence(ManagerUpdate.Iterant);

      // add action to save configuration on start
      //Configuration config = LoadConfig(Fs.Combine(AppDomain.CurrentDomain.BaseDirectory, ".Efz"));
      //ManagerUpdate.OnStart.AddOnDone(() => config.Save());
      ManagerUpdate.OnStart.AddOnDone(() => SaveNode(configuration.Parent, Fs.Combine(AppDomain.CurrentDomain.BaseDirectory, ".Efz")));
    }
    
    /// <summary>
    /// End the resource manager.
    /// </summary>
    protected override void End(Node configuration) {
      
      // add action to save configuration on end
      Configuration config = LoadConfig(".Efz");
      ManagerUpdate.OnEnd.AddOnDone(() => config.Save());
      
    }
    
  }
  
  
}
