using System;
using System.Threading.Tasks;

namespace Cubic.Core.Broker
{
  public interface IEventHandler<TEvent> : IEventHandler
  {
    //Predicate<TEvent> Filter { get; }
    //Action<TEvent> Handler { get; }
    //Func<TEvent, Task> AsyncHandler { get; }

    bool Filter(TEvent @event);
    Task HandleAsync(TEvent @event);
  }

  public interface IEventHandler
  {
    bool Filter(object @event);
    Task HandleAsync(object @event);
  }


  public abstract class EventHandlerBase<TEvent> : IEventHandler<TEvent>
  {
    public virtual bool Filter(TEvent @event) => true;
    public abstract Task HandleAsync(TEvent @event);
    public bool Filter(object @event) => Filter((TEvent) @event);

    public Task HandleAsync(object @event) => HandleAsync((TEvent) @event);
  }

  //internal class EventHandler<TEvent> : IEventHandler<TEvent>
  //{
  //  public EventHandler(Predicate<TEvent> filter, Action<TEvent> handler, Func<TEvent, Task> asyncHandler)
  //  {
  //    Filter = filter;
  //    Handler = handler;
  //    AsyncHandler = asyncHandler;

  //    if (filter == null) Filter = @event => true;
  //    if (Handler == null) Handler = @event => AsyncHandler(@event).Wait();
  //    if (AsyncHandler == null) AsyncHandler = @event => Task.Run( () => Handler(@event));
  //  }
  //  public Predicate<TEvent> Filter { get; }
  //  public Action<TEvent> Handler { get; }
  //  public Func<TEvent, Task> AsyncHandler { get; }
  //  bool IEventHandler<TEvent>.Filter(TEvent @event) => Filter(@event);

  //  public void Handle(TEvent @event) => Handler(@event);

  //  public Task HandleAsync(TEvent @event) => AsyncHandler(@event);
  //}
}