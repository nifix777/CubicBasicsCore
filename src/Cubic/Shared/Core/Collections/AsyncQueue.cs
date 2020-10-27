using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cubic.Core.Collections
{
  public class AsyncQueue<T> where T : class 
  {
    private readonly Queue<T> _itemsQueue = new Queue<T>();

    private volatile LinkedList<TaskCompletionSource<T>> _waiters = new LinkedList<TaskCompletionSource<T>>();

    public void Enqueue(T item)
    {
      lock (_itemsQueue)
      {
        _itemsQueue.Enqueue(item);

        while (_waiters.FirstOrDefault() != null)
        {
          _waiters.First.Value.TrySetResult((T)null);
          _waiters.RemoveFirst();
        }
      }
    }

    public async Task<IEnumerable<T>> DrainAsync()
    {
      while (true)
      {
        TaskCompletionSource<T> taskCompletionSource;

        lock (_itemsQueue)
        {
          if (_itemsQueue.Count > 0)
          {
            return YieldAllItems();
          }

          taskCompletionSource = new TaskCompletionSource<T>();
          _waiters.AddLast(taskCompletionSource);

        }

        await taskCompletionSource.Task;
      }
    }

    private IEnumerable<T> YieldAllItems()
    {
      while (_itemsQueue.Count > 0)
      {
        yield return _itemsQueue.Dequeue();
      }
    }
  }
}