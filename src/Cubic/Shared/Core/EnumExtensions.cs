using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Cubic.Core.Collections;

namespace Cubic.Core
{
  public static class EnumExtensions
  {
    public static IEnumerable<EnumItem<E>> GetEnumItems<E>() where E : struct, IComparable, IFormattable, IConvertible
    {
      if (!typeof (E).IsEnum)
      {
        throw new NotSupportedException(string.Format("Type: {0} ist not supported! Only enums can be used.", typeof(E)));
      }

      IList<EnumItem<E>> items = new List<EnumItem<E>>();

      var values = Enum.GetValues(typeof (E));

      for (int index = 0; index < values.Count(); index++)
      {
        var value = values.GetValue(index);
        var name = Enum.GetName(typeof (E), value);

        items.Add(new EnumItem<E>(index, name, value));
      }

      return items;
    }


    public static T ToEnum<T>(this object value) where T : struct, IComparable, IFormattable, IConvertible
    {
      if (value is string) return EnumExtensions.ToEnum<T>(value.ToString());

      return (T)Enum.ToObject(typeof(T), value);
    }

    public static T ToEnum<T>( this string value ) where T : struct, IComparable, IFormattable, IConvertible
    {
      return ( T ) Enum.Parse( typeof( T ) , value , true );
    }

    public static T ToEnum<T>( this int value ) where T : struct, IComparable, IFormattable, IConvertible
    {
      return ( T ) Enum.ToObject( typeof( T ) , value );
    }

    public static T ToEnum<T>( this long value ) where T : struct, IComparable, IFormattable, IConvertible
    {
      return ( T ) Enum.ToObject( typeof( T ) , value );
    }

    public static T ToEnum<T>( this byte value ) where T : struct, IComparable, IFormattable, IConvertible
    {
      return ( T ) Enum.ToObject( typeof( T ) , value );
    }

    public static string GetName<T>( this Enum value ) where T : struct, IComparable, IFormattable, IConvertible
    {
      return Enum.GetName( typeof( T ) , value );
    }

    public static bool IsBitArray<E>() where E : struct, IComparable, IFormattable, IConvertible
    {
      return Attribute.IsDefined(typeof (E), typeof (FlagsAttribute));
    }

    public static E SetAllBits<E>() where E : struct, IComparable, IFormattable, IConvertible
    {
      var value = default(E);

      if (!typeof (E).IsEnum) return value;

      try
      {
        int allBits = 0;

        foreach (var enumValue in GetAllItems<E>())
        {
          int bitValue = 0;

          if ( !enumValue.IsNull() )
          {
            bitValue = Convert.ToInt32( enumValue );
          }
          if ( bitValue != 0 )
          {
            allBits = allBits.SetBit(bitValue, true);
          }
        }
      }
      catch (Exception)
      {
        //ignored
      }

      return value;
    }

    /// <summary>
    /// Gets all items for an enum type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T> GetAllItems<T>() where T : struct, IComparable, IFormattable, IConvertible
    {
      return Enum.GetValues( typeof( T ) ).Cast<T>();
    }

    /// <summary>
    /// Gets all combined items from an enum value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    /// <example>
    /// Displays ValueA and ValueB.
    /// <code>
    /// EnumExample dummy = EnumExample.Combi;
    /// foreach (var item in dummy.GetAllSelectedItems<EnumExample>())
    /// {
    ///    Console.WriteLine(item);
    /// }
    /// </code>
    /// </example>
    public static IEnumerable<T> GetAllSelectedItems<T>( this Enum value ) where T : struct, IComparable, IFormattable, IConvertible
    {
      var valueAsInt = Convert.ToInt32( value , CultureInfo.InvariantCulture );

      foreach ( object item in Enum.GetValues( typeof( T ) ) )
      {
        int itemAsInt = Convert.ToInt32( item , CultureInfo.InvariantCulture );

        if ( itemAsInt == ( valueAsInt & itemAsInt ) )
        {
          yield return ( T ) item;
        }
      }
    }

    /// <summary>
    /// Determines whether the enum value contains a specific value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="request">The request.</param>
    /// <returns>
    ///     <c>true</c> if value contains the specified value; otherwise, <c>false</c>.
    /// </returns>
    /// <example>
    /// <code>
    /// EnumExample dummy = EnumExample.Combi;
    /// if (dummy.Contains<EnumExample>(EnumExample.ValueA))
    /// {
    ///     Console.WriteLine("dummy contains EnumExample.ValueA");
    /// }
    /// </code>
    /// </example>
    public static bool Contains<T>( this Enum value , T request ) where  T : struct, IComparable, IFormattable, IConvertible
    {
      int valueAsInt = Convert.ToInt32( value , CultureInfo.InvariantCulture );
      int requestAsInt = Convert.ToInt32( request , CultureInfo.InvariantCulture );

      if ( requestAsInt == ( valueAsInt & requestAsInt ) )
      {
        return true;
      }

      return false;
    }
  }

  /// <summary>
  /// Represents a Enum value with the Name, Value and Index.
  /// </summary>
  /// <typeparam name="TEnum">The type of the enum.</typeparam>
  public sealed class EnumItem<TEnum> where TEnum : struct, IComparable, IFormattable, IConvertible
  {
    private string _name;

    private int _index;

    private object _rawValue;

    public EnumItem(int index, string name, object rawValue)
    {
      _index = index;
      _name = name;
      _rawValue = rawValue;
    }

    public string Name => _name;

    public int Index => _index;

    public TEnum Value => (TEnum) _rawValue;

    public object RawValue => _rawValue;
  }

  //public interface IEnumConstraint : IComparable, IConvertible, IFormattable
  //{
    
  //}
}