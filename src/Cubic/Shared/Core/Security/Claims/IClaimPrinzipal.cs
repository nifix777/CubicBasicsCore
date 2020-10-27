using System;
using System.Collections.Generic;

namespace Cubic.Core.Security.Claims
{
  public interface IClaimPrinzipal
  {
    IList<IClaimsIdentity> Identities { get; }
    
    IClaimsIdentity Identity { get; }
    
    Func<IClaimsIdentity, bool> PrimaryIdentitySelector { get; set; }
    
    IEnumerable<IClaim> Claims { get; }   
  }
}