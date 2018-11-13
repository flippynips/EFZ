/*
 * User: Joshua
 * Date: 29/09/2016
 * Time: 9:02 PM
 */
using System;
using System.Collections.Generic;

using System.Text;
using Efz.Collections;
using Efz.Tools;

namespace Efz.Cql {
  
  /// <summary>
  /// Common helper methods and constants.
  /// </summary>
  public static class Common {
    
    //----------------------------------//
    
    internal const string Select           = "SELECT ";
    internal const string Update           = "UPDATE ";
    internal const string Insert           = "INSERT INTO ";
    internal const string Delete           = "DELETE ";
    
    internal const string And              = " AND ";
    internal const string Where            = " WHERE ";
    internal const string If               = " IF ";
    internal const string From             = " FROM ";
    internal const string As               = " AS ";
    internal const string In               = " IN ";
    internal const string Set              = " SET ";
    
    internal const string Distinct         = "DISTINCT ";
    internal const string OrderBy          = " ORDER BY ";
    internal const string Limit            = " LIMIT ";
    internal const string Decending        = " DESC";
    internal const string Values           = ") VALUES (";
    
    internal const string Token            = "TOKEN(";
    internal const string Contains         = " CONTAINS ";
    internal const string ContainsKey      = " CONTAINS KEY ";
    internal const string WriteTime        = " WRITETIME ";
    internal const string TimeToLive       = " TTL ";
    internal const string AllowFiltering   = " ALLOW FILTERING";
    
    internal const string Equal            = "=";
    internal const string NotEqual         = "!=";
    internal const string LessThan         = "<";
    internal const string GreaterThan      = ">";
    internal const string LessEqualThan    = "<=";
    internal const string GreaterEqualThan = ">=";
    
    internal const string Count            = "COUNT(*)";
    
    internal const string Colon            = " : ";
    
    internal const string SetOpen          = "{";
    internal const string SetClose         = "}";
    internal const string ListOpen         = "[";
    internal const string ListClose        = "]";
    
    private static WeakReferencer<Dictionary<Type, DataType>> typeMap;
    internal static Dictionary<Type, DataType> TypeMap {
      get {
        if(typeMap == null) typeMap = new WeakReferencer<Dictionary<Type, DataType>>(GetTypeMap);
        return typeMap.Item;
      }
    }
    
    //----------------------------------//
    
    /// <summary>
    /// Get the equivalent Cassandra data type for the specified net type.
    /// </summary>
    public static DataType GetDataType(Type type) {
      // get the data type
      DataType dataType;
      if(TypeMap.TryGetValue(type, out dataType)) return dataType;
      
      // it's a user defined type or tuple
      return DataType.Frozen;
    }
    
    /// <summary>
    /// Get the equivalent Cassandra data type for the specified net type.
    /// </summary>
    public static string GetDataTypeString(Type type) {
      
      // get the data type
      DataType dataType;
      Type baseType;
      
      if(type.IsGenericType && !type.IsGenericTypeDefinition) baseType = type.GetGenericTypeDefinition();
      else baseType = type;
      
      if(TypeMap.TryGetValue(baseType, out dataType)) {
        switch(dataType) {
          case DataType.List:
            return "LIST<" + GetDataTypeString(type.GetGenericArguments()[0]) + Chars.GreaterThan;
          case DataType.Set:
            return "SET<" + GetDataTypeString(type.GetGenericArguments()[0]) + Chars.GreaterThan;
          case DataType.Map:
            var args = type.GetGenericArguments();
            if(args.Length != 2) throw new ArgumentException("Invalid number of arguments for MAP type.");
            return "MAP<" + GetDataTypeString(args[0]) + Chars.Comma + GetDataTypeString(args[1]) + Chars.GreaterThan;
          case DataType.Tuple:
            StringBuilder sb = StringBuilderCache.Get();
            sb.Append("TUPLE<");
            bool first = true;
            foreach(Type genType in type.GetGenericArguments()) {
              if(first) first = false;
              else sb.Append(Chars.Comma);
              sb.Append(GetDataTypeString(genType));
            }
            sb.Append(Chars.GreaterThan);
            return StringBuilderCache.SetAndGet(sb);
          default:
            return dataType.ToString().ToLowercase();
        }
      }
      
      // it's a user defined type or tuple
      return "blob";
    }
    
