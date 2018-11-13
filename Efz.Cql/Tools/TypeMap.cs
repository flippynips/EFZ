/*
 * User: Joshua
 * Date: 25/09/2016
 * Time: 9:15 PM
 */
using System;
using System.Reflection;

using Cassandra;

namespace Efz.Cql {
  
  /// <summary>
  /// Generic implementation of an entity that can be mapped to rows or cells in a Cassandra Table.
  /// </summary>
  public class TypeMap<TEntity> : UdtMap {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    private FuncSet<TEntity> activator;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a table entity for the specified generic type.
    /// </summary>
    public TypeMap(Type type = null) : base(type ?? typeof(TEntity), (type ?? typeof(TEntity)).Name) {
      
      // iterate through the properties of the table entity
      foreach(PropertyInfo info in this.NetType.GetProperties(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)) {
        this.AddPropertyMapping(info, info.Name);
      }
      
      // create the activator for the cell type defined
      activator = Dynamic.Constructor<FuncSet<TEntity>>(type);
    }
    
    //-------------------------------------------//
    
    protected override void Automap() {
      if (this.Definition == null) {
        throw new ArgumentException("Udt definition not specified");
      }
      foreach (ColumnDesc current in this.Definition.Fields) {
        PropertyInfo property = this.NetType.GetProperty(current.Name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        if (property != null) {
          this.AddPropertyMapping(property, current.Name);
        }
      }
    }
    
    protected override void Build(UdtColumnInfo definition) {
      this.Definition = definition;
      if (this._fieldNameToProperty.Count == 0) {
        this.Automap();
      }
    }
    
    protected override object CreateInstance() {
      return activator.Run();
    }
    
  }
  
  /// <summary>
  /// Non-generic implementation of an entity that can be mapped to rows or cells in a Cassandra Table.
  /// </summary>
  public class TypeMap : UdtMap {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    /// <summary>
    /// Activator for the type this map defines.
    /// </summary>
    private IFunc<object> _activator;
    
    //-------------------------------------------//
    
    /// <summary>
    /// Initialize a table entity for the specified generic type.
    /// </summary>
    public TypeMap(Type type) : base(type, type.Name) {
      // iterate through the properties of the entity
      foreach(PropertyInfo info in NetType.GetProperties(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)) {
        if(info.CanRead && info.CanWrite) {
          this.AddPropertyMapping(info, info.Name);
        }
      }
      
      _activator = Dynamic.Constructor<IFunc<object>>(type);
    }
    
    //-------------------------------------------//
    
    protected override void Automap() {
      if (this.Definition == null) {
        throw new ArgumentException("Udt definition not specified");
      }
      foreach (ColumnDesc current in this.Definition.Fields) {
        PropertyInfo property = this.NetType.GetProperty(current.Name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        if (property != null) {
          this.AddPropertyMapping(property, current.Name);
        }
      }
    }
    
    protected override void Build(UdtColumnInfo definition) {
      this.Definition = definition;
      if (this._fieldNameToProperty.Count == 0) {
        this.Automap();
      }
    }
    
    protected override object CreateInstance() {
      return _activator.Run();
    }
    
  }
  
}
