using System;
using System.Globalization;

namespace Cubic.Core.Data
{
  public static class Conversion
  {
    public static string DateTimeToISOString(DateTime value)
    {
      return value.ToString("s", CultureInfo.InvariantCulture);
    }

    public static SimpleDataType DataTypeSimpleFromSystem(Type value)
    {
      if (value == typeof(bool))
      {
        return SimpleDataType.Boolean;
      }
      if (value == typeof(short) || value == typeof(int))
      {
        return SimpleDataType.Integer;
      }
      if(value == typeof(long))
      {
        return SimpleDataType.Long;
      }
      if (value == typeof(decimal))
      {
        return SimpleDataType.Decimal;
      }
      if (value == typeof(DateTime))
      {
        return SimpleDataType.DateTime;
      }
      return SimpleDataType.String;
    }

    public static Type DataTypeSimpleToSystem(SimpleDataType dataType)
    {
      switch (dataType)
      {
        case SimpleDataType.None:
          {
            return typeof(object);
          }
        case SimpleDataType.Boolean:
          {
            return typeof(bool);
          }
        case SimpleDataType.Integer:
          {
            return typeof(int);
          }
        case SimpleDataType.Decimal:
          {
            return typeof(decimal);
          }
        case SimpleDataType.Date:
        case SimpleDataType.DateTime:
          {
            return typeof(DateTime);
          }
        case SimpleDataType.String:
          {
            return typeof(string);
          }
        case SimpleDataType.Time:
          {
            return typeof(string);
          }
        case SimpleDataType.Duration:
          {
            return typeof(int);
          }

        default:
          {
            return typeof(string);
          }
      }
    }
  }
}