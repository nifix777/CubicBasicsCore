namespace Cubic.Core.Execution
{
  public interface IRetryStrategy
  {
    ShouldRetry GetShouldRetry();
  }
}