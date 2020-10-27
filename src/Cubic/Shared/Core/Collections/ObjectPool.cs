using System;
using System.Collections.Generic;
using System.Threading;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Collections
{
  public class ObjectPool<T> where T : class
  {
    private Stack<T> _instancePool = new Stack<T>();

    private object _sync = new object();

    private Func<T> _factory;

    private readonly int _initalPoolSize = 0;
    private readonly int maxSize;

    public ObjectPool(Func<T> facortyMethod, int initialSize = 0, int maxSize = 0 )
    {
      Guard.ArgumentNull(facortyMethod, nameof(facortyMethod));

      _factory = facortyMethod;

      _initalPoolSize = initialSize;
      this.maxSize = maxSize;
      Init();

    }

    public EventHandler<T> OnFree { get; set; }

    public int PooledObjectes
    {
      get
      {
        lock (_sync)
        {
          return _instancePool.Count;
        }
      }
    }

    private void Init()
    {
      if (_initalPoolSize > 0)
      {
        for (int i = 0; i < _initalPoolSize; i++)
        {
          _instancePool.Push(_factory());
        }
      }
    }

    public T Get()
    {
      if(maxSize != 0)
      {
        var size = GetSizeSafe();
        while (maxSize != 0 && size == maxSize)
        {
          Thread.Sleep(30);
          size = GetSizeSafe();
        }
      }


      lock ( _sync )
      {
        if ( _instancePool.Count == 0 )
        {
          return _factory();
        }
        else
        {
          return _instancePool.Pop();
        }
      }
    }

    public void Free( T instance )
    {
      lock ( _sync )
      {
        OnFree?.Invoke(this, instance);
        _instancePool.Push( instance );
      }
    }

    private int GetSizeSafe()
    {
      lock(_sync)
      {
        return _instancePool.Count;
      }
    }


  }
}