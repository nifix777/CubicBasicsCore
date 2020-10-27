using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Cubic.Core.Threading;
using Xunit;

namespace Cubic.Basics.Testing
{
  
  public class ThreadingTests
  {
    [Fact]
    public void JoinableThreadTest()
    {
      var joinableThread = JoinableThreadContext.Instance;

      var onOtherThread = Task.Factory.StartNew( async () =>
      {
        Thread.Sleep(100);
        Assert.False(joinableThread.IsOnMain);

        await joinableThread.SwitchToMainThreadAsync();
        Assert.True(joinableThread.IsOnMain);

      });

      onOtherThread.Wait();

    }


    
  }
}
