using System.Collections.Generic;
using System.Collections.Specialized;

namespace Cubic.Core.Collections
{
  public interface IChangeTrackingCollection<T> : IList<T>, INotifyCollectionChanged
  {
    void AcceptChanges();

    bool HasChanges { get; }

    IEnumerable<T> Modified { get; } 
    IEnumerable<T> New { get; } 
    IEnumerable<T> Removed { get; } 
  }
}