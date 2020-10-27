using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cubic.Core.Observer
{
  public static class ObserverableFunctions
  {
    public static IEnumerable<T> Interval<T> (this IEnumerable<T> source, TimeSpan interval)
    {
      return new IntervalEnumerable<T>(source, interval);
    }
  }
}