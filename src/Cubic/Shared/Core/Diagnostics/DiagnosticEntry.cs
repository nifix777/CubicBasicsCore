using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cubic.Shared.Core.Cubic.Shared.Core.Diagnostics
{
  [DataContract]
  public class DiagnosticEntry
  {
    [DataMember]
    public int ProcessId { get; set; }

    [DataMember]
    public string ProcessName { get; set; }

    [DataMember]
    public string Component { get; set; }

    [DataMember]
    public MessageSeverity Severity { get; set; }

    [DataMember]
    public Guid? TraceId { get; set; }

    [DataMember]
    public Exception Exception { get; set; }

    [DataMember]
    public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();
  }
}
