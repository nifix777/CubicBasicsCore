using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Cubic.Core.Data
{
  [Serializable]
  [DataContract]
  [KnownType( typeof( DataContainer ) )]
  //[KnownType( typeof( DataContainerSet<> ) )]
  public class DataContainerSet : DataContainer, IList<DataContainer>
  {
    public DataContainerSet()
    {
      
    }

    public DataContainerSet( DataContainer source ) : base(source)
    {
      
    }
    public IEnumerator<DataContainer> GetEnumerator()
    {

      return _childs.GetEnumerator();
    }

    public void Add(DataContainer item)
    {
      _childs.Add(item);
    }

    public bool Contains(DataContainer item)
    {
      return _childs.Contains(item);
    }

    public void Clear()
    {
      _childs.Clear();
    }

    public void CopyTo(DataContainer[] array, int arrayIndex)
    {
      _childs.CopyTo(array, arrayIndex);
    }

    public bool Remove(DataContainer item)
    {
      return _childs.Remove(item);
    }

    public int Count
    {
      get { return _childs.Count; }
    }

    public bool IsReadOnly => false;

    public int IndexOf(DataContainer item)
    {
      return _childs.IndexOf(item);
    }

    public void Insert(int index, DataContainer item)
    {
      _childs.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
      _childs.RemoveAt(index);
    }

    public DataContainer this[int index]
    {
      get { return _childs[index]; }
      set { _childs[index] = value; }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable) _childs).GetEnumerator();
    }
  }

  [DataContract( IsReference = true )]
  [KnownType( typeof( DataContainerSet ) )]
  [KnownType(typeof(DataContainer))]
  public class DataContainerSet<DC> : DataContainerSet , IList<DC> where DC : DataContainer, new()
  {

    public DataContainerSet()
    {
      
    }

    public DataContainerSet( DataContainer source ) : base(source)
    {
      this.PrepareEntrys();
    }

    private void PrepareEntrys()
    {
      foreach (var dataContainer in Childs.ToArray())
      {
        var specialisedEntry = new DC();
        DataContainer.FillFrom(specialisedEntry, dataContainer);
        this.Childs.Remove(dataContainer);
        this.Childs.Add(specialisedEntry);

      }
    }

    public new IEnumerator<DC> GetEnumerator()
    {
      return (IEnumerator<DC>) base.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public void Add(DC item)
    {
      base.Add(item);
    }

    public new void Clear()
    {
      base.Clear();
    }

    public bool Contains(DC item)
    {
      return base.Contains(item);
    }

    public void CopyTo(DC[] array, int arrayIndex)
    {
      base.CopyTo(array, arrayIndex);
    }

    public bool Remove(DC item)
    {
      return base.Remove(item);
    }

    public int IndexOf(DC item)
    {
      return base.IndexOf(item);
    }

    public void Insert(int index, DC item)
    {
      base.Insert(index, item);
    }

    public new void RemoveAt(int index)
    {
      base.RemoveAt(index);
    }

    public new DC this[int index]
    {
      get { return (DC) base[index]; }
      set { base[index] = value; }
    }
  }
}