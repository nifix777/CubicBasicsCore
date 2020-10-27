using System.Collections;
using System.Collections.Generic;

namespace Cubic.Core.Collections
{
  public struct Pair<T> : IPair<T>
  {
    public Pair( T first , T second ) : this()
    {
      First = first;
      Second = second;
    }
    public T First { get; }
    public T Second { get; }

    #region IEnumerable<T>
    public IEnumerator<T> GetEnumerator()
    {
      yield return First;
      yield return Second;
    }
    #endregion IEnumerable<T>

    #region IEnumerable Members
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
    #endregion
  }

  public interface IPair<T> : IEnumerable<T>
  {
    T First { get; }
    T Second { get; }
  }
}