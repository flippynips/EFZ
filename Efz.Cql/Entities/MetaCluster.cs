/*
 * User: Joshua
 * Date: 27/09/2016
 * Time: 10:48 PM
 */
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

using Cassandra;

using Efz.Collections;
using Efz.Threading;

namespace Efz.Cql {
  
  /// <summary>
  /// Wrapper for a Cassandra cluster. Manages the associated Cluster instance and
  /// has references to contained Keyspaces.
  /// </summary>
  public class MetaCluster : IDisposable {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Get the cluster name.
    /// </summary>
    public string Name {
      get { return Cluster.Metadata.ClusterName; }
    }
    
    /// <summary>
    /// Optional data centers and associated replication for constructed keyspaces to use.
    /// </summary>
    public Dictionary<string,int> DataCenters;
    
    /// <summary>
    /// The default keyspace used by entities to access the database.
    /// </summary>
    internal Keyspace DefaultKeyspace {
      get {
        // if a default cluster has not been set
        if(_defaultKeyspace == null && ManagerCql.DefaultKeyspaceName != null) {
          Log.Info("Starting default keyspace with name '"+ManagerCql.DefaultKeyspaceName+"'.");
          new Keyspace(ManagerCql.DefaultKeyspaceName, this);
        }
        return _defaultKeyspace;
      }
      set {
        _defaultKeyspace = value;
      }
    }
    
    /// <summary>
    /// Collection of Keyspaces instantiated using this MetaCluster.
    /// </summary>
    internal Dictionary<string, Keyspace> Keyspaces;
    /// <summary>
    /// The entry points for the cluster.
    /// </summary>
    internal ArrayRig<IPEndPoint> EntryPoints;
    /// <summary>
    /// Cassandra.Cluster reference this instance manages.
    /// </summary>
    internal Cluster Cluster;
    
    //-------------------------------------------//
    
    /// <summary>
    /// The inner reference to the current default keyspace.
    /// </summary>
    private Keyspace _defaultKeyspace;
    /// <summary>
    /// Lock for external access.
    /// </summary>
    private readonly Lock _lock;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new Cluster instance with the specified contact point addresses.
    /// A Cassandra.Cluster will be initialized with a default configuration.
    /// </summary>
    internal MetaCluster(ArrayRig<string> entryPointAddresses, int port = 0, string username = null, string password = null) {
      
      _lock = new Lock();
      Keyspaces = new Dictionary<string, Keyspace>();

      // initialize a collection of end points
      EntryPoints = new ArrayRig<IPEndPoint>(entryPointAddresses.Count);
      foreach (var entryPointString in entryPointAddresses) {
        IPAddress address;
        IPEndPoint endpoint;
        if (entryPointString.TryParseEndPoint(out endpoint)) {
          EntryPoints.Add(endpoint);
        } else if (port == 0) {
          Log.Warning("An ip address could not be derived : '" + entryPointString + "'.");
        } else if (IPAddress.TryParse(entryPointString, out address)) {
          endpoint = new IPEndPoint(address, port);
          EntryPoints.Add(endpoint);
        }
      }
      
      if(EntryPoints.Count == 0) {
        Log.Warning("A cluster wasn't initialized. There were zero valid entry points.");
        return;
      }
      
      // get the existing type definitions
      var typeDefinitions = new Cassandra.Serialization.TypeSerializerDefinitions();
      foreach(Type type in Generic.GetTypes<Cassandra.Serialization.TypeSerializer>(type => !type.IsAbstract && !type.IsInterface)) {
        // call 'Define' for all type definitions
        var methodInfo = typeDefinitions.GetType().GetMethod("Define", BindingFlags.Public);
        var types = type.GetGenericArguments();
        if(types.Length != 0) {
          methodInfo = methodInfo.MakeGenericMethod(types[0]);
          methodInfo.Invoke(typeDefinitions, new [] { Activator.CreateInstance(type, false) });
        }
      }
      
      // initialize a cluster connection
      Builder builder = Cluster.Builder()
        .WithTypeSerializers(typeDefinitions)
        .WithLoadBalancingPolicy(new LoadBalancer(this))
        .WithCompression(CompressionType.Snappy)
        .AddContactPoints(EntryPoints.ToArray());
      
      if(!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password)) {
        builder = builder.WithCredentials(username, password);
      }
      
      Cluster = builder.Build();
      
    }
    
