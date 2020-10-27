using System;

namespace Cubic.Core
{
  public static class GuidExtensions
  {
    /// <summary>
    /// Creates a sequential GUID according to SQL Server's ordering rules.
    /// </summary>
    public static Guid NewSequentialId()
    {
      var guidBytes = Guid.NewGuid().ToByteArray();

      byte[] sequential = BitConverter.GetBytes((DateTime.Now.Ticks / 10000L) - DateTimeFunctions.EpochMilliseconds);

      if (BitConverter.IsLittleEndian)
      {
        guidBytes[10] = sequential[5];
        guidBytes[11] = sequential[4];
        guidBytes[12] = sequential[3];
        guidBytes[13] = sequential[2];
        guidBytes[14] = sequential[1];
        guidBytes[15] = sequential[0];
      }
      else
      {
        Buffer.BlockCopy(sequential, 2, guidBytes, 10, 6);
      }

      return new Guid(guidBytes);
    }
  }
}