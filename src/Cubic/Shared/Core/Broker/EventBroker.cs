using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cubic.Core.Collections;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Broker
{
  public class EventBroker : IEventBroker
  {
    private IDictionary<Type, List<IEventHandler>> _handlerDictionary;

    private Predicate<Exception> _errorHandler;

    public EventBroker(Predicate<Exception> errorHandler = null)
    {
      _errorHandler = errorHandler;
      _handlerDictionary = new Dictionary<Type, List<IEventHandler>>();
    }

    internal void SubscribeCore<TEvent>(IEventHandler<TEvent> handler) where TEvent : class
    {
      Guard.AgainstNull(handler, nameof(handler));


      var handlers = GetOrAddSubscriptions(typeof(TEvent));

      CheckDublicateHandlers(handlers, handler);

      handlers.Add(handler);

    }

    public IDisposable Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class
    {
      SubscribeCore(handler);

      return new EventSubscription<TEvent>(this, handler);
    }

    public void Unsubscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class
    {
      Guard.AgainstNull(handler, nameof(handler));

      var handlers = GetOrAddSubscriptions(typeof(TEvent));

      handlers.Remove(handler);
    }

    private IEnumerable<IEventHandler> GetSubscriptions(Type eventType)
    {
      

      if(_handlerDictionary.ContainsKey(eventType))
      {
        return _handlerDictionary[eventType];
      }

      return Enumerable.Empty<IEventHandler>();

    }

    private IList<IEventHandler> GetOrAddSubscriptions(Type eventType)
    {
      List<IEventHandler> handler;

      if (_handlerDictionary.ContainsKey(eventType))
      {
        handler = _handlerDictionary[eventType];
      }
      else
      {
        handler = new List<IEventHandler>();
        _handlerDictionary[eventType] = handler;
      }

      return handler;
    }

    private void CheckDublicateHandlers(IEnumerable<IEventHandler> exisitng, IEventHandler newInstance)
    {
      foreach (var eventHandler in exisitng)
      {
        // check wehter its the same Handler-Class
        if (eventHandler == newInstance)
        {
          throw new DublicateHandlerException("Dublicate handler found");

          //if (eventHandler.Handle == handler.Handle || eventHandler.AsyncHandler == handler.AsyncHandler)
          //{
          //  throw new DublicateHandlerException("Dublicate handler-delegtes found");
          //}
        }
      }
    }

    //public void Subscribe<TEvent>(Action<TEvent> handler, Predicate<TEvent> filter) where TEvent : class
    //{
    //  var handlerInstance = new EventHandler<TEvent>(filter, handler, null);
    //  this.Subscribe(handlerInstance);
    //}

    //public void Raise<TEvent>(TEvent @event) where TEvent : class
    //{
    //  if (_handlerDictionary.ContainsKey(typeof (TEvent)))
    //  {
    //    var handlers = _handlerDictionary[typeof (TEvent)];
    //    foreach (var eventHandler in handlers)
    //    {
    //      try
    //      {
    //        eventHandler.Handle(@event);
    //      }
    //      catch (Exception err)
    //      {
    //        var handeld = _errorHandler?.Invoke(err) ?? false;
    //        if (!handeld)
    //        {
    //          throw;
    //        }
    //      }
    //    }
    //  }
    //}

    public virtual async Task RaiseAsync<TEvent>(TEvent @event) where TEvent : class
    {
      foreach (var eventHandler in GetSubscriptions(typeof(TEvent)).Where(h => h.Filter(@event)))
      {
        try
        {
          await eventHandler.HandleAsync(@event);
        }
        catch (Exception err)
        {
          if (!_errorHandler?.Invoke(err) ?? false)
          {
            throw;
          }
        }
      }
    }

    public virtual async Task RaiseAsync(object @event)
    {
      foreach (var eventHandler in GetSubscriptions(@event.GetType()).Where(h => h.Filter(@event)))
      {
        try
        {
          await eventHandler.HandleAsync(@event);
        }
        catch (Exception err)
        {
          if (!_errorHandler?.Invoke(err) ?? false)
          {
            throw;
          }
        }
      }
    }


  }

  internal class DublicateHandlerException : Exception
  {
    public DublicateHandlerException(string message) : base(message)
    {
      
    }
  }
}