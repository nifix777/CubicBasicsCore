using System;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Components
{
  public class FactoryResolver<T> : BaseResolver, IResolver where T : class 
  {
    private Func<IContainer , object> _factory;

    public FactoryResolver( IContainer container, Func<IContainer , T> instanceFactory ) : base(container, typeof(T))
    {

      Guard.ArgumentNull(instanceFactory, nameof(instanceFactory));

      _factory = instanceFactory;
    }

    public override object Resolve()
    {
      return _factory(_container);
    }

    public override void Dispose()
    {
      
    }
  }

  public static class FactoryResolverFunctions
  {
    public static void RegisterFactory<I>( this IContainer container, Func<IContainer , I> instanceFactory ) where I : class
    {
      container.Use( new FactoryResolver<I>( container, instanceFactory ) );
    }
  }
}