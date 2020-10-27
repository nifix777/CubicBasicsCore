using System;
using Cubic.Core.Diagnostics;

namespace Cubic.Core
{
  public class Subscription : IDisposable
  {
    private Action _disposeCallback;

    public Subscription(Action disposeCallback)
    {
      Guard.AgainstNull(disposeCallback, nameof(disposeCallback));
      _disposeCallback = disposeCallback;
    }
    public void Dispose()
    {
      _disposeCallback();
    }
  }
}