using System;
using System.Collections.Generic;
using System.Linq;

namespace Cubic.Core.Components
{
  public class ScopedContainer : IContainer
  {
    private IContainer _root;

    private bool _disposedCalled;

    private readonly IList<IDisposable> _disposables = new List<IDisposable>();

    public ScopedContainer(IContainer rootContainer)
    {
      _root = rootContainer;
    }
    public object GetService(Type serviceType)
    {
      var service = _root.GetService(serviceType);

      CaptureDisposableService(service);

      return service;
    }

    public void Dispose()
    {
      lock (_disposables)
      {

        if(_disposedCalled) return;

        _disposedCalled = true;

        foreach (var disposable in _disposables)
        {
          disposable.Dispose();
        }

        _disposables.Clear();
      }
    }

    public void Use(IResolver typeResolver)
    {
      _root.Use(typeResolver);
    }

    public void Replace(IResolver typeResolver)
    {
      _root.Replace(typeResolver);
    }

    public void Register(string key, IResolver keyResolver)
    {
      _root.Register(key, keyResolver);
    }

    public T Resolve<T>() where T : class
    {
      var service = _root.Resolve<T>();

      CaptureDisposableService(service);

      return service;
    }

    public IEnumerable<T> ResolveMany<T>() where T : class
    {
      var services = _root.ResolveMany<T>().ToList();

      if (services.Any())
      {
        foreach (var service in services)
        {
          CaptureDisposableService(service);
        }
      }

      return services;
    }

    public object Resolve(Type interfacType)
    {
      var service = _root.Resolve(interfacType);

      CaptureDisposableService(service);

      return service;
    }

    public object Resolve(string key)
    {
      var service = _root.Resolve(key);

      CaptureDisposableService(service);

      return service;
    }

    public T Resolve<T>(string key) where T : class
    {
      var service = _root.Resolve<T>(key);

      CaptureDisposableService(service);

      return service;
    }

    private void CaptureDisposableService(object service)
    {
      if(service == null) return;

      var disposableService = service as IDisposable;
      if (disposableService != null)
      {
        if (!ReferenceEquals(this, disposableService) | !ReferenceEquals(_root, disposableService))
        {
          _disposables.Add(disposableService);
        }
      }
    }
  }
}