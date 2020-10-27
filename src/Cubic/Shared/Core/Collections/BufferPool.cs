using System;

namespace Cubic.Core.Collections
{
  public class BufferPool : ArrayPool<byte>
  {
    public BufferPool(Func<int, byte[]> facortyMethod, bool directInitialse = true, int defaultBufferSize = 1024) : base(facortyMethod, directInitialse, defaultBufferSize)
    {
    }

    public new static BufferPool Shared { get; } = new BufferPool(DefaultBufferFactory);
    private static byte[] DefaultBufferFactory(int requestedSize)
    {
      return new byte[requestedSize];
    }
  }
}