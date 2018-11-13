/*
 * User: Bob
 * Date: 13/11/2016
 * Time: 10:53
 */
/*
 * User: Joshua
 * Date: 6/08/2016
 * Time: 6:11 PM
 */
using System;
using System.Collections.Generic;
using System.Text;

using Efz.Collections;
using Efz.Web.Display;

namespace Efz.Web.Display {
  
  /// <summary>
  /// Comment element that gets written to a new line. Only the Content
  /// is taken into account.
  /// </summary>
  public class ElementTable : Element {
    
    //----------------------------------//
    
    /// <summary>
    /// Class that represents a row.
    /// </summary>
    public class Row {
      /// <summary>
      /// Row element.
      /// </summary>
      public Element RowElement {
        get {
          Table._skip = true;
          if(_rowElement == null) _rowElement = new Element { Tag = Tag.TableRow, Parent = Table };
          Table._skip = false;
          return _rowElement;
        }
        set {
          Table._skip = true;
          if(_rowElement != null) _rowElement.Replace(value);
          else {
            _rowElement = value;
            _rowElement.Parent = Table;
          }
          Table._skip = false;
        }
      }
      
      /// <summary>
      /// Get the number of cells in the row.
      /// </summary>
      public int CellCount {
        get { return Cells == null ? 0 : Cells.Count; }
      }
      /// <summary>
      /// Cell collection for the row. Modify with care, list isn't tracked.
      /// </summary>
      public ArrayRig<Element> Cells;
      /// <summary>
      /// The table this row belongs to.
      /// </summary>
      public ElementTable Table;
      
      /// <summary>
      /// Inner row element.
      /// </summary>
      private Element _rowElement;
      
      /// <summary>
      /// Get or set a row cell by index.
      /// </summary>
      public Element this[int x] {
        get {
          while(Table._columnCount <= x) {
            foreach(var row in Table._rows) row.Cells.Add(null);
            if(Table._header != null) Table._header.Cells.Add(null);
            ++Table._columnCount;
          }
          if(Cells[x] == null) {
            Element cell;
            if(Table._header == this) {
              cell = new Element { Tag = Tag.TableHeadCell, Parent = RowElement };
            } else {
              cell = new Element { Tag = Tag.TableCell, Parent = RowElement };
            }
            Cells[x] = cell;
            return cell;
          }
          return Cells[x];
        }
        set {
          SetCell(x, value);
        }
      }
      
      /// <summary>
      /// Create the row for the specified table.
      /// </summary>
      public Row(ElementTable table) {
        Table = table;
        Cells = new ArrayRig<Element>(Table._columnCount);
      }
      
      /// <summary>
      /// Add a cell to this row. If the element doesn't have the 'TableCell' tag, it
      /// is added as the child of a table cell element.
      /// </summary>
      public Element AddCell(Element cell) {
        // does the cell have the table cell tag?
        if(cell.Tag != Tag.TableCell) {
          // no, add a cell element
          var tableCell = new Element(Tag.TableCell);
          cell.Parent = tableCell;
          cell = tableCell;
        }
        Cells.Add(cell);
        if(Table._columnCount < Cells.Count) Table._columnCount = Cells.Count;
        if(cell != null) cell.Parent = RowElement;
        return cell;
      }
      
      /// <summary>
      /// Sets the element in the specified column index.
      /// </summary>
      public void SetCell(int index, Element cell) {
        while(Table._columnCount <= index) {
          foreach(var row in Table._rows) row.Cells.Add(null);
          if(Table._header != null) Table._header.Cells.Add(null);
          ++Table._columnCount;
        }
        var current = Cells[index];
        if(cell == null) {
          if(current != null) {
            current.Remove();
            Cells[index] = null;
          }
          return;
        }
        if(cell.Tag != Tag.TableCell) {
          var tableCell = new Element(Tag.TableCell);
          cell.Parent = tableCell;
          cell = tableCell;
        }
        if(current == null) {
          Cells[index] = cell;
          cell.Parent = RowElement;
        } else {
          current.Replace(cell);
          Cells.Replace(current, cell);
        }
      }
      
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Get or set the row count.
    /// </summary>
    public int RowCount {
      get {
        if(_update) Update();
        return _rows.Count;
      }
      set {
        if(_update) Update();
        while(_rows.Count >= value) _rows.Pop();
        while(_rows.Count <= value) _rows.Add(new Row(this));
      }
    }
    
