using System.Collections;
using System.Collections.Generic;

namespace Cubic.Core.Collections
{
  public class BinaryTree<T> : IBinaryTree<T>
  {

    public BinaryTree()
    {
      Nodes = new List<IBinaryTree<T>>();
      this.Left = null;
      this.Right = null;
    }
    // ...
    #region IEnumerable<T>
    public IEnumerator<T> GetEnumerator()
    {
      // Return the item at this node.
      yield return Value;
      // Iterate through each of the elements in the pair.
      foreach ( IBinaryTree<T> tree in Nodes )
      {
        if ( tree != null )
        {
          // Because each element in the pair is a tree,
          // traverse the tree and yield each element.
          foreach ( T item in tree )
          {
            yield return item;
          }
        }
      }
    }
    #endregion IEnumerable<T>
    #region IEnumerable Members
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
    #endregion

    public IList<IBinaryTree<T>> Nodes { get; }

    public IBinaryTree<T> Left
    {
      get => Nodes[0];
      set => Nodes[0] = value;
    }

    public IBinaryTree<T> Right
    {
      get => Nodes[1];
      set => Nodes[1] = value;
    }

    public T Value { get; set; }
  }
}