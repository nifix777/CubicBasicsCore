using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Cubic.Core.Reflection;

namespace Cubic.Core.Components
{
  public class BasicContainer : IContainer
  {
    private readonly bool _throwOnError;
    private IDictionary<string, IResolver> _keyReslover;

    private IList<IResolver> _resolvers;

    public BasicContainer(bool throwOnError = false)
    {
      _throwOnError = throwOnError;
      _keyReslover = new ConcurrentDictionary<string, IResolver>();
      _resolvers = new List<IResolver>();
    }

    public void Register(string key, IResolver keyResolver)
    {
      _keyReslover[key] = keyResolver;
    }

    public void Use(IResolver typeResolver)
    {
      _resolvers.Add(typeResolver);
    }

    public void Replace(IResolver typeResolver)
    {
      var replaces = GetResolvers(typeResolver.ServiceType).ToArray();

      foreach (var resolver in replaces)
      {
        _resolvers.Remove(resolver);
      }

      _resolvers.Add(typeResolver);
    }

    public T Resolve<T>() where T : class
    {
      return (T)Resolve(typeof (T));
    }

    public IEnumerable<T> ResolveMany<T>() where T : class
    {
      var resolvers = GetResolvers(typeof (T));

      foreach (var resolver in resolvers )
      {
        T result;

        try
        {
          result = (T)resolver.Resolve();

        }
        catch(Exception error)
        {
          if (_throwOnError)
          {
            throw new CompositionException($"Exception on Resolving '{typeof(T).FullName}'", error );
          }
          result = default(T);
        }

        yield return result;
      }
    }

    public object Resolve(Type interfacType)
    {
      var resolver = GetResolvers(interfacType).FirstOrDefault();
      object result = interfacType.GetDefault();

      try
      {
        result = resolver?.Resolve();
      }
      catch (Exception ex)
      {
        if (_throwOnError)
        {
          throw new CompositionException($"Exception on Resolving '{interfacType.FullName}'", ex);
        }
      }

      return result;
    }

    private IEnumerable<IResolver> GetResolvers(Type serviceType)
    {
      return _resolvers.Where(r => serviceType.IsAssignableFrom(r.ServiceType));
    }

    public object Resolve(string key)
    {
      object result = null;

      try
      {
        return _keyReslover[key].Resolve();
      }
      catch (Exception ex)
      {
        if (_throwOnError)
        {
          throw new CompositionException($"Exception on Resolving '{key}'", ex);
        }
        return null;
      }
    }

    public T Resolve<T>(string key) where T : class
    {
      return this.Resolve(key) as T;
    }

    public object GetService(Type serviceType)
    {
      return this.Resolve(serviceType);
    }

    public void Dispose()
    {
      foreach (var resolver in _resolvers)
      {
        resolver.Dispose();
      }

      foreach (var kvResolver in _keyReslover)
      {
        kvResolver.Value.Dispose();
      }

      _resolvers.Clear();
      _keyReslover.Clear();
    }
  }
}