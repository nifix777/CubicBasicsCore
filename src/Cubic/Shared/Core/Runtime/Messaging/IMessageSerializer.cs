using System;
using System.IO;
using System.Threading.Tasks;

namespace Cubic.Core.Runtime.Messaging
{
  public partial class Envelope
  {
    public interface IMessageSerializer
    {
      Type DotNetType { get; }

      string ContentType { get; }
      byte[] Write(object model);
      Task WriteToStream(object model, Stream response);
    }
  }
}
