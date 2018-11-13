/*
 * User: Joshua
 * Date: 24/09/2016
 * Time: 6:27 PM
 */
using System;

namespace Efz.Cql {
  
  /// <summary>
  /// Struct to initialize a select command.
  /// </summary>
  public struct CqlInsert<A> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// The query builder that collects the keywords, values and columns.
    /// </summary>
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder, params Column[] columns) {
      _builder = builder;
      _builder.Add(Cql.Insert);
      foreach(Column column in columns) {
        _builder.Add(column);
      }
    }
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Add column values to the insert command.
    /// </summary>
    public CqlInsertValues With(A valA) {
      _builder.Add(valA);
      return new CqlInsertValues(_builder);
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Struct to initialize a select command.
  /// </summary>
  public struct CqlInsert<A,B> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// The query builder that collects the keywords, values and columns.
    /// </summary>
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder, params Column[] columns) {
      _builder = builder;
      _builder.Add(Cql.Insert);
      foreach(Column column in columns) {
        _builder.Add(column);
      }
    }
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Add column values to the insert command.
    /// </summary>
    public CqlInsertValues With(A valA, B valB) {
      _builder.Add(valA, valB);
      return new CqlInsertValues(_builder);
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Struct to initialize a select command.
  /// </summary>
  public struct CqlInsert<A,B,C> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// The query builder that collects the keywords, values and columns.
    /// </summary>
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder, params Column[] columns) {
      _builder = builder;
      _builder.Add(Cql.Insert);
      foreach(Column column in columns) {
        _builder.Add(column);
      }
    }
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Add column values to the insert command.
    /// </summary>
    public CqlInsertValues With(A valA, B valB, C valC) {
      _builder.Add(valA, valB, valC);
      return new CqlInsertValues(_builder);
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Struct to initialize a select command.
  /// </summary>
  public struct CqlInsert<A,B,C,D> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// The query builder that collects the keywords, values and columns.
    /// </summary>
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder, params Column[] columns) {
      _builder = builder;
      _builder.Add(Cql.Insert);
      foreach(Column column in columns) {
        _builder.Add(column);
      }
    }
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Add column values to the insert command.
    /// </summary>
    public CqlInsertValues With(A valA, B valB, C valC, D valD) {
      _builder.Add(valA, valB, valC, valD);
      return new CqlInsertValues(_builder);
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Struct to initialize a select command.
  /// </summary>
  public struct CqlInsert<A,B,C,D,E> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// The query builder that collects the keywords, values and columns.
    /// </summary>
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder, params Column[] columns) {
      _builder = builder;
      _builder.Add(Cql.Insert);
      foreach(Column column in columns) {
        _builder.Add(column);
      }
    }
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Add column values to the insert command.
    /// </summary>
    public CqlInsertValues With(A valA, B valB, C valC, D valD, E valE) {
      _builder.Add(valA, valB, valC, valD, valE);
      return new CqlInsertValues(_builder);
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Struct to initialize a select command.
  /// </summary>
  public struct CqlInsert<A,B,C,D,E,F> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// The query builder that collects the keywords, values and columns.
    /// </summary>
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder, params Column[] columns) {
      _builder = builder;
      _builder.Add(Cql.Insert);
      foreach(Column column in columns) {
        _builder.Add(column);
      }
    }
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Add column values to the insert command.
    /// </summary>
    public CqlInsertValues With(A valA, B valB, C valC, D valD, E valE, F valF) {
      _builder.Add(valA, valB, valC, valD, valE, valF);
      return new CqlInsertValues(_builder);
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Struct to initialize a select command.
  /// </summary>
  public struct CqlInsert<A,B,C,D,E,F,G> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// The query builder that collects the keywords, values and columns.
    /// </summary>
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder, params Column[] columns) {
      _builder = builder;
      _builder.Add(Cql.Insert);
      foreach(Column column in columns) {
        _builder.Add(column);
      }
    }
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Add column values to the insert command.
    /// </summary>
    public CqlInsertValues With(A valA, B valB, C valC, D valD, E valE, F valF, G valG) {
      _builder.Add(valA, valB, valC, valD, valE, valF, valG);
      return new CqlInsertValues(_builder);
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Struct to initialize a select command.
  /// </summary>
  public struct CqlInsert<A,B,C,D,E,F,G,H> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// The query builder that collects the keywords, values and columns.
    /// </summary>
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder, params Column[] columns) {
      _builder = builder;
      _builder.Add(Cql.Insert);
      foreach(Column column in columns) {
        _builder.Add(column);
      }
    }
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Add column values to the insert command.
    /// </summary>
    public CqlInsertValues With(A valA, B valB, C valC, D valD, E valE, F valF, G valG, H valH) {
      _builder.Add(valA, valB, valC, valD, valE, valF, valG, valH);
      return new CqlInsertValues(_builder);
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Struct to initialize a select command.
  /// </summary>
  public struct CqlInsert<A,B,C,D,E,F,G,H,I> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// The query builder that collects the keywords, values and columns.
    /// </summary>
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder, params Column[] columns) {
      _builder = builder;
      _builder.Add(Cql.Insert);
      foreach(Column column in columns) {
        _builder.Add(column);
      }
    }
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Add column values to the insert command.
    /// </summary>
    public CqlInsertValues With(A valA, B valB, C valC, D valD, E valE, F valF, G valG, H valH, I valI) {
      _builder.Add(valA, valB, valC, valD, valE, valF, valG, valH, valI);
      return new CqlInsertValues(_builder);
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Struct to initialize a select command.
  /// </summary>
  public struct CqlInsert<A,B,C,D,E,F,G,H,I,J> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    /// <summary>
    /// The query builder that collects the keywords, values and columns.
    /// </summary>
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder, params Column[] columns) {
      _builder = builder;
      _builder.Add(Cql.Insert);
      foreach(Column column in columns) {
        _builder.Add(column);
      }
    }
    
    /// <summary>
    /// Construct a new cql select component.
    /// </summary>
    internal CqlInsert(Query builder) {
      _builder = builder;
    }
    
    /// <summary>
    /// Add column values to the insert command.
    /// </summary>
    public CqlInsertValues With(A valA, B valB, C valC, D valD, E valE, F valF, G valG, H valH, I valI, J valJ) {
      _builder.Add(valA, valB, valC, valD, valE, valF, valG, valH, valI, valJ);
      return new CqlInsertValues(_builder);
    }
    
    //----------------------------------//
    
  }
  
}
