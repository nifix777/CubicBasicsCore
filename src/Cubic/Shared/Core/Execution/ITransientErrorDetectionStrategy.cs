using System;

namespace Cubic.Core.Execution
{
  public interface ITransientErrorDetectionStrategy
  {
    bool IsTransiant(Exception exception);
  }
}