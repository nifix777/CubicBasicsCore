using System;
using System.Collections.Generic;
using System.Linq;

namespace Cubic.Core.Security.Claims
{
  [Serializable]
  public class ClaimPrinzipal : IClaimPrinzipal
  {
    private IList<IClaimsIdentity> _identities = new List<IClaimsIdentity>();
    public IList<IClaimsIdentity> Identities => _identities;

    public IClaimsIdentity Identity
    {
      get
      {
        if (PrimaryIdentitySelector == null) return null;

        return Identities.FirstOrDefault(PrimaryIdentitySelector);
      }
    }

    public Func<IClaimsIdentity, bool> PrimaryIdentitySelector { get; set; }
    public IEnumerable<IClaim> Claims => _identities.SelectMany(i => i.Claims);
  }
}