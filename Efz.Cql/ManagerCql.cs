using System;
using System.Net;

using Efz;
using Efz.Data;
using Efz.Collections;

namespace Efz.Cql {
  
  /// <summary>
  /// Data storing and access.
  /// </summary>
  public class ManagerCql : Singleton<ManagerCql> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Medium to high priority.
    /// </summary>
    protected override byte SingletonPriority { get { return 200; } }
    
    /// <summary>
    /// Process handle for the cassandra process.
    /// </summary>
    public static ProcessHandle CassandraProcess;
    
    /// <summary>
    /// The default cluster used by entities.
    /// </summary>
    internal static MetaCluster DefaultCluster {
      get {
        // if a default cluster has not been set
        if(_defaultCluster == null) {
          _defaultCluster = new MetaCluster(new ArrayRig<IPEndPoint>(new [] {
            new IPEndPoint(new IPAddress(new byte[] {127, 0, 0, 1}), 11102)
          }));
        }
        return _defaultCluster;
      }
      set {
        _defaultCluster = value;
      }
    }
    
    /// <summary>
    /// Active data clusters.
    /// </summary>
    internal static ArrayRig<MetaCluster> Clusters;
    /// <summary>
    /// Default keyspace name for all instantiated clusters without keyspaces.
    /// </summary>
    internal static string DefaultKeyspaceName;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Inner default cluster.
    /// </summary>
    private static MetaCluster _defaultCluster;
    
    /// <summary>
    /// Process id for the cassandra process.
    /// </summary>
    private static int _cassandraPid;
    
    //-------------------------------------------//
    
    static ManagerCql() {
      // initialize collections
      Clusters = new ArrayRig<MetaCluster>();
    }
    
    /// <summary>
    /// Add a cluster to the current cluster connections.
    /// </summary>
    public static MetaCluster AddCluster(ArrayRig<string> entryPointAddresses, int port = 0, string username = null, string password = null) {
      
      var cluster = new MetaCluster(entryPointAddresses, port, username, password);
      
      // if the default cluster has not been set assign the new cluster
      if(_defaultCluster == null) DefaultCluster = cluster;
      
      Clusters.Add(cluster);
      
      return cluster;
    }
    
    /// <summary>
    /// Add a cluster to the current cluster connections.
    /// </summary>
    public static MetaCluster AddCluster(ArrayRig<IPEndPoint> entryPoints, string username = null, string password = null) {
      
      var cluster = new MetaCluster(entryPoints, username, password);
      
      // if the default cluster has not been set assign the new cluster
      if(_defaultCluster == null) DefaultCluster = cluster;
      
      Clusters.Add(cluster);
      
      return cluster;
      
    }
    
    /// <summary>
    /// Add a cluster to the current cluster connections.
    /// </summary>
    public static MetaCluster AddCluster(Cassandra.Cluster cluster) {
      
      var metaCluster = new MetaCluster(cluster);
      
      // if the default cluster has not been set assign the new cluster
      if(_defaultCluster == null) DefaultCluster = metaCluster;
      
      Clusters.Add(metaCluster);
      
      return metaCluster;
      
    }
    
    /// <summary>
    /// Remove a data cluster from the active collections.
    /// </summary>
    internal static void RemoveCluster(MetaCluster cluster) {
      // if the cluster being removed is the default cluster
      if(cluster == _defaultCluster) {
        if(Clusters.Count == 0) _defaultCluster = null;
        else DefaultCluster = Clusters[0];
      }
      Clusters.RemoveQuick(cluster);
    }
    
