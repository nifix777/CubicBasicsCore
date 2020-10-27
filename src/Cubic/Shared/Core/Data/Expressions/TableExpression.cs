using Cubic.Core.Expressions;
using System;

namespace Cubic.Core.Data.Expressions
{
  [Serializable]
  public class TableExpression : FromExpression
  {
    public string Alias
    {
      get;
      set;
    }

    public string Name
    {
      get;
      set;
    }

    private TableExpression()
    {
    }

    public TableExpression(string name)
    {
      this.Name = name;
    }

    public TableExpression(string name, string alias)
    {
      this.Name = name;
      this.Alias = alias;
    }

    public TableExpression As(string alias)
    {
      this.Alias = alias;
      return this;
    }

    public override IExpression Clone()
    {
      return new TableExpression(Name, Alias);
    }
  }
}
