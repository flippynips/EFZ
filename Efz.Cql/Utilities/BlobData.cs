/*
 * User: FloppyNipples
 * Date: 19/02/2017
 * Time: 22:45
 */
using System;
using Efz.Data;

namespace Efz.Cql {
  
  /// <summary>
  /// Blob data table.
  /// </summary>
  public class BlobData : Table {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Id of the blob data row.
    /// </summary>
    [Column(Column.ColumnClass.Primary)]
    public Column<string> Id;
    /// <summary>
    /// Index of the blob data row.
    /// </summary>
    [Column(Column.ColumnClass.Primary)]
    public Column<int> SectionIndex;
    /// <summary>
    /// Collection of bytes associated with the row.
    /// </summary>
    [Column(Column.ColumnClass.Data)]
    public Column<byte[]> Bytes;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Blob stream cache.
    /// </summary>
    protected Cache<Struct<string, int>, byte[]> _cache;
    
    //-------------------------------------------//
    
    public BlobData(string keyspace, long size) : base(keyspace) {
      _cache = new Cache<Struct<string, int>, byte[]>(size, m => m.Length);
    }
    
    /// <summary>
    /// Remove 'count' data rows associated with the specified id.
    /// </summary>
    public void Remove(string id, int count) {
      while(--count >= 0) {
        // remove the sections from the cache
        _cache.Remove(new Struct<string, int>(id, count));
        // remove the section rows
        Delete().Where(Id).EqualTo(id)
          .Where(SectionIndex).EqualTo(count)
          .Run();
      }
    }
    
    /// <summary>
    /// Remove 'count' data rows associated with the specified id.
    /// </summary>
    public void RemoveAsync(string id, int count) {
      while(--count >= 0) {
        // remove the sections from the cache
        _cache.Remove(new Struct<string, int>(id, count));
        // remove the section rows
        Delete().Where(Id).EqualTo(id)
          .Where(SectionIndex).EqualTo(count)
          .RunAsync();
      }
    }
    
    /// <summary>
    /// Get a blob data row by id and index.
    /// </summary>
    public byte[] Get(string id, int index) {
      
      var cacheId = new Struct<string, int>(id, index);
      // get from the cache
      var data = _cache.Get(cacheId);
      
      // was the data retrieved from the cache?
      if(data == null) {
        // no, get the row from the table
        data = Select(Bytes)
          .Where(Id).EqualTo(id)
          .Where(SectionIndex).EqualTo(index)
          .First().A;
        
        // were the bytes found? no, return null
        if(data == null) return null;
      }
      
      // add the retrieved data to the cache
      return _cache.AddOrGet(cacheId, data);
    }
    
    /// <summary>
    /// Add or update a blob data row.
    /// </summary>
    public void Set(string id, int index, byte[] bytes) {
      // add to the cache
      _cache.Add(new Struct<string, int>(id, index), bytes);
      // update the table
      Update(Bytes).With(bytes)
        .Where(Id).EqualTo(id)
        .Where(SectionIndex).EqualTo(index)
        .RunAsyncNoBatch();
    }
    
    //-------------------------------------------//
    
  }
  
}
