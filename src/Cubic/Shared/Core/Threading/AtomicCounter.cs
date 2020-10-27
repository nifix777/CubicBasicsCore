using System.Threading;

namespace Cubic.Core.Threading
{
  public class AtomicCounter
  {
    long counter = 0;

    public AtomicCounter(long initialCount = 0)
    {
      this.counter = initialCount;
    }

    public long Increment()
    {
      return Interlocked.Increment(ref counter);
    }

    public void IncrementIfNonzero(ref bool success)
    {
      long origValue = counter;
      while (true)
      {
        if (origValue == 0)
        {
          success = false;
          return;
        }
        long result = Interlocked.CompareExchange(ref counter, origValue + 1, origValue);
        if (result == origValue)
        {
          success = true;
          return;
        };
        origValue = result;
      }
    }

    public long Decrement()
    {
      return Interlocked.Decrement(ref counter);
    }

    public long Count
    {
      get
      {
        return Interlocked.Read(ref counter);
      }
    }
  }
}
