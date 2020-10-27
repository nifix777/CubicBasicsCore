namespace Cubic.Core.Expressions
{
  public interface IExpressionVisitor
  {
    bool Visit(IExpression expression);
  }
}