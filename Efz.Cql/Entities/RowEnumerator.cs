/*
 * User: Joshua
 * Date: 1/10/2016
 * Time: 3:57 PM
 */
using System;
using System.Collections.Generic;

using Cassandra;

namespace Efz.Cql {
  
  /// <summary>
  /// Enumeration of the results of a query.
  /// </summary>
  public class RowEnumerator<TRow> : IEnumerable<TRow>, IEnumerator<TRow> where TRow : IRow, new() {
    
    //----------------------------------//
    
    /// <summary>
    /// Get the current row instance.
    /// </summary>
    public TRow Current {
      get {
        return _current;
      }
    }
    
    /// <summary>
    /// The last index of incremental updates.
    /// </summary>
    internal int LastUpdated;
    /// <summary>
    /// The Cassandra.RowSet of all row results that can be enumerated.
    /// </summary>
    internal RowSet RowSet;
    /// <summary>
    /// The collection of changed rows.
    /// </summary>
    internal RowCollection Collection;
    
    //----------------------------------//
    
    /// <summary>
    /// Inner current enumeration item.
    /// </summary>
    object System.Collections.IEnumerator.Current { get { return this._current; } }
    
    /// <summary>
    /// Current enumerator of the row set.
    /// </summary>
    private IEnumerator<Row> _enumerator;
    
    /// <summary>
    /// Callback on a new page or results being requested.
    /// </summary>
    private readonly IAction<RowEnumerator<TRow>> _onPage;
    /// <summary>
    /// Has the callback been set.
    /// </summary>
    private readonly bool _onPageSet;
    
    /// <summary>
    /// Flag for currently fetching a page of rows.
    /// </summary>
    private bool _fetching;
    /// <summary>
    /// Flag for the first enumeration move.
    /// </summary>
    private bool _started;
    /// <summary>
    /// Remaining rows in the current page before more results must be retrieved.
    /// </summary>
    private int _remainingPageRows;
    
    /// <summary>
    /// The table the rows should be added to.
    /// </summary>
    private Table _table;
    
    /// <summary>
    /// Inner current row.
    /// </summary>
    private TRow _current;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize a row enumerator for the specified row set.
    /// </summary>
    public RowEnumerator(Table table, RowSet rowSet, Action<RowEnumerator<TRow>> onPage) : this(table, rowSet, new Act<RowEnumerator<TRow>>(onPage)) {}
    /// <summary>
    /// Initialize a row enumerator for the specified row set.
    /// </summary>
    public RowEnumerator(Table table, RowSet rowSet, IAction<RowEnumerator<TRow>> onPage = null) {
      _table = table;
      RowSet = rowSet;
      _onPage = onPage;
      _onPageSet = _onPage != null;
      
      if(_onPageSet) _onPage.ArgA = this;
      
      _enumerator = RowSet.GetEnumerator();
      LastUpdated = 0;
      Collection = new RowCollection();
      // add the table columns queried to the row collection
      bool isUpdatable = false;
      foreach(CqlColumn col in RowSet.Columns) {
        Column column = _table.Columns.GetSingle(c => c.Name.Equals(col.Name));
        if(column.IsIdentifier) isUpdatable = true;
        Collection.Columns.Add(column);
      }
      
      // are row updates enabled?
      if(_table.UpdateRowChanges && isUpdatable) {
        // yeah, add the row collection to the table
        _table.AddCollection(Collection);
      }
    }
    
    /// <summary>
    /// Move to the next row in the result set.
    /// </summary>
    public bool MoveNext() {
      
      // if the callback has been set
      if(_onPageSet) {
        // if a new page has just been fetched
        if(_fetching) {
          _fetching = false;
          
          // update the page row count
          _remainingPageRows = RowSet.GetAvailableWithoutFetching() - 1;
          
        } else if(--_remainingPageRows == 0) {
          
          // flag as fetching
          _fetching = true;
          // start a task to fetch more results and then run the callback method
          ManagerUpdate.Control.AddSingle(new ActionPair(RowSet.FetchMoreResults, _onPage));
          
          return false;
        }
      }
      
      // if there are more rows to enumerate
      if(_enumerator.MoveNext()) {
        
        // if the enumeration has started
        if(_started) {
          // if the current row has been updated
          if(_current.Updated) {
            // add the current row to the collection
            Collection.Rows.Enqueue(_current);
          }
        } else {
          _started = true;
        }
        
        // create the next row instance
        _current = new TRow();
        // populate the row with values
        _current.Create(_enumerator.Current);
        
        return true;
      }
      
      // if the enumeration has started and the current row has been updated
      if(_started && _current.Updated) {
        _started = false;
        // add the current row to the collection
        Collection.Rows.Enqueue(_current);
      }
      
      // no more rows
      return false;
    }
    
    public void Reset() {
    }
    
    public void Dispose() {
      // if the enumeration has started and the current row has been updated
      if(_started && _current.Updated) {
        // add the current row to the collection
        Collection.Rows.Enqueue(_current);
      }
      
      _enumerator.Dispose();
    }
    
    public IEnumerator<TRow> GetEnumerator() {
      return this;
    }
    
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
      return this;
    }
    
    //----------------------------------//
    
  }
  
  
}
