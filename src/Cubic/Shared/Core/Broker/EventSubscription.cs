using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Broker
{
  public class EventSubscription<TEvent> : IDisposable where TEvent : class
  {
    private readonly IEventBroker broker;

    private readonly IEventHandler<TEvent> handler;

    public EventSubscription(IEventBroker broker, IEventHandler<TEvent> handler)
    {
      this.broker = broker;
      this.handler = handler;
    }

    public void Dispose()
    {
      broker?.Unsubscribe<TEvent>(handler);
    }
  }
}
