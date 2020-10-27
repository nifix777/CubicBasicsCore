using System;
using System.Collections.Generic;

namespace Cubic.Core
{
  public struct ValueTuple<TKey, TValue> : IEquatable<ValueTuple<TKey , TValue>>
  {

    public ValueTuple(TKey key, TValue value) : this()
    {
      Key = key;
      Value = value;
    }

    public ValueTuple(KeyValuePair<TKey, TValue> keyValuePair ) : this(keyValuePair.Key, keyValuePair.Value)
    {
      
    }
    public TKey Key { get; }

    public TValue Value { get; }

    public KeyValuePair<TKey, TValue> ToKeyValuePair()
    {
      return new KeyValuePair<TKey, TValue>(Key, Value);
    }

    public override int GetHashCode()
    {
      // Real implementation is a bit more complicated. Not a simple XOR
      return EqualityComparer<TKey>.Default.GetHashCode( Key ) ^
          EqualityComparer<TValue>.Default.GetHashCode( Value );
    }

    public bool Equals( ValueTuple<TKey , TValue> other )
    {
      return (Key != null && EqualityComparer<TKey>.Default.Equals(Key, other.Key)) &&
             (Value != null && EqualityComparer<TValue>.Default.Equals(Value, other.Value));

      //return ( Key != null && Key.Equals( other.Key ) ) &&
      //        ( Value != null && Value.Equals( other.Value ) );
    }

    public override bool Equals( object obj )
    {
      return obj is ValueTuple<TKey , TValue> &&
          Equals( ( ValueTuple<TKey , TValue> ) obj );
    }
  }
}