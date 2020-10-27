using System;
using System.Runtime.Serialization;

namespace Cubic.Core.Diagnostics
{
  [Serializable]
  public class AppException : Exception
  {

    public int ErrorCode { get; private set; }

    public AppException(int errorCode)
    {
      ErrorCode = errorCode;
    }

    public AppException(int errorCode, string message) : base(message)
    {
      ErrorCode = errorCode;
    }

    public AppException(int errorCode, string message, Exception innerException) : base(message, innerException)
    {
      ErrorCode = errorCode;
    }

    protected AppException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
      ErrorCode = info.GetInt32(nameof(ErrorCode));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue(nameof(ErrorCode), ErrorCode);
      base.GetObjectData(info, context);

    }
  }
}