    /// <summary>
    /// Get or set the header row.
    /// </summary>
    public Row Header {
      get {
        if(_update) Update();
        if(_header == null) _header = new Row(this);
        return _header;
      }
      set {
        if(_update) Update();
        if(_header != null) _header.RowElement.Remove();
        _header = value;
      }
    }
    
    /// <summary>
    /// Get or set the column group row.
    /// </summary>
    public Row ColumnGroup {
      get {
        if(_update) Update();
        if(_columnGroup == null) _columnGroup = new Row(this);
        return _columnGroup;
      }
      set {
        if(_update) Update();
        if(_columnGroup != null) _columnGroup.RowElement.Remove();
        _columnGroup = value;
      }
    }
    
    /// <summary>
    /// Get or set a row by index. Row elements must have the tag 'TableRow'.
    /// </summary>
    public Row this[int y] {
      get {
        if(_update) Update();
        if(_rows.Count <= y) while(_rows.Count <= y) _rows.Add(new Row(this));
        return _rows[y];
      }
      set {
        if(_update) Update();
        if(_rows.Count <= y) while(_rows.Count <= y) _rows.Add(new Row(this));
        if(value == null) _rows.RemoveQuick(y);
        else _rows[y] = value;
      }
    }
    
    /// <summary>
    /// Get or set the cell element. Elements must have the tag 'TableCell'.
    /// </summary>
    public Element this[int x, int y] {
      get {
        if(_update) Update();
        if(_rows.Count <= y) while(_rows.Count <= y) _rows.Add(new Row(this));
        while(_columnCount <= x) {
          foreach(Row row in _rows) row.Cells.Add(null);
          if(_header != null) _header.Cells.Add(null);
          ++_columnCount;
        }
        return _rows[y][x];
      }
      set {
        SetCell(x, y, value);
      }
    }
    
    /// <summary>
    /// Maximum number of columns in the table.
    /// </summary>
    public int Columns {
      get { return _columnCount; }
    }
    
    /// <summary>
    /// Flag that will cause cells in partial rows to be merged with 'colspan' attribute.
    /// </summary>
    public bool MergePartialRows = true;
    
    //----------------------------------//
    
    /// <summary>
    /// Inner reference to the number of column.
    /// </summary>
    protected int _columnCount;
    /// <summary>
    /// Collection of rows.
    /// </summary>
    protected ArrayRig<Row> _rows;
    
    /// <summary>
    /// Inner header reference.
    /// </summary>
    protected Row _header;
    /// <summary>
    /// Row that represents the column group.
    /// </summary>
    protected Row _columnGroup;
    
    /// <summary>
    /// Does the table need to be updated?
    /// </summary>
    protected bool _update;
    /// <summary>
    /// Should the flip of the update tag be skipped?
    /// </summary>
    protected bool _skip;
    
    //----------------------------------//
    
    public ElementTable() {
      _rows = new ArrayRig<Row>();
      Tag = Tag.Table;
      _columnCount = 0;
    }
    
    public ElementTable(params string[] headers) : this() {
      for (int i = 0; i < headers.Length; ++i) {
        Header[i].ContentString = headers[i];
      }
    }
    
    /// <summary>
    /// Create an element table out of the rows and content of the specified element.
    /// </summary>
    public ElementTable(Element element) : this() {
      
      this.Tag = element.Tag;
      if(element._content != null) this._content = new ArrayRig<string>(element.Content);
      if(this._attributes != null) this._attributes = new Dictionary<string, string>(this._attributes);
      if(this._style != null) this._style = new Style(this._style);
      
      if(this.Children != null) {
        // iterate the children of this element
        foreach(var child in this.Children) {
          this.AddChild(child.Clone());
        }
      }
    }
    
