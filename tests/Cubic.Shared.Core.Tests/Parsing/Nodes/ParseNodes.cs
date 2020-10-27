//using System;
//using System.Linq;
//using System.Text;
//using Cubic.Core.Collections;
//using Cubic.Core.Text.Nodes;
//using Xunit;

//namespace Cubic.Basics.Testing.Parsing.Nodes
//{
//  
//  public class ParseNodes
//  {
//    [Fact]
//    public void ParseNodesTest()
//    {
//      var file = @".\Parsing\Nodes\NodeTest.txt";

//      var pool = new ArrayPool<char>(defaultBufferSize: 64);

//      using (var provider = new PieceTableProvider(file, pool))
//      {
//        var table = provider.Read(Encoding.Default);

//        Assert.NotNull(table);
//        Assert.NotNull(table.BufferNodes);

//        var sumLines = table.BufferNodes.WhereNotNull().SelectMany(b => b.LineStarts).Count();

//        Assert.Equal(3, sumLines);

//      }
//    }
//  }
//}
