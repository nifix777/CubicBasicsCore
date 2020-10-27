using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Broker
{
  public delegate TResponse RequestHandlerDelegate<TResponse>();

  public interface IRequest
  {
    
  }

  public interface IRequestHandler<in TRequest> where TRequest : IRequest
  {
    Task Handle(TRequest request, CancellationToken cancellationToken);
  }



  public interface IRequestHandler<in TRequest, TResponse> where TRequest : IRequest
  {
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
  }


}