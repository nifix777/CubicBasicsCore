﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cubic.Core.Annotations;

namespace Cubic.Core.Runtime
{
  /// <summary>
  /// A simple set of weak references.
  /// </summary>
  /// <typeparam name="T">The type of the referenced objects.</typeparam>
  public class WeakReferenceSet<T> : IEnumerable<T> where T : class
  {
    private int _cleanupCycleCounter;

    private List<WeakReference> _items = new List<WeakReference>();

    /// <summary>
    /// Adds the specified element to the set.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>
    /// <c>true</c> if the element is added to the WeakReferenceSet{T} object; <c>false</c> if the element is already present.
    /// </returns>
    public bool Add([CanBeNull] T item)
    {
      if (this.Contains(item))
        return false;

      if ((++_cleanupCycleCounter & 0x7F) == 0)
        Cleanup();

      _items.Add(new WeakReference(item));
      return true;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the alive items of the collection.
    /// </summary>
    /// <returns>
    /// A System.Collections.Generic.IEnumerator{T} that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator<T> GetEnumerator()
    {
      return _items.Where(reference => reference.IsAlive)
        .Select(reference => (T)reference.Target)
        .GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    private void Cleanup()
    {
      _items = new List<WeakReference>(_items.Where(reference => reference.IsAlive));
    }
  }
}