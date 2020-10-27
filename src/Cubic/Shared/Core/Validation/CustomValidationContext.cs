using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cubic.Core.Validation
{
  public class CustomValidationContext
  {
    private IList<ValidationMessage> _validationMessages = new List<ValidationMessage>();

    public CustomValidationContext(object instance)
    {
      InstanceType = instance.GetType();
      Instance = instance;

    }

    public void SetProperty(string name, object value)
    {
      this.CurrentPropertyName = name;
      this.CurrentPropertyValue = value;
    }
     public Type InstanceType { get;  }
     public object Instance { get;  }

    public object CurrentPropertyValue { get; private set; }
    public string CurrentPropertyName { get; private set; }

    public bool HasWarnings => Warnings.Any();
    public bool HasErrors => Errors.Any();
    public bool IsValid => (HasErrors | HasWarnings);

    private IEnumerable<ValidationMessage> Errors => _validationMessages.Where(v => v.Severity == MessageSeverity.Error); 
    private IEnumerable<ValidationMessage> Warnings => _validationMessages.Where(v => v.Severity == MessageSeverity.Warning); 
    private IEnumerable<ValidationMessage> Infos => _validationMessages.Where(v => v.Severity == MessageSeverity.Information); 

    public void AddError(string message, string details = "", string key = "")
    {
      _validationMessages.Add(new ValidationMessage(MessageSeverity.Error, message, details, key));
    }
    public void AddWarning( string message , string details = "" , string key = "" )
    {
      _validationMessages.Add( new ValidationMessage( MessageSeverity.Warning , message , details , key ) );
    }

    public void AddInformation( string message , string details = "" , string key = "" )
    {
      _validationMessages.Add( new ValidationMessage( MessageSeverity.Information , message , details , key ) );
    }

  }

  [Serializable]
  public class ValidationMessage
  {
    public ValidationMessage()
    {
      
    }

    public ValidationMessage(MessageSeverity severity, string message , string details = "" , string key = "" )
    {
      Severity = severity;
      Message = message;
      Details = details;
      Key = key;
    }
    public MessageSeverity Severity { get; set; }

    public string Message { get; set; }
    public string Details { get; set; }
    public string Key { get; set; }

    public override string ToString()
    {
      StringBuilder text = new StringBuilder();
      text.AppendLine(Severity.ToString());
      text.AppendLine(string.Format(" - {0}", Message));
      text.AppendLine(string.Format(" - {0}", Details));
      text.AppendLine(string.Format(" - {0}", Key));
      return text.ToString();
    }
  }
}