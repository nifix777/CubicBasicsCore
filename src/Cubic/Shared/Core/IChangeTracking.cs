using System.Collections.Generic;

namespace Cubic.Core
{
  public interface IChangeTrackingExt : System.ComponentModel.IChangeTracking
  {
    IEnumerable<string> ChangedProperties { get; }

    void Track(string propName, object oldValue, object newValue);

    void RevertChanges();
  }
}