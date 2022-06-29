using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using Cubic.Core.Diagnostics;
using Cubic.Core.Numeric;
using Cubic.Core.Reflection;

namespace Cubic.Core.Tools
{
  public static class Converter
  {
    public static object Convert(Type targetType, object value, IFormatProvider provider)
    {
      return Converter.ConvertCore(targetType, value, provider);
    }

    public static object Convert(Type targetType, object value)
    {
      return Converter.Convert(targetType, value, CultureInfo.CurrentCulture);
    }


    public static TTarget To<TTarget>(object value)
    {
      return (TTarget)Converter.Convert(typeof(TTarget), value);
    }

    public static TTarget Convert<TFrom, TTarget>(TFrom value) where TFrom : class where TTarget : class
    {
      return (TTarget) Converter.Convert(typeof(TTarget), value);
    }

    public static object Convert(Type fromTypeCode, Type toTypeCode, object value)
    {
      if (Type.GetTypeCode(fromTypeCode) == TypeCode.String && Type.GetTypeCode(toTypeCode) == TypeCode.Boolean)
      {
        var nullableBool = ConvertStringToBool(value.ToString());

        if(toTypeCode.IsNullable())
        {
          return nullableBool;
        }

        return false;
      }

      return ConvertCore(toTypeCode, value);
    }

    private static object ConvertCore(Type type, object value)
    {
      return ConvertCore(type, value, CultureInfo.CurrentCulture);
    }

    private static object ConvertCore(Type type, object value, IFormatProvider provider)
    {

      if (type == null)
        throw new ArgumentNullException("type");

      if (value != null && value.GetType() == type)
        return value;

      var isNullable = type.IsNullable();

      if (isNullable)
        type = Nullable.GetUnderlyingType(type);

      if (value == null || value == DBNull.Value)
      {
        if (isNullable || !type.IsValueType)
          return null;
        else
          return Activator.CreateInstance(type);
      }

      var typeCode = Type.GetTypeCode(type);

      switch (typeCode)
      {
        case TypeCode.Empty:
          return null;

        case TypeCode.Object:
          return value;

        case TypeCode.DBNull:
          if (value.IsNullOrDbNull()) return DBNull.Value;
          return value;

        case TypeCode.Boolean:
          return value.ToBool();

        case TypeCode.Char:
          return value.ToChar();

        case TypeCode.SByte:
          return value.ToSByte();

        case TypeCode.Byte:
          return value.ToByte();

        case TypeCode.Int16:
          return value.ToInt16();

        case TypeCode.UInt16:
          return value.ToUInt16();

        case TypeCode.Int32:
          return value.ToInt32();

        case TypeCode.UInt32:
          return value.ToUInt32();

        case TypeCode.Int64:
          return value.ToInt64();

        case TypeCode.UInt64:
          return value.ToUInt64();

        case TypeCode.Single:
          return value.ToSingle();

        case TypeCode.Double:
          return value.ToDouble();

        case TypeCode.Decimal:
          return value.ToDecimal();

        case TypeCode.DateTime:
          return value.ToDateTime();

        case TypeCode.String:
          return value.ToString();
      }

      // fallback to System.Convert for IConvertible types
      return System.Convert.ChangeType(value, typeCode, provider);
    }



    public static FixDigitsNumber ToFixDigitsNumber(object expression)
    {
      FixDigitsNumber fixDigitsNumber;
      FixDigitsNumber fixDigitsNumber1;
      Exception exception;
      IConvertible convertible = expression as IConvertible;
      if (convertible != null)
      {
        TypeCode typeCode = convertible.GetTypeCode();
        switch (typeCode)
        {
          case TypeCode.Empty:
          case TypeCode.DBNull:
            {
              return 0;
            }
          case TypeCode.Object:
            {
              try
              {
                fixDigitsNumber1 = new FixDigitsNumber(convertible.ToDecimal(null));
              }
              catch (Exception exception1)
              {
                exception = exception1;
                break;
              }
              return fixDigitsNumber1;
            }
          default:
            {
              switch (typeCode)
              {
                case TypeCode.DateTime:
                  {
                    return 0;
                  }
                case TypeCode.Object | TypeCode.DateTime:
                  {
                    try
                    {
                      fixDigitsNumber1 = new FixDigitsNumber(convertible.ToDecimal(null));
                    }
                    catch (Exception exception1)
                    {
                      exception = exception1;
                      break;
                    }
                    return fixDigitsNumber1;
                  }
                case TypeCode.String:
                  {
                    if (FixDigitsNumber.TryParse(expression as string, out fixDigitsNumber))
                    {
                      return fixDigitsNumber;
                    }
                    return 0;
                  }
                default:
                  {
                    try
                    {
                      fixDigitsNumber1 = new FixDigitsNumber(convertible.ToDecimal(null));
                    }
                    catch (Exception exception1)
                    {
                      exception = exception1;
                      break;
                    }
                    return fixDigitsNumber1;
                  }
              }
              break;
            }
        }
      }
      return 0;
    }


    #region Bool


    private static bool ValidBooleanTrue(string parameterValue)
    {
      return ((string.Compare(parameterValue, "true", StringComparison.OrdinalIgnoreCase) == 0) ||
              (string.Compare(parameterValue, "on", StringComparison.OrdinalIgnoreCase) == 0) ||
              (string.Compare(parameterValue, "yes", StringComparison.OrdinalIgnoreCase) == 0) ||
              (string.Compare(parameterValue, "!false", StringComparison.OrdinalIgnoreCase) == 0) ||
              (string.Compare(parameterValue, "!off", StringComparison.OrdinalIgnoreCase) == 0) ||
              (string.Compare(parameterValue, "!no", StringComparison.OrdinalIgnoreCase) == 0));
    }


    private static bool ValidBooleanFalse(string parameterValue)
    {
      return ((string.Compare(parameterValue, "false", StringComparison.OrdinalIgnoreCase) == 0) ||
              (string.Compare(parameterValue, "off", StringComparison.OrdinalIgnoreCase) == 0) ||
              (string.Compare(parameterValue, "no", StringComparison.OrdinalIgnoreCase) == 0) ||
              (string.Compare(parameterValue, "!true", StringComparison.OrdinalIgnoreCase) == 0) ||
              (string.Compare(parameterValue, "!on", StringComparison.OrdinalIgnoreCase) == 0) ||
              (string.Compare(parameterValue, "!yes", StringComparison.OrdinalIgnoreCase) == 0));
    }

    /// <summary>
    /// Returns true if the string can be successfully converted to a bool,
    /// such as "on" or "yes"
    /// </summary>
    internal static bool CanConvertStringToBool(string parameterValue)
    {
      return (ValidBooleanTrue(parameterValue) || ValidBooleanFalse(parameterValue));
    }


    /// <summary>
    /// Converts a string to a bool.  We consider "true/false", "on/off", and 
    /// "yes/no" to be valid boolean representations in the XML.
    /// </summary>
    /// <param name="parameterValue">The string to convert.</param>
    /// <returns>Boolean true or false, corresponding to the string.</returns>
    internal static bool? ConvertStringToBool(string parameterValue)
    {
      if (ValidBooleanTrue(parameterValue))
      {
        return true;
      }
      else if (ValidBooleanFalse(parameterValue))
      {
        return false;
      }
      else
      {
        // Unsupported boolean representation.
        //error.VerifyThrowArgument(false, "Shared.CannotConvertStringToBool", parameterValue);
        return null;
      }
    }

    #endregion

    #region Numbers

    /// <summary>
    /// Converts a string like "123.456" or "0xABC" into a double.
    /// Tries decimal conversion first.
    /// </summary>
    public static double ConvertDecimalOrHexToDouble(string number)
    {
      if (Converter.ValidDecimalNumber(number))
      {
        return Converter.ConvertDecimalToDouble(number);
      }
      else if (Converter.ValidHexNumber(number))
      {
        return Converter.ConvertHexToDouble(number);
      }
      else
      {
        throw new FormatException($" {number} could not be Converted to double");
        //ErrorUtilities.VerifyThrow(false, "Cannot numeric evaluate");
        //return 0.0D;
      }
    }

    public static bool IsNumeric(object expression)
    {
      return double.TryParse(System.Convert.ToString(expression, CultureInfo.InvariantCulture), NumberStyles.Any, (IFormatProvider)NumberFormatInfo.InvariantInfo, out double num);
    }

    /// <summary>
    /// Converts a string like "123.456" into a double. Leading sign is allowed.
    /// </summary>
    internal static double ConvertDecimalToDouble(string number)
    {
      return Double.Parse(number, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture.NumberFormat);
    }

    /// <summary>
    /// Converts a hex string like "0xABC" into a double.
    /// </summary>
    internal static double ConvertHexToDouble(string number)
    {
      return (double)Int32.Parse(number.Substring(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture.NumberFormat);
    }

    /// <summary>
    /// Returns true if the string is a valid hex number, like "0xABC"
    /// </summary>
    private static bool ValidHexNumber(string number)
    {
      bool canConvert = false;
      if (number.Length >= 3 && number[0] == '0' && (number[1] == 'x' || number[1] == 'X'))
      {
        int value;
        canConvert = Int32.TryParse(number.Substring(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture.NumberFormat, out value);
      }
      return canConvert;
    }

    /// <summary>
    /// Returns true if the string is a valid decimal number, like "-123.456"
    /// </summary>
    private static bool ValidDecimalNumber(string number)
    {
      double value;
      return Double.TryParse(number, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture.NumberFormat, out value);
    }

    /// <summary>
    /// Returns true if the string is a valid decimal or hex number
    /// </summary>
    internal static bool ValidDecimalOrHexNumber(string number)
    {
      return ValidDecimalNumber(number) || ValidHexNumber(number);
    }

    #endregion

    #region String

    /// <summary>
    /// Returns a hex representation of a byte array.
    /// </summary>
    /// <param name="bytes">The bytes to convert</param>
    /// <returns>A string byte types formated as X2.</returns>
    internal static string ConvertByteArrayToHex(byte[] bytes)
    {
      var sb = new StringBuilder();
      foreach (var b in bytes)
      {
        sb.AppendFormat("{0:X2}", b);
      }

      return sb.ToString();
    }

    #endregion


    ////public static Version Parse(int versiopnExpression)
    ////{
    ////  //var bytes = BitConverter.GetBytes(versiopnExpression);

    ////  //if(BitConverter.IsLittleEndian) Array.Reverse(bytes);

    ////  var bits = new BitVector32(versiopnExpression);
    ////  return new Version(bits[0], bits[1], bits[2], bits[3]);
    ////}
    ///


    /// <summary>
    /// Returns a value converted to the specified type using the specified CultureInfo.
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="destinationType">Type that the value needs to be converted to.</param>
    /// <param name="culture">The culture to use for the conversion. If null will use the curent culture.</param>
    /// <returns>If the value can be converted to the specified type, the converted value will be returned. If the value is already of that type or cannot be converted to that type, the <paramref name="value" /> will be returned; otherwise null is returned.</returns>
    public static object ConvertValue(object value, Type destinationType, CultureInfo culture)
    {
      Guard.ArgumentNull(destinationType, nameof(destinationType));

      if (value == null)
      {
        return null;
      }

      if (destinationType.IsGenericType && destinationType.GetGenericTypeDefinition() == typeof(Nullable<>))
      {
        destinationType = Nullable.GetUnderlyingType(destinationType);
      }

      Type type = value.GetType();
      if (type == destinationType || destinationType.IsAssignableFrom(type))
      {
        return value;
      }

      if (culture == null)
      {
        culture = CultureInfo.CurrentCulture;
      }

      if (value is IConvertible && (destinationType.IsPrimitive || destinationType == typeof(string) || destinationType == typeof(DateTime)))
      {
        return System.Convert.ChangeType(value, destinationType, culture);
      }

      TypeConverter converter = TypeDescriptor.GetConverter(destinationType);

      if (converter.CanConvertFrom(type))
      {
        return converter.ConvertFrom(null, culture, value);
      }

      TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
      if (typeConverter.CanConvertTo(destinationType))
      {
        return typeConverter.ConvertTo(null, culture, value, destinationType);
      }

      if (!destinationType.IsEnum || !type.IsValueType)
      {
        return value;
      }

      return Enum.ToObject(destinationType, value);
    }

    /// <summary>
    /// Used to convert an object value to an enum of the specified type.
    /// </summary>
    /// <param name="value">Object to convert</param>
    /// <param name="defaultValue">Default value - the enum type is derived from the value and this value is when an invalid value is specified.</param>
    /// <returns>The value converted to an enum of the same type as the default value or the default value itself is returned if the value is invalid for the specified enum type</returns>
    /// <remarks>
    /// The value is verified to be a defined enum value for the specified enum type so
    /// this method should not be used where the enum type is a flagged enumeration
    /// since the IsDefined method of Enum will return false when a combined bit
    /// value is specified.
    /// </remarks>
    public static Enum ConvertEnum(object value, Enum defaultValue)
    {
      object obj;
      if (value == null)
      {
        return defaultValue;
      }
      Type type = value.GetType();
      Type type1 = defaultValue.GetType();
      if (type.IsEnum && type != type1)
      {
        return defaultValue;
      }
      obj = (type != type1 ? Converter.ConvertValue(value, Enum.GetUnderlyingType(type1)) : value);
      if (Enum.IsDefined(type1, obj))
      {
        return (Enum)Enum.ToObject(type1, obj);
      }
      object[] customAttributes = type1.GetCustomAttributes(typeof(FlagsAttribute), false);
      if ((int)customAttributes.Length <= 0 || !(customAttributes[0] is FlagsAttribute))
      {
        return defaultValue;
      }
      return (Enum)Enum.ToObject(type1, obj);
    }

    /// <summary>
    /// Converts a value to a string.
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="useCurrentCulture">If true, uses the current culture, otherwise, the invariant culture is used.</param>
    /// <returns>The converted string.</returns>
    public static string ConvertToString(object value, bool useCurrentCulture)
    {
      return Converter.ConvertToString(value, null, useCurrentCulture);
    }

    /// <summary>
    /// Converts a value to a string.
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="format">The format string.</param>
    /// <param name="useCurrentCulture">If true, uses the current culture, otherwise, the invariant culture is used.</param>
    /// <returns>The converted string.</returns>
    public static string ConvertToString(object value, string format, bool useCurrentCulture)
    {
      return Converter.ConvertToString(value, format, (useCurrentCulture ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Converts a value to a string.
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="formatProvider">The format provider to use for the conversion.</param>
    /// <returns>The converted string.</returns>
    public static string ConvertToString(object value, IFormatProvider formatProvider)
    {
      return Converter.ConvertToString(value, null, formatProvider);
    }

    /// <summary>
    /// Converts a value to a string.
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="format">The format string.</param>
    /// <param name="formatProvider">The format provider to use for the conversion.</param>
    /// <returns>The converted string.</returns>
    public static string ConvertToString(object value, string format, IFormatProvider formatProvider)
    {
      if (value == null)
      {
        return null;
      }
      if (formatProvider == null)
      {
        formatProvider = CultureInfo.CurrentCulture;
      }
      IFormattable formattable = value as IFormattable;
      if (formattable != null)
      {
        return formattable.ToString(format, formatProvider);
      }
      CultureInfo cultureInfo = formatProvider as CultureInfo ?? CultureInfo.CurrentCulture;
      return Converter.ConvertValue(value, typeof(string), cultureInfo) as string;
    }

    /// <summary>
    /// Returns a value converted to the specified type using the InvariantCulture.
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="destinationType">Type that the value needs to be converted to.</param>
    /// <returns>If the value can be converted to the specified type, the converted value will be returned. If the value is already of that type or cannot be converted to that type, the <paramref name="value" /> will be returned; otherwise null is returned.</returns>
    public static object ConvertValue(object value, Type destinationType)
    {
      return Converter.ConvertValue(value, destinationType, false);
    }

    /// <summary>
    /// Returns a value converted to the specified type using the InvariantCulture or CurrentCulture.
    /// </summary>
    /// <param name="value">Value to convert</param>
    /// <param name="destinationType">Type that the value needs to be converted to.</param>
    /// <param name="useCurrentCulture">If true, uses the current culture, otherwise, the invariant culture is used.</param>
    /// <returns>If the value can be converted to the specified type, the converted value will be returned. If the value is already of that type or cannot be converted to that type, the <paramref name="value" /> will be returned; otherwise null is returned.</returns>
    public static object ConvertValue(object value, Type destinationType, bool useCurrentCulture)
    {
      return Converter.ConvertValue(value, destinationType, (useCurrentCulture ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture));
    }

    public static TryParseDelegate GetTryParse(Type type)
    {
      var method = type.GetMethod("TryParse", new[] { typeof(string), type });

      if (method == null) return null;

      return (TryParseDelegate)method.CreateDelegate(typeof(TryParseDelegate));
    }


    public static TryParseFormatDelegate GetTryParseFormat(Type type)
    {
      var method = type.GetMethod("TryParse", new[] { typeof(string), typeof(string), type });

      if (method == null) return null;

      return (TryParseFormatDelegate)method.CreateDelegate(typeof(TryParseFormatDelegate));
    }

    public static bool TryParse(Type type, string input, out object value, string format = null)
    {

      if(format == null)
      {
        var tryParseMethod = GetTryParse(type);
        if(tryParseMethod != null)
        {
          if(tryParseMethod(input, out value)) return true;
        }
      }
      else
      {
        var tryParseMethod = GetTryParseFormat(type);
        if (tryParseMethod != null)
        {
          if (tryParseMethod(input, format, out value)) return true;
        }
      }

      var converter = TypeDescriptor.GetConverter(type);

      if(converter != null && converter.CanConvertFrom(typeof(string)))
      {
        value = converter.ConvertFrom(input);
        return true;
      }

      try
      {
        value = System.Convert.ChangeType(input, type);
        return true;
      }
      catch (Exception)
      {
        value = type.GetDefault();
        return false;
      }
    }

    public delegate bool TryParseDelegate(string input, out object output);

    public delegate bool TryParseFormatDelegate(string input, string format, out object output);
  }
}