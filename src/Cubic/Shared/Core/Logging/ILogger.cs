using System;
using System.Diagnostics;

namespace Cubic.Core.Logging
{
  public interface ILogger
  {
    /// <summary>
    /// Logs the given message.
    /// </summary>
    /// <param name="severity">The severity.</param>
    void Message(MessageSeverity severity, string message);

    /// <summary>
    /// Logs the specified entry.
    /// </summary>
    /// <param name="entry">The entry.</param>
    void Message(LoggingEntry entry);

    /// <summary>
    /// Logs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    void Information(string message);

    /// <summary>
    /// Logs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="parameter">The parameter.</param>
    void Information(string message, params object[] parameter);

    /// <summary>
    /// Logs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    void Verbose(string message);

    /// <summary>
    /// Logs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="parameter">The parameter.</param>
    void Verbose(string message, params object[] parameter);

    /// <summary>
    /// Logs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    void Warning(string message);

    /// <summary>
    /// Logs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="parameter">The parameter.</param>
    void Warning(string message, params object[] parameter);

    /// <summary>
    /// Logs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    void Error(string message);

    /// <summary>
    /// Logs the specified error.
    /// </summary>
    /// <param name="error">The error.</param>
    void Error(Exception error);

    /// <summary>
    /// Logs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="parameter">The parameter.</param>
    void Error(string message, params object[] parameter);

    /// <summary>
    /// Logs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    void Critical(string message);

    /// <summary>
    /// Logs the specified error.
    /// </summary>
    /// <param name="error">The error.</param>
    void Critical(Exception error);

    /// <summary>
    /// Logs the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="parameter">The parameter.</param>
    void Critical(string message, params object[] parameter);

    /// <summary>
    /// Logs a START of the specified action.
    /// </summary>
    /// <param name="action">The action.</param>
    void Start(string action);

    /// <summary>
    /// Logs a STOP of the specified action.
    /// </summary>
    /// <param name="action">The action.</param>
    void Stop(string action);

    /// <summary>
    /// Writes a trace transfer message to the trace listeners in the Listeners collection using the specified numeric identifier, message, and related activity identifier. 
    /// </summary>
    /// <param name="id">A numeric identifier for the event.</param>
    /// <param name="message">The trace message to write.</param>
    /// <param name="relatedActivityId">A structure that identifies the related activity.</param>
    void TraceTransfer(int id, string message, Guid relatedActivityId);

    /// <summary>
    /// Writes a trace event message to the trace listeners in the Listeners collection using the specified event type and event identifier. 
    /// </summary>
    /// <param name="eventType">One of the enumeration values that specifies the event type of the trace data.</param>
    /// <param name="id">A numeric identifier for the event.</param>
    void TraceEvent(TraceEventType eventType, int id);

    /// <summary>
    /// Writes a trace event message to the trace listeners in the Listeners collection using the specified event type, event identifier, and message. 
    /// </summary>
    /// <param name="eventType">One of the enumeration values that specifies the event type of the trace data.</param>
    /// <param name="id">A numeric identifier for the event.</param>
    /// <param name="message">The trace message to write.</param>
    void TraceEvent(TraceEventType eventType, int id, string message);

    /// <summary>
    /// Writes a trace event to the trace listeners in the Listeners collection using the specified event type, event identifier, and argument array and format. 
    /// </summary>
    /// <param name="eventType">One of the enumeration values that specifies the event type of the trace data.</param>
    /// <param name="id">A numeric identifier for the event.</param>
    /// <param name="format">A composite format string (see Remarks) that contains text intermixed with zero or more format items, which correspond to objects in the args array.</param>
    /// <param name="args">An object array containing zero or more objects to format.</param>
    void TraceEvent(TraceEventType eventType, int id, string format, params object[] args);

  }
}