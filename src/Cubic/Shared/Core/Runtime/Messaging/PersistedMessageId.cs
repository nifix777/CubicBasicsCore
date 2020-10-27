﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Runtime.Messaging
{

  public class PersistedMessageId
  {
    public Guid SourceInstanceId { get; set; }
    public Guid MessageIdentifier { get; set; }

    public static PersistedMessageId GenerateRandom()
    {
      return new PersistedMessageId
      {
        SourceInstanceId = Guid.NewGuid(),
        MessageIdentifier = GenerateGuidComb()
      };
    }

    public static PersistedMessageId Parse(string id)
    {
      var parts = id.Split('/');
      var messageId = Guid.Parse(parts[0]);
      var instanceId = Guid.Parse(parts[1]);
      return new PersistedMessageId
      {
        MessageIdentifier = messageId,
        SourceInstanceId = instanceId
      };
    }

    public static Guid GenerateGuidComb()
    {
      var guidArray = Guid.NewGuid().ToByteArray();

      var baseDate = new DateTime(1900, 1, 1);
      var now = DateTime.Now;

      // Get the days and milliseconds which will be used to build the byte string
      var days = new TimeSpan(now.Ticks - baseDate.Ticks);
      var msecs = now.TimeOfDay;

      // Convert to a byte array
      // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333
      var daysArray = BitConverter.GetBytes(days.Days);
      var msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

      // Reverse the bytes to match SQL Servers ordering
      Array.Reverse(daysArray);
      Array.Reverse(msecsArray);

      // Copy the bytes into the guid
      Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
      Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

      return new Guid(guidArray);
    }

    public bool Equals(PersistedMessageId other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return other.SourceInstanceId.Equals(SourceInstanceId) && other.MessageIdentifier.Equals(MessageIdentifier);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof(PersistedMessageId)) return false;
      return Equals((PersistedMessageId)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (SourceInstanceId.GetHashCode() * 397) ^ MessageIdentifier.GetHashCode();
      }
    }

    public override string ToString()
    {
      return $"{MessageIdentifier}/{SourceInstanceId}";
    }
  }
}
