using Cubic.Core;
using Cubic.Core.Reflection;
using Xunit;

namespace Cubic.Basics.Testing.Reflection
{
  
  public class ReflectionTests
  {
    //[Fact]
    //public void TestPropertyWalk_StringLenght()
    //{
    //  var recordObject = new RecordClass() { Id = 1, Name = "Hello World" };
    //  var nameLenght = recordObject.WalkProperty("Name.Lenght").ToInt32();

    //  Assert.Equal(recordObject.Name.Length, nameLenght);

    //}

    [Fact]
    public void TestPropertyWalk_StringEmpty()
    {
      var recordObject = new RecordClass() { Id = 1, Name = "Hello World" };
      var emptyString = recordObject.GetValue("Name.Empty").ToString();

      Assert.Equal(string.Empty, emptyString);

    }

    [Fact]
    public void TestPropertyWalk_SetInt()
    {
      var recordObject = new RecordClass() { Id = 1, Name = "Hello World" };
      recordObject.SetValue("Id", 2);

      Assert.Equal(2, recordObject.Id);

    }
  }
}