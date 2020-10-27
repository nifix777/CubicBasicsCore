using Cubic.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Data.Expressions
{
  public abstract class SqlExpression : Expression
  {
    public FromExpression FromExpression
    {
      get;
      set;
    }

    protected SqlExpression()
    {
    }
  }
}
