using System;

namespace Cubic.Core
{
  public static class Conditions
  {
    public static T BindConditionally<T>(Func<bool> conditionFunc, Func<T> trueExpression, Func<T> falseExpression)
    {
      if (conditionFunc())
      {
        return trueExpression();
      }
      else
      {
        return falseExpression();
      }
    }
  }
}