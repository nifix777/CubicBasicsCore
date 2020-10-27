using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Cubic.Core.Reflection
{
  public static class ExpressionHelper
  {
    public static PropertyInfo GetProperty<T>(Expression<Func<T, object>> propertyExpression)
    {
      MemberExpression expression = null;

      if (propertyExpression.Body is UnaryExpression)
      {
        var unaryExpression = (UnaryExpression)propertyExpression.Body;
        if (unaryExpression.Operand is MemberExpression)
        {
          expression = (MemberExpression)unaryExpression.Operand;
        }
        else
          throw new ArgumentException("Expression is not an Member-Expression");
      }
      else if (propertyExpression.Body is MemberExpression)
      {
        expression = (MemberExpression)propertyExpression.Body;
      }
      else
      {
        throw new ArgumentException("Expression is not an Member-Expression");
      }

      return (PropertyInfo)expression.Member;
    }

    public static MemberInfo GetMethodInfo<T>(Expression<Func<T, Delegate>> expression)
    {
      var unaryExpression = (UnaryExpression)expression.Body;
      var methodCallExpression = (MethodCallExpression)unaryExpression.Operand;
      var methodInfoExpression = (ConstantExpression)methodCallExpression.Arguments.Last();
      var methodInfo = (MemberInfo)methodInfoExpression.Value;
      return methodInfo;
    }

    public static Func<object> GetConstructorParameters(Expression<Func<object>> constructorExpression)
    {

      if (constructorExpression.Body is NewExpression newExpression)
      {
        LambdaExpression lambdaExpression = Expression.Lambda(typeof(Func<object>), newExpression);

        if(_constructorCache.ContainsKey(newExpression.Type))
        {
          return (Func<object>)_constructorCache[newExpression.Type];
        }

        Func<object> compiledExpression = (Func<object>)lambdaExpression.Compile();
        _constructorCache[newExpression.Type] = compiledExpression;
        return compiledExpression;


      }

      return null;

    }

    private static ConcurrentDictionary<object, object> _constructorCache = new ConcurrentDictionary<object, object>();

  }
}