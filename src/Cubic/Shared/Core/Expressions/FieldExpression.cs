using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Expressions
{
  public class FieldExpression : Expression
  {

    public string Alias
    {
      get;
      set;
    }

    public LiteralExpression LiteralExpression
    {
      get;
      set;
    }

    public string Name
    {
      get;
      set;
    }

    public override ExpressionType NodeType
    {
      get
      {
        return ExpressionType.Field;
      }
    }

    public string Resource
    {
      get;
      set;
    }

    private FieldExpression()
    {
    }

    private FieldExpression(string name, string resource, string alias, LiteralExpression literal)
    {
      this.Name = name;
      this.Resource = resource;
      this.Alias = alias;
      this.LiteralExpression = literal;
    }

    public FieldExpression(string name) : this(name, null, null, null)
    {
    }

    public FieldExpression(string name, string resource) : this(name, resource, null, null)
    {
    }

    public FieldExpression(string name, string resource, string alias) : this(name, resource, alias, null)
    {
    }

    public FieldExpression(LiteralExpression literal, string alias) : this(null, null, alias, literal)
    {
    }

    public override IExpression Clone()
    {
      return new FieldExpression(Name, Resource, Alias, LiteralExpression);
    }
  }
}
