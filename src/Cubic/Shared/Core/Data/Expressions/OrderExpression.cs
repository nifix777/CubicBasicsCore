using Cubic.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Data.Expressions
{
  public class OrderExpression : Expression
  {
    public bool Descending
    {
      get;
      set;
    }

    public FieldExpression OrderField
    {
      get;
      set;
    }

    public OrderExpression()
    {
    }

    public OrderExpression(FieldExpression field) : this(field, false)
    {
    }

    public OrderExpression(FieldExpression field, bool descending)
    {
      this.OrderField = field;
      this.Descending = descending;
    }

    public override IExpression Clone()
    {
      FieldExpression fieldExpression;
      if (this.OrderField == null)
      {
        fieldExpression = null;
      }
      else
      {
        fieldExpression = (FieldExpression)this.OrderField.Clone();
      }
      return new OrderExpression(fieldExpression, this.Descending);
    }
  }
}
