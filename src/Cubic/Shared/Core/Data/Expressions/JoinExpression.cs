using Cubic.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Data.Expressions
{
  public class JoinExpression : FromExpression
  {
    public Expression Condition
    {
      get;
      internal set;
    }

    public FromExpression Left
    {
      get;
      internal set;
    }

    public JoinOperator Operator
    {
      get;
      internal set;
    }

    public FromExpression Right
    {
      get;
      internal set;
    }

    private JoinExpression()
    {
    }

    internal JoinExpression(JoinOperator joinOperator)
    {
      this.Operator = joinOperator;
    }

    public JoinExpression InnerJoin(string tableName, string alias)
    {
      return FromExpression.InnerJoin().SetLeft(this).SetRight(tableName, alias);
    }

    public JoinExpression InnerJoin(TableExpression table)
    {
      return FromExpression.InnerJoin().SetLeft(this).SetRight(table);
    }

    public JoinExpression InnerJoin(JoinExpression join)
    {
      return FromExpression.InnerJoin().SetLeft(this).SetRight(join);
    }

    public JoinExpression LeftJoin(string tableName, string alias)
    {
      return FromExpression.LeftJoin().SetLeft(this).SetRight(tableName, alias);
    }

    public JoinExpression LeftJoin(TableExpression table)
    {
      return FromExpression.LeftJoin().SetLeft(this).SetRight(table);
    }

    public JoinExpression LeftJoin(JoinExpression join)
    {
      return FromExpression.LeftJoin().SetLeft(this).SetRight(join);
    }

    public JoinExpression On(Expression condition)
    {
      this.Condition = condition;
      return this;
    }

    public JoinExpression RightJoin(string tableName, string alias)
    {
      return FromExpression.RightJoin().SetLeft(this).SetRight(tableName, alias);
    }

    public JoinExpression RightJoin(TableExpression table)
    {
      return FromExpression.RightJoin().SetLeft(this).SetRight(table);
    }

    public JoinExpression RightJoin(JoinExpression join)
    {
      return FromExpression.RightJoin().SetLeft(this).SetRight(join);
    }

    public JoinExpression SetLeft(string tableName)
    {
      this.Left = FromExpression.Table(tableName);
      return this;
    }

    public JoinExpression SetLeft(string tableName, string alias)
    {
      this.Left = FromExpression.Table(tableName, alias);
      return this;
    }

    public JoinExpression SetLeft(TableExpression table)
    {
      this.Left = table;
      return this;
    }

    public JoinExpression SetLeft(JoinExpression join)
    {
      this.Left = join;
      return this;
    }

    public JoinExpression SetLeft(FromExpression fromExpression)
    {
      this.Left = fromExpression;
      return this;
    }

    public JoinExpression SetRight(string tableName)
    {
      this.Right = FromExpression.Table(tableName);
      return this;
    }

    public JoinExpression SetRight(string tableName, string alias)
    {
      this.Right = FromExpression.Table(tableName, alias);
      return this;
    }

    public JoinExpression SetRight(TableExpression table)
    {
      this.Right = table;
      return this;
    }

    public JoinExpression SetRight(JoinExpression join)
    {
      this.Right = join;
      return this;
    }

    public JoinExpression SetRight(FromExpression fromExpression)
    {
      this.Right = fromExpression;
      return this;
    }

    public override IExpression Clone()
    {
      return new JoinExpression(Operator).SetLeft(Left).SetRight(Right).On(Condition);
    }
  }
}
