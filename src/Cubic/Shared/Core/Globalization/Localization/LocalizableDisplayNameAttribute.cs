using System;
using System.ComponentModel;
using System.Threading;

namespace Cubic.Core.Globalization.Localization
{
  [AttributeUsage(AttributeTargets.All)]
  public class LocalizableDisplayNameAttribute : DisplayNameAttribute
  {
    private bool _replaced;

    private readonly Type _decoratedType;

    public override string DisplayName
    {
      get
      {
        if (!this._replaced)
        {
          try
          {
            base.DisplayNameValue = ResourceHelper.GetResourceString(this._decoratedType, base.DisplayName);
          }
          finally
          {
            Thread.MemoryBarrier();
            this._replaced = true;
          }
        }
        return base.DisplayNameValue;
      }
    }

    public LocalizableDisplayNameAttribute(Type decoratedType, string description) : base(description)
    {
      this._decoratedType = decoratedType;
    }
  }
}