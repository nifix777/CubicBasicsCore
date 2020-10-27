using System;

namespace Cubic.Core.Runtime.Native
{
  public interface INativeLibary : IDisposable
  {
    void LoadLibary(string nativeDllFile);

    object GetDelegate(Type delegateType, string delegateName);

    TMethod GetDelegate<TMethod>(string delegateName);
  }
}