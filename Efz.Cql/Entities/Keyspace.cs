/*
 * User: Joshua
 * Date: 24/09/2016
 * Time: 4:03 PM
 */
using System;
using System.Collections.Generic;
using System.Linq;

using Cassandra;
using Cassandra.Mapping;
using Efz.Collections;
using Efz.Threading;
using Efz.Tools;

namespace Efz.Cql {
  
  /// <summary>
  /// Represents a Cassandra Keyspace that contains data in the form of Tables.
  /// </summary>
  public class Keyspace : IDisposable {
    
    //----------------------------------//
    
    /// <summary>
    /// The time between each automatic update of changes to table row values.
    /// Set to '-1' to disable automatic updates.
    /// </summary>
    public long AutoUpdateTime {
      get {
        return _autoUpdateTime;
      }
      set {
        _autoUpdateTime = value;
        if(_autoUpdateTime < 1) {
          if(_autoUpdateWatch != null) {
            _autoUpdateWatch.Dispose();
            _autoUpdateWatch = null;
          }
        } else {
          if(_autoUpdateWatch == null) _autoUpdateWatch = new Watch(_autoUpdateTime, true, OnAutoUpdate, true);
          else _autoUpdateWatch.Time = value;
        }
      }
    }
    
    /// <summary>
    /// Time in milliseonds between batch executions.
    /// </summary>
    public long BatchInterval = 200;
    /// <summary>
    /// Timeout in milliseconds for batch statements.
    /// </summary>
    public int BatchTimeout = 1000;
    /// <summary>
    /// The targeted maximum number of statements per batch.
    /// </summary>
    public long BatchMaxStatements = 50;
    
    /// <summary>
    /// Whether the keyspace has access to the cluster.
    /// </summary>
    public bool Connected { get { return Metadata != null; } }
    
    /// <summary>
    /// POCO mapper for this Keyspace.
    /// </summary>
    internal readonly Mapper Mapper;
    /// <summary>
    /// Session reference for this Keyspace.
    /// </summary>
    internal ISession Session {
      get {
        // has the session been assigned?
        if(_session == null) {
          // create an inline timer to wait for keyspace initialization
          TimeInline timer = new TimeInline(4000, () => _session != null);
          
          // wait until timeout or keyspace metadata being populated
          while(timer.Wait) {}
          
          // was the metadata retrieved?
          if(!timer.Success) {
            // no, error log
            Log.Error("Keyspace session could not be openned.");
          }
        }
        return _session;
      }
    }
    /// <summary>
    /// Keyspace metadata for this instance.
    /// </summary>
    internal readonly KeyspaceMetadata Metadata;
    /// <summary>
    /// Common cluster reference.
    /// </summary>
    internal readonly MetaCluster MetaCluster;
    
    /// <summary>
    /// Prepared statements and associated ids generated using the integer value of the cql keywords
    /// and the number of values between each cql keyword. These statments can be bound with new values
    /// without being rewritten.
    /// </summary>
    internal readonly Shared<Dictionary<int, PreparedStatement>> Statements;
    /// <summary>
    /// Tables that have been accessed within this Keyspace.
    /// </summary>
    internal readonly Shared<Dictionary<string, Table>> Tables;
    /// <summary>
    /// Metadata for tables.
    /// </summary>
    internal readonly Shared<Dictionary<string, TableMetadata>> TableMetadata;
    
    //----------------------------------//
    
    /// <summary>
    /// Inner session.
    /// </summary>
    protected ISession _session;
    
    /// <summary>
    /// Inner time between each table update.
    /// </summary>
    protected long _autoUpdateTime = 0;
    
    /// <summary>
    /// Collection of statements to be executed in a batch.
    /// </summary>
    protected ArrayRig<Statement> _batch;
    /// <summary>
    /// Action callbacks run when the batch is executed.
    /// </summary>
    protected ArrayRig<IAction> _onExecuted;
    /// <summary>
    /// Callback with results.
    /// </summary>
    protected IAction<RowSet> _onExecutedResults;
    
    /// <summary>
    /// Automatic change update timer. If enabled, it ensures periodic updates of any changes made to
    /// table rows.
    /// </summary>
    protected Watch _autoUpdateWatch;
    
    /// <summary>
    /// Lock for the current batch statement.
    /// </summary>
    protected Lock _batchLock;
    /// <summary>
    /// Timer for execution of batch statements.
    /// </summary>
    protected Timer _batchTimer;
    /// <summary>
    /// Flag for if the current batch collection contains a statement
    /// which will return a row set.
    /// </summary>
    protected bool _batchReturnsResults;
    /// <summary>
    /// The paging size for results from the current batch. '0' uses the default paging size.
    /// </summary>
    protected int _batchPageSize;
    /// <summary>
    /// The row set returned from the batch execution.
    /// </summary>
    protected RowSet _batchRows;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a Keyspace by name.
    /// </summary>
    public Keyspace(string name) : this(name, null) {}
    
