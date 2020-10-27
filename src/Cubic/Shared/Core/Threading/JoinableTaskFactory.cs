using Cubic.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Threading
{
  public class JoinableTaskFactory
  {
    private readonly IMainThread _mainThread;

    public JoinableTaskFactory(IMainThread mainThread)
    {
      _mainThread = mainThread;
    }

    public MainThreadAwaitable SwitchToMainThreadAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
      Guard.ArgumentNull(_mainThread, "Mainthread");

      return _mainThread.SwitchToAsync();
    }
  }
}
