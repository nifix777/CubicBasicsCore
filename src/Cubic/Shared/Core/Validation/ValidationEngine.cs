using System;
using System.Collections.Generic;
using System.Linq;

namespace Cubic.Core.Validation
{
  public class ValidationEngine : IDisposable
  {
    
    private object _validationInstance;

    private IEnumerable<InstanceValidatorBase> _instanceValidators;
    private IEnumerable<PropertyValidationAttributeBase> _propValidators;

    public CustomValidationContext Context { get; private set; }

    public void Validate<T>(T instance) where T : class
    {
      _validationInstance = instance;

      Context = new CustomValidationContext(instance);

      var props = typeof( T ).GetProperties().Where( p => p.GetCustomAttributes( typeof( PropertyValidationAttributeBase ) , true ).Any() );

      foreach ( var propertyInfo in props )
      {
        Context.SetProperty( propertyInfo.Name , propertyInfo.GetValue( instance ) );
        _propValidators =
          propertyInfo.GetCustomAttributes( typeof( PropertyValidationAttributeBase ) , true )
            .OfType<PropertyValidationAttributeBase>();

        foreach ( var propValidator in _propValidators )
        {
          propValidator.Validate( Context );
        }
      }


      if (!Context.IsValid)
      {
        return;
      }

      _instanceValidators = typeof( T ).GetCustomAttributes( typeof( InstanceValidatorBase ) , true ).OfType<InstanceValidatorBase>();

      foreach ( var instanceValidator in _instanceValidators )
      {
        instanceValidator.Validate( Context );
      }
    }

    public void Dispose()
    {
      Context = null;
      _instanceValidators = Enumerable.Empty<InstanceValidatorBase>();
      _propValidators = Enumerable.Empty<PropertyValidationAttributeBase>();
    }
  }
}