    /// <summary>
    /// Clone the element.
    /// </summary>
    public override Element Clone() {
      // create a new element table
      ElementTable clone = new ElementTable();
      
      clone.Tag = this.Tag;
      if(this._content != null) clone._content = new ArrayRig<string>(this._content);
      if(this._attributes != null) clone._attributes = new Dictionary<string, string>(this._attributes);
      if(this._style != null) clone._style = new Style(this._style);
      
      // have the columns been assigned?
      if(this._columnGroup != null) {
        // yes, create the collection
        var columnsRow = new Row(clone);
        clone.ColumnGroup = columnsRow;
        
        // clone the row element
        columnsRow.RowElement = _columnGroup.RowElement.Clone();
        
        // iterate the rows children
        foreach(var cell in _columnGroup.RowElement.Children) {
          if(cell.Tag == Tag.Column) clone.ColumnGroup.Cells.Add(cell);
        }
      }
      
      if(_header != null) {
        // create a new row
        var headerRow = new Row(clone);
        clone.Header = headerRow;
        
        // clone the row element
        headerRow.RowElement = _header.RowElement.Clone();
        
        // iterate the rows children
        foreach(var cell in _header.RowElement.Children) {
          if(cell.Tag == Tag.TableHeadCell) clone.Header.Cells.Add(cell);
        }
        
      }
      
      // iterate the rows
      foreach(var row in _rows) {
        
        // create a new row
        var cloneRow = new Row(clone);
        clone._rows.Add(cloneRow);
        
        // clone the row element
        cloneRow.RowElement = row.RowElement.Clone();
        
        // iterate the rows children
        foreach(var cell in cloneRow.RowElement.Children) {
          if(cell.Tag == Tag.TableCell) cloneRow.Cells.Add(cell);
        }
        
      }
      
      return clone;
    }
    
    /// <summary>
    /// Add a child to this element.
    /// </summary>
    public override void AddChild(Element element) {
      if(_update) Update();
      if(!_skip) {
        if(element.Tag == Tag.TableRow) {
          var row = new Row(this);
          row.RowElement = element;
          
          if(element.Children != null) {
            foreach(var cell in element.Children) {
              if(cell.Tag == Tag.TableCell || cell.Tag == Tag.TableHeadCell) {
                row.Cells.Add(cell);
                if(row.Cells.Count > _columnCount) _columnCount = row.Cells.Count;
              }
            }
          }
          
          if(row.Cells.Count != 0 && row.Cells[0].Tag == Tag.TableHeadCell) _header = row;
          else _rows.Add(row);
          
        } else if(element.Tag == Tag.ColumnGroup) {
          var row = new Row(this);
          row.RowElement = element;
          
          if(element.Children != null) {
            foreach(var cell in element.Children) {
              if(cell.Tag == Tag.Column) {
                row.Cells.Add(cell);
                if(row.Cells.Count > _columnCount) _columnCount = row.Cells.Count;
              }
            }
          }
          
          if(_columnGroup != null) _columnGroup.RowElement.Remove();
          _columnGroup = row;
        }
      }
      base.AddChild(element);
    }
    
    /// <summary>
    /// Add a child at the specified index to this element.
    /// </summary>
    public override void InsertChild(Element element, int index) {
      if(_update) Update();
      if(!_skip) _update = element.Tag == Tag.TableRow || element.Tag == Tag.ColumnGroup;
      base.InsertChild(element, index);
    }
    
    /// <summary>
    /// Remove the specified element from this elements child collection.
    /// </summary>
    public override void RemoveChild(Element element) {
      if(_update) Update();
      if(!_skip) _update = element.Tag == Tag.TableRow || element.Tag == Tag.ColumnGroup;
      base.RemoveChild(element);
    }
    
    /// <summary>
    /// Add a row of elements to the table.
    /// </summary>
    public Row AddRow(params Element[] cells) {
      if(_update) Update();
      // create a new row
      var row = new Row(this);
      
      // iterate the cells to be added to the new row
      foreach(var cell in cells) {
        row.AddCell(cell);
        if(row.Cells.Count > _columnCount) _columnCount = row.Cells.Count;
      }
      
      if(_columnCount < cells.Length) _columnCount = cells.Length;
      
      _rows.Add(row);
      return row;
    }
    
    /// <summary>
    /// Set the element of a cell. The element must have the tag 'TableCell'.
    /// </summary>
    public void SetCell(int x, int y, Element element) {
      if(_update) Update();
      while(_rows.Count <= y) _rows.Add(new Row(this));
      while(_columnCount <= x) {
        foreach(Row row in _rows) row.AddCell(null);
        if(_header != null) _header.AddCell(null);
        ++_columnCount;
      }
      if(element == null && x == _columnCount-1) {
        bool clear = true;
        foreach(var row in _rows) {
          if(row.Cells[x] != null) {
            clear = false;
            break;
          }
        }
        if(clear) {
          foreach(var row in _rows) row.Cells.Pop();
          return;
        }
      }
      _rows[y].SetCell(x, element);
    }
    
