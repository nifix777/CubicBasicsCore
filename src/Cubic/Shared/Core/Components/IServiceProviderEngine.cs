using System;
using System.Collections.Generic;

namespace Cubic.Core.Components
{
  public interface IServiceProviderEngine : IDisposable
  {
    event EventHandler<ServiceDescriptor> OnCreate;

    event EventHandler<ServiceDescriptor> OnResolve;
    object GetRealService(Type serviceType);

    object GetRealService(Type serviceType, IServiceScope serviceScope);
  }
}
