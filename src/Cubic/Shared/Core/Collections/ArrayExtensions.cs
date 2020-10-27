using System;
using System.Linq;

namespace Cubic.Core.Collections
{

  internal static class EmptyArray<T>
  {
    public static readonly T[] Value = new T[0];
  }

  public static class ArrayExtensions
  {
    #region Array


    public static T[] Empty<T>()
    {
      return EmptyArray<T>.Value;
    }


    public static void ForEach(this Array array, Action<Array, int[]> action)
    {
      if (array.LongLength == 0) return;
      ArrayTraverse walker = new ArrayTraverse(array);
      do action(array, walker.Position);
      while (walker.Step());
    }

    public static void FastCopy<T>(this T[] source, T[] destination)
    {
      // for small arrays, lokal copy sould be the fastest

      if (source.Length < 100)
      {
        for (int i = 0; i < source.Length; i++)
        {
          destination[i] = source[i];
        }
      }
      else
      {
        System.Buffer.BlockCopy(source, 0, destination, 0, System.Buffer.ByteLength(source));
      }
    }

    /// <summary>
    /// Search for the nee
    /// </summary>
    /// <param name="source">the bytes as source</param>
    /// <param name="search">the searched bytes</param>
    /// <returns>Index of the found bytes, otherwise -1</returns>
    public static int SearchBytes(byte[] source, byte[] search)
    {

      var len = search.Length;

      var limit = source.Length - len;

      for (var i = 0; i <= limit; i++)
      {

        var k = 0;

        for (; k < len; k++)
        {

          if (search[k] != source[i + k]) break;

        }

        if (k == len) return i;

      }

      return -1;

    }

    public static byte[] RemoveNullBytes(this byte[] data)
    {
      return Enumerable.ToArray(data.TakeWhile((v, index) => data.Skip(index).Any(w => w != 0x00)));
    }

    public static ulong ToUInt64(this byte[] timestampBytes)
    {
      return BitConverter.ToUInt64(timestampBytes, 0);
    }

    public static ulong ToUInt64Reverse(this byte[] timestampBytes)
    {
      return BitConverter.ToUInt64(Enumerable.ToArray(timestampBytes.Reverse()), 0);
    }

    public static string ToUHexString(this byte[] timestampBytes)
    {
      return string.Join(String.Empty, Array.ConvertAll(timestampBytes, x => x.ToString("X2")));
    }

    public static string ToSqlTimestmapString(this byte[] timestampBytes)
    {
      return string.Format("0x{0:X}", timestampBytes.ToUInt64());
    }

    public static bool NeedsResize<T>(this T[] array, int currentIndex)
    {
      return (currentIndex == array.Length);
    }
    public static void EnsureResize<T>(ref T[] array, int currentIndex, int newsize = 64)
    {
      if (array.NeedsResize(currentIndex)) Array.Resize(ref array, newsize);
    }

    public static void Append<T>(ref T[] array, int currentIndex, int newsize = 0)
    {
      if (newsize == 0) newsize = array.Length + 1;
      if (array.NeedsResize(currentIndex)) Array.Resize(ref array, newsize);
    }

    public static void Append<T>(ref T[] array, int currentIndex, T value, int newsize = 0)
    {
      ArrayExtensions.Append(ref array, currentIndex, newsize);

      array[currentIndex] = value;
    }

    public static bool IsArrayEqual<T>(this T[] thisArray, T[] thatArray)
    {
      if (thisArray.Length != thatArray.Length)
      {
        return false;
      }

      for (int i = 0; i < thisArray.Length; i++)
      {
        if (!thisArray[i].Equals(thatArray[i]))
        {
          return false;
        }
      }

      return true;
    }


    #endregion

  }
}