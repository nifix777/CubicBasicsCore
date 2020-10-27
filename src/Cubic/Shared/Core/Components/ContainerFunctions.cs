using System;
using System.Linq;
using System.Reflection;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Components
{
  public static class ContainerFunctions
  {
    public static T ResolveName<T>(this IContainer container) where T : class
    {
      return container.Resolve(typeof (T).Name) as T;
    }

    public static IContainer UseAssemlby<T>(this IContainer container, Assembly assembly, Predicate<Type> filter = null) where T : class
    {
      var interfaceType = typeof (T);
      var types = assembly.GetExportedTypes().Where(t => interfaceType.IsAssignableFrom(t)).Where(t => !t.IsAbstract).OrderBy(t => t.Name).ToArray();

      if (filter != null)
      {
        types = types.Where(filter.Invoke).OrderBy(t => t.Name).ToArray();
      }

      foreach (var type in types)
      {
        container.Use(new InjectionResolver(container, interfaceType, type ));
      }


      return container;
    }

    public static ScopedContainer CreateScoped(this IContainer container)
    {
      Guard.ArgumentNull(container, nameof(container));
      return new ScopedContainer(container);
    }
  }
}