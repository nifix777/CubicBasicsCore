using System;
using Cubic.Core.Text;
using Xunit;

namespace Cubic.Shared.Core.Tests.Text
{
  public class String_Quote_Test
  {
    [Fact]
    public void Escape_none_Test()
    {
      var input = "abc";
      var expected = "\"abc\"";
      var output = input.Quote();
      Assert.Equal(expected, output);
    }

    [Fact]
    public void Escape_simple_Test()
    {
      var input = "ab\"c";
      var expected = "\"ab\\\"c\"";
      var output = input.Quote();
      Assert.Equal(expected, output);
    }
  }
}
