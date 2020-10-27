using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Cubic.Core.Collections
{
  [Serializable]
  public sealed class ReadOnlyArray<T> : System.Collections.ICollection, ICollection<T>, IReadOnlyCollection<T>, IReadOnlyList<T>
  {
    private T[] _items;

    internal ReadOnlyArray(T[] items)
    {
      _items = items;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      Array.Copy(_items, 0, array, arrayIndex, _items.Length);
    }

    void System.Collections.ICollection.CopyTo(Array array, int index)
    {
      Array.Copy(_items, 0, array, index, _items.Length);
    }


    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return new Enumerator<T>(_items);
    }

    public System.Collections.IEnumerator GetEnumerator()
    {
      return new Enumerator<T>(_items);
    }

    bool System.Collections.ICollection.IsSynchronized
    {
      get { return false; }
    }

    object System.Collections.ICollection.SyncRoot
    {
      get { return _items; }
    }

    bool ICollection<T>.IsReadOnly
    {
      get { return true; }
    }

    void ICollection<T>.Add(T value)
    {
      throw new NotSupportedException();
    }

    void ICollection<T>.Clear()
    {
      throw new NotSupportedException();
    }

    bool ICollection<T>.Contains(T value)
    {
      return Array.IndexOf(_items, value) >= 0;
    }

    bool ICollection<T>.Remove(T value)
    {
      throw new NotSupportedException();
    }

    public int Count
    {
      get { return _items.Length; }
    }

    public T this[int index] => _items[index];

    [Serializable]
    public struct Enumerator<K> : IEnumerator<K>
    { // based on List<T>.Enumerator
      private K[] _items;
      private int _index;

      internal Enumerator(K[] items)
      {
        _items = items;
        _index = -1;
      }

      public void Dispose()
      {
      }

      public bool MoveNext()
      {
        return (++_index < _items.Length);
      }

      public K Current
      {
        get
        {
          return _items[_index];
        }
      }

      public void Reset()
      {
        throw new NotImplementedException();
      }

      object IEnumerator.Current => Current;
    }
  }
}