    /// <summary>
    /// Initialize a Keyspace by name in association with the specified cluster.
    /// </summary>
    internal Keyspace(string name, MetaCluster metaCluster) {
      if(metaCluster == null) MetaCluster = ManagerCql.DefaultCluster;
      else MetaCluster = metaCluster;
      
      // initialize cache collections
      Tables = new Shared<Dictionary<string, Table>>(new Dictionary<string, Table>());
      Statements = new Shared<Dictionary<int, PreparedStatement>>(new Dictionary<int, PreparedStatement>());
      TableMetadata = new Shared<Dictionary<string, TableMetadata>>(new Dictionary<string, TableMetadata>());
      // initialize the batch related fields
      _batch = new ArrayRig<Statement>();
      _batchLock = new Lock();
      
      // decrease the batch timeout if debugging
      #if DEBUG
      BatchTimeout = 10000;
      #endif
      
      // create the act used to execute batch statements
      Act executeBatch = new Act(() => ExecuteBatch());
      _batchTimer = new Timer(BatchInterval, executeBatch, false);
      _onExecuted = new ArrayRig<IAction>();
      
      // setup the auto update watch
      if(_autoUpdateTime > 0) _autoUpdateWatch = new Watch(_autoUpdateTime, true, OnAutoUpdate);
      
      try {
        // connect to the cluster
        _session = MetaCluster.Cluster.Connect();
      } catch (Exception ex) {
        // log the error
        Log.Error("Problem getting Keyspace '" + name + "' metadata from the Cluster.", ex);
        return;
      }
      
      // initialize a Mapper instance for use by this Keyspace
      Mapper = new Mapper(_session);
      
      // get the metadata associated with this Keyspace
      Metadata = MetaCluster.Cluster.Metadata.GetKeyspace(name);
      
      // if the Keyspace doesn't exist
      if(Metadata == null) {
        // create the Keyspace
        if(MetaCluster.DataCenters == null) {
          _session.CreateKeyspace(name,
            ReplicationStrategies.CreateSimpleStrategyReplicationProperty(1));
        } else {
          _session.CreateKeyspace(name,
            ReplicationStrategies.CreateNetworkTopologyStrategyReplicationProperty(MetaCluster.DataCenters));
        }
        
        // get the metadata
        Metadata = MetaCluster.Cluster.Metadata.GetKeyspace(name);
      }
      
      // add the Keyspace to the DataCluster
      MetaCluster.AddKeyspace(this);
      
      // make the target Keyspace of the Session to this Keyspace
      _session.ChangeKeyspace(Metadata.Name);
      
      TableMetadata.Take();
      foreach(var tableMetadata in Metadata.GetTablesMetadata()) {
        TableMetadata.Item.Add(tableMetadata.Name, tableMetadata);
      }
      TableMetadata.Release();
      
    }
    
    /// <summary>
    /// Dispose of this Keyspace instance and associated resources.
    /// </summary>
    public void Dispose() {
      
      ArrayRig<Table> tables = new ArrayRig<Table>();

      // copy the tables collection
      foreach(var entry in Tables.TakeItem()) tables.Add(entry.Value);
      Tables.Release();
      // dispose of the associated tables
      foreach(var table in tables) table.Dispose();

      // dispose of the ISession
      _session.Dispose();
      // remove this Keyspace from the DataCluster
      MetaCluster.RemoveKeyspace(this);
      
    }
    
    /// <summary>
    /// Remove the Keyspace from existance. This disposes of this instance.
    /// </summary>
    public void DeleteKeyspace() {
      // have the session destroy the Keyspace
      _session.DeleteKeyspace(Metadata.Name);
      // dispose of this instance to avoid errors
      Dispose();
    }
    
    /// <summary>
    /// Update any changes to active Tables in the keyspace.
    /// </summary>
    public void SubmitChanges() {
      foreach(Table table in Tables.TakeItem().Values) {
        table.SubmitChanges();
      }
      Tables.Release();
    }
    