    /// <summary>
    /// Add a column to the element table. Returns the new 'col' element.
    /// </summary>
    public Element AddColumn() {
      if(_columnGroup == null) _columnGroup = new Row(this);
      Element column = new Element(Tag.Column);
      _columnGroup.Cells.Add(column);
      _columnGroup.RowElement.AddChild(column);
      if(_columnGroup.Cells.Count > _columnCount) _columnCount = _columnGroup.Cells.Count;
      return column;
    }
    
    /// <summary>
    /// Get the column specified by index.
    /// </summary>
    public Element GetColumn(int index) {
      if(_columnGroup == null) _columnGroup = new Row(this);
      Element column;
      while(_columnGroup.Cells.Count <= index) {
        column = new Element(Tag.Column);
        _columnGroup.Cells.Add(column);
        _columnGroup.RowElement.AddChild(column);
      }
      if(_columnGroup.Cells.Count > _columnCount) _columnCount = _columnGroup.Cells.Count;
      column = _columnGroup[index];
      return column;
    }
    
    /// <summary>
    /// Append the element to the specified string builder.
    /// </summary>
    public override void Build(StringBuilder builder) {
      if(_update) Update();
      
      // open the table tag
      BuildTagOpen(builder);
      
      // has the column group been assigned?
      if(_columnGroup != null) {
        
        // yes, open the column group tag
        _columnGroup.RowElement.Tag = Tag.ColumnGroup;
        _columnGroup.RowElement.BuildTagOpen(builder);
        
        // add the columns
        for(int i = 0; i < _columnGroup.Cells.Count; ++i) {
          // build each column definition
          var cell = _columnGroup.Cells[i];
          if(cell == null) continue;
          
          cell.Tag = Tag.Column;
          cell.Build(builder);
        }
        
        // close the column group tag
        _columnGroup.RowElement.BuildTagClose(builder);
      }
      
      // has the header been assigned?
      if(_header != null) {
        // yes, open the header tag
        _header.RowElement.BuildTagOpen(builder);
        
        // add the header cells
        for(int i = 0; i < _header.Cells.Count; ++i) {
          // build each header cell
          var cell = _header.Cells[i];
          if(cell == null) continue;
          
          if(MergePartialRows && _header.Cells.Count < _columnCount) cell["colspan"] = (_columnCount / _header.Cells.Count).ToString();
          cell.Tag = Tag.TableHeadCell;
          cell.Build(builder);
        }
        
        // close the header tag
        _header.RowElement.BuildTagClose(builder);
      }
      
      // iterate the rows
      foreach(var row in _rows) {
        // open the row tag
        row.RowElement.BuildTagOpen(builder);
        for(int i = 0; i < row.CellCount; ++i) {
          // build each cell
          var cell = row.Cells[i];
          if(cell == null) continue;
          if(MergePartialRows && row.CellCount < _columnCount) cell["colspan"] = (_columnCount / row.CellCount).ToString();
          cell.Build(builder);
        }
        // close the row tag
        row.RowElement.BuildTagClose(builder);
      }
      
      // close the table tag
      BuildTagClose(builder);
      
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Update the structure of the element table.
    /// </summary>
    protected void Update() {
      _update = false;
      
      _rows.Dispose();
      _rows = new ArrayRig<Row>();
      
      // update the row elements
      foreach(var child in Children) {
        if(child.Tag == Tag.TableRow) {
          var row = new Row(this);
          row.RowElement = child;
          
          if(child.Children == null) continue;
          
          foreach(var cell in child.Children) {
            if(cell.Tag == Tag.TableCell || cell.Tag == Tag.TableHeadCell) {
              row.Cells.Add(cell);
              if(row.Cells.Count > _columnCount) _columnCount = row.Cells.Count;
            }
          }
          
          if(row.Cells.Count > 0 &&
            row.Cells[0] != null &&
            row.Cells[0].Tag == Tag.TableHeadCell) {
            Header = row;
          } else {
            _rows.Add(row);
          }
          
        } else if(child.Tag == Tag.ColumnGroup) {
          var row = new Row(this);
          row.RowElement = child;
          
          if(child.Children == null) continue;
          
          foreach(var cell in child.Children) {
            if(cell.Tag == Tag.Column) {
              row.Cells.Add(cell);
              if(row.Cells.Count > _columnCount) _columnCount = row.Cells.Count;
            }
          }
          
          ColumnGroup = row;
        }
      }
      
    }
    
  }
  
}
