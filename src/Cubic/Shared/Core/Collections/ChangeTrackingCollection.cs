using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Cubic.Core.Collections
{
  public class ChangeTrackingCollection<T> : IList<T>, IChangeTrackingCollection<T>
  {
    private IList<T> _items;

    private IList<T> _removed;

    private IList<T> _new;

    private IList<T> _modified;

    public ChangeTrackingCollection()
    {
      _items = new List<T>();
      _removed = new List<T>();
      _modified = new List<T>();
      _new = new List<T>();
    }

    public ChangeTrackingCollection(IEnumerable<T> source )
    {
      _items.AddRange(source);
      AcceptChanges();
    }

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    
    public void AcceptChanges()
    {
      _removed.Clear();
      _modified.Clear();
      _new.Clear();
    }

    public bool HasChanges => _removed.Any() | _new.Any() | _modified.Any();
    public IEnumerable<T> Modified => _modified;
    public IEnumerable<T> New => _new;
    public IEnumerable<T> Removed => _removed;

    public IEnumerator<T> GetEnumerator()
    {
      return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable) _items).GetEnumerator();
    }

    public void Add(T item)
    {
      _items.Add(item);
      _new.Add(item);
      FireEvent( NotifyCollectionChangedAction.Add );
    }

    public void Clear()
    {
      _items.Clear();
      FireEvent( NotifyCollectionChangedAction.Reset );
    }

    public bool Contains(T item)
    {
      return _items.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      _items.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
      if (_items.Remove(item))
      {
        _removed.Add(item);
        FireEvent( NotifyCollectionChangedAction.Remove );
        return true;
      }

      return false;
    }

    public int Count
    {
      get { return _items.Count; }
    }

    public bool IsReadOnly
    {
      get { return _items.IsReadOnly; }
    }

    public int IndexOf(T item)
    {
      return _items.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
      _items.Insert(index, item);
      _new.Add(item);
      FireEvent( NotifyCollectionChangedAction.Add );
    }

    public void RemoveAt(int index)
    {
      var item = _items[index];
      _items.RemoveAt(index);
      _removed.Add(item);
      FireEvent(NotifyCollectionChangedAction.Remove);
    }

    public T this[int index]
    {
      get { return _items[index]; }
      set
      {
        var tempItem = _items[index];
        _items[index] = value;
        _modified.Add(tempItem);
        FireEvent(NotifyCollectionChangedAction.Replace);
      }
    }

    private void FireEvent( NotifyCollectionChangedAction action)
    {
      var args = new NotifyCollectionChangedEventArgs(action);
      CollectionChanged?.Invoke(this, args);
    }
  }
}