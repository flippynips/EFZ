/*
 * User: Joshua
 * Date: 19/06/2016
 * Time: 12:40 AM
 */
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Efz.Collections;

namespace Efz {
  
  /// <summary>
  /// Methods for constructing common expressions/things.
  /// </summary>
  public static class Dynamic {
    
    /// <summary>
    /// The dynamic funcs constructed. Persisted for speed.
    /// </summary>
    private readonly static Dictionary<ConstructorInfo, IFunc> _constructors = new Dictionary<ConstructorInfo, IFunc>();
    
    /// <summary>
    /// Generic FuncSet types that are to be used with their associated numbers of generic parameters.
    /// IFuncs are ordered by parameter number with [0] being '1' parameter with the correct number of
    /// generic parameters.
    /// </summary>
    private static readonly Type[] _funcSets;
    /// <summary>
    /// Generic Func types that are to be used with their associated numbers of generic parameters.
    /// </summary>
    private static readonly Type[] _funcs;
    
    static Dynamic() {
      _funcs = new []{
        typeof(Func<>),
        typeof(Func<,>),
        typeof(Func<,,>),
        typeof(Func<,,,>),
        typeof(Func<,,,,>),
        typeof(Func<,,,,,>),
        typeof(Func<,,,,,,>),
        typeof(Func<,,,,,,,>),
        typeof(Func<,,,,,,,,>),
        typeof(Func<,,,,,,,,,>),
        typeof(Func<,,,,,,,,,,>)
      };
      _funcSets = new [] {
        typeof(FuncSet<>),
        typeof(FuncSet<,>),
        typeof(FuncSet<,,>),
        typeof(FuncSet<,,,>),
        typeof(FuncSet<,,,,>),
        typeof(FuncSet<,,,,,>),
        typeof(FuncSet<,,,,,,>),
        typeof(FuncSet<,,,,,,,>),
        typeof(FuncSet<,,,,,,,,>),
        typeof(FuncSet<,,,,,,,,,>),
        typeof(FuncSet<,,,,,,,,,,>)
      };
    }
    
    /// <summary>
    /// Create an activator function that will create an instance of the type specified.
    /// Optionally using the specified constructor.
    /// </summary>
    public static TFunc Constructor<TFunc>(Type type = null, ConstructorInfo constructor = null) where TFunc : IFunc {
      // represents the type of Func<> delegate to call the constructor
      Type iFuncType = typeof(TFunc);
      
      // this will contain all the parameter types for the generic func constructor
      // and the return type
      Type[] parameters = iFuncType.GetGenericArguments();
      
      // if the type wasn't specified - derive it from the generic specification
      if(type == null) type = parameters[parameters.Length-1];
      
      // get a constructor if not specified
      if(constructor == null) {
        Type[] constructorParameters = new Type[parameters.Length-1];
        for(int i = constructorParameters.Length-1; i >= 0; --i) constructorParameters[i] = parameters[i];
        
        // get the constructor that matches the generic func to be returned
        constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
          null, CallingConventions.Any, constructorParameters, null);
        if(constructor == null) {
          Log.Error("Dynamic - No constructor was available for the type '" + type.Name + "'.");
          return default(TFunc);
        }
      }
      
      // get the function if cached
      IFunc function;
      if(_constructors.TryGetValue(constructor, out function)) return (TFunc)function;
      
      // if there are arguments to be passed to the constructor
      if(parameters.Length > 1) {
        // initialize the parameter expressions that reflect the constructor parameters
        ParameterExpression[] parameterExpressions = new ParameterExpression[parameters.Length-1];
        
        // create a typed expression of them of each parameter
        for(int i = parameterExpressions.Length-1; i >= 0; --i) {
          // add to the expression arguments
          parameterExpressions[i] = Expression.Parameter(parameters[i], i.ToString());
        }
        
        // make a NewExpression that calls the constructor with the expressions arguments we just created
        NewExpression constructorExpression = Expression.New(constructor, parameterExpressions);
        
        // initialize the func type to generate with the parameter types
        var funcType = _funcs[parameterExpressions.Length].MakeGenericType(parameters);
        
        // create a lambda expression with the constructor body and the expression parameters then get the resulting func
        TFunc result = (TFunc)Activator.CreateInstance(iFuncType, new object[] {
          Expression.Lambda(funcType, constructorExpression, parameterExpressions).Compile()
        });
        
        // persist the constructor
        _constructors[constructor] = result;
        
        return result;
      } else {
        // make a NewExpression that calls the constructor with the expressions arguments we just created
        NewExpression constructorExpression = Expression.New(constructor);
        
        // create lambda that just returns the constructed item
        var funcType = _funcs[0].MakeGenericType(parameters);
        
        // create a lambda expression with the constructor body and the expression parameters then get the resulting func
        TFunc result = (TFunc)Activator.CreateInstance(iFuncType, new object[] {
          Expression.Lambda(funcType, constructorExpression).Compile()
        });
        
        // persist the constructor
        _constructors[constructor] = result;
        
        return result;
      }
    }
    
