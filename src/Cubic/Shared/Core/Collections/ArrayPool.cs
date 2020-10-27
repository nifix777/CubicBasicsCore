using System;
using System.Collections.Generic;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Collections
{
  public class ArrayPool<TBuffer> : IDisposable, IArrayPool<TBuffer>
  {
    private Stack<TBuffer[]> _instancePool = new Stack<TBuffer[]>();

    private object _sync = new object();

    private Func<int,TBuffer[]> _factory;

    private readonly int _defaultBufferSize = 0;


    public ArrayPool( Func<int ,TBuffer[]> facortyMethod , bool directInitialse = true, int defaultBufferSize = 1024)
    {
      Guard.ArgumentNull( facortyMethod, nameof( facortyMethod ));

      _factory = facortyMethod;

      _defaultBufferSize = defaultBufferSize;

      if ( directInitialse )
      {
        Init(); 
      }

    }

    public ArrayPool(bool directInitialse = true, int defaultBufferSize = 1024) : this(FacortyMethod, directInitialse, defaultBufferSize)
    {
      
    }

    private static TBuffer[] FacortyMethod(int i)
    {
      return new TBuffer[i];
    }


    public EventHandler<Array> OnFree { get; set; }

    private void Init()
    {
      lock (_sync)
      {
        _instancePool.Push( _factory(_defaultBufferSize));
      }

    }

    public bool HasFreeObjectes
    {
      get
      {
        lock ( _sync )
        {
          return _instancePool.Count > 0;
        }
      }
    }


    public TBuffer[] Rent(int lenght)
    {
      var bufferLenght = lenght == 0 ? _defaultBufferSize : lenght;
      TBuffer[] rent;
      lock ( _sync )
      {
        if ( _instancePool.Count == 0 )
        {
          rent = _factory( bufferLenght );
        }
        else
        {
          //foreach (var buffer in _instancePool)
          //{
          //  if ( bufferLenght <= buffer.Length)
          //  {

          //  }
          //}

          var tmp = _instancePool.Peek();

          if (bufferLenght <= tmp.Length)
          {
            rent = _instancePool.Pop();
          }

          rent = _factory( bufferLenght );
        }
      }
      return rent;
    }

    // Only clears the array if T is a reference type.
    public void Free( TBuffer[] buffer  )
    {
      this.Free(buffer, typeof(TBuffer).IsByRef);
    }

    // Clears the array if clearBuffer is true for both value and reference types.
    public void Free(TBuffer[] buffer, bool clearBuffer)
    {
      lock (_sync)
      {
        // Claer Array
        if (clearBuffer)
        {
          Array.Clear(buffer, 0, buffer.Length);
        }

        OnFree?.Invoke(this, buffer);

        _instancePool.Push(buffer);
      }
    }

    public void Dispose()
    {
      lock (_sync)
      {
        _instancePool?.Clear();
        _instancePool = null;
      }
    }

    public static ArrayPool<TBuffer> Shared = new ArrayPool<TBuffer>();
  }
}