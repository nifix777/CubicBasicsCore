using System;
using System.Reflection;

namespace Cubic.Core.Components
{
  public class PropertyMetada
  {
    public PropertyMetada(bool isReadable = true, bool isWritable = true, object defaultValue = null, Action<DependencyObject, PropertyChangedArgs> propertyChangedCallback = null)
    {
      DefaultValue = defaultValue;
      IsReadable = isReadable;
      IsWritable = isWritable;
      PropertyChangedCallback = propertyChangedCallback;
    }

    public object DefaultValue { get; }

    public bool IsReadable { get; }
    public bool IsWritable { get; }

    public Action<DependencyObject, PropertyChangedArgs> PropertyChangedCallback { get; }
  }
}