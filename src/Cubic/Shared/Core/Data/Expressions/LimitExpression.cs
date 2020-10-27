using Cubic.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Data.Expressions
{
  public class LimitExpression : Expression
  {
    public IExpression Count
    {
      get;
      set;
    }

    public bool HasValue
    {
      get
      {
        if (this.Offset != null)
        {
          return true;
        }
        return this.Count != null;
      }
    }

    public IExpression Offset
    {
      get;
      set;
    }

    public LimitExpression()
    {
    }

    public LimitExpression(IExpression offset, IExpression count)
    {
      this.Offset = offset;
      this.Count = count;
    }

    public override IExpression Clone()
    {
      return new LimitExpression(Offset, Count);
    }
  }
}
