using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cubic.Shared.Core.Diagnostics
{
  public class Cli
  {
    public static async Task<int> Wrap(string cmd, string args = null, Action<string> pipe = null, CancellationToken cancellationToken = default(CancellationToken))
    {
      var tcs = new TaskCompletionSource<int>();

      try
      {
        var start = new ProcessStartInfo(cmd, args);

        var proc = new Process();
        proc.StartInfo = start;
        if(pipe != null)
        {
          proc.StartInfo.RedirectStandardOutput = true;
          proc.OutputDataReceived += (s, data) => pipe(data.Data);
        }
        
        proc.EnableRaisingEvents = true;
        proc.Exited += (s, data) => tcs.SetResult(((Process)s).ExitCode);
        proc.Start();
        if (pipe != null) proc.BeginOutputReadLine();

      }
      catch ( Exception ex)
      {
        tcs.SetException(ex);
      }

      using (cancellationToken.Register(() => tcs.TrySetCanceled()))
      {
        return await tcs.Task.ConfigureAwait(false);
      }
      //return tcs.Task;
      
    }
  }
}
