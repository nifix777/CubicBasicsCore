using Cubic.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Data.Expressions
{
  public class SelectExpression : SqlExpression
  {
    public List<Expression> Fields
    {
      get;
      set;
    }

    public LimitExpression LimitExpression
    {
      get;
      set;
    }

    public SortExpression OrderByExpression
    {
      get;
      set;
    }

    public LogicalOperationExpression WhereExpression
    {
      get;
      set;
    }

    public SelectExpression()
    {
      this.Fields = new List<Expression>();
      this.LimitExpression = new LimitExpression();
    }

    public static SelectExpression From(FromExpression Expression)
    {
      return new SelectExpression()
      {
        FromExpression = Expression
      };
    }

    public static SelectExpression From(string tableName)
    {
      return new SelectExpression()
      {
        FromExpression = FromExpression.Table(tableName)
      };
    }

    public static SelectExpression From(string tableName, string alias)
    {
      SelectExpression SelectExpression = new SelectExpression()
      {
        FromExpression = FromExpression.Table(tableName, alias)
      };
      return SelectExpression;
    }

    public SelectExpression Limit(long offset, int count)
    {
      this.LimitExpression.Offset = Expression.Literal(offset);
      this.LimitExpression.Count = Expression.Literal(count);
      return this;
    }

    public SelectExpression Limit(ParameterExpression offset, ParameterExpression count)
    {
      this.LimitExpression.Offset = offset;
      this.LimitExpression.Count = count;
      return this;
    }

    public SelectExpression Limit(string offsetParameterName, string countParameterName)
    {
      this.LimitExpression.Offset = new ParameterExpression(offsetParameterName, typeof(long));
      this.LimitExpression.Count = new ParameterExpression(countParameterName, typeof(long));
      return this;
    }

    public SelectExpression OrderBy(SortExpression sortOrder)
    {
      this.OrderByExpression = sortOrder;
      return this;
    }

    public SelectExpression Select(params string[] fieldNames)
    {
      this.Fields.AddRange(
          from item in fieldNames
          select Expression.Field(item));
      return this;
    }

    public SelectExpression Select(params Expression[] fields)
    {
      this.Fields.AddRange(fields);
      return this;
    }

    public SelectExpression SelectAll()
    {
      this.Fields.Add(Expression.Field("*"));
      return this;
    }

    public SelectExpression SelectAll(string tableName)
    {
      this.Fields.Add(Expression.Field("*", tableName));
      return this;
    }

    public SelectExpression Where(LogicalOperationExpression restrictions)
    {
      this.WhereExpression = restrictions;
      return this;
    }

    public override IExpression Clone()
    {
      return new SelectExpression() { LimitExpression = (LimitExpression)LimitExpression.Clone()}.Select(Fields.ToArray()).Where(WhereExpression);
    }
  }
}
