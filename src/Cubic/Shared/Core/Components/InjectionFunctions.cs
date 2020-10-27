using System;

namespace Cubic.Core.Components
{
  public static class InjectionFunctions
  {
    public static IContainer Register<I, T>(this IContainer container) where I : class where T : class 
    {
      container.Use(new InjectionResolver(container, typeof(I), typeof(T)));
      return container;
    }
    public static IContainer Register(this IContainer container, Type serviceType, Type resolvingType)
    {
      container.Use(new InjectionResolver(container, serviceType, resolvingType));
      return container;
    }

    public static IContainer Register(this IContainer container, string key, Type serviceType, Type resolvingType)
    {
      container.Register(key, new InjectionResolver(container, serviceType, resolvingType));
      return container;
    }

    public static IContainer Singleton<I, T>(this IContainer container) where I : class where T : class
    {
      container.Use(new SingletonInjectionResolver(container, typeof(I), typeof(T)));
      return container;
    }

    public static IContainer Singleton(this IContainer container, Type serviceType, Type resolvingType)
    {
      container.Use(new SingletonInjectionResolver(container, serviceType, resolvingType));
      return container;
    }

    public static IContainer Singleton(this IContainer container,string key, Type serviceType, Type resolvingType)
    {
      container.Register(key ,new SingletonInjectionResolver(container, serviceType, resolvingType));
      return container;
    }
  }
}