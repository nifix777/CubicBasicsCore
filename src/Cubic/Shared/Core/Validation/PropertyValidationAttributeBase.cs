using System;

namespace Cubic.Core.Validation
{
  [AttributeUsage(AttributeTargets.Property)]
  public abstract class PropertyValidationAttributeBase : System.Attribute
  {
    public abstract void Validate(CustomValidationContext context);
  }
}