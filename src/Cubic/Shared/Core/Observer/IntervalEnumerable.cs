using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Cubic.Core.Observer
{


  public class IntervalEnumerable<T> : IEnumerable<T>
  {
    private IEnumerable<T> _source;

    private TimeSpan _interval;
    public IntervalEnumerable( IEnumerable<T> source , TimeSpan interval )
    {
      _interval = interval;
      _source = source;
    }

    public IEnumerator<T> GetEnumerator()
    {
      return new IntervalEnumerator<T>( _source , _interval );
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
  public class IntervalEnumerator<T> : IEnumerator<T>
  {
    private IEnumerator<T> _source;
    private TimeSpan _interval;
    public IntervalEnumerator(IEnumerable<T> source, TimeSpan interval )
    {
      _source = source.GetEnumerator();
      _interval = interval;
    }

    public void Dispose()
    {
      _source.Dispose();
    }

    public bool MoveNext()
    {
      Thread.Sleep(_interval);
      return _source.MoveNext();
    }

    public void Reset()
    {
      _source.Reset();
    }

    public T Current
    {
      get { return _source.Current; }
    }

    object IEnumerator.Current
    {
      get { return _source.Current; }
    }
  }

}