    /// <summary>
    /// Execute the specified statement synchronously, returning the set of results.
    /// </summary>
    internal RowSet ExecuteResults(Cql type, Statement statement) {
      switch(type) {
        case Cql.Insert:
        case Cql.Update:
        case Cql.Delete:
          // if there are no pending queries that will not return results
          _batchLock.Take();
          if(_batchReturnsResults || _batch.Count == 0) {
            _batchLock.Release();
            return _session.Execute(statement);
          }
          
          _batchPageSize = statement.PageSize;
          
          // add this statement
          _batch.Add(statement);
          
          // execute the batch - releases the lock
          return ExecuteBatch(false);
        default:
          // execute the statement synchronously
          return _session.Execute(statement);
      }
    }
    
    /// <summary>
    /// Execute the specified statement asynchronously, returning the set results when executed.
    /// </summary>
    internal void ExecuteResultsAsync(Cql type, Statement statement, IAction<RowSet> onExecuted) {
      if(ManagerUpdate.Stopping) {
        ExecuteStatement(statement, onExecuted);
        return;
      }
      switch(type) {
        case Cql.Insert:
        case Cql.Update:
        case Cql.Delete:
          _batchLock.Take();
          // if the current batch statements will return results
          if(_batchReturnsResults) {
            _batchLock.Release();
            // execute the single statement asynchronously
            ManagerUpdate.Control.AddSingle(ExecuteStatement, statement, onExecuted);
          } else {
            _onExecutedResults = onExecuted;
            _batchReturnsResults = true;
            _batchPageSize = statement.PageSize;
            _batch.Add(statement);
            
            if(_batch.Count == BatchMaxStatements) {
              _batchTimer.Reset(BatchInterval);
              _batchLock.Release();
              _batchTimer.OnDone.Run();
            } else {
              if(!_batchTimer.Run) _batchTimer.Run = true;
              _batchLock.Release();
            }
          }
          break;
        default:
          // execute the statement asynchronously
          ManagerUpdate.Control.AddSingle(ExecuteStatement, statement, onExecuted);
          break;
      }
    }
    
    /// <summary>
    /// Execute the specified statement synchronously.
    /// </summary>
    internal void Execute(Cql type, Statement statement) {
      switch(type) {
        case Cql.Insert:
        case Cql.Update:
        case Cql.Delete:
          _batchLock.Take();
          _batch.Add(statement);
          ExecuteBatch(false);
          break;
        default:
          // execute the statement synchronously
          _session.Execute(statement);
          break;
      }
    }
    
    /// <summary>
    /// Execute the specified statement asynchronously.
    /// </summary>
    internal void ExecuteAsync(Cql type, Statement statement) {
      if(ManagerUpdate.Stopping) {
        Execute(type, statement);
        return;
      }
      switch(type) {
        case Cql.Insert:
        case Cql.Update:
        case Cql.Delete:
          // execute the statement asynchronously
          _batchLock.Take();
          _batch.Add(statement);
          
          if(_batch.Count == BatchMaxStatements) {
            _batchTimer.Reset(BatchInterval);
            ExecuteBatch(false);
          } else {
            if(!_batchTimer.Run) _batchTimer.Run = true;
            _batchLock.Release();
          }
          
          break;
        default:
          // execute the statement asynchronously
          ManagerUpdate.Control.AddSingle(ExecuteStatement, statement);
          break;
      }
    }
    
    /// <summary>
    /// Execute the specified statement asynchronously and run the callback
    /// action when done.
    /// </summary>
    internal void ExecuteAsync(Cql type, Statement statement, IAction onExecuted) {
      if(ManagerUpdate.Stopping) {
        Execute(type, statement);
        onExecuted.Run();
        return;
      }
      switch(type) {
        case Cql.Insert:
        case Cql.Update:
        case Cql.Delete:
          // execute the statement asynchronously
          _batchLock.Take();
          _batch.Add(statement);
          _onExecuted.Add(onExecuted);
          
          if(_batch.Count == BatchMaxStatements) {
            _batchTimer.Reset(BatchInterval);
            _batchLock.Release();
            _batchTimer.OnDone.Run();
          } else {
            if(!_batchTimer.Run) _batchTimer.Run = true;
            _batchLock.Release();
          }
          break;
        default:
          // execute the statement asynchronously
          ManagerUpdate.Control.AddSingle(ExecuteStatement, statement, onExecuted);
          break;
      }
    }
    
    /// <summary>
    /// Execute the specified statement synchronously.
    /// </summary>
    internal void ExecuteNoBatch(IStatement statement) {
      // execute the statement synchronously
      _session.Execute(statement);
    }
    
    /// <summary>
    /// Execute the specified statement asynchronously.
    /// </summary>
    internal void ExecuteAsyncNoBatch(Statement statement) {
      if(ManagerUpdate.Stopping) {
        ExecuteNoBatch(statement);
        return;
      }
      // execute the statement asynchronously
      ManagerUpdate.Control.AddSingle(ExecuteStatement, statement);
    }
    