    /// <summary>
    /// Dispose of the cluster connections.
    /// </summary>
    public override void Dispose() {
      // shutdown and dispose of any remaining clusters
      for(int i = Clusters.Count-1; i >= 0; --i) Clusters.Array[i].Dispose();
      base.Dispose();
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// On start of the cql manager.
    /// </summary>
    protected void OnStart(string cassandraPath, int cassandraPid) {
      
      if(_cassandraPid != 0) {
        CassandraProcess = ProcessHandle.Get(cassandraPid);
        if(CassandraProcess == null) cassandraPid = 0;
        else _cassandraPid = cassandraPid;
      }
      
      // has the cassandra path been assigned?
      if(CassandraProcess == null &&
        cassandraPath != null &&
        System.IO.File.Exists(cassandraPath)) {
        
        // yes, does the file exist?
        CassandraProcess = new ProcessHandle(cassandraPath, false, false);
        _cassandraPid = CassandraProcess.Process.Id;
      }
      
    }
    
    /// <summary>
    /// Initialize components of the data manager.
    /// </summary>
    protected override void Setup(Node configuration) {
      
      string cassandraPath = null;
      int cassandraPid = 0;
      
      if(configuration.Contains("CassandraPath")) {
        cassandraPath = configuration["CassandraPath"].String;
      }
      if(configuration.Contains("CassandraPID")) {
        cassandraPid = configuration["CassandraPID"].Int32;
      }
      
      ManagerUpdate.OnStart.Add("Cassandra Process Joined", ActionSet.New(OnStart, cassandraPath, cassandraPid));
      
      // initialize the default endpoints
      var defaultEndPoints = new ArrayRig<IPEndPoint>();
      
      // if the default MetaCluster config has been defined
      if(configuration.DictionarySet) {
        
        // get the configuration of the default cluster
        Node cluster = configuration["DefaultCluster"];
        if(cluster["EntryPoints"].ArraySet) {
          
          foreach(Node entryPoint in cluster["EntryPoints"].Array) {
            string[] components = entryPoint.String.Split(Chars.Colon);
            if(components.Length != 2) {
              Log.Warning("A configuration address was incorrect '" + entryPoint.String + "'.");
              continue;
            }
            
            IPAddress address;
            if(!IPAddress.TryParse(components[0], out address)) {
              Log.Warning("An ip address could not be parsed : '" + components[0] + "'.");
              continue;
            }
            int port;
            if(!int.TryParse(components[1], out port)) {
              Log.Warning("A port could not be parsed : '" + components[1] + "'.");
              continue;
            }
            
            // add an endpoint
            defaultEndPoints.Add(new IPEndPoint(address, port));
          }
          
          // initialize the default cluster
          AddCluster(defaultEndPoints, cluster["username"].String, cluster["password"].String);
          
        }
        
        // read the default keyspace name
        DefaultKeyspaceName = cluster.Default<string>("Efz", "DefaultKeyspace");
        
      } else {
        
        DefaultKeyspaceName = "Efz";
        
      }
      
    }
    
    /// <summary>
    /// Dispose of all connections and save the last configuration.
    /// </summary>
    protected override void End(Node configuration) {
      
      // set the cassandra PID if set
      if(_cassandraPid != 0) {
        configuration["CassandraPID"].Int32 = _cassandraPid;
      }
      
      // set the default cluster
      if(_defaultCluster != null) {
        // save the current configuration
        Node cluster = configuration["DefaultCluster"];
        Node entry = cluster["EntryPoints"];
        
        // iterate endpoints and add to the configuration
        foreach(IPEndPoint endpoint in DefaultCluster.EntryPoints) {
          entry.Add(new Node(entry, endpoint.Address.ToString() + Chars.Colon + endpoint.Port));
        }
        
        // if there are keyspaces set the default keyspace name
        if(_defaultCluster.Keyspaces.Count != 0) {
          foreach(var keyspace in _defaultCluster.Keyspaces) {
            cluster["DefaultKeyspace"].String = keyspace.Value.Metadata.Name;
            break;
          }
        }
      }
      
      #if DEBUG
      if(CassandraProcess != null) Log.Debug("Cassandra process still running.");
      #else
      if(CassandraProcess != null) CassandraProcess.Dispose();
      #endif
      
    }
    
  }

}
