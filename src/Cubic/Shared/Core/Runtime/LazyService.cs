using System;
using System.Globalization;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Runtime
{
  public static class LazyServices
  {
    public static T GetNotNullValue<T>(this Lazy<T> lazy, string argument) where T : class
    {
      Guard.AgainstNull(lazy);
      T value = lazy.Value;
      if (value == null)
      {
        throw new InvalidOperationException(
            string.Format(CultureInfo.CurrentCulture, "Lazy Value of Type '{0}' is NULL. {1}", typeof(T), argument));
      }

      return value;
    }
  }
}