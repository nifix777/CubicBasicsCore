using System.Threading.Tasks;

namespace Cubic.Core.Broker
{
  public interface IMediator
  {
    Task Send<TRequest>(TRequest request) where TRequest : IRequest;

    Task<TResponse> Send<TRequest, TResponse>(TRequest request) where TRequest : IRequest;
  }
}