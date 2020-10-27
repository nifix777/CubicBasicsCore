using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Cubic.Core.Annotations;
using Cubic.Core.Collections;
using Cubic.Core.Diagnostics;
using Cubic.Core.Reflection;

namespace Cubic.Core.Threading
{
  public static class TaskEx
  {

    private const int s_defaultTimeout = 5000;

    private static Task<bool> _true = Task.FromResult(true);
    private static Task<bool> _false = Task.FromResult(false);
    private static Task<object> _empty = Task.FromResult<object>(null);



    public static Task Completed
    {
      get { return _true; }
    }

    public static Task<bool> False
    {
      get { return _false; }
    }

    public static Task<bool> True
    {
      get { return _false; }
    }

    public static Task<object> Empty
    {
      get
      {
        return _empty;
      }
    }


    public static Task<T> WrapAsync<T>(this T value)
    {
      return Task.FromResult(value);
    }

    public static Task<TResult> FromError<TResult>(Exception exception)
    {
      TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
      tcs.SetException(exception);
      return tcs.Task;
    }

    public static Task<T> WrapAsync<T>(Func<IAsyncResult> beginFunc, Func<T> returnFunc)
    {
      var tcs = new TaskCompletionSource<T>();

      try
      {
        beginFunc();

        tcs.TrySetResult(returnFunc());
      }
      catch (OperationCanceledException)
      {
        tcs.TrySetCanceled();
      }
      catch (Exception ex)
      {
        tcs.TrySetException(ex);
      }

      return tcs.Task;
    } 

    public static async Task CatchCancel(this Task task)
    {
      await task.Catch<TaskCanceledException>();
    }

    public static async Task<T> CatchCancel<T>( this Task<T> task )
    {
      return await task.Catch<T,TaskCanceledException>();
    }
    public static async Task Catch<E>( this Task task , Action<E> onError = null ) where E : Exception
    {
      try
      {
        await task;
      }
      catch ( E ex )
      {
        onError?.Invoke( ex);
      }

    }

    public static async Task<T> Catch<T,E>( this Task<T> task , Action<E> onError = null ) where E : Exception
    {
      try
      {
        return await task;
      }
      catch ( E ex )
      {
        onError?.Invoke( ex );
        return (T) await Task.FromResult(typeof (T).GetDefault());
      }

    }

