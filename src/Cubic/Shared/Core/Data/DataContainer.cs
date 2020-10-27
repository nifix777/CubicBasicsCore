using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Cubic.Core.Collections;
using Cubic.Core.Reflection;

namespace Cubic.Core.Data
{
  [Serializable]
  [DataContract]
  [KnownType(typeof(DataContainerSet))]
  //[KnownType(typeof(DataContainerSet<>))]
  public class DataContainer
  {
    private List<NamedValue> _values;

    protected List<DataContainer> _childs;

    public DataContainer()
    {
      _values = new List<NamedValue>();
      _childs = new List<DataContainer>();

      State = DataContainerState.Undifiend;
    }

    public DataContainer(DataContainer container) : this()
    {
      DataContainer.FillFrom(this, container);
    }

    public static void FillFrom(DataContainer target, DataContainer source)
    {
      target.KeyValue = source.KeyValue;
      target.Values.AddRange(source.Values);
      target.Version = source.Version;

      foreach (var child in source.Childs)
      {
        var newChild = new DataContainer();
        DataContainer.FillFrom(newChild, child);
        target.Childs.Add(newChild);
      }
    }

    public static DataContainer CreateContainerType(Type type)
    {
      return Activator.CreateInstance(type) as DataContainer;
    }

    #region Values

    [DataMember]
    public List<NamedValue> Values
    {
      get { return _values; }
      set { _values = value; }
    }  


    #endregion

    #region Container

    [DataMember]
    public List<DataContainer> Childs
    {
      get { return _childs; }
      set { _childs = value; }
    }

    #endregion

    [DataMember]
    public string KeyValue { get; set; }

    [DataMember]
    public string Version { get; set; }

    [DataMember]
    public DateTime? LastUpdated { get; set; }

    [DataMember]
    public DataContainerState State { get; set; }


    public DataContainer Parent { get; set; }

    public object this[string name]
    {
      get
      {
        if (ValueExists(name))
        {
          return GetValueTuple( name ).Value;
        }
        else
        {
          return GetChild(name);
        }
      }
      set { Fill(name, value);}
    }

    public NamedValue GetValueTuple(string name)
    {
      return _values.First(v => v.Name.Equals(name));
    }

    public T GetValue<T>(string name)
    {
      return (T)_values.First(v => v.Name.Equals(name)).Value;

      //var valueTuple = _values.First(v => v.Name.Equals(name));
      //var type = typeof( T );

    }

    public T GetChild<T>(string name) where T : DataContainer
    {
      if ( typeof(T) == typeof(DataContainer) )
      {
        return ( T ) GetChild( name ); 
      }
      else
      {
        var data = DataContainer.CreateContainerType(typeof (T));
        DataContainer.FillFrom(data, GetChild(name));
        return (T) data;
      }
    }

    private DataContainer GetChild(string name)
    {
      return _childs.First(c => c.KeyValue.Equals(name));
    }

    public bool ChildExists(string name)
    {
      return _childs.Exits(c => c.KeyValue.Equals(name));
    }

    public bool ValueExists( string name )
    {
      return _values.Exits( v => v.Name.Equals( name ) );
    }

    public void Fill(string name, object value)
    {

      if (value.GetType().IsSimple())
      {
        if (ValueExists(name))
        {
          GetValueTuple(name).Value = value;
        }
        else
        {
          _values.Add(new NamedValue(name, value));
        }
      }
      else if(value is DataContainerSet)
      {
        DataContainerSet dataContainerSet = value as DataContainerSet;
        if ( ChildExists( name ) )
        {
          var dataSet = GetChild<DataContainerSet>(name);
          dataSet = dataContainerSet;
        }
        else
        {
          _childs.Add(dataContainerSet);
        }
      }
      else if(value is ICollection)
      {
        if ( ValueExists( name ) )
        {
          GetValueTuple( name ).Value = value;
        }
        else
        {
          _values.Add( new NamedValue( name , value ) );
        }
      }
      else if (value is DataContainer)
      {
        if ( ChildExists( name ) )
        {
          var old = GetChild<DataContainer>( name );
          _childs.Remove( old );
        }
        var container = ((DataContainer) value).ToDataContainer();
        container.KeyValue = name;
        _childs.Add( container);
      }
      else
      {
        throw new NotSupportedException(string.Format("Type {0} not supported", value.GetType()));
      }
    }

    public DataContainer ToDataContainer()
    {
      var copy = new DataContainer();
      copy.KeyValue = KeyValue;
      copy.Version = Version;

      copy.Values = this.Values;



      foreach (var container in this.Childs)
      {
        copy.Childs.Add( container.ToDataContainer());
      }

      return copy;
    }

  }
}