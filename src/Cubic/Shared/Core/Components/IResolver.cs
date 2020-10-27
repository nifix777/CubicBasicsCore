using System;

namespace Cubic.Core.Components
{
  public interface IResolver : IDisposable
  {
    object Resolve();

    Type ServiceType { get; }
  }
}