using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Cubic.Core.Annotations;
using Cubic.Core.Diagnostics;
using Cubic.Core.Reflection;
using Cubic.Core.Runtime;

namespace Cubic.Core.Collections
{
  /// <summary>
  /// Methods for Collections
  /// </summary>
  public static class CollectionExtensions
  {
    #region Queue

    /// <summary>
    /// Determines whether this instance is empty.
    /// </summary>
    /// <param name="queue">The queue.</param>
    /// <returns>
    ///   <c>true</c> if the specified queue is empty; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsEmpty(this Queue queue)
    {
      return queue.Count == 0;
    }

    /// <summary>
    /// Determines whether this instance is empty.
    /// </summary>
    /// <param name="queue">The queue.</param>
    /// <returns>
    ///   <c>true</c> if the specified queue is empty; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsEmpty<T>(this Queue<T> queue)
    {
      return queue.Count == 0;
    }

    /// <summary>
    /// Determines whether this instance is empty.
    /// </summary>
    /// <param name="queue">The queue.</param>
    /// <returns>
    ///   <c>true</c> if the specified queue is empty; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsEmptySync<T>(this Queue<T> queue)
    {
      lock (queue)
      {
        return queue.Count == 0;
      }
    }

    /// <summary>
    /// Determines whether this instance is empty.
    /// </summary>
    /// <param name="queue">The queue.</param>
    /// <returns>
    ///   <c>true</c> if the specified queue is empty; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsEmpty<T>(this ConcurrentQueue<T> queue)
    {
      return queue.IsEmpty;
    }

    #endregion

    #region Segment

    public static ISegment<T> Split<T>(this IReadOnlyList<T> source, int offset, int lenght)
    {
      return new Segment<T>(source, offset, lenght);
    }

    public static ISegment<T> Split<T>(this IReadOnlyList<T> source)
    {
      return new Segment<T>(source, 0, source.Count);
    }

    #endregion


    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
      foreach(var item in items)
      {
        collection.Add(item);
      }
    }

    public static Stack<T> Copy<T>(this Stack<T> stack)
    {
      Guard.AgainstNull(stack, nameof(stack));

      // Stack<T>.GetEnumerator walks from top to bottom 
      // of the stack, whereas Stack<T>(IEnumerable<T>) 
      // pushes to bottom from top, so we need to reverse 
      // the stack to get them in the right order.
      return new Stack<T>(stack.Reverse());
    }



    public static bool IsCollectionEqual<T>(this IList<T> thisList, IList<T> thatList)
    {
      if (thisList.Count != thatList.Count)
      {
        return false;
      }

      for (int i = 0; i < thisList.Count; i++)
      {
        if (!thisList[i].Equals(thatList[i]))
        {
          return false;
        }
      }

      return true;
    }

    public static bool IsCollectionEqual<T>(this IReadOnlyList<T> thisList, IReadOnlyList<T> thatList)
    {
      if (thisList.Count != thatList.Count)
      {
        return false;
      }

      for (int i = 0; i < thisList.Count; i++)
      {
        if (!thisList[i].Equals(thatList[i]))
        {
          return false;
        }
      }

      return true;
    }

    public static bool Override<T>(this ICollection<T> collection, T value)
    {
      bool result = false;

      if(collection.Contains(value))
      {
        result = collection.Remove(value);
      }
      collection.Add(value);

      return result;
    }
  }
}