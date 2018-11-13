/*
 * User: Joshua
 * Date: 29/07/2016
 * Time: 3:25 PM
 */
using System;
using System.Collections.Generic;
using System.Reflection;

using Efz.Collections;

namespace Efz {
  
  /// <summary>
  /// A structure for fast type lookups pertaining to inhertance. Subscribe to types
  /// of different specifications.
  /// </summary>
  public static class TypeMap {
    
    //-------------------------------------------//
    
    //-------------------------------------------//
    
    private static ArrayRig<Assembly> _assemblies;
    private static Dictionary<Type, HashSet<Type>> _mappings;
    
    //-------------------------------------------//
    
    static TypeMap() {
      _assemblies = new ArrayRig<Assembly>();
      _mappings = new Dictionary<Type, HashSet<Type>>();
    }
    
    /// <summary>
    /// Add a new assembly which types are parsed by the type map.
    /// </summary>
    public static void AddAssembly(Assembly _assembly) {
      _assemblies.Add(_assembly);
      
      foreach(KeyValuePair<Type, HashSet<Type>> mapping in _mappings) {
        // if looking for types implementing an interface
        if(mapping.Key.IsInterface) {
          // iterate assembly types
          foreach(Type type in _assembly.GetTypes()) {
            // not mapping interfaces
            if(!type.IsInterface) {
              // iterate interfaces implemented by type
              foreach(Type inter in type.GetInterfaces()) {
                // if interface matches
                if(inter.IsEquivalentTo(mapping.Key)) {
                  // add to mapping
                  mapping.Value.Add(type);
                }
              }
            }
          }
        } else {
          // iterate assembly types
          foreach(Type type in _assembly.GetTypes()) {
            // not mapping interfaces or abstracts
            if(!type.IsAbstract && !type.IsInterface && type.BaseType.IsEquivalentTo(mapping.Key)) {
              // add to mapping
              mapping.Value.Add(type);
            }
          }
        }
      }
      
    }
    
    /// <summary>
    /// Add a base type to the type map. This makes its base types searchable.
    /// Base types can be abstract or interfaces.
    /// </summary>
    public static void AddType(Type _baseType) {
      HashSet<Type> index = new HashSet<Type>();
      _mappings.Add(_baseType, index);
      
      // if looking for types implementing an interface
      if(_baseType.IsInterface) {
        // iterate assemblies
        foreach(Assembly assembly in _assemblies) {
          // iterate assembly types
          foreach(Type type in assembly.GetTypes()) {
            // not mapping interfaces
            if(!type.IsInterface) {
              // iterate interfaces implemented by type
              foreach(Type inter in type.GetInterfaces()) {
                // if interface matches
                if(inter.IsEquivalentTo(_baseType)) {
                  // add to mapping
                  index.Add(type);
                }
              }
            }
          }
        }
      } else {
        // iterate assemblies
        foreach(Assembly assembly in _assemblies) {
          // iterate assembly types
          foreach(Type type in assembly.GetTypes()) {
            // not mapping interfaces
            if(!type.IsAbstract && !type.IsInterface && type.BaseType.IsEquivalentTo(_baseType)) {
              // add to mapping
              index.Add(type);
            }
          }
        }
      }
      
    }
    
    /// <summary>
    /// Get if one type is derived from another.
    /// </summary>
    public static bool IsDerivedFrom(Type _base, Type _derived) {
      return _mappings[_base].Contains(_derived);
    }
    
    /// <summary>
    /// Get the types that are more derived than the specified base type.
    /// </summary>
    public static IEnumerator<Type> DerivingTypes(Type _base) {
      return _mappings[_base].GetEnumerator();
    }
    
    //-------------------------------------------//
    
  }
}
