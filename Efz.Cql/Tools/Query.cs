/*
 * User: Joshua
 * Date: 29/09/2016
 * Time: 9:42 PM
 */
using System;

using Cassandra;

using Efz.Collections;
using Efz.Cql;
using Efz.Cql.Entities;

namespace Efz.Cql {
  
  /// <summary>
  /// The components required to construct a cql command.
  /// </summary>
  internal class Query : IQuery {
    
    //----------------------------------//
    
    /// <summary>
    /// The cql keywords in the query.
    /// </summary>
    internal readonly ArrayRig<Cql> Keywords;
    /// <summary>
    /// Current collection of values being added to.
    /// </summary>
    internal readonly ArrayRig<Column> Columns;
    /// <summary>
    /// Object values that are to be bound to the statement.
    /// </summary>
    internal readonly ArrayRig<object> Values;
    /// <summary>
    /// The value count of each set in the query.
    /// </summary>
    internal readonly ArrayRig<int> Sets;
    
    /// <summary>
    /// The table this query is to be executed upon.
    /// </summary>
    internal readonly Table Table;
    /// <summary>
    /// The keyspace that the builder uses for the query.
    /// </summary>
    internal Keyspace Keyspace;
    
    /// <summary>
    /// Limit to apply to SELECT queries.
    /// </summary>
    internal int Limit;
    
    //----------------------------------//
    
    /// <summary>
    /// The current set count.
    /// </summary>
    protected int _currentSetCount;
    /// <summary>
    /// The page size for iteration through the results.
    /// </summary>
    protected int _pageSize;
    /// <summary>
    /// Type of cql query.
    /// </summary>
    protected Cql _type;
    
    //----------------------------------//
    
    /// <summary>
    /// Start building a new command. Optionally with the specified callback for async pagination.
    /// </summary>
    public Query(Table table, int pageSize = -1) {
      Table = table;
      Keyspace = table.Keyspace;
      _pageSize = pageSize == -1 ? Table.DefaultPageSize : pageSize;
      
      // initialize the collections of value sets
      Keywords = new ArrayRig<Cql>(3);
      Sets = new ArrayRig<int>(3);
      Values = new ArrayRig<object>(Table.Columns.Count);
      Columns = new ArrayRig<Column>(Table.Columns.Count);
    }
    
    /// <summary>
    /// Initialize a new query for the specified table and keyspace.
    /// </summary>
    public Query(Table table, Keyspace keyspace, int pageSize = -1) {
      Table = table;
      Keyspace = keyspace;
      _pageSize = pageSize == -1 ? Table.DefaultPageSize : pageSize;
      
      // initialize the collections of value sets
      Keywords = new ArrayRig<Cql>(3);
      Sets = new ArrayRig<int>(3);
      Values = new ArrayRig<object>(Table.Columns.Count);
      Columns = new ArrayRig<Column>(Table.Columns.Count);
    }
    
    /// <summary>
    /// Add a cql keyword to the query.
    /// </summary>
    public void Add(Cql cql) {
      if(Keywords.Count == 0) _type = cql;
      Keywords.Add(cql);
      // if the last set of values is not empty
      if(_currentSetCount != 0) {
        // add the last set count
        Sets.Add(_currentSetCount);
        _currentSetCount = 0;
      }
    }
    
    /// <summary>
    /// Add a column definition to the query.
    /// </summary>
    public void Add(Column column) {
      Columns.Add(column);
      ++_currentSetCount;
    }
    
    /// <summary>
    /// Add a set of columns to the query.
    /// </summary>
    public void Add(params Column[] columns) {
      // add the columns the builder was initialized with if any
      Columns.AddItems(columns);
      _currentSetCount += columns.Length;
    }
    
    /// <summary>
    /// Add a string literal to the query.
    /// </summary>
    public void Add(object value) {
      Values.Add(value);
      ++_currentSetCount;
    }
    
    /// <summary>
    /// Add a string literals to the query.
    /// </summary>
    public void Add(params object[] values) {
      Values.AddItems(values);
      _currentSetCount += values.Length;
    }
    
    /// <summary>
    /// Execute the query synchronously with no return value.
    /// </summary>
    public void Execute() {
      // construct and bind values to the query statement
      Statement statement = Bind();
      // execute the statement
      Keyspace.Execute(_type, statement);
    }
    
    /// <summary>
    /// Execute the query synchronously with no return value.
    /// </summary>
    public void ExecuteNoBatch() {
      // construct and bind values to the query statement
      Statement statement = Bind();
      // execute the statement
      Keyspace.ExecuteNoBatch(statement);
    }
    
    /// <summary>
    /// Execute the quest asyncronously with no return value.
    /// </summary>
    public void ExecuteAsync() {
      // construct and bind values to the query statement
      Statement statement = Bind();
      // execute the statement asynchronously
      Keyspace.ExecuteAsync(_type, statement);
    }
    
