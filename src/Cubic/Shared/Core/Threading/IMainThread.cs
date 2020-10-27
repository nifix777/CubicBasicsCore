using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Threading
{
  public interface IMainThread
  {
    /// <summary>
    /// Main (UI) thread id. 
    /// </summary>
    int ThreadId { get; }

    /// <summary>
    /// Posts cancellable action on UI thread.
    /// </summary>
    /// <param name="action"></param>
    void Post(Action action);

    /// <summary>
    /// Creates main thread awaiter implementation
    /// </summary>
    IMainThreadAwaiter CreateMainThreadAwaiter(CancellationToken cancellationToken);
  }
}
