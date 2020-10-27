using System;
using System.Reflection;

namespace Cubic.Core.Reflection
{
  public sealed class Member
  {
    private readonly MemberInfo _memberInfo;

    internal Member(MemberInfo memberInfo)
    {
      _memberInfo = memberInfo;
    }

    internal MemberInfo MemberInfo => _memberInfo;
    public string Name => _memberInfo.Name;

    public Type Type
    {
      get
      {
        if (_memberInfo is FieldInfo) return ((FieldInfo)_memberInfo).FieldType;
        if (_memberInfo is PropertyInfo) return ((PropertyInfo)_memberInfo).PropertyType;
        throw new NotSupportedException(_memberInfo.GetType().FullName);

      }
    }

    public Attribute GetAttribute(Type attributeType, bool inherit)
    {
      return Attribute.GetCustomAttribute(_memberInfo, attributeType, inherit);
    }

    public bool CanWrite
    {
      get
      {
        switch (_memberInfo.MemberType)
        {
          case MemberTypes.Property: return ((PropertyInfo)_memberInfo).CanWrite;
          default: throw new NotSupportedException(_memberInfo.MemberType.ToString());
        }
      }
    }

    public bool CanRead
    {
      get
      {
        switch (_memberInfo.MemberType)
        {
          case MemberTypes.Property: return ((PropertyInfo)_memberInfo).CanRead;
          default: throw new NotSupportedException(_memberInfo.MemberType.ToString());
        }
      }
    }
  }
}