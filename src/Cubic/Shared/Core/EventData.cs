using System;
using System.Dynamic;

namespace Cubic.Core
{
  public class EventData<T>
  {
    private readonly T _data;

    public EventData(T data)
    {
      _data = data;
    }

    public T Data => _data;

    public static EventData<T> Empty { get; } = new EventData<T>(default(T));

    
  }
}