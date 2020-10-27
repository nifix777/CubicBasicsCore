using System;

namespace Cubic.Core
{
  /// <summary>
  /// Abstract implementation of IDisposable.
  /// </summary>
  /// <remarks>
  /// Overwrite <see cref="DisposeResources"/> if there are other <see cref="IDisposable"/> resources
  /// </remarks>
  public abstract class DisposableObject : IDisposable
  {
    private bool _disposed;
    private readonly object _locko = new object();

    // gets a value indicating whether this instance is disposed.
    // for internal tests only (not thread safe)
    //TODO make this internal + rename "Disposed" when we can break compatibility
    internal bool Disposed => _disposed;

    // implements IDisposable
    public void Dispose()
    {
      Dispose( true );
      GC.SuppressFinalize( this );
    }

    // finalizer
    ~DisposableObject()
    {
      Dispose( false );
    }

    private void Dispose( bool disposing )
    {
      lock ( _locko )
      {
        if ( _disposed ) return;

      }

      try
      {
        DisposeUnmanagedResources();
        if (disposing)
          DisposeResources();
      }
      catch
      {
        // ignored
      }
      finally
      {
        lock (_locko)
        {
          _disposed = true;
        }
      }
    }

    /// <summary>
    /// Throws if disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException"></exception>
    public void ThrowIfDisposed()
    {
      lock (_locko)
      {
        if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);
      }
    }

    protected virtual void DisposeResources()
    {

    }

    protected abstract void DisposeUnmanagedResources();
  }
}