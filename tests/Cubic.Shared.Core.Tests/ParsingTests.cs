using System.Collections.Generic;
using System.Linq;
using Cubic.Core;
using Cubic.Core.Text;
using Xunit;

namespace Cubic.Basics.Testing
{
    
    public class ParsingTests
    {
      [Fact]
      public void TestBinarySearching()
      {
          var text = "Hello World";
          var search = "World";

          Assert.True(text.BinarySearch(search) != 0);
      }

      [Fact]
      public void TestRegexSearching()
      {
          var text = "Hello World";
          var search = "World";

          Assert.True(text.RegexSearchExits(search));
      }

      [Fact]
      public void TestFirstStringOcureSearching()
      {
          var text = "Hello World! Is Value=@value ?";
          var starting = "@";
          var expected = "value";
          var results = text.GetWordsStartingWith(starting).ToList();
          Assert.True(results.Contains(expected));
      }

    [Fact]
    public void TestVariableReplacing()
    {
      var text = "Hello World! Is {variable} ?";
      var variableValue = 100;
      IDictionary<string, object> vars = new Dictionary<string, object>();
      vars.Add( "variable", variableValue );
      var results = text.ReplaceVariables(vars);
      Assert.True( results.Contains( variableValue.ToString() ) );
    }
  }
}