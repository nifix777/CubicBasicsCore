using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;
using Cubic.Core.Data;
using Xunit;
using Newtonsoft.Json;

namespace Cubic.Basics.Testing.Data
{
  
  public class DataContainerSetTest
  {
    [Fact]
    public void DataContainerSetBasic()
    {
      var container = new DataContainer();
      container["Name"] = "Hello World!";
      container["Nummber"] = 100;
      container["Bool"] = true;
      container["Complex"] = CreateComplex();
      var set = new DataContainerSet();
      set.Add( container );

      var newCont = new DataContainerSet();
      DataContainer.FillFrom(newCont, set);

      Assert.Equal( set.Count , newCont.Count );
      Assert.Equal( set[0]["Name"] , newCont[0]["Name"] );
    }

    [Fact]
    public void DataContainerSetJson()
    {

      var container = new DataContainer();
      container["Name"] = "Hello World!";
      container["Nummber"] = 100;
      container["Bool"] = true;
      container["Complex"] = CreateComplex();
      var set = new DataContainerSet();
      set.Add( container );

      var raw = JsonConvert.SerializeObject( set );
      Trace.Write( (string) raw );
      var newCont = JsonConvert.DeserializeObject<DataContainerSet>( raw );

      Assert.NotNull( newCont );
      Assert.Equal<int>( set.Count , newCont.Count );
      Assert.Equal( set[0]["Name"] , newCont[0]["Name"] );
    }

    [Fact]
    public void DataContainerSetXml()
    {
      var container = new DataContainer();
      container["Name"] = "Hello World!";
      container["Nummber"] = 100;
      container["Bool"] = true;
      container["Complex"] = CreateComplex();
      var set = new DataContainerSet();
      set.Add( container );

      DataContainerSet newCont = null;
      using ( var stream = new MemoryStream() )
      {
        var serializer = new XmlSerializer( typeof( DataContainerSet ) );
        serializer.Serialize( stream , set );
        Trace.Write( Encoding.UTF8.GetString( stream.ToArray() ) );
        stream.Seek( 0 , SeekOrigin.Begin );

        newCont = serializer.Deserialize( stream ) as DataContainerSet;
      }

      Assert.NotNull( newCont );
      Assert.Equal( set.Count , newCont.Count );
      Assert.Equal( set[0]["Name"] , newCont[0]["Name"] );
    }

    [Fact]
    public void DataContainerSetDataContractJson()
    {
      var container = new DataContainer();
      container["Name"] = "Hello World!";
      container["Nummber"] = 100;
      container["Bool"] = true;
      container["Complex"] = CreateComplex();
      var set = new DataContainerSet();
      set.Add(container);

      DataContainerSet newCont = null; ;

      using (var stream = new MemoryStream())
      {
        var serializer = new DataContractJsonSerializer(typeof(DataContainerSet));
        serializer.WriteObject(stream, set);
        Trace.Write(Encoding.UTF8.GetString(stream.ToArray()));
        stream.Seek(0, SeekOrigin.Begin);

        newCont = serializer.ReadObject(stream) as DataContainerSet;
      }

      Assert.NotNull(newCont);
      Assert.Equal(set.Count, newCont.Count);
      Assert.Equal(set[0]["Name"], newCont[0]["Name"]);
    }

    [Fact]
    public void DataContainerSetBinary()
    {
      var container = new DataContainer();
      container["Name"] = "Hello World!";
      container["Nummber"] = 100;
      container["Bool"] = true;
      container["Complex"] = CreateComplex();
      var set = new DataContainerSet();
      set.Add( container );

      DataContainerSet newCont = null;

      using ( var stream = new MemoryStream() )
      {
        var serializer = new BinaryFormatter();
        serializer.Serialize( stream , set );
        Trace.Write( Convert.ToBase64String( stream.ToArray() ) );
        stream.Seek( 0 , SeekOrigin.Begin );

        newCont = serializer.Deserialize( stream ) as DataContainerSet;
      }

      Assert.NotNull( newCont );
      Assert.Equal( set.Count , newCont.Count );
      Assert.Equal( set[0]["Name"] , newCont[0]["Name"] );

    }

    [Fact]
    public void DataContainerSetDataContractXml()
    {
      var container = new DataContainer();
      container["Name"] = "Hello World!";
      container["Nummber"] = 100;
      container["Bool"] = true;
      container["Complex"] = CreateComplex();
      var set = new DataContainerSet();
      set.Add(container);

      DataContainerSet newCont = null; ;

      using (var stream = new MemoryStream())
      {
        var serializer = new DataContractSerializer(typeof(DataContainerSet));
        serializer.WriteObject(stream, set);
        Trace.Write(Encoding.UTF8.GetString(stream.ToArray()));
        stream.Seek(0, SeekOrigin.Begin);

        newCont = serializer.ReadObject(stream) as DataContainerSet;
      }

      Assert.NotNull(newCont);
      Assert.Equal(set.Count, newCont.Count);
      Assert.Equal(set[0]["Name"], newCont[0]["Name"]);
    }

    public DataContainer CreateComplex()
    {
      var container = new DataContainer();
      container["Name"] = "Hello World!";
      container["Nummber"] = 100;
      container["Bool"] = true;
      return container;
    }
  }
}
