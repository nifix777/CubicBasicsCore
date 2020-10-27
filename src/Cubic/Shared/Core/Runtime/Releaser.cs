using System;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Runtime
{
  public sealed class Releaser : IDisposable
  {
    private WeakAction _releaseAction;

    public Releaser(Action releaseAction)
    {
      Guard.AgainstNull(releaseAction, nameof(releaseAction));
      _releaseAction = new WeakAction(releaseAction);
    }

    public void Dispose()
    {
      if (_releaseAction.IsAlive)
      {
        _releaseAction.Execute();
      }
    }
  }
}