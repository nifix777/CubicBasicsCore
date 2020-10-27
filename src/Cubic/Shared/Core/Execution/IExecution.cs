using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cubic.Core.Execution
{
  public interface IExecution
  {
     IList<IExecutionParameter> Parameters { get; }

    Task<IExecutionResult> ExecuteAsync();
  }
}