using System;
using System.Collections.Generic;

namespace Cubic.Core.Net.Http
{
  public class RestClientFactory : IRestClientFactory
  {
    private IDictionary<string, IRestClient> _clients = new Dictionary<string, IRestClient>();

    private Lazy<IRestClient> _sharedClient = new Lazy<IRestClient>(() => new RestClient()); 
    public IRestClient GetClient()
    {
      return _sharedClient.Value;
    }

    public IRestClient GetClient(string name)
    {
      return _clients[name];
    }

    public void Add(string name, Func<IRestClient> factory)
    {
      _clients[name] = factory();
    }
  }
}