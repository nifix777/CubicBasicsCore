using System.Collections.Generic;
using System.Diagnostics;
using Cubic.Core.Text;
using Xunit;

namespace Cubic.Basics.Testing
{
  
  public class StringParsingTests
  {
    [Fact]
    public void StringPlaceholderTest()
    {
      var template = "Hello {Name} World!";

      var placeholders = new Dictionary<string, object>() { {"NAME", "Me"} };

      var output = template.FormatEx(placeholders);

      Trace.WriteLine(output);
      Assert.True(output.Contains("Me"));
      Assert.False(output.Contains("{Name}"));
    }
  }
}