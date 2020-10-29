using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Cubic.Core.Security
{
  [DebuggerNonUserCode]
  [DebuggerStepThrough]
  [Serializable]
  public sealed class UnauthorizedCodeAccessException : Exception
  {
    public UnauthorizedCodeAccessException()
    {
      HResult = 1000;
    }

    internal UnauthorizedCodeAccessException(SerializationInfo info, StreamingContext context ) : base(info, context)
    {
      
    }

    internal UnauthorizedCodeAccessException(string message, Exception innerException) : base(message, innerException)
    {
    }

    internal UnauthorizedCodeAccessException(string message) : base(message)
    {

    }
  }
}