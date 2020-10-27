using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Expressions
{
  public class LogicalOperationExpression : Expression
  {
    public LogicalOperationExpression(Expression left, LogicalOperation operation, Expression right)
    {
      Left = left;
      Operation = operation;
      Right = right;
    }

    public LogicalOperation Operation { get; }

    public Expression Left { get; }

    public Expression Right { get; }

    public virtual bool IsEmpty => (Left != null && Right != null);

    public override IExpression Clone()
    {
      return new LogicalOperationExpression(Left, Operation, Right);
    }
  }

  public enum LogicalOperation
  {
    Equals = 1,
    NotEquals = 2,
    GreaterThen = 3,
    LessThen = 4,
    GreaterOrEqals = 5,
    LessOrEqals = 6,
    Like = 7,
    NotLike = 8,
    Between = 9,
    NotBetween = 10
  }
}
