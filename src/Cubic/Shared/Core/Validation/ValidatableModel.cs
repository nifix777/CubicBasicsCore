using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cubic.Core.Validation
{
  /// <summary>
  /// Base class to provide an Validationable Model to support <see cref="INotifyDataErrorInfo"/> and <see cref="INotifyPropertyChanged"/>
  /// 
  /// <example>
  /// XAML:
  /// <TextBox Text="{Binding UserName, ValidatesOnNotifyDataErrors=True}"/>
  /// </example>
  /// 
  /// </summary>
  /// <seealso cref="System.ComponentModel.INotifyDataErrorInfo" />
  /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
  public class ValidatableModel : INotifyDataErrorInfo, INotifyPropertyChanged
  {
    private object _lock = new object();

    private ConcurrentDictionary<string , List<string>> _errors = new ConcurrentDictionary<string , List<string>>();

    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Gets or sets the on validation handlers.
    /// Called after the classic Validation with DataAnnotaions. For Support of Validation outside of the Model.
    /// </summary>
    /// <value>
    /// The on validation.
    /// </value>
    public Action<ValidatableModel, ValidationContext, IList<ValidationResult>> OnValidation { get; set; } 

    public void RaisePropertyChanged( string propertyName )
    {
      var handler = PropertyChanged;
      handler?.Invoke( this , new PropertyChangedEventArgs( propertyName ) );
      ValidateAsync();
    }

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public void OnErrorsChanged( string propertyName )
    {
      var handler = ErrorsChanged;
      handler?.Invoke( this , new DataErrorsChangedEventArgs( propertyName ) );
    }

    public IEnumerable GetErrors( string propertyName )
    {
      List<string> errorsForName;
      _errors.TryGetValue( propertyName , out errorsForName );
      return errorsForName;
    }

    public bool HasErrors
    {
      get { return _errors.Any( kv => kv.Value != null && kv.Value.Count > 0 ); }
    }

    public Task ValidateAsync()
    {
      return Task.Run( () => Validate() );
    }

    private void ValidationHandler(ValidatableModel model, ValidationContext context, IList<ValidationResult> validationResults)
    {
      OnValidation?.Invoke(model, context, validationResults);
    }
    public void Validate()
    {
      lock ( _lock )
      {
        var validationContext = new ValidationContext( this , null , null );
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject( this , validationContext , validationResults , true );

        ValidationHandler(this, validationContext, validationResults);

        foreach ( var kv in _errors.ToList() )
        {
          if ( validationResults.All( r => r.MemberNames.All( m => m != kv.Key ) ) )
          {
            List<string> outLi;
            _errors.TryRemove( kv.Key , out outLi );
            OnErrorsChanged( kv.Key );
          }
        }

        var q = from r in validationResults
                from m in r.MemberNames
                group r by m into g
                select g;

        foreach ( var prop in q )
        {
          var messages = prop.Select( r => r.ErrorMessage ).ToList();

          if ( _errors.ContainsKey( prop.Key ) )
          {
            List<string> outLi;
            _errors.TryRemove( prop.Key , out outLi );
          }
          _errors.TryAdd( prop.Key , messages );
          OnErrorsChanged( prop.Key );
        }
      }
    }
  }

}