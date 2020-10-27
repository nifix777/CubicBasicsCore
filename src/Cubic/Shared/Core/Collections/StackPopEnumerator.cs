using System;
using System.Collections;
using System.Collections.Generic;

namespace Cubic.Core.Collections
{
  public class StackPopEnumerator<T> : IEnumerator<T>
  {
    private Stack<T> _stack;

    private T _current;

    public StackPopEnumerator(Stack<T> stack)
    {
      _stack = stack;
    }

    public void Dispose()
    {
      
    }

    public bool MoveNext()
    {
      if (_stack.Count != 0)
      {
        _current = _stack.Pop();
        return true;
      }

      return false;
    }

    public void Reset()
    {
      throw new InvalidOperationException();
    }

    public T Current => _current;

    object IEnumerator.Current => Current;
  }
}