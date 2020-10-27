using System;

namespace Cubic.Core.Threading
{
  public interface ISyncLock
  {
    /// <summary>
    /// Enters the Monitor. Dispose the result to exit.
    /// </summary>
    IDisposable Enter();
  }
}