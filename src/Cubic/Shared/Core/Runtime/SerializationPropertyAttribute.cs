using System;
using System.Reflection;

namespace Cubic.Core.Runtime
{
  [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
  public sealed class SerializationPropertyAttribute : Attribute
  {

    // This is a positional argument
    public SerializationPropertyAttribute(string name, Type type)
    {
      this.Name = name;
      Type = type;
    }
    public string Name { get; private set; }

    // This is a named argument
    public Type Type { get; private set; }

    public PropertyInfo Property { get; set; }
  }
}