using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Cubic.Core.Threading
{
  internal class AsyncInfo : IAsyncInfo
  {
    private readonly Func<CancellationToken, Task> taskProvider;

    private readonly CancellationTokenSource tokenSource;

    public AsyncInfo(Func<CancellationToken, Task> taskProvider, CancellationTokenSource tokenSource)
    {
      this.taskProvider = taskProvider ?? throw new ArgumentNullException(nameof(taskProvider));
      this.tokenSource = tokenSource ?? throw new ArgumentNullException(nameof(tokenSource));
    }

    public Task Task => taskProvider(tokenSource.Token);

    public void Cancel()
    {
      tokenSource.Cancel();
    }
  }
}
