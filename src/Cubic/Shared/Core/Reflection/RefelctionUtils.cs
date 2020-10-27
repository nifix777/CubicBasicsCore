using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Cubic.Core.Collections;
using Cubic.Core.Diagnostics;
using Cubic.Core.Text;

namespace Cubic.Core.Reflection
{
  public static class RefelctionUtils
  {
    private static readonly HashSet<string> AssamblySearchPaths = new HashSet<string>();

    public static void UseAssemblyLoadPath(string directory)
    {
      AssamblySearchPaths.Add(directory);
    }
    /// <summary>
    /// Creates the specified parameter resolver function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameterResolverFunc">The parameter resolver function.</param>
    /// <returns></returns>
    public static T Create<T>(Func<Type, object> parameterResolverFunc) where T : class
    {
      return (T) RefelctionUtils.Create(typeof (T), parameterResolverFunc);
    }

    /// <summary>
    /// Creates the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="parameterResolverFunc">The parameter resolver function.</param>
    /// <returns></returns>
    /// <exception cref="System.InvalidOperationException"></exception>
    public static object Create(Type type, Func<Type, object> parameterResolverFunc)
    {
      var ctor = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault();

      if (ctor != null)
      {
        var parameters = ctor.GetParameters();
        if (!parameters.Any()) // Default Construktor
        {
          return Activator.CreateInstance(type);
        }
        else
        {
          var ctorParameters = parameters.Where(pi => !pi.HasDefaultValue && !pi.IsOptional).ToList();
          object[] callingParameters = new object[ctorParameters.Count];

          for (int i = 0; i < ctorParameters.Count; i++)
          {
            callingParameters[i] = parameterResolverFunc(ctorParameters[i].ParameterType);
          }

          return ctor.Invoke(callingParameters);

        }
      }

      throw new InvalidOperationException(string.Format("'{0}' does not have a public Constructor."));
    }

    public static Assembly TryLoadAssembly(string assemblyName)
    {
      var asm = Assembly.Load(assemblyName);

      if (asm == null)
      {
        foreach (var assamblySearchPath in AssamblySearchPaths)
        {
          if (Directory.Exists(assamblySearchPath))
          {
            var file = Directory.EnumerateFiles( assamblySearchPath, string.Concat(assemblyName, ".dll"),
              SearchOption.AllDirectories).FirstOrDefault(fileName => fileName.Contains(assemblyName));

            if (!file.IsNullOrEmpty())
            {
              asm = Assembly.LoadFile(Path.GetFullPath(file));
            }
          }
        }
      }

      return asm;
    }

    public static Assembly LoadAssembly(string assemblyName)
    {
      return Assembly.Load(assemblyName);
    }

    public static object GetStaticProperty(string typeName, string propertyName)
    {
      var type = Type.GetType(typeName, throwOnError:false);

      if (type != null) return GetStaticProperty(type, propertyName);

      return null;
    }

    public static object GetStaticProperty(Type type, string propertyName)
    {
      return type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)?.GetValue(null);
    }

    public static IEnumerable<T> GetPropertiesOfType<T>(Type rootType, object rootObject)
    {
      return rootType.GetPublicProperties()
            .Where(p => typeof(T).IsAssignableFrom(p.PropertyType))
            .Select(p => p.GetValue(rootType))
            .OfType<T>().ToArray();
    }

    public static IEnumerable<T> GetFieldsOfType<T>(Type rootType, object rootObject)
    {
      return
        rootType.GetFields(BindingFlags.Instance)
            .Where(p => typeof(T).IsAssignableFrom(p.FieldType))
            .Select(p => p.GetValue(rootType))
            .OfType<T>().ToArray();
    }

    /// <summary>
    /// Create a dynamic object with all Properties from the given object.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public static dynamic ToDynamic(this object value)
    {
      IDictionary<string, object> expando = new ExpandoObject();
      var properties = TypeDescriptor.GetProperties(value.GetType());

      foreach (PropertyDescriptor prop in properties)
      {
        expando.Add(prop.Name, prop.GetValue(value));
      }

      return (ExpandoObject) expando;
    }

    public static IEnumerable<IProperty> GetProperties(Type type, object instance)
    {
      return type.GetPublicProperties().Where(p => p.PropertyType is IProperty).Select(p => p.GetValue(instance)).Cast<IProperty>();
    }

    //
    // Uses reflection to find the named event and calls DynamicInvoke on it
    //
    public static void RaiseEvent(object instance, string name, EventArgs args)
    {
      if (instance == null)
        throw new ArgumentNullException(nameof(instance));
      if (string.IsNullOrEmpty(name))
        throw new ArgumentException("eventName cant be null/empty", nameof(name));

      var fieldInfo = instance.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
      if (fieldInfo == null)
        return;
      var multicastDelegate = fieldInfo.GetValue(instance) as MulticastDelegate;

      // NOTE: Using DynamicInvoke so tests work!
      multicastDelegate?.DynamicInvoke(new object[] { instance, args });
    }

    public static IEnumerable<PropertyInfo> GetProperties(Type type, IEnumerable<string> propertyNames)
    {
      return type.GetPublicProperties().WhereNotNull().Where(p => propertyNames.Contains(p.Name));
    }

    public static bool HasProperty(this Type type, string propertyname)
    {
      return type.GetProperty(propertyname) != null;
    }

    public static void SetPropertyValue(PropertyInfo property, object instance, object value)
    {
      property.SetValue(instance, value);
    }

    public static void SetPropertyValue(string propertyname, object instance, object value)
    {
      instance.GetType().GetProperty(propertyname)?.SetValue(instance, value);
    }

    public static T EvaluateProperty<T>(object container, string property)
    {
      string[] expressionPath = property.Split('.');
      object baseobject = container;

      for (var i = 0; i < expressionPath.Length; i++)
      {
        string currentProperty = expressionPath[i];

        if (!string.IsNullOrEmpty(currentProperty))
        {
          PropertyDescriptorCollection descriptorcollection = TypeDescriptor.GetProperties(baseobject);

          PropertyDescriptor descriptor = descriptorcollection.Find(currentProperty, true);

          baseobject = descriptor.GetValue(baseobject);
        }
      }
      return (T)baseobject;
    }

    public static TResult GetValueNonVirtual<TResult>(this MemberInfo member, object obj, params object[] arguments)
    {
      Guard.EnsuresArgument(member is PropertyInfo || member is MethodInfo, "member is PropertyInfo || member is MethodInfo");
      var method = member is PropertyInfo property
          ? property.GetMethod
          : (MethodInfo)member;

      var funcType = Expression.GetFuncType(method.GetParameters()
        .Select(x => x.ParameterType)
        .Concat(method.ReturnType)
        .ToArray());
      var functionPointer = method.NotNull("method != null").MethodHandle.GetFunctionPointer();
      var nonVirtualDelegate = (Delegate)Activator.CreateInstance(funcType, obj, functionPointer)
          .NotNull("nonVirtualDelegate != null");

      return (TResult)nonVirtualDelegate.DynamicInvoke(arguments);
    }
  }
}
