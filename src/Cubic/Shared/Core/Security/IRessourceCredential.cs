using System.Net;

namespace Cubic.Core.Security
{
  public interface IRessourceCredential
  {

    NetworkCredential Credential { get; }
    string Ressource { get; }
  }
}