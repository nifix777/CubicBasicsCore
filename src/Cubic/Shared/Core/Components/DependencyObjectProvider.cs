using System;
using System.Collections.Generic;
using System.Linq;

namespace Cubic.Core.Components
{
  public static class DependencyObjectProvider
  {

    private static Dictionary<Type, HashSet<DependencyProperty>> _registerdProperties = new Dictionary<Type, HashSet<DependencyProperty>>();

    public static DependencyProperty RegisterCommon(Type ownerType, DependencyProperty dependencyProperty)
    {
      if (!_registerdProperties.ContainsKey(ownerType))
      {
        _registerdProperties[ownerType] = new HashSet<DependencyProperty>();
      }

      var properties = _registerdProperties[ownerType];

      if (!properties.Contains(dependencyProperty))
      {
        properties.Add(dependencyProperty);
      }

      return dependencyProperty;
    }

    public static IEnumerable<DependencyProperty> GetDependencyProperties(Type targetType)
    {
      if (_registerdProperties.ContainsKey(targetType))
      {
        return _registerdProperties[targetType];
      }

      return Enumerable.Empty<DependencyProperty>();
    }

  }
}