    /// <summary>
    /// Execute the specified statement asynchronously and run the callback
    /// action when done.
    /// </summary>
    internal void ExecuteAsyncNoBatch(Statement statement, IAction onExecuted) {
      if(ManagerUpdate.Stopping) {
        ExecuteNoBatch(statement);
        onExecuted.Run();
        return;
      }
      // execute the statement asynchronously
      ManagerUpdate.Control.AddSingle(ExecuteStatement, statement, onExecuted);
    }
    
    /// <summary>
    /// Add a Table to the Keyspace instance.
    /// </summary>
    internal void AddTable(Table table) {
      Tables.TakeItem().Add(table.Name, table);
      Tables.Release();
    }
    
    /// <summary>
    /// Remove a table instance from the Keyspace.
    /// </summary>
    internal void RemoveTable(Table table) {
      Tables.TakeItem().Remove(table.Name);
      Tables.Release();
    }
    
    /// <summary>
    /// Get a string summary of the keyspace.
    /// </summary>
    public override string ToString() {
      if(Metadata == null) return "'Unconnected Keyspace'";
      return Metadata.Name;
    }
    
    //----------------------------------//
    
    /// <summary>
    /// On iteration of the automatic update timer.
    /// </summary>
    protected void OnAutoUpdate() {
      // iterate tables
      foreach(Table table in Tables.TakeItem().Values) {
        if(table.Updated) {
          ManagerUpdate.Control.AddSingle(table.SubmitChanges);
        }
      }
      Tables.Release();
    }
    
    /// <summary>
    /// Execute the specified statement, returning results with the callback action.
    /// </summary>
    protected void ExecuteStatement(Statement statement, IAction<RowSet> onExecuted) {
      onExecuted.ArgA = _session.Execute(statement);
      onExecuted.Run();
    }
    
    /// <summary>
    /// Execute the specified statement, with the callback action being run on complete.
    /// </summary>
    protected void ExecuteStatement(Statement statement, IAction onExecuted) {
      _session.Execute(statement);
      onExecuted.Run();
    }
    
    /// <summary>
    /// Execute the specified statement.
    /// </summary>
    protected void ExecuteStatement(Statement statement) {
      _session.Execute(statement);
    }
    
    /// <summary>
    /// Execute the current collection of statements.
    /// The batch lock should be taken when this is called and is released during execution.
    /// </summary>
    protected RowSet ExecuteBatch(bool takeLock = true) {
      // is the lock to be used? yeah, take it
      if(takeLock) _batchLock.Take();
      
      // reset the timer
      _batchTimer.Reset(BatchInterval);
      _batchTimer.Run = false;
      
      // create a new batch statement
      BatchStatement batchStatement = new BatchStatement();
      batchStatement.SetReadTimeoutMillis(BatchTimeout);
      
      // add all current statements
      for (int i = 0; i < _batch.Count; ++i) batchStatement.Add(_batch[i]);
      
      // set the batch page size
      batchStatement.SetPageSize(_batchPageSize);
      
      // reset the batch
      _batch.Reset();
      _batchReturnsResults = false;
      
      // copy the collection of callbacks
      ArrayRig<IAction> callbacks = new ArrayRig<IAction>(_onExecuted);
      _onExecuted.Clear();
      
      // release the batch lock
      _batchLock.Release();
      
      RowSet rowSet;
      try {
        // execute the batch statement
        rowSet = _session.Execute(batchStatement);
      } catch(Exception ex) {
  		  
        var builder = StringBuilderCache.Get();
        builder.Append("An exception occured executing a batch statement '");
        var obj = typeof(BatchStatement).GetProperty("Queries",
          System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.NonPublic)
          .GetValue(batchStatement);
        if(obj == null) {
          builder.Append("Unknown");
        } else {
          var statements = (List<Statement>)obj;
          bool first = true;
          foreach(var statement in statements) {
            if(statement.OutgoingPayload == null) continue;
            foreach(var query in statement.OutgoingPayload.Keys) {
              if(first) first = false;
              else builder.Append(", ");
              builder.Append(query);
            }
          }
        }
        
        builder.Append("'. ");
        Log.Error(builder.ToString(), ex);
        
        return null;
      }
      
      // run callback with results
      if(_onExecutedResults != null) {
        _onExecutedResults.ArgA = rowSet;
        _onExecutedResults.Run();
        _onExecutedResults = null;
      }
      
      // run callbacks
      foreach(IAction callback in callbacks) callback.Run();
      callbacks.Dispose();
      
      return rowSet;
    }
    
  }
  
}
