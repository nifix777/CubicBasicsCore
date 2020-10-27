﻿using System;
using System.ComponentModel;

namespace Cubic.Core.Collections
{
  [Serializable]
  public class ObservableKeyValuePair<TKey, TValue> : INotifyPropertyChanged
  {
    #region properties
    private TKey key;
    private TValue value;

    public TKey Key
    {
      get { return key; }
      set
      {
        key = value;
        OnPropertyChanged("Key");
      }
    }

    public TValue Value
    {
      get { return value; }
      set
      {
        this.value = value;
        OnPropertyChanged("Value");
      }
    }
    #endregion

    #region INotifyPropertyChanged Members

    [field: NonSerialized]
    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string name)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(name));
    }

    #endregion
  }
}