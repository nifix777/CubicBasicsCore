using System;
using Cubic.Core;
using Cubic.Core.Text;
using Xunit;

namespace Cubic.Basics.Testing
{
  
  public class ToSqlTest
  {
    [Fact]
    public void TestDateTime()
    {
      var date = DateTime.Now;
      var sql = date.ToSql();
      Assert.False(string.IsNullOrWhiteSpace(sql));
    }
  }
}