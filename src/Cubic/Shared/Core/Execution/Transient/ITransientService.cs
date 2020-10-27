using System;

namespace Cubic.Core.Execution.Transient
{
  public interface ITransientService : IDisposable
  {
    void Register(IDisposable transiant);

    void Unregister(IDisposable transiant);
  }
}