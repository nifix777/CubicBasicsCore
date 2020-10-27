using System;
using System.Collections.Generic;

namespace Cubic.Core.Security.Claims
{
  [Serializable]
  public class Claim : IClaim , IEquatable<IClaim>
  {
    public string Name { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }
    public IDictionary<string, string> Properties { get; } = new Dictionary<string, string>();

    public bool Equals(IClaim other)
    {
      return other != null && (string.Equals(Name, other.Name) && string.Equals(Type, other.Type) && string.Equals(Value, other.Value));
    }
  }
}