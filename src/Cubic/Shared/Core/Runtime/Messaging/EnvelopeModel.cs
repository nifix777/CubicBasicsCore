using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Runtime.Messaging
{

  public class EnvelopeModel
  {
    public EnvelopeModel(Envelope envelope, Exception exception = null)
    {
      Headers = envelope.Headers;
      CorrelationId = envelope.Id;
      ParentId = envelope.ParentId;
      Description = envelope.ToString();
      Source = envelope.Source;
      Destination = envelope.Destination;
      Message = envelope.Message;
      MessageType = new MessageTypeModel(envelope.Message?.GetType());
      Attempts = envelope.Attempts;
      Exception = exception?.Message;
      StackTrace = exception?.ToString();
      HasError = exception != null ? true : false;
    }

    public IDictionary<string, string> Headers { get; }

    public Guid CorrelationId { get; }
    public Guid ParentId { get; }
    public MessageTypeModel MessageType { get; }
    public string Description { get; }
    public string Source { get; }
    public Uri Destination { get; }
    public object Message { get; }
    public int Attempts { get; }
    public bool HasError { get; }
    public string Exception { get; set; }
    public string StackTrace { get; set; }
  }
}
