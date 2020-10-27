using Cubic.Core.Collections;
using Xunit;

namespace Cubic.Basics.Testing
{
  
  public class ArrayResizeTest
  {
    [Fact]
    public void NeedsResizeTest()
    {
      var array = new int[3];
      var arrayIndex = 3;

      Assert.True(array.NeedsResize(arrayIndex));

      arrayIndex = 2;
      Assert.False(array.NeedsResize(arrayIndex));
    }

    [Fact]
    public void EnsureIndexTest()
    {
      var array = new int[3];
      var arrayIndex = 3;

      ArrayExtensions.EnsureResize(ref array ,arrayIndex, 10);
      
      Assert.Equal(10, array.Length);
    }
  }
}