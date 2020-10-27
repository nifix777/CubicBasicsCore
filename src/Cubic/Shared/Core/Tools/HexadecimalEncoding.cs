using System;
using System.Globalization;
using System.Text;

namespace Cubic.Core.Tools
{
  public static class HexadecimalEncoding
  {
    private static readonly char[] _hexadecimalChars;

    private static readonly byte _offset_0;

    private static readonly byte _offset_A;

    private static readonly byte _offset_A16;

    private static readonly byte _offset_a;

    private static readonly byte _offset_a16;

    static HexadecimalEncoding()
    {
      HexadecimalEncoding._hexadecimalChars = HexadecimalEncoding.InitHexadecimalChars();
      HexadecimalEncoding._offset_0 = 48;
      HexadecimalEncoding._offset_A = 65;
      HexadecimalEncoding._offset_A16 = (byte)(HexadecimalEncoding._offset_A - 10);
      HexadecimalEncoding._offset_a = 97;
      HexadecimalEncoding._offset_a16 = (byte)(HexadecimalEncoding._offset_a - 10);
    }

    public static byte[] CreateByteArray(string hexadecimalValue)
    {
      return HexadecimalEncoding.CreateByteArray(hexadecimalValue, false);
    }

    public static byte[] CreateByteArray(string hexadecimalValue, bool reverse)
    {
      int length = hexadecimalValue.Length;
      byte[] numArray = new byte[length / 2];
      int num = 0;
      if (reverse)
      {
        num = length / 2 - 1;
      }
      for (int i = 0; i < length; i += 2)
      {
        byte _offsetA16 = (byte)hexadecimalValue[i];
        if (_offsetA16 < HexadecimalEncoding._offset_a)
        {
          _offsetA16 = (_offsetA16 < HexadecimalEncoding._offset_A ? (byte)(_offsetA16 - HexadecimalEncoding._offset_0) : (byte)(_offsetA16 - HexadecimalEncoding._offset_A16));
        }
        else
        {
          _offsetA16 = (byte)(_offsetA16 - HexadecimalEncoding._offset_a16);
        }
        byte _offsetA161 = (byte)hexadecimalValue[i + 1];
        if (_offsetA161 < HexadecimalEncoding._offset_a)
        {
          _offsetA161 = (_offsetA161 < HexadecimalEncoding._offset_A ? (byte)(_offsetA161 - HexadecimalEncoding._offset_0) : (byte)(_offsetA161 - HexadecimalEncoding._offset_A16));
        }
        else
        {
          _offsetA161 = (byte)(_offsetA161 - HexadecimalEncoding._offset_a16);
        }
        if (!reverse)
        {
          int num1 = num;
          num = num1 + 1;
          numArray[num1] = (byte)(16 * _offsetA16 + _offsetA161);
        }
        else
        {
          int num2 = num;
          num = num2 - 1;
          numArray[num2] = (byte)(16 * _offsetA16 + _offsetA161);
        }
      }
      return numArray;
    }

    public static byte[] CreateByteArray(byte[] initialArray, int requiredSize)
    {
      byte[] numArray;
      int length = (int)initialArray.Length;
      if (length != requiredSize)
      {
        numArray = new byte[requiredSize];
        int num = (int)numArray.Length;
        if (num > length)
        {
          num = length;
        }
        Array.Copy(initialArray, numArray, num);
      }
      else
      {
        numArray = initialArray;
      }
      return numArray;
    }

    private static char[] InitHexadecimalChars()
    {
      char[] charArray = "0123456789ABCDEF".ToCharArray();
      Array.Sort<char>(charArray);
      return charArray;
    }

    public static bool IsHexadecimalChar(char symbol)
    {
      symbol = char.ToUpper(symbol, CultureInfo.InvariantCulture);
      return Array.BinarySearch<char>(HexadecimalEncoding._hexadecimalChars, symbol) >= 0;
    }

    public static bool IsHexadecimalString(string hexadecimalValue)
    {
      bool flag = HexadecimalEncoding.IsValidHexadecimalCharLength(hexadecimalValue);
      if (flag)
      {
        string str = hexadecimalValue;
        int num = 0;
        while (num < str.Length)
        {
          if (HexadecimalEncoding.IsHexadecimalChar(str[num]))
          {
            num++;
          }
          else
          {
            flag = false;
            break;
          }
        }
      }
      return flag;
    }

    private static bool IsValidHexadecimalCharLength(string hexadecimalValue)
    {
      if (hexadecimalValue == null)
      {
        return false;
      }
      return hexadecimalValue.Length >= 2;
    }

    public static string ToString(byte[] value)
    {
      if (value == null || (int)value.Length == 0)
      {
        throw new ArgumentNullException("value");
      }
      StringBuilder stringBuilder = new StringBuilder();
      byte[] numArray = value;
      for (int i = 0; i < (int)numArray.Length; i++)
      {
        byte num = numArray[i];
        stringBuilder.Append(num.ToString("X2", CultureInfo.InvariantCulture));
      }
      return stringBuilder.ToString();
    }
  }
}