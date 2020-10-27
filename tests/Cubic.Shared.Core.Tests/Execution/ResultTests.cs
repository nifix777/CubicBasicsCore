using System;
using Cubic.Core.Execution;
using Xunit;

namespace Cubic.Basics.Testing.Execution
{
  
  public class ResultTests
  {
    #region Status
    [Fact]
    public void StatusOk()
    {
      var status = ResultHelper.Ok();

      Assert.True(status);
    }

    [Fact]
    public void StatusError()
    {
      var status = ResultHelper.Error();

      Assert.False(status);
    }

    [Fact]
    public void StatusErrorConversion()
    {
      var status = ResultHelper.Error<int>(0);

      Assert.False(status);
      Assert.Equal(0, status.Error);
    }
    #endregion

    #region Result
    [Fact]
    public void ResultOk()
    {
      var status = ResultHelper.Ok(1);

      Assert.True(status);
      Assert.Equal(1, status.Value);
    }

    [Fact]
    public void ResultError()
    {
      var status = ResultHelper.Error(0);

      Assert.False(status);
      Assert.Equal(0, status.Error);
    } 
    #endregion
  }

}
