using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cubic.Core.Threading;

namespace Cubic.Core.Collections
{
  public class AsyncEnumerator<T> : IAsyncEnumerator<T>
  {
    private Func<IEnumerable<Task<T>>>  taskSelector;

    private IEnumerator<Task<T>> _taskEnumerator; 

    public AsyncEnumerator( Func<IEnumerable<Task<T>>> taskSelectorFunc)
    {
      taskSelector = taskSelectorFunc;
    }
    public void Dispose()
    {
      taskSelector = null;
    }

    public T Current { get; private set; }

    public async Task<bool> MoveNextAsync(CancellationToken token)
    {
      if (_taskEnumerator == null)
      {
        this.Reset();
      }

      if (_taskEnumerator == null) return false;

      if (_taskEnumerator.MoveNext())
      {
        await _taskEnumerator.Current;
        return true;
      }

      return false;
    }

    public void Reset()
    {
      _taskEnumerator = taskSelector().GetEnumerator();
    }
  }

  internal sealed class AsyncEnumeratorWrapper<T> : IAsyncEnumerator<T>
  {
    private IEnumerator<T> _enumerator;
    private bool _runSynchronously;

    public AsyncEnumeratorWrapper(IEnumerator<T> enumerator, bool runSynchronously)
    {
      _enumerator = enumerator;
      _runSynchronously = runSynchronously;
    }

    public T Current => _enumerator.Current;

    public Task<bool> MoveNextAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
      if (_runSynchronously)
      {
        var result = _enumerator.MoveNext();
        return result ? TaskEx.True : TaskEx.False;
      }
      else
      {
        return Task.Run(() => _enumerator.MoveNext(), cancellationToken);
      }
    }

    public void Dispose()
    {
      _enumerator.Dispose();
    }
  }
}