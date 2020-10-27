using System;
using System.Diagnostics;

namespace Cubic.Core.Diagnostics
{
  public static class ExpetionInfoHelper
  {
    public static ExceptionInfo GetExceptionInfo(Exception exception, bool captureDetails = true)
    {
      var info = new ExceptionInfo()
      {
        When = DateTimeOffset.UtcNow,
        ErrorCode = exception.HResult,
        Message = exception.Message
      };

      var stack = new StackTrace(exception, captureDetails);

      foreach(var frame in stack.GetFrames())
      {
        var stackinfo = new CallStackInfo()
        {
          Method = frame.GetMethod().Name,
          Filename = frame.GetFileName(),
          LineNumber = frame.GetFileLineNumber()
        };

        info.CallStack.Add(stackinfo);

      }
      
      if(exception.InnerException != null)
      {
        info.Inner = GetExceptionInfo(exception.InnerException, captureDetails);
      }

      return info;
    }
  }
   

}
