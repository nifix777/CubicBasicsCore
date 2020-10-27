using System.Threading;

namespace Cubic.Core.Threading
{
  public struct MainThreadAwaitable
  {
    private readonly IMainThread _mainThread;
    private readonly CancellationToken _cancellationToken;

    public MainThreadAwaitable(IMainThread mainThread, CancellationToken cancellationToken = default(CancellationToken))
    {
      _mainThread = mainThread;
      _cancellationToken = cancellationToken;
    }

    public MainThreadAwaiter GetAwaiter() => new MainThreadAwaiter(_mainThread, _cancellationToken);
  }
}
