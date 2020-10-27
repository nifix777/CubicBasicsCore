using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Threading
{
  public class DirtyEventSource
  {
    private readonly object _source;
    private int _dirty;

    public event EventHandler Event;

    public DirtyEventSource(object source)
    {
      _source = source;
    }

    public void Reset() => Interlocked.Exchange(ref _dirty, 0);

    public void FireOnce()
    {
      if (Interlocked.Exchange(ref _dirty, 1) == 0)
      {
        Event?.Invoke(_source, new EventArgs());
      }
    }
  }
}
