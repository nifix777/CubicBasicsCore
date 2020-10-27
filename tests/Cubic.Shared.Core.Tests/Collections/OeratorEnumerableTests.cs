using System;
using System.Linq;
using Cubic.Core.Collections;
using Xunit;

namespace Cubic.Basics.Testing.Collections
{
  
  public class OeratorEnumerableTests
  {
    [Fact]
    public void Test_Ctor()
    {
      var source = new int[] { 1, 2, 3 };
      var operand = new OperatorEnumerable<int>(source);

      Assert.Contains(1, operand);
    }

    [Fact]
    public void Test_Addition()
    {
      var source = new int[] { 1, 2, 3 };
      var operand = new OperatorEnumerable<int>(source) + 4;


      Assert.Contains(4, operand);
    }

    [Fact]
    public void Test_Substract()
    {
      var source = new int[] { 1, 2, 3 };
      var operand = new OperatorEnumerable<int>(source) - 3;

      Assert.False(operand.Contains(3));
    }

    [Fact]
    public void Test_Batch()
    {
      var source = new int[] { 1, 2, 3 };
      var operand = new OperatorEnumerable<int>(source) / 1;

      Assert.True(operand.Count() == 3);
    }

    [Fact]
    public void Test_True()
    {
      var source = new int[] { 1, 2, 3 };
      var operand = new OperatorEnumerable<int>(source);

      Assert.True(operand);
    }

    [Fact]
    public void Test_False()
    {
      var source = new int[] { };
      var operand = new OperatorEnumerable<int>(source);

      Assert.False(operand);
    }

    //[Fact]
    //public void Test_ShiftLeft()
    //{
    //  var source = new int[] { 1, 2, 3 };
    //  var operand = (new OperatorEnumerable<int>(source) << 1).ToList();

    //  Assert.False(operand.First() != 1 && operand.Last() == 1 );
    //}

    //[Fact]
    //public void Test_ShiftRight()
    //{
    //  var source = new int[] { 1, 2, 3 };
    //  var operand = new OperatorEnumerable<int>(source) << 1;

    //  Assert.False(operand.Contains(2));
    //}
  }
}
