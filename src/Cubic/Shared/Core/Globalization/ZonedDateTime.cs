using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Globalization
{
  [Serializable]
  public struct ZonedDateTime : ISerializable, IComparable<ZonedDateTime>, IEquatable<ZonedDateTime>, IEquatable<DateTimeOffset>
  {
    public string ZoneId;

    public DateTimeOffset DateTime;

    public ZonedDateTime(DateTimeOffset dateTimeOffset)
    {
      ZoneId = TimeZoneInfo.Local.Id;
      DateTime = dateTimeOffset;
    }

    public ZonedDateTime(string zoneId, DateTimeOffset dateTimeOffset)
    {
      ZoneId = zoneId;
      DateTime = dateTimeOffset;
    }

    public ZonedDateTime(SerializationInfo info, StreamingContext context)
    {
      ZoneId = info.GetString(nameof(ZoneId));
      DateTime = (DateTimeOffset)info.GetValue(nameof(DateTime), typeof(DateTimeOffset));
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue(nameof(ZoneId), ZoneId);
      info.AddValue(nameof(DateTime), DateTime);
    }

    public int CompareTo(ZonedDateTime other)
    {
      if(this.ZoneId != other.ZoneId)
      {
        throw new InvalidOperationException("Can not compare");
      }

      return DateTime.CompareTo(other.DateTime);
    }

    public bool Equals(ZonedDateTime other)
    {
      if (this.ZoneId != other.ZoneId)
      {
        return false;
      }

      return DateTime == other.DateTime;
    }

    public bool Equals(DateTimeOffset other)
    {
      var localZone = new ZonedDateTime(other);

      return this.Equals(other);
    }

    public override string ToString()
    {
      var zone = ZoneId;
      var dateTime = DateTime;
      return $"{dateTime} {TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(z => z.Id == zone)}";
    }
  }
}
