/*
 * User: Joshua
 * Date: 3/10/2016
 * Time: 3:42 PM
 */
using System;

namespace Efz.Cql {
  
  /// <summary>
  /// Cassandra data types
  /// </summary>
  public enum DataType : byte {
    /// <summary>
    /// No defined data type.
    /// </summary>
    Unknown = 0,
    /// <summary>
    /// strings - US-ASCII character string.
    /// </summary>
    Ascii   = 1,
    /// <summary>
    /// integers - 64-bit signed long.
    /// </summary>
    BigInt  = 2,
    /// <summary>
    /// blobs - Arbitrary bytes (no validation), expressed as hexadecimal.
    /// </summary>
    Blob    = 3,
    /// <summary>
    /// booleans - true or false.
    /// </summary>
    Boolean = 4,
    /// <summary>
    /// integers - Distributed counter value (64-bit long).
    /// </summary>
    Counter = 5,
    /// <summary>
    /// integers, floats - Variable-precision decimal.
    /// </summary>
    Decimal = 6,
    /// <summary>
    /// integers, floats - 64-bit IEEE-754 floating point.
    /// </summary>
    Double  = 7,
    /// <summary>
    /// integers, floats - 32-bit IEEE-754 floating point.
    /// </summary>
    Float   = 8,
    /// <summary>
    /// tuples, collections, user-defined types - A frozen value serializes multiple
    /// components into a single value. Non-frozen types allow updates to individual
    /// fields. Cassandra treats the value of a frozen type as a blob. The entire
    /// value must be overwritten.
    /// e.g. 'frozen <tuple <int, tuple<text, double>>>'
    /// </summary>
    Frozen  = 9,
    /// <summary>
    /// strings - IP address string in IPv4 or IPv6 format, used by the python-cql driver and CQL native protocols.
    /// </summary>
    INet    = 10,
    /// <summary>
    /// integers - 32-bit signed integer.
    /// </summary>
    Int     = 11,
    /// <summary>
    /// n/a - A collection of one or more ordered elements.
    /// </summary>
    List    = 12,
    /// <summary>
    /// n/a - A JSON-style array of literals: { literal : literal, literal : literal ... }
    /// </summary>
    Map     = 13,
    /// <summary>
    /// n/a - A collection of one or more elements.
    /// </summary>
    Set     = 14,
    /// <summary>
    /// strings - UTF-8 encoded string.
    /// </summary>
    Text    = 15,
    /// <summary>
    /// integers, strings - Date plus time, encoded as 8 bytes since epoch.
    /// </summary>
    TimeStamp = 16,
    /// <summary>
    /// uuids - Type 1 UUID only.
    /// </summary>
    TimeUuid  = 17,
    /// <summary>
    /// uuids - A UUID in standard UUID format.
    /// </summary>
    Uuid    = 18,
    /// <summary>
    /// n/a - A group of 2-3 fields.
    /// </summary>
    Tuple   = 19,
    /// <summary>
    /// strings - UTF-8 encoded string/
    /// </summary>
    VarChar = 20,
    /// <summary>
    /// integers - Arbitrary-precision integer/
    /// </summary>
    VarInt  = 21
    
  }
}
