using System.Collections.Generic;
using System.Linq;
using Cubic.Core.Collections;
using Cubic.Core.Threading;

namespace Cubic.Core.Data
{
  public class ParameterBag : IParameterBag
  {
    private Lock _lock;

    private IDictionary<string, object> _objects = new Dictionary<string, object>();


    public ParameterBag()
    {
      _lock = new Lock();
    }

    public IEnumerable<object> Values => _objects.Values.ToObjectArray();
    public IEnumerable<string> Keys => _objects.Keys;
    public void Set(string key, object value)
    {
      using (var write = _lock.LockWrite())
      {
        _objects[key] = value;
      }
    }

    public object Get(string key)
    {
      using (var read = _lock.LockRead())
      {
        if (_objects.ContainsKey(key))
        {
          return _objects[key];
        }

        return null;
      }

    }

    public object this[string key]
    {
      get { return Get(key); }
      set { Set(key, value); }
    }

    public T Get<T>(string key)
    {
      return this.Get(key).OrDefault<T>(typeof(T));
    }

    public bool Remove(string key)
    {
      using (var write = _lock.LockWrite())
      {
        return _objects.Remove(key);
      }
    }
  }

  interface IParameterBag
  {

    IEnumerable<object> Values { get; }

    IEnumerable<string> Keys { get; }
    void Set(string key, object value);
    object Get(string key);

    object this[string key] { get; set; }

    T Get<T>(string key);

    bool Remove(string key);
  }
}