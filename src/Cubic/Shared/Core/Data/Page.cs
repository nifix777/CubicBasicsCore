using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Cubic.Core.Data
{
  public abstract class Page<T>
  {
    public int Index { get; }
    public abstract IReadOnlyList<T> Values { get; }
  }
}
