using System.Collections.Generic;
using System.Linq;
using Cubic.Core;
using Cubic.Core.Reflection;
using Xunit;

namespace Cubic.Basics.Testing.Reflection
{
  public class RecordClass
  {
    public string Name { get; set; }

    public int Id { get; set; }
  }

  
  public class ObjectReaderTests
  {
    [Fact]
    public void GetTypeAccessor()
    {
      var recordObject = new RecordClass() { Id = 1, Name = "Hello World"};
      var accessor = Cubic.Core.Reflection.TypeAccessor.Create<RecordClass>();

      Assert.NotNull(accessor);
      Assert.Equal(2, accessor.GetMembers().Count);
      Assert.Equal(1, accessor.GetValue(recordObject, "Id"));
    }

    [Fact]
    public void GetObjectReader()
    {
      var recordObject = new RecordClass() { Id = 1, Name = "Hello World" };
      var source = new List<RecordClass> {recordObject};
      var record = Cubic.Core.Data.ObjectReader.Create<RecordClass>(source, new[] {"Id", "Name"});

      Assert.NotNull(record);
      while (record.Read())
      {
        Assert.Equal(2, record.FieldCount);
        Assert.Equal(1, record.GetInt32(0)); 
      }
    }
  }




}
