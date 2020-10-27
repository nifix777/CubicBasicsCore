using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cubic.Core.Collections;

namespace Cubic.Core.Reflection
{
  public static class TypeExtensions
  {

    /// <summary>
    /// Determines whether the given <paramref name="type"/> [has default constructor] (Arguments == 0).
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    ///   <c>true</c> if [has default constructor] [the specified type]; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasDefaultConstructor(this Type type)
    {
      return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        .Any(ctor => ctor.GetParameters().Length == 0);
    }

    /// <summary>
    /// Gets the matching constructor-delegate for the given <paramref name="type"/>.
    /// <paramref name="parameters"/> must match a the number of arguments, Type and order of any Constructor.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>the matching ConstructorInfo.Invoke or  null</returns>
    public static Func<object[], object> GetMatchingConstructor(this Type type, params object[] parameters)
    {
      var parameterTypes = parameters?.Select(p => p.GetType()).ToArray() ?? Type.EmptyTypes;
      var foundCtor =
        type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
          .FirstOrDefault(c => ConstructorMatcher(c, parameterTypes));

      if (foundCtor != null)
      {

        return foundCtor.Invoke;
      }

      return null;

    }

    private static bool ConstructorMatcher(ConstructorInfo constructorInfo, Type[] parameterTypes)
    {
      var parameters = constructorInfo.GetParameters();
      if (parameters.Length != parameterTypes.Length) return false;

      for (int i = 0; i < parameters.Length; i++)
      {
        if (parameters[i].ParameterType != parameterTypes[i]) return false;
      }

      return true;
    }


    /// <summary>
    /// Gets the default value for that Type. if the Type is an <see cref="IEnumerable"/> it will return en Emppty instance.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public static object GetDefault(this Type type)
    {
      if (type.IsValueType && !type.IsNullable())
      {
        return Activator.CreateInstance(type);
      }

      return null;
    }

    public static bool IsNullable(this Type type)
    {
      return Nullable.GetUnderlyingType(type) != null;
    }

    public static object CreateGenericInstance(this Type genericType)
    {
      var genericArgs = genericType.GetGenericArguments();

      var constructedType = genericType.MakeGenericType(genericArgs);

      return Activator.CreateInstance(constructedType);

    }

    public static object CreateInstance(this Type type, params object[] arguments)
    {
      var genericArgs = type.GetGenericArguments();

      var constructedType = type.MakeGenericType(genericArgs);

      return Activator.CreateInstance(constructedType);

    }

    public static object Construct(this Type type, Func<Type, object> constructionParameterFunc)
    {
      var consstructor = type.GetConstructors().FirstOrDefault();

      if (consstructor == null) throw new Exception(String.Format("Missing ctor on Type: {0}", type.FullName));

      var arguments = Enumerable.ToArray(consstructor.GetParameters().Select(p => constructionParameterFunc(p.ParameterType)));

      return consstructor.Invoke(arguments);

    }

    public static object Construct(this Type type, Func<Type, object> constructionParameterFunc, Func<ParameterInfo, bool> parameterFilter)
    {
        var consstructor = type.GetConstructors().FirstOrDefault();

        if (consstructor == null) throw new Exception(String.Format("Missing ctor on Type: {0}", type.FullName));

        var arguments = Enumerable.ToArray(consstructor.GetParameters().Where(parameterFilter).Select(p => constructionParameterFunc(p.ParameterType)));

        return consstructor.Invoke(arguments);

    }

    public static object CreateGenericInstance(this Type genericType, params Type[] genericTypes)
    {

      var constructedType = genericType.MakeGenericType(genericTypes);

      return Activator.CreateInstance(constructedType);

    }

    public static object CreateGenericInstance(this Type genericType,  Func<Type, object> constructionParameterFunc, params Type[] genericTypes)
    {

      var constructedType = genericType.MakeGenericType(genericTypes);

      var consstructor = constructedType.GetConstructors().FirstOrDefault();

      if(consstructor == null) throw new Exception(String.Format("Missing ctor on Type: {0}", constructedType.FullName));

      var arguments = Enumerable.ToArray(consstructor.GetParameters().Select(p => constructionParameterFunc(p.ParameterType)));

      return consstructor.Invoke(arguments);

    }

    /// <summary>
    /// Determines whether this instance is a simple Type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public static bool IsSimple(this Type type)
    {
      if (type.IsPrimitive || type == typeof(string) || type == typeof(Guid) || type == typeof(Enum) ||
          type == typeof(decimal) || type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(TimeSpan) || type == typeof(DateTime))
      {
        return true;
      }

      return false;
    }

    public static bool IsPrimitive(this Type type)
    {
      if (type == typeof(String)) return true;
      return (type.IsValueType & type.IsPrimitive);
    }

