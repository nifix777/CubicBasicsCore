using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Security
{
  [DebuggerNonUserCode]
  [DebuggerStepThrough]
  [Serializable]
  public class UnauthorizedCodeAccessException : AppException
  {
    private const int ErrorNumber = 1000;

    internal UnauthorizedCodeAccessException(SerializationInfo info, StreamingContext context ) : base(info, context)
    {
      
    }

    internal UnauthorizedCodeAccessException(string message, Exception innerException) : base(ErrorNumber, message, innerException)
    {
    }

    internal UnauthorizedCodeAccessException(string message) : base(ErrorNumber, message)
    {

    }
  }
}