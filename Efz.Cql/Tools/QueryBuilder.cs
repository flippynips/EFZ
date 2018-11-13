/*
 * User: Joshua
 * Date: 3/10/2016
 * Time: 2:48 AM
 */
using System;
using System.Text;
using Efz.Collections;

namespace Efz.Cql {
  
  /// <summary>
  /// Class responsible for constructing queries from sets of values and cql keywords.
  /// </summary>
  internal class QueryBuilder {
    
    //----------------------------------//
    
    /// <summary>
    /// The string representation of the query.
    /// </summary>
    public string QueryString {
      get {
        if(_build) Build();
        return _queryString;
      }
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Instance of a query this builder will operate on.
    /// </summary>
    protected Query _query;
    /// <summary>
    /// If the query needs to be constructed.
    /// </summary>
    protected bool _build;
    
    /// <summary>
    /// When building the query - track iteration through the keywords.
    /// </summary>
    protected int _keywordIndex;
    /// <summary>
    /// When building the query - tracks iteration through the columns.
    /// </summary>
    protected int _columnIndex;
    /// <summary>
    /// When building the query - tracking of the sets collection index.
    /// </summary>
    protected int _setIndex;
    /// <summary>
    /// When building the query - the string builder instance of the string representation.
    /// </summary>
    protected StringBuilder _builder;
    
    /// <summary>
    /// String representation of the complete query.
    /// </summary>
    protected string _queryString;
    
    /// <summary>
    /// Flag that is tripped on range operations to enable the Cassandra query
    /// to bypass expensive operation prevention.
    /// </summary>
    protected bool _allowFiltering;
    
    //----------------------------------//
    
    /// <summary>
    /// Initialize the query builder with the necessary collections.
    /// </summary>
    public QueryBuilder(Query query) {
      _query = query;
      _build = true;
    }
    
    /// <summary>
    /// Get the complete string representation and objects to bind to the query.
    /// </summary>
    public void Build() {
      _build = false;
      
      // initialize the string builder
      _builder = StringBuilderCache.Get();
      
      if(_query.Keywords.Count == 0) {
        Log.Warning("Query contains zero commands.");
      } else {
        switch(_query.Keywords[_keywordIndex]) {
          case Cql.Select:
            ++_keywordIndex;
            ConstructSelect();
            break;
          case Cql.Update:
            ++_keywordIndex;
            ConstructUpdate();
            break;
          case Cql.Insert:
            ++_keywordIndex;
            ConstructInsert();
            break;
          case Cql.Delete:
            ++_keywordIndex;
            ConstructDelete();
            break;
          case Cql.Create:
            ++_keywordIndex;
            switch(_query.Keywords[_keywordIndex]) {
              case Cql.Table:
                ++_keywordIndex;
                ConstructCreateTable();
                break;
              default:
                Log.Warning("Unexpected initial command type '" + _query.Keywords[0] + "'.");
                break;
            }
            
            break;
          default:
            Log.Warning("Unexpected initial command type '" + _query.Keywords[0] + "'.");
            break;
        }
      }
      
      _queryString = StringBuilderCache.SetAndGet(_builder);
      _builder = null;
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Construct a 'Select' query from the query commands and values.
    /// Returns the query string and value collection to bind to a prepared statement.
    /// </summary>
    private void ConstructSelect() {
      bool skip = false;
      
      // append the 'SELECT' command keywords
      _builder.Append(Common.Select);
      
      // if the next keyword is related to the 'SELECT' command
      switch(_query.Keywords[_keywordIndex]) {
        case Cql.Distinct:
          _builder.Append(Common.Distinct);
          ++_keywordIndex;
          break;
        case Cql.Count:
          _builder.Append(Common.Count);
          ++_keywordIndex;
          skip = true;
          break;
        case Cql.All:
          _builder.Append(Chars.Asterisk);
          ++_keywordIndex;
          skip = true;
          break;
      }
      
      if(!skip) {
        int setCount = _query.Sets[_setIndex];
        // append the columns of the select statement
        if(setCount == 1) {
          // append a single column
          _builder.Append(_query.Columns[_columnIndex].Name);
          ++_columnIndex;
        } else {
          // append the first column
          _builder.Append(_query.Columns[_columnIndex].Name);
          ++_columnIndex;
          
          // append the remaining columns
          for(int i = setCount-2; i >= 0; --i) {
            _builder.Append(Chars.Comma);
            _builder.Append(_query.Columns[_columnIndex].Name);
            ++_columnIndex;
          }
        }
        ++_setIndex;
        skip = false;
      }
      
      // append the 'FROM' command keywords
      _builder.Append(Common.From);
      
      // append the table name
      _builder.Append(_query.Table.Name);
      
      // append any 'WHERE' expressions
      if(_query.Keywords[_keywordIndex] == Cql.Where) {
        // flag to indicate the first 'WHERE' expression
        bool first = true;
        
        _builder.Append(Common.Where);
        
        while(_keywordIndex < _query.Keywords.Count && _query.Keywords[_keywordIndex] == Cql.Where) {
          ++_keywordIndex;
          
          // if this the first expression - invert flag
          if(first) first = false;
          // append 'AND'
          else _builder.Append(Common.And);
          
          bool closeBrace = false;
          
          // if the next keyword pertains to the value
          if(_query.Keywords[_keywordIndex] == Cql.Token) {
            ++_keywordIndex;
            _builder.Append(Common.Token);
            closeBrace = true;
          }
          
          // get the number of columns in the expression
          int setCount = _query.Sets[_setIndex];
          ++_setIndex;
          
          bool token = false;
          
          if(setCount == 1) {
            // append the first column of the 'WHERE' expression
            if(_query.Columns[_columnIndex].Class == Column.ColumnClass.Primary) {
              switch(_query.Keywords[_keywordIndex]) {
                case Cql.GreaterThan:
                case Cql.GreaterOrEqual:
                case Cql.LessThan:
                case Cql.LessOrEqual:
                  _builder.Append(Common.Token);
                  _builder.Append(_query.Columns[_columnIndex].Name);
                  _builder.Append(Chars.BracketClose);
                  ++_columnIndex;
                  token = true;
                  break;
                default:
                  _builder.Append(_query.Columns[_columnIndex].Name);
                  ++_columnIndex;
                  break;
              }
            } else {
              _builder.Append(_query.Columns[_columnIndex].Name);
              ++_columnIndex;
            }
          } else {
            _builder.Append(_query.Columns[_columnIndex].Name);
            ++_columnIndex;
            // append any remaining columns
            while(--setCount != 0) {
              _builder.Append(Chars.Comma);
              _builder.Append(_query.Columns[_columnIndex].Name);
              ++_columnIndex;
            }
          }
          
          // close the open brace of the 'TOKEN' keyword if it exists
          if(closeBrace) _builder.Append(Chars.BracketClose);
          
          // append the relational operation
          switch(_query.Keywords[_keywordIndex]) {
            case Cql.Equal:
              _builder.Append(Common.Equal);
              _allowFiltering = true;
              break;
            case Cql.NotEqual:
              _builder.Append(Common.NotEqual);
              break;
            case Cql.LessThan:
              _builder.Append(Common.LessThan);
              _allowFiltering = true;
              break;
            case Cql.GreaterThan:
              _builder.Append(Common.GreaterThan);
              _allowFiltering = true;
              break;
            case Cql.LessOrEqual:
              _builder.Append(Common.LessEqualThan);
              _allowFiltering = true;
              break;
            case Cql.GreaterOrEqual:
              _builder.Append(Common.GreaterEqualThan);
              _allowFiltering = true;
              break;
            case Cql.In:
              _builder.Append(Common.In);
              break;
            case Cql.Contains:
              _builder.Append(Common.Contains);
              _allowFiltering = true;
              break;
            default:
              Log.Error("Unexpected keyword in 'SELECT' query builder.");
              return;
          }
          ++_keywordIndex;
          
          // append the right-hand side of the expression
          
          // get the number of literal values
          setCount = _query.Sets[_setIndex];
          ++_setIndex;
          
          // append a bind token (?) for each of the literal objects
          if(setCount == 1) {
            if(token) {
              _builder.Append("TOKEN(?)");
            } else {
              _builder.Append(Chars.Question);
            }
          } else {
            _builder.Append(Chars.Question);
            while(--setCount != 0) {
              _builder.Append(Chars.Comma);
              if(token) {
                _builder.Append("TOKEN(?)");
              } else {
                _builder.Append(Chars.Question);
              }
            }
          }
        }
      }
      
      // append any 'ORDER BY' expressions
      if(_keywordIndex < _query.Keywords.Count && _query.Keywords[_keywordIndex] == Cql.OrderBy) {
        ++_keywordIndex;
        _builder.Append(Common.OrderBy);
        // append the clustering column name
        _builder.Append(_query.Columns[_columnIndex].Name);
        ++_columnIndex;
        // if the order is to be descending
        if(_query.Keywords[_keywordIndex] == Cql.Decending) {
          ++_keywordIndex;
          _builder.Append(Common.Decending);
        } else {
          // the default sort order is ascending
          _builder.Append(Chars.Space);
        }
      }
      
      // append any 'LIMIT' expression
      if(_query.Limit != 0) {
        _builder.Append(Common.Limit);
        _builder.Append(_query.Limit);
      }
      
      // if the filtering flag was tripped by ranges of values
      if(_allowFiltering) {
        _builder.Append(Common.AllowFiltering);
      }
      
      // terminate the query with a semicolon
      _builder.Append(Chars.SemiColon);
    }
    
    /// <summary>
    /// Construct an 'Update' query from the commands and values.
    /// </summary>
    private void ConstructUpdate() {
      
      // append the 'UPDATE' command keyword
      _builder.Append(Common.Update);
      
      // append the table name
      _builder.Append(_query.Table.Name);
      
      // append 'SET'
      _builder.Append(Common.Set);
      
      // append the assignment operators
      int count = _query.Sets[_setIndex] / 2;
      ++_setIndex;
      
      _builder.Append(_query.Columns[_columnIndex].Name);
      ++_columnIndex;
      _builder.Append(Common.Equal);
      _builder.Append(Chars.Question);
      while(--count != 0) {
        _builder.Append(Chars.Comma);
        _builder.Append(_query.Columns[_columnIndex].Name);
        ++_columnIndex;
        _builder.Append(Common.Equal);
        _builder.Append(Chars.Question);
      }
      
      // append all 'WHERE' constraints
      bool first = true;
      while(_keywordIndex != _query.Keywords.Count && _query.Keywords[_keywordIndex] == Cql.Where) {
        ++_keywordIndex;
        
        if(first) {
          // append 'WHERE' keyword
          _builder.Append(Common.Where);
          first = false;
        } else {
          // append 'AND' keyword
          _builder.Append(Common.And);
        }
        
        // get the number of columns in the 'WHERE' statement
        count = _query.Sets[_setIndex];
        ++_setIndex;
        
        // append each column name
        _builder.Append(_query.Columns[_columnIndex].Name);
        ++_columnIndex;
        while(--count != 0) {
          _builder.Append(Chars.Comma);
          _builder.Append(_query.Columns[_columnIndex].Name);
          ++_columnIndex;
        }
        
        // check the assignment opperator
        switch(_query.Keywords[_keywordIndex]) {
          case Cql.Equal:
            ++_keywordIndex;
            _builder.Append(Common.Equal);
            break;
          case Cql.In:
            ++_keywordIndex;
            _builder.Append(Common.In);
            break;
          default:
            Log.Error("Unhandled CQL 'WHERE' constraint in 'UPDATE' constraint specification.");
            return;
        }
        
        // get the number of values in the assignment
        count = _query.Sets[_setIndex];
        ++_setIndex;
        
        // append a bind token (?) for each of the literal objects
        _builder.Append(Chars.Question);
        while(--count != 0) {
          _builder.Append(Chars.Comma);
          _builder.Append(Chars.Question);
        }
      }
      
      // append any 'IF' constraint on the execution of the 'UPDATE' command
      if(_keywordIndex != _query.Keywords.Count && _query.Keywords[_keywordIndex] == Cql.If) {
        _builder.Append(Common.If);
        _builder.Append(_query.Columns[_columnIndex].Name);
        ++_columnIndex;
        _builder.Append(Common.Equal);
        _builder.Append(Chars.Question);
        ++_keywordIndex;
        
        while(_keywordIndex != _query.Keywords.Count && _query.Keywords[_keywordIndex] == Cql.If) {
          _builder.Append(Common.And);
          _builder.Append(_query.Columns[_columnIndex].Name);
          ++_columnIndex;
          _builder.Append(Common.Equal);
          _builder.Append(Chars.Question);
          ++_keywordIndex;
        }
      }
      
      // check if the 'IF EXISTS' modifier should be added
      if(_keywordIndex != _query.Keywords.Count && _query.Keywords[_keywordIndex] == Cql.IfExists) {
        _builder.Append(" IF EXISTS;");
      } else {
        _builder.Append(Chars.SemiColon);
      }
    }
    
    /// <summary>
    /// Construct a 'Insert' query from the commands and values.
    /// </summary>
    private void ConstructInsert() {
      // append the insert command
      _builder.Append(Common.Insert);
      
      // append the table name
      _builder.Append(_query.Table.Name);
      
      _builder.Append(" (");
      
      // append all column names
      _builder.Append(_query.Columns[_columnIndex].Name);
      while(++_columnIndex != _query.Columns.Count) {
        _builder.Append(Chars.Comma);
        _builder.Append(_query.Columns[_columnIndex].Name);
      }
      
      _builder.Append(Common.Values);
      
      // append binding symbols for the values
      _columnIndex = _query.Values.Count;
      _builder.Append(Chars.Question);
      // if there are values to be set
      while(--_columnIndex != 0) {
        _builder.Append(Chars.Comma);
        _builder.Append(Chars.Question);
      }
      
      // add the if not keyword
      if(_keywordIndex != _query.Keywords.Count && _query.Keywords[_keywordIndex] == Cql.IfNotExists) {
        _builder.Append(") IF NOT EXISTS;");
        ++_keywordIndex;
      } else {
        _builder.Append(");");
      }
      
    }
    
    /// <summary>
    /// Construct a 'Delete' command with the query keywords, columns and values.
    /// </summary>
    private void ConstructDelete() {
      // append the insert command
      _builder.Append(Common.Delete);
      
      // is the query to delete all columns?
      if(_query.Keywords[_keywordIndex] == Cql.All) {
        // yes, don't add anything
        ++_keywordIndex;
      } else {
        // get the number of columns
        int count = _query.Sets[_setIndex];
        ++_setIndex;
        
        // get the first column
        Column column = _query.Columns[_columnIndex];
        ++_columnIndex;
        
        _builder.Append(column.Name);
        
        // iterate the remaining columns to add
        while(--count != 0) {
          column = _query.Columns[_columnIndex];
          ++_columnIndex;
          
          _builder.Append(Chars.Comma);
          _builder.Append(column.Name);
        }
      }
      
      // append the table name
      _builder.Append(Common.From);
      _builder.Append(_query.Table.Name);
      
      // append all 'WHERE' constraints
      bool first = true;
      while(_keywordIndex != _query.Keywords.Count && _query.Keywords[_keywordIndex] == Cql.Where) {
        ++_keywordIndex;
        
        if(first) {
          // append the 'WHERE' keyword
          _builder.Append(Common.Where);
          first = false;
        } else _builder.Append(Common.And);
        
        // get the number of columns in the where statement
        int count = _query.Sets[_setIndex];
        ++_setIndex;
        
        // append each column name
        _builder.Append(_query.Columns[_columnIndex].Name);
        ++_columnIndex;
        while(--count != 0) {
          _builder.Append(Chars.Comma);
          _builder.Append(_query.Columns[_columnIndex].Name);
          ++_columnIndex;
        }
        
        // check the assignment operator
        switch(_query.Keywords[_keywordIndex]) {
          case Cql.Equal:
            _builder.Append(Common.Equal);
            // get the number of values in the assignment
            count = _query.Sets[_setIndex];
            ++_setIndex;
            
            // append a bind token (?) for each of the literal objects
            _builder.Append(Chars.Question);
            while(--count != 0) {
              _builder.Append(Chars.Comma);
              _builder.Append(Chars.Question);
            }
            break;
          case Cql.In:
            _builder.Append(Common.In);
            _builder.Append(Chars.BracketOpen);
            // get the number of values in the assignment
            count = _query.Sets[_setIndex];
            ++_setIndex;
            
            // append a bind token (?) for each of the literal objects
            _builder.Append(Chars.Question);
            while(--count != 0) {
              _builder.Append(Chars.Comma);
              _builder.Append(Chars.Question);
            }
            _builder.Append(Chars.BracketClose);
            break;
          default:
            Log.Error("Unhandled CQL command keyword in 'DELETE' constraint specification.");
            return;
        }
        ++_keywordIndex;
        
      }
      
      // append all 'IF' constraints
      while(_keywordIndex != _query.Keywords.Count && _query.Keywords[_keywordIndex] == Cql.If) {
        _builder.Append(_query.Columns[_columnIndex].Name);
        ++_columnIndex;
        
        _builder.Append(Chars.Equal);
        _builder.Append(Chars.Question);
      }
      
      // if the 'DELETE' command has been added
      if(_keywordIndex != _query.Keywords.Count && _query.Keywords[_keywordIndex] == Cql.IfExists) {
        _builder.Append(" IF EXISTS");
      }
      
      // end the 'DELETE' command with a colon
      _builder.Append(Chars.SemiColon);
    }
    
    /// <summary>
    /// Construct a Cassandra query in order to create a Table.
    /// </summary>
    private void ConstructCreateTable() {
      
      // append the 'CREATE TABLE' command keyword
      _builder.Append("CREATE TABLE ");
      // append the table name to the query
      _builder.Append(_query.Table.Name);
      
      // open brace for the column definitions
      _builder.Append(" (");
      
      ArrayRig<Column> primary = new ArrayRig<Column>();
      ArrayRig<Column> cluster = new ArrayRig<Column>();
      
      // get the number of columns
      int count = _query.Sets[_setIndex];
      ++_setIndex;
      
      // get the first column
      Column column = _query.Columns[_columnIndex];
      ++_columnIndex;
      
      // append the column and type
      _builder.Append(column.Name);
      _builder.Append(Chars.Space);
      _builder.Append(column.DataType);
      
      // if - add the column to the primary collection
      if(column.Class == Column.ColumnClass.Primary) primary.Add(column);
      // if - add the column to the cluster collections
      if(column.Class == Column.ColumnClass.Cluster) cluster.Add(column);
      
      while(--count != 0) {
        _builder.Append(Chars.Comma);
        
        // get the column
        column = _query.Columns[_columnIndex];
        ++_columnIndex;
        
        // append the column and type
        _builder.Append(column.Name);
        _builder.Append(Chars.Space);
        _builder.Append(column.DataType);
        
        // if - add the column to the primary collection
        if(column.Class == Column.ColumnClass.Primary) primary.Add(column);
        // if - add the column to the cluster collections
        if(column.Class == Column.ColumnClass.Cluster) cluster.Add(column);
      }
      
      _builder.Append(", PRIMARY KEY (");
      
      if(primary.Count == 1) {
        _builder.Append(primary[0].Name);
        
        if(cluster.Count != 0) {
          // append the clustering columns
          _builder.Append(Chars.Comma);
          _builder.Append(cluster[--cluster.Count].Name);
          while(cluster.Count != 0) {
            _builder.Append(Chars.Comma);
            _builder.Append(cluster[--cluster.Count].Name);
          }
        }
        
      } else {
        if(cluster.Count != 0) {
          // open brace for multiple partition keys
          _builder.Append(Chars.BracketOpen);
          
          // append the primary columns
          _builder.Append(primary[--primary.Count].Name);
          while(primary.Count != 0) {
            _builder.Append(Chars.Comma);
            _builder.Append(primary[--primary.Count].Name);
          }
          
          // close brace for the partition keys
          _builder.Append(Chars.BracketClose);
          
          // add the comma
          _builder.Append(Chars.Comma);
          
          // append the clustering columns
          _builder.Append(cluster[--cluster.Count].Name);
          while(cluster.Count != 0) {
            _builder.Append(Chars.Comma);
            _builder.Append(cluster[--cluster.Count].Name);
          }
          
          
        } else {
          // append the primary columns
          _builder.Append(primary[--primary.Count].Name);
          while(primary.Count != 0) {
            _builder.Append(Chars.Comma);
            _builder.Append(primary[--primary.Count].Name);
          }
          
        }
      }
      
      // close brace for the column definitions
      _builder.Append(Chars.BracketClose, 2);
      
      // if any table properties were defined
      if(_keywordIndex != _query.Keywords.Count && _query.Keywords[_keywordIndex] == Cql.Where) {
        ++_keywordIndex;
        
        // end the column definitions
        _builder.Append(") WITH ");
        
        // get the number of properties
        count = _query.Sets[_setIndex];
        ++_setIndex;
        int valueIndex = 0;
        
        // append specified Table properties
        _builder.Append(_query.Values[valueIndex]);
        while(count != 0) {
          _builder.Append(Common.And);
          _builder.Append(_query.Values[++valueIndex]);
        }
        
        // remove all values from being bound
        _query.Values.Clear();
      }
      
      // end the query
      _builder.Append(Chars.SemiColon);
    }
    
    
    
    
    
    
    
    
    
  }
  
}
