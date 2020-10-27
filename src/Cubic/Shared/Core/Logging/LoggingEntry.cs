using System;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Logging
{
  /// <summary>
  /// 
  /// </summary>
  [Serializable]
  public class LoggingEntry
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingEntry"/> class.
    /// </summary>
    public LoggingEntry()
    {
        this.Time = DateTime.Now;
        this.Component = "Application";
        this.Severity = MessageSeverity.Verbose;
        this.Message = String.Empty;
    }

    public LoggingEntry(string message) : this()
    {
        this.Message = message;
    }

    public LoggingEntry(string message, MessageSeverity severity) : this(message)
    {
        this.Severity = severity;
    }

    public LoggingEntry(string message, Exception exception) : this(string.Format("{0}:{1}", message, exception.GetAllMessages()))
    {
        this.Severity = MessageSeverity.Error;
    }

    public LoggingEntry(Exception exception) : this(exception.GetAllMessages())
    {
        this.Severity = MessageSeverity.Error;
    }

    public LoggingEntry(Exception exception, MessageSeverity severity) : this(exception.GetAllMessages())
    {
        this.Severity = severity;
    }

    public DateTime Time { get; set; }

    public MessageSeverity Severity { get; set; }

    public string Component { get; set; }

    public string Module { get; set; }

    public int ProcessId { get; set; }

    public string Message { get; set; }       
  }
}