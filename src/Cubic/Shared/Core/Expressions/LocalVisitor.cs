using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Expressions
{
  public class LocalVisitor: IExpressionVisitor
  {

    public bool Visit(IExpression expression)
    {
      if (expression is null) return false;

      Result = null;
      ParameterStack.Clear();

      if(expression is LogicalOperationExpression logicalOperation)
      {
        this.Visit(logicalOperation.Left);
        this.Visit(logicalOperation.Right);
      }
      else if (expression is LiteralExpression literal)
      {
        return this.VisitLiteral(literal);
      }
      else if (expression is FieldExpression field)
      {
        return this.Visit(field);
      }
      else if (expression is ParameterExpression parameter)
      {
        return VisitParameter(parameter);
      }
      else if (expression is FunctionExpression function)
      {
        return VisitFunction(function);
      }

      return false;
    }

    private bool VisitLiteral(LiteralExpression literal)
    {
      Result = literal.Value;
      if (Result == null && !literal.AllowEmpty) return false;

      return true;
    }

    private bool VisitFunction(FunctionExpression function)
    {

      var type = Type.GetType(function.BaseName);
      if (type == null) return false;

      //foreach (var param in function.Parameters)
      //{
      //  this.Visit(param);
      //}

      var paramTypes = function.Parameters.Select(p => p.Type).ToArray();
      var target = type.GetMethod(function.Name, paramTypes);

      if (target == null) return false;

      var parameters = function.Parameters;
      object[] values = new object[function.Parameters.Count];

      for (int i = 0; i < parameters.Count; i++)
      {
        ParameterExpression para = parameters[i];
        if (this.Visit(para.Value))
        {
          values[i] = Result;
        }
      }

      Result = target.Invoke(Target, values);

      return true;
    }

    private bool VisitParameter(ParameterExpression parameter)
    {
      if (!ParameterStack.Contains(parameter))
      {
        ParameterStack.Add(parameter);
      }

      return true;
    }

    public object Result;

    private object Target;

    private List<ParameterExpression> ParameterStack = new List<ParameterExpression>();

    public static bool Eval(IExpression expression, out object result)
    {
      var visitor = new LocalVisitor();
      result = null;

      if(expression.Visit(visitor))
      {
        result = visitor.Result;
        return true;
      }

      return false;
    }
  }
}
