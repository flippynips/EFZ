/*
 * User: FloppyNipples
 * Date: 19/02/2017
 * Time: 22:45
 */
using System;

using Efz.Data;

namespace Efz.Cql {
  
  /// <summary>
  /// Blob metadata table.
  /// </summary>
  public class BlobMeta : Table {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Id of the blob metadata row.
    /// </summary>
    [Column(Column.ColumnClass.Primary)]
    public Column<string> Id;
    /// <summary>
    /// Size of the blob in bytes.
    /// </summary>
    [Column(Column.ColumnClass.Data)]
    public Column<long> Length;
    /// <summary>
    /// Number of sections in the blob.
    /// </summary>
    [Column(Column.ColumnClass.Data)]
    public Column<int> SectionCount;
    /// <summary>
    /// Number of bytes in each full section.
    /// </summary>
    [Column(Column.ColumnClass.Data)]
    public Column<int> SectionLength;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Blob stream cache.
    /// </summary>
    protected Cache<string, Meta> _cache;
    
    //-------------------------------------------//
    
    public BlobMeta(string keyspace, long cacheSize) : base(keyspace) {
      _cache = new Cache<string, Meta>(cacheSize);
    }
    
    /// <summary>
    /// Remove a blob meta row by id. The existing rows 'section count' is returned.
    /// </summary>
    public int Remove(string id) {
      // remove and retrieve the metadata by id from the cache if present
      var meta = _cache.RemoveGet(id);
      
      int sectionCount;
      if(meta == null) {
        // get the section count from the table
        var row = Select(SectionCount)
          .Where(Id).EqualTo(id)
          .First();
        
        if(row == null) return 0;
        sectionCount = row.A;
      } else {
        sectionCount = meta.SectionCount;
      }
      
      // delete the entry
      Delete().Where(Id).EqualTo(id).Run();
      
      // return the section count
      return sectionCount;
    }
    
    /// <summary>
    /// Remove a blob meta row by id. The existing rows 'section count' is returned.
    /// </summary>
    public int RemoveAsync(string id) {
      
      // remove and retrieve the metadata by id from the cache if present
      var meta = _cache.RemoveGet(id);
      
      int sectionCount;
      if(meta == null) {
        // get the section count from the table
        var row = Select(SectionCount)
          .Where(Id).EqualTo(id)
          .First();
        
        if(row == null) return 0;
        sectionCount = row.A;
      } else {
        sectionCount = meta.SectionCount;
      }
      
      // delete the entry
      Delete().Where(Id).EqualTo(id).RunAsync();
      
      // return the section count
      return sectionCount;
    }
    
    /// <summary>
    /// Get the details of a blob from a specified id. Returns 'Null' if the
    /// data isn't available.
    /// </summary>
    public Meta Get(string id) {
      // check the cache
      var meta = _cache.Get(id);
      
      // was the metadata retrieved from the cache?
      if(meta == null) {
        // no, get the row
        var row = Select(Length, SectionCount, SectionLength)
          .Where(Id).EqualTo(id)
          .First();
        
        // was the row retrieved? no, return null
        if(row == null) return null;
        
        // return a completed blob specification
        meta = new Meta(row.A, row.B, row.C);
      }
      
      // return the metadata retrieved or existing in the cache
      return _cache.AddOrGet(id, meta);
    }
    
    /// <summary>
    /// Add or update the blob metdata.
    /// </summary>
    public void Set(string id, long length, int sectionCount, int sectionLength) {
      // add the new meta instance to the cache
      _cache.Add(id, new Meta(length, sectionCount, sectionLength));
      // add or update
      Update(Length, SectionCount, SectionLength)
        .With(length, sectionCount, sectionLength)
        .Where(Id).EqualTo(id)
        .RunAsyncNoBatch();
    }
    
    //-------------------------------------------//
    
  }
  
}
