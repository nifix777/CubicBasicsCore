using System;

namespace Cubic.Core.Validation
{
  [AttributeUsage(AttributeTargets.Class)]
  public abstract class InstanceValidatorBase : Attribute
  {
    public abstract void Validate(CustomValidationContext context);
  }
}