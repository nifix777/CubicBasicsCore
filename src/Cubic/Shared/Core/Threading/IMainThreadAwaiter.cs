using System;

namespace Cubic.Core.Threading
{
  public interface IMainThreadAwaiter
  {
    bool IsCompleted { get; }
    void OnCompleted(Action continuation);
    void GetResult();
  }
}
