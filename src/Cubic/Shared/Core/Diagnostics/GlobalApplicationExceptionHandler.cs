using Cubic.Core.Collections;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace Cubic.Core.Diagnostics
{
  public static class GlobalApplicationExceptionHandler
  {
    private static object _syncObject = new object();

    public static EventHandler<ExceptionHandlerEventArgs> GlobalApplicationException { get; set; }

    public static void Handle(object sender, Exception exception)
    {
      var dispatchinfo = ExceptionDispatchInfo.Capture(exception);

      lock (_syncObject)
      {
        Trace.TraceError(dispatchinfo.SourceException.ToString());

        //GlobalApplicationException?.Invoke(sender, new ExceptionHandlerEventArgs(dispatchinfo));

        var invokes = GlobalApplicationException?.GetInvocationList();

        if (invokes != null && invokes.Any())
        {
          var args = new ExceptionHandlerEventArgs(dispatchinfo);
          foreach (var handler in invokes)
          {
            handler.DynamicInvoke(args);

            if (args.IsHandled)
            {
              break;
            }
          }

          if (!args.IsHandled) args.ExceptionDispatchInfo.Throw();
        }

      }
    }

    public static void UseGlobalExeptionHandler()
    {
      GlobalApplicationExceptionHandler.GlobalApplicationException += (sender, args) =>
        Trace.TraceError(args.ExceptionDispatchInfo.SourceException.ToString());
    }
  }

  public class ExceptionHandlerEventArgs : EventArgs
  {
    public readonly ExceptionDispatchInfo ExceptionDispatchInfo;

    public ExceptionHandlerEventArgs(ExceptionDispatchInfo exceptionDispatchInfo)
    {
      ExceptionDispatchInfo = exceptionDispatchInfo;
    }

    public bool IsHandled { get; set; }
  }


}