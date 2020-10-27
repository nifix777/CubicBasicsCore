using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core
{
  [Serializable]
  public struct Enum<TEnum> : IEquatable<TEnum>, IComparable<TEnum>, IFormattable, /*IEquatable<Enum<TEnum>,*/ ISerializable where TEnum : struct, IComparable, IFormattable, IConvertible
  {
    private const string NameSerData = "Name";
    private const string ValueSerData = "Value";
    private readonly string name;

    /// <summary>
    /// Represents value of the enum member.
    /// </summary>
    public TEnum Value { get; }

    /// <summary>
    /// Represents name of the enum member.
    /// </summary>
    public string Name => name ?? Enum.GetName(typeof(TEnum), Value);


    private Enum(TEnum value, string name)
    {
      Value = value;
      this.name = name;
    }

    private Enum(SerializationInfo info, StreamingContext context)
    {
      name = info.GetString(NameSerData);
      Value = (TEnum)info.GetValue(ValueSerData, typeof(TEnum));
    }

    /// <summary>
    /// Determines whether two enum members are equal.
    /// </summary>
    /// <param name="other">Other member to compare.</param>
    /// <returns><see langword="true"/> if this enum member is the same as other; otherwise, <see langword="false"/>.</returns>
    public bool Equals(Enum<TEnum> other) => Equals(other.Value) && Equals(Name, other.Name);

    /// <summary>
    /// Determines whether this value equals to the other enum value.
    /// </summary>
    /// <param name="other">Other value to compare.</param>
    /// <returns>Equality check result.</returns>
    public override bool Equals(object other)
    {
      switch (other)
      {
        case Enum<TEnum> en:
          return Equals(en);
        case TEnum en:
          return Equals(en);
        default:
          return false;
      }
    }

    /// <summary>
    /// Gets hash code of the enum member.
    /// </summary>
    /// <returns>The hash code of the enum member.</returns>
    public override int GetHashCode()
    {
      var hashCode = -1670801664;
      hashCode = hashCode * -1521134295 + Value.GetHashCode();
      hashCode = hashCode * -1521134295 + Name.GetHashCode();
      return hashCode;
    }


    private readonly struct Tuple : IEquatable<Tuple>
    {
      internal readonly string Name;
      internal readonly TEnum Value;

      private Tuple(string name)
      {
        Name = name;
        Value = default;
      }

      private Tuple(TEnum value)
      {
        Value = value;
        Name = default;
      }

      public static implicit operator Tuple(string name) => new Tuple(name);
      public static implicit operator Tuple(TEnum value) => new Tuple(value);

      public bool Equals(Tuple other)
          => Name is null ? other.Name is null && Value.Equals(other.Value) : Name == other.Name;

      public override bool Equals(object other) => other is Tuple t && Equals(t);
      public override int GetHashCode() => Name is null ? Value.GetHashCode() : Name.GetHashCode();
    }

    private sealed class Mapping : Dictionary<Tuple, long>
    {
      internal readonly Enum<TEnum>[] Members;

      internal Mapping(out Enum<TEnum> min, out Enum<TEnum> max)
      {
        var names = Enum.GetNames(typeof(TEnum));
        var values = (TEnum[])Enum.GetValues(typeof(TEnum));
        Members = new Enum<TEnum>[names.LongLength];
        min = max = default;
        for (var index = 0L; index < names.LongLength; index++)
        {
          var entry = Members[index] = new Enum<TEnum>(values[index], names[index]);
          Add(entry.Name, index);
          base[entry.Value] = index;
          //detect min and max
          min = entry.Value.CompareTo(min.Value) < 0 ? entry : min;
          max = entry.Value.CompareTo(max.Value) > 0 ? entry : max;
        }
      }

      internal Enum<TEnum> this[string name] => Members[base[name]];

      internal bool TryGetValue(TEnum value, out Enum<TEnum> member)
      {
        if (base.TryGetValue(value, out var index))
        {
          member = Members[index];
          return true;
        }
        else
        {
          member = default;
          return false;
        }
      }
    }


    /// <summary>
    /// Returns an indication whether a constant with a specified value exists in a enumeration of type <typeparamref name="E"/>.
    /// </summary>
    /// <param name="value">The value of a constant in <typeparamref name="E"/>.</param>
    /// <returns><see langword="true"/> if a constant in <typeparamref name="E"/> has a value equal to <paramref name="value"/>; otherwise, <see langword="false"/>.</returns>
    public static bool IsDefined(TEnum value) => mapping.ContainsKey(value);

    /// <summary>
    /// Returns an indication whether a constant with a specified name exists in a enumeration of type <typeparamref name="E"/>.
    /// </summary>
    /// <param name="name">The name of a constant in <typeparamref name="E"/>.</param>
    /// <returns><see langword="true"/> if a constant in <typeparamref name="E"/> has a name equal to <paramref name="name"/>; otherwise, <see langword="false"/>.</returns>
    public static bool IsDefined(string name) => mapping.ContainsKey(name);

    /// <summary>
    /// Gets enum member by its value.
    /// </summary>
    /// <param name="value">The enum value.</param>
    /// <returns>The enum member.</returns>
    public static Enum<TEnum> GetMember(TEnum value) => mapping.TryGetValue(value, out var result) ? result : new Enum<TEnum>(value, null);

    /// <summary>
    /// Attempts to retrieve enum member which constant value is equal to the given value.
    /// </summary>
    /// <param name="value">Enum value.</param>
    /// <param name="member">Enum member which constant value is equal to <paramref name="value"/>.</param>
    /// <returns><see langword="true"/>, if there are member declared the given constant value exist; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetMember(TEnum value, out Enum<TEnum> member) => mapping.TryGetValue(value, out member);

    /// <summary>
    /// Attempts to retrieve enum member which name is equal to the given value.
    /// </summary>
    /// <param name="name">The name of a constant.</param>
    /// <param name="member">Enum member which name is equal to <paramref name="name"/>.</param>
    /// <returns><see langword="true"/>, if there are member declared the given constant value exist; otherwise, <see langword="false"/>.</returns>
    public static bool TryGetMember(string name, out Enum<TEnum> member)
    {
      if (Enum.TryParse<TEnum>(name, out var value))
      {
        member = new Enum<TEnum>(value, name);
        return true;
      }
      else
      {
        member = default;
        return false;
      }
    }

    /// <summary>
    /// Gets enum member by its case-sensitive name.
    /// </summary>
    /// <param name="name">The name of the enum value.</param>
    /// <returns>The enum member.</returns>
    /// <exception cref="KeyNotFoundException">Enum member with the requested name doesn't exist in enum.</exception>
    public static Enum<TEnum> GetMember(string name) => mapping[name];

    /// <summary>
    /// Gets declared enum members.
    /// </summary>
    public static IReadOnlyList<Enum<TEnum>> Members => mapping.Members;

    /// <summary>
    /// Gets the underlying type of the specified enumeration.
    /// </summary>
    public static Type UnderlyingType => Enum.GetUnderlyingType(typeof(TEnum));

    /// <summary>
    /// Gets code of the underlying primitive type.
    /// </summary>
    public static TypeCode UnderlyingTypeCode => Type.GetTypeCode(typeof(TEnum));

    private static readonly Mapping mapping = new Mapping(out MinValue, out MaxValue);

    /// <summary>
    /// Maximum enum value.
    /// </summary>
    public static readonly Enum<TEnum> MaxValue;

    /// <summary>
    /// Minimum enum value.
    /// </summary>
    public static readonly Enum<TEnum> MinValue;


    /// <summary>
    /// Converts typed enum wrapper into actual enum value.
    /// </summary>
    /// <param name="en">Enum wrapper to convert.</param>
    public static implicit operator TEnum(in Enum<TEnum> en) => en.Value;

    /// <summary>
    /// Compares this enum value with other.
    /// </summary>
    /// <param name="other">Other value to compare.</param>
    /// <returns>Comparison result.</returns>
    public int CompareTo(TEnum other) => Comparer<TEnum>.Default.Compare(Value, other);

    /// <summary>
    /// Determines whether this value equals to the other enum value.
    /// </summary>
    /// <param name="other">Other value to compare.</param>
    /// <returns>Equality check result.</returns>
    public bool Equals(TEnum other) => Equals(Value, other);


    /// <summary>
    /// Returns textual representation of the enum value.
    /// </summary>
    /// <returns>The textual representation of the enum value.</returns>
    public override string ToString() => Enum.GetName(typeof(TEnum), Value);

    string IFormattable.ToString(string format, IFormatProvider provider) => ToString<TEnum>(Value, format, provider);

    /// <summary>
    /// Determines whether two enum members are equal.
    /// </summary>
    /// <param name="first">The first member to compare.</param>
    /// <param name="second">The second member to compare.</param>
    /// <returns><see langword="true"/> if this enum member is the same as other; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(Enum<TEnum> first, Enum<TEnum> second) => first.Equals(second);

    /// <summary>
    /// Determines whether two enum members are not equal.
    /// </summary>
    /// <param name="first">The first member to compare.</param>
    /// <param name="second">The second member to compare.</param>
    /// <returns><see langword="true"/> if this enum member is not the same as other; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(Enum<TEnum> first, Enum<TEnum> second) => !first.Equals(second);

    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue(NameSerData, name, typeof(string));
      info.AddValue(ValueSerData, Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ToString<T>(T value, IFormatProvider provider = null) where T : struct, IConvertible => value.ToString(provider);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ToString<T>(T value, string format, IFormatProvider provider = null) where T : struct, IFormattable => value.ToString(format, provider);
  }


}
