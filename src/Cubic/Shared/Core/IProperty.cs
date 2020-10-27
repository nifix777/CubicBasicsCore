using System;
using System.ComponentModel;

namespace Cubic.Core
{
  public interface IProperty<T> : IProperty, INotifyPropertyChanged, INotifyPropertyChanging
  {
    new T Value { get; set; }
  }

  public interface IProperty
  {
    object Value { get; set; }

    Type ValueType { get; }

    string Name { get; }
  }
}