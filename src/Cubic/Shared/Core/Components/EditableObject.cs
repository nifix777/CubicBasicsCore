using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Components
{
  public class Editable<TClonable> : IEditableObject, IRevertibleChangeTracking where TClonable : class, ICloneable
  {
    private TClonable _backup;

    public Editable(TClonable backup)
    {
      _backup = backup;
    }

    public TClonable Current { get; private set; }

    public bool IsChanged => Current != null;

    public void AcceptChanges()
    {
      if (Current != null)
      {
        _backup = Current;
      }
    }

    public void BeginEdit()
    {
      if(Current != null)
      {
        throw new InvalidOperationException();
      }

      Current = (TClonable)_backup.Clone();
    }

    public void CancelEdit()
    {
      if(Current != null)
      {
        Current = null;
        Current = (TClonable)_backup.Clone();
      }
    }

    public void EndEdit()
    {
      if(Current != null)
      {
        _backup = Current;
      }
    }

    public void RejectChanges()
    {
      if(Current != null)
      {
        Current = null;
      }
    }
  }
}
