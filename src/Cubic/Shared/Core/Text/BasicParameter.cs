using System;

namespace Cubic.Core.Text
{
  public struct BasicParameter : IEquatable<BasicParameter>
  {

    public BasicParameter(string name, string value) : this()
    {
      Name = name;
      Value = value;
    }

    public string Name { get; }

    public string Value { get; }

    public bool Equals(BasicParameter other)
    {
      return string.Equals(Name, other.Name) && string.Equals(Value, other.Value);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      return obj is BasicParameter && Equals((BasicParameter) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
      }
    }
  }
}