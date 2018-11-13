/*
 * User: FloppyNipples
 * Date: 27/04/2017
 * Time: 23:09
 */
using System;
using System.IO;
using Efz.Threading;


namespace Efz.Data {
  
  /// <summary>
  /// Writer of csv file.
  /// </summary>
  public class CsvWriter : IDisposable {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Stream to the file.
    /// </summary>
    private readonly FileStream _stream;
    /// <summary>
    /// Stream writer.
    /// </summary>
    private readonly ByteBuffer _writer;
    
    /// <summary>
    /// Path to the csv to be written.
    /// </summary>
    private readonly string _path;
    
    /// <summary>
    /// Lock used for concurrency.
    /// </summary>
    private readonly Lock _lock;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a new template.
    /// </summary>
    public CsvWriter(string path, params string[] titles) {
      if(titles.Length == 0) throw new InvalidOperationException("No titles in csv file.");
      
      _path = path;
      _lock = new Lock();
      
      // get a path to the csv file
      _stream = new FileStream(_path, FileMode.Create, FileAccess.Write, FileShare.Read);
      _writer = new ByteBuffer(_stream);
      
      bool first = true;
      foreach(var title in titles) {
        if(first) first = false;
        else _writer.Write(Chars.Comma);
        _writer.Write(title);
      }
      _writer.Write(Chars.Comma);
      
    }
    
    /// <summary>
    /// Dispose of the csv writer and underlying stream.
    /// </summary>
    public void Dispose() {
      _writer.Dispose();
      _stream.Dispose();
    }
    
    /// <summary>
    /// Add a record to the csv file.
    /// </summary>
    public void AddRow(params string[] cells) {
      bool first = true;
      _lock.Take();
      foreach(var cell in cells) {
        if(first) first = false;
        else _writer.Write(Chars.Comma);
        if(cell.Contains(Chars.Comma)) {
          _writer.Write(Chars.DoubleQuote);
          _writer.Write(cell);
          _writer.Write(Chars.DoubleQuote);
        } else {
          _writer.Write(cell);
        }
      }
      _writer.Write(Chars.NewLine);
      _lock.Release();
    }
    
    //-------------------------------------------//
    
  }
  
}
