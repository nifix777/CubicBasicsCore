using System;
using System.Threading;

namespace Cubic.Core
{
  public class ProgressBase<T> : IProgressBase<T> where T : class
  {
    private T _token;

    public ProgressBase()
    {
      _token = default(T);
    }

    public ProgressBase(T token)
    {
      _token = token;
    }

    public T Token => _token;

    public void Report(T value)
    {
      Interlocked.Exchange( ref _token, value );

      FireChanged();
    }


    public event EventHandler<T> OnChange;

    protected void FireChanged()
    {
      OnChange?.Invoke(this, _token);
    }
  }
}