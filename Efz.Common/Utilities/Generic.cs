using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace Efz {

  /// <summary>
  /// Generic helper functions related to generics  and types.
  /// </summary>
  public static class Generic {
    
    /// <summary>
    /// Get all values of an enum type.
    /// </summary>
    public static IEnumerable<T> EnumValues<T>() {
      return Enum.GetValues(typeof(T)).Cast<T>();
    }
    
    /// <summary>
    /// Get the value of a field or property on the specified object of the specified name.
    /// Returns default(T) if no member is found.
    /// </summary>
    public static T GetValue<T>(object item, string memberName, BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic) {
      MemberInfo[] members = item.GetType().GetMember(memberName, flags);
      // check each member
      foreach(MemberInfo member in members) {
        if(member.MemberType == MemberTypes.Property) {
          // get the property value if can read
          PropertyInfo property = (PropertyInfo)member;
          if(property.CanRead) {
            return (T)property.GetValue(item);
          }
        }
        if(member.MemberType == MemberTypes.Field) {
          // get the field value
          return (T)((FieldInfo)member).GetValue(item);
        }
      }
      return default(T);
    }
    
    /// <summary>
    /// Clone an object.
    /// </summary>
    public static T Clone<T>(T item) {
      if(!typeof(T).IsSerializable) {
        throw new ArgumentException("The type must be serializable.", "item");
      }
      
      // Don't serialize a null object, simply return the default for that object
      if(Object.ReferenceEquals(item, null)) {
        return default(T);
      }
      
      IFormatter formatter = new BinaryFormatter();
      Stream stream = new MemoryStream();
      using(stream) {
        formatter.Serialize(stream, item);
        stream.Seek(0, SeekOrigin.Begin);
        return (T)formatter.Deserialize(stream);
      }
    }
    
    /// <summary>
    /// Create and initialize a number of items with default values and return them.
    /// </summary>
    public static T[] Series<T>(int number) where T : new() {
      T[] items = new T[number];
      while(--number > -1) {
        items[number] = new T();
      }
      return items;
    }
    
    /// <summary>
    /// Create a number of items with a specified initial value and return them.
    /// </summary>
    public static T[] Series<T>(int number, T value) {
      T[] items = new T[number];
      while(--number >= 0) items[number] = value;
      return items;
    }
    
    /// <summary>
    /// Derive one collection from another.
    /// </summary>
    public static B[] Derive<A, B>(this A[] a, Func<A, B> deriver) {
      B[] items = new B[a.Length];
      for(int i = a.Length-1; i >= 0; --i) {
        items[i] = deriver(a[i]);
      }
      return items;
    }
    
    /// <summary>
    /// Get a sub section of the specified array.
    /// </summary>
    public static A[] SubArray<A>(this A[] array, int startIndex, int length = -1) {
      if(length < 0) length = array.Length - startIndex;
      A[] result = new A[length];
      int index = 0;
      while(index < length) {
        result[index] = array[startIndex];
        ++index;
        ++startIndex;
      }
      return result;
    }
    
    /// <summary>
    /// Check the existance of an item in a collection.
    /// </summary>
    public static bool Contains<A>(this A[] array, A item) {
      for(int i = array.Length-1; i >= 0; --i) {
        if(Equals(array[i], item)) return true;
      }
      return false;
    }
    
    /// <summary>
    /// Get properties of by type. Returns the values of T types on selected class of type V.
    /// </summary>
    public static T[] Properties<T, V>(V target, BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly) {
      var types = typeof(V).GetProperties(flags | BindingFlags.GetProperty);
      T[] properties = new T[types.Length];
      int index = 0;
      foreach(PropertyInfo property in types) {
        properties[index] = (T)property.GetValue(target);
        ++index;
      }
      return properties;
    }
    
    /// <summary>
    /// Get all the more derived, non abstract types of a base type.
    /// </summary>
    public static IEnumerable<Type> GetTypes<T>(Expression<System.Func<Type, bool>> expression = null) {
      return GetTypes(typeof(T), expression);
    }
    
    /// <summary>
    /// Get all the more derived, non abstract types of a base type.
    /// </summary>
    public static IEnumerable<Type> GetTypes(Type baseType, Expression<System.Func<Type, bool>> expression = null) {
      
      IQueryable<Type> types = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                from type in assembly.GetTypes() where
                                  baseType.IsAssignableFrom(type) &&
                                  type.IsClass &&
                                  !type.IsAbstract
                                select type).AsQueryable();
      
      if(expression != null) {
        types.Where(expression);
      }
      
      return types.AsEnumerable();
    }
    
    /// <summary>
    /// Get all types belonging to the specified namespace, optionally allow only types assigned the specified attribute type.
    /// </summary>
    public static IEnumerable<Type> GetTypes(string @namespace, Type attributeType = null) {
      if(attributeType == null ) {
        return (from assembly in AppDomain.CurrentDomain.GetAssemblies() where
                  assembly.FullName.StartsWith(@namespace, StringComparison.Ordinal)
                from type in assembly.GetTypes() where
                  type.FullName.StartsWith(@namespace, StringComparison.Ordinal)
                select type).AsEnumerable();
      }
      return (from assembly in AppDomain.CurrentDomain.GetAssemblies() where
                assembly.FullName.StartsWith(@namespace, StringComparison.Ordinal)
              from type in assembly.GetTypes() where
                type.FullName.StartsWith(@namespace, StringComparison.Ordinal) &&
                type.GetCustomAttribute(attributeType) != null
              select type).AsEnumerable();
    }


    /// <summary>
    /// Get all the more derived, non abstract types of a generic base type.
    /// </summary>
    public static IEnumerable<Type> GetGenericImplementations(Type baseType, Expression<System.Func<Type, bool>> expression = null)
    {
      IQueryable<Type> types = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
        from type in assembly.GetTypes()
        where
          type.BaseType != null && type.BaseType.IsGenericType &&
          type.BaseType.GetGenericTypeDefinition() == baseType
        select type).AsQueryable();

      if (expression != null) types.Where(expression);

      return types.AsEnumerable();
      
    }

    /// <summary>
    /// Construct instances of all the more derived, non abstract types of a base type.
    /// Note that derived types must have parameterless constructors.
    /// </summary>
    static public T[] GetInstances<T>(Expression<System.Func<Type, bool>> expression = null, BindingFlags flags = BindingFlags.Instance) where T : class {
      // get types deriving from T.
      IQueryable<Type> types = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                from assemblyType in domainAssembly.GetTypes()
                                  where typeof(T).IsAssignableFrom(assemblyType) && !assemblyType.IsAbstract
                                  select assemblyType).AsQueryable();
      
      if(expression != null) {
        types.Where(expression);
      }
      // initialize instance array
      T[] instances = new T[types.Count()];
      int index = 0;
      // iterate through types and initialize with no parameters
      foreach(Type type in types) {
        instances[index] = type.GetConstructor(flags, null, CallingConventions.Any, Type.EmptyTypes, null).Invoke(null) as T;
        ++index;
      }
      return instances;
    }
    
  }
}
