using System.Collections;
using System.Collections.Generic;

namespace Cubic.Core.Collections
{
  internal class SegmentEnumerator<T> : IEnumerator<T>
  {
    private ISegment<T> _segment;

    private int _max;

    private int _current = 0;

    public SegmentEnumerator( ISegment<T> stringSegment )
    {
      _segment = stringSegment;
      _max = _segment.Offset + _segment.Length;
    }
    public void Dispose()
    {

    }

    public bool MoveNext()
    {
      if ( _segment.Length == 0 ) return false;
      if ( _current == ( _max ) ) return false;

      _current++;

      return true;
    }

    public void Reset()
    {
      _current = 0;
    }

    public T Current => _segment[_current];

    object IEnumerator.Current
    {
      get { return Current; }
    }

  }
}