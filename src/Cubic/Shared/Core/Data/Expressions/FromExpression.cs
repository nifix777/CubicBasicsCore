using Cubic.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Data.Expressions
{
  [Serializable]
  public abstract class FromExpression : Expression
  {

    public static JoinExpression RightJoin()
    {
      return new JoinExpression(JoinOperator.Right);
    }

    public static JoinExpression InnerJoin()
    {
      return new JoinExpression(JoinOperator.Inner);
    }

    public static JoinExpression LeftJoin()
    {
      return new JoinExpression(JoinOperator.Left);
    }

    public static TableExpression Table(string name)
    {
      return new TableExpression(name);
    }

    public static TableExpression Table(string name, string alias)
    {
      return new TableExpression(name, alias);
    }

  }
}
