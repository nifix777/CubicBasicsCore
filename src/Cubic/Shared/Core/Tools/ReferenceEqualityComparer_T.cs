using System.Collections.Generic;
using Cubic.Core.Annotations;

namespace Cubic.Core.Tools
{
  /// <inheritdoc />
  public class ReferenceEqualityComparer<T> : ReferenceEqualityComparer, IEqualityComparer<T>
  {
    [NotNull]
    private static readonly ReferenceEqualityComparer<T> DefaultInternal;

    [NotNull]
    public new static ReferenceEqualityComparer<T> Default
    {
      get
      {
        return ReferenceEqualityComparer<T>.DefaultInternal;
      }
    }

    static ReferenceEqualityComparer()
    {
      ReferenceEqualityComparer<T>.DefaultInternal = new ReferenceEqualityComparer<T>();
    }

    protected ReferenceEqualityComparer()
    {
    }

    public bool Equals(T x, T y)
    {
      return base.Equals(x, y);
    }

    public int GetHashCode(T obj)
    {
      return base.GetHashCode(obj);
    }
  }
}