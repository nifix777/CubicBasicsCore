using Cubic.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubic.Core.Data.Expressions
{
  public class SqlDeleteStatement : SqlExpression
  {
    public LogicalOperationExpression WhereExpression
    {
      get;
      set;
    }

    public SqlDeleteStatement()
    {
    }

    public static SqlDeleteStatement From(string tableName)
    {
      return new SqlDeleteStatement()
      {
        FromExpression = FromExpression.Table(tableName)
      };
    }

    public static SqlDeleteStatement From(string tableName, string alias)
    {
      SqlDeleteStatement sqlDeleteStatement = new SqlDeleteStatement()
      {
        FromExpression = FromExpression.Table(tableName, alias)
      };
      return sqlDeleteStatement;
    }

    //public static SqlDeleteStatement From(ITableInfo tableInfo)
    //{
    //  SqlDeleteStatement sqlDeleteStatement = new SqlDeleteStatement()
    //  {
    //    FromExpression = FromExpression.Table(tableInfo.Name)
    //  };
    //  return sqlDeleteStatement;
    //}

    //public static SqlDeleteStatement From(ITableInfo tableInfo, string alias)
    //{
    //  SqlDeleteStatement sqlDeleteStatement = new SqlDeleteStatement()
    //  {
    //    FromExpression = FromExpression.Table(tableInfo.Name, alias)
    //  };
    //  return sqlDeleteStatement;
    //}

    public SqlDeleteStatement Where(LogicalOperationExpression restrictions)
    {
      this.WhereExpression = restrictions;
      return this;
    }

    public override IExpression Clone()
    {
      var clone = new SqlDeleteStatement();
      clone.FromExpression = (FromExpression)this.FromExpression.Clone();
      clone.WhereExpression = (LogicalOperationExpression) this.WhereExpression.Clone();

      return clone;
    }
  }
}
