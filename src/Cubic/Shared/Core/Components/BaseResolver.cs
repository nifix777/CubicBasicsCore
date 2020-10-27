using System;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Components
{
  public abstract class BaseResolver : IResolver
  {
    protected IContainer _container;

    protected BaseResolver(IContainer container, Type serviceType)
    {
      Guard.ArgumentNull(container, nameof(container));
      Guard.ArgumentNull(serviceType, nameof(serviceType));

      _container = container;
      ServiceType = serviceType;
    }

    public abstract object Resolve();
    public Type ServiceType { get; }
    public virtual void Dispose()
    {
      
    }
  }
}