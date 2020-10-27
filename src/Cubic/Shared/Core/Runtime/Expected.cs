using System;

namespace Cubic.Core.Runtime
{
  public class Expected<T> 
  {
    //private T? _value;
    private readonly T _value;

    private readonly bool _hasValueOrReference;

    private Exception _exception;

    public Expected(T expression)
    {
      _value = expression;

      if (expression != null)
      {
        _hasValueOrReference = true;
      }
    }
  
    public Expected(object expression)
    {
      if (typeof(T).IsValueType)
      {
        if (expression != null)
        {
          _hasValueOrReference = true;
          _value = (T)expression;
        }
      }
      else
      {
        _value = (T) expression;
        _hasValueOrReference = (expression != null);
      }
    }

    public Expected(Exception exception)
    {
      _hasValueOrReference = false;
      _exception = exception;
    }

    public bool HasValue => _hasValueOrReference;
    public T Value => _value;

    public Exception Error => _exception;

    public static Expected<TValue> FromNullable<TValue>(TValue? value) where TValue : struct
    {
      if (value.HasValue)
      {
        return new Expected<TValue>(value.Value);
      }
      
      return new Expected<TValue>(null);
    }
  }
}