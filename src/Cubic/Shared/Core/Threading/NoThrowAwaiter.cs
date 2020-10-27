using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cubic.Core.Threading
{
  internal struct NoThrowAwaiter : ICriticalNotifyCompletion
  {
    private readonly Task _task;
    public NoThrowAwaiter(Task task) { _task = task; }
    public NoThrowAwaiter GetAwaiter() => this;
    public bool IsCompleted => _task.IsCompleted;
    // Observe exception
    public void GetResult() { var ex = _task.Exception; }
    public void OnCompleted(Action continuation) => _task.GetAwaiter().OnCompleted(continuation);
    public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);
  }
}