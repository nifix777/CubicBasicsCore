using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Cubic.Core.Data
{
  public interface IDataAccess : IDictionary, IEnumerable, ICollection, ISerializable, ICloneable, IEquatable<IDataAccess>
  {
    AccessorType AccessorType { get; set; }

    string AccessName { get; set; }

    string AccessConnectionString { get; set; }
  }
}