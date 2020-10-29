using System;
using System.Threading;
using System.Threading.Tasks;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Execution
{
  public class RetryPolicy
  {
    private int _retryCount;

    public RetryPolicy NoRetry = new RetryPolicy(RetryCountStrategy.NoRetry, new AllErrorStrategy());

    protected RetryPolicy()
    {
      _retryCount = 0;
    }

    internal RetryPolicy(IRetryStrategy strategy, ITransientErrorDetectionStrategy errorDetectionStrategy) : this()
    {
      Guard.AgainstNull(strategy, nameof(strategy));
      Guard.AgainstNull(errorDetectionStrategy, nameof(errorDetectionStrategy));
      Strategy = strategy;
      ErrorDetectionStrategy = errorDetectionStrategy;
    }

    public event EventHandler<EventArgs> Retrying;

    public IRetryStrategy Strategy { get; }

    public ITransientErrorDetectionStrategy ErrorDetectionStrategy { get; }

    public void ExecuteAction(Action action)
    {
      try
      {        
        action();
      }
      catch (Exception e)
      {

        if (ErrorDetectionStrategy.IsTransiant(e))
        {
          _retryCount++;
          var shouldRetry = Strategy.GetShouldRetry();

          TimeSpan delay;
          if (shouldRetry(_retryCount, e, out delay))
          {
            Thread.Sleep(delay);
            ExecuteAction(action);
          }
        }

        throw;
      }
    }

    public TResult ExecuteAction<TResult>(Func<TResult> function)
    {
      try
      {
        return function();
      }
      catch (Exception e)
      {

        if (ErrorDetectionStrategy.IsTransiant(e))
        {
          _retryCount++;
          var shouldRetry = Strategy.GetShouldRetry();

          TimeSpan delay;
          if (shouldRetry(_retryCount, e, out delay))
          {
            Thread.Sleep(delay);
            return ExecuteAction(function);
          }
        }

        throw;
      }
    }

    public async Task ExecuteActionAsync(Func<Task> task)
    {
      try
      {
        await task();
      }
      catch (Exception e)
      {

        if (ErrorDetectionStrategy.IsTransiant(e))
        {
          _retryCount++;
          var shouldRetry = Strategy.GetShouldRetry();

          TimeSpan delay;
          if (shouldRetry(_retryCount, e, out delay))
          {
            await Task.Delay(delay);
            await ExecuteActionAsync(task);
          }
        }

        throw;
      }
    }

    public async Task ExecuteActionAsync(Func<Task> task, CancellationToken ct)
    {
      try
      {
        if(ct.IsCancellationRequested) return;
        await task();
      }
      catch (Exception e)
      {

        if (ErrorDetectionStrategy.IsTransiant(e))
        {
          _retryCount++;
          var shouldRetry = Strategy.GetShouldRetry();

          TimeSpan delay;
          if (shouldRetry(_retryCount, e, out delay))
          {
            if (ct.IsCancellationRequested) return;
            await Task.Delay(delay);
            await ExecuteActionAsync(task, ct);
          }
        }

        throw;
      }
    }

    public async Task<TResult> ExecuteActionAsync<TResult>(Func<Task<TResult>> task)
    {
      try
      {
        return await task();
      }
      catch (Exception e)
      {

        if (ErrorDetectionStrategy.IsTransiant(e))
        {
          _retryCount++;
          var shouldRetry = Strategy.GetShouldRetry();

          TimeSpan delay;
          if (shouldRetry(_retryCount, e, out delay))
          {
            await Task.Delay(delay);
            return await ExecuteActionAsync(task);
          }
        }

        throw;
      }
    }

    public async Task<TResult> ExecuteActionAsync<TResult>(Func<Task<TResult>> task, CancellationToken ct)
    {
      try
      {
        if(ct.IsCancellationRequested) return default(TResult);
        return await task();
      }
      catch (Exception e)
      {

        if (ErrorDetectionStrategy.IsTransiant(e))
        {
          _retryCount++;
          var shouldRetry = Strategy.GetShouldRetry();

          TimeSpan delay;
          if (shouldRetry(_retryCount, e, out delay))
          {
            if (ct.IsCancellationRequested) return default(TResult);
            await Task.Delay(delay);
            return await ExecuteActionAsync(task, ct);
          }
        }

        throw;
      }
    }

  }

  public delegate bool ShouldRetry(int retryCount, Exception lastException, out TimeSpan delay);
}