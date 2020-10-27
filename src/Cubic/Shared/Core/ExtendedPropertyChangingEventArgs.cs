using System.ComponentModel;

namespace Cubic.Core
{
  public class ExtendedPropertyChangingEventArgs : PropertyChangingEventArgs
  {
    public object NewValue
    {
      get;
      set;
    }

    public object OldValue
    {
      get;
      private set;
    }

    public ExtendedPropertyChangingEventArgs(string propertyName, object oldValue, object newValue) : base(propertyName)
    {
      this.OldValue = oldValue;
      this.NewValue = newValue;
    }
  }

  public interface IExtendedNotifyPropertyChanging : INotifyPropertyChanging
  {
    event ExtendedPropertyChangingEventHandler PropertyChangingExtended;
  }

  public delegate void ExtendedPropertyChangingEventHandler(object sender, ExtendedPropertyChangingEventArgs args);
}