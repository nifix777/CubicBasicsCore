using System;

namespace Cubic.Core.Observer
{
  public interface IObservable<out T>
  {
    IDisposable Subscribe(IObserver<T> observer);

    void Unsubscribe(IObserver<T> observer);
  }
}