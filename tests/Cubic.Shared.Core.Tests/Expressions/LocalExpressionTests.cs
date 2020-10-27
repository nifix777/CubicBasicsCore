using Cubic.Core.Expressions;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Tests.Expressions
{
  
  public class LocalExpressionTests
  {
    [Fact]
    public void Eval_Const()
    {
      var expected = 1;
      var expression = new LiteralExpression(expected, false);

      object result = null;
      var success = LocalVisitor.Eval(expression, out result);

      Assert.True(success);
      Assert.Equal(expected, result);
    }

    [Fact]
    public void Eval_Method()
    {
      var expected = 1;
      var type = typeof(int);
      var expression = new FunctionExpression(type.FullName, nameof(int.Parse), new ParameterExpression[] { new ParameterExpression("", new LiteralExpression("1")) });

      object result = null;
      var success = LocalVisitor.Eval(expression, out result);

      Assert.True(success);
      Assert.Equal(expected, result);
    }
  }
}
