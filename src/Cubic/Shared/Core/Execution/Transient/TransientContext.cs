using System;
using System.Collections.Generic;
using System.Linq;

namespace Cubic.Core.Execution.Transient
{
  public abstract class TransientContext : ITransientService
  {
    private readonly HashSet<IDisposable> _transients;

    protected TransientContext()
    {
      _transients = new HashSet<IDisposable>();
    }

    public virtual void Dispose()
    {
      foreach (var disposable in _transients)
      {
        disposable.Dispose();
      }
      _transients.Clear();
    }

    public void Register(IDisposable transiant)
    {
      if (!_transients.Contains(transiant))
      {
        _transients.Add(transiant);
      }
    }

    public void Unregister(IDisposable transiant)
    {
      if (_transients.Contains(transiant))
      {
        _transients.Remove(transiant);
      }
    }
  }
}