using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Components
{
  public class Injection
  {
    private readonly Type _serviceImplementation;

    public Injection(Type serviceImplementation)
    {
      _serviceImplementation = serviceImplementation;
    }

    public object Callback(IServiceContainer container, Type serviceType)
    {
      return ServiceContainerExtensions.InjectionCallback(container, _serviceImplementation);
    }

  }

}
