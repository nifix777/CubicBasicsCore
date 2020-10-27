using System;
using System.Collections.Generic;
using Cubic.Core.Tools;

namespace Cubic.Core.Security
{
  [Serializable]
  public class TokenAuthenticationTicket : AuthenticationTicket
  {
    private const string TokenClaimName = "access_token";

    public IAuthenticationTicket BootstrapTicket
    {
      get;
      private set;
    }

    public string Token
    {
      get
      {
        return this.GetClaimOrDefault("access_token", null);
      }
    }

    public TokenAuthenticationTicket(IAuthenticationTicket bootstrapTicket, string token)
    {
      this.Initialize(bootstrapTicket, token, null);
    }

    public TokenAuthenticationTicket(IAuthenticationTicket bootstrapTicket, string token, string applicationUserName)
    {
      this.Initialize(bootstrapTicket, token, applicationUserName);
    }

    private void Initialize(IAuthenticationTicket bootstrapTicket, string token, string applicationUserName)
    {
      if (bootstrapTicket == null)
      {
        return;
      }
      base.Name = applicationUserName ?? bootstrapTicket.Name;
      this.BootstrapTicket = bootstrapTicket;
      base.AccessHash = HashGenerator.GenerateSHA1(base.Name);
      foreach (KeyValuePair<string, string> claim in bootstrapTicket.Claims)
      {
        base.Claims[claim.Key] = claim.Value;
      }
      if (!string.IsNullOrWhiteSpace(token))
      {
        base.Claims["access_token"] = token;
        base.IsValidated = true;
      }
    }
  }
}