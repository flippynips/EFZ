/*
 * User: Joshua
 * Date: 25/09/2016
 * Time: 10:25 PM
 */
using System;
using System.Reflection;

using Cassandra;

using Efz.Collections;
using Efz.Cql;
using Efz.Threading;
using Efz.Tools;

namespace Efz.Cql {
  
  /// <summary>
  /// Table entity interface which is required to be implemented
  /// any class representing a data type in a Table.
  /// Any columns should be defined as Column<T> properties with both get
  /// and set accessible to this class.
  /// </summary>
  public abstract class Table : IDisposable {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Name of the Table in the Keyspace it resides in.
    /// </summary>
    public virtual string Name { get; private set; }
    /// <summary>
    /// Get or set the keyspace this table is associated with.
    /// </summary>
    public Keyspace Keyspace {
      get {
        // if the keyspace isn't set - set the default
        if(_initialize) Initialize();
        return _keyspace;
      }
      set {
        // set Keyspace reference
        if(_keyspace != null) _keyspace.RemoveTable(this);
        
        if(value == null) Log.Warning("A Tables Keyspace was assigned to 'Null'.");
        
        _keyspace = value;
        _keyspace.AddTable(this);
      }
    }
    
    /// <summary>
    /// Does this table have updated rows?
    /// </summary>
    public virtual bool Updated { get { return _currentCollections.Count != 0; } }
    
    /// <summary>
    /// Automatically update any changes made to rows during iteration.
    /// Else, collections can be updated manually.
    /// </summary>
    public virtual bool UpdateRowChanges { get { return true; } }
    /// <summary>
    /// The default size of row set pages returned from Cassandra.
    /// This can be overridden per query. Default is '100'.
    /// </summary>
    public virtual int DefaultPageSize { get { return 100; } }
    
    /// <summary>
    /// Cassandra metadata for this table.
    /// </summary>
    internal TableMetadata Metadata;
    
    /// <summary>
    /// Internal collection of columns this Table contains.
    /// </summary>
    internal ArrayRig<Column> Columns;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Flag for the initialization of this Table including column population.
    /// </summary>
    private bool _initialize;
    /// <summary>
    /// Lock used to ensure single initialization and for editting
    /// row collections.
    /// </summary>
    private readonly Lock _lock;
    
    /// <summary>
    /// The keyspace of this table of which the Cassandra.ISession is used.
    /// </summary>
    private Keyspace _keyspace;
    /// <summary>
    /// The Keyspace name this Table will be initialized with.
    /// </summary>
    private readonly string _keyspaceName;
    
