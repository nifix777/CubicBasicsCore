using System;
using System.Runtime.InteropServices;

namespace Cubic.Core.Runtime.Native
{
  public abstract class NativeLibary : INativeLibary
  {

    protected IntPtr _dllPointer;

    private bool _disposed;

    protected abstract void FreeLibaryInternal(IntPtr dllPointer);
    protected abstract IntPtr LoadLibaryInternal(string nativeDllFile);

    public void Dispose()
    {
      if (!_disposed)
      {
        if (IntPtr.Zero != _dllPointer)
        {
          FreeLibaryInternal(_dllPointer);
          _disposed = true;
        }

      }
    }

    public void LoadLibary(string nativeDllFile)
    {
      _dllPointer = LoadLibaryInternal(nativeDllFile);
    }

    public object GetDelegate(Type delegateType, string delegateName)
    {
      IntPtr delegatePtr = GetDelegatePtr(_dllPointer, delegateType, delegateName);

      return Marshal.GetDelegateForFunctionPointer(delegatePtr, delegateType);
    }

    protected abstract IntPtr GetDelegatePtr(IntPtr dllPointer, Type delegateType, string delegateName);

    public TMethod GetDelegate<TMethod>(string delegateName)
    {
      return (TMethod) GetDelegate(typeof(TMethod), delegateName);
    }
  }
}