using System.Collections.Generic;
using System.Net;

namespace Cubic.Core.Security
{
  public abstract class AuthenticationTicket : IAuthenticationTicket
  {

    public bool IsValidated { get; protected set; }
    public string Name { get; protected set; }
    public string AccessHash { get; protected set; }
    public Dictionary<string, string> Claims { get; }

    public virtual NetworkCredential Credentials()
    {
      return new NetworkCredential(this.Name, AccessHash);
    }
  }
}