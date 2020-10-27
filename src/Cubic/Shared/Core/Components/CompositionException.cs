using System;
using System.Runtime.Serialization;

namespace Cubic.Core.Components
{
  [Serializable]
  public class CompositionException : Exception
  {
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public CompositionException()
    {
    }

    public CompositionException(string message) : base(message)
    {
    }

    public CompositionException(string message, Exception inner) : base(message, inner)
    {
    }

    protected CompositionException(
      SerializationInfo info,
      StreamingContext context) : base(info, context)
    {
    }
  }
}