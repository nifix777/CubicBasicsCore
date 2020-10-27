using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Broker
{
  public interface IPipelineBehaviorr<in TRequest, TResponse> where TRequest : IRequest
  {
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next);
  }
}