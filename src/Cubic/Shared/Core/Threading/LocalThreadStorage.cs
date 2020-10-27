using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Threading
{
  public class LocalThreadStorage<T>
  {
    [ThreadStatic]
    private T _localData;

    public T Value
    {
      get => _localData;
      set => _localData = value;
    }
  }
}
