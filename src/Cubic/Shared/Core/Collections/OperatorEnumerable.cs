using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cubic.Core.Collections
{
  public class OperatorEnumerable<T> : IEnumerable<T>
  {
    private readonly IEnumerable<T> _source;

    public OperatorEnumerable(IEnumerable<T> source)
    {
      _source = source;
    }

    public IEnumerator<T> GetEnumerator()
    {
      return _source.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _source.GetEnumerator();
    }

    public static IEnumerable<T> operator +(OperatorEnumerable<T> left, OperatorEnumerable<T> right)
    {
      return left.Concat(right);
    }

    public static IEnumerable<T> operator +(OperatorEnumerable<T> left, T right)
    {
      return left.Concat(right);
    }

    public static IEnumerable<T> operator -(OperatorEnumerable<T> left, OperatorEnumerable<T> right)
    {
      return left.Except(right);
    }

    public static IEnumerable<T> operator -(OperatorEnumerable<T> left, T right)
    {
      return left.Except(right.Yield());
    }


    public static IEnumerable<OperatorEnumerable<T>> operator /(OperatorEnumerable<T> lhs, int rhs)
    {
      return Batch(lhs, rhs);
    }

    private static IEnumerable<OperatorEnumerable<T>> Batch(OperatorEnumerable<T> source, int size)
    {
      List<T> currentList = new List<T>(size);
      foreach (var item in source)
      {
        currentList.Add(item);
        if (currentList.Count == size)
        {
          yield return new OperatorEnumerable<T>(currentList);
          currentList = new List<T>(size);
        }
      }
    }

    public static OperatorEnumerable<T> operator ^(OperatorEnumerable<T> left, IEnumerable<T> right)
    {
      return new OperatorEnumerable<T>(left.Except(right).Union(right.Except(left)));
    }

    public static OperatorEnumerable<T> operator ~(OperatorEnumerable<T> operand)
    {
      return new OperatorEnumerable<T>(Shuffle(operand));
    }

    public static implicit operator bool(OperatorEnumerable<T> operand)
    {
      return operand.Any();
    }

    //public static bool operator false(OperatorEnumerable<T> operand)
    //{
    //  return !operand.Any();
    //}

    private static IEnumerable<T> Shuffle(IEnumerable<T> source)
    {
      // Never do this!
      Random rng = new Random();
      var array = source.ToArray();
      // Note i > 0 to avoid final pointless iteration
      for (int i = array.Length - 1; i > 0; i--)
      {
        // Swap element "i" with a random earlier element it (or itself)
        int swapIndex = rng.Next(i + 1);
        T tmp = array[i];
        array[i] = array[swapIndex];
        array[swapIndex] = tmp;
      }
      // Lazily yield (avoiding aliasing issues etc)
      foreach (T element in array)
      {
        yield return element;
      }
    }

    public static OperatorEnumerable<T> operator <<(OperatorEnumerable<T> lhs, int rhs)
    {
      return new OperatorEnumerable<T>(lhs.Skip(rhs).Concat(lhs.Take(rhs)));
    }

    public static OperatorEnumerable<T> operator >>(OperatorEnumerable<T> lhs, int rhs)
    {
      int count = lhs.Count();
      return lhs << (count - rhs);
    }
  }
}
