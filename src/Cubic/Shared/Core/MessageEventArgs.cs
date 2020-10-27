using System;

namespace Cubic.Core
{
  public class MessageEventArgs : EventArgs
  {
    public readonly MessageSeverity Severity;

    public readonly string Source;

    public readonly string Message;

    public MessageEventArgs(string message) : this(message, MessageSeverity.Verbose)
    {

    }

    public MessageEventArgs(string message, MessageSeverity severity, string source = null )
    {
      Message = message;
      Source = source;
      Severity = severity;
    }
  }
}