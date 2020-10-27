using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using Cubic.Core.Annotations;
using Cubic.Core.Text;

namespace Cubic.Core
{
  /// <summary>
  /// Varius Extension for the Object-Type
  /// </summary>
  public static class ObjectExtensions
  {
    public static string ToInvariant(this object expression)
    {
      return string.Format(CultureInfo.InvariantCulture, "{0}", expression);
    }

    /// <summary>
    /// Formats the value of the object to valid SQL-String.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    public static string ToSql( this object expression )
    {
      return SqlStrings.ToSql(expression);
    }

    /// <summary>
    /// Gets the value or <see cref="DBNull.Value"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns>value or <see cref="DBNull.Value"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object DbValueOrNull<T>(this T? value) where T : struct
    {
      return value.HasValue ? (object)value.Value : DBNull.Value;
    }

    /// <summary>
    /// To the bytes.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static byte[] ToBytes( this object expression )
    {
      if ( expression is byte[] ) return ( byte[] )expression;

      if ( expression is string )
      {
        var text = expression.ToString();
        byte[] bytes = new byte[text.Length * sizeof( char )];
        System.Buffer.BlockCopy( text.ToCharArray() , 0 , bytes , 0 , bytes.Length );
        return bytes;
      }
      if ( expression is char )
      {
        return BitConverter.GetBytes( ( char ) expression );
      }
      if ( expression is bool )
      {
        return BitConverter.GetBytes( ( bool ) expression );
      }
      if ( expression is short )
      {
        return BitConverter.GetBytes( ( short ) expression );
      }
      if ( expression is int )
      {
        return BitConverter.GetBytes( ( int ) expression );
      }
      if ( expression is long )
      {
        return BitConverter.GetBytes( ( long ) expression );
      }

      return ComplexObjectToBytes(expression);
    }

    /// <summary>
    /// Complexes the object to bytes.
    /// </summary>
    /// <param name="complexObject">The complex object.</param>
    /// <returns></returns>
    public static byte[] ComplexObjectToBytes(object complexObject)
    {
      using (var stream = new MemoryStream())
      {
        new BinaryFormatter().Serialize(stream, complexObject);
        stream.Seek(0, SeekOrigin.Begin);
        return stream.ToArray();
      }
    }

    /// <summary>
    /// Complexes the object from bytes.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns></returns>
    public static object ComplexObjectFromBytes(byte[] data)
    {
      using (var stream = new MemoryStream(data))
      {
        return new BinaryFormatter().Deserialize(stream);
      }
    }

    /// <summary>
    /// To the string enhanced.
    /// Wrap the object with the wrapper.
    /// <example>
    /// *0* = 0.ToStringEnhanced("*");
    /// </example>
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="wrapper">The wrapper.</param>
    /// <returns></returns>
    public static string ToStringEnhanced( this object expression , string wrapper )
    {
      string result = string.Empty;

      try
      {
        result = string.Format( "{1}{0}{1}" , expression , wrapper );
      }
      catch
      {

      }

      return result;
    }

    /// <summary>
    /// To the string enhanced.
    /// Wrap the object with the wrapper.
    /// <example>
    /// *0* = 0.ToStringEnhanced("*");
    /// </example>
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="wrapper">The wrapper.</param>
    /// <returns></returns>
    public static string ToStringEnhanced(this object obj, char wrapper)
    {
      return obj.ToStringEnhanced(wrapper.ToString());
    }

