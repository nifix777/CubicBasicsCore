using System;
using Cubic.Core.Reflection;

namespace Cubic.Core.Components
{
  public class LazyResolver<T> : BaseResolver where T : class
  {
    private Lazy<T> _instance;

    private object ParameterResolverFunc(Type type)
    {
      if(type == typeof(IContainer)) return _container;

      return _container.Resolve(type);
    }

    public LazyResolver(IContainer container, Type serviceType) : base(container, serviceType)
    {
      _instance = new Lazy<T>(()=> RefelctionUtils.Create<T>(ParameterResolverFunc));
    }

    public override object Resolve()
    {
      return _instance.Value;
    }
  }

  public static class LazyResolverExtensions
  {
    public static void Lazy<I, T>(this IContainer container) where T : class
    {
      container.Use(new LazyResolver<T>(container, typeof(I)));
    }
  }
}