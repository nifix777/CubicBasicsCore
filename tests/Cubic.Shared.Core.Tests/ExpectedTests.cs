using System;
using Cubic.Core;
using Cubic.Core.Runtime;
using Xunit;

namespace Cubic.Basics.Testing
{
  
  public class ExpectedTests
  {
    [Fact]
    public void TestReference()
    {
      var reference = "Hello World";

      var expected = new Expected<string>(reference);

      Assert.NotNull(expected);
      Assert.True(expected.HasValue);
      Assert.NotNull(expected.Value);
      Assert.Null(expected.Error);
    }

    [Fact]
    public void TestReferenceNull()
    {
      string reference = null;

      var expected = new Expected<string>(reference);

      Assert.NotNull(expected);
      Assert.False(expected.HasValue);
      Assert.Null(expected.Value);
      Assert.Null(expected.Error);
    }

    [Fact]
    public void TestValue()
    {
      var value = 0;

      var expected = new Expected<int>(value);

      Assert.NotNull(expected);
      Assert.True(expected.HasValue);
      Assert.NotNull(expected.Value);
      Assert.Null(expected.Error);
    }

    [Fact]
    public void TestValueNull()
    {
      int? value = null;

      var expected = new Expected<int>(value);

      Assert.NotNull(expected);
      Assert.False(expected.HasValue);
      Assert.Null(expected.Error);
    }


    [Fact]
    public void TestException()
    {
      var error = new Exception("Dummy");

      var expected = new Expected<int>(error);

      Assert.NotNull(expected);
      Assert.False(expected.HasValue);
      Assert.NotNull(expected.Error);
    }
  }
}