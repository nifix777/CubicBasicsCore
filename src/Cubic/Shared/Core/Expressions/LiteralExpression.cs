using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Expressions
{
  public class LiteralExpression : Expression
  {

    public bool AllowEmpty
    {
      get;
      set;
    }

    public override ExpressionType NodeType
    {
      get
      {
        return ExpressionType.Literal;
      }
    }

    public object Value
    {
      get;
      set;
    }

    public Type ValueType
    {
      get;
      set;
    }

    public LiteralExpression()
    {
    }

    public LiteralExpression(object value) : this(value, false)
    {
    }

    public LiteralExpression(object value, bool allowEmpty)
    {
      this.Value = value;
      this.AllowEmpty = allowEmpty;
      if (value == null)
      {
        // TODO: Nullable ??
        this.ValueType = typeof(object);
        return;
      }
      this.ValueType = value.GetType();
    }

    public override IExpression Clone()
    {
      return new LiteralExpression(Value, AllowEmpty);
    }
  }
}
