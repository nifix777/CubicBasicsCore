using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using Cubic.Core;
using Cubic.Core.Data;
using Xunit;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Cubic.Basics.Testing.Data
{
  
  public class DataContainerTest
  {
    [Fact]
    public void DataContainerBasic()
    {
      var container = CreateComplex();

      var copy = container.ToDataContainer();


      Assert.Equal( container.Text, copy[nameof(container.Text)] );
      Assert.Equal( container.Number, copy[nameof(container.Number)] );
      Assert.Equal( container.Boolean, copy[nameof(container.Boolean)] );

      var copyData = copy.GetChild<MyHirachyDataContainer>(nameof(container.MyData));
      Assert.Equal(container.MyData.Text, copyData.Text);

      copyData.Text = "Test";

      Assert.Equal( copy.GetChild<MyHirachyDataContainer>( nameof( container.MyData ) ).Text, copyData.Text );
    }

    [Fact]
    public void DataContainerJson()
    {

      var container = CreateComplex();

      var raw = JsonConvert.SerializeObject( container );
      Trace.Write( (string) raw );
      var data = JsonConvert.DeserializeObject<DataContainer>( raw , new JsonSerializerSettings() {TypeNameHandling =  TypeNameHandling.All, ReferenceLoopHandling = ReferenceLoopHandling.Serialize});

      var newCont = new MyDataContainer( data );

      Assert.Equal( container.Text , newCont[nameof( container.Text )] );
      Assert.Equal( container.Number , newCont[nameof( container.Number )] );
      Assert.Equal( container.Boolean , newCont[nameof( container.Boolean )] );

      var copyData = newCont.GetChild<MyHirachyDataContainer>( nameof( container.MyData ) );
      Assert.Equal( container.MyData.Text , copyData.Text );
    }

    [Fact]
    public void DataContainerXml()
    {
      var container = CreateComplex();
      var data = container.ToDataContainer();;

      using ( var stream = new MemoryStream() )
      {
        var serializer = new XmlSerializer( typeof( DataContainer ), new Type[] {typeof(ValueTuple)} );
        serializer.Serialize( stream , data );
        Trace.Write( Encoding.UTF8.GetString( stream.ToArray() ) );
        stream.Seek( 0 , SeekOrigin.Begin );

        data = serializer.Deserialize( stream ) as DataContainer;
      }

      var newCont = new MyDataContainer( data );

      Assert.Equal( container.Text , newCont[nameof( container.Text )] );
      Assert.Equal( container.Number , newCont[nameof( container.Number )] );
      Assert.Equal( container.Boolean , newCont[nameof( container.Boolean )] );

      var copyData = newCont.GetChild<MyHirachyDataContainer>( nameof( container.MyData ) );
      Assert.Equal( container.MyData.Text , copyData.Text );
    }

    [Fact]
    public void DataContainerDataContractXml()
    {
      var container = CreateComplex();
      var data = container.ToDataContainer(); ;

      using ( var stream = new MemoryStream() )
      {
        var serializer = new DataContractSerializer(typeof(DataContainer));
        serializer.WriteObject( stream , data );
        Trace.Write( Encoding.UTF8.GetString( stream.ToArray() ) );
        stream.Seek( 0 , SeekOrigin.Begin );

        data = serializer.ReadObject( stream ) as DataContainer;
      }

      var newCont = new MyDataContainer( data );

      Assert.Equal( container.Text , newCont[nameof( container.Text )] );
      Assert.Equal( container.Number , newCont[nameof( container.Number )] );
      Assert.Equal( container.Boolean , newCont[nameof( container.Boolean )] );

      var copyData = newCont.GetChild<MyHirachyDataContainer>( nameof( container.MyData ) );
      Assert.Equal( container.MyData.Text , copyData.Text );
    }

    [Fact]
    public void DataContainerDataContractJson()
    {
      var container = CreateComplex();
      var data = container.ToDataContainer(); ;

      using ( var stream = new MemoryStream() )
      {
        var serializer = new DataContractJsonSerializer(typeof(DataContainer));
        serializer.WriteObject( stream , data );
        Trace.Write( Encoding.UTF8.GetString( stream.ToArray() ) );
        stream.Seek( 0 , SeekOrigin.Begin );

        data = serializer.ReadObject( stream ) as DataContainer;
      }

      var newCont = new MyDataContainer( data );

      Assert.Equal( container.Text , newCont[nameof( container.Text )] );
      Assert.Equal( container.Number , newCont[nameof( container.Number )].ToInt64() );
      Assert.Equal( container.Boolean , newCont[nameof( container.Boolean )] );

      var copyData = newCont.GetChild<MyHirachyDataContainer>( nameof( container.MyData ) );
      Assert.Equal( container.MyData.Text , copyData.Text );
    }

    [Fact]
    public void DataContainerBinary()
    {
      var container = CreateComplex();
      var data = container.ToDataContainer();

      using ( var stream = new MemoryStream() )
      {
        var serializer = new BinaryFormatter();
        serializer.Serialize( stream , data );
        Trace.Write( Convert.ToBase64String( stream.ToArray() ) );
        stream.Seek( 0 , SeekOrigin.Begin );

        data = serializer.Deserialize( stream ) as DataContainer;
      }
      var newCont = new MyDataContainer(data);
      Assert.Equal( container.Text , newCont[nameof( container.Text )] );
      Assert.Equal( container.Number , newCont[nameof( container.Number )] );
      Assert.Equal( container.Boolean , newCont[nameof( container.Boolean )] );

      var copyData = newCont.GetChild<MyHirachyDataContainer>( nameof( container.MyData ) );
      Assert.Equal( container.MyData.Text , copyData.Text );

    }

    public MyDataContainer CreateComplex()
    {

      var container = new MyDataContainer();
      container.Text = "Hello World!";
      container.Number = 100;
      container.Boolean = true;

      var child = new MyHirachyDataContainer();
      child.Text = container.Text;
      container.MyData = child;

      return container;
    }

    public class MyDataContainer : DataContainer
    {
      public MyDataContainer()
      {
        
      }

      public MyDataContainer(DataContainer source) : base(source)
      {
        
      }

      public string Text
      {
        get { return GetValue<string>(nameof(Text)); }
        set { Fill(nameof(Text), value);}
      }

      public long Number
      {
        get { return GetValue<long>( nameof( Number ) ); }
        set { Fill( nameof( Number ) , value ); }
      }

      public bool Boolean
      {
        get { return GetValue<bool>( nameof( Boolean ) ); }
        set { Fill( nameof( Boolean ) , value ); }
      }

      public MyHirachyDataContainer MyData
      {
        get { return GetChild<MyHirachyDataContainer>( nameof( MyData ) ); }
        set { Fill( nameof( MyData ) , value ); }
      }

    }

    public class MyHirachyDataContainer : DataContainer
    {

      public MyHirachyDataContainer()
      {
        
      }
      public MyHirachyDataContainer( DataContainer source ) : base(source)
      {

      }

      public string Text
      {
        get { return GetValue<string>( nameof( Text ) ); }
        set { Fill( nameof( Text ) , value ); }
      }
    }
  }
}
