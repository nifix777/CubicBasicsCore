using System;

namespace Cubic.Core.Data
{
  public static class DataContainerExtensions
  {
    public static T CastAs<T>(this DataContainer container) where T : DataContainer
    {
      if (container is T variable)
      {
        return variable;
      }

      T t = Activator.CreateInstance<T>();
      DataContainer.FillFrom(t, container);
      return t;
    }
  }
}