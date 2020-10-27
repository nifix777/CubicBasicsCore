using Cubic.Core.Diagnostics;
using Cubic.Core.Reflection;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Cubic.Core.Components
{
  internal class ServiceProviderEngine : IServiceProviderEngine, IServiceProvider
  {
    private IServiceCollection services;

    private ConcurrentDictionary<ServiceDescriptor, object> singletons;

    private ConcurrentBag<IDisposable> disposables;

    public ServiceProviderEngine(IServiceCollection descriptors)
    {
      services = descriptors;

      singletons = new ConcurrentDictionary<ServiceDescriptor, object>();

      disposables = new ConcurrentBag<IDisposable>();
    }

    public event EventHandler<ServiceDescriptor> OnCreate;
    public event EventHandler<ServiceDescriptor> OnResolve;

    private object GetServiceFromDescriptor(ServiceDescriptor serviceDescriptor, IServiceScope serviceScope)
    {
      Guard.AgainstNull(serviceDescriptor, nameof(serviceDescriptor));

      FireOnResolve(serviceDescriptor);

      object instance = null;

      switch (serviceDescriptor.Lifetime)
      {
        case ServiceLifetime.Singleton:
          if (!singletons.ContainsKey(serviceDescriptor))
          {
            FireOnCreate(serviceDescriptor);
            instance = CreateInstance(serviceDescriptor);
            singletons.TryAdd(serviceDescriptor, instance);
          }
          break;
        case ServiceLifetime.Scoped:

          instance = GetScoped(serviceDescriptor);

          break;
        case ServiceLifetime.Transient:
          instance = CreateInstance(serviceDescriptor);
          break;
        default:
          ExceptionHelper.Create<ArgumentOutOfRangeException>("Unknwon ServiceLifetime '{0}'", serviceDescriptor);
          break;
      }

      return instance;
    }

    private object GetScoped(ServiceDescriptor descriptor)
    {
      IDisposable disposable;
      while (disposables.TryPeek(out disposable))
      {
        if (descriptor.ServiceType.IsAssignableFrom(disposable.GetType()))
        {
          return disposable;
        }
      }

      var scoped = CreateInstance(descriptor);

      if (scoped is IDisposable)
      {
        disposable = (IDisposable)scoped;
        disposables.Add(disposable);
      }

      return scoped;
    }



    private object CreateInstance(ServiceDescriptor descriptor)
    {
      if (descriptor.ImplementationInstance != null)
      {
        return descriptor.ImplementationInstance;
      }
      else if (descriptor.ImplementationFactory != null)
      {
        return descriptor.ImplementationFactory(this);
      }
      else
      {
        return ConstructType(descriptor);
      }

    }

    private object ConstructType(ServiceDescriptor descriptor)
    {
      return descriptor.ImplementationType.Construct(GetRealService, (parameter) => !parameter.HasDefaultValue);
    }

    private ServiceDescriptor FindDescriptor(Type serviceType)
    {
      return services.FirstOrDefault(d => d.ServiceType.Equals(serviceType));
    }

    private IEnumerable<ServiceDescriptor> FindDescriptors(Type serviceType)
    {
      return services.Where(d => d.ServiceType.IsAssignableFrom(serviceType));
    }

    private void FireOnResolve(ServiceDescriptor descriptor)
    {
      var handler = OnResolve;
      handler?.Invoke(this, descriptor);
    }

    private void FireOnCreate(ServiceDescriptor descriptor)
    {
      var handler = OnCreate;
      handler?.Invoke(this, descriptor);
    }


    #region IDisposable Support

    public void Dispose()
    {
      while (disposables.TryTake(out IDisposable disposable))
      {
        disposable?.Dispose();
      }
    }

    public object GetRealService(Type serviceType)
    {
      Guard.AgainstNull(serviceType, nameof(serviceType));

      object instance;

      if (typeof(IEnumerable<>).IsAssignableFrom(serviceType))
      {
        var singleType = serviceType.GetGenericArguments()[0];
        var descriptors = FindDescriptors(singleType);

        IList services = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(singleType));

        foreach (var item in descriptors)
        {
          var service = GetServiceFromDescriptor(item, null);
          services.Add(service);
        }

        instance = services;
      }
      else
      {
        var descriptor = FindDescriptor(serviceType);

        if (descriptor == null)
        {
          instance = null;
        }
        else
        {

          instance = GetServiceFromDescriptor(descriptor, null);
        }

      }

      return instance;
    }

    public object GetRealService(Type serviceType, IServiceScope serviceScope)
    {
      Guard.AgainstNull(serviceType, nameof(serviceType));

      object instance;

      if (typeof(IEnumerable<>).IsAssignableFrom(serviceType))
      {
        var singleType = serviceType.GetGenericArguments()[0];
        var descriptors = FindDescriptors(singleType);

        IList services = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(singleType));

        foreach (var item in descriptors)
        {
          var service = GetServiceFromDescriptor(item, serviceScope);
          services.Add(service);
        }

        instance = services;
      }
      else
      {
        var descriptor = FindDescriptor(serviceType);

        if (descriptor == null)
        {
          instance = null;
        }
        else
        {

          instance = GetServiceFromDescriptor(descriptor, serviceScope);
        }

      }

      return instance;
    }

    public object GetService(Type serviceType)
    {
      return this.GetRealService(serviceType);
    }
    #endregion
  }
}
