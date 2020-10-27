using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;


namespace Cubic.Core.Threading
{
  /// <summary>
  /// TODO: 
  /// </summary>
  public class JoinableThreadContext
  {
    private static JoinableThreadContext _joinableThreadContext;

    private static object _sync = new object();




    public static JoinableThreadContext Instance
    {
      get
      {
        var mainthread = _joinableThreadContext;
        if (mainthread == null)
        {
          lock (_sync)
          {
            mainthread = _joinableThreadContext;

            if (mainthread == null)
            {
              _joinableThreadContext = new JoinableThreadContext();
            }
          }

        }

        return _joinableThreadContext;
      }
    }


    private readonly Thread _mainThread;

    private readonly int _mainThreadId;

    private readonly SynchronizationContext _synchronizationContext;

    public JoinableThreadContext(Thread mainThread = null, SynchronizationContext synchronizationContext = null)
    {
      _mainThread = mainThread ?? Thread.CurrentThread;
      _mainThreadId = _mainThread.ManagedThreadId;
      _synchronizationContext = synchronizationContext ??  SynchronizationContext.Current; // may still be null
    }

    public bool IsOnMain
    {
      get
      {
        if (_mainThreadId == int.MinValue) throw new NotSupportedException("you have to call Instance first");
        return Environment.CurrentManagedThreadId == _mainThreadId;
      }
    }

    public Task Queue(Action action)
    {
      var tcs = new TaskCompletionSource<object>();

      _synchronizationContext.Post((state) => {
        try
        {
          action?.Invoke();
          tcs.SetResult(null);
        }
        catch (Exception ex)
        {
          tcs.SetException(ex);
        }
      }, null);
      return tcs.Task;
    }

    public Task SwitchToMainThreadAsync()
    {
      if (IsOnMain) return TaskEx.Completed;

      var tcs = new TaskCompletionSource<bool>();

      _synchronizationContext.Post(state => tcs.SetResult(true), null );

      return tcs.Task;
    }





  }
}