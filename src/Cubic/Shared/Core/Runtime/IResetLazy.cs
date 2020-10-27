using System;

namespace Cubic.Core.Runtime
{
  public interface IResetLazy<T>
  {
    void Reset();
    T Value { get; }

    bool IsValueCreated { get; }
    Type DeclaringType { get; }
  }
}