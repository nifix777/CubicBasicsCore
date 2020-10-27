using Cubic.Core.Cubic.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Core.Diagnostics
{
  public static class ProccessExtensions
  {
    private static readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(30);
    public static Task<int> GetAwaiter(this Process process)
    {
      var tcs = new TaskCompletionSource<int>();
      process.EnableRaisingEvents = true;
      process.Exited += (s, e) => tcs.TrySetResult(process.ExitCode);
      if (process.HasExited) tcs.TrySetResult(process.ExitCode);
      return tcs.Task;
    }

    public static void KillTree(this Process process)
    {
      process.KillTree(_defaultTimeout);
    }

    public static void KillTree(this Process process, TimeSpan timeout)
    {
      string stdout;
      RunProcessAndWaitForExit(
          "taskkill",
          $"/T /F /PID {process.Id}",
          timeout,
          out stdout);
    }

    public static int RunProcessAndWaitForExit(string fileName, string arguments, TimeSpan timeout, out string stdout)
    {
      var startInfo = new ProcessStartInfo
      {
        FileName = fileName,
        Arguments = arguments,
        RedirectStandardOutput = true,
        UseShellExecute = false
      };

      var process = Process.Start(startInfo);

      stdout = null;
      if (process.WaitForExit((int)timeout.TotalMilliseconds))
      {
        stdout = process.StandardOutput.ReadToEnd();
      }
      else
      {
        process.Kill();
      }

      return process.ExitCode;
    }

    public static async Task<int> WaitForExitAsync(this Process process, CancellationToken cancellationToken = default(CancellationToken))
    {
      Guard.AgainstNull(process, nameof(process));

      var tcs = new TaskCompletionSource<int>();
      EventHandler exitHandler = (s, e) =>
      {
        tcs.TrySetResult(process.ExitCode);
      };
      try
      {

        process.Exited += exitHandler;
        process.EnableRaisingEvents = true;

        if (process.HasExited)
        {
          // Allow for the race condition that the process has already exited.
          tcs.TrySetResult(process.ExitCode);
        }

        using (cancellationToken.Register(() => tcs.TrySetCanceled()))
        {
          return await tcs.Task.ConfigureAwait(false);
        }
      }
      finally
      {
        process.Exited -= exitHandler;
      }
    }

    public static async Task<ProcessResult> RunAsync(
    string filename,
    string arguments,
    string workingDirectory = null,
    bool throwOnError = true,
    IDictionary<string, string> environmentVariables = null,
    Action<string> outputDataReceived = null,
    Action<string> errorDataReceived = null,
    Action<int> onStart = null,
    Action<int> onStop = null,
    CancellationToken cancellationToken = default)
    {

      var startInfo = new ProcessStartInfo()
      {
        FileName = filename,
        Arguments = arguments,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = false, // true for NonWindows Platform,
        WindowStyle = ProcessWindowStyle.Hidden

      };

      using (Process process = new Process()
      {
        StartInfo = startInfo,
        EnableRaisingEvents = true
      })
      {
        if (workingDirectory != null)
        {
          process.StartInfo.WorkingDirectory = workingDirectory;
        }

        if (environmentVariables != null)
        {
          foreach (var kvp in environmentVariables)
          {
            process.StartInfo.EnvironmentVariables.Add(kvp.Key, kvp.Value);
          }
        }

        var outputBuilder = new StringBuilder();
        process.OutputDataReceived += (s, e) =>
        {
          if (e.Data != null)
          {
            if (outputDataReceived != null)
            {
              outputDataReceived.Invoke(e.Data);
            }
            else
            {
              outputBuilder.AppendLine(e.Data);
            }
          }
        };

        var errorBuilder = new StringBuilder();
        process.ErrorDataReceived += (s, e) =>
        {
          if (e.Data != null)
          {
            if (errorDataReceived != null)
            {
              errorDataReceived.Invoke(e.Data);
            }
            else
            {
              errorBuilder.AppendLine(e.Data);
            }
          }
        };

        var processLifetimeTask = new TaskCompletionSource<ProcessResult>();

        process.Exited += (s, e) =>
        {
          if (throwOnError && process.ExitCode != 0)
          {
            processLifetimeTask.TrySetException(new InvalidOperationException($"Command {filename} {arguments} returned exit code {process.ExitCode}"));
          }
          else
          {
            processLifetimeTask.TrySetResult(new ProcessResult(outputBuilder.ToString(), errorBuilder.ToString(), process.ExitCode));
          }
        };

        process.Start();
        onStart?.Invoke(process.Id);

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        var cancelledTcs = new TaskCompletionSource<object>();

        using (var cts = cancellationToken.Register(() => cancelledTcs.TrySetResult(null)))
        {
          var result = await Task.WhenAny(processLifetimeTask.Task, cancelledTcs.Task);

          if (result == cancelledTcs.Task)
          {
            if (!process.CloseMainWindow())
            {
              process.Kill();
            }

            if (!process.HasExited)
            {
              var cancel = new CancellationTokenSource();
              await Task.WhenAny(processLifetimeTask.Task, Task.Delay(TimeSpan.FromSeconds(5), cancel.Token));
              cancel.Cancel();

              if (!process.HasExited)
              {
                process.Kill();
              }
            }
          }

          var processResult = await processLifetimeTask.Task;
          onStop?.Invoke(processResult.ExitCode);
          return processResult;
        }


      }

      //if (workingDirectory != null)
      //{
      //  process.StartInfo.WorkingDirectory = workingDirectory;
      //}

      //if (environmentVariables != null)
      //{
      //  foreach (var kvp in environmentVariables)
      //  {
      //    process.StartInfo.EnvironmentVariables.Add(kvp);
      //  }
      //}

      //var outputBuilder = new StringBuilder();
      //process.OutputDataReceived += (_, e) =>
      //{
      //  if (e.Data != null)
      //  {
      //    if (outputDataReceived != null)
      //    {
      //      outputDataReceived.Invoke(e.Data);
      //    }
      //    else
      //    {
      //      outputBuilder.AppendLine(e.Data);
      //    }
      //  }
      //};

      //var errorBuilder = new StringBuilder();
      //process.ErrorDataReceived += (_, e) =>
      //{
      //  if (e.Data != null)
      //  {
      //    if (errorDataReceived != null)
      //    {
      //      errorDataReceived.Invoke(e.Data);
      //    }
      //    else
      //    {
      //      errorBuilder.AppendLine(e.Data);
      //    }
      //  }
      //};

      //var processLifetimeTask = new TaskCompletionSource<ProcessResult>();

      //process.Exited += (_, e) =>
      //{
      //  if (throwOnError && process.ExitCode != 0)
      //  {
      //    processLifetimeTask.TrySetException(new InvalidOperationException($"Command {filename} {arguments} returned exit code {process.ExitCode}"));
      //  }
      //  else
      //  {
      //    processLifetimeTask.TrySetResult(new ProcessResult(outputBuilder.ToString(), errorBuilder.ToString(), process.ExitCode));
      //  }
      //};

      //process.Start();
      //onStart?.Invoke(process.Id);

      //process.BeginOutputReadLine();
      //process.BeginErrorReadLine();

      //var cancelledTcs = new TaskCompletionSource<object?>();
      //await using var _ = cancellationToken.Register(() => cancelledTcs.TrySetResult(null));

      //var result = await Task.WhenAny(processLifetimeTask.Task, cancelledTcs.Task);

      //if (result == cancelledTcs.Task)
      //{
      //  if (!IsWindows)
      //  {
      //    sys_kill(process.Id, sig: 2); // SIGINT
      //  }
      //  else
      //  {
      //    if (!process.CloseMainWindow())
      //    {
      //      process.Kill();
      //    }
      //  }

      //  if (!process.HasExited)
      //  {
      //    var cancel = new CancellationTokenSource();
      //    await Task.WhenAny(processLifetimeTask.Task, Task.Delay(TimeSpan.FromSeconds(5), cancel.Token));
      //    cancel.Cancel();

      //    if (!process.HasExited)
      //    {
      //      process.Kill();
      //    }
      //  }
      //}

      //var processResult = await processLifetimeTask.Task;
      //onStop?.Invoke(processResult.ExitCode);
      //return processResult;
    }
  }
}