    /// <summary>
    /// Collections of Rows that have been updated.
    /// </summary>
    private ArrayRig<RowCollection> _currentCollections;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Construct a new Table instance.
    /// </summary>
    protected Table(string keyspaceName = null) {
      _keyspaceName = keyspaceName;
      _initialize = true;
      _lock = new Lock();
      
      _currentCollections = new ArrayRig<RowCollection>();
      
      // if the name wasn't set
      Name = Name == null ? this.GetType().Name.ToLowercase() : Name.ToLowercase();
      
      // populate the columns
      Columns = new ArrayRig<Column>();
      
      // iterate properties
      foreach(PropertyInfo info in this.GetType().GetProperties(
        BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
        
        if(info.PropertyType.BaseType == typeof(Column)) {
          
          // get the column attribute
          ColumnAttribute attribute = info.GetCustomAttribute<ColumnAttribute>(true);
          
          // if the attribute isn't found - assume default values
          if(attribute == null) {
            attribute = new ColumnAttribute(Column.ColumnClass.Data, info.Name);
          }
          
          // get the column value if it's been set
          Column column = (Column)info.GetValue(this);
          
          // if the column hasn't been assigned
          if(column == null) {
            // create a new instance of the column
            IFunc<ColumnAttribute, Column> activator = Dynamic.Constructor<IFunc<ColumnAttribute, Column>>(info.PropertyType);
            if(attribute.Name == null) attribute.Name = info.Name;
            activator.ArgA = attribute;
            column = activator.Run();
            info.SetValue(this, column);
          }
          
          // add the column to the collection
          Columns.Add(column);
        }
        
      }
      
      // iterate fields
      foreach(FieldInfo info in this.GetType().GetFields(
        BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
        
        if(info.FieldType.BaseType == typeof(Column)) {
          
          // get the column attribute
          ColumnAttribute attribute = info.GetCustomAttribute<ColumnAttribute>(true);
          
          // if the attribute isn't found - assume default values
          if(attribute == null) {
            attribute = new ColumnAttribute(Column.ColumnClass.Data, info.Name);
          }
          
          // get the column value if it's been set
          Column column = (Column)info.GetValue(this);
          
          // if the column hasn't been assigned
          if(column == null) {
            // create a new instance of the column
            IFunc<ColumnAttribute, Column> activator = Dynamic.Constructor<FuncSet<ColumnAttribute, Column>>(info.FieldType);
            if(attribute.Name == null) attribute.Name = info.Name;
            activator.ArgA = attribute;
            column = activator.Run();
            info.SetValue(this, column);
          }
          
          // add the column to the collection
          Columns.Add(column);
        }
        
      }
    }
    
    /// <summary>
    /// Dispose of this Table instance.
    /// </summary>
    public virtual void Dispose() {
      if(!_initialize) {
        _keyspace.RemoveTable(this);
        _keyspace = null;
        Columns.Dispose();
      }
    }
    
    /// <summary>
    /// Delete the table from the keyspace.
    /// </summary>
    public void DeleteTable() {
      _keyspace.RemoveTable(this);
      _keyspace.ExecuteNoBatch(new SimpleStatement("DROP TABLE " + Name.ToLowercase()));
      _initialize = true;
    }
    
    /// <summary>
    /// Add a row collection that could potentially be updated.
    /// </summary>
    internal void AddCollection(RowCollection collection) {
      _lock.Take();
      _currentCollections.Add(collection);
      _lock.Release();
    }
    
    /// <summary>
    /// Manually submit all unsubmitted changes to rows.
    /// </summary>
    public void SubmitChanges() {
      // ensure initialization
      if(_initialize) Initialize();
      
      // if no updated collections - skip
      if(_currentCollections.Count == 0) return;
      
      _lock.Take();
      
      // copy the updated collections
      ArrayRig<RowCollection> collections = _currentCollections;
      _currentCollections = new ArrayRig<RowCollection>(collections.Count);
      
      _lock.Release();
      
      // iterate all updated collections
      foreach(RowCollection collection in collections) {
        
        // run an update for the collection
        Update(collection, true);
        
      }
      
      collections.Dispose();
    }
    
    /// <summary>
    /// Update the specified row in association with the specified columns.
    /// </summary>
    public void Update<TRow>(TRow row, params Column[] columns) where TRow : IRow {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      
      object[] values = new object[columns.Length];
      bool[] updated = new bool[columns.Length];
      bool[] idColumns = new bool[columns.Length];
      
      // build an update query for the each row
      Query builder = new Query(this);
      builder.Add(Cql.Update);
      
      // get the current values and updated flags for the cells
      row.GetValues(values);
      row.GetUpdated(updated);
      
      // add the updated columns and values to the builder
      for(int i = values.Length-1; i >= 0; --i) {
        // update the identifier columns
        if(columns[i].IsIdentifier) idColumns[i] = true;
        if(updated[i]) {
          builder.Add(columns[i]);
          builder.Add(values[i]);
        }
      }
      
      // add the 'WHERE' portion of the statement
      builder.Add(Cql.Where);
      
      // add the identifying columns
      for(int i = values.Length-1; i >= 0; --i) {
        if(idColumns[i]) {
          builder.Add(columns[i]);
          builder.Add(values[i]);
        }
      }
      
      row.Updated = false;
      
      // execute the query
      builder.ExecuteAsync();
    }
    
    /// <summary>
    /// Submit the specified row collection with 'Update' statements.
    /// </summary>
    public void Update(RowCollection collection, bool onlyUpdated = false) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      
      object[] values = new object[collection.Columns.Count];
      
      // only update rows that have changed?
      if(onlyUpdated) {
        bool[] updated = new bool[collection.Columns.Count];
        
        // iterate the updated rows collection
        foreach(IRow row in collection.Rows) {
          if(row.Updated) {
            
            // build an update query for the each row
            Query builder = new Query(this);
            builder.Add(Cql.Update);
            
            // get the current values and updated flags for the cells
            row.GetValues(values);
            row.GetUpdated(updated);
            
            // add the updated columns and values to the builder
            for(int i = values.Length-1; i >= 0; --i) {
              if(updated[i]) {
                builder.Add(collection.Columns[i]);
                builder.Add(values[i]);
              }
            }
            
            // add the identifier columns
            for(int i = values.Length-1; i >= 0; --i) {
              if(collection.Columns[i].IsIdentifier) {
                // add the 'WHERE' command for the identifier
                builder.Add(Cql.Where);
                builder.Add(collection.Columns[i]);
                builder.Add(Cql.Equal);
                builder.Add(values[i]);
              }
            }
            
            row.Updated = false;
            
            // execute the query
            builder.ExecuteAsync();
          }
        }
      } else {
        // iterate the rows collection
        foreach(IRow row in collection.Rows) {
          
          // build an update query for the each row
          Query builder = new Query(this);
          builder.Add(Cql.Update);
          
          // get the current values and updated flags for the cells
          row.GetValues(values);
          
          // add the updated columns and values to the builder
          for(int i = values.Length-1; i >= 0; --i) {
            builder.Add(collection.Columns[i]);
            builder.Add(values[i]);
          }
          
          // add the 'WHERE' portion of the statement
          builder.Add(Cql.Where);
          
          // add the identifying columns
          for(int i = values.Length-1; i >= 0; --i) {
            if(collection.Columns[i].IsIdentifier) {
              builder.Add(collection.Columns[i]);
              builder.Add(values[i]);
            }
          }
          
          row.Updated = false;
          
          // execute the query
          builder.ExecuteAsync();
        }
      }
      
    }
    
    /// <summary>
    /// Start an update command for this table.
    /// </summary>
    public CqlUpdate<A> Update<A>(Column<A> colA) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlUpdate<A>(builder, colA);
    }
    
