using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Cubic.Core.Diagnostics;

namespace Cubic.Core.Collections
{
  [Serializable]
  public class DynamicDictionary : DynamicObject, IDictionary<string, object>
  {
    private IDictionary<string , object> _fields;
    public DynamicDictionary()
    {
      _fields = new Dictionary<string, object>();
    }

    public DynamicDictionary(IDictionary<string, object> valueDictionary )
    {
      Guard.ArgumentNull(valueDictionary, nameof(valueDictionary));

      _fields = valueDictionary;
    }

    public IDictionary<string, object> Fields => _fields; 

    // If you try to get a value of a property 
    // not defined in the class, this method is called.
    public override bool TryGetMember(GetMemberBinder binder , out object result )
    {
      // Converting the property name to lowercase
      // so that property names become case-insensitive.
      string name = binder.Name.ToLower();

      // If the property name is found in a dictionary,
      // set the result parameter to the property value and return true.
      // Otherwise, return false.
      return _fields.TryGetValue( name , out result );
    }

    // If you try to set a value of a property that is
    // not defined in the class, this method is called.
    public override bool TrySetMember(SetMemberBinder binder , object value )
    {
      // Converting the property name to lowercase
      // so that property names become case-insensitive.
      _fields[binder.Name.ToLower()] = value;

      // You can always add a value to a dictionary,
      // so this method always returns true.
      return true;
    }
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
      return _fields.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ( ( IEnumerable ) Fields ).GetEnumerator();
    }

    public void Add(KeyValuePair<string, object> item)
    {
      _fields.Add(item);
    }

    public void Clear()
    {
      _fields.Clear();
    }

    public bool Contains(KeyValuePair<string, object> item)
    {
      return _fields.Contains(item);
    }

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
       _fields.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<string, object> item)
    {
      return  _fields.Remove(item);
    }

    public int Count
    {
      get { return _fields.Count; }
    }

    public bool IsReadOnly
    {
      get { return _fields.IsReadOnly; }
    }

    public bool ContainsKey(string key)
    {
      return _fields.ContainsKey(key);
    }

    public void Add(string key, object value)
    {
      _fields.Add(key, value);
    }

    public bool Remove(string key)
    {
      return _fields.Remove(key);
    }

    public bool TryGetValue(string key, out object value)
    {
      return _fields.TryGetValue(key, out value);
    }

    public object this[string key]
    {
      get { return _fields[key]; }
      set { _fields[key] = value; }
    }

    public ICollection<string> Keys
    {
      get { return _fields.Keys; }
    }

    public ICollection<object> Values
    {
      get { return _fields.Values; }
    }
  }
}