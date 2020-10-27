using System;

namespace Cubic.Core
{
  public class SingletonBase<T> where T : class, new()
  {
    private static volatile T _instance;

    private static object _lock = new object();

    /// <summary>
    /// Initializes a new instance of the <see cref="SingletonBase{T}"/> class.
    /// </summary>
    public SingletonBase()
    {
      _lock = new object();
    }

    /// <summary>
    /// Gets the current.
    /// </summary>
    /// <value>
    /// The current.
    /// </value>
    public static T Current
    {

      get
      {
        if (_instance == null)
        {
          lock (_lock)
          {
            if (_instance == null)
            {
              _instance = new T();
            }
          }
        }

        return _instance;
      }

    }

    /// <summary>
    /// Sets the instance.
    /// </summary>
    /// <param name="instance">The instance.</param>
    protected void SetInstance(T instance)
    {
      lock (_lock)
      {
        _instance = instance;
      }
    }
  }

  public class SingletonFactory<T> where T : class
  {
    private static volatile T _instance;

    private static Func<T> _factory;

    private static object _lock = new object();

    /// <summary>
    /// Initializes a new instance of the <see cref="SingletonBase{T}"/> class.
    /// </summary>
    public SingletonFactory(Func<T> factory)
    {
      _lock = new object();
      _factory = factory;
    }

    /// <summary>
    /// Gets the current.
    /// </summary>
    /// <value>
    /// The current.
    /// </value>
    public static T Current
    {

      get
      {
        if (_instance == null)
        {
          lock (_lock)
          {
            if (_instance == null)
            {
              _instance = _factory();
            }
          }
        }

        return _instance;
      }

    }
  }
}