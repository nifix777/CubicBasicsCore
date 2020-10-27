using System;
using System.Reflection;

namespace Cubic.Core.Runtime
{
  [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
  public sealed class SerializationFieldAttribute : Attribute
  {

    // This is a positional argument
    public SerializationFieldAttribute(string name, Type type)
    {
      this.Name = name;
      Type = type;
    }
    public string Name { get; private set; }

    // This is a named argument
    public Type Type { get; private set; }

    public FieldInfo Field { get; set; }
  }
}