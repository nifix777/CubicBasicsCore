using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Expressions
{
  public class FunctionExpression : MemberExpression
  {

    public FunctionExpression(string baseName, string functionName, Type returnType, ParameterExpression[] parameters = null) : base(baseName, functionName)
    {
      if (parameters != null) Parameters = new List<ParameterExpression>(parameters);
      ReturnType = returnType;
    }

    public FunctionExpression(string baseName, string functionName, ParameterExpression[] parameters = null) : base(baseName, functionName)
    {
      if (parameters != null) Parameters = new List<ParameterExpression>(parameters);
      ReturnType = typeof(string);
    }

    public override ExpressionType NodeType => ExpressionType.Function;

    public Type ReturnType { get; }

    public IList<ParameterExpression> Parameters { get; } = new List<ParameterExpression>();

    public override IExpression Clone()
    {
      return new FunctionExpression(BaseName, Name, Parameters.ToArray());
    }
  }
}
