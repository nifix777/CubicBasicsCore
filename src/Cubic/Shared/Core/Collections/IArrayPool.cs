using System;

namespace Cubic.Core.Collections
{
  public interface IArrayPool<TBuffer>
  {
    bool HasFreeObjectes { get; }
    EventHandler<Array> OnFree { get; set; }

    void Dispose();
    void Free(TBuffer[] buffer);
    void Free(TBuffer[] buffer, bool clearBuffer);
    TBuffer[] Rent(int lenght);
  }
}