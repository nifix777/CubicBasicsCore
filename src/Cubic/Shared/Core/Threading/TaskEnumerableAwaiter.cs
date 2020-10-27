using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cubic.Core.Threading
{
  public struct TaskEnumerableAwaiter : INotifyCompletion
  {
    private TaskAwaiter _awaiter;

    public bool IsCompleted => _awaiter.IsCompleted;

    internal TaskEnumerableAwaiter(IEnumerable<Task> tasks)
    {
      _awaiter = Task.WhenAll(tasks).GetAwaiter();
    }



    public void OnCompleted(Action continuation) =>
        _awaiter.OnCompleted(continuation);

    public void GetResult() =>
        _awaiter.GetResult();
  }

  public static class EnumerableExtensions
  {
    public static TaskEnumerableAwaiter GetAwaiter(this IEnumerable<Task> tasks) =>
        new TaskEnumerableAwaiter(tasks);
  }
}