using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Cubic.Core.Collections;

namespace Cubic.Core
{
  public class ChangeTracking: BindableBase, IChangeTracking
  {

    private IDictionary<string, List<object>> _propertyHistory  = new System.Collections.Generic.Dictionary<string, List<object>>();

    public ChangeTracking()
    {
      this.PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
      var key = propertyChangedEventArgs.PropertyName;

      if ( !_propertyHistory.ContainsKey(key))
      {
        _propertyHistory[key] = new List<object>();
      }
    }

    public IEnumerable<string> ChangedProperties => _propertyHistory.Keys;
    public void Track(string propName, object oldValue, object newValue)
    {
      this.OnPropertyChanged(this, new PropertyChangedEventArgs(propName));

      if (oldValue != newValue)
      {
        _propertyHistory[propName].Add(oldValue);
      }
    }

    public virtual void RevertChanges()
    {
      throw new NotImplementedException();
    }

    protected IEnumerable<object> GetProprtyHistory(string propname)
    {
      return _propertyHistory[propname];
    } 

    public void AcceptChanges()
    {
      _propertyHistory.Clear();
    }

    public bool IsChanged => _propertyHistory.Keys.Any();
  }
}