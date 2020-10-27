using System;
using System.Globalization;

namespace Cubic.Core.Numeric
{
  public static class NumericExtensiosn
  {


    public static string ToOrdinal(this int num)
    {
      switch (num % 100)
      {
        case 11:
        case 12:
        case 13:
          return num.ToString("#,###0") + "th";
      }

      switch (num % 10)
      {
        case 1:
          return num.ToString("#,###0") + "st";
        case 2:
          return num.ToString("#,###0") + "nd";
        case 3:
          return num.ToString("#,###0") + "rd";
        default:
          return num.ToString("#,###0") + "th";
      }
    }

    public static string ToFileSizeDisplay(this int i)
    {
      return ToFileSizeDisplay((long)i, 2);
    }

    public static string ToFileSizeDisplay(this int i, int decimals)
    {
      return ToFileSizeDisplay((long)i, decimals);
    }

    public static string ToFileSizeDisplay(this long i)
    {
      return ToFileSizeDisplay(i, 2);
    }

    public static string ToFileSizeDisplay(this long i, int decimals)
    {
      if (i < 1024 * 1024 * 1024) // 1 GB
      {
        string value = Math.Round((decimal)i / 1024m / 1024m, decimals).ToString("N" + decimals);
        if (decimals > 0 && value.EndsWith(new string('0', decimals)))
          value = value.Substring(0, value.Length - decimals - 1);

        return string.Concat(value, " MB");
      }
      else
      {
        string value = Math.Round((decimal)i / 1024m / 1024m / 1024m, decimals).ToString("N" + decimals);
        if (decimals > 0 && value.EndsWith(new string('0', decimals)))
          value = value.Substring(0, value.Length - decimals - 1);

        return string.Concat(value, " GB");
      }
    }

    public static decimal ToDecimalFromGerman(this string value)
    {
      decimal result;

      if (decimal.TryParse(value, NumberStyles.Any, System.Globalization.CultureInfo.GetCultureInfo("de-DE"), out result))
      {
        return result;
      }

      return 0;
    }

    public static string ToStringGerman(this decimal value,
                                     out Exception returnErr)
    {
      returnErr = null;

      try
      {
        value = value.RoundDIN1333(2);

        return value.ToString("#,#.00#;(#,#.00#)", System.Globalization.CultureInfo.GetCultureInfo("de-DE"));
      }
      catch (Exception ex)
      {
        returnErr = ex;
      }

      return string.Empty;
    }

    #region Round
    /// <summary>
    /// System AG: <b>Rounds</b> the specified <see cref="System.Decimal"/> value to the given unit.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="decimals">The decimals.</param>
    /// <returns>Rounded value.</returns>
    public static decimal Round(decimal value,
                                 short decimals)
    {
      return RoundToUnit(value, 1M / Convert.ToDecimal(Math.Pow(10.0, decimals)));
    }

    /// <summary>
    /// System AG: <b>Rounds</b> the specified <see cref="System.Double"/> value to the given unit.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="decimals">The decimals.</param>
    /// <returns>Rounded value.</returns>
    public static double Round(double value,
                                short decimals)
    {
      return RoundToUnit(value, 1.0 / Math.Pow(10.0, decimals));
    }

    public static decimal RoundDIN1333(this decimal value,
                                    short decimals = 2)
    {
      return Round(value, decimals, RoundingType.DIN1333);
    }

    public static decimal RoundWithType(this decimal value, short decimals, RoundingType rounding)
    {
      return Round(value, decimals, rounding);
    }

