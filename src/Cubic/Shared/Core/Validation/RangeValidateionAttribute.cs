using System;

namespace Cubic.Core.Validation
{
  [AttributeUsage(AttributeTargets.Property)]
  public class InRangeValidateionAttribute : PropertyValidationAttributeBase
  {
    private int _min;

    private int _max;

    public InRangeValidateionAttribute( int min, int max)
    {
      _min = min;
      _max = max;
    }
    public override void Validate(CustomValidationContext context)
    {
      if (_min >= context.CurrentPropertyValue.ToInt32() | _max <= context.CurrentPropertyValue.ToInt32())
      {
        context.AddError(string.Format("{0} is out of Range({1} - {2})", context.CurrentPropertyName, _min, _max), key: context.CurrentPropertyName);
      }
    }
  }

  public class PositiveOrZeroNumberAttribute : InRangeValidateionAttribute
  {
    public PositiveOrZeroNumberAttribute(int max = int.MaxValue) : base(0, max)
    {
      
    }
  }

  public class PastOrPresentAttribute : PropertyValidationAttributeBase
  {

    public override void Validate(CustomValidationContext context)
    {
      var current = context.CurrentPropertyValue.ToDateTime();
      if (current <= DateTime.Now)
      {
        context.AddError(string.Format("{0} is out of Range.", current), key: context.CurrentPropertyName);
      }
    }
  }

  public class FutureOrPresentAttribute : PropertyValidationAttributeBase
  {

    public override void Validate(CustomValidationContext context)
    {
      var current = context.CurrentPropertyValue.ToDateTime();
      if (current >= DateTime.Now)
      {
        context.AddError(string.Format("{0} is out of Range.", current), key: context.CurrentPropertyName);
      }
    }
  }
}