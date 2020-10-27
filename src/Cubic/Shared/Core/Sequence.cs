using System;

namespace Cubic.Core
{
  public struct Sequence : IComparable<Sequence>, IEquatable<Sequence>
  {
    public Guid Current { get; private set; }

    public static Sequence Create()
    {
      return new Sequence(Guid.NewGuid());
    }

    public static Sequence Create(Guid currentGuid)
    {
      return new Sequence(currentGuid);
    }

    public static Sequence Parse(string sequence)
    {
      return Create(Guid.Parse(sequence));
    }

    public static Sequence Parse(byte[] sequence)
    {
      return Create(new Guid(sequence));
    }

    public Sequence(Guid previous)
    {
      Current = previous;
    }

    public static Sequence operator ++(Sequence sequentialGuid)
    {
      byte[] bytes = sequentialGuid.Current.ToByteArray();
      for (int mapIndex = 0; mapIndex < 16; mapIndex++)
      {
        int bytesIndex = SqlOrderMap[mapIndex];
        bytes[bytesIndex]++;
        if (bytes[bytesIndex] != 0)
        {
          break; // No need to increment more significant bytes
        }
      }

      return Sequence.Parse(bytes);

      //sequentialGuid.Current = new Guid(bytes);
      //return sequentialGuid;
    }

    private static int[] _sqlOrderMap = null;
    private static int[] SqlOrderMap
    {
      get
      {
        if (_sqlOrderMap == null)
        {
          _sqlOrderMap = new int[16] { 3, 2, 1, 0, 5, 4, 7, 6, 9, 8, 15, 14, 13, 12, 11, 10 };
          // 3 - the least significant byte in Guid ByteArray [for SQL Server ORDER BY clause]
          // 10 - the most significant byte in Guid ByteArray [for SQL Server ORDERY BY clause]
        }
        return _sqlOrderMap;
      }
    }

    public byte[] ToByteArray()
    {
      return Current.ToByteArray();
    }

    public int CompareTo(Sequence other)
    {
      return this.Current.CompareTo(other.Current);
    }

    public bool Equals(Sequence other)
    {
      return this.Current.Equals(other.Current);
    }

    public override string ToString()
    {
      return Current.ToString();
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
      return Current.ToString(format, formatProvider);
    }
  }
}