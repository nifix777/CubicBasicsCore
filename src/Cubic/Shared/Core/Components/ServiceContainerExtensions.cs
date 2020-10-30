using Cubic.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;

namespace Cubic.Core.Components
{
  public static class ServiceContainerExtensions
  {

    public static TService GetService<TService>(this IServiceProvider provider) => (TService)provider.GetService(typeof(TService));



    public static object InjectionCallback(IServiceProvider provider, Type type)
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
          var ctorParameters = parameters.ToList();
          object[] callingParameters = new object[ctorParameters.Count];

          for (int i = 0; i < ctorParameters.Count; i++)
          {
            var parameter = ctorParameters[i];
            if (parameter.HasDefaultValue)
            {
              callingParameters[i] = provider.GetService(parameter.ParameterType) ?? parameter.DefaultValue;
            }
            else
            {
              callingParameters[i] = provider.GetService(ctorParameters[i].ParameterType);
            }
            //if (ctorParameters[i].ParameterType == typeof(IServiceProvider))
            //{
            //  callingParameters[i] = provider;
            //  continue;
            //}


          }

          return ctor.Invoke(callingParameters);

        }
      }

      throw new Exception($"No matching Constructor found for Type '{type}'");
    }


    public static IServiceProvider CreateScope(this IServiceProvider provider)
    {
      Guard.AgainstNull(provider, nameof(provider));

      return new ServiceContainer(provider);
    }


    /// <summary>
    /// Get an enumeration of services of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="T">The type of service object to get.</typeparam>
    /// <param name="provider">The <see cref="IServiceProvider"/> to retrieve the services from.</param>
    /// <returns>An enumeration of services of type <typeparamref name="T"/>.</returns>
    public static IEnumerable<T> GetServices<T>(this IServiceProvider provider)
    {
      if (provider == null)
      {
        throw new ArgumentNullException(nameof(provider));
      }

      return provider.GetService<IEnumerable<T>>();
    }

    /// <summary>
    /// Get an enumeration of services of type <paramref name="serviceType"/> from the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="provider">The <see cref="IServiceProvider"/> to retrieve the services from.</param>
    /// <param name="serviceType">An object that specifies the type of service object to get.</param>
    /// <returns>An enumeration of services of type <paramref name="serviceType"/>.</returns>
    public static IEnumerable<object> GetServices(this IServiceProvider provider, Type serviceType)
    {
      if (provider == null)
      {
        throw new ArgumentNullException(nameof(provider));
      }

      if (serviceType == null)
      {
        throw new ArgumentNullException(nameof(serviceType));
      }

      var genericEnumerable = typeof(IEnumerable<>).MakeGenericType(serviceType);
      return (IEnumerable<object>)provider.GetService(genericEnumerable);
    }
  }
}
