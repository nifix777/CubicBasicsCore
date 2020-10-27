using System;
using System.Runtime.Serialization;

namespace TestProject.Microsoft.VisualBasic.FileIO
{
  [Serializable]
  public class MalformedLineException : Exception
  {
    private string v;
    private long m_ErrorLineNumber;

    public MalformedLineException()
    {
    }

    public MalformedLineException(string message) : base(message)
    {
    }

    public MalformedLineException(string v, long m_ErrorLineNumber)
    {
      this.v = v;
      this.m_ErrorLineNumber = m_ErrorLineNumber;
    }

    public MalformedLineException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected MalformedLineException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}

