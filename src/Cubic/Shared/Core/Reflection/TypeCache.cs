using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Reflection
{
  public class TypeCache
  {
    private ConcurrentDictionary<string, Type> _cache = new ConcurrentDictionary<string, Type>();

    public Type GetType(string typeName)
    {
      return _cache.GetOrAdd(typeName, (s) => Type.GetType(typeName));
    }

    public Type GetType(string typeName, Assembly assembly)
    {
      return _cache.GetOrAdd(typeName, (s) => assembly.GetType(typeName));
    }
  }
}
