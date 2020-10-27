using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cubic.Core.Text;
using Cubic.Core.Threading;

namespace Cubic.Core.Execution
{
  public class InProcessSemaphoreProvider : Semaphore.ISemaphoreProvider
  {
    private HashSet<InProcSemaphore> _semaphores; 

    public InProcessSemaphoreProvider()
    {
      _semaphores = new HashSet<InProcSemaphore>();

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
              var newItem = new InProcSemaphore(key, mode);
              newItem.Semaphore.Wait();
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

    private IEnumerable<InProcSemaphore> GetExisting(string key)
    {
      lock (_semaphores)
      {
        return _semaphores.Where(s => s.Key == key);
      }
    }

    private bool CheckLockMode(InProcSemaphore existingSemaphore, Semaphore.SemaphoreLockMode lockMode)
    {
      if (existingSemaphore.Mode == Semaphore.SemaphoreLockMode.Exclusive | existingSemaphore.Mode == lockMode) return false;

      return true;
    }

    public void Dispose()
    {
        lock (_semaphores)
        {
            foreach(var item in _semaphores)
            {
                    item?.Dispose();
            }
            _semaphores.Clear();
        }
    }

    internal class InProcSemaphore : IDisposable
    {
      public InProcSemaphore(string key, Semaphore.SemaphoreLockMode mode)
      {
        Key = key;
        Mode = mode;

        Semaphore = new SemaphoreSlim(0, 1);
      }
      internal string Key { get; }

      internal Semaphore.SemaphoreLockMode Mode { get; set; }

      internal SemaphoreSlim Semaphore { get; }

      public void Dispose()
      {
        Semaphore.Dispose();
      }
    }
  }
}