using System;

namespace Cubic.Core
{
  public interface IProgressBase<T> : IProgress<T> where T : class
  {
    T Token { get; }

    event EventHandler<T> OnChange;
  }
}