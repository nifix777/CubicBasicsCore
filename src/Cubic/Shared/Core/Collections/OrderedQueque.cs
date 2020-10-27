using System.Collections.Generic;
using System.Linq;

namespace Cubic.Core.Collections
{
  /// <summary>
  /// Implementation of an Ordered Queque. Uses intenaly <see cref="List{T}"/> the OrderBy LINQ-Method <see cref="Enumerable.OrderBy{TSource,TKey}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TKey})"/>
  /// </summary>
  /// <typeparam name="P"></typeparam>
  /// <typeparam name="V"></typeparam>
  public class OrderedQueque<P, V>
  {
    private List<KeyValuePair<P, V>>  _values = new List<KeyValuePair<P, V>>();

    private IOrderedEnumerable<KeyValuePair<P, V>> _ordered;

    public int Count => _values.Count;

    public bool IsEmpty => _values.Any();

    public void Enquue(P priority, V item)
    {
      this.Enquue(new KeyValuePair<P, V>(priority, item));
    }

    public void Enquue( KeyValuePair<P , V> kv )
    {
      _values.Add( kv );
      Reorder();
    }

    private void Reorder()
    {
      _ordered = _values.OrderBy(kv => kv.Key);
    }

    public KeyValuePair<P , V> Dequeue()
    {
      var last = PeekInternal();

      _values.Remove(last);
      Reorder();
      return last;
    }

    public KeyValuePair<P , V> Peek()
    {
      return PeekInternal();
    }

    private KeyValuePair<P, V> PeekInternal()
    {
      return (KeyValuePair<P, V>) _ordered.First();
    } 

    public void Update(KeyValuePair<P, V> kv)
    {
      if (_values.Contains(kv))
      {
        _values.Remove(kv);
        Reorder();
      }
    }
  }
}