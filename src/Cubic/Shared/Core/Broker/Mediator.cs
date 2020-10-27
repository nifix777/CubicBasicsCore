using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cubic.Core.Broker
{
  public class Mediator : IMediator
  {
    private IServiceProvider _provider;

    private Dictionary<Type, HashSet<Type>> _requestHandlerTypes;
    private Dictionary<Tuple<Type>, HashSet<Type>> _requestHandlerTypesResponse;

    private Dictionary<Type, Type> _requestPreProcessorTypes;
    private Dictionary<Tuple<Type>, Tuple<Type, Type>> _requestPostProcessorTypes;

    private Dictionary<Type, Tuple<Type, Type>> _requestPipelineBehaviorTypes;

    public Mediator(IServiceProvider provider)
    {
      _provider = provider;

      _requestHandlerTypes = new Dictionary<Type, HashSet<Type>>();
      _requestHandlerTypesResponse = new Dictionary<Tuple<Type>, HashSet<Type>>();
    }


    public void Register<THandler, TRequest>() where THandler : IRequestHandler<TRequest> where TRequest : IRequest
    {
      if (!_requestHandlerTypes.ContainsKey(typeof(TRequest)))
      {
        _requestHandlerTypes[typeof(TRequest)] = new HashSet<Type>();
      }

      _requestHandlerTypes[typeof(TRequest)].Add(typeof(THandler));
    }

    public Task Send<TRequest>(TRequest request) where TRequest : IRequest
    {
      throw new System.NotImplementedException();
    }

    public Task<TResponse> Send<TRequest, TResponse>(TRequest request) where TRequest : IRequest
    {
      throw new System.NotImplementedException();
    }

    private Task BuildRequestPipeline<TRequest>(TRequest request)
    {
      throw new System.NotImplementedException();
    }
  }
}