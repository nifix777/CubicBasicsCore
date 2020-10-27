using System;

namespace Cubic.Core.Observer
{
  public class EventObservable<T> : ObserverableBase<T>, IDisposable
  {
    private EventHandler<T> _eventHandler;

    public EventObservable( EventHandler<T> eventHandler)
    {
      _eventHandler = eventHandler;
      _eventHandler += EventHandler;
    }

    private void EventHandler(object sender, T e)
    {
      this.Next(e);
    }

    public void Dispose()
    {
      if ( _eventHandler != null )
      {
        _eventHandler -= EventHandler; 
      }
    }
  }
}