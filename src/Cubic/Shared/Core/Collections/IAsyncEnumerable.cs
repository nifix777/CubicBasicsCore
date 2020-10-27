namespace Cubic.Core.Collections
{
  public interface IAsyncEnumerable<T>
  {
    IAsyncEnumerator<T> GetAsyncEnumerator();
  }

  public interface IAsyncEnumerable2<T>
  {
    IAsyncEnumerator<T> GetAsyncEnumerator();
  }
}