using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Cubic.Core.Annotations;

namespace Cubic.Core.Collections
{
  public class ObserverableListAdapter<T> : IObservableCollection<T>
  {
    private readonly IList<T> _source;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableListAdapter{T}"/> class.
    /// </summary>
    /// <param name="source">The source.</param>
    public ObserverableListAdapter([NotNull] IList<T> source)
    {

      _source = source;

      var ncc = source as INotifyCollectionChanged;
      if (ncc != null)
        ncc.CollectionChanged += Source_CollectionChanged;

      var npc = source as INotifyPropertyChanged;
      if (npc != null)
        npc.PropertyChanged += Source_PropertyChanged;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return _source.GetEnumerator();
    }

    /// <summary>
    /// Copies the elements of the <see cref="T:System.Collections.ICollection"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
    /// </summary>
    /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"/>. The <see cref="T:System.Array"/> must have zero-based indexing. </param><param name="index">The zero-based index in <paramref name="array"/> at which copying begins. </param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero. </exception><exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"/> is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.-or-The type of the source <see cref="T:System.Collections.ICollection"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
    public void CopyTo(Array array, int index)
    {
      for (var i = 0; i < Count; i++)
      {
        array.SetValue(this[i], i + index);
      }
    }

    /// <summary>
    /// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
    /// </summary>
    /// <returns>
    /// The number of elements contained in the <see cref="T:System.Collections.ICollection"/>.
    /// </returns>
    public int Count => _source.Count;

    /// <summary>
    /// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
    /// </summary>
    /// <returns>
    /// An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
    /// </returns>
    public object SyncRoot => this;

    /// <summary>
    /// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe).
    /// </summary>
    /// <returns>
    /// true if access to the <see cref="T:System.Collections.ICollection"/> is synchronized (thread safe); otherwise, false.
    /// </returns>
    public bool IsSynchronized => false;

    /// <summary>
    /// Adds an item to the <see cref="T:System.Collections.IList"/>.
    /// </summary>
    /// <returns>
    /// The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection,
    /// </returns>
    /// <param name="value">The object to add to the <see cref="T:System.Collections.IList"/>. </param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
    public void Add(T value)
    {
      _source.Add(value);
    }

    /// <summary>
    /// Determines whether the <see cref="T:System.Collections.IList"/> contains a specific value.
    /// </summary>
    /// <returns>
    /// true if the <see cref="T:System.Object"/> is found in the <see cref="T:System.Collections.IList"/>; otherwise, false.
    /// </returns>
    /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList"/>. </param>
    public bool Contains(T value)
    {
      return _source.Contains(value);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      _source.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes all items from the <see cref="T:System.Collections.IList"/>.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only. </exception>
    public void Clear()
    {
      _source.Clear();
    }

    /// <summary>
    /// Determines the index of a specific item in the <see cref="T:System.Collections.IList"/>.
    /// </summary>
    /// <returns>
    /// The index of <paramref name="value"/> if found in the list; otherwise, -1.
    /// </returns>
    /// <param name="value">The object to locate in the <see cref="T:System.Collections.IList"/>. </param>
    public int IndexOf([CanBeNull] T value)
    {
      return _source.IndexOf(value);
    }

    /// <summary>
    /// Inserts an item to the <see cref="T:System.Collections.IList"/> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which <paramref name="value"/> should be inserted. </param><param name="value">The object to insert into the <see cref="T:System.Collections.IList"/>. </param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception><exception cref="T:System.NullReferenceException"><paramref name="value"/> is null reference in the <see cref="T:System.Collections.IList"/>.</exception>
    public void Insert(int index, [CanBeNull] T value)
    {
      _source.Insert(index, value);
    }

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"/>.
    /// </summary>
    /// <param name="value">The object to remove from the <see cref="T:System.Collections.IList"/>. </param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
    public bool Remove([CanBeNull] T value)
    {
      return _source.Remove(value);
    }

    /// <summary>
    /// Removes the <see cref="T:System.Collections.IList"/> item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove. </param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>. </exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"/> is read-only.-or- The <see cref="T:System.Collections.IList"/> has a fixed size. </exception>
    public void RemoveAt(int index)
    {
      _source.RemoveAt(index);
    }

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <returns>
    /// The element at the specified index.
    /// </returns>
    /// <param name="index">The zero-based index of the element to get or set. </param>
    /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:System.Collections.IList"/>.</exception>
    /// <exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.IList"/> is read-only. </exception>
    public T this[int index]
    {
      get => _source[index];
      set => _source[index] = value;
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> is read-only.
    /// </summary>
    /// <returns>
    /// true if the <see cref="T:System.Collections.IList"/> is read-only; otherwise, false.
    /// </returns>
    public bool IsReadOnly => _source.IsReadOnly;

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:System.Collections.IList"/> has a fixed size.
    /// </summary>
    /// <returns>
    /// true if the <see cref="T:System.Collections.IList"/> has a fixed size; otherwise, false.
    /// </returns>
    public bool IsFixedSize => false;

    /// <summary>
    /// Occurs when the collection changes.
    /// </summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private void Source_PropertyChanged([CanBeNull] object sender, [CanBeNull] PropertyChangedEventArgs e)
    {
      OnPropertyChanged(e);
    }

    private void Source_CollectionChanged([CanBeNull] object sender, [CanBeNull] NotifyCollectionChangedEventArgs e)
    {
      OnCollectionChanged(e);
    }

    private void OnCollectionChanged([CanBeNull] NotifyCollectionChangedEventArgs e)
    {
      CollectionChanged?.Invoke(this, e);
    }

    private void OnPropertyChanged([CanBeNull] PropertyChangedEventArgs e)
    {
      PropertyChanged?.Invoke(this, e);
    }

    public IEnumerator GetEnumerator()
    {
      return _source.GetEnumerator();
    }
  }
}