using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Threading
{
  /// <summary>Class to track a "busy" State with low allocations</summary>
  public static class BusyManager
  {
    public struct BusyLock : IDisposable
    {
      public static readonly BusyLock Failed = new BusyLock(null);

      private readonly List<object> _objectList;

      internal BusyLock(List<object> objectList)
      {
        _objectList = objectList;
      }

      public bool Success => _objectList != null;

      public void Dispose()
      {
        _objectList?.RemoveAt(_objectList.Count - 1);
      }
    }

    [ThreadStatic] private static List<object> _activeObjects;

    public static BusyLock Enter(object obj)
    {
      var activeObjects = _activeObjects ?? (_activeObjects = new List<object>());
      if (activeObjects.Any(t => t == obj))
      {
        return BusyLock.Failed;
      }
      activeObjects.Add(obj);
      return new BusyLock(activeObjects);
    }
  }
}
