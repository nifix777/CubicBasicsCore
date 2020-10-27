using System;
using System.Runtime.InteropServices;

namespace Cubic.Core.Runtime
{
  public class ManagedGCHandle : IDisposable
  {
    private GCHandle _handle;

    private bool _isDisposed;

    /// <summary>
    /// Provides the target of the GCHandle
    /// </summary>
    public object Target
    {
      get => _handle.IsAllocated ? _handle.Target : null;
      set
      {
        if (!_handle.IsAllocated)
        {
          _handle = GCHandle.Alloc(value, GCHandleType.Weak);
        }
        else
        {
          _handle.Target = value;
        }
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    public virtual void Dispose(bool disposing)
    {
      if (_isDisposed)
      {
        return;
      }

      if (_handle.IsAllocated)
      {
        _handle.Free();
      }

      _isDisposed = true;
    }

    ~ManagedGCHandle()
    {
      this.Dispose(false);
    }
  }
}
