using Cubic.Core.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Cubic.Core.Text
{

  // 

  /// <summary>
  /// 
  /// </summary>
  /// <seealso cref="Utf8String" />
  /// <seealso cref="System.ICloneable" />
  /// <seealso cref="byte" />
  /// <seealso cref="System.IConvertible" />
  /// <remarks>Experimental!!!!!!</remarks>
  public struct Utf8String : IEquatable<Utf8String>, ICloneable, IEnumerable<byte>, IConvertible
  {
    private readonly byte[] _bytes;

    private static readonly Encoding _utf8 = Encoding.UTF8;

    public static Utf8String Empty = new Utf8String(string.Empty);

    public byte this[int index]
    {
      get
      {
        if (index < 0 || index > _bytes.Length) throw new ArgumentOutOfRangeException(nameof(index));

        return _bytes[index];

      }
      set
      {
        if (index < 0 || index > _bytes.Length) throw new ArgumentOutOfRangeException(nameof(index));
        _bytes[index] = value;
      }

    }

    public int Length => _bytes.Length;

    public bool IsEmpty => _bytes.Length == 0;

    public IReadOnlyCollection<byte> Buffer => new System.Collections.ObjectModel.ReadOnlyCollection<byte>(_bytes);

    internal byte[] UnderlyingArray => _bytes;

    public Utf8String(string utf16String)
    {
      _bytes = Encoding.UTF8.GetBytes(utf16String);
    }

    public Utf8String(IReadOnlyCollection<byte> readOnlyBytes)
    {
      _bytes = readOnlyBytes.ToArray();
    }

    private Utf8String(byte[] nocopyBytes)
    {
      _bytes = nocopyBytes;
    }

    public bool Equals(Utf8String other)
    {
      if (object.Equals(this.Buffer, other.Buffer)) return true;

      if (_bytes.Length != other.Length) return false;

      return Enumerable.SequenceEqual(_bytes, other);
    }

    public override string ToString()
    {
      return Encoding.UTF8.GetString(_bytes);
    }

    internal byte[] BufferRaw => _bytes;

    public object Clone()
    {
      return new Utf8String(_bytes.ToArray());
    }

    public IEnumerator<byte> GetEnumerator()
    {
      return new Utf8Enumerator(_bytes);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #region Operators

    public static implicit operator Utf8String(string utf8String)
    {
      return new Utf8String(_utf8.GetBytes(utf8String));
    }

    public static implicit operator string (Utf8String utf8)
    {
      return _utf8.GetString(utf8.BufferRaw);
    }

    public static implicit operator char[](Utf8String utf8)
    {
      return _utf8.GetChars(utf8.BufferRaw);
    }

    #endregion


    #region IConvertible
    public TypeCode GetTypeCode()
    {
      return TypeCode.Object;
    }

    public bool ToBoolean(IFormatProvider provider)
    {
      return Boolean.Parse(this.ToString());
    }

    public char ToChar(IFormatProvider provider)
    {
      return char.Parse(ToString());
    }

    public sbyte ToSByte(IFormatProvider provider)
    {
      return sbyte.Parse(ToString());
    }

    public byte ToByte(IFormatProvider provider)
    {
      if (!IsEmpty) return _bytes[0];

      return default(byte);
    }

    public short ToInt16(IFormatProvider provider)
    {
      return short.Parse(ToString());
    }

    public ushort ToUInt16(IFormatProvider provider)
    {
      return ushort.Parse(ToString());
    }

    public int ToInt32(IFormatProvider provider)
    {
      return int.Parse(ToString());
    }

    public uint ToUInt32(IFormatProvider provider)
    {
      return uint.Parse(ToString());
    }

    public long ToInt64(IFormatProvider provider)
    {
      return long.Parse(ToString());
    }

    public ulong ToUInt64(IFormatProvider provider)
    {
      return ulong.Parse(ToString());
    }

    public float ToSingle(IFormatProvider provider)
    {
      return float.Parse(ToString());
    }

    public double ToDouble(IFormatProvider provider)
    {
      return double.Parse(ToString());
    }

    public decimal ToDecimal(IFormatProvider provider)
    {
      return decimal.Parse(ToString());
    }

    public DateTime ToDateTime(IFormatProvider provider)
    {
      return DateTime.Parse(ToString());
    }

    public string ToString(IFormatProvider provider)
    {
      return ToString();
    }

    public object ToType(Type conversionType, IFormatProvider provider)
    {
      if (conversionType == typeof(Utf8String)) return this;

      if (conversionType == typeof(byte[])) return _bytes.ToArray();

      if (conversionType == typeof(string)) return ToString();



      throw new InvalidCastException($"Converting type \"{typeof(Utf8String)}\" to type \"{conversionType.Name}\" is not supported.");
    }

    #endregion




    private struct Utf8Enumerator : IEnumerator<byte>
    {
      private byte[] _source;

      private int _index;

      public Utf8Enumerator(byte[] bytes)
      {
        _source = bytes;
        _index = 0;
      }

      public void Dispose()
      {
        Reset();
      }

      public bool MoveNext()
      {
        if (_index <= _source.Length)
        {
          _index++;
          return true;
        }

        return false;

        //_index++;
        //return true;


      }

      public void Reset()
      {
        _index = 0;
      }

      public byte Current => _source[_index];

      object IEnumerator.Current => Current;
    }
  }

}