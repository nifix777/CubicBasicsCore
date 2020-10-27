using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Broker
{
  public interface IRequestPreProcessor<in TRequest> where TRequest : IRequest
  {
    Task Process(TRequest request, CancellationToken cancellationToken);
  }
}