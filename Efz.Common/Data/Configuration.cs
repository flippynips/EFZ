using System;

using Efz.Network;
using Efz.Threading;
using Efz.Tools;

namespace Efz.Data {
  
  /// <summary>
  /// Helper class to load and store data nodes easily. Nodes are stored in a format slightly more compact than JSON.
  /// Configurations are automatically saved before being disposed.
  /// </summary>
  public class Configuration {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The current state of the configuration.
    /// </summary>
    public AssetState State;
    
    /// <summary>
    /// Node the configuration refers to.
    /// </summary>
    public Node Node;
    /// <summary>
    /// The path to this configuration.
    /// </summary>
    public string Path { get { return _path; } }
    
    /// <summary>
    /// Returns the node value with the specified key.
    /// </summary>
    public Node this[string key] {
      get {
        if(State == AssetState.Unloaded) {
          Load();
        }
        return Node[key];
      }
      set {
        Node[key] = value;
      }
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Path to the configuration file.
    /// </summary>
    protected string _path;
    
    /// <summary>
    /// Callback on configuration loaded.
    /// </summary>
    protected ActionEvent<Configuration> _onLoad;
    /// <summary>
    /// Lock for singular access to the configuration.
    /// </summary>
    protected Lock _lock;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initlialize a configuration with the specified path for saving/loading.
    /// </summary>
    public Configuration(string path) {
      Node = new Node();
      _onLoad = new ActionEvent<Configuration>();
      _lock = new Lock();
      _path = path;
    }
    
    /// <summary>
    /// Manually stop the connection between this configuration used for saving/loading.
    /// </summary>
    public void StopConnection() {
      ManagerConnections.Remove<ConnectionLocal>(_path);
    }
    
    /// <summary>
    /// Load the configuration inline if the callback is null.
    /// </summary>
    public void Load(IAction<Configuration> onLoad = null) {
      _lock.Take();
      // check state
      switch(State) {
        case AssetState.Unloaded:
        case AssetState.Broken:
          State = AssetState.Loading;
          
          // inline load or callback?
          if(onLoad == null) {
            // no callback so load immediately
            
            // get the connection
            ConnectionLocal connection = ManagerConnections.Get<ConnectionLocal>(_path);
            // ensure the file modes are right
            connection.FileMode = System.IO.FileMode.OpenOrCreate;
            connection.FileAccess = System.IO.FileAccess.Read;
            
            // get the stream
            Teple<LockShared, ByteBuffer> resource;
            connection.Get(out resource);
            
            // run load logic
            OnLoad(resource.ArgB);
            resource.ArgA.Release();
            
          } else {
            _onLoad += onLoad;
            
            // get the connection
            ConnectionLocal connection = ManagerConnections.Get<ConnectionLocal>(_path);
            // ensure the file modes are right
            connection.FileMode = System.IO.FileMode.OpenOrCreate;
            connection.FileAccess = System.IO.FileAccess.Read;
            
            // callback load
            connection.Get(new Act<ByteBuffer>(OnLoad));
          }
          break;
        case AssetState.Loading:
          _onLoad += onLoad;
          _lock.Release();
          break;
        default:
          _lock.Release();
          onLoad.ArgA = this;
          onLoad.Run();
          break;
      }
    }
    
    /// <summary>
    /// Save the configuration.
    /// </summary>
    public void Save(Act<Configuration> onSave = null) {
      _lock.Take();
      if(State.Is(AssetState.Loaded) || State.Is(AssetState.Unloaded)) {
        State = AssetState.Saving;
        
        if(onSave == null) {
          // get the connection
          ConnectionLocal connection = ManagerConnections.Get<ConnectionLocal>(_path);
          // ensure the file modes are right
          connection.FileMode = System.IO.FileMode.Truncate;
          connection.FileAccess = System.IO.FileAccess.Write;
          connection.FileShare = System.IO.FileShare.Read;
          
          // get the stream
          Teple<LockShared, ByteBuffer> resource;
          connection.Get(out resource);
          
          // run save logic
          OnSave(resource.ArgB);
          resource.ArgA.Release();
          
        } else {
          onSave.ArgA = this;
          
          // get the connection
          ConnectionLocal connection = ManagerConnections.Get<ConnectionLocal>(_path);
          
          // ensure the file modes are right
          connection.FileMode = System.IO.FileMode.Create;
          connection.FileAccess = System.IO.FileAccess.Write;
          
          // callback save
          connection.Get(new Act<ByteBuffer>(new ActionPair<ByteBuffer>(OnSave, s => onSave.Run())));
        }
      } else {
        _lock.Release();
      }
    }
    
    /// <summary>
    /// Gets a string value on the top level of the config. Replaces it with default if it equals null.
    /// </summary>
    public string GetString(string key, string @default = null) {
      Node value;
      if(Node.DictionarySet && Node.Dictionary.TryGetValue(key, out value)) return value.String;
      return Node[key].String = @default;
    }
    /// <summary>
    /// Gets a double value on the top level of the config. Replaces it with default if it equals 0.
    /// </summary>
    public double GetDouble(string key, double @default = 0.0) {
      Node value;
      if(Node.DictionarySet && Node.Dictionary.TryGetValue(key, out value)) return value.Double;
      return Node[key].Double = @default;
    }
    /// <summary>
    /// Gets a fload value on the top level of the config. Replaces it with default if it equals 0.
    /// </summary>
    public double GetFloat(string key, float @default = 0.0f) {
      Node value;
      if(Node.DictionarySet && Node.Dictionary.TryGetValue(key, out value)) return value.Float;
      return Node[key].Float = @default;
    }
    /// <summary>
    /// Gets an integer value on the top level of the config. Replaces it with default if it equals 0.
    /// </summary>
    public int GetInt32(string key, int @default = 0) {
      Node value;
      if(Node.DictionarySet && Node.Dictionary.TryGetValue(key, out value)) return value.Int32;
      return Node[key].Int32 = @default;
    }
    /// <summary>
    /// Gets an unsigned integer value on the top level of the config. Replaces it with default if it equals 0.
    /// </summary>
    public uint GetUInt32(string key, uint @default = 0u) {
      Node value;
      if(Node.DictionarySet && Node.Dictionary.TryGetValue(key, out value)) return value.UInt32;
      return Node[key].UInt32 = @default;
    }
    /// <summary>
    /// Gets an integer value on the top level of the config. Replaces it with default if it equals 0.
    /// </summary>
    public long GetInt64(string key, long @default = 0) {
      Node value;
      if(Node.DictionarySet && Node.Dictionary.TryGetValue(key, out value)) return value.Int64;
      return Node[key].Int64 = @default;
    }
    /// <summary>
    /// Gets an unsigned integer value on the top level of the config. Replaces it with default if it equals 0.
    /// </summary>
    public ulong GetUInt64(string key, ulong @default = 0ul) {
      Node value;
      if(Node.DictionarySet && Node.Dictionary.TryGetValue(key, out value)) return value.UInt64;
      return Node[key].UInt64 = @default;
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// On the stream to the configuration file being established.
    /// </summary>
    private void OnLoad(ByteBuffer stream) {
      if(stream == null) {
        State = AssetState.Broken;
        
        Log.Debug("Loading configuration failed '"+_path+"'.");
      } else {
        
        // parse the node from the stream
        Node = NodeSerialization.Parse(stream);
        State = AssetState.Loaded;
        
        stream.Flush();
        
        Log.Info("Loaded configuration '"+_path+"'.");
        
      }
      
      _onLoad.ArgA = this;
      _onLoad.Run();
      _lock.Release();
    }
    
    /// <summary>
    /// On the stream to the configuration file being established.
    /// </summary>
    private void OnSave(ByteBuffer stream) {
      if(stream == null) {
        
        State = AssetState.Broken;
        Log.Debug("Saving configuration failed '"+_path+"'.");
        
      } else {
        
        if(NodeSerialization.Serialize(Node, stream)) {
          Log.Info("Saved configuration '"+_path+"'.");
        } else {
          Log.Warning("Saving configuration at '"+_path+"' failed. The node serialization didn't work.");
        }
        
        State = AssetState.Loaded;
        
        stream.Flush();
        
      }
      _lock.Release();
    }
    
  }

}
