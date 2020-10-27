using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Threading
{
  internal static class InterlockedExtensions
  {
    public const byte NumberOfBits = 64;

    public static bool FlipBit(ref long target, byte bitOffset, bool bit)
    {
      long num;
      long num1;
      if (bitOffset >= 64)
      {
        CultureInfo invariantCulture = CultureInfo.InvariantCulture;
        string bitOffsetNotValid = "Invalid BitOffSet of {0}";
        object[] objArray = new object[] { bitOffset };
        throw new ArgumentException(string.Format(invariantCulture, bitOffsetNotValid, objArray));
      }
      long num2 = (long)1 << (bitOffset & 63);
      long num3 = Thread.VolatileRead(ref target);
      do
      {
        num1 = num3;
        if ((num1 & num2) == (long)0 ^ bit)
        {
          return false;
        }
        num = (!bit ? num1 & ~num2 : num1 | num2);
        num3 = Interlocked.CompareExchange(ref target, num, num1);
      }
      while (num1 != num3);
      return true;
    }
  }
}
