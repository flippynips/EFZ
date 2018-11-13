using System;
using System.Collections.Generic;

using Efz.Collections;
using Efz.Data;
using Efz.Network;
using Efz.Threading;
using Efz.Tools;

namespace Efz {
  
  /// <summary>
  /// Factory for API instances.
  /// </summary>
  public class ManagerConnections : Singleton<ManagerConnections> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// The connection manager will in most cases be initialized second.
    /// </summary>
    protected override byte SingletonPriority { get { return 245; } }
    
    /// <summary>
    /// The default port this application will listen and broadcast to.
    /// </summary>
    public static int Port { get; protected set; }
    /// <summary>
    /// Default ip address for any server interface.
    /// </summary>
    public static byte[] IpAddress { get; protected set; }
    
    //-------------------------------------------//
    
    /// <summary>
    /// All current connections.
    /// </summary>
    private static Dictionary<Type, ArrayRig<IConnection>> _sources;
    /// <summary>
    /// The shared lock for this managers methods.
    /// </summary>
    private static LockShared _lock;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Manager of connections to external resources.
    /// </summary>
    public ManagerConnections() {
      _sources = new Dictionary<Type, ArrayRig<IConnection>>();
      _lock = new LockShared();
    }
    
    /// <summary>
    /// Get a supported class from the connection. Deferred loading with callback on get class.
    /// </summary>
    public static void Get<T, C>(string path, IAction<T> onResource, Needle needle = null) where C : Connection<C>, IGetValue<T>, new() {
      Get<T, C>(path, onResource, needle, true);
    }
    private static void Get<T, C>(string path, IAction<T> onResource, Needle needle, bool tryLock) where C : Connection<C>, IGetValue<T>, new() {
      // get the lock
      if(tryLock && !(tryLock = _lock.TryTake)) {
        _lock.TryLock(new ActionSet<string, IAction<T>, Needle, bool>(Get<T, C>, path, onResource, needle, false));
        return;
      }
      
      // get the type of connection
      Type sourceType = typeof(C);
      ArrayRig<IConnection> source;
      
      // check if there are currently any types of connections like this
      if(!_sources.TryGetValue(sourceType, out source)) {
        // create the dictionary
        source = new ArrayRig<IConnection>();
        _sources[sourceType] = source;
      }
      
      // try get the connection
      C connection = source.GetSingle(c => c.Path == path) as C;
      
      // if the connection doesn't exist
      if(connection == null) {
        
        // initialize a new connection
        connection = new C();
        connection.Path = path;
        // add to connections
        source.Add(connection);
        
      }
      
      // unlock
      if(tryLock) _lock.Release();
      
      // get the resource
      connection.Get(onResource);
    }
    
    /// <summary>
    /// Get a supported class from the connection.
    /// </summary>
    public static void Get<T, C>(C connection, Act<T> onResource) where C : Connection<C>, IGetValue<T>, new() {
      Get<T, C>(connection, onResource, true);
    }
    private static void Get<T, C>(C connection, Act<T> onResource, bool tryLock) where C : Connection<C>, IGetValue<T>, new() {
      // get the lock
      if(tryLock && !(tryLock = _lock.TryTake)) {
        _lock.TryLock(new Act<C, Act<T>, bool>(Get<T,C>, connection, onResource, false));
        return;
      }
      
      Type sourceType = typeof(C);
      ArrayRig<IConnection> source;
      // check if there are currently any types of connections like this
      if(!_sources.TryGetValue(sourceType, out source)) {
        // create the source dictionary and add it
        _sources[sourceType] = source = new ArrayRig<IConnection>();
      }
      // does the connection exist in source?
      if(!source.Contains(connection)) {
        // add to the connections
        source.Add(connection);
      }
      
      // if manually locked, unlock
      if(tryLock) _lock.Release();
      
      // get the resource
      connection.Get(onResource);
    }
    
    /// <summary>
    /// Get a supported class from the connection. Resource is unlocked after the action called is ended.
    /// </summary>
    public static void Get<T, C>(string path, out Teple<LockShared, T> resource) where C : Connection<C>, IGetValue<T>, new() {
      _lock.Take();
      
      // get the type of connection
      Type sourceType = typeof(C);
      ArrayRig<IConnection> source;
      
      // check if there are currently any types of connections like this
      if(!_sources.TryGetValue(sourceType, out source)) {
        // create the dictionary
        source = new ArrayRig<IConnection>();
        _sources[sourceType] = source;
      }
      
      // try get the connection
      C connection = source.GetSingle(c => c.Path == path) as C;
      
      // if the connection doesn't exist
      if(connection == null) {
        
        // create a new connection
        connection = new C();
        connection.Path = path;
        // add to the source
        source.Add(connection);
        
      }
      
      // unlock the sources lock
      _lock.Release();
      
      // get the resource
      connection.Get(out resource);
    }
    
    /// <summary>
    /// Add or Get a connection synchronously to the connections known by the Api Manager.
    /// This doesn't open the connection.
    /// </summary>
    public static C Get<C>(string path) where C : Connection<C>, new() {
      // get the lock
      _lock.Take();
      
      Type sourceType = typeof(C);
      ArrayRig<IConnection> connections;
      if(!_sources.TryGetValue(sourceType, out connections)) {
        _sources[sourceType] = connections = new ArrayRig<IConnection>();
      }
      
      // try get the connection
      C connection = connections.GetSingle(c => c.Path == path) as C;
      if(connection == null) {
        
        // connection doesn't exist, create it
        connection = new C();
        connection.Path = path;
        _sources[sourceType].Add(connection);
        
      }
      
      // if manually locked, unlock
      _lock.Release();
      
      return connection;
    }
    
