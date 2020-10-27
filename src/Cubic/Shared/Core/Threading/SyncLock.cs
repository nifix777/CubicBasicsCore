using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cubic.Core.Threading
{
  public sealed class SyncLock
    : ISyncLock,
      IDisposable
  {
    private readonly object syncLock;


    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="syncLock">Optional: if null, <see langword="this"/>
    /// is used.</param>
    public SyncLock(object syncLock)
      => this.syncLock = syncLock ?? this;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable Enter()
    {
      Monitor.Enter(syncLock);
      return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
      => Monitor.Exit(syncLock);
  }
}