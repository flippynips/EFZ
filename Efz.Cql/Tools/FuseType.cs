/*
 * User: Joshua
 * Date: 17/10/2016
 * Time: 7:35 PM
 */
using System;

using Cassandra;
using Cassandra.Serialization;

namespace Efz.Cql.Entities {
  
  /// <summary>
  /// Base class for implementing serialization and deserialization for the specified type.
  /// </summary>
  public abstract class FuseType<T> : TypeSerializer<T> {
    
    //-------------------------------------------//
    
    /// <summary>
    /// Get Cassandra data type this serializer can be used for.
    /// </summary>
    public override ColumnTypeCode CqlType {
      get {
        throw new NotImplementedException();
      }
    }
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Deserialize a value from the specified buffer.
    /// </summary>
    public unsafe override T Deserialize(ushort protocolVersion, byte[] buffer, int offset, int length, IColumnInfo typeInfo) {
      throw new NotImplementedException();
    }
    
    /// <summary>
    /// Serialize the specified value.
    /// </summary>
    public unsafe override byte[] Serialize(ushort protocolVersion, T value) {
      throw new NotImplementedException();
    }
    
    //-------------------------------------------//
    
  }
  
}
