using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Cubic.Core.Components
{
  public abstract class DependencyObject
  {

    private Dictionary<DependencyProperty, object> _dependencyPropertyValues;

    protected DependencyObject()
    {
      _dependencyPropertyValues = new Dictionary<DependencyProperty, object>();

      InitialiaseProperties();
    }

    private void InitialiaseProperties()
    {
      foreach (var dependencyProperty in DependencyObjectProvider.GetDependencyProperties(this.GetType()))
      {
        _dependencyPropertyValues.Add(dependencyProperty, dependencyProperty.Metada.DefaultValue);
      }
    }

    public object GetValue(DependencyProperty property)
    {
      if (!property.Metada.IsReadable)
      {
        throw new InvalidOperationException($"Property/Field {property.PropertyName} on '{this.GetType()}' is not readable");
      }

      if (_dependencyPropertyValues.ContainsKey(property))
      {
        return _dependencyPropertyValues[property];
      }

      return property.Metada.DefaultValue;

    }

    public void SetValue(DependencyProperty property, object value)
    {
      if (!property.Metada.IsWritable)
      {
        throw new InvalidOperationException($"{property.PropertyName} on '{this.GetType()}' is not writeable");
      }

      if(!_dependencyPropertyValues.ContainsKey(property))
      {
        var newProperty = DependencyObjectProvider.RegisterCommon(this.GetType(), property);
        _dependencyPropertyValues[property] = newProperty.Metada.DefaultValue;
      }

      if (_dependencyPropertyValues.ContainsKey(property))
      {
        object oldValue = _dependencyPropertyValues[property];
        bool fireChanged = property.Metada.DefaultValue != value && oldValue != value;

        _dependencyPropertyValues[property] = value;

        if (fireChanged)
        {
          FirePropertyChanged(property, value, _dependencyPropertyValues[property]); 
        }
      }
    }

    private void FirePropertyChanged(DependencyProperty dependencyProperty, object newValue, object oldValue)
    {
      DependnecyPropertyChanged?.Invoke(this, new PropertyChangedArgs(oldValue, newValue, dependencyProperty));
    }

    public EventHandler<PropertyChangedArgs> DependnecyPropertyChanged { get; set; }
  }

  public class PropertyChangedArgs : EventArgs
  {
    public PropertyChangedArgs(object oldValue, object newValue, DependencyProperty property)
    {
      OldValue = oldValue;
      NewValue = newValue;
      Property = property;
    }
    public object OldValue { get; }

    public object NewValue { get; }

    public DependencyProperty Property { get; }
  }


}