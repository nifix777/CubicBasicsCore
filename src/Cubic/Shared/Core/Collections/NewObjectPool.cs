using System;
using System.Collections.Generic;
using System.Threading;

namespace Cubic.Core.Collections
{
  public class NewObjectPool<T> where T : class
  {
    private Stack<T> _instancePool = new Stack<T>();

    private readonly Func<T> _factory;
    private readonly Action<T> _release;

    public NewObjectPool(Func<T> factory, Action<T> release)
    {
      _factory = factory;
      _release = release;
    }

    public T Rent()
    {
      var pool = _instancePool;
      T item;
      int counter = pool.Count;

      if(counter > 0)
      {
        if(Interlocked.CompareExchange(ref counter, 0, counter) >= counter)
        {
          item = pool.Pop();
          if (item == Interlocked.CompareExchange(ref item, null, item))
          {
            return item;
          }
        }

      }



      return CreateInstance();
    }

    public void Return(T item)
    {
      var releaser = _release;

      releaser?.Invoke(item);

      var pool = _instancePool;
      pool.Push(item);
    }

    private T CreateInstance()
    {
      return _factory();
    }
  }
}