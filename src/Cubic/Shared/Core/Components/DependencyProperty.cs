using System;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Components
{
  public class DependencyProperty : IEquatable<DependencyProperty>
  {
    public PropertyMetada Metada { get; private set; }

    public string PropertyName { get; }

    public Type OwnerType { get; }

    public Type PropertyType { get; }


    public static DependencyProperty Register(string name, Type propertyType, Type ownerType)
    {
      return Register(name, propertyType, ownerType, null);
    }

    public static DependencyProperty Register(string name, Type propertyType, Type ownerType, PropertyMetada typeMetadata)
    {
      RegisterParameterValidation(name, propertyType, ownerType);

      // Register an attached property
      PropertyMetada defaultMetadata = null;
      if (typeMetadata == null)
      {
        defaultMetadata = CreateFromPropertyType(propertyType);
      }

      DependencyProperty property = RegisterCommon(name, propertyType, ownerType, defaultMetadata);

      if (typeMetadata != null)
      {
        // Apply type-specific metadata to owner type only
        property.OverrideMetadata(ownerType, typeMetadata);
      }

      return property;
    }

    private static PropertyMetada CreateFromPropertyType(Type propertyType)
    {
      return new PropertyMetada(defaultValue: propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null);

    }

    private void OverrideMetadata(Type ownerType, PropertyMetada typeMetadata)
    {
      Metada = typeMetadata;
    }

    private static DependencyProperty RegisterCommon(string name, Type propertyType, Type ownerType, PropertyMetada metadata)
    {
      var dpe = new DependencyProperty(propertyType, metadata, ownerType, name);

      DependencyObjectProvider.RegisterCommon(ownerType, dpe);

      return dpe;
    }

    private static void RegisterParameterValidation(string name, Type propertyType, Type ownerType)
    {
      Guard.AgainstNullOrEmpty(nameof(name), name);
      Guard.AgainstNull(propertyType, nameof(propertyType));
      Guard.AgainstNull(ownerType, nameof(ownerType));
    }


    public DependencyProperty(Type propertyType, PropertyMetada metada, Type ownerType, string propertyName)
    {
      PropertyType = propertyType;
      Metada = metada;
      OwnerType = ownerType;
      PropertyName = propertyName;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashCode = (PropertyType != null ? PropertyType.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (OwnerType != null ? OwnerType.GetHashCode() : 0);
        hashCode = (hashCode * 397) ^ (PropertyName != null ? PropertyName.GetHashCode() : 0);
        return hashCode;
      }
    }

    public bool Equals(DependencyProperty other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return PropertyType == other.PropertyType && OwnerType == other.OwnerType && string.Equals(PropertyName, other.PropertyName);
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((DependencyProperty) obj);
    }
  }
}