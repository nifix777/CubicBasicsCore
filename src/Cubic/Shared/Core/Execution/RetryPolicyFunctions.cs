using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cubic.Core.Execution
{
  public static class RetryPolicyFunctions
  {
    internal static Task<TResult> Retry<TResult>(Func<Task<TResult>> task, RetryPolicy policy)
    {
      return policy.ExecuteActionAsync(task);
    }

    internal static Task Retry(Func<Task> task, RetryPolicy policy)
    {
      return policy.ExecuteActionAsync(task);
    }

    public static Task<TResult> Retry<TResult>(Func<Task<TResult>> task, IRetryStrategy strategy, ITransientErrorDetectionStrategy errorDetectionStrategy)
    {
      return Retry(task, new RetryPolicy(strategy, errorDetectionStrategy));
    }

    public static Task Retry(Func<Task> task, IRetryStrategy strategy, ITransientErrorDetectionStrategy errorDetectionStrategy)
    {
      return Retry(task, new RetryPolicy(strategy, errorDetectionStrategy));
    }

    public static TResult Retry<TResult>(Func<TResult> func, IRetryStrategy strategy, ITransientErrorDetectionStrategy errorDetectionStrategy)
    {
      return Retry(func, new RetryPolicy(strategy, errorDetectionStrategy));
    }

    public static Task<TResult> Retry<TResult>(this Func<Task<TResult>> task, int retrys, TimeSpan delay = default (TimeSpan))
    {
      return Retry(task, new RetryPolicy(new RetryCountStrategy(retrys, delay), new CommonErrorStrategy()));
    }

    public static Task Retry(this Func<Task> task, int retrys, TimeSpan delay = default(TimeSpan))
    {
      return Retry(task, new RetryPolicy(new RetryCountStrategy(retrys, delay), new CommonErrorStrategy()));
    }

    internal static TResult Retry<TResult>(Func<TResult> function, RetryPolicy policy)
    {
      return policy.ExecuteAction(function);
    }

    public static TResult Retry<TResult>(this Func<TResult> function, int retrys, TimeSpan delay = default(TimeSpan))
    {
      return Retry(function, new RetryPolicy(new RetryCountStrategy(retrys, delay), new CommonErrorStrategy()));
    }

  }
}