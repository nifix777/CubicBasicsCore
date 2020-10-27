using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Cubic.Core.Numeric
{
  [Serializable]
  public struct FixDigitsNumber : IFormattable, IComparable, IConvertible, IComparable<FixDigitsNumber>, IEquatable<FixDigitsNumber>
  {
    public const int Maxdigits = 4;

    [DecimalConstant(4, 0, 0, 0, 1)]
    public static readonly decimal DigitUnit;

    private decimal _value;

    static FixDigitsNumber()
    {
      FixDigitsNumber.DigitUnit = new decimal(1, 0, 0, false, 4);
    }

    private FixDigitsNumber(decimal value, bool useBankersRounding)
    {
      if (!useBankersRounding)
      {
        this._value = FixDigitsNumber.Round(value);
        return;
      }
      this._value = FixDigitsNumber.RoundToEven(value, 4);
    }

    public FixDigitsNumber(decimal value) : this(value, false)
    {
    }

    public FixDigitsNumber(double value) : this(new decimal(value), false)
    {
    }

    public FixDigitsNumber(float value) : this(new decimal(value), false)
    {
    }

    public FixDigitsNumber(int value) : this(new decimal(value), false)
    {
    }

    public FixDigitsNumber Abs()
    {
      return FixDigitsNumber.Abs(this);
    }

    public static FixDigitsNumber Abs(FixDigitsNumber f)
    {
      return Math.Abs(f._value);
    }

    public static FixDigitsNumber Add(FixDigitsNumber f1, FixDigitsNumber f2)
    {
      return f1._value + f2._value;
    }

    public FixDigitsNumber Ceiling()
    {
      return FixDigitsNumber.Ceiling(this);
    }

    public static FixDigitsNumber Ceiling(FixDigitsNumber f)
    {
      return decimal.Ceiling(f._value);
    }

    public static int Compare(FixDigitsNumber f1, FixDigitsNumber f2)
    {
      return decimal.Compare(f1._value, f2._value);
    }

    public int CompareTo(object obj)
    {
      if (obj == null)
      {
        return 1;
      }
      if (!(obj is FixDigitsNumber))
      {
        throw new ArgumentException("Object is not a FixDigitsNumber", nameof(obj));
      }
      return FixDigitsNumber.Compare(this, (FixDigitsNumber)obj);
    }

    public int CompareTo(FixDigitsNumber other)
    {
      return FixDigitsNumber.Compare(this, other);
    }

    public static FixDigitsNumber Divide(FixDigitsNumber f1, FixDigitsNumber f2)
    {
      return new FixDigitsNumber(f1._value / f2._value, true);
    }

    public bool Equals(FixDigitsNumber other)
    {
      return FixDigitsNumber.Compare(this, other) == 0;
    }

    public override bool Equals(object obj)
    {
      if (!(obj is FixDigitsNumber))
      {
        return false;
      }
      return FixDigitsNumber.Compare(this, (FixDigitsNumber)obj) == 0;
    }

    private static decimal Fix(decimal value)
    {
      return Math.Sign(value) * Math.Floor(Math.Abs(value));
    }

    public FixDigitsNumber Fix()
    {
      return FixDigitsNumber.Fix(this);
    }

    public static FixDigitsNumber Fix(FixDigitsNumber f)
    {
      return FixDigitsNumber.Fix(f._value);
    }

    public FixDigitsNumber Floor()
    {
      return FixDigitsNumber.Floor(this);
    }

    public static FixDigitsNumber Floor(FixDigitsNumber f)
    {
      return decimal.Floor(f._value);
    }

    public override int GetHashCode()
    {
      return this._value.GetHashCode();
    }

    public static FixDigitsNumber Max(FixDigitsNumber f1, FixDigitsNumber f2)
    {
      return Math.Max(f1._value, f2._value);
    }

    public static FixDigitsNumber Min(FixDigitsNumber f1, FixDigitsNumber f2)
    {
      return Math.Min(f1._value, f2._value);
    }

    public static FixDigitsNumber Multiply(FixDigitsNumber f1, FixDigitsNumber f2)
    {
      return new FixDigitsNumber(f1._value * f2._value, true);
    }

    public static FixDigitsNumber Negate(FixDigitsNumber f)
    {
      return decimal.Negate(f._value);
    }

    public static FixDigitsNumber operator +(FixDigitsNumber f1, FixDigitsNumber f2)
    {
      return FixDigitsNumber.Add(f1, f2);
    }

    public static FixDigitsNumber operator /(FixDigitsNumber f1, FixDigitsNumber f2)
    {
      return FixDigitsNumber.Divide(f1, f2);
    }

    public static bool operator ==(FixDigitsNumber f1, FixDigitsNumber f2)
    {
      return f1._value == f2._value;
    }

    public static explicit operator Decimal(FixDigitsNumber f)
    {
      return f._value;
    }

    public static explicit operator Double(FixDigitsNumber f)
    {
      return (double)((double)f._value);
    }

    public static explicit operator Single(FixDigitsNumber f)
    {
      return (float)((float)f._value);
    }

    public static explicit operator Int64(FixDigitsNumber f)
    {
      return FixDigitsNumber.ToInt64(f);
    }

    public static explicit operator Int32(FixDigitsNumber f)
    {
      return FixDigitsNumber.ToInt32(f);
    }

    public static bool operator >(FixDigitsNumber f1, FixDigitsNumber f2)
    {
      return f1._value > f2._value;
    }

    public static bool operator >=(FixDigitsNumber f1, FixDigitsNumber f2)
    {
      return f1._value >= f2._value;
    }

    public static implicit operator FixDigitsNumber(decimal value)
    {
      return new FixDigitsNumber(value);
    }

    public static implicit operator FixDigitsNumber(double value)
    {
      return new FixDigitsNumber(value);
    }

    public static implicit operator FixDigitsNumber(float value)
    {
      return new FixDigitsNumber(value);
    }

    public static implicit operator FixDigitsNumber(long value)
    {
      return new FixDigitsNumber(value.ToDecimal());
    }

    public static implicit operator FixDigitsNumber(int value)
    {
      return new FixDigitsNumber(value);
    }

    public static bool operator !=(FixDigitsNumber f1, FixDigitsNumber f2)
    {
      return f1._value != f2._value;
    }

    public static bool operator <(FixDigitsNumber f1, FixDigitsNumber f2)
    {
      return f1._value < f2._value;
    }

    public static bool operator <=(FixDigitsNumber f1, FixDigitsNumber f2)
    {
      return f1._value <= f2._value;
    }

    public static FixDigitsNumber operator *(FixDigitsNumber f1, FixDigitsNumber f2)
    {
      return FixDigitsNumber.Multiply(f1, f2);
    }

    public static FixDigitsNumber operator -(FixDigitsNumber f1, FixDigitsNumber f2)
    {
      return FixDigitsNumber.Subtract(f1, f2);
    }

    public static FixDigitsNumber operator -(FixDigitsNumber f)
    {
      return FixDigitsNumber.Negate(f);
    }

    public static FixDigitsNumber operator +(FixDigitsNumber f)
    {
      return FixDigitsNumber.Plus(f);
    }

    public static FixDigitsNumber Parse(string s)
    {
      return FixDigitsNumber.Parse(s, NumberStyles.Number, null);
    }

    public static FixDigitsNumber Parse(string s, NumberStyles style)
    {
      return FixDigitsNumber.Parse(s, style, null);
    }

    public static FixDigitsNumber Parse(string s, NumberStyles style, IFormatProvider provider)
    {
      return decimal.Parse(s, style, provider);
    }

    public static FixDigitsNumber Parse(string s, IFormatProvider provider)
    {
      return FixDigitsNumber.Parse(s, NumberStyles.Number, provider);
    }

    public FixDigitsNumber Percentage(FixDigitsNumber percent)
    {
      return new FixDigitsNumber(this._value * (percent._value / new decimal(100)), true);
    }

    public static FixDigitsNumber Plus(FixDigitsNumber f)
    {
      return f;
    }

    public FixDigitsNumber Pow(FixDigitsNumber power)
    {
      return new FixDigitsNumber((decimal)((double)Math.Pow((double)((double)this._value), (double)((double)power))), true);
    }

    public static FixDigitsNumber Remainder(FixDigitsNumber f1, FixDigitsNumber f2)
    {
      return new FixDigitsNumber(decimal.Remainder(f1._value, f2._value), true);
    }

    private static decimal Round(decimal value)
    {
      return FixDigitsNumber.RoundToUnit(value, new decimal(1, 0, 0, false, 4));
    }

    private static decimal RoundToDigits(decimal value, int digits)
    {
      return FixDigitsNumber.RoundToUnit(value, new decimal(1) / Convert.ToDecimal(Math.Pow(10, (double)digits)));
    }

    public FixDigitsNumber RoundToDigits(int digits)
    {
      return FixDigitsNumber.RoundToDigits(this, digits);
    }

    public static FixDigitsNumber RoundToDigits(FixDigitsNumber f, int digits)
    {
      return FixDigitsNumber.RoundToDigits(f._value, digits);
    }

    private static decimal RoundToEven(decimal value, int digits)
    {
      return decimal.Round(value, digits, MidpointRounding.ToEven);
    }

    private static decimal RoundToUnit(decimal value, decimal unit)
    {
      return (Math.Sign(value) * FixDigitsNumber.Fix((Math.Abs(value) / unit) + new decimal(5, 0, 0, false, 1))) * unit;
    }

    public FixDigitsNumber RoundToUnit(FixDigitsNumber unit)
    {
      return FixDigitsNumber.RoundToUnit(this, unit);
    }

    public static FixDigitsNumber RoundToUnit(FixDigitsNumber f, FixDigitsNumber unit)
    {
      return FixDigitsNumber.RoundToUnit(f._value, unit._value);
    }

    public static FixDigitsNumber Subtract(FixDigitsNumber f1, FixDigitsNumber f2)
    {
      return f1._value - f2._value;
    }

    TypeCode System.IConvertible.GetTypeCode()
    {
      return TypeCode.Object;
    }

    bool System.IConvertible.ToBoolean(IFormatProvider provider)
    {
      throw new InvalidCastException();
    }

    byte System.IConvertible.ToByte(IFormatProvider provider)
    {
      return FixDigitsNumber.ToByte(this._value);
    }

    char System.IConvertible.ToChar(IFormatProvider provider)
    {
      throw new InvalidCastException();
    }

    DateTime System.IConvertible.ToDateTime(IFormatProvider provider)
    {
      throw new InvalidCastException();
    }

    decimal System.IConvertible.ToDecimal(IFormatProvider provider)
    {
      return this._value;
    }

    double System.IConvertible.ToDouble(IFormatProvider provider)
    {
      return Convert.ToDouble(this._value);
    }

    short System.IConvertible.ToInt16(IFormatProvider provider)
    {
      return FixDigitsNumber.ToInt16(this._value);
    }

    int System.IConvertible.ToInt32(IFormatProvider provider)
    {
      return FixDigitsNumber.ToInt32(this._value);
    }

    long System.IConvertible.ToInt64(IFormatProvider provider)
    {
      return FixDigitsNumber.ToInt64(this._value);
    }

    sbyte System.IConvertible.ToSByte(IFormatProvider provider)
    {
      return Convert.ToSByte(FixDigitsNumber.RoundToEven(this._value, 0));
    }

    float System.IConvertible.ToSingle(IFormatProvider provider)
    {
      return Convert.ToSingle(this._value);
    }

    object System.IConvertible.ToType(Type conversionType, IFormatProvider provider)
    {
      return ((IConvertible)(object)this._value).ToType(conversionType, provider);
    }

    ushort System.IConvertible.ToUInt16(IFormatProvider provider)
    {
      return Convert.ToUInt16(FixDigitsNumber.RoundToEven(this._value, 0));
    }

    uint System.IConvertible.ToUInt32(IFormatProvider provider)
    {
      return Convert.ToUInt32(FixDigitsNumber.RoundToEven(this._value, 0));
    }

    ulong System.IConvertible.ToUInt64(IFormatProvider provider)
    {
      return Convert.ToUInt64(FixDigitsNumber.RoundToEven(this._value, 0));
    }

    public static byte ToByte(FixDigitsNumber f)
    {
      return decimal.ToByte(FixDigitsNumber.RoundToEven(f._value, 0));
    }

    public static decimal ToDecimal(FixDigitsNumber f)
    {
      return f._value;
    }

    public static double ToDouble(FixDigitsNumber f)
    {
      return decimal.ToDouble(f._value);
    }

    public static short ToInt16(FixDigitsNumber f)
    {
      return decimal.ToInt16(FixDigitsNumber.RoundToEven(f._value, 0));
    }

    public static int ToInt32(FixDigitsNumber f)
    {
      return decimal.ToInt32(FixDigitsNumber.RoundToEven(f._value, 0));
    }

    public static long ToInt64(FixDigitsNumber f)
    {
      return decimal.ToInt64(FixDigitsNumber.RoundToEven(f._value, 0));
    }

    public static float ToSingle(FixDigitsNumber f)
    {
      return decimal.ToSingle(f._value);
    }

    public string ToString(IFormatProvider provider)
    {
      return this.ToString(null, provider);
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
      return ((double)((double)this._value)).ToString(format, formatProvider);
    }

    public override string ToString()
    {
      return this.ToString(null, null);
    }

    public static bool TryParse(string s, out FixDigitsNumber result)
    {
      decimal num;
      bool flag = decimal.TryParse(s, out num);
      result = num;
      return flag;
    }

    public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out FixDigitsNumber result)
    {
      decimal num;
      bool flag = decimal.TryParse(s, style, provider, out num);
      result = num;
      return flag;
    }
  }
}