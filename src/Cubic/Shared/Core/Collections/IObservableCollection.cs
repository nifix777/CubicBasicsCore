using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Cubic.Core.Collections
{
  public interface IObservableCollection<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
  {
  }
}