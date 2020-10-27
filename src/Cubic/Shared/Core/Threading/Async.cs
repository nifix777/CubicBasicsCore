using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Cubic.Core.Threading
{
  public class Async
  {
    public static IAsyncInfo Run<TProgress>(Func<CancellationToken, Task> taskProvider, CancellationTokenSource tokenSource = null) => new AsyncInfo(taskProvider, tokenSource ?? new CancellationTokenSource());
    public static IAsyncInfoProgress<TProgress> Run<TProgress>(Func<CancellationToken, IProgress<TProgress>, Task> taskProvider, IProgress<TProgress> progress, CancellationTokenSource tokenSource = null) => new AsyncInfoProgress<TProgress>(taskProvider, tokenSource ?? new CancellationTokenSource(), progress);
  }

  public interface IAsyncInfo
  {
    void Cancel();

    Task Task { get; }
  }

  public interface IAsyncInfo<TResult>
  {
    void Cancel();

    Task<TResult> Task { get; }
  }

  public interface IAsyncInfoProgress<TProgres> : IAsyncInfo
  {
    IProgress<TProgres> Progress { get; }
  }

  public interface IAsyncInfoProgress<TResult, TProgres> : IAsyncInfo<TResult>
  {
    IProgress<TProgres> Progress { get; }
  }
}
