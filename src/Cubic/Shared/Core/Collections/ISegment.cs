using System;
using System.Collections.Generic;

namespace Cubic.Core.Collections
{

  public interface ISegment<T> : IEnumerable<T>, IEquatable<ISegment<T>>, IEquatable<IReadOnlyList<T>>, IReadOnlyList<T>
  {
    int Offset { get; }

    int Length { get; }

    int End { get; }

    bool IsEmpty { get; }

    bool Contains(int position);

    int IndexOf(T item);

    bool Contains(ISegment<T> segment);

    bool OverlapsWith(ISegment<T> segment);

    //T this[int index] { get; }
  }
}