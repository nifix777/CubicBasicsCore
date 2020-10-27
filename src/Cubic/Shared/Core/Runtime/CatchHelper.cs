using System;

namespace Cubic.Core.Runtime
{
  public static class CatchHelper
  {
    /// <summary>
    /// Catches the specified Exception-Type <see cref="E"/> and ignore it by execution of <see cref="func"/> .
    /// </summary>
    /// <typeparam name="T">The return-type</typeparam>
    /// <typeparam name="E">Type of Exception, that should be ignored</typeparam>
    /// <param name="func">The function.</param>
    /// <returns>T or the default of <see cref="T"/> when catche Exception</returns>
    public static T Catch<T, E>(this Func<T> func) where E : Exception
    {
      return CatchHelper.Catch<T, E>(func, default(T));
    }

    /// <summary>
    /// Catches the specified Exception-Type <see cref="E" /> and ignore it by execution of <see cref="func" /> .
    /// </summary>
    /// <typeparam name="T">The return-type</typeparam>
    /// <typeparam name="E">Type of Exception, that should be ignored</typeparam>
    /// <param name="func">The function.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>
    /// T or the default of <see cref="T" /> when catche Exception
    /// </returns>
    public static T Catch<T, E>(this Func<T> func, T defaultValue) where E : Exception
    {
      try
      {
        return func();
      }
      catch (E)
      {
        //ignore
        return defaultValue;
      }
    }
  }
}