    /// <summary>
    /// Start an update command for this table.
    /// </summary>
    public CqlUpdate<A,B> Update<A,B>(Column<A> colA, Column<B> colB) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlUpdate<A,B>(builder, colA, colB);
    }
    
    /// <summary>
    /// Start an update command for this table.
    /// </summary>
    public CqlUpdate<A,B,C> Update<A,B,C>(Column<A> colA, Column<B> colB, Column<C> colC) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlUpdate<A,B,C>(builder, colA, colB, colC);
    }
    
    /// <summary>
    /// Start an update command for this table.
    /// </summary>
    public CqlUpdate<A,B,C,D> Update<A,B,C,D>(Column<A> colA, Column<B> colB, Column<C> colC, Column<D> colD) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlUpdate<A,B,C,D>(builder, colA, colB, colC, colD);
    }
    
    /// <summary>
    /// Start an update command for this table.
    /// </summary>
    public CqlUpdate<A,B,C,D,E> Update<A,B,C,D,E>(Column<A> colA, Column<B> colB, Column<C> colC, Column<D> colD,
      Column<E> colE) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlUpdate<A,B,C,D,E>(builder, colA, colB, colC, colD, colE);
    }
    
    /// <summary>
    /// Start an update command for this table.
    /// </summary>
    public CqlUpdate<A,B,C,D,E,F> Update<A,B,C,D,E,F>(Column<A> colA, Column<B> colB, Column<C> colC, Column<D> colD,
      Column<E> colE, Column<F> colF) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlUpdate<A,B,C,D,E,F>(builder, colA, colB, colC, colD, colE, colF);
    }
    
    /// <summary>
    /// Start an update command for this table.
    /// </summary>
    public CqlUpdate<A,B,C,D,E,F,G> Update<A,B,C,D,E,F,G>(Column<A> colA, Column<B> colB, Column<C> colC, Column<D> colD,
      Column<E> colE, Column<F> colF, Column<G> colG) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlUpdate<A,B,C,D,E,F,G>(builder, colA, colB, colC, colD, colE, colF, colG);
    }
    
    /// <summary>
    /// Start an update command for this table.
    /// </summary>
    public CqlUpdate<A,B,C,D,E,F,G,H> Update<A,B,C,D,E,F,G,H>(Column<A> colA, Column<B> colB, Column<C> colC, Column<D> colD,
      Column<E> colE, Column<F> colF, Column<G> colG, Column<H> colH) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlUpdate<A,B,C,D,E,F,G,H>(builder, colA, colB, colC, colD, colE, colF, colG, colH);
    }
    
    /// <summary>
    /// Start an update command for this table.
    /// </summary>
    public CqlUpdate<A,B,C,D,E,F,G,H,I> Update<A,B,C,D,E,F,G,H,I>(Column<A> colA, Column<B> colB, Column<C> colC, Column<D> colD,
      Column<E> colE, Column<F> colF, Column<G> colG, Column<H> colH, Column<I> colI) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlUpdate<A,B,C,D,E,F,G,H,I>(builder, colA, colB, colC, colD, colE, colF, colG, colH, colI);
    }
    
    /// <summary>
    /// Start an update command for this table.
    /// </summary>
    public CqlUpdate<A,B,C,D,E,F,G,H,I,J> Update<A,B,C,D,E,F,G,H,I,J>(Column<A> colA, Column<B> colB, Column<C> colC, Column<D> colD,
      Column<E> colE, Column<F> colF, Column<G> colG, Column<H> colH, Column<I> colI, Column<J> colJ) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlUpdate<A,B,C,D,E,F,G,H,I,J>(builder, colA, colB, colC, colD, colE, colF, colG, colH, colI, colJ);
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlSelect<Row<A>> Select<A>(Column<A> colA) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlSelect<Row<A>>(builder, colA);
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlSelect<Row<A,B>> Select<A,B>(Column<A> colA, Column<B> colB) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlSelect<Row<A,B>>(builder, colA, colB);
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlSelect<Row<A,B,C>> Select<A,B,C>(Column<A> colA, Column<B> colB, Column<C> colC) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlSelect<Row<A,B,C>>(builder, colA, colB, colC);
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlSelect<Row<A,B,C,D>> Select<A,B,C,D>(Column<A> colA, Column<B> colB, Column<C> colC, Column<D> colD) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlSelect<Row<A,B,C,D>>(builder, colA, colB, colC, colD);
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlSelect<Row<A,B,C,D,E>> Select<A,B,C,D,E>(Column<A> colA, Column<B> colB, Column<C> colC,
    Column<D> colD, Column<E> colE) {
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlSelect<Row<A,B,C,D,E>>(builder, colA, colB, colC, colD, colE);
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlSelect<Row<A,B,C,D,E,F>> Select<A,B,C,D,E,F>(Column<A> colA, Column<B> colB, Column<C> colC,
    Column<D> colD, Column<E> colE, Column<F> colF) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlSelect<Row<A,B,C,D,E,F>>(builder, colA, colB, colC, colD, colE, colF);
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlSelect<Row<A,B,C,D,E,F,G>> Select<A,B,C,D,E,F,G>(Column<A> colA, Column<B> colB, Column<C> colC,
    Column<D> colD, Column<E> colE, Column<F> colF, Column<G> colG) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlSelect<Row<A,B,C,D,E,F,G>>(builder, colA, colB, colC, colD, colE, colF, colG);
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlSelect<Row<A,B,C,D,E,F,G,H>> Select<A,B,C,D,E,F,G,H>(Column<A> colA, Column<B> colB, Column<C> colC,
    Column<D> colD, Column<E> colE, Column<F> colF, Column<G> colG, Column<H> colH) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlSelect<Row<A,B,C,D,E,F,G,H>>(builder, colA, colB, colC, colD, colE, colF, colG, colH);
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlSelect<Row<A,B,C,D,E,F,G,H,I>> Select<A,B,C,D,E,F,G,H,I>(Column<A> colA, Column<B> colB, Column<C> colC,
    Column<D> colD, Column<E> colE, Column<F> colF, Column<G> colG, Column<H> colH, Column<I> colI) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlSelect<Row<A,B,C,D,E,F,G,H,I>>(builder, colA, colB, colC, colD, colE, colF, colG, colH, colI);
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlSelect<Row<A,B,C,D,E,F,G,H,I,J>> Select<A,B,C,D,E,F,G,H,I,J>(Column<A> colA, Column<B> colB, Column<C> colC,
    Column<D> colD, Column<E> colE, Column<F> colF, Column<G> colG, Column<H> colH, Column<I> colI, Column<J> colJ) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlSelect<Row<A,B,C,D,E,F,G,H,I,J>>(builder, colA, colB, colC, colD, colE, colF, colG, colH, colI, colJ);
    }
    
    /// <summary>
    /// Submit the specified row collection with 'Insert' statements.
    /// </summary>
    public void Insert(RowCollection collection) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      
      object[] values = new object[collection.Columns.Count];
      
      // iterate the updated rows collection
      foreach(IRow row in collection.Rows) {
        
        // build an update query for the each row
        Query builder = new Query(this);
        builder.Add(Cql.Insert);
        
        // iterate the columns in the collection
        foreach(Column column in collection.Columns) {
          builder.Add(column);
        }
        
        builder.Add(Cql.Set);
        
        // get the current values and updated flags for the cells
        row.GetValues(values);
        
        // add an assignment combination for each changed cell
        for(int i = 0; i < values.Length; ++i) {
          builder.Add(values[i]);
        }
        
        // execute the query
        builder.ExecuteAsync();
      }
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlInsert<A> Insert<A>(Column<A> colA) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlInsert<A>(builder, colA);
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlInsert<A,B> Insert<A,B>(Column<A> colA, Column<B> colB) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlInsert<A,B>(builder, colA, colB);
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlInsert<A,B,C> Insert<A,B,C>(Column<A> colA, Column<B> colB, Column<C> colC) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlInsert<A,B,C>(builder, colA, colB, colC);
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlInsert<A,B,C,D> Insert<A,B,C,D>(Column<A> colA, Column<B> colB, Column<C> colC,
    Column<D> colD) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlInsert<A,B,C,D>(builder, colA, colB, colC, colD);
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlInsert<A,B,C,D,E> Insert<A,B,C,D,E>(Column<A> colA, Column<B> colB, Column<C> colC,
    Column<D> colD, Column<E> colE) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlInsert<A,B,C,D,E>(builder, colA, colB, colC, colD, colE);
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlInsert<A,B,C,D,E,F> Insert<A,B,C,D,E,F>(Column<A> colA, Column<B> colB, Column<C> colC,
    Column<D> colD, Column<E> colE, Column<F> colF) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlInsert<A,B,C,D,E,F>(builder, colA, colB, colC, colD, colE, colF);
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlInsert<A,B,C,D,E,F,G> Insert<A,B,C,D,E,F,G>(Column<A> colA, Column<B> colB, Column<C> colC,
    Column<D> colD, Column<E> colE, Column<F> colF, Column<G> colG) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlInsert<A,B,C,D,E,F,G>(builder, colA, colB, colC, colD, colE, colF, colG);
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlInsert<A,B,C,D,E,F,G,H> Insert<A,B,C,D,E,F,G,H>(Column<A> colA, Column<B> colB, Column<C> colC,
    Column<D> colD, Column<E> colE, Column<F> colF, Column<G> colG, Column<H> colH) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlInsert<A,B,C,D,E,F,G,H>(builder, colA, colB, colC, colD, colE, colF, colG, colH);
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlInsert<A,B,C,D,E,F,G,H,I> Insert<A,B,C,D,E,F,G,H,I>(Column<A> colA, Column<B> colB, Column<C> colC,
    Column<D> colD, Column<E> colE, Column<F> colF, Column<G> colG, Column<H> colH, Column<I> colI) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlInsert<A,B,C,D,E,F,G,H,I>(builder, colA, colB, colC, colD, colE, colF, colG, colH, colI);
    }
    
    /// <summary>
    /// Retrieve rows from this Table.
    /// </summary>
    public CqlInsert<A,B,C,D,E,F,G,H,I,J> Insert<A,B,C,D,E,F,G,H,I,J>(Column<A> colA, Column<B> colB, Column<C> colC,
    Column<D> colD, Column<E> colE, Column<F> colF, Column<G> colG, Column<H> colH, Column<I> colI, Column<J> colJ) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      Query builder = new Query(this);
      return new CqlInsert<A,B,C,D,E,F,G,H,I,J>(builder, colA, colB, colC, colD, colE, colF, colG, colH, colI, colJ);
    }
    
    /// <summary>
    /// Delete the specified columns from the Table. If no columns are specified, the entire row
    /// will be removed.
    /// </summary>
    public CqlDelete Delete(params Column[] columns) {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      
      if(columns.Length == 0) return new CqlDelete(new Query(this));
      return new CqlDelete(new Query(this), columns);
    }
    
    /// <summary>
    /// Delete the specified row from the Table.
    /// </summary>
    public void DeleteRow<TRow>(TRow row, params Column[] columns) where TRow : IRow {
      // ensure the Table is initialized
      if(_initialize) Initialize();
      
      // build an update query for the each row
      Query builder = new Query(this);
      builder.Add(Cql.Delete);
      builder.Add(Cql.All);
      
      // get the current values and updated flags for the cells
      object[] values = new object[columns.Length];
      row.GetValues(values);
      
      // add an assignment combination for each changed cell
      for(int i = 0; i < values.Length; ++i) {
        if(columns[i].IsIdentifier) {
          builder.Add(Cql.Where);
          builder.Add(columns[i]);
          builder.Add(Cql.Equal);
          builder.Add(values[i]);
        }
      }
      
      // execute the query
      builder.ExecuteAsync();
    }
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize this Table instance.
    /// </summary>
    private void Initialize() {
      // take the initialize lock
      _lock.Take();
      // was the table initialized by another thread? yes, skip
      if(!_initialize) {
        _lock.Release();
        return;
      }
      
      // if the keyspace name wasn't set - get the current default
      _keyspace = _keyspaceName == null ?
        ManagerCql.DefaultCluster.DefaultKeyspace :
        ManagerCql.DefaultCluster.GetKeyspace(_keyspaceName);
      
      // get the table metadata
      if(_keyspace.Metadata == null) {
        // create an inline timer to wait for keyspace initialization
        TimeInline timer = new TimeInline(4000, () => _keyspace.Metadata != null);
        
        // wait until timeout or keyspace metadata being populated
        while(timer.Wait) { }
        
        // was the metadata retrieved?
        if(!timer.Success) {
          // no, error log
          Log.Error("Table '" + Name + "' isn't available. Keyspace '"+_keyspaceName+"' couldn't initialize.");
          _lock.Release();
          return;
        }
      }
      
      // get the metadata for the table - does the table exist?
      if(!_keyspace.TableMetadata.TakeItem().TryGetValue(Name, out Metadata)) {
        // no, create the table
        _keyspace.TableMetadata.Release();
        
        bool primaryKey = false;
        
        // add this Table to the Keyspace
        Query builder = new Query(this, _keyspace);
        CqlCreateTable createQuery = new CqlCreateTable(builder);
        // iterate the columns to create
        foreach(Column column in Columns) {
          createQuery.With(column);
          primaryKey |= column.Class == Column.ColumnClass.Primary;
        }
        
        // if there were no primary keys
        if(!primaryKey) {
          Log.Error("Cannot create a Table with no primary keys.");
          _lock.Release();
          return;
        }
        
        // run the query and get the metadata
        Metadata = createQuery;
        
        // add the metadata to the keyspace
        _keyspace.TableMetadata.TakeItem().Add(Name, Metadata);
        
      }
      _keyspace.TableMetadata.Release();
      
      // add this table to the keyspace
      _keyspace.AddTable(this);
      
      ArrayRig<Column> newColumns = new ArrayRig<Column>(Columns);
      ArrayRig<TableColumn> oldColumns = new ArrayRig<TableColumn>();
      
      // iterate table column in the metadata
      foreach(TableColumn col in Metadata.TableColumns) {
        
        // flag to indicate the column reference exists
        bool found = false;
        
        // iterate the columns
        for(int i = Columns.Count-1; i >= 0; --i) {
          // if the name matches
          if(col.Name == Columns[i].Name) {
            // set the index
            Columns[i].Index = col.Index;
            // remove from add collection
            newColumns.RemoveQuick(Columns[i]);
            found = true;
            break;
          }
        }
        
        // was the column found in the metadata? no, add to be removed
        if(!found) oldColumns.Add(col);
        
      }
      
      // add each new column
      foreach(var newColumn in newColumns) {
        
        // log the removal
        Log.CriticalChange("A COLUMN IS ABOUT TO BE Added : " + Keyspace.Metadata.Name + Chars.Stop, 2);
        
        // get the builder
        var builder = StringBuilderCache.Get();
        
        // append a query that will add the new column
        builder.Append("ALTER TABLE ");
        builder.Append(Keyspace.Metadata.Name);
        builder.Append(Chars.Stop);
        builder.Append(Name);
        builder.Append(" ADD ");
        builder.Append(newColumn.Name);
        builder.Append(Chars.Space);
        builder.Append(newColumn.DataType);
        builder.Append(Chars.SemiColon);
        
        // get a simple statement
        var addStatement = new SimpleStatement(builder.ToString());
        
        // execute the statement
        Keyspace.Session.Execute(addStatement);
        
      }
      
      // iterate the old columns
      foreach(var oldColumn in oldColumns) {
        
        // log the removal
        Log.CriticalChange("A COLUMN IS ABOUT TO BE DROPPED : " + Keyspace.Metadata.Name + Chars.Stop, 10);
        
        // get the builder
        var builder = StringBuilderCache.Get();
        
        // append a query that will remove the old column
        builder.Append("ALTER TABLE ");
        builder.Append(Keyspace.Metadata.Name);
        builder.Append(Chars.Stop);
        builder.Append(Name);
        builder.Append(" DROP ");
        builder.Append(oldColumn.Name);
        builder.Append(Chars.SemiColon);
        
        // get a simple statement
        var removeStatement = new SimpleStatement(builder.ToString());
        
        // execute the statement
        Keyspace.Session.Execute(removeStatement);
        
      }
      
      // the table has been initialized
      _initialize = false;
      
      // release the lock
      _lock.Release();
    }
    
  }
  
  
}
