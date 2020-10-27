using Cubic.Core.Diagnostics;
using Cubic.Core.Reflection;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Cubic.Core.Components
{
  public class ServiceProvider : IServiceProvider
  {
    private readonly IServiceProviderEngine _engine;

    public ServiceProvider(IServiceProviderEngine engine)
    {
      _engine = engine;
    }

    public object GetService(Type serviceType)
    {
      return _engine.GetRealService(serviceType);
    }

    #region IDisposable Support
    private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          // TODO: verwalteten Zustand (verwaltete Objekte) entsorgen.
          _engine.Dispose();
        }

        // TODO: nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer weiter unten überschreiben.
        // TODO: große Felder auf Null setzen.

        disposedValue = true;
      }
    }

    // TODO: Finalizer nur überschreiben, wenn Dispose(bool disposing) weiter oben Code für die Freigabe nicht verwalteter Ressourcen enthält.
    // ~ServiceProvider()
    // {
    //   // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
    //   Dispose(false);
    // }

    // Dieser Code wird hinzugefügt, um das Dispose-Muster richtig zu implementieren.
    public void Dispose()
    {
      // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
      Dispose(true);
      // TODO: Auskommentierung der folgenden Zeile aufheben, wenn der Finalizer weiter oben überschrieben wird.
      // GC.SuppressFinalize(this);
    }
    #endregion
  }
}
