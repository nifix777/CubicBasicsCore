using Cubic.Core.Collections;
using Cubic.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Data.Expressions
{
  public class SortExpression : Expression
  {
    private readonly IList<OrderExpression> _orderExpressions = new List<OrderExpression>();

    public bool IsEmpty
    {
      get
      {
        return this._orderExpressions.Count == 0;
      }
    }

    public IList<OrderExpression> OrderExpressions
    {
      get
      {
        return this._orderExpressions;
      }
    }

    public SortExpression()
    {
    }

    public SortExpression Add(OrderExpression expression)
    {
      this._orderExpressions.Add(expression);
      return this;
    }

    private SortExpression AddRange(IEnumerable<OrderExpression> expressions)
    {
      this._orderExpressions.AddRange(expressions);
      return this;
    }

    public SortExpression AddAscending(FieldExpression expression)
    {
      this._orderExpressions.Add(new OrderExpression(expression, false));
      return this;
    }

    public SortExpression AddAscending(string name)
    {
      this._orderExpressions.Add(new OrderExpression(new FieldExpression(name), false));
      return this;
    }

    public SortExpression AddDescending(FieldExpression expression)
    {
      this._orderExpressions.Add(new OrderExpression(expression, true));
      return this;
    }

    public SortExpression AddDescending(string name)
    {
      this._orderExpressions.Add(new OrderExpression(new FieldExpression(name), true));
      return this;
    }

    public override IExpression Clone()
    {
      return new SortExpression().AddRange(_orderExpressions);
    }
  }
}