    /// <summary>
    /// Execute the query asynchronously with no return value.
    /// </summary>
    public void ExecuteAsync(IAction onExecuted) {
      // construct and bind values to the query statement
      Statement statement = Bind();
      // execute the statement
      Keyspace.ExecuteAsync(_type, statement, onExecuted);
    }
    
    /// <summary>
    /// Execute the statement without utilizing any batch capabilities.
    /// </summary>
    public void ExecuteAsyncNoBatch() {
      // construct and bind values to the query statement
      Statement statement = Bind();
      // execute the statement
      Keyspace.ExecuteAsyncNoBatch(statement);
    }
    
    /// <summary>
    /// Execute the query and return whether the query was successful or not with
    /// the criteria for success depending on the type of query.
    /// </summary>
    public bool ExecuteBoolean() {
      // switch the first cql keyword
      switch(_type) {
        case Cql.Insert:
          // execute the bound statement
          Statement statement = Bind();
          foreach(IRow row in Keyspace.ExecuteResults(_type, statement)) {
            object[] values = new object[1];
            row.GetValues(values);
            return (bool)values[0];
          }
          return false;
        default:
          Log.Debug("Unexpected return type and keyword " + _type);
          return false;
      }
    }
    
    /// <summary>
    /// Execute the query and get an enumerator for the results asynchonously.
    /// </summary>
    public void ExecuteEnumerator<TRow>(IAction<RowEnumerator<TRow>> onPage) where TRow : IRow, new() {
      // execute the bound statement
      Statement statement = Bind();
      // execute the query and assign the callback argument
      onPage.ArgA = new RowEnumerator<TRow>(Table, Keyspace.ExecuteResults(_type, statement));
      // run the callback
      onPage.Run();
    }
    
    /// <summary>
    /// Execute the query and get an enumerator for the results.
    /// </summary>
    public RowEnumerator<TRow> ExecuteEnumerator<TRow>() where TRow : IRow, new() {
      // execute the bound statement
      Statement statement = Bind();
      
      // construct an enumerator with the results
      return new RowEnumerator<TRow>(Table, Keyspace.ExecuteResults(_type, statement));
    }
    
    /// <summary>
    /// Execute the query with no return value.
    /// </summary>
    public Statement Bind() {
      // ensure there is no remaining set count
      if(_currentSetCount != 0) {
        Sets.Add(_currentSetCount);
        _currentSetCount = 0;
      }
      
      Statement statement;
      // if there are no values to bind
      if(Values.Count == 0) {
        // construct the query string
        QueryBuilder builder = new QueryBuilder(this);
        
        // create the simple statement
        statement = new SimpleStatement(builder.QueryString, new object[0]);
      } else {
        
        // get an identifier for the query and value collections counts
        int id = 0;
        int index = 1;
        unchecked {
          id = Table.Name.GetHashCode();
          // iterate keywords
          foreach(Cql cql in Keywords) {
            // add each keywords integer value
            id += (int)cql * index + 2000;
            ++index;
          }
          // iterate and add set counts
          foreach(int count in Sets) {
            ++index;
            id += count * index + 50000;
          }
        }
        
        PreparedStatement prepared;
        
        if(Keyspace == null) {
          Log.Warning("Keyspace for table '"+Table.Name+"' hasn't been created.");
        }
        
        // if the statement could not be found in the cache
        if(!Keyspace.Statements.TakeItem().TryGetValue(id, out prepared)) {
          Keyspace.Statements.Release();
          // construct the query string
          QueryBuilder builder = new QueryBuilder(this);
          
          // create a new prepared statement
          prepared = Keyspace.Session.Prepare(builder.QueryString);
          
          // add the prepared statement to the keyspace
          Keyspace.Statements.TakeItem()[id] = prepared;
        }
        Keyspace.Statements.Release();
        
        // bind query objects to the statement
        statement = prepared.Bind(Values.ToArray());
      }
      
      statement.SetPageSize(_pageSize);
      return statement;
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Get the type of command that is represented by a string.
    /// </summary>
    private static Cql DeriveType(string str) {
      // derive the command type
      switch(str[0]) {
        case Chars.A:
          return Cql.Alter;
        case Chars.B:
          return Cql.Batch;
        case Chars.C:
          return Cql.Create;
        case Chars.D:
          switch(str[1]) {
            case Chars.E:
              return Cql.Delete;
            case Chars.R:
              return Cql.Drop;
          }
          break;
        case Chars.G:
          return Cql.Grant;
        case Chars.I:
          return Cql.Insert;
        case Chars.L:
          return Cql.List;
        case Chars.R:
          return Cql.Revoke;
        case Chars.S:
          return Cql.Select;
        case Chars.T:
          return Cql.Truncate;
        case Chars.U:
          switch(str[1]) {
            case Chars.P:
              return Cql.Update;
            case Chars.S:
              return Cql.Use;
          }
          break;
      }
      // the command type was not recognised
      return Cql.Unknown;
    }
    
  }
  
}
