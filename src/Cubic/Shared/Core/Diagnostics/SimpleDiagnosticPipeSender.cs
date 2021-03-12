
using Cubic.Shared.Core.Tools;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace Cubic.Shared.Core.Cubic.Shared.Core.Diagnostics
{
  public class SimpleDiagnosticPipeSender : IDisposable
  {

    private const int CopyToBufferSize = 81920;

    readonly AnonymousPipeClientStream _server;

    readonly Thread _thread;
    readonly bool _interProcess;
    readonly ObjectSerializer _serializer;
    readonly CancellationTokenSource _tokenSource;

    readonly ConcurrentQueue<DiagnosticEntry> _sendqueue;
    public SimpleDiagnosticPipeSender(string pipeHandlerName, ObjectSerializer serializer)
    {
      _server = new AnonymousPipeClientStream(PipeDirection.In, pipeHandlerName);
      _sendqueue = new ConcurrentQueue<DiagnosticEntry>();
      _serializer = serializer;
      _thread = new Thread(Run)
      {
        IsBackground = true
      };
      _thread.Start();
    }


    public string PipeName { get; }


    public void Dispose()
    {
      _thread.Join();
      _server.Dispose();
    }

    public void Log(DiagnosticEntry entry)
    {
      _sendqueue.Enqueue(entry);
    }

    void Run()
    {
      try
      {
        var msgBuffer = new MemoryStream(CopyToBufferSize * 4);
        while (_sendqueue.TryDequeue(out DiagnosticEntry entry) && !_tokenSource.IsCancellationRequested)
        {
          _serializer.Serialize(msgBuffer, entry, typeof(DiagnosticEntry), _tokenSource.Token);
          var buffer = msgBuffer.ToArray();
          _server.Write(buffer, 0, buffer.Length);
        }
      }
      catch (Exception ex)
      {
        //Trace.TraceError(ex, $"While {nameof(SimpleLogPipeReceiver)}.Run.");
        Trace.TraceError($" {ex} : While {nameof(SimpleLogPipeReceiver)}.Run.");
      }
    }
  }
}
