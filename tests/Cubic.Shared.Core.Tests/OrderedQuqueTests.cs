using Cubic.Core.Collections;
using Xunit;

namespace Cubic.Basics.Testing
{
  
  public class OrderedQuqueTests
  {
      
    [Fact]
    public void OrderedQuequeTest()
    {
      var _ordered = new OrderedQueque<int , string>();
      _ordered.Enquue(0, "Test0");
      _ordered.Enquue(1, "Test1");
      _ordered.Enquue(2, "Test2");
      _ordered.Enquue(3, "Test3");

      var top = _ordered.Peek();

      Assert.Equal( "Test0", top.Value );
    }

    [Fact]
    public void OrderedQuequeUpdateTest()
    {
      var _ordered = new OrderedQueque<int , string>();
      _ordered.Enquue( 0 , "Test0" );
      _ordered.Enquue( 1 , "Test1" );
      _ordered.Enquue( 2 , "Test2" );
      _ordered.Enquue( 3 , "Test3" );

      var top = _ordered.Dequeue();
      Assert.Equal( "Test0" , top.Value );

      top = _ordered.Dequeue();
      Assert.Equal( "Test1" , top.Value );
    }

    [Fact]
    public void OrderedQuequeAddTest()
    {
      var _ordered = new OrderedQueque<int , string>();

      _ordered.Enquue( 2 , "Test2" );
      _ordered.Enquue( 9 , "Test9" );

      var top = _ordered.Peek();

      Assert.Equal( "Test2" , top.Value );

      _ordered.Enquue(0, "Test0");

      Assert.Equal( "Test0" , _ordered.Peek().Value );
    }
  }
}