using System;

namespace Cubic.Core.Tools
{
  public class ApplicationStateInfoEventArgs : EventArgs
  {
    private ApplicationStateInfoItem _item;

    public ApplicationStateInfoItem Item
    {
      get
      {
        return this._item;
      }
      set
      {
        this._item = value;
      }
    }

    public ApplicationStateInfoEventArgs()
    {
    }

    public ApplicationStateInfoEventArgs(ApplicationStateInfoItem item)
    {
      this._item = item;
    }
  }
}
