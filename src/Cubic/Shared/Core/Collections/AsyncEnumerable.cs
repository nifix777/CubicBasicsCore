using Cubic.Core.Collections;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Collections
{
  public class AsyncEnumerable<T> : IAsyncEnumerable<T>
  {

    private Func<IEnumerable<Task<T>>> _taskSelector;

    public AsyncEnumerable(Func<IEnumerable<Task<T>>> taskSelectorFunc)
    {
      _taskSelector = taskSelectorFunc;
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator()
    {
      return new AsyncEnumerator<T>(_taskSelector);
    }
  }

  internal struct ConfiguredAsyncEnumerable<T> : IAsyncEnumerable<T>
  {
    private readonly IAsyncEnumerable<T> _source;

    private readonly CancellationToken _cancellation;

    private readonly bool _captureContext;

    public ConfiguredAsyncEnumerable(IAsyncEnumerable<T> soruce, CancellationToken cancellation, bool captureContext) : this()
    {
      _source = soruce;
      this._cancellation = cancellation;
      this._captureContext = captureContext;
    }

    //public ConfiguredAsyncEnumerable<T>(IAsyncEnumerable<T> source, CancellationToken token = default(CancellationToken), bool catpureContext = false)
    //{
    //  _source = source;
    //}

    public IAsyncEnumerator<T> GetAsyncEnumerator()
    {
      return new ConfiguredAsyncEnumerator<T>(_source, _cancellation, _captureContext);
    }
  }

  internal struct ConfiguredAsyncEnumerator<T> : IAsyncEnumerator<T>
  {
    private readonly IAsyncEnumerator<T> _enumerator;
    private readonly IAsyncEnumerable<T> _source;
    private readonly CancellationToken _token;
    private readonly bool _captureContext;

    public ConfiguredAsyncEnumerator(IAsyncEnumerable<T> source, CancellationToken token, bool captureContext) : this()
    {
      _source = source;
      _token = token;
      _captureContext = captureContext;
      _enumerator = _source.GetAsyncEnumerator();
    }

    public T Current => _enumerator.Current;

    public void Dispose()
    {
      _enumerator.Dispose();
    }

    public async Task<bool> MoveNextAsync(CancellationToken token)
    {
      return await _enumerator.MoveNextAsync(_token).ConfigureAwait(_captureContext);
    }
  }
}

