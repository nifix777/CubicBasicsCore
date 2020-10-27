using System;

namespace Cubic.Core.Execution
{
  public class InputAttribute : Attribute
  {
    public InputAttribute()
    {
      Name = string.Empty;
    }

    public InputAttribute(string name)
    {
      Name = name;
    }

    public string Name { get; }
  }
}