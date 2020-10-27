using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

namespace Cubic.Core.Diagnostics
{
  public class ProcessWatcher : IDisposable
  {
    private Process _process;

    private ProcessStartInfo _startInfo;

    public ProcessWatcher(string path, string args)
    {
      _startInfo = new ProcessStartInfo(path, args);
    }

    public ProcessWatcher(ProcessStartInfo startInfo)
    {
      _startInfo = startInfo;
    }

    public Action<string> OutputDelegate { get; set; } 
    public Action<string> ErrorDelegate { get; set; }

    public bool IsWatching => _process != null;

    public void Watch()
    {

      if (_process != null)
      {
        throw new InvalidOperationException("watching process already");
      }

      _startInfo.RedirectStandardOutput = true;
      _startInfo.RedirectStandardInput = true;
      _startInfo.RedirectStandardError = true;

      _process = new Process();

      _process.StartInfo = _startInfo;
      _process.OutputDataReceived += ProcessOnOutputDataReceived;
      _process.ErrorDataReceived += ProcessOnErrorDataReceived;
      _process.Exited += ProcessOnExited;
    }

    private void ProcessOnExited(object sender, EventArgs eventArgs)
    {
      this.Dispose();
    }

    public void Write(string data)
    {
      if (_process == null)
      {
        throw new InvalidOperationException("no proccess started");
      }

      _process.StandardInput.Write(data);
    }

    private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs)
    {
      ErrorDelegate?.Invoke(dataReceivedEventArgs.Data);
    }

    private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs)
    {
      OutputDelegate?.Invoke(dataReceivedEventArgs.Data);
    }

    public void Dispose()
    {
      if (_process != null)
      {
        _process.ErrorDataReceived -= ProcessOnErrorDataReceived;
        _process.OutputDataReceived -= ProcessOnOutputDataReceived;
        _process.Exited -= ProcessOnExited;
        _process.Dispose();
        _process = null;
      }
    }
  }
}