using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.IO
{
  public class SeekTask : IDisposable
  {
    public SeekTask(Stream stream, long offset, SeekOrigin origin)
    {
      Stream = stream;
      PreviousPosition = stream.Position;
      Stream.Seek(offset, origin);
    }

    public Stream Stream { get; private set; }

    /// <summary>
    /// Gets the absolute position to which the <see cref="Stream"/> will be rewound after this task is
    /// disposed.
    /// </summary>
    public long PreviousPosition { get; private set; }

    /// <summary>
    /// Rewinds the <see cref="Stream"/> to its previous position.
    /// </summary>
    public void Dispose()
    {
      Stream.Seek(PreviousPosition, SeekOrigin.Begin);
    }
  }
}
