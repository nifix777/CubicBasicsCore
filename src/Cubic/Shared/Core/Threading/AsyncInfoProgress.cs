using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Cubic.Core.Threading
{
  internal class AsyncInfoProgress<TProgress> : IAsyncInfoProgress<TProgress>
  {
    private readonly IProgress<TProgress> progress;

    private readonly Func<CancellationToken, IProgress<TProgress>, Task> taskProvider;

    private readonly CancellationTokenSource tokenSource;

    public AsyncInfoProgress(Func<CancellationToken, IProgress<TProgress>, Task> taskProvider, CancellationTokenSource tokenSource, IProgress<TProgress> progress)
    {
      this.taskProvider = taskProvider ?? throw new ArgumentNullException(nameof(taskProvider));
      this.tokenSource = tokenSource ?? throw new ArgumentNullException(nameof(tokenSource));
      this.progress = progress ?? throw new ArgumentNullException(nameof(progress));
    }

    public IProgress<TProgress> Progress => progress;

    public Task Task => taskProvider(tokenSource.Token, progress);

    public void Cancel()
    {
      tokenSource.Cancel();
    }
  }
}
