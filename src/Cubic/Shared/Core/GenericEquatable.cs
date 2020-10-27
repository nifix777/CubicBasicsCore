using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Cubic.Core
{

  /// <summary>
  /// Abstract generic class to perform reflection Based Equaity-Checks.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <seealso cref="System.IEquatable{T}" />
  public abstract class GenericEquatable<T> : IEquatable<T> where T : GenericEquatable<T>
  {
    public override bool Equals(object other)
    {
      return Equals(other as T);
    }

    public bool Equals(T other)
    {
      return EquatableHelper.PropertiesEquals(this, other);
    }

    public override int GetHashCode()
    {
      return EquatableHelper.PropertiesGetHashCode(this);
    }

    public static bool operator ==(T x, GenericEquatable<T> y)
    {
      return EquatableHelper.PropertiesEquals(x, y);
    }

    public static bool operator !=(T x, GenericEquatable<T> y)
    {
      return !(x == y);
    }
  }

  internal static class EquatableHelper
  {
    private static readonly ConcurrentDictionary<Type, Func<object, object, bool>>
        GetEqualsFunctions
            = new ConcurrentDictionary<Type, Func<object, object, bool>>();
    private static readonly ConcurrentDictionary<Type, Func<object, int>>
        GetHashCodeFunctions
            = new ConcurrentDictionary<Type, Func<object, int>>();

    public static bool PropertiesEquals(object x, object y)
    {
      if (ReferenceEquals(x, y)) { return true; }
      if (x == null || y == null) { return false; }

      Type type = x.GetType();
      var getEqualsFunction = GetEqualsFunctions.GetOrAdd(type,
              MakeEqualsMethod);
      return getEqualsFunction(x, y);
    }

    public static int PropertiesGetHashCode(object obj)
    {
      if (obj == null) { return 0; }

      Type type = obj.GetType();
      var getHashCodeFunction = GetHashCodeFunctions.GetOrAdd(type,
              MakeGetHashCodeMethod);
      return getHashCodeFunction(obj);
    }

    private static Func<object, object, bool> MakeEqualsMethod(Type type)
    {
      var paramThis = Expression.Parameter(typeof(object), "x");
      var paramThat = Expression.Parameter(typeof(object), "y");

      var paramCastThis = Expression.Convert(paramThis, type);
      var paramCastThat = Expression.Convert(paramThat, type);

      Expression last = null;
      var equalsMethod = typeof(object)
              .GetMethod("Equals", BindingFlags.Public | BindingFlags.Static);
      foreach (PropertyInfo property in type.GetProperties())
      {
        // Boxing is necessary for the call of the Equals method.
        var propertyAccessX =
                Expression.Convert(
                Expression.Property(paramCastThis, property), typeof(object));
        var propertyAccessY =
                Expression.Convert(
                Expression.Property(paramCastThat, property), typeof(object));

        // Use the Equals method instead of Expression.Equals 
        // which calls the "== operator"
        var equals = Expression.Call(equalsMethod, propertyAccessX,
                propertyAccessY);
        if (last == null)
        {
          last = equals;
        }
        else
        {
          last = Expression.AndAlso(last, equals);
        }
      }
      if (last == null)
      {
        // Type has no public instance properties: 
        // true if both types are the same
        last = Expression.Condition(Expression.TypeIs(paramThat, type),
                Expression.Constant(true), Expression.Constant(false));
      }
      return Expression.Lambda<Func<object, object, bool>>(last, paramThis,
          paramThat).Compile();
    }

    private static Func<object, int> MakeGetHashCodeMethod(Type type)
    {
      ParameterExpression paramThis = Expression.Parameter(typeof(object),
          "obj");
      UnaryExpression paramCastThis = Expression.Convert(paramThis, type);

      Expression last = null;
      Expression nullValue = Expression.Constant(null);
      foreach (PropertyInfo property in type.GetProperties())
      {
        // Boxing is necessary for the call of the GetHashCode method.
        var propertyAccess = Expression.Convert(
            Expression.Property(paramCastThis, property), typeof(object));
        var hash = Expression.Condition(
            Expression.Equal(propertyAccess, nullValue),
            Expression.Constant(0),
            Expression.Call(propertyAccess, "GetHashCode", Type.EmptyTypes));
        if (last == null)
        {
          last = hash;
        }
        else
        {
          last = Expression.ExclusiveOr(last, hash);
        }
      }
      if (last == null)
      {
        // Type has no public instance properties
        last = Expression.Constant(0);
      }
      return Expression.Lambda<Func<object, int>>(last, paramThis).Compile();
    }
  }
}