    public static async Task<T> Catch<T>( this Task<T> task , Action<Exception> onError = null )
    {
      try
      {
        return await task;
      }
      catch ( AggregateException ex )
      {
        onError?.Invoke( ex.Flatten().InnerException );
        return (T) typeof (T).GetDefault();
      }

      catch ( Exception ex )
      {
        onError?.Invoke( ex );
        return ( T ) typeof( T ).GetDefault();
      }
    }
    public static async Task Catch( this Task task , Action<Exception> onError = null )
    {
      try
      {
        await task;
      }
      catch ( AggregateException ex )
      {
        onError?.Invoke( ex.Flatten().InnerException );
      }

      catch ( Exception ex )
      {
        onError?.Invoke( ex );
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Forget(this Task task)
    {
      //Empty
    }

    public static async void FireAndForgett(this Task task, Action<Exception> onError = null  )
    {
      try
      {
        await task;
      }
      catch (AggregateException ex)
      {
        onError?.Invoke(ex.Flatten().InnerException);
      }

      catch ( Exception ex )
      {
        onError?.Invoke(ex);
      }
    }

    public static Task OrTimeout(this Task task, int milliseconds = s_defaultTimeout)
    {
      return OrTimeout(task, new TimeSpan(0, 0, 0, 0, milliseconds));
    }

    public static async Task OrTimeout(this Task task, TimeSpan timeout)
    {
      var completed = await Task.WhenAny(task, Task.Delay(timeout));
      if (completed != task)
      {
        throw new TimeoutException();
      }

      await task;
    }

    public static Task<T> OrTimeout<T>(this Task<T> task, int milliseconds = s_defaultTimeout)
    {
      return OrTimeout(task, new TimeSpan(0, 0, 0, 0, milliseconds));
    }

    public static async Task<T> OrTimeout<T>(this Task<T> task, TimeSpan timeout)
    {
      var completed = await Task.WhenAny(task, Task.Delay(timeout));
      if (completed != task)
      {
        throw new TimeoutException();
      }

      return await task;
    }



    public static async Task NoThrow(this Task task)
    {
      await new NoThrowAwaiter(task);
    }


    //public static async Task ForEachAsync<T>(this IEnumerable<T> source, Action<T> action)
    //{
    //  var tasks = source.Select(arg => Task.Run(() => action(arg)));

    //  await Task.WhenAll(tasks);
    //}

    public static IEnumerable<Task<T>> AsAsync<T>(this IEnumerable<T> source, Func<T, T> func, bool spwnNewTask = false )
    {
      if (spwnNewTask)
      {
        return source.Select( t => Task.Run( () => func( t ) ) );
      }

      return source.Select(t => func(t).WrapAsync());
    }

    public static IEnumerable<Task<T>> AsAsync<T>( this IEnumerable<T> source , Func<T, Task<T>> func)
    {
      return source.Select( func);
    }

    #region AsyncEnumarable

    public static IAsyncEnumerable<T> AsAsyncEnumerable<T>( IEnumerable<T> source )
    {
      var enumerable = source as IList<T> ?? source.ToList();
      return new AsyncEnumerable<T>( () => enumerable.AsAsync(t => t));
    }

    [DebuggerStepThrough]
    public static ConfiguredTaskAwaitable<TResult> AnyContext<TResult>(this Task<TResult> task)
    {
      return task.ConfigureAwait(continueOnCapturedContext: false);
    }

    [DebuggerStepThrough]
    public static ConfiguredTaskAwaitable AnyContext(this Task task)
    {
      return task.ConfigureAwait(continueOnCapturedContext: false);
    }

    #endregion


    public static void RunSafe(Action action, out Exception error)
    {
      error = null;

      try
      {
        Task.Run(action).GetAwaiter().GetResult();
      }
      catch (AggregateException aggrEx)
      {
        error = aggrEx.Flatten().InnerException;
      }
      catch (Exception ex)
      {
        error = ex;
      }

    }

    public static void RunSafe(Action action, CancellationToken cancellationToken, out Exception error)
    {
      error = null;

      try
      {
        Task.Run(action, cancellationToken).GetAwaiter().GetResult();
      }
      catch (AggregateException aggrEx)
      {
        error = aggrEx.Flatten().InnerException;
      }
      catch (Exception ex)
      {

        error = ex;
      }

    }

    public static void RunSafe(Action action, CancellationToken cancellationToken, TimeSpan timeout, out Exception error)
    {
      error = null;

      try
      {
        Task.Run(action, timeout.ToCancellationToken()).GetAwaiter().GetResult();
      }
      catch (AggregateException aggrEx)
      {
        error = aggrEx.Flatten().InnerException;
      }
      catch (Exception ex)
      {

        error = ex;
      }

    }

    public static async Task ForEachAsync<T>(this IEnumerable<T> source, Action<T> callback)
    {
      foreach (var item in source)
      {
        await Task.Yield();
        callback(item);
      }
    }

    public static async Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> callback)
    {
      foreach (var item in source)
      {
        await callback(item);
      }
    }

    public static IAsyncEnumerable<T> Configure<T>(this IAsyncEnumerable<T> soruce, CancellationToken cancellation = default(CancellationToken), bool captureContext = false)
    {
      return new ConfiguredAsyncEnumerable<T>(soruce, cancellation, captureContext);
    }

    public static Task RunWithoutSynchronizationContext([NotNull] Func<Task> func)
    {
      Task task;
      if (func == null)
      {
        throw new ArgumentNullException("func");
      }
      SynchronizationContext current = SynchronizationContext.Current;
      SynchronizationContext.SetSynchronizationContext(null);
      try
      {
        task = func();
      }
      finally
      {
        SynchronizationContext.SetSynchronizationContext(current);
      }
      return task;
    }

    public static Task<T> RunWithoutSynchronizationContext<T>([NotNull] Func<Task<T>> func)
    {
      Task<T> task;
      if (func == null)
      {
        throw new ArgumentNullException("func");
      }
      SynchronizationContext current = SynchronizationContext.Current;
      SynchronizationContext.SetSynchronizationContext(null);
      try
      {
        task = func();
      }
      finally
      {
        SynchronizationContext.SetSynchronizationContext(current);
      }
      return task;
    }


    public static async Task Timebox(this IEnumerable<Task> tasks, TimeSpan timeoutAfter, string messageWhenTimeboxReached)
    {
      using (var tokenSource = new CancellationTokenSource())
      {
        var delayTask = Task.Delay(Debugger.IsAttached ? TimeSpan.MaxValue : timeoutAfter, tokenSource.Token);
        var allTasks = Task.WhenAll(tasks);

        var returnedTask = await Task.WhenAny(delayTask, allTasks).ConfigureAwait(false);
        tokenSource.Cancel();
        if (returnedTask == delayTask)
        {
          throw new TimeoutException(messageWhenTimeboxReached);
        }

        await allTasks.ConfigureAwait(false);
      }
    }


    public static bool IsStillAlive(this Task task)
    {
      return !task.IsCompleted && !task.IsCanceled && !task.IsFaulted;
    }
  }
}