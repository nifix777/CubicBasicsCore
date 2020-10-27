using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cubic.Core.Security
{
  public static class AuthenticationTicketExtensions
  {
    public static IAuthenticationTicket CreateFromBase64(string value)
    {
      IAuthenticationTicket authenticationTicket;
      if (value == null)
      {
        return null;
      }
      using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(value)))
      {
        authenticationTicket = (new BinaryFormatter()).Deserialize(memoryStream) as IAuthenticationTicket;
      }
      return authenticationTicket;
    }

    public static string GetClaimOrDefault(this IAuthenticationTicket ticket, string key, string defaultValue)
    {
      string str;
      if (ticket.Claims.TryGetValue(key, out str))
      {
        return str;
      }
      return defaultValue;
    }

    public static string ToBase64(this IAuthenticationTicket ticket)
    {
      string base64String;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        (new BinaryFormatter()).Serialize(memoryStream, ticket);
        memoryStream.Flush();
        memoryStream.Seek((long)0, SeekOrigin.Begin);
        base64String = Convert.ToBase64String(memoryStream.GetBuffer());
      }
      return base64String;
    }
  }
}