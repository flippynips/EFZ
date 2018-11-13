/*
 * User: Joshua
 * Date: 1/10/2016
 * Time: 2:17 PM
 */
using System;

using Cassandra;

namespace Efz.Cql {
  
  /// <summary>
  /// Represents an row in a data table.
  /// </summary>
  public class Row<TA> : IRow {
    
    //----------------------------------//
    
    public bool Updated { get; set; }
    
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TA A { get { return _a.Value; } set { Updated = true; _a.Value = value; } }
    
    //----------------------------------//
    
    internal Cell<TA> _a;
    
    //----------------------------------//
    
    public void Create(Row row) {
      _a = new Cell<TA>((TA)row[0]);
    }
    
    /// <summary>
    /// Get the values of the row.
    /// </summary>
    public void GetValues(object[] values) {
      values[0] = _a.InnerValue;
    }
    /// <summary>
    /// Get the flags indicating the updated value indexes.
    /// </summary>
    public void GetUpdated(bool[] updated) {
      updated[0] = _a.Updated;
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Represents an row in a data table.
  /// </summary>
  public class Row<TA,TB> : IRow {
    
    //----------------------------------//
    
    public bool Updated { get; set; }
    
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TA A { get { return _a.Value; } set { Updated = true; _a.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TB B { get { return _b.Value; } set { Updated = true; _b.Value = value; } }
    
    //----------------------------------//
    
    internal Cell<TA> _a;
    internal Cell<TB> _b;
    
    //----------------------------------//
    
    public void Create(Row row) {
      _a = new Cell<TA>((TA)row[0]);
      _b = new Cell<TB>((TB)row[1]);
    }
    
    /// <summary>
    /// Get the values of the row.
    /// </summary>
    public void GetValues(object[] values) {
      values[0] = _a.InnerValue;
      values[1] = _b.InnerValue;
    }
    /// <summary>
    /// Get the flags indicating the updated value indexes.
    /// </summary>
    public void GetUpdated(bool[] updated) {
      updated[0] = _a.Updated;
      updated[1] = _b.Updated;
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Represents an row in a data table.
  /// </summary>
  public class Row<TA,TB,TC> : IRow {
    
    //----------------------------------//
    
    public bool Updated { get; set; }
    
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TA A { get { return _a.Value; } set { Updated = true; _a.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TB B { get { return _b.Value; } set { Updated = true; _b.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TC C { get { return _c.Value; } set { Updated = true; _c.Value = value; } }
    
    //----------------------------------//
    
    internal Cell<TA> _a;
    internal Cell<TB> _b;
    internal Cell<TC> _c;
    
    //----------------------------------//
    
    public void Create(Row row) {
      _a = new Cell<TA>((TA)row[0]);
      _b = new Cell<TB>((TB)row[1]);
      _c = new Cell<TC>((TC)row[2]);
    }
    
    /// <summary>
    /// Get the values of the row.
    /// </summary>
    public void GetValues(object[] values) {
      values[0] = _a.InnerValue;
      values[1] = _b.InnerValue;
      values[2] = _c.InnerValue;
    }
    /// <summary>
    /// Get the flags indicating the updated value indexes.
    /// </summary>
    public void GetUpdated(bool[] updated) {
      updated[0] = _a.Updated;
      updated[1] = _b.Updated;
      updated[2] = _c.Updated;
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Represents an row in a data table.
  /// </summary>
  public class Row<TA,TB,TC,TD> : IRow {
    
    //----------------------------------//
    
    public bool Updated { get; set; }
    
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TA A { get { return _a.Value; } set { Updated = true; _a.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TB B { get { return _b.Value; } set { Updated = true; _b.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TC C { get { return _c.Value; } set { Updated = true; _c.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TD D { get { return _d.Value; } set { Updated = true; _d.Value = value; } }
    
    //----------------------------------//
    
    internal Cell<TA> _a;
    internal Cell<TB> _b;
    internal Cell<TC> _c;
    internal Cell<TD> _d;
    
    //----------------------------------//
    
    public void Create(Row row) {
      _a = new Cell<TA>((TA)row[0]);
      _b = new Cell<TB>((TB)row[1]);
      _c = new Cell<TC>((TC)row[2]);
      _d = new Cell<TD>((TD)row[3]);
    }
    
    /// <summary>
    /// Get the values of the row.
    /// </summary>
    public void GetValues(object[] values) {
      values[0] = _a.InnerValue;
      values[1] = _b.InnerValue;
      values[2] = _c.InnerValue;
      values[3] = _d.InnerValue;
    }
    /// <summary>
    /// Get the flags indicating the updated value indexes.
    /// </summary>
    public void GetUpdated(bool[] updated) {
      updated[0] = _a.Updated;
      updated[1] = _b.Updated;
      updated[2] = _c.Updated;
      updated[3] = _d.Updated;
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Represents an row in a data table.
  /// </summary>
  public class Row<TA,TB,TC,TD,TE> : IRow {
    
    //----------------------------------//
    
    public bool Updated { get; set; }
    
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TA A { get { return _a.Value; } set { Updated = true; _a.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TB B { get { return _b.Value; } set { Updated = true; _b.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TC C { get { return _c.Value; } set { Updated = true; _c.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TD D { get { return _d.Value; } set { Updated = true; _d.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TE E { get { return _e.Value; } set { Updated = true; _e.Value = value; } }
    
    //----------------------------------//
    
    internal Cell<TA> _a;
    internal Cell<TB> _b;
    internal Cell<TC> _c;
    internal Cell<TD> _d;
    internal Cell<TE> _e;
    
    //----------------------------------//
    
    public void Create(Row row) {
      _a = new Cell<TA>((TA)row[0]);
      _b = new Cell<TB>((TB)row[1]);
      _c = new Cell<TC>((TC)row[2]);
      _d = new Cell<TD>((TD)row[3]);
      _e = new Cell<TE>((TE)row[4]);
    }
    
    /// <summary>
    /// Get the values of the row.
    /// </summary>
    public void GetValues(object[] values) {
      values[0] = _a.InnerValue;
      values[1] = _b.InnerValue;
      values[2] = _c.InnerValue;
      values[3] = _d.InnerValue;
      values[4] = _e.InnerValue;
    }
    /// <summary>
    /// Get the flags indicating the updated value indexes.
    /// </summary>
    public void GetUpdated(bool[] updated) {
      updated[0] = _a.Updated;
      updated[1] = _b.Updated;
      updated[2] = _c.Updated;
      updated[3] = _d.Updated;
      updated[4] = _e.Updated;
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Represents an row in a data table.
  /// </summary>
  public class Row<TA,TB,TC,TD,TE,TF> : IRow {
    
    //----------------------------------//
    
    public bool Updated { get; set; }
    
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TA A { get { return _a.Value; } set { Updated = true; _a.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TB B { get { return _b.Value; } set { Updated = true; _b.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TC C { get { return _c.Value; } set { Updated = true; _c.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TD D { get { return _d.Value; } set { Updated = true; _d.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TE E { get { return _e.Value; } set { Updated = true; _e.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TF F { get { return _f.Value; } set { Updated = true; _f.Value = value; } }
    
    //----------------------------------//
    
    internal Cell<TA> _a;
    internal Cell<TB> _b;
    internal Cell<TC> _c;
    internal Cell<TD> _d;
    internal Cell<TE> _e;
    internal Cell<TF> _f;
    
    //----------------------------------//
    
    public void Create(Row row) {
      _a = new Cell<TA>((TA)row[0]);
      _b = new Cell<TB>((TB)row[1]);
      _c = new Cell<TC>((TC)row[2]);
      _d = new Cell<TD>((TD)row[3]);
      _e = new Cell<TE>((TE)row[4]);
      _f = new Cell<TF>((TF)row[5]);
    }
    
    /// <summary>
    /// Get the values of the row.
    /// </summary>
    public void GetValues(object[] values) {
      values[0] = _a.InnerValue;
      values[1] = _b.InnerValue;
      values[2] = _c.InnerValue;
      values[3] = _d.InnerValue;
      values[4] = _e.InnerValue;
      values[5] = _f.InnerValue;
    }
    /// <summary>
    /// Get the flags indicating the updated value indexes.
    /// </summary>
    public void GetUpdated(bool[] updated) {
      updated[0] = _a.Updated;
      updated[1] = _b.Updated;
      updated[2] = _c.Updated;
      updated[3] = _d.Updated;
      updated[4] = _e.Updated;
      updated[5] = _f.Updated;
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Represents an row in a data table.
  /// </summary>
  public class Row<TA,TB,TC,TD,TE,TF,TG> : IRow {
    
    //----------------------------------//
    
    public bool Updated { get; set; }
    
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TA A { get { return _a.Value; } set { Updated = true; _a.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TB B { get { return _b.Value; } set { Updated = true; _b.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TC C { get { return _c.Value; } set { Updated = true; _c.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TD D { get { return _d.Value; } set { Updated = true; _d.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TE E { get { return _e.Value; } set { Updated = true; _e.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TF F { get { return _f.Value; } set { Updated = true; _f.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TG G { get { return _g.Value; } set { Updated = true; _g.Value = value; } }
    
    //----------------------------------//
    
    internal Cell<TA> _a;
    internal Cell<TB> _b;
    internal Cell<TC> _c;
    internal Cell<TD> _d;
    internal Cell<TE> _e;
    internal Cell<TF> _f;
    internal Cell<TG> _g;
    
    //----------------------------------//
    
    public void Create(Row row) {
      _a = new Cell<TA>((TA)row[0]);
      _b = new Cell<TB>((TB)row[1]);
      _c = new Cell<TC>((TC)row[2]);
      _d = new Cell<TD>((TD)row[3]);
      _e = new Cell<TE>((TE)row[4]);
      _f = new Cell<TF>((TF)row[5]);
      _g = new Cell<TG>((TG)row[6]);
    }
    
    /// <summary>
    /// Get the values of the row.
    /// </summary>
    public void GetValues(object[] values) {
      values[0] = _a.InnerValue;
      values[1] = _b.InnerValue;
      values[2] = _c.InnerValue;
      values[3] = _d.InnerValue;
      values[4] = _e.InnerValue;
      values[5] = _f.InnerValue;
      values[6] = _g.InnerValue;
    }
    /// <summary>
    /// Get the flags indicating the updated value indexes.
    /// </summary>
    public void GetUpdated(bool[] updated) {
      updated[0] = _a.Updated;
      updated[1] = _b.Updated;
      updated[2] = _c.Updated;
      updated[3] = _d.Updated;
      updated[4] = _e.Updated;
      updated[5] = _f.Updated;
      updated[6] = _g.Updated;
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Represents an row in a data table.
  /// </summary>
  public class Row<TA,TB,TC,TD,TE,TF,TG,TH> : IRow {
    
    //----------------------------------//
    
    public bool Updated { get; set; }
    
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TA A { get { return _a.Value; } set { Updated = true; _a.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TB B { get { return _b.Value; } set { Updated = true; _b.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TC C { get { return _c.Value; } set { Updated = true; _c.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TD D { get { return _d.Value; } set { Updated = true; _d.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TE E { get { return _e.Value; } set { Updated = true; _e.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TF F { get { return _f.Value; } set { Updated = true; _f.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TG G { get { return _g.Value; } set { Updated = true; _g.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TH H { get { return _h.Value; } set { Updated = true; _h.Value = value; } }
    
    //----------------------------------//
    
    internal Cell<TA> _a;
    internal Cell<TB> _b;
    internal Cell<TC> _c;
    internal Cell<TD> _d;
    internal Cell<TE> _e;
    internal Cell<TF> _f;
    internal Cell<TG> _g;
    internal Cell<TH> _h;
    
    //----------------------------------//
    
    public void Create(Row row) {
      _a = new Cell<TA>((TA)row[0]);
      _b = new Cell<TB>((TB)row[1]);
      _c = new Cell<TC>((TC)row[2]);
      _d = new Cell<TD>((TD)row[3]);
      _e = new Cell<TE>((TE)row[4]);
      _f = new Cell<TF>((TF)row[5]);
      _g = new Cell<TG>((TG)row[6]);
      _h = new Cell<TH>((TH)row[7]);
    }
    
    /// <summary>
    /// Get the values of the row.
    /// </summary>
    public void GetValues(object[] values) {
      values[0] = _a.InnerValue;
      values[1] = _b.InnerValue;
      values[2] = _c.InnerValue;
      values[3] = _d.InnerValue;
      values[4] = _e.InnerValue;
      values[5] = _f.InnerValue;
      values[6] = _g.InnerValue;
      values[7] = _h.InnerValue;
    }
    /// <summary>
    /// Get the flags indicating the updated value indexes.
    /// </summary>
    public void GetUpdated(bool[] updated) {
      updated[0] = _a.Updated;
      updated[1] = _b.Updated;
      updated[2] = _c.Updated;
      updated[3] = _d.Updated;
      updated[4] = _e.Updated;
      updated[5] = _f.Updated;
      updated[6] = _g.Updated;
      updated[7] = _h.Updated;
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Represents an row in a data table.
  /// </summary>
  public class Row<TA,TB,TC,TD,TE,TF,TG,TH,TI> : IRow {
    
    //----------------------------------//
    
    public bool Updated { get; set; }
    
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TA A { get { return _a.Value; } set { Updated = true; _a.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TB B { get { return _b.Value; } set { Updated = true; _b.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TC C { get { return _c.Value; } set { Updated = true; _c.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TD D { get { return _d.Value; } set { Updated = true; _d.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TE E { get { return _e.Value; } set { Updated = true; _e.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TF F { get { return _f.Value; } set { Updated = true; _f.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TG G { get { return _g.Value; } set { Updated = true; _g.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TH H { get { return _h.Value; } set { Updated = true; _h.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TI I { get { return _i.Value; } set { Updated = true; _i.Value = value; } }
    
    //----------------------------------//
    
    internal Cell<TA> _a;
    internal Cell<TB> _b;
    internal Cell<TC> _c;
    internal Cell<TD> _d;
    internal Cell<TE> _e;
    internal Cell<TF> _f;
    internal Cell<TG> _g;
    internal Cell<TH> _h;
    internal Cell<TI> _i;
    
    //----------------------------------//
    
    public void Create(Row row) {
      _a = new Cell<TA>((TA)row[0]);
      _b = new Cell<TB>((TB)row[1]);
      _c = new Cell<TC>((TC)row[2]);
      _d = new Cell<TD>((TD)row[3]);
      _e = new Cell<TE>((TE)row[4]);
      _f = new Cell<TF>((TF)row[5]);
      _g = new Cell<TG>((TG)row[6]);
      _h = new Cell<TH>((TH)row[7]);
      _i = new Cell<TI>((TI)row[8]);
    }
    
    /// <summary>
    /// Get the values of the row.
    /// </summary>
    public void GetValues(object[] values) {
      values[0] = _a.InnerValue;
      values[1] = _b.InnerValue;
      values[2] = _c.InnerValue;
      values[3] = _d.InnerValue;
      values[4] = _e.InnerValue;
      values[5] = _f.InnerValue;
      values[6] = _g.InnerValue;
      values[7] = _h.InnerValue;
      values[8] = _i.InnerValue;
    }
    /// <summary>
    /// Get the flags indicating the updated value indexes.
    /// </summary>
    public void GetUpdated(bool[] updated) {
      updated[0] = _a.Updated;
      updated[1] = _b.Updated;
      updated[2] = _c.Updated;
      updated[3] = _d.Updated;
      updated[4] = _e.Updated;
      updated[5] = _f.Updated;
      updated[6] = _g.Updated;
      updated[7] = _h.Updated;
      updated[8] = _i.Updated;
    }
    
    //----------------------------------//
    
  }
  
  /// <summary>
  /// Represents an row in a data table.
  /// </summary>
  public class Row<TA,TB,TC,TD,TE,TF,TG,TH,TI,TJ> : IRow {
    
    //----------------------------------//
    
    public bool Updated { get; set; }
    
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TA A { get { return _a.Value; } set { Updated = true; _a.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TB B { get { return _b.Value; } set { Updated = true; _b.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TC C { get { return _c.Value; } set { Updated = true; _c.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TD D { get { return _d.Value; } set { Updated = true; _d.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TE E { get { return _e.Value; } set { Updated = true; _e.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TF F { get { return _f.Value; } set { Updated = true; _f.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TG G { get { return _g.Value; } set { Updated = true; _g.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TH H { get { return _h.Value; } set { Updated = true; _h.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TI I { get { return _i.Value; } set { Updated = true; _i.Value = value; } }
    /// <summary>
    /// A row cell value.
    /// </summary>
    public TJ J { get { return _j.Value; } set { Updated = true; _j.Value = value; } }
    
    //----------------------------------//
    
    internal Cell<TA> _a;
    internal Cell<TB> _b;
    internal Cell<TC> _c;
    internal Cell<TD> _d;
    internal Cell<TE> _e;
    internal Cell<TF> _f;
    internal Cell<TG> _g;
    internal Cell<TH> _h;
    internal Cell<TI> _i;
    internal Cell<TJ> _j;
    
    //----------------------------------//
    
    public void Create(Row row) {
      _a = new Cell<TA>((TA)row[0]);
      _b = new Cell<TB>((TB)row[1]);
      _c = new Cell<TC>((TC)row[2]);
      _d = new Cell<TD>((TD)row[3]);
      _e = new Cell<TE>((TE)row[4]);
      _f = new Cell<TF>((TF)row[5]);
      _g = new Cell<TG>((TG)row[6]);
      _h = new Cell<TH>((TH)row[7]);
      _i = new Cell<TI>((TI)row[8]);
      _j = new Cell<TJ>((TJ)row[9]);
    }
    
    /// <summary>
    /// Get the values of the row.
    /// </summary>
    public void GetValues(object[] values) {
      values[0] = _a.InnerValue;
      values[1] = _b.InnerValue;
      values[2] = _c.InnerValue;
      values[3] = _d.InnerValue;
      values[4] = _e.InnerValue;
      values[5] = _f.InnerValue;
      values[6] = _g.InnerValue;
      values[7] = _h.InnerValue;
      values[8] = _i.InnerValue;
      values[9] = _j.InnerValue;
    }
    /// <summary>
    /// Get the flags indicating the updated value indexes.
    /// </summary>
    public void GetUpdated(bool[] updated) {
      updated[0] = _a.Updated;
      updated[1] = _b.Updated;
      updated[2] = _c.Updated;
      updated[3] = _d.Updated;
      updated[4] = _e.Updated;
      updated[5] = _f.Updated;
      updated[6] = _g.Updated;
      updated[7] = _h.Updated;
      updated[8] = _i.Updated;
      updated[9] = _j.Updated;
    }
    
    //----------------------------------//
    
  }
  
}
