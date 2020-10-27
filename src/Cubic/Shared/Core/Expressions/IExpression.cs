using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Expressions
{
  public interface IExpression
  {
    ExpressionType NodeType { get; }

    bool Visit(IExpressionVisitor visitor);

    IExpression Clone();
  }

  public enum ExpressionType
  {
    Undefined,
    Between,
    Equals,
    Field,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Literal,
    LogicalAnd,
    LogicalOr,
    NotEquals,
    Not,
    NotNull,
    Null,
    Parantheses,
    Like,
    Plus,
    Minus,
    Divide,
    Multiply,
    Function,
    Case,
    FieldAssignment,
    Order,
    In,
    Parameter
  }
}
