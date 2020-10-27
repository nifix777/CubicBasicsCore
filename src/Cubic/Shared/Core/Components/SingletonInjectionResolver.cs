using System;
using Cubic.Core.Reflection;

namespace Cubic.Core.Components
{
  public class SingletonInjectionResolver : BaseResolver
  {
    //private IContainer _container;
    private readonly Type _resolvingType;
    private Lazy<object> _lazy;

    public SingletonInjectionResolver(IContainer container, Type serviceType, Type resolvingType) : base(container, serviceType)
    {
      _container = container;
      _resolvingType = resolvingType;

      if (_resolvingType.IsInterface)
      {
        throw new InvalidOperationException(string.Format("Interface Type {0} cant be resolved!", _resolvingType.Name));
      }

      _lazy = new Lazy<object>(ValueFactory);
    }

    private object ValueFactory()
    {
      return RefelctionUtils.Create(_resolvingType, type => _container.Resolve(type));
    }

    public override object Resolve()
    {
      return _lazy.Value;
    }
  }
}