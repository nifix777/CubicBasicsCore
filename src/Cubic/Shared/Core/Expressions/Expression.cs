using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Expressions
{
  [Serializable]
  public abstract class Expression : IExpression
  {
    public virtual ExpressionType NodeType => ExpressionType.Undefined;

    public abstract IExpression Clone();

    public bool Visit(IExpressionVisitor visitor)
    {
      return visitor.Visit(this);
    }

    protected static Expression Literal(long offset)
    {
      return new LiteralExpression(offset);
    }

    protected static Expression Field(string item)
    {
      return new FieldExpression(item);
    }
    protected static Expression Field(string item, string tableName)
    {
      return new FieldExpression(item, tableName);
    }
  }
}
