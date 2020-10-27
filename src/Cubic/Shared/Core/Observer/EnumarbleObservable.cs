using System.Collections.Generic;

namespace Cubic.Core.Observer
{
  public class EnumarbleObservable<T> : ActionObserverable<T>
  {

    private IEnumerable<T> _source;

    public EnumarbleObservable(IEnumerable<T> source )
    {
      _source = source;
    }


    protected override void Do()
    {
      foreach (var item in _source)
      {
        Next(item);
      }
    }
  }
}