    /*
    /// <summary>
    /// Collection of keywords reserved by Cassandra.
    /// </summary>
    public static readonly string[] Reserved = {
      "ADD",
      "ALLOW",
      "ALTER",
      "AND",
      "ANY",
      "APPLY",
      "ASC",
      "AUTHORIZE",
      "BATCH",
      "BEGIN",
      "BY",
      "COLUMNFAMILY",
      "CREATE",
      "DELETE",
      "DESC",
      "DROP",
      "EACH_QUORUM",
      "FROM",
      "FULL",
      "GRANT",
      "IF",
      "IN",
      "INDEX",
      "INET",
      "INFINITY",
      "INSERT",
      "INTO",
      "KEYSPACE",
      "KEYSPACES",
      "LIMIT",
      "LOCAL_ONE",
      "LOCAL_QUORUM",
      "MODIFY",
      "NAN",
      "NORECURSIVE",
      "NOT",
      "OF",
      "ON",
      "ONE",
      "ORDER",
      "PASSWORD",
      "PRIMARY",
      "QUORUM",
      "RENAME",
      "REVOKE",
      "SCHEMA",
      "SELECT",
      "SET",
      "TABLE",
      "THREE",
      "TO",
      "TOKEN",
      "TRUNCATE",
      "TWO",
      "UNLOGGED",
      "UPDATE",
      "USE",
      "USING",
      "WHERE",
      "WITH"
    };
    */
    
    //----------------------------------//
    
    private static Dictionary<Type, DataType> GetTypeMap() {
      return new Dictionary<Type, DataType> {
        { typeof(int), DataType.Int },
        { typeof(long), DataType.BigInt },
        { typeof(float), DataType.Float },
        { typeof(double), DataType.Double },
        { typeof(string), DataType.Text },
        { typeof(byte[]), DataType.Blob },
        { typeof(Decimal), DataType.Decimal },
        { typeof(bool), DataType.Boolean },
        { typeof(DateTimeOffset), DataType.TimeStamp },
        { typeof(IEnumerable<>), DataType.Set },
        { typeof(ArrayRig<>), DataType.Set },
        { typeof(IDictionary<,>), DataType.Map },
        { typeof(Array), DataType.List },
        { typeof(SortedList<,>), DataType.List },
        { typeof(SortedRig<,>), DataType.List },
        { typeof(Tuple<>), DataType.Tuple },
        { typeof(Tuple<,>), DataType.Tuple },
        { typeof(Tuple<,,>), DataType.Tuple },
        { typeof(Tuple<,,,>), DataType.Tuple },
        { typeof(Tuple<,,,,>), DataType.Tuple },
        { typeof(Tuple<,,,,,>), DataType.Tuple },
        { typeof(Tuple<,,,,,,>), DataType.Tuple },
        { typeof(Tuple<,,,,,,,>), DataType.Tuple },
        { typeof(IArgs<>), DataType.Tuple },
        { typeof(IArgs<,>), DataType.Tuple },
        { typeof(IArgs<,,>), DataType.Tuple },
        { typeof(IArgs<,,,>), DataType.Tuple },
        { typeof(IArgs<,,,,>), DataType.Tuple },
        { typeof(IArgs<,,,,,>), DataType.Tuple },
        { typeof(IArgs<,,,,,,>), DataType.Tuple },
        { typeof(IArgs<,,,,,,,>), DataType.Tuple },
        { typeof(IArgs<,,,,,,,,>), DataType.Tuple },
        { typeof(IArgs<,,,,,,,,,>), DataType.Tuple }
      };
    }
    
  }
}
