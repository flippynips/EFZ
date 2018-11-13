/*
 * User: FloppyNipples
 * Date: 19/02/2017
 * Time: 22:45
 */
using System;

namespace Efz.Cql {
  
  /// <summary>
  /// Blob management.
  /// </summary>
  public class Blobs {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Blob metadata table.
    /// </summary>
    public BlobMeta BlobMeta;
    /// <summary>
    /// Blob data table.
    /// </summary>
    public BlobData BlobData;
    
    /// <summary>
    /// Default section size for new blobs.
    /// </summary>
    public long DefaultSectionSize = Global.Megabyte / 2;
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    public Blobs(string keyspace, long cacheSize) {
      BlobMeta = new BlobMeta(keyspace, (cacheSize / DefaultSectionSize) / 2);
      BlobData = new BlobData(keyspace, cacheSize);
    }
    
    /// <summary>
    /// Change the key of a blob. Costly as it copies all data into new rows.
    /// </summary>
    public void ChangeKey(string currentKey, string newKey) {
      // get the blob metadata
      var details = BlobMeta.Get(currentKey);
      
      // was the row found? no, return
      if(details == null) return;
      
      // remove the current metadata
      BlobMeta.Remove(currentKey);
      
      // copy all sections
      for(int i = 0; i < details.SectionCount; ++i) {
        var bytes = BlobData.Get(currentKey, i);
        BlobData.Set(newKey, i, bytes);
      }
      
      // remove previous data
      BlobData.Remove(currentKey, details.SectionCount);
      // add the new metadata
      BlobMeta.Set(newKey, details.Length, details.SectionCount, details.SectionLength);
      
    }
    
    /// <summary>
    /// Get a blob stream for the specified id.
    /// </summary>
    public BlobStream GetNewStream(string id) {
      return new BlobStream(id, this);
    }
    
    /// <summary>
    /// Get a blob stream for the specified id. Will return 'Null' if the id
    /// cannot be found.
    /// </summary>
    public BlobStream GetExistingStream(string id) {
      // get the blob metadata
      var details = BlobMeta.Get(id);
      
      // was the row found? no, return null
      if(details == null) return null;
      
      // return the a new blob stream
      return new BlobStream(id, this, details.Length, details.SectionCount, details.SectionLength);
    }
    
    /// <summary>
    /// Remove an existing blob.
    /// </summary>
    public void RemoveAsync(string id) {
      var sectionCount = BlobMeta.RemoveAsync(id);
      BlobData.RemoveAsync(id, sectionCount);
    }
    
    /// <summary>
    /// Remove an existing blob.
    /// </summary>
    public void Remove(string id) {
      var sectionCount = BlobMeta.Remove(id);
      BlobData.Remove(id, sectionCount);
    }
    
    //-------------------------------------------//
    
  }
  
}
