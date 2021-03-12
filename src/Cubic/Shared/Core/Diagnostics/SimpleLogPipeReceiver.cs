
using Cubic.Shared.Core.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace Cubic.Shared.Core.Cubic.Shared.Core.Diagnostics
{
  public class SimpleLogPipeReceiver : IDisposable
  {

    private const int CopyToBufferSize = 81920;

    readonly AnonymousPipeServerStream _server;

    readonly Thread _thread;
    readonly bool _interProcess;
    readonly ObjectSerializer _serializer;
    readonly CancellationTokenSource _tokenSource;
    public SimpleLogPipeReceiver(bool interProcess, ObjectSerializer serializer)
    {
      _interProcess = interProcess;
      var inherit = interProcess ? HandleInheritability.Inheritable : HandleInheritability.None;
      _server = new AnonymousPipeServerStream(PipeDirection.In, inherit);
      _serializer = serializer;
      PipeName = _server.GetClientHandleAsString();
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

    public EventHandler<DiagnosticEntry> OnEntry;

    void Run()
    {
      try
      {
        if (_interProcess) _server.DisposeLocalCopyOfClientHandle();


        var msgBuffer = new MemoryStream(CopyToBufferSize * 4);
        do
        {
          var buffer = new byte[CopyToBufferSize];
          _server.Read(buffer, 0, buffer.Length);
          msgBuffer.Write(buffer, 0, buffer.Length);
        } while (!_server.IsMessageComplete);

        var msg = (DiagnosticEntry)_serializer.Deserialize(msgBuffer, typeof(DiagnosticEntry), _tokenSource.Token);

        var handler = OnEntry;
        handler?.Invoke(this, msg);
      }
      catch (Exception ex)
      {
        //Trace.TraceError(ex, $"While {nameof(SimpleLogPipeReceiver)}.Run.");
        Trace.TraceError($" {ex} : While {nameof(SimpleLogPipeReceiver)}.Run.");
      }
    }
  }
}
