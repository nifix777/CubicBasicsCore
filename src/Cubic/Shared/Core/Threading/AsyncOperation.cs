using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Threading
{
  public class AsyncOperation : IDisposable
  {
    private Task _task;

    private Func<CancellationToken, Task> _asyncCall;

    private readonly CancellationTokenSource _tokenSource;

    public AsyncOperation(Func<CancellationToken, Task> asyncCall)
    {
      _asyncCall = asyncCall;
      _tokenSource = new CancellationTokenSource();
    }

    public void Call()
    {
      _task = _asyncCall(_tokenSource.Token);
    }

    public void Cancel()
    {
      _tokenSource.Cancel();
    }

    public void Complete()
    {
      _task.Wait();
    }

    public void Dispose()
    {
      _tokenSource?.Cancel();
      _tokenSource?.Dispose();
      
      _task?.Dispose();
    }
  }
}
