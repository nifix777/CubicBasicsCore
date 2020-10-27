using System;
using System.Runtime.CompilerServices;

namespace Cubic.Core.Threading
{
  /// <summary>
  /// Implements a do-nothing <see cref="ISyncLock"/>.
  /// </summary>
  public struct NoOpSyncLock
    : ISyncLock,
      IDisposable
  {
    /// <summary>
    /// Provides a "singleton" instance.
    /// </summary>
    public static readonly NoOpSyncLock Instance = new NoOpSyncLock();


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IDisposable Enter()
      => this;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() { }
  }
}