using Cubic.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Expressions
{
  public interface IExpressionEvaluator
  {
    object Eval(IExpression expression, IDictionary<string, object> parameters = null);
    T Eval<T>(IExpression expression, IDictionary<string, object> parameters = null);
  }
}