    /// <summary>
    /// Create an activator function that will call a constructor for the specified type.
    /// Optionally specify the constructor to use.
    /// </summary>
    public static IFunc BuildConstructor(Type type, ConstructorInfo constructor = null) {
      // get the constructor if not specified
      if(constructor == null) {
        // get the first constructor
        constructor = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)[0];
      }
      
      // get the function if persisted
      IFunc function;
      if(_constructors.TryGetValue(constructor, out function)) {
        return function;
      }
      
      // get the parameters
      ParameterInfo[] parameterInfos = constructor.GetParameters();
      
      // this will contain all the parameter types for the generic func constructor
      // and the return type
      Type[] parameters = new Type[parameterInfos.Length+1];
      // the return type of the Func is the result of the constructor
      parameters[parameters.Length-1] = type;
      
      // if there are arguments to be passed to the constructor
      if(parameterInfos.Length > 0) {
        // create a typed expression of them    of each parameter
        for(int i = parameterInfos.Length-1; i >= 0; --i) {
          // get this parameter type
          Type parameterType = parameterInfos[i].ParameterType;
          // remember each parameter type
          parameters[i] = parameterType;
        }
      }
      
      // get the func set type with the correct number of parameters
      Type funcSetType = _funcSets[parameters.Length-1].MakeGenericType(parameters);
      
      // if there are arguments to be passed to the constructor
      if(parameters.Length > 1) {
        // initialize the parameter expressions that reflect the constructor parameters
        ParameterExpression[] parameterExpressions = new ParameterExpression[parameters.Length-1];
        
        // create a typed expression of them of each parameter
        for(int i = parameterExpressions.Length-1; i >= 0; --i) {
          // add to the expression arguments
          parameterExpressions[i] = Expression.Parameter(parameters[i], i.ToString());
        }
        
        // make a NewExpression that calls the constructor with the expressions arguments we just created
        NewExpression constructorExpression = Expression.New(constructor, parameterExpressions);
        
        // initialize the func type to generate with the parameter types
        var funcType = _funcs[parameterExpressions.Length].MakeGenericType(parameters);
        
        // create a lambda expression with the constructor body and the expression parameters then get the resulting func
        IFunc result = (IFunc)Activator.CreateInstance(funcSetType, new object[] {
          Expression.Lambda(funcType, constructorExpression, parameterExpressions).Compile()
        });
        
        // persist the constructor
        _constructors[constructor] = result;
        
        return result;
      } else {
        // make a NewExpression that calls the constructor with the expressions arguments we just created
        NewExpression constructorExpression = Expression.New(constructor);
        
        // create lambda that just returns the constructed item
        var funcType = _funcs[0].MakeGenericType(parameters);
        
        // create a lambda expression with the constructor body and the expression parameters then get the resulting func
        IFunc result = (IFunc)Activator.CreateInstance(funcSetType, new object[] {
          Expression.Lambda(funcType, constructorExpression).Compile()
        });
        
        // persist the constructor
        _constructors[constructor] = result;
        
        return result;
      }
      
//      
//      if(_constructorMethod == null) {
//        _constructorMethod = typeof(Dynamic).GetMethod("Constructor",
//          new []{ typeof(Type), typeof(ConstructorInfo) });
//      }
//      
//      function = (IFunc)_constructorMethod.MakeGenericMethod(funcCreatorType).Invoke(null, BindingFlags.Static, null, new object[] { type, constructor }, System.Globalization.CultureInfo.InvariantCulture);
//      
//      // persist constructor
//      _constructors[constructor] = function;
//      
//      // new FuncSet<T>(lambda.Compile()); equivalent
//      return function;
    }
    
  }
  
}
