using System.Diagnostics;
using System.Linq;
using Cubic.Core.Text;
using Xunit;

namespace Cubic.Basics.Testing
{
  
  public class StringSegmentTests
  {
    [Fact]
    public void StringSegmentTest()
    {
      var source = "Hello World";
      var expected = "Hello";
      var segment = source.Segment(0, 5);

      Trace.WriteLine( string.Format( "Segment:{0} String:{1}" , segment , expected ) );

      Assert.True(segment.Equals("Hello"));


    }

    [Fact]
    public void StringSegmentRevertTest()
    {
      var source = "Hello World";
      var expected = "World";
      var segment = source.Segment( 6 , 5 );

      Trace.WriteLine(string.Format("Segment:{0} String:{1}", segment, expected));

      Assert.True(segment.Equals(expected) );


    }


    [Fact]
    public void TestStringSegmentTrimTest()
    {
      var source = " Hello World ";

      var segment = source.TrimToSegment();

      Assert.Equal( source.Length - 2 , segment.Length );
    }

    [Fact]
    public void TestStringSegmentSplitTest()
    {
      var source = "Hello World";

      var segments = source.SplitInSegments(' ').ToList();

      Assert.Equal(2, segments.Count);

    }

    [Fact]
    public void TestStringSegmentSplit2Test()
    {
      var source = "Hello;World;Iam;AString";

      var segments = source.SplitInSegments( ';' ).ToList();

      Assert.Equal( 4 , segments.Count );


    }

  }
}