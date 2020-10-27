using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Runtime
{
  public interface IAsyncProgressAction<TProgress>
  {

    IProgress<TProgress> Progress { get; }
    
    Task GetResult(CancellationToken ct = default(CancellationToken));
  }

  public class AsyncProgressActionBase : IAsyncProgressAction<int>
  {
    private readonly IProgress<int> _progress;

    private readonly Func<CancellationToken, IProgress<int>, Task> _asyncFunc;

    public AsyncProgressActionBase(Func<CancellationToken, IProgress<int>, Task> asyncFunc, IProgress<int> progress)
    {
      _asyncFunc = asyncFunc;
      _progress = progress;
    }

    public IProgress<int> Progress => _progress;

    public Task GetResult(CancellationToken ct = default(CancellationToken))
    {
      return _asyncFunc(ct, _progress);
    }
  }
}