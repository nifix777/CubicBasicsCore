using System;
using Cubic.Core.Runtime;

namespace Cubic.Core
{

  public interface IFreezable
  {
    void Freeze();
  }

  public abstract class FreezableBase : IFreezable
  {
    private AtomicBoolean _frozen;

    protected abstract void FreezeCore();

    protected FreezableBase()
    {
      _frozen = new AtomicBoolean(false);
    }

    public void Freeze()
    {
      if (!_frozen.Value)
      {
        FreezeCore();
        _frozen.FalseToTrue();
      }
    }

    public bool IsFrozen()
    {
      return _frozen.Value;
    }
  }
}