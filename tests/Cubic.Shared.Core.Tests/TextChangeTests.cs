using Cubic.Core.Text;
using Xunit;

namespace Cubic.Basics.Testing
{
  using System.Diagnostics;
  using System.Linq;

  
  public class TextChangeTests
  {
    [Fact]
    public void ProcessTestChanges()
    {
      var oldText = "Hello World my Name is 001001010101";
      var newText = "Hello World my Name is 001001010101_";

      var diffs = DocumentFunctions.GetTextChanges(oldText, newText).Result;

      Assert.True(diffs.Any());

      foreach (var textChange in diffs)
      {
        Trace.WriteLine("######### DIFF #########");
        Trace.WriteLine(string.Format("Start:{0} End:{1} NewText:{2}", textChange.TextSegment.Offset, textChange.TextSegment.End, textChange.NewTextSegment.ToString()));
        Trace.WriteLine(string.Format("Indexes with differences: {0}", string.Join(",", textChange.ChangedIndexes)));
      }
    }
  }
}