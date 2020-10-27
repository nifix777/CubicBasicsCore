using System;
using System.Runtime.Serialization;

namespace Cubic.Core.Net.Ftp
{
  [Serializable]
  public class FtpException : Exception
  {
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public FtpException()
    {
    }

    public FtpException(string message, int ftpStatuscode = 0) : base(message)
    {
      FtpStatusCode = ftpStatuscode;
    }

    public FtpException(string message, Exception inner) : base(message, inner)
    {
    }

    protected FtpException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
      FtpStatusCode = info.GetInt32(nameof(FtpStatusCode));
    }



    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue(nameof(FtpStatusCode), FtpStatusCode);
    }

    public int FtpStatusCode { get; }
  }
}