using System;

namespace Cubic.Core.Numeric
{
  public struct Unit : IComparable<Unit>, IEquatable<Unit>
  {
    private readonly double _tolerance;

    public Unit(string type, double value, double tolerance = 0.0001)
    {
      Type = type;
      Value = value;
      _tolerance = tolerance;
    }

    public double Value { get; }

    public string Type { get; }

    public int CompareTo(Unit other)
    {
      if (this.Type.Equals(other.Type, StringComparison.InvariantCultureIgnoreCase)) throw new InvalidOperationException($"Can not Compare different Unit-Types: {Type} & {other.Type}");

      return Value.CompareTo(this.Value);
    }

    public bool Equals(Unit other)
    {
      if (this.Type.Equals(other.Type, StringComparison.InvariantCultureIgnoreCase)) return false;

      return Math.Abs(Value - other.Value) < _tolerance;
    }
  }
}