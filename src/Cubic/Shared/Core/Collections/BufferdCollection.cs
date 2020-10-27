using System;
using System.Collections;
using System.Collections.Generic;
using Cubic.Core.Observer;

namespace Cubic.Core.Collections
{
  public class BufferdCollection<T> : Observerable<T>, ICollection<T>
  {
    private readonly int _bufferSize;
    private readonly Stack<T> _buffer;

    public BufferdCollection(int bufferSize)
    {
      _bufferSize = bufferSize;
      _buffer = new Stack<T>(_bufferSize);
    }

    private void BufferInternal(T item)
    {
      if (_buffer.Count != _bufferSize)
      {
        _buffer.Push(item);
      }
      else
      {
        this.Next(_buffer.Pop());
      }
      
    }

    public IEnumerator<T> GetEnumerator()
    {
      return _buffer.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable) _buffer).GetEnumerator();
    }

    public void Add(T item)
    {
      this.BufferInternal(item);
    }

    public void Clear()
    {
      _buffer.Clear();
    }

    public bool Contains(T item)
    {
      return _buffer.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    public bool Remove(T item)
    {
      throw new NotImplementedException();
    }

    public int Count => _buffer.Count;
    public bool IsReadOnly => false;
  }
}