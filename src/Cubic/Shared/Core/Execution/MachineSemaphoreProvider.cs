using Cubic.Core.Text;
using Cubic.Core.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cubic.Core.Execution
{
  public class MachineSemaphoreProvider : Semaphore.ISemaphoreProvider
  {
    private HashSet<MachineSemaphore> _semaphores;

    public MachineSemaphoreProvider()
    {
      _semaphores = new HashSet<MachineSemaphore>();

      LockStation = Environment.MachineName;
      LockUser = Environment.UserName;
    }
    public string LockStation { get; }
    public string LockUser { get; }

    public Task ReleaseLock(string name, string atom, Semaphore.SemaphoreLockMode mode)
    {
      var key = atom.IsNullOrEmpty() ? "{name}.{atom}" : name;

      var existing = Enumerable.ToArray(GetExisting(key));

      if (existing.Any())
      {
        foreach (var item in existing)
        {
          if (item.Mode == mode)
          {
            item.Semaphore.Dispose();

            lock (_semaphores)
            {
              _semaphores.Remove(item);
            }
          }
        }
      }

      return TaskEx.Completed;
    }

    public Task<bool> SetLock(string name, string atom, Semaphore.SemaphoreLockMode mode)
    {
      var key = atom.IsNullOrEmpty() ? "{name}.{atom}" : name;

      var existing = Enumerable.ToArray(GetExisting(key));

      bool result = false;

      if (existing.Any())
      {
        foreach (var item in existing)
        {
          if (CheckLockMode(item, mode))
          {
            lock (_semaphores)
            {
              var newItem = new MachineSemaphore(key, mode);
              newItem.Semaphore.WaitOne();
              _semaphores.Add(newItem);
              result = true;
              break;
            }
          }
        }
      }
      else
      {
        result = true;
      }

      return Task.FromResult(result);

    }

    private IEnumerable<MachineSemaphore> GetExisting(string key)
    {
      lock (_semaphores)
      {
        var semaphore = new MachineSemaphore(key, Semaphore.SemaphoreLockMode.Exclusive);

        if (semaphore.IsNew)
        {
          semaphore.Dispose();
          return Enumerable.Empty<MachineSemaphore>();
        }

        return semaphore.Yield();
      }
    }

    private bool CheckLockMode(MachineSemaphore existingSemaphore, Semaphore.SemaphoreLockMode lockMode)
    {
      if (existingSemaphore.Mode == Semaphore.SemaphoreLockMode.Exclusive | existingSemaphore.Mode == lockMode) return false;

      return true;
    }

    public void Dispose()
    {
      lock(_semaphores)
      {
        foreach (var sem in _semaphores)
        {
          sem?.Dispose();
        }
        _semaphores.Clear();
      }
    }

    internal class MachineSemaphore : IDisposable
    {
      public MachineSemaphore(string key, Semaphore.SemaphoreLockMode mode)
      {
        Key = key;
        Mode = mode;

        Semaphore = new System.Threading.Semaphore(0,1, Key, out var createdNew);
        IsNew = createdNew;
      }
      internal string Key { get; }

      internal bool IsNew { get; }

      internal Semaphore.SemaphoreLockMode Mode { get; set; }

      internal System.Threading.Semaphore Semaphore { get; }

      public void Dispose()
      {
        Semaphore.Dispose();
      }
    }
  }
}