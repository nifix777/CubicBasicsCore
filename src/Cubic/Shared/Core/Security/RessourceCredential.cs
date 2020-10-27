using System.Net;

namespace Cubic.Core.Security
{
  public class RessourceCredential : IRessourceCredential
  {
    public NetworkCredential Credential { get; }
    public string Ressource { get; }
  }
}