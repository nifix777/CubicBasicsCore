using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core
{
  public class ObservableNameValue<T> : INotifyPropertyChanged
  {
    private T _value;

    public ObservableNameValue(string name, T value)
    {
      Name = name;
      _value = value;
    }

    public string Name { get; }
    public T Value
    {
      get { return _value; }
      set
      {
        _value = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
  }
}
