using System;
using System.Threading;

namespace Cubic.Core.Threading
{
  public sealed class Lock : IDisposable
  {
    private ReaderWriterLockSlim _thisLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
    private int _isDisposed = 0;
    public void EnterReadLock()
    {
      _thisLock.EnterReadLock();
    }

    public void EnterWriteLock()
    {
      _thisLock.EnterWriteLock();
    }

    public void ExitReadLock()
    {
      _thisLock.ExitReadLock();
    }

    public void ExitWriteLock()
    {
      _thisLock.ExitWriteLock();
    }

    public void Dispose()
    {
      if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
      {
        _thisLock.Dispose();
      }
    }
  }

  internal struct WriteLock : IDisposable
  {
    private readonly Lock _lock;
    private int _isDisposed;

    public WriteLock(Lock @lock)
    {
      _isDisposed = 0;
      _lock = @lock;
      _lock.EnterWriteLock();
    }

    public void Dispose()
    {
      if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
      {
        _lock.ExitWriteLock();
      }
    }
  }

  internal struct ReadLock : IDisposable
  {
    private readonly Lock _lock;
    private int _isDisposed;

    public ReadLock(Lock @lock)
    {
      _isDisposed = 0;
      _lock = @lock;
      _lock.EnterReadLock();
    }

    public void Dispose()
    {
      if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
      {
        _lock.ExitReadLock();
      }
    }
  }

  public static class LockExtensions
  {
    public static IDisposable LockRead(this Lock lockObj)
    {
      return new ReadLock(lockObj);
    }

    public static IDisposable LockWrite(this Lock lockObj)
    {
      return new WriteLock(lockObj);
    }
  }
}