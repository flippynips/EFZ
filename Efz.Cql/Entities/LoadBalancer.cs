/*
 * User: Joshua
 * Date: 27/09/2016
 * Time: 11:09 PM
 */
using System;

using System.Collections.Generic;
using Cassandra;

namespace Efz.Cql {
  
  /// <summary>
  /// Implementation of a load balancing policy to aid in determining query order of hosts.
  /// </summary>
  public class LoadBalancer : ILoadBalancingPolicy {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// The meta cluster instance.
    /// </summary>
    private MetaCluster _metaCluster;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Construct a new instance of the load balancer.
    /// </summary>
    public LoadBalancer(MetaCluster metaCluster) {
      _metaCluster = metaCluster;
    }
    
    /// <summary>
    /// Initialize this load balancer for the specified ICluster.
    /// </summary>
    public void Initialize(ICluster cluster) {
      if(_metaCluster.Cluster != cluster) Log.Warning("Mismatch between initialized cluster and constructor cluster.");
    }
    
    /// <summary>
    /// Determine the distance category to the specified Host.
    /// </summary>
    public HostDistance Distance(Host host) {
      return HostDistance.Ignored;
    }
    
    /// <summary>
    /// Determine an optimal query order for the specified keyspace and IStatement.
    /// </summary>
    public IEnumerable<Host> NewQueryPlan(string keyspace, IStatement statement) {
      return _metaCluster.Cluster.AllHosts();
    }
    
    //-------------------------------------------//
    
  }
  
}
