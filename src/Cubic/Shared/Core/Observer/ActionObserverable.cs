using System;
using System.Threading.Tasks;

namespace Cubic.Core.Observer
{
  public abstract class ActionObserverable<T> : ObserverableBase<T>
  {
    private void WorkerCore()
    {
      try
      {
        Task.Run( () => Do() );
        Complete();
      }
      catch ( Exception ex )
      {
        Error( ex );
      }
    }

    protected abstract void Do();
  }
}