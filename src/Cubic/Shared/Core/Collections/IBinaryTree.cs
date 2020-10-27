using System.Collections.Generic;

namespace Cubic.Core.Collections
{
  public interface IBinaryTree<T> : IEnumerable<T>
  {
    IList<IBinaryTree<T>> Nodes { get; }

    IBinaryTree<T> Left { get; }

    IBinaryTree<T> Right { get; }

    T Value { get; }
  }
}