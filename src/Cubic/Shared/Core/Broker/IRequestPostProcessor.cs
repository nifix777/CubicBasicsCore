using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Broker
{
  public interface IRequestPostProcessor<in TRequest, in TResponse> where TRequest : IRequest
  {
    Task Process(TRequest request, TResponse response, CancellationToken cancellationToken);
  }
}