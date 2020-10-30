using Cubic.Core.Components;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cubic.Shared.Core.Tests
{
  public class ServiceProviderTest
  {
    [Fact]
    public void Check_Dependency()
    {
      var services = new ServiceCollection();
      services.AddSingleton<TestDependency>();
      services.AddScoped<IMyService, MyService>();

      var sp = new ServiceProvider(new ServiceProviderEngine(services));
      var mySvc = sp.GetService<IMyService>();
      Assert.NotNull(mySvc);
    }
  }


  interface IMyService
  {

  }

  class MyService : IMyService
  {
    private TestDependency dp;

    public MyService(TestDependency dp, string hasDefault = default)
    {
      Assert.NotNull(dp);
      //this.dp = dp ?? throw new ArgumentNullException(nameof(dp));
      Assert.True(hasDefault == default);
    }
  }
  class TestDependency
  {

  }
}
