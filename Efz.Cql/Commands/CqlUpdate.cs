/*
 * User: Joshua
 * Date: 9/10/2016
 * Time: 7:23 PM
 */
using System;

namespace Efz.Cql {
  
  /// <summary>
  /// First component of an 'Update' command.
  /// </summary>
  public struct CqlUpdate<A> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct the start of an 'Update' command.
    /// </summary>
    internal CqlUpdate(Query builder, Column<A> colA) {
      _builder = builder;
      // add the Update keyword
      _builder.Add(Cql.Update);
      _builder.Add(colA);
    }
    
    /// <summary>
    /// Construct a new update component.
    /// </summary>
    internal CqlUpdate(Query builder) {
      _builder = builder;
    }
    
    public CqlUpdateWith With(A valA) {
      _builder.Add(valA);
      return new CqlUpdateWith(_builder);
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// First component of an 'Update' command.
  /// </summary>
  public struct CqlUpdate<A,B> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct the start of an 'Update' command.
    /// </summary>
    internal CqlUpdate(Query builder, Column<A> colA, Column<B> colB) {
      _builder = builder;
      // add the Update keyword
      _builder.Add(Cql.Update);
      _builder.Add(colA, colB);
    }
    
    /// <summary>
    /// Construct a new update component.
    /// </summary>
    internal CqlUpdate(Query builder) {
      _builder = builder;
    }
    
    public CqlUpdateWith With(A valA, B valB) {
      _builder.Add(valA, valB);
      return new CqlUpdateWith(_builder);
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// First component of an 'Update' command.
  /// </summary>
  public struct CqlUpdate<A,B,C> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct the start of an 'Update' command.
    /// </summary>
    internal CqlUpdate(Query builder, Column<A> colA, Column<B> colB, Column<C> colC) {
      _builder = builder;
      // add the Update keyword
      _builder.Add(Cql.Update);
      _builder.Add(colA, colB, colC);
    }
    
    /// <summary>
    /// Construct a new update component.
    /// </summary>
    internal CqlUpdate(Query builder) {
      _builder = builder;
    }
    
    public CqlUpdateWith With(A valA, B valB, C valC) {
      _builder.Add(valA, valB, valC);
      return new CqlUpdateWith(_builder);
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// First component of an 'Update' command.
  /// </summary>
  public struct CqlUpdate<A,B,C,D> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct the start of an 'Update' command.
    /// </summary>
    internal CqlUpdate(Query builder, Column<A> colA, Column<B> colB, Column<C> colC, Column<D> colD) {
      _builder = builder;
      // add the Update keyword
      _builder.Add(Cql.Update);
      _builder.Add(colA, colB, colC, colD);
    }
    
    /// <summary>
    /// Construct a new update component.
    /// </summary>
    internal CqlUpdate(Query builder) {
      _builder = builder;
    }
    
    public CqlUpdateWith With(A valA, B valB, C valC, D valD) {
      _builder.Add(valA, valB, valC, valD);
      return new CqlUpdateWith(_builder);
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// First component of an 'Update' command.
  /// </summary>
  public struct CqlUpdate<A,B,C,D,E> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct the start of an 'Update' command.
    /// </summary>
    internal CqlUpdate(Query builder, Column<A> colA, Column<B> colB, Column<C> colC, Column<D> colD, Column<E> colE) {
      _builder = builder;
      // add the Update keyword
      _builder.Add(Cql.Update);
      _builder.Add(colA, colB, colC, colD, colE);
    }
    
    /// <summary>
    /// Construct a new update component.
    /// </summary>
    internal CqlUpdate(Query builder) {
      _builder = builder;
    }
    
    public CqlUpdateWith With(A valA, B valB, C valC, D valD, E valE) {
      _builder.Add(valA, valB, valC, valD, valE);
      return new CqlUpdateWith(_builder);
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// First component of an 'Update' command.
  /// </summary>
  public struct CqlUpdate<A,B,C,D,E,F> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct the start of an 'Update' command.
    /// </summary>
    internal CqlUpdate(Query builder, Column<A> colA, Column<B> colB, Column<C> colC, Column<D> colD, Column<E> colE,
      Column<F> colF) {
      _builder = builder;
      // add the Update keyword
      _builder.Add(Cql.Update);
      _builder.Add(colA, colB, colC, colD, colE, colF);
    }
    
    /// <summary>
    /// Construct a new update component.
    /// </summary>
    internal CqlUpdate(Query builder) {
      _builder = builder;
    }
    
    public CqlUpdateWith With(A valA, B valB, C valC, D valD, E valE, F valF) {
      _builder.Add(valA, valB, valC, valD, valE, valF);
      return new CqlUpdateWith(_builder);
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// First component of an 'Update' command.
  /// </summary>
  public struct CqlUpdate<A,B,C,D,E,F,G> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct the start of an 'Update' command.
    /// </summary>
    internal CqlUpdate(Query builder, Column<A> colA, Column<B> colB, Column<C> colC, Column<D> colD, Column<E> colE,
      Column<F> colF, Column<G> colG) {
      _builder = builder;
      // add the Update keyword
      _builder.Add(Cql.Update);
      _builder.Add(colA, colB, colC, colD, colE, colF, colG);
    }
    
    /// <summary>
    /// Construct a new update component.
    /// </summary>
    internal CqlUpdate(Query builder) {
      _builder = builder;
    }
    
    public CqlUpdateWith With(A valA, B valB, C valC, D valD, E valE, F valF, G valG) {
      _builder.Add(valA, valB, valC, valD, valE, valF, valG);
      return new CqlUpdateWith(_builder);
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// First component of an 'Update' command.
  /// </summary>
  public struct CqlUpdate<A,B,C,D,E,F,G,H> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct the start of an 'Update' command.
    /// </summary>
    internal CqlUpdate(Query builder, Column<A> colA, Column<B> colB, Column<C> colC, Column<D> colD, Column<E> colE,
      Column<F> colF, Column<G> colG, Column<H> colH) {
      _builder = builder;
      // add the Update keyword
      _builder.Add(Cql.Update);
      _builder.Add(colA, colB, colC, colD, colE, colF, colG, colH);
    }
    
    /// <summary>
    /// Construct a new update component.
    /// </summary>
    internal CqlUpdate(Query builder) {
      _builder = builder;
    }
    
    public CqlUpdateWith With(A valA, B valB, C valC, D valD, E valE, F valF, G valG, H valH) {
      _builder.Add(valA, valB, valC, valD, valE, valF, valG, valH);
      return new CqlUpdateWith(_builder);
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// First component of an 'Update' command.
  /// </summary>
  public struct CqlUpdate<A,B,C,D,E,F,G,H,I> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct the start of an 'Update' command.
    /// </summary>
    internal CqlUpdate(Query builder, Column<A> colA, Column<B> colB, Column<C> colC, Column<D> colD, Column<E> colE,
      Column<F> colF, Column<G> colG, Column<H> colH, Column<I> colI) {
      _builder = builder;
      // add the Update keyword
      _builder.Add(Cql.Update);
      _builder.Add(colA, colB, colC, colD, colE, colF, colG, colH, colI);
    }
    
    /// <summary>
    /// Construct a new update component.
    /// </summary>
    internal CqlUpdate(Query builder) {
      _builder = builder;
    }
    
    public CqlUpdateWith With(A valA, B valB, C valC, D valD, E valE, F valF, G valG, H valH, I valI) {
      _builder.Add(valA, valB, valC, valD, valE, valF, valG, valH, valI);
      return new CqlUpdateWith(_builder);
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// First component of an 'Update' command.
  /// </summary>
  public struct CqlUpdate<A,B,C,D,E,F,G,H,I,J> {
    
    //----------------------------------//
    
    //----------------------------------//
    
    private Query _builder;
    
    //----------------------------------//
    
    /// <summary>
    /// Construct the start of an 'Update' command.
    /// </summary>
    internal CqlUpdate(Query builder, Column<A> colA, Column<B> colB, Column<C> colC, Column<D> colD, Column<E> colE,
      Column<F> colF, Column<G> colG, Column<H> colH, Column<I> colI, Column<J> colJ) {
      _builder = builder;
      // add the Update keyword
      _builder.Add(Cql.Update);
      _builder.Add(colA, colB, colC, colD, colE, colF, colG, colH, colI, colJ);
    }
    
    /// <summary>
    /// Construct a new update component.
    /// </summary>
    internal CqlUpdate(Query builder) {
      _builder = builder;
    }
    
    public CqlUpdateWith With(A valA, B valB, C valC, D valD, E valE, F valF, G valG, H valH, I valI, J valJ) {
      _builder.Add(valA, valB, valC, valD, valE, valF, valG, valH, valI, valJ);
      return new CqlUpdateWith(_builder);
    }
    
    //----------------------------------//
    
  }
  
  
}