    /// <summary>
    /// Initialize a new Pool and Cluster instance with the specified contact points.
    /// The cluster will be initialized with a default configuration.
    /// </summary>
    internal MetaCluster(ArrayRig<IPEndPoint> entryPoints, string username = null, string password = null) {
      
      _lock = new Lock();
      Keyspaces = new Dictionary<string, Keyspace>();

      // reference the entry points
      EntryPoints = entryPoints;
      
      // get the existing type definitions
      var typeDefinitions = new Cassandra.Serialization.TypeSerializerDefinitions();
      foreach(Type type in Generic.GetTypes<Cassandra.Serialization.TypeSerializer>()) {
        
        // if the type is in the Cassandra assembly - skip
        if(type.FullName.StartsWith("Cassandra.Serialization.", StringComparison.Ordinal)) continue;
        
        // call 'Define' for all type definitions
        var methodInfo = typeDefinitions.GetType().GetMethod("Define", BindingFlags.Instance | BindingFlags.Public);
        Type baseType = type.BaseType;
        while(!baseType.IsGenericType) { baseType = baseType.BaseType; }
        methodInfo = methodInfo.MakeGenericMethod(baseType.GetGenericArguments()[0]);
        
        methodInfo.Invoke(typeDefinitions, new [] {
          type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, CallingConventions.Any, new Type[0], null).Invoke(null)
        });
        
      }
      
      // initialize a cluster connection
      Builder builder = Cluster.Builder()
        .WithTypeSerializers(typeDefinitions)
        .WithLoadBalancingPolicy(new LoadBalancer(this))
        .WithCompression(CompressionType.Snappy)
        .AddContactPoints(entryPoints.ToArray());
      
      if(!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password)) {
        builder = builder.WithCredentials(username, password);
      }
      
      Cluster = builder.Build();
      
      if(Cluster == null) Log.Warning("The cluster builder failed in MetaCluster.");
      
    }
    
    /// <summary>
    /// Initialize a new Pool instance with the specified Cassandra Cluster.
    /// </summary>
    internal MetaCluster(Cluster cluster) {
      
      _lock = new Lock();
      Keyspaces = new Dictionary<string, Keyspace>();

      // initialize a cluster connection
      Cluster = cluster;
      
    }
    
    /// <summary>
    /// Dispose of the data cluster and all related resources.
    /// </summary>
    public void Dispose() {
      
      ArrayRig<Keyspace> keyspaces = new ArrayRig<Keyspace>();
      // iterate and dispose of Keyspaces
      foreach(var keyspace in Keyspaces) keyspaces.Add(keyspace.Value);
      foreach(var keyspace in keyspaces) keyspace.Dispose();
      
      _lock.Take();
      
      Keyspaces.Clear();
      Keyspaces = null;
      
      _lock.Release();
      
      // remove the cluster from the manager
      ManagerCql.RemoveCluster(this);
      
      // dispose of the cassandra cluster
      Cluster.Shutdown();
      Cluster.Dispose();
      
    }
    
    /// <summary>
    /// Get a Keyspace by name. An associated Cassandra.Keyspace will be created if
    /// it doesn't yet exist.
    /// </summary>
    public Keyspace GetKeyspace(string name) {
      
      Keyspace keyspace;
      
      _lock.Take();
      
      // iterate initialized references
      if(Keyspaces.TryGetValue(name, out keyspace)) {
        _lock.Release();
        return keyspace;
      }
      
      _lock.Release();
      
      // return the new Keyspace
      return new Keyspace(name, this);
    }
    
    /// <summary>
    /// Remove a Keyspace from this DataCluster.
    /// </summary>
    internal void RemoveKeyspace(Keyspace keyspace) {
      _lock.Take();
      
      // if the keyspace removed is the default keyspace
      Keyspaces.Remove(keyspace.Metadata.Name);
      if(_defaultKeyspace == keyspace) {
        if(Keyspaces.Count == 0) _defaultKeyspace = null;
        else {
          foreach(var entry in Keyspaces) {
            _defaultKeyspace = entry.Value;
            break;
          }
        }
      }
      _lock.Release();
    }
    
    /// <summary>
    /// Add a Keyspace to this DataCluster.
    /// </summary>
    internal void AddKeyspace(Keyspace keyspace) {
      _lock.Take();
      // if the default keyspace hasn't been set
      if(_defaultKeyspace == null) _defaultKeyspace = keyspace;
      Keyspaces.Add(keyspace.Metadata.Name, keyspace);
      _lock.Release();
    }
    
    //-------------------------------------------//
    
  }
  
}
