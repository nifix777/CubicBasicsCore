using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Runtime
{
  /// <summary>
  /// Wrapper class for serializing a <see cref="T:System.Type" /> instance
  /// </summary>
  [Serializable]
  public struct TypeWrapper : IObjectReference
  {
    private string assemblyQualifiedName;

    private string fullName;

    /// <summary>
    /// Returns the AssemblyQualifiedName of the type
    /// </summary>        
    public string AssemblyQualifiedName
    {
      get
      {
        return this.assemblyQualifiedName;
      }
    }

    /// <summary>
    /// Returns the FullName of the type
    /// </summary>
    public string FullName
    {
      get
      {
        return this.fullName;
      }
    }

    /// <summary>
    /// Initializes a new <b>TypeWrapper</b>
    /// </summary>
    /// <param name="type">Type to serialize</param>
    public TypeWrapper(Type type)
    {
      if (type == null)
      {
        throw new ArgumentNullException("type", "A valid type must be specified");
      }
      this.fullName = type.FullName;
      this.assemblyQualifiedName = type.AssemblyQualifiedName;
    }

    internal Type GetResolvedType()
    {
      return System.Reflection.Assembly.Load(this.assemblyQualifiedName).GetType(this.fullName);
    }

    object System.Runtime.Serialization.IObjectReference.GetRealObject(StreamingContext context)
    {
      return this.GetResolvedType();
    }
  }
}
