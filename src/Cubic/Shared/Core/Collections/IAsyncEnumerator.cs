using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Collections
{
  public interface IAsyncEnumerator<T> : IDisposable
  {
    T Current { get; }

    Task<bool> MoveNextAsync(CancellationToken token);
  }

  public interface IAsyncEnumerator2<T> : IEnumerator<Task<T>>
  {

  }

  public class AsyncEnumrator2<TResult, TState> : IAsyncEnumerator2<TResult>
  {
    private readonly Func<TState, Task<TResult>> _getNextResultAsync;
    private readonly TState _state;

    public AsyncEnumrator2(Func<TState, Task<TResult>> getNextResultAsync, TState state)
    {
      _getNextResultAsync = getNextResultAsync;
      _state = state;
    }

    public virtual void Dispose()
    {
      
    }

    public virtual bool MoveNext()
    {
      Current = _getNextResultAsync(_state);
      if (Current == null) return false;
      return true;
    }

    public virtual void Reset()
    {
      throw new NotImplementedException();
    }

    public Task<TResult> Current { get; set; }

    object IEnumerator.Current => Current;
  }

}