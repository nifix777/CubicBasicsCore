using System;
using System.Collections.Generic;

namespace Cubic.Core.Collections
{
  public class MultiKeyDictionary<TKey1, TKey2, TItem> : Dictionary<MultiKey<TKey1, TKey2>, TItem>
  {
    
  }

  public class MultiKeyDictionary<TKey1, TKey2, TKey3, TItem> : Dictionary<MultiKey<TKey1, TKey2, TKey3>, TItem>
  {

  }

  public struct MultiKey<TKey1, TKey2> : IEquatable<MultiKey<TKey1, TKey2>>
  {
    public readonly TKey1 Key1;

    public readonly TKey2 Key2;

    public MultiKey(TKey1 key1, TKey2 key2)
    {
      Key1 = key1;
      Key2 = key2;
    }

    public bool Equals(MultiKey<TKey1, TKey2> other)
    {
      return EqualityComparer<TKey1>.Default.Equals(Key1, other.Key1) && EqualityComparer<TKey2>.Default.Equals(Key2, other.Key2);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      return obj is MultiKey<TKey1, TKey2> && Equals((MultiKey<TKey1, TKey2>) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return (EqualityComparer<TKey1>.Default.GetHashCode(Key1) * 397) ^ EqualityComparer<TKey2>.Default.GetHashCode(Key2);
      }
    }
  }

  public struct MultiKey<TKey1, TKey2, TKey3> : IEquatable<MultiKey<TKey1, TKey2, TKey3>>
  {
    public readonly TKey1 Key1;

    public readonly TKey2 Key2;

    public readonly TKey3 Key3;

    public MultiKey(TKey1 key1, TKey2 key2, TKey3 key3)
    {
      Key1 = key1;
      Key2 = key2;
      Key3 = key3;
    }

    public bool Equals(MultiKey<TKey1, TKey2, TKey3> other)
    {
      return EqualityComparer<TKey1>.Default.Equals(Key1, other.Key1) && EqualityComparer<TKey2>.Default.Equals(Key2, other.Key2) && EqualityComparer<TKey3>.Default.Equals(Key3, other.Key3);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      return obj is MultiKey<TKey1, TKey2, TKey3> && Equals((MultiKey<TKey1, TKey2, TKey3>) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = EqualityComparer<TKey1>.Default.GetHashCode(Key1);
        hashCode = (hashCode * 397) ^ EqualityComparer<TKey2>.Default.GetHashCode(Key2);
        hashCode = (hashCode * 397) ^ EqualityComparer<TKey3>.Default.GetHashCode(Key3);
        return hashCode;
      }
    }
  }
}