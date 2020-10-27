using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Threading
{
  public sealed class AsyncLock : IDisposable
  {
    private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);

    public async Task<IDisposable> AcquireAsync()
    {
      await this.semaphore.WaitAsync();
      return new AsyncLockReleaser(this);
    }

    public static async Task<IDisposable> LockAsync()
    {
      var @lock = new AsyncLock();
      await @lock.AcquireAsync();
      return new AsyncLockReleaser(@lock);
    }

    public void Release()
    {
      this.semaphore.Release();
    }

    public void Dispose()
    {
      this.semaphore.Dispose();
    }

    private sealed class AsyncLockReleaser : IDisposable
    {
      private readonly AsyncLock asyncLock;

      public AsyncLockReleaser(AsyncLock asyncLock)
      {
        this.asyncLock = asyncLock;
      }

      public void Dispose()
      {
        this.asyncLock.Release();
      }
    }
  }
}