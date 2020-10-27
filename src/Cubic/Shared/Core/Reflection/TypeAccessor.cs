using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Reflection
{
  public class TypeAccessor
  {
    private static readonly Hashtable publicAccessors = new Hashtable();

    private Dictionary<string, PropertySpec> _getter;

    private Dictionary<string, MemberSetter> _setter; 

    private List<Member> _members;

    private Type _type;

    internal TypeAccessor(Type type)
    {
      _type = type;
      _getter = new Dictionary<string, PropertySpec>();
      _setter = new Dictionary<string, MemberSetter>();

      foreach (var member in GetMembers())
      {
        if (member.CanRead)
        {
          _getter[member.Name] = FastPropertyAccessor.CreatePropertySpec(member.Name);
        }
        if (member.CanWrite)
        {
          _setter[member.Name] = CreateSetter(((PropertyInfo) member.MemberInfo).GetSetMethod());
        }
      }
    }

    public static TypeAccessor Create<T>()
    {
      return TypeAccessor.Create(typeof(T));
    }

    public static TypeAccessor Create(Type type)
    {
      Guard.ArgumentNull(type, nameof(type));

      if (publicAccessors.ContainsKey(type))
      {
        return (TypeAccessor)publicAccessors[type];
      }

      lock (publicAccessors)
      {
        if (publicAccessors.ContainsKey(type))
        {
          return (TypeAccessor)publicAccessors[type];
        }

        publicAccessors[type] = new TypeAccessor(type);
        return (TypeAccessor)publicAccessors[type];
      }
    }

    public virtual IList<Member> GetMembers()
    {
      if (_members != null) return _members;

      _members = _type.GetPublicProperties().Select(p => new Member(p)).ToList();

      return _members;
    }

    private MemberSetter CreateSetter(MethodInfo setterMethodInfo)
    {
      //return (Action<object>) Delegate.CreateDelegate(typeof (Action<object>), setterMethodInfo);
      return (target, value) => setterMethodInfo.Invoke(target, new[] {value});
    }

    public object GetValue(object instance, string member)
    {
      if (!_getter.ContainsKey(member)) throw new InvalidOperationException("Member has no Getter");
      return _getter[member].Fetch(instance);
    }

    public void SetValue(object instance, string member, object value)
    {
      //_setter[member].Invoke(instance, value);

      if(!_setter.ContainsKey(member)) throw new InvalidOperationException("Member has no Setter");
      _setter[member].Invoke(instance, value);
    }
  }

  public delegate void MemberSetter(object instance, object value);
}