    public static decimal Round(decimal value,
                             short decimals,
                             RoundingType roundingType)
    {
      #region "Keep compatible with ERP vendor behaviour"

      if (roundingType == RoundingType.DIN1333 && decimals < 2)
      {
        decimal unit = 1;

        if (decimals == 1)
        {
          unit = (decimal)0.1;
        }

        return RoundToUnit(value, unit);
      }

      if ((roundingType == RoundingType.DIN1333 || roundingType == RoundingType.DIN1333OnlyTwoOrMoreDecimals)
           && decimals > 4)
      {
        throw new ArgumentException("201603256: Rounding types according to DIN1333 are only suitable for a maximum of four digits! Consider using method 'RoundToUnit' instead for more digits!", "roundingType");
      }

      if (roundingType == RoundingType.DIN1333OnlyTwoOrMoreDecimals && decimals < 2)
      {
        throw new ArgumentException("201603255: Rounding type 'DIN1333OnlyTwoOrMoreDecimals' explicitly requires two or more decimals for rounding parameter!", "roundingType");
      }

      if ((roundingType == RoundingType.DIN1333 || roundingType == RoundingType.DIN1333OnlyTwoOrMoreDecimals)
           && decimals == 2)
      {
        return RoundToUnit(value, 0.01.ToDecimal());
      }

      if ((roundingType == RoundingType.DIN1333 || roundingType == RoundingType.DIN1333OnlyTwoOrMoreDecimals)
           && decimals == 3)
      {
        return RoundToUnit(value, 0.001.ToDecimal());
      }

      if ((roundingType == RoundingType.DIN1333 || roundingType == RoundingType.DIN1333OnlyTwoOrMoreDecimals)
           && decimals == 4)
      {
        return RoundToUnit(value, 0.0001.ToDecimal());
      }

      #endregion

      #region "Classic approach"

      decimal num = Round(value, decimals);

      switch (roundingType)
      {
        case RoundingType.DIN1333:
        case RoundingType.DIN1333OnlyTwoOrMoreDecimals:
          if (num < value)
          {
            num += Convert.ToDecimal((1.0 / Math.Pow(10.0, decimals)));
          }
          return num;

        case RoundingType.Cut:
          if (num > value)
          {
            num -= Convert.ToDecimal((1.0 / Math.Pow(10.0, decimals)));
          }
          return num;
      }

      return num;

      #endregion
    }

    public static decimal RoundToUnit(decimal value,
                                   decimal unit)
    {
      if (unit == 0)
      {
        unit = 1;
      }

      return ((Math.Sign(value) * Fix((Math.Abs(value) / unit) + 0.5M)) * unit);
    }

    public static double RoundToUnit(double value,
                                  double unit)
    {
      return ((Math.Sign(value) * Math.Floor(((Math.Abs(value) / unit) + 0.5000001))) * unit);
    } 
    #endregion

    public static decimal Fix(decimal value)
    {
      return (Math.Sign(value) * Math.Floor(Math.Abs(value)));
    }


    public static decimal Max(decimal c1,
                           decimal c2)
    {
      return Math.Max(c1, c2);
    }

    public static short Max(short n1,
                         short n2)
    {
      return Math.Max(n1, n2);
    }

    public static int Max(int l1,
                       int l2)
    {
      return Math.Max(l1, l2);
    }
    public static int Min(int l1,
                       int l2)
    {
      return Math.Min(l1, l2);
    }

    public static short Min(short n1,
                         short n2)
    {
      return Math.Min(n1, n2);
    }
    public static decimal Min(decimal c1,
                           decimal c2)
    {
      return Math.Min(c1, c2);
    }

    #region ToPositive
    public static int ToPositive(this int value)
    {
      return Math.Abs(value);
    }

    public static short ToPositive(this short value)
    {
      return Math.Abs(value);
    }

    public static decimal ToPositive(this decimal value)
    {
      return Math.Abs(value);
    }

    public static float ToPositive(this float value)
    {
      return Math.Abs(value);
    }

    public static long ToPositive(this long value)
    {
      return Math.Abs(value);
    } 
    #endregion
  }

  public enum RoundingType
  {
    Cut,
    DIN1333OnlyTwoOrMoreDecimals,
    DIN1333
  }
}