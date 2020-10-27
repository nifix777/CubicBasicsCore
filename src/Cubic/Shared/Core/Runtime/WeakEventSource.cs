using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Cubic.Core.Annotations;
using Cubic.Core.Collections;

namespace Cubic.Core.Runtime
{
  public class WeakEventSource<TEventArgs> where TEventArgs : EventArgs
  {

    // ReSharper disable once AssignNullToNotNullAttribute
    private readonly List<WeakDelegate> _handlers = new List<WeakDelegate>();

    /// <summary>
    /// Raises the event with the specified sender and argument.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see paramref="TEventArgs"/> instance containing the event data.</param>
    [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
    public void Raise(object sender, TEventArgs e)
    {
      lock (_handlers)
      {
        _handlers.Remove(h => !h.Invoke(sender, e));
      }
    }

    /// <summary>
    /// Subscribes the specified handler for the event.
    /// </summary>
    /// <param name="handler">The handler.</param>
    public void Subscribe([NotNull] EventHandler<TEventArgs> handler)
    {
      // ReSharper disable once AssignNullToNotNullAttribute
      var weakHandlers = handler
          .GetInvocationList()
          // ReSharper disable once AssignNullToNotNullAttribute
          .Select(d => new WeakDelegate(d))
          .ToArray();

      lock (_handlers)
      {
        _handlers.AddRange(weakHandlers);
      }
    }

    /// <summary>
    /// Unsubscribes the specified handler from the event.
    /// </summary>
    /// <param name="handler">The handler.</param>
    public void Unsubscribe([NotNull] EventHandler<TEventArgs> handler)
    {
      lock (_handlers)
      {
        while (true)
        {
          var index = _handlers.FindIndex(h1 => h1.Matches(handler));
          if (index < 0)
            return;

          _handlers.RemoveAt(index);
        }
      }

    }

    private class WeakDelegate
    {
      [CanBeNull]
      private readonly WeakReference _weakTarget;
      [NotNull]
      private readonly MethodInfo _method;

      public WeakDelegate([NotNull] Delegate handler)
      {
        _weakTarget = handler.Target != null ? new WeakReference(handler.Target) : null;
        // ReSharper disable once AssignNullToNotNullAttribute
        _method = handler.GetMethodInfo();
      }

      public bool Invoke(object sender, TEventArgs e)
      {
        object target = null;

        if (_weakTarget != null)
        {
          target = _weakTarget.Target;
          if (target == null)
            return false;
        }

        _method.Invoke(target, new[] { sender, e });

        return true;
      }

      public bool Matches([NotNull] EventHandler<TEventArgs> handler)
      {
        return ReferenceEquals(handler.Target, _weakTarget?.Target)
               && Equals(handler.GetMethodInfo(), _method);
      }
    }
  }
}