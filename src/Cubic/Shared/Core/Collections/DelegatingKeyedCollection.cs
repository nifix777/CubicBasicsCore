using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cubic.Core.Collections
{
  [Serializable]
  public class DelegatingKeyedCollection<TKey, TItem> : KeyedCollection<TKey, TItem>
  {
    private readonly Func<TItem, TKey> _getKeyDelegate;

    public DelegatingKeyedCollection(Func<TItem, TKey> getKeyDelegate)
    {
      this._getKeyDelegate = getKeyDelegate;
    }

    public DelegatingKeyedCollection(Func<TItem, TKey> getKeyDelegate, IEqualityComparer<TKey> comparer) : base(comparer)
    {
      this._getKeyDelegate = getKeyDelegate;
    }

    public void AddRange(IEnumerable<TItem> items)
    {
      foreach (TItem item in items)
      {
        base.Add(item);
      }
    }

    protected override TKey GetKeyForItem(TItem item)
    {
      return this._getKeyDelegate(item);
    }
  }
}