using System;
using System.Runtime.Serialization;

namespace Cubic.Core.Data
{
  [Serializable]
  [DataContract]
  public class NamedValue
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public object Value { get; set; }

    public NamedValue()
    {
      
    }

    public NamedValue(string name, object value = null)
    {
      this.Name = name;
      this.Value = value;
    }
  }

  //[Serializable]
  //[DataContract]
  //public class ContainerTuple
  //{
  //  [DataMember]
  //  public string Name { get; set; }

  //  [DataMember]
  //  public DataContainer Container { get; set; }

  //  public ContainerTuple()
  //  {
      
  //  }

  //  public ContainerTuple(string name, DataContainer container)
  //  {
  //    this.Name = name;
  //    this.Container = container;
  //  }
  //}
}