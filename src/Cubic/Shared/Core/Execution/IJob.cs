using System;
using System.Threading;
using System.Threading.Tasks;
using Cubic.Core.Threading;

namespace Cubic.Core.Execution
{
  public interface IJob
  {
    string Name { get; }
    Task<JobResult> RunAsync(CancellationToken cancellationToken = default(CancellationToken));
  }

  public static class JobExtensions
  {
    public static async Task<JobResult> TryRunAsync(this IJob job, CancellationToken cancellationToken = default(CancellationToken))
    {
      try
      {
        return await job.RunAsync(cancellationToken).AnyContext();
      }
      catch (Exception ex)
      {
        return JobResult.FromException(ex);
      }
    }
  }
}