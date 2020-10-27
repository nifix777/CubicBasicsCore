using System;

namespace Cubic.Core.Execution
{
  public class RetryCountStrategy : IRetryStrategy
  {
    public static IRetryStrategy NoRetry = new RetryCountStrategy(1);

    private readonly int _retryCount;

    private readonly TimeSpan _delay;



    public RetryCountStrategy(int retryCount, TimeSpan delay = default(TimeSpan))
    {
      _retryCount = retryCount;
      _delay = delay;
    }

    public ShuoldRetry GetShuoldRetry()
    {
      return ShuoldRetryCore;
    }

    private bool ShuoldRetryCore(int retry, Exception exception, out TimeSpan delay)
    {
      delay = _delay;

      return (retry < _retryCount);
    }
  }

  public class AllErrorStrategy : ITransientErrorDetectionStrategy
  {
    public bool IsTransiant(Exception exception)
    {
      return true;
    }
  }

  public class CommonErrorStrategy : ITransientErrorDetectionStrategy
  {
    public bool IsTransiant(Exception exception)
    {
      return !(exception is BadImageFormatException || exception is ApplicationException ||
               exception is StackOverflowException || exception is DivideByZeroException || exception is ObjectDisposedException || exception is NullReferenceException);
    }
  }
}