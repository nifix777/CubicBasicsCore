using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Cubic.Core.Diagnostics
{
  public static class ExceptionExtension
  {
    //public static bool Verbose = false;

    public static string GetAllMessages(this Exception exception)
    {
      string msg = String.Empty;

      if (exception != null)
      {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat($"{exception.GetType().FullName}:{exception.Message}");
        sb.Append(Constants.Lf);

        if (exception.InnerException != null)
        {
          if (exception.InnerException is AggregateException aggregateException)
          {
            sb.AppendLine(aggregateException.Flatten().InnerException.GetAllMessages());
          }
          else
          {
            sb.AppendLine(exception.InnerException.GetAllMessages());
          }

        }

        msg = sb.ToString();
      }

      return msg;
    }

    public static string GetAllDetails( this Exception exception )
    {
      string msg = String.Empty;

      if ( exception != null )
      {
        StringBuilder sb = new StringBuilder( exception.GetExceptionDetails() );

        sb.Append(Constants.Colon);

        if ( exception.InnerException != null )
        {
          sb.Append( exception.InnerException.GetAllDetails() );
        }

        msg = sb.ToString();
      }

      return msg;
    }

    public static string GetExceptionDetails(this Exception exception)
    {
      var properties = exception.GetType()
        .GetProperties();
      var fields = properties
        .Select(property => new
        {
          Name = property.Name,
          Value = property.GetValue(exception, null)
        })
        .Select(x => string.Format(
          "{0} = {1}",
          x.Name,
          x.Value != null ? x.Value.ToString() : String.Empty
          ));
      return string.Join("\n", fields);
    }

    public static void ReThrow(this Exception exception)
    {
      ExceptionDispatchInfo.Capture(exception).Throw();
    }

    public static Exception Flatten(this Exception exception)
    {
      if(exception is AggregateException aggregateException)
      {
        return aggregateException.Flatten().InnerException;
      }

      return exception;
    }
  }
}