    /// <summary>
    /// Add or Get a connection to the connections known by the Api Manager.
    /// This opens the connection before the callback.
    /// </summary>
    public static void Get<C>(string path, Act<C> onOpen) where C : Connection<C>, new() {
      Get<C>(path, onOpen, true);
    }
    private static void Get<C>(string path, Act<C> onOpen, bool tryLock) where C : Connection<C>, new() {
      // get the lock
      if(tryLock && !(tryLock = _lock.TryTake)) {
        _lock.TryLock(new Act<string, Act<C>, bool>(Get<C>, path, onOpen, false));
        return;
      }
      
      Type sourceType = typeof(C);
      ArrayRig<IConnection> connections;
      if(!_sources.TryGetValue(sourceType, out connections)) {
        _sources[sourceType] = connections = new ArrayRig<IConnection>();
      }
      
      // try get the connection
      C connection = connections.GetSingle(c => c.Path == path) as C;
      if(connection == null) {
        
        // connection doesn't exist, create it
        connection = onOpen.ArgA = new C();
        connection.Path = path;
        _sources[sourceType].Add(connection);
        connection.Open(onOpen);
        
      } else {
        onOpen.ArgA = connection;
        if(connection.State.Is(ConnectionState.Closed)) connection.Open(onOpen);
        else onOpen.Run();
      }
      
      // if manually locked, unlock
      if(tryLock) _lock.Release();
    }
    
    /// <summary>
    /// Remove and close a connection by path from the connections known by the manager.
    /// </summary>
    public static void Remove<C>(string path, bool now = true) where C : Connection<C>, new() {
      if(now) {
        _lock.Take();
        RemoveInner<C>(path, false);
        _lock.Release();
      } else {
        RemoveInner<C>(path, true);
      }
    }
    private static void RemoveInner<C>(string path, bool tryLock) where C : Connection<C>, new() {
      // get the lock
      if(tryLock && !(tryLock = _lock.TryTake)) {
        _lock.TryLock(new Act<string, bool>(Remove<C>, path, false));
        return;
      }
      
      Type sourceType = typeof(C);
      ArrayRig<IConnection> connections;
      if(!_sources.TryGetValue(sourceType, out connections)) {
        _sources[sourceType] = connections = new ArrayRig<IConnection>();
      }
      
      // try remove and close the connection
      if(!connections.RemoveSingle(c => {
          if(c.Path == path) {
            c.Close();
            return true;
          }
          return false;
        })) {
        
        Log.Debug("Connection to be removed was not found : " + path);
        
      }
      
      // if manually locked, unlock
      if(tryLock) _lock.Release();
      
    }
    
    /// <summary>
    /// Remove a connection from the connections known by the manager.
    /// </summary>
    public static void Remove(IConnection connection) {
      Remove(connection, true);
    }
    private static void Remove(IConnection connection, bool tryLock) {
      // get the lock
      if(tryLock && !(tryLock = _lock.TryTake)) {
        _lock.TryLock(new Act<IConnection, bool>(Remove, connection, false));
        return;
      }
      
      ArrayRig<IConnection> connections;
      if(!_sources.TryGetValue(connection.GetType(), out connections)) {
        // no connections of the specified connection type are known
        Log.Debug("Connection to be removed was not found : " + connection.Path);
        
        return;
      }
      
      // remove the connection
      connections.RemoveQuick(connection);
      
      // if manually locked, unlock
      if(tryLock) _lock.Release();
    }
    
    /// <summary>
    /// Add a connection to the collection of connections known by the manager.
    /// </summary>
    public static void Add(IConnection connection) {
      Add(connection, true);
    }
    private static void Add(IConnection connection, bool tryLock) {
      // get the lock
      if(tryLock && !(tryLock = _lock.TryTake)) {
        _lock.TryLock(new Act<IConnection, bool>(Add, connection, false));
        return;
      }
      
      // get the list of connections of that type
      Type sourceType = connection.GetType();
      ArrayRig<IConnection> connections;
      if(!_sources.TryGetValue(sourceType, out connections)) {
        _sources[sourceType] = connections = new ArrayRig<IConnection>();
      }
      
      // add the connection
      _sources[sourceType].Add(connection);
      
      // if manually locked, unlock
      if(tryLock) _lock.Release();
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Set default parameters.
    /// </summary>
    protected override void Setup(Node configuration) {
      
      // get a port from the configuration
      Port = configuration.Default(63906, "Port");
      
    }
    
    /// <summary>
    /// Dispose of all connections.
    /// </summary>
    protected override void End(Node node) {
      
      _lock.Take();
      foreach(ArrayRig<IConnection> source in _sources.Values) {
        foreach(IConnection connection in source) {
          connection.Close();
        }
        source.Dispose();
      }
      _sources.Clear();
      _lock.Release();
      
      // create configuration node
      node["Port"].Object = Port;
      
    }
    
  }

}
