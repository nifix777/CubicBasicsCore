using System.Collections.Generic;

namespace Cubic.Core.Security.Claims
{
  public interface IClaimsIdentity
  {
    string AuthenticationType { get; }
    
    IEnumerable<IClaim> Claims { get; }
    
    string Name { get; }
    
    bool IsAuthenticated { get; }

    bool HasCliam(IClaim cliam);

    void AddClaim(IClaim claim);
    void AddClaim(IEnumerable<IClaim> claims);

    bool RemoveClaim(IClaim claim);
  }
}