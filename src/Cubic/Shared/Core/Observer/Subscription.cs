using System;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Observer
{
  public class Subscription<O,S, T> : IDisposable where O : IObservable<T> where S : IObserver<T> 
  {
    private O Observer;

    private S Subscriber;

    private bool _disposed;

    public bool IsActive => !_disposed;

    public Subscription(O observer, S subscriber)
    {
      Guard.ArgumentNull(observer, nameof(observer));
      Guard.ArgumentNull(subscriber, nameof(subscriber));

      Observer = observer;
      Subscriber = subscriber;
    }
    public void Dispose()
    {
      Observer?.Unsubscribe(Subscriber);
      _disposed = true;
    }

    public override string ToString()
    {
      var observerName = Observer.IsNull() ? typeof (O).Name : Observer.GetType().Name;
      var subscriberName = Subscriber.IsNull() ? typeof (S).Name : Subscriber.GetType().Name;
      return string.Format("{0} subscribed on {1}", subscriberName, observerName);
    }
  }
}