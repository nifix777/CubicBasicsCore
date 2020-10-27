using System.Threading.Tasks;

namespace Cubic.Core.Threading
{
  public interface IAsyncResult<T>
  {
    Task<T> Execute();
  }
}