using System.Collections.Generic;
using System.Net;

namespace Cubic.Core.Security
{
  public interface IAuthenticationTicket
  {
    bool IsValidated { get; }
    string Name { get; }
    string AccessHash { get; }
    Dictionary<string, string> Claims { get; }

    NetworkCredential Credentials();
  }
}