    /// <summary>
    /// Calls <see cref="IDisposable.Dispose"/> if the object is <see cref="IDisposable"/>.
    /// </summary>
    /// <param name="instance">The instance of an object.</param>
    public static void TryDispose(this object instance)
    {
      if (instance is IDisposable)
      {
        var disposeable = (IDisposable) instance;
        disposeable.Dispose();
      }
    }

    //public static bool IsNullable(Type type)
    //{
    //  return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    //}

    public static PropertyInfo[] GetPublicProperties(this Type type)
    {
      return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
    }

    public static object GetValueCore(object instance, string propertyName)
    {
      var property = instance.GetType().GetProperty(propertyName);
      if (property != null) return property.GetValue(instance);

      var field = instance.GetType().GetField(propertyName);
      if (field != null) return field.GetValue(instance);

      return null;
    }

    public static MemberInfo GetMemberCore(object instance, string propertyName)
    {
      var property = instance.GetType().GetProperty(propertyName);
      if (property != null) return property;

      var field = instance.GetType().GetField(propertyName);
      if (field != null) return field;

      return null;
    }

    public static void SetValueCore(object instance, object value, string propertyName)
    {
      var property = value.GetType().GetProperty(propertyName);
      if (property != null)  property.SetValue(instance, value);

      var field = value.GetType().GetField(propertyName);
      if (field != null) field.SetValue(instance, value);
    }

    //public static object GetMember(this object _this, string propertyName)
    //{
    //  //return _this.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)?.GetValue(_this);
    //  var current = _this;
    //  string[] nameParts = propertyName.Split(Constants.Dot.ToChar());
    //  if (nameParts.Length == 1)
    //  {
    //    var member = current.GetType().GetMember(propertyName)[0];

    //    if (member is PropertyInfo property) return property.GetValue(current, null);

    //    if (member is FieldInfo field) return field.GetValue(current);

    //    if (member is MethodInfo method) return method.Invoke(current, null);

    //    return null;
    //  }

    //  foreach (string part in nameParts)
    //  {
    //    if (current == null) { return null; }

    //    Type type = current.GetType();

    //    MemberInfo info = null;

    //    if (typeof(string) == type && part == "Lenght")
    //    {

    //      current = current.GetMember("Chars.Lenght");
    //    }
    //    else
    //    {

    //      info = type.GetMember(part)[0];
    //    }

    //    if (info == null) { return null; }

    //    if (info is PropertyInfo property) current = property.GetValue(current, null);

    //    if (info is FieldInfo field) current = field.GetValue(current);

    //    if (info is MethodInfo method) current = method.Invoke(current, null);

    //    //current = null;

    //    //current = info.GetValue(_this, null);
    //  }
    //  return current;
    //}

    public static object GetValue(this object _this, string expression)
    {
      Stack<string> propertyStack = new Stack<string>();

      if (expression.Contains(Constants.Dot))
      {
        foreach (var propName in expression.Split(Constants.Dot.ToChar()).Reverse())
        {
          propertyStack.Push(propName);
        }

        
      }

      var currentName = propertyStack.Pop();

      object currentProperty = _this;

      while (!currentName.IsNullOrEmpty())
      {
        currentProperty = TypeExtensions.GetValueCore(currentProperty, currentName);
        currentName = string.Empty;

        if (propertyStack.Count != 0)
        {
          currentName = propertyStack.Pop();
        }
      }

      return currentProperty;
    }

    public static void SetValue(this object _this, string expression, object value)
    {
      Stack<string> propertyStack = new Stack<string>();

      string currentName;

      if (expression.Contains(Constants.Dot))
      {
        foreach (var propName in expression.Split(Constants.Dot.ToChar()).Reverse())
        {
          propertyStack.Push(propName);
        }

        currentName = propertyStack.Pop();
      }
      else
      {
        currentName = expression;
      }



      object currentProperty = _this;

      while (!currentName.IsNullOrEmpty())
      {
        currentProperty = TypeExtensions.GetMemberCore(currentProperty, currentName);
        currentName = string.Empty;

        if (propertyStack.Count != 0)
        {
          currentName = propertyStack.Pop();
        }
      }

      if (currentProperty is PropertyInfo property)
      {
        property.SetValue(_this, value);
      }

      if (currentProperty is FieldInfo field)
      {
        field.SetValue(_this, value);
      }
    }

    //public static object GetProperty(this object _this, string propertyName)
    //{
    //  return _this.GetType().GetTypeInfo().GetDeclaredProperty(propertyName)?.GetValue(_this);
    //}

    public static Type MakeGenericType(this IProperty property, Type genericType)
    {
      return typeof(IProperty<>).MakeGenericType(genericType);
    }

    public static T GetPropertyValue<T>(this Type type, string name, object instance)
    {
      return (T)type.GetProperty(name, typeof(T), Type.EmptyTypes)?.GetValue(instance);
    }

  }
}