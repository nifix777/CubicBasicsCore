using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cubic.Core.Observer;

namespace Cubic.Core.Observer
{
  public abstract class ObserverableBase<T> : IObservable<T>
  {
    private IList<IObserver<T>> _observers;

    private object _sync;

    protected ObserverableBase()
    {
      _sync = new object();
      _observers = new List<IObserver<T>>();
    }
    


    protected virtual void Next( T item )
    {
      foreach (var observer in _observers.ToArray())
      {
        observer.OnNext(item);
      }
    }

    protected virtual void Complete()
    {
      foreach (var observer in _observers.ToArray())
      {
        observer.OnCompleted();
      }
    }

    protected virtual void Error( Exception exception )
    {
      foreach (var observer in _observers.ToArray())
      {
        observer.OnError(exception);
      }
    }

    #region Subscription
    public IDisposable Subscribe( IObserver<T> observer )
    {
      lock ( _sync )
      {
        if ( !_observers.Contains( observer ) ) _observers.Add( observer );
      }

      return new Subscription<IObservable<T> , IObserver<T> , T>( this , observer );
    }

    public void Unsubscribe( IObserver<T> observer )
    {
      lock ( _sync )
      {
        if ( _observers.Contains( observer ) ) _observers.Remove( observer );
      }
    } 
    #endregion
  }
}