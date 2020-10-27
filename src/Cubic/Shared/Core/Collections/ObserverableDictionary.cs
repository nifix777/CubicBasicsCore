using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Cubic.Core.Collections
{
  public class UiObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
  {
    private const string CountString = "Count";

    private const string IndexerName = "Item[]";

    private const string KeysName = "Keys";

    private const string ValuesName = "Values";

    public int Count
    {
      get
      {
        return this.Dictionary.Count;
      }
    }

    protected IDictionary<TKey, TValue> Dictionary
    {
      get;
      private set;
    }

    public bool IsReadOnly
    {
      get
      {
        return this.Dictionary.IsReadOnly;
      }
    }

    public TValue this[TKey key]
    {
      get
      {
        if (!this.Dictionary.Keys.Contains(key))
        {
          return default(TValue);
        }
        return this.Dictionary[key];
      }
      set
      {
        this.Insert(key, value, false);
      }
    }

    public ICollection<TKey> Keys
    {
      get
      {
        return this.Dictionary.Keys;
      }
    }

    public ICollection<TValue> Values
    {
      get
      {
        return this.Dictionary.Values;
      }
    }

    public UiObservableDictionary()
    {
      this.Dictionary = new Dictionary<TKey, TValue>();
    }

    public UiObservableDictionary(IDictionary<TKey, TValue> dictionary)
    {
      this.Dictionary = new Dictionary<TKey, TValue>(dictionary);
    }

    public UiObservableDictionary(IEqualityComparer<TKey> comparer)
    {
      this.Dictionary = new Dictionary<TKey, TValue>(comparer);
    }

    public UiObservableDictionary(int capacity)
    {
      this.Dictionary = new Dictionary<TKey, TValue>(capacity);
    }

    public UiObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
    {
      this.Dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
    }

    public UiObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
    {
      this.Dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
    }

    public void Add(TKey key, TValue value)
    {
      this.Insert(key, value, true);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
      this.Insert(item.Key, item.Value, true);
    }

    public void AddRange(IDictionary<TKey, TValue> items)
    {
      if (items == null)
      {
        throw new ArgumentNullException("items");
      }
      if (items.Count > 0)
      {
        if (items.Keys.Any<TKey>((TKey k) => this.Dictionary.ContainsKey(k)))
        {
          throw new ArgumentException("An item with the same key has already been added.");
        }
        foreach (KeyValuePair<TKey, TValue> item in items)
        {
          this.Dictionary.Add(item);
        }
        this.OnCollectionChanged(NotifyCollectionChangedAction.Add, Enumerable.ToArray<KeyValuePair<TKey, TValue>>(items));
      }
    }

    public void Clear()
    {
      if (this.Dictionary.Count > 0)
      {
        this.Dictionary.Clear();
        this.OnCollectionChanged();
      }
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      return this.Dictionary.Contains(item);
    }

    public bool ContainsKey(TKey key)
    {
      return this.Dictionary.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      this.Dictionary.CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      return this.Dictionary.GetEnumerator();
    }

    private void Insert(TKey key, TValue value, bool add)
    {
      TValue tValue;
      if (key == null)
      {
        throw new ArgumentNullException("key");
      }
      if (!this.Dictionary.TryGetValue(key, out tValue))
      {
        this.Dictionary[key] = value;
        this.OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
        return;
      }
      if (add)
      {
        throw new ArgumentException("An item with the same key has already been added.");
      }
      if (object.Equals(tValue, value))
      {
        return;
      }
      this.Dictionary[key] = value;
      this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, tValue));
    }

    private void OnCollectionChanged()
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
    {
      this.OnDictionaryPropertyChanged();
      NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler = this.CollectionChanged;
      if (notifyCollectionChangedEventHandler != null)
      {
        notifyCollectionChangedEventHandler(this, eventArgs);
      }
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem)
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, (object)changedItem));
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, (object)newItem, (object)oldItem));
    }

    private void OnCollectionChanged(NotifyCollectionChangedAction action, IList newItems)
    {
      this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItems));
    }

    private void OnDictionaryPropertyChanged()
    {
      this.OnPropertyChanged(CountString);
      this.OnPropertyChanged(IndexerName);
      this.OnPropertyChanged(KeysName);
      this.OnPropertyChanged(ValuesName);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChangedEventHandler propertyChangedEventHandler = this.PropertyChanged;
      if (propertyChangedEventHandler != null)
      {
        propertyChangedEventHandler(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    public bool Remove(TKey key)
    {
      TValue tValue;
      if (key == null)
      {
        throw new ArgumentNullException("key");
      }
      this.Dictionary.TryGetValue(key, out tValue);
      bool flag = this.Dictionary.Remove(key);
      if (flag)
      {
        this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, tValue));
      }
      return flag;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      return this.Remove(item.Key);
    }

    IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.Dictionary.GetEnumerator();
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      return this.Dictionary.TryGetValue(key, out value);
    }

    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public event PropertyChangedEventHandler PropertyChanged;
  }
}