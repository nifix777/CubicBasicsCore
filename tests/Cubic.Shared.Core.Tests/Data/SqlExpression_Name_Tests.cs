using Cubic.Shared.Core.Data.Expressions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cubic.Shared.Core.Tests.Data
{
  public class SqlExpression_Name_Tests
  {
    [Fact]
    public void Test_Simplename()
    {
      var name = "table.property";
      var sqlName = new SqlNameExpression(name);

      Assert.Equal("property", sqlName.Property);
      Assert.Equal("table", sqlName.Collection);
    }


    [Fact]
    public void Test_SynonymName()
    {
      var name = "server.dbo.table.property";
      var sqlName = new SqlNameExpression(name);

      Assert.Equal("property", sqlName.Property);
      Assert.Equal("table", sqlName.Collection);
      Assert.Equal("dbo", sqlName.Scheme);
      Assert.Equal("server", sqlName.Synonym);
    }

    [Fact]
    public void Test_SynonymName_Modify()
    {
      var name = "server.dbo.table.property";
      var sqlName = new SqlNameExpression(name);

      sqlName.Scheme = "dbreader";

      Assert.Equal("property", sqlName.Property);
      Assert.Equal("table", sqlName.Collection);
      Assert.Equal("dbreader", sqlName.Scheme);
      Assert.Equal("server", sqlName.Synonym);
    }

    [Fact]
    public void Test_SynonymName_Append()
    {
      var name = "table.property";
      var sqlName = new SqlNameExpression(name);

      sqlName.Scheme = "dbo";

      Assert.Equal("property", sqlName.Property);
      Assert.Equal("table", sqlName.Collection);
      Assert.Equal("dbo", sqlName.Scheme);
    }
  }
}
