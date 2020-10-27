using System;

namespace Cubic.Core.Components
{
  public class SingletonResolver<T> : BaseResolver, IResolver where T : class 
  {

    private Lazy<T> _lazy;
    //private IResetLazy<T> _lazy;


    public SingletonResolver(IContainer container, Func<IContainer, T> factory ) : base (container, typeof(T))
    {
      _lazy = new Lazy<T>(() => factory(_container));
      //_lazy = new ResetLazy<T>( () => factory(_container) );
    }

    public override object Resolve()
    {
      return _lazy.Value;
    }
  }

  public static class SingletonResolverFunctions
  {

    public static void SingletonKey<T>( this IContainer container, string key, Func<IContainer , T> factory ) where T : class
    {
      container.Register( key, new SingletonResolver<T>( container , factory ) );
    }

    public static void Singleton<I, T>(this IContainer container, T instance) where T : class, I where I : class
    {
      container.Use(new SingletonResolver<I>(container, container1 => instance));
    }

    public static void Singleton<T>( this IContainer container , Func<IContainer , T> factory ) where T : class
    {
      container.Use( new SingletonResolver<T>( container , factory ) );
    }
  }
}