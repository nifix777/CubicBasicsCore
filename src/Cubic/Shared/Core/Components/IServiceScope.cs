using Cubic.Core.Diagnostics;
using System;
using System.Collections.Generic;

namespace Cubic.Core.Components
{
  /// <summary>
  /// The <see cref="System.IDisposable.Dispose"/> method ends the scope lifetime. Once Dispose
  /// is called, any scoped services that have been resolved from
  /// <see cref="Microsoft.Extensions.DependencyInjection.IServiceScope.ServiceProvider"/> will be
  /// disposed.
  /// </summary>
  public interface IServiceScope : IDisposable
  {
    /// <summary>
    /// The <see cref="System.IServiceProvider"/> used to resolve dependencies from the scope.
    /// </summary>
    IServiceProvider ServiceProvider { get; }
  }

  internal class ServiceProviderEngineScope : IServiceScope, IServiceProvider
  {
    // For testing only
    internal Action<object> _captureDisposableCallback;

    private List<IDisposable> _disposables;

    private bool _disposed;

    public ServiceProviderEngineScope(IServiceProviderEngine engine)
    {
      Engine = engine;
    }

    Dictionary<ServiceCacheKey, object> ResolvedServices { get; } = new Dictionary<ServiceCacheKey, object>();

    public IServiceProviderEngine Engine { get; }

    public object GetService(Type serviceType)
    {
      if (_disposed)
      {
        ExceptionHelper.ThrowObjectDisposedException(this);
      }

      return Engine.GetRealService(serviceType, this);
    }

    public IServiceProvider ServiceProvider => this;

    public void Dispose()
    {
      lock (ResolvedServices)
      {
        if (_disposed)
        {
          return;
        }

        _disposed = true;
        if (_disposables != null)
        {
          for (var i = _disposables.Count - 1; i >= 0; i--)
          {
            var disposable = _disposables[i];
            disposable.Dispose();
          }

          _disposables.Clear();
        }

        ResolvedServices.Clear();
      }
    }

    internal object CaptureDisposable(object service)
    {
      _captureDisposableCallback?.Invoke(service);

      if (!ReferenceEquals(this, service))
      {
        if (service is IDisposable disposable)
        {
          lock (ResolvedServices)
          {
            if (_disposables == null)
            {
              _disposables = new List<IDisposable>();
            }

            _disposables.Add(disposable);
          }
        }
      }
      return service;
    }
  }
}
