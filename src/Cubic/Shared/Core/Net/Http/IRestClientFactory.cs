namespace Cubic.Core.Net.Http
{
  public interface IRestClientFactory
  {
    IRestClient GetClient();

    IRestClient GetClient(string name);
  }
}