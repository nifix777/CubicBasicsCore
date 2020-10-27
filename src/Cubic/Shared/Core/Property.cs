using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Cubic.Core.Collections;
using Cubic.Core.Reflection;

namespace Cubic.Core
{
  public class Property<TProperty> : IProperty<TProperty>
  {
    private TProperty _value;

    public Property(string name)
    {
      Name = name;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public TProperty Value
    {
      get { return _value; }
      set
      {
        this.Set(ref _value, value);
      }
    }

    object IProperty.Value
    {
      get { return _value; }
      set { this.Set(ref _value, (TProperty)value); }
    }

    public Type ValueType => typeof (TProperty);

    public string Name { get; }

    protected void Set<T>(ref T property, T value, [CallerMemberName] string name = "")
    {
      if (!EqualityComparer<T>.Default.Equals(property, value))
      {
        NotifyPropertyChanging(name);

        property = value;

        NotifyPropertyChanged(name);
      }
    }

    public event PropertyChangingEventHandler PropertyChanging;

    protected void NotifyPropertyChanging(string propname)
    {
      PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propname));
    }

    protected void NotifyPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler handler = this.PropertyChanged;
      if (handler != null)
      {
        var e = new PropertyChangedEventArgs(propertyName);
        handler(this, e);
      }
    }



    public static IProperty WalkProperty(string name, Type type, object instance)
    {
      var keys = new Stack<string>(name.Split('.').Reverse());

      var key = keys.Pop();
      IProperty prop = null;
      while (!key.IsNullOrEmpty())
      {
        prop= type.GetPropertyValue<IProperty>(name, instance);
        key = keys.Pop();
      }

      return prop;
    }
  }


}