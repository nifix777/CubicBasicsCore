using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Cubic.Core.Runtime
{
  public abstract class SerializableBase : ISerializable
  {
    private List<SerializationPropertyAttribute> _properties;
    private List<SerializationFieldAttribute> _fields;
    protected SerializableBase()
    {
      BuildSeralizationnNfo(this.GetType());
    }

    protected SerializableBase(SerializationInfo info, StreamingContext context) : this()
    {
      foreach (var attribute in _properties)
      {
        attribute.Property.SetValue(this, info.GetValue(attribute.Name, attribute.Type));
      }

      foreach (var attribute in _fields)
      {
        attribute.Field.SetValue(this, info.GetValue(attribute.Name, attribute.Type));
      }
    }

    private void BuildSeralizationnNfo(Type type)
    {
      _properties = new List<SerializationPropertyAttribute>();
      foreach (var property in type.GetProperties().Where(pi => pi.CustomAttributes.OfType<SerializationPropertyAttribute>().Any()))
      {
        var attrib = property.GetCustomAttribute<SerializationPropertyAttribute>();
        attrib.Property = property;
        _properties.Add(attrib);
      }

      _fields = new List<SerializationFieldAttribute>();

      foreach (var field in type.GetFields().Where(fi => fi.GetCustomAttributes<SerializationFieldAttribute>().Any()))
      {
        var attrib = field.GetCustomAttribute<SerializationFieldAttribute>();
        attrib.Field = field;
        _fields.Add(attrib);
      }
    }
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      foreach (var attribute in _properties)
      {
        info.AddValue(attribute.Name, attribute.Property.GetValue(this), attribute.Type);
      }

      foreach (var attribute in _fields)
      {
        info.AddValue(attribute.Name, attribute.Field.GetValue(this), attribute.Type);
      }
    }
  }
}