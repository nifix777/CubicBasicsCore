using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Cubic.Core.Reflection;

namespace Cubic.Core.Data
{
  public class ObjectReader : DbDataReader
  {
    private IEnumerator _enumerator;

    private readonly string[] _memberNames;

    private readonly Type[] _effectiveTypes;

    private readonly BitArray _allowNull;

    private TypeAccessor _accessor;

    public static ObjectReader Create<T>(IEnumerable<T> source, params string[] properties)
    {
      return new ObjectReader(typeof(T), source, properties);
    }

    public ObjectReader(Type type, IEnumerable soruce, string[] memberNames) : this(type, soruce.GetEnumerator(), memberNames)
    {

    }

    public ObjectReader(Type type, IEnumerator enumerator, params string[] properties)
    {
      _enumerator = enumerator;
      _memberNames = properties;

      _allowNull = new BitArray(properties.Length);
      _effectiveTypes = new Type[properties.Length];

      _accessor = TypeAccessor.Create(type);

      //for (int i = 0; i < properties.Length; i++)
      //{
      //  Type propertyType = null;
      //  bool allowNUll = true;
      //  string currentProperty = properties[i];

      //}

      foreach (var member in _accessor.GetMembers())
      {
        Type propertyType = null;
        bool allowNUll = true;
        var index = Array.IndexOf(properties, member.Name);

        if (index >= 0)
        {
          propertyType = member.Type;
          allowNUll = !propertyType.IsValueType;

          _allowNull[index] = allowNUll;
          _effectiveTypes[index] = propertyType;
        }

      }

    }


    public override string GetName(int ordinal)
    {
      return _memberNames[ordinal];
    }

    public override int GetValues(object[] values)
    {
      for (int i = 0; i < _memberNames.Length; i++)
      {
        values[i] = this.GetValue(i);
      }

      return _memberNames.Length;
    }

    public override bool IsDBNull(int ordinal)
    {
      return _allowNull[ordinal];
    }

    public override int FieldCount => _memberNames.Length;

    public override object this[int ordinal]
    {
      get { return _accessor.GetValue(_enumerator.Current, GetName(ordinal)); }
    }

    public override object this[string name]
    {
      get { return _accessor.GetValue(_enumerator.Current, name); }
    }

    public override bool HasRows => _memberNames.Length >= 1;
    public override bool IsClosed => false;
    public override int RecordsAffected => _memberNames.Length;

    public override bool NextResult()
    {
      return false;
    }

    public override bool Read()
    {
      return _enumerator.MoveNext();
    }

    public override int Depth => 0;

    public override int GetOrdinal(string name)
    {
      return Array.IndexOf(_memberNames, name);
    }

    public override bool GetBoolean(int ordinal)
    {
      return _accessor.GetValue(_enumerator.Current, GetName(ordinal)).ToBool();
    }

    public override byte GetByte(int ordinal)
    {
      return _accessor.GetValue(_enumerator.Current, GetName(ordinal)).ToByte();
    }

    public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
    {
      throw new NotImplementedException();
    }

    public override char GetChar(int ordinal)
    {
      return _accessor.GetValue(_enumerator.Current, GetName(ordinal)).ToChar();
    }

    public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
    {
      throw new NotImplementedException();
    }

    public override Guid GetGuid(int ordinal)
    {
      return _accessor.GetValue(_enumerator.Current, GetName(ordinal)).ToGuid();
    }

    public override short GetInt16(int ordinal)
    {
      return _accessor.GetValue(_enumerator.Current, GetName(ordinal)).ToInt16();
    }

    public override int GetInt32(int ordinal)
    {
      return _accessor.GetValue(_enumerator.Current, GetName(ordinal)).ToInt32();
    }

    public override long GetInt64(int ordinal)
    {
      return _accessor.GetValue(_enumerator.Current, GetName(ordinal)).ToInt64();
    }

    public override DateTime GetDateTime(int ordinal)
    {
      return _accessor.GetValue(_enumerator.Current, GetName(ordinal)).ToDateTime();
    }

    public override string GetString(int ordinal)
    {
      return _accessor.GetValue(_enumerator.Current, GetName(ordinal)).ToString();
    }

    public override decimal GetDecimal(int ordinal)
    {
      return _accessor.GetValue(_enumerator.Current, GetName(ordinal)).ToDecimal();
    }

    public override double GetDouble(int ordinal)
    {
      return _accessor.GetValue(_enumerator.Current, GetName(ordinal)).ToDouble();
    }

    public override float GetFloat(int ordinal)
    {
      return _accessor.GetValue(_enumerator.Current, GetName(ordinal)).ToFloat();
    }

    public override string GetDataTypeName(int ordinal)
    {
      return GetFieldType(ordinal).FullName;
    }

    public override Type GetFieldType(int ordinal)
    {
      return _effectiveTypes[ordinal];
    }

    public override object GetValue(int ordinal)
    {
      return _accessor.GetValue(_enumerator.Current, GetName(ordinal));
    }

    public override IEnumerator GetEnumerator()
    {
      return _enumerator;
    }
  }


}