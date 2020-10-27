using System.Collections;
using System.Runtime.CompilerServices;
using Cubic.Core.Annotations;

namespace Cubic.Core.Tools
{
  /// <summary>
  /// Copnares Reference
  /// </summary>
  /// <seealso cref="System.Collections.IEqualityComparer" />
  public class ReferenceEqualityComparer : IEqualityComparer
  {
    [NotNull]
    private readonly static ReferenceEqualityComparer DefaultInternal;

    [NotNull]
    public static ReferenceEqualityComparer Default
    {
      get
      {
        return ReferenceEqualityComparer.DefaultInternal;
      }
    }

    static ReferenceEqualityComparer()
    {
      ReferenceEqualityComparer.DefaultInternal = new ReferenceEqualityComparer();
    }

    protected ReferenceEqualityComparer()
    {
    }

    public new bool Equals(object x, object y)
    {
      return object.ReferenceEquals(x, y);
    }

    public int GetHashCode(object obj)
    {
      return RuntimeHelpers.GetHashCode(obj);
    }
  }
}