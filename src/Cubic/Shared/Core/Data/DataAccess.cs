using System;
using System.Collections;
using System.Data.Common;
using System.Runtime.Serialization;

namespace Cubic.Core.Data
{
  public class DataAccess : IDataAccess
  {
    public DataAccess()
    {
      AccessStringBuilder = new DbConnectionStringBuilder();
      AccessorType = AccessorType.INDIVIDUAL;
    }

    public DataAccess(AccessorType accessorType, string name) : this()
    {
      AccessorType = accessorType;
      AccessName = name;
    }

    public DataAccess(string accessConnectionString) : this()
    {
      AccessStringBuilder.ConnectionString = accessConnectionString;
    }

    private DbConnectionStringBuilder AccessStringBuilder { get; }

    public bool Contains(object key)
    {
      return ((IDictionary) AccessStringBuilder).Contains(key);
    }

    public void Add(object key, object value)
    {
      ((IDictionary) AccessStringBuilder).Add(key, value);
    }

    public void Clear()
    {
      AccessStringBuilder.Clear();
    }

    public IDictionaryEnumerator GetEnumerator()
    {
      return ((IDictionary) AccessStringBuilder).GetEnumerator();
    }

    public void Remove(object key)
    {
      ((IDictionary) AccessStringBuilder).Remove(key);
    }

    public object this[object key]
    {
      get => ((IDictionary) AccessStringBuilder)[key];
      set => ((IDictionary) AccessStringBuilder)[key] = value;
    }

    public ICollection Keys => AccessStringBuilder.Keys;

    public ICollection Values => AccessStringBuilder.Values;

    public bool IsReadOnly => AccessStringBuilder.IsReadOnly;

    public bool IsFixedSize => AccessStringBuilder.IsFixedSize;

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable) AccessStringBuilder).GetEnumerator();
    }

    public void CopyTo(Array array, int index)
    {
      ((ICollection) AccessStringBuilder).CopyTo(array, index);
    }

    public int Count => AccessStringBuilder.Count;

    public object SyncRoot => ((ICollection) AccessStringBuilder).SyncRoot;

    public bool IsSynchronized => ((ICollection) AccessStringBuilder).IsSynchronized;

    public virtual object Clone()
    {
      return new DataAccess(this.AccessConnectionString);
    }

    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue(Constants.CommonConnectionString, AccessConnectionString, typeof(string));
    }

    public AccessorType AccessorType
    {
      get => AccessStringBuilder[nameof(AccessorType)].ToString().ToEnum<AccessorType>();
      set => AccessStringBuilder[nameof(AccessorType)] = value;
    }

    public string AccessName
    {
      get => AccessStringBuilder[nameof(AccessName)].ToString();
      set => AccessStringBuilder[nameof(AccessName)] = value;
    }

    public string AccessConnectionString
    {
      get => AccessStringBuilder.ConnectionString;
      set => AccessStringBuilder.ConnectionString = value;
    }


    public bool Equals(IDataAccess other)
    {
      if (ReferenceEquals(other, null)) return false;
      if (ReferenceEquals(this, null)) return false;

      return this.AccessorType == other.AccessorType && this.AccessName == other.AccessName;
    }

    public override string ToString()
    {
      return AccessConnectionString;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((int) AccessorType * 397) ^ (AccessName != null ? AccessName.GetHashCode() : 0);
      }
    }
  }
}