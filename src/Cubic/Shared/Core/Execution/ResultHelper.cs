using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Execution
{
  public static class ResultHelper
  {
    public static Status Ok()
    {
      return Status.SuccessStatus;
    }

    public static Status Error()
    {
      return Status.ErrorStatus;
    }

    public static Status<TError> Error<TError>(TError error)
    {
      return error;
    }



    public static Result<TResult> Ok<TResult>(TResult result)
    {
      return result;
    }

    public static Result<TResult> Error<TResult>()
    {
      return Result<TResult>.ErrorResult;
    }

    public static Result<TResult, TError> Ok<TResult, TError>(TResult result)
    {
      return result;
    }

    public static Result<TResult, TError> Error<TResult, TError>(TError error)
    {
      return error;
    }
  }
}
