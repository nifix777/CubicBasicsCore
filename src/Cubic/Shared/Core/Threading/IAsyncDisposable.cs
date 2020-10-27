using System.Threading.Tasks;

namespace Cubic.Core.Threading
{
  /// <summary>
  /// Defines an asynchronous method to release allocated resources.
  /// </summary>
  public interface IAsyncDisposable
  {
    /// <summary>
    /// Performs application-defined tasks associated with freeing,
    /// releasing, or resetting unmanaged resources asynchronously.
    /// </summary>
    Task DisposeAsync();
  }
}