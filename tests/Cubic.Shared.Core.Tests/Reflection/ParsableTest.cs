using System;
using Cubic.Core;
using Xunit;

namespace Cubic.Basics.Testing.Reflection
{
  
  public class ParsableTest
  {
    [Fact]
    public void Test_Int_ToString()
    {
      var value = 64;
      var format = "X";
      var result = Parsable<int>.ToString(value, format);
      Assert.Equal(value.ToString(format), result);
    }

    [Fact]
    public void Test_Int_Parse()
    {
      var value = "64";
      var result = Parsable<int>.Parse(value);
      Assert.Equal(int.Parse(value), result);
    }
  }
}
