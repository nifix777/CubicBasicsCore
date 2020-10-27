using System;
using System.Collections.Generic;
using System.Linq;
using Cubic.Core.Collections;

namespace Cubic.Core.Security.Claims
{
  [Serializable]
  public class ClaimsIdentity : IClaimsIdentity
  {
    private List<IClaim> _claims = new List<IClaim>();

    public string AuthenticationType { get; set; }

    public IEnumerable<IClaim> Claims => _claims;

    public string Name { get; set; }

    public bool IsAuthenticated { get; }

    public bool HasCliam(IClaim cliam)
    {
      return _claims.Any(c => c.Name == cliam.Name && c.Type == cliam.Type);
    }

    public void AddClaim(IClaim claim)
    {
      _claims.Add(claim);
    }

    public void AddClaim(IEnumerable<IClaim> claims)
    {
      _claims.AddRange(claims);
    }

    public bool RemoveClaim(IClaim claim)
    {
      return _claims.Remove(claim);
    }
  }
}