namespace Cubic.Core.Execution
{
  public interface IRetryStrategy
  {
    ShuoldRetry GetShuoldRetry();
  }
}