using System;

namespace Cubic.Core.Execution
{
  public class OutputAttribute : Attribute
  {
    public OutputAttribute()
    {
      Name = string.Empty;
    }

    public OutputAttribute(string name)
    {
      Name = name;
    }
    public string Name { get; }
  }
}