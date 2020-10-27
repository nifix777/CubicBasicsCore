using System;
using System.Threading.Tasks;

namespace Cubic.Core.Broker
{
  public interface IEventBroker
  {
    IDisposable Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class;

    //void Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class;

    void Unsubscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class;

    //void Subscribe<TEvent>(Action<TEvent> handler, Predicate<TEvent> filter  ) where TEvent : class;
    //void Raise<TEvent>(TEvent @event) where TEvent : class;
    Task RaiseAsync<TEvent>(TEvent @event) where TEvent : class;

    Task RaiseAsync(object @event);
  }

  //public delegate bool FilterDelegate(object expression);
  //public delegate void EventHandleDelegate(object expression);
  //public delegate Task AsyncEventHandleDelegate(object expression);
}