    /// <summary>
    /// To the string enhanced.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static string ToStringEnhanced( this object expression )
    {
      string result = string.Empty;

      try
      {
        if (expression is string)
        {
          result = expression.ToString();
        }

        if ( expression is char )
        {
          result = expression.ToString();
        }

        if ( expression is bool )
        {
          result = expression.ToString().ToLowerInvariant();
        }

        if ( expression is byte )
        {
          result = ((byte)expression).ToString();
        }

        if ( expression is byte[] )
        {
          byte[] bytes = expression as byte[];

          // Use ExtensionMethod
          result = bytes.BytesToString();
          //result = System.Text.Encoding.Unicode.GetString(bytes);


        }

        if ( expression is short )
        {
          result = ((short)expression).ToString(CultureInfo.InvariantCulture);
        }

        if ( expression is int )
        {
          result = ( ( int ) expression ).ToString( CultureInfo.InvariantCulture );
        }

        if ( expression is long )
        {
          result = ( ( long ) expression ).ToString( CultureInfo.InvariantCulture );
        }

        if ( expression is BigInteger )
        {
          result = ( ( BigInteger ) expression ).ToString( CultureInfo.InvariantCulture );
        }

        if ( expression is Guid )
        {
          result = ( ( Guid ) expression ).ToString();
        }

        if ( expression is DateTime )
        {
          result = ( ( DateTime ) expression ).ToString( CultureInfo.InvariantCulture );
        }

        if ( expression is TimeSpan )
        {
          result = ( ( TimeSpan ) expression ).ToString();
        }
      }
      catch
      {

      }

      return result;
    }

    /// <summary>
    /// Returns the first char from string.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    public static char ToChar(this string text)
    {
      if (string.IsNullOrEmpty(text))
      {
        return char.MinValue;
      }

      return text[0];
    }

    /// <summary>
    /// To the character.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static char ToChar( this object expression )
    {
      if (expression is char) return (char) expression;

      return Convert.ToChar(expression);
    }

    ///// <summary>
    ///// To the int32.
    ///// </summary>
    ///// <param name="expression">The expression.</param>
    ///// <returns></returns>
    //public static int ToInt32( this object expression )
    //{
    //  return ToInt32(expression, CultureInfo.CurrentCulture);
    //}

    /// <summary>
    /// To the int32.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static int ToInt32(this object expression, IFormatProvider provider = null)
    {
      int result = 0;

      if (expression is int) return (int)expression;

      if (int.TryParse(expression.ToString(), out result))
      {
        return result;
      }

      return Convert.ToInt32(expression, provider);
    }

    /// <summary>
    /// To the int64.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static long ToInt64(this object expression, IFormatProvider provider = null)
    {
      var convertible = expression as IConvertible;
      if (convertible != null)
      {
        switch (convertible.GetTypeCode())
        {
          case TypeCode.Empty:
          case TypeCode.DBNull:
          case TypeCode.DateTime:
            return 0;

          case TypeCode.String:
            {
              long result;
              if (!long.TryParse(expression as string, out result))
              {
                return 0;
              }
              return result;
            }
        }
        try
        {
          return convertible.ToInt64(provider);
        }
        catch
        {
          // ignored
        }
      }
      return 0;
    }

    /// <summary>
    /// To the int16.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static short ToInt16( this object expression, IFormatProvider provider = null)
    {
      short result = 0;

      try
      {
        result = Convert.ToInt16( expression, provider );
      }
      catch
      {
        // ignored
      }
      return result;
    }

    /// <summary>
    /// To the u int16.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    public static ushort ToUInt16(this object obj, IFormatProvider provider = null)
    {
      ushort result = 0;

      try
      {
        result = Convert.ToUInt16(obj, provider);
      }
      catch
      {
        // ignored
      }
      return result;
    }

    /// <summary>
    /// To the u int32.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    public static uint ToUInt32(this object obj, IFormatProvider provider = null)
    {
      uint result = 0;

      try
      {
        result = Convert.ToUInt32(obj, provider);
      }
      catch (Exception)
      {

      }
      return result;
    }

    /// <summary>
    /// To the u int64.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns></returns>
    public static ulong ToUInt64(this object obj, IFormatProvider provider = null)
    {
      return Catch<ulong, Exception>(() => Convert.ToUInt64(obj, provider));
    }

    /// <summary>
    /// To the float.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static float ToFloat( this object expression )
    {
      return expression.ToFloat();
    }
    /// <summary>
    /// To the single.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static float ToSingle(this object expression)
    {
      return ToSingle(expression);
    }

    /// <summary>
    /// To the float.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="useInvariantCulture">if set to <c>true</c> [use invariant culture].</param>
    /// <returns></returns>
    public static float ToFloat(this object expression, IFormatProvider provider = null)
    {
      return ToSingle(expression, provider);
    }

    /// <summary>
    /// To the single.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="useInvariantCulture">if set to <c>true</c> [use invariant culture].</param>
    /// <returns></returns>
    public static float ToSingle(this object expression, IFormatProvider provider = null)
    {
      var convertible = expression as IConvertible;
      if (convertible != null)
      {
        switch (convertible.GetTypeCode())
        {
          case TypeCode.Empty:
          case TypeCode.DBNull:
          case TypeCode.Char:
          case TypeCode.DateTime:
            return 0f;

          case TypeCode.SByte:
          case TypeCode.Byte:
          case TypeCode.Int16:
          case TypeCode.UInt16:
          case TypeCode.Int32:
          case TypeCode.UInt32:
          case TypeCode.Decimal:
            return Convert.ToSingle(expression, CultureInfo.InvariantCulture);

          case TypeCode.String:
            float num;
            if (!float.TryParse(expression as string, NumberStyles.Any, provider, out num))
            {
              return 0f;
            }
            return num;
        }
        try
        {
          return convertible.ToSingle(provider);
        }
        catch
        {
          // ignored
        }
      }

      return 0f;
    }

    /// <summary>
    /// To the double.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static double ToDouble( this object expression )
    {
      if (expression is double) return (double)expression;

      return expression.ToDouble();
    }

    /// <summary>
    /// To the double.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="useInvariantCulture">if set to <c>true</c> [use invariant culture].</param>
    /// <returns></returns>
    public static double ToDouble(this object expression, IFormatProvider provider = null)
    {

      var convertible = expression as IConvertible;
      if (convertible != null)
      {
        switch (convertible.GetTypeCode())
        {
          case TypeCode.Empty:
          case TypeCode.DBNull:
          case TypeCode.Char:
          case TypeCode.DateTime:
            return 0.0;

          case TypeCode.SByte:
          case TypeCode.Byte:
          case TypeCode.Int16:
          case TypeCode.UInt16:
          case TypeCode.Int32:
          case TypeCode.UInt32:
          case TypeCode.Decimal:
            return Convert.ToDouble(expression, provider);

          case TypeCode.String:
            double num;
            if (!double.TryParse(expression as string, NumberStyles.Any, provider, out num))
            {
              return 0.0;
            }
            return num;
        }
        try
        {
          return convertible.ToDouble(provider);
        }
        catch
        {
          // ignored
        }
      }

      return 0.0;
    }

    /// <summary>
    /// To the decimal.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static decimal ToDecimal( this object expression )
    {
      if (expression is decimal) return (decimal)expression;

      return expression.ToDecimal();
    }

    /// <summary>
    /// To the decimal.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="useInvariantCulture">if set to <c>true</c> [use invariant culture].</param>
    /// <returns></returns>
    public static decimal ToDecimal(this object expression, IFormatProvider provider = null)
    {

      var convertible = expression as IConvertible;
      if (convertible != null)
      {
        switch (convertible.GetTypeCode())
        {
          case TypeCode.Empty:
          case TypeCode.DBNull:
          case TypeCode.Char:
          case TypeCode.DateTime:
            return 0M;

          case TypeCode.SByte:
          case TypeCode.Byte:
          case TypeCode.Int16:
          case TypeCode.UInt16:
          case TypeCode.Int32:
          case TypeCode.UInt32:
          case TypeCode.Decimal:
            return Convert.ToDecimal(expression, provider);

          case TypeCode.String:
            {
              decimal result;
              if (!decimal.TryParse(expression as string, NumberStyles.Any, provider, out result))
              {
                return 0M;
              }
              return result;
            }
        }
        try
        {
          return convertible.ToDecimal(provider);
        }
        catch
        {
          // ignored
        }
      }

      return 0M;
    }

    /// <summary>
    /// To the byte.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static byte ToByte( this object expression )
    {

      if (expression is byte) return (byte)expression;

      return Convert.ToByte( expression );
    }

    public static sbyte ToSByte(this object expression)
    {

      if (expression is sbyte) return (sbyte)expression;

      return Convert.ToSByte(expression);
    }

    /// <summary>
    /// To the unique identifier.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static Guid ToGuid( this object expression )
    {
      Guid result;

      if (expression is Guid) return (Guid)expression;

      if ( Guid.TryParse(expression.ToString() , out result ) )
      {
        return result;
      }
      return Guid.Empty;
    }

    /// <summary>
    /// To the date time.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static DateTime ToDateTime( this object expression, IFormatProvider provider = null)
    {
      DateTime result;

      if ( expression is DateTime ) return ( DateTime )expression;

      if (DateTime.TryParse(expression.ToString(), provider, DateTimeStyles.None, out result))
      {
        return result;
      }

      return DateTime.MinValue;
    }

    /// <summary>
    /// To the date time.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static DateTimeOffset ToDateTimeOffset(this object expression, IFormatProvider provider = null)
    {
      DateTimeOffset result;

      if (expression is DateTimeOffset) return (DateTimeOffset)expression;

      if (DateTimeOffset.TryParse(expression.ToString(), provider, DateTimeStyles.None, out result))
      {
        return result;
      }

      return DateTimeOffset.MinValue;
    }

    /// <summary>
    /// To the time span.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static TimeSpan ToTimeSpan( this object expression, IFormatProvider provider = null)
    {
      TimeSpan result;
      if ( expression is TimeSpan ) return ( TimeSpan )expression;

      if (TimeSpan.TryParse(expression.ToString(), provider, out result))
      {
        return result;
      }

      return TimeSpan.Zero;
    }

    /// <summary>
    /// To the bool.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static bool ToBool( this object expression, IFormatProvider provider = null)
    {
      if ( expression is bool )
      {
        return ( bool )expression;
      }

      if ( expression is byte )
      {
        return ObjectExtensions.ToBool( ( byte ) expression );
      }


      if ( expression is int )
      {
        return ObjectExtensions.ToBool( ( int ) expression );
      }

      if ( expression is short )
      {
        return ObjectExtensions.ToBool( Convert.ToInt32( expression, provider ) );
      }

      if ( expression is long )
      {
        return ObjectExtensions.ToBool( Convert.ToInt32( expression, provider ) );
      }

      if ( expression is string )
      {
        return ObjectExtensions.ToBool( ( string ) expression );
      }

      return Core.Tools.Converter.To<bool>(expression);
      return Convert.ToBoolean( expression );

    }

    /// <summary>
    /// To the bool.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>TRUE if the string is "yes" , "ja", "true", "wahr" </returns>
    public static bool ToBool( string expression )
    {
      var trueValues = new string[] { "yes" , "ja", "true", "wahr", "!false", "!nein", "!falsch", "!no", "!off" };

      if ( trueValues.Contains(expression.ToLowerInvariant().Trim() ) ) return true;

      return bool.Parse(expression);
    }


    public static bool ToBool( int expression )
    {
      var trueValues = new int[] { 1, -1 };

      if (trueValues.Contains(expression)) return true;

      return false;

      //return expression == 1;
    }

    /// <summary>
    /// To the database boolean.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>Returns -1 if <see cref="expression"/> is evaluated to TRUE</returns>
    public static short ToDatabaseBoolean(this object expression)
    {
      if (ToBool(expression))
      {
        return -1;
      }
      return 0;
    }

    /// <summary>
    /// To the bool.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    public static bool ToBool( byte expression )
    {
      if (1 == expression) return true;

      return false;
    }

    /// <summary>
    /// Returns default of Type if not successfull
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    [CanBeNull]
    public static T SafeCast<T>([CanBeNull] this object value)
    {
      return (value.IsNull()) ? default(T) : (T)value;
    }


    /// <summary>
    /// Determines whether the specified object is null.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns><count>true</count> if the specified object is null; otherwise, <count>false</count>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNull( this object expression )
    {
      return ( expression is null );
    }

    /// <summary>
    /// Ors the default.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expression">The expression.</param>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public static T OrDefault<T>(this object expression, Type type)
    {
      if (expression != null)
      {
        return (T)expression;
      }

      // is ValueType?
      if (type.IsValueType)
      {
        return default(T);
      }

      return default(T);
    }

    /// <summary>
    /// Ors the default.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T OrDefault<T>(this T expression) where T : class 
    {
      if (expression != null)
      {
        return expression;
      }

      //// is ValueType?
      //if (typeof (T).IsValueType)
      //{
      //  return default(T);
      //}

      return default(T);
    }

    /// <summary>
    /// Ors the default.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue OrDefault<TValue>(this TValue? expression) where TValue : struct 
    {
      if (expression.HasValue)
      {
        return expression.Value;
      }

      return default(TValue);
    }

    /// <summary>
    /// Ors the specified value if null.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="valueIfNull">The value if null.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue Or<TValue>(this TValue? value, TValue valueIfNull) where TValue : struct
    {
      if (value.HasValue) return value.Value;

      return valueIfNull;
    }

    /// <summary>
    /// Ors the specified value if null.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="instance">The instance.</param>
    /// <param name="valueIfNull">The value if null.</param>
    /// <returns></returns>
    public static TValue Or<TValue>(this TValue instance, TValue valueIfNull) where TValue : class 
    {
      if (instance == null) return valueIfNull;

      return instance;
    }

    /// <summary>
    /// Determines whether this instance is default.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="expression">The expression.</param>
    /// <returns>
    ///   <c>true</c> if the specified expression is default; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsDefault<TValue>(this TValue expression) where TValue : struct , IComparable, IComparable<TValue>
    {
      return default(TValue).CompareTo(expression) == 0;
    } 

    /// <summary>
    /// To the object array.
    /// </summary>
    /// <param name="argumets">The argumets.</param>
    /// <returns></returns>
    public static object[] ToObjectArray( params object[] argumets )
    {
      if (argumets != null) return argumets;

      return new object[] {};
    }

    public static object[] ToObjectArray( this object expression )
    {
      return ObjectExtensions.ToObjectArray( argumets: expression );
    }

    /// <summary>
    /// Determines whether this instance is type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns>
    ///   <c>true</c> if the specified value is type; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is<T>( this object value )
    {
      return ( value is T );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDbNull(this object value)
    {
      return object.ReferenceEquals(DBNull.Value, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrDbNull(this object value)
    {
      return value.IsNull() || value.IsDbNull();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> Yield<T>(this T value)
    {
      yield return value;
    }

    public static IEnumerable<T> Yield<TEnumerable, T>(this TEnumerable collection) where TEnumerable : IEnumerable<T>
    {
      foreach(T item in collection)
      {
        yield return item;
      }
    }

    /// <summary>
    /// Tries the cast.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expression">The expression.</param>
    /// <param name="expressionT">The expression t.</param>
    /// <returns></returns>
    public static bool TryCast<T>(this object expression, out T expressionT) where T : class
    {
      expressionT = default(T);

      if (expression.Is<T>())
      {
        expressionT = expression as T;
        return true;
      }

      return false;
    }

    /// <summary>
    /// Prüft ob value zwischen den übergebenen Grenzen liegt oder einer dieser entspricht
    /// </summary>
    /// <typeparam name="T">Typ der Prüfung muss IComparable implementieren</typeparam>
    /// <param name="value">Wert der geprüft werden soll</param>
    /// <param name="min">Mindestwert der eine positive Prüfung ergibt</param>
    /// <param name="max">Maximalwert der eine positive Prüfung ergibt</param>
    /// <returns></returns>
    public static bool IsBetween<T>( this T value , T min , T max ) where T : IComparable
    {
      return ( Comparer<T>.Default.Compare( min , max ) <= 0 && Comparer<T>.Default.Compare( max , value ) >= 0 );
    }

    /// <summary>
    /// Catches a <see cref="E"/> if the specified function throws.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <param name="function">The function.</param>
    /// <returns> default of <see cref="T"/> if <see cref="function"/> throws <see cref="E"/> </returns>
    public static T Catch<T, E>( Func<T> function) where E : Exception
    {
      try
      {
        return function();
      }
      catch (E)
      {

      }

      return default(T);
    }

    #region Numbers

    public static int CountOfBitsSet( long v )
    {
      // http://graphics.stanford.edu/~seander/bithacks.htm
      int c = 0;
      while ( v != 0 )
      {
        // clear the least significant bit set
        v &= unchecked(v - 1);
        c++;
      }

      return c;
    }

    public static bool IsPrime(this int number)
    {
      if (number == 1) return false;
      if (number == 2) return true;

      for (int i = 2; i <= Math.Ceiling(Math.Sqrt(number)); ++i)
      {
        if (number % i == 0) return false;
      }

      return true;
    }

    #endregion
  }
}