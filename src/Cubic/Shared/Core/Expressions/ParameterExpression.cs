using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Expressions
{
  public class ParameterExpression : Expression
  {
    public string Name
    {
      get;
      set;
    }

    public IExpression Value
    {
      get;
      set;
    }

    public Type Type { get; set; }

    public override ExpressionType NodeType
    {
      get
      {
        return ExpressionType.Parameter;
      }
    }

    public ParameterExpression()
    {
    }

    public ParameterExpression(string name, IExpression value = null) : this(name, typeof(string), value)
    {
    }

    public ParameterExpression(string name, Type type, IExpression value = null)
    {
      this.Name = name;
      this.Type = type;
      this.Value = value;
    }

    public override IExpression Clone()
    {
      return new ParameterExpression(this.Name, this.Type, this.Value);
    }
  }
}
