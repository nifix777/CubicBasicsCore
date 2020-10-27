using System.Collections.Generic;
using System.Dynamic;

namespace Cubic.Core.Reflection
{
  public abstract class DynamicDataObject : DynamicObject
  {
    private IDictionary<string, object> _data;
    private readonly bool _canSetData;

    public DynamicDataObject(IDictionary<string, object> data, bool canSetData = false )
    {
      _data = data;
      _canSetData = canSetData;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      result = _data[binder.Name.ToLowerInvariant()];
      return true;
    }

    public override IEnumerable<string> GetDynamicMemberNames()
    {
      return _data.Keys;
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      if (_canSetData)
      {
        _data[binder.Name.ToLowerInvariant()] = value;
        return true;
      }

      return false;
    }
  }
}