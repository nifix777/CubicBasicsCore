using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cubic.Core.Annotations;

namespace Cubic.Core.Collections
{
  public static class ListExtensions
  {
    #region IList

    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target">The target.</param>
    /// <param name="soruce">The soruce.</param>
    public static void AddRange<T>(this IList<T> target, IEnumerable<T> soruce)
    {
      foreach (var item in soruce)
      {
        target.Add(item);
      }
    }

    /// <summary>
    /// Adds the range.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="soruce">The soruce.</param>
    public static void AddRange(this IList target, IEnumerable soruce)
    {
      foreach (var item in soruce)
      {
        target.Add(item);
      }
    }


    /// <summary>
    /// Inserts the range.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">The list.</param>
    /// <param name="index">The index.</param>
    /// <param name="items">The items.</param>
    /// <exception cref="System.ArgumentNullException">
    /// list
    /// or
    /// items
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">index</exception>
    public static void InsertRange<T>(this IList<T> list, int index, [NotNull] IEnumerable<T> items)
    {
      if (list == null)
      {
        throw new ArgumentNullException("list");
      }
      if (index < 0 || list.Count > index)
      {
        throw new ArgumentOutOfRangeException("index");
      }
      if (items == null)
      {
        throw new ArgumentNullException("items");
      }
      foreach (T t in items.Reverse<T>())
      {
        list.Insert(index, t);
      }
    }

    /// <summary>
    /// Removes items that matches the specified filter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item">The item.</param>
    /// <param name="filter">The filter.</param>
    /// <exception cref="System.ArgumentNullException">
    /// item
    /// or
    /// filter
    /// </exception>
    public static void Remove<T>(this IList<T> item, [NotNull] Predicate<T> filter)
    {
      if (item == null)
      {
        throw new ArgumentNullException("item");
      }
      if (filter == null)
      {
        throw new ArgumentNullException("filter");
      }
      for (int i = 0; i < item.Count; i++)
      {
        if (filter(item[i]))
        {
          item.RemoveAt(i);
          i--;
        }
      }
    }

    #endregion

    public static IList<T> Clone<T>(this IList<T> source)
    {
      return new List<T>(source);
    }

  }
}