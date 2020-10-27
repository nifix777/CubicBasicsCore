using System;
using Cubic.Core.Text;
using Xunit;

namespace Cubic.Core.Tests.Text
{
  
  public class FixedString_Test
  {
    [Fact]
    public void ToString_Test()
    {
      var expected = "ABC";
      var fstring = new FixedString(expected);
      string result = fstring;
      Assert.Equal(expected, result);
    }

    [Fact]
    public void FromString_Test()
    {
      var expected = "ABC";
      FixedString fstring = expected;
      string result = fstring;
      Assert.Equal(expected, result);
    }

    [Fact]
    public void To_Long_Exception_Test()
    {
      var expected = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
      Assert.Throws<ArgumentOutOfRangeException>(() => new FixedString(expected));
    }
  }
}
