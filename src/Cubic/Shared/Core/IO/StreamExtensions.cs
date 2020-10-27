using System;
using System.IO;

namespace Cubic.Core.IO
{
  public static class StreamExtensions
  {

    public static IDisposable TemporarySeek(this Stream stream, long offset, SeekOrigin seekOrigin)
    {
      return new SeekTask(stream, offset, seekOrigin);
    }

  }
}