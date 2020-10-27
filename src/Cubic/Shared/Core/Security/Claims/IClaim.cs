using System.Collections.Generic;

namespace Cubic.Core.Security.Claims
{
  public interface IClaim
  {
    string Name { get; }
    string Type { get; }

    string Value { get; }

    IDictionary<string, string> Properties { get; }
  }
}