using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Data
{
  public struct GeoPoint : IEquatable<GeoPoint>
  {
    public readonly decimal Lat;

    public readonly decimal Long;

    public override bool Equals(object obj)
    {
      return obj is GeoPoint && Equals((GeoPoint)obj);
    }

    public bool Equals(GeoPoint other)
    {
      return Lat == other.Lat &&
             Long == other.Long;
    }

    public override int GetHashCode()
    {
      var hashCode = -1450610079;
      hashCode = hashCode * -1521134295 + Lat.GetHashCode();
      hashCode = hashCode * -1521134295 + Long.GetHashCode();
      return hashCode;
    }

    public override string ToString()
    {
      return $"{Lat.ToInvariant()} {Long.ToInvariant